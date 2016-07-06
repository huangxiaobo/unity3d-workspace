using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class RenderShadowVolume : MonoBehaviour {
	public MeshFilter[] objects;
	public Light shadowLight;
	public float extrusionDistance = 20.0f;
	public Shader extrusionShader;
	public Shader volumeAlphaShader;

	private Material extrusionMat;
	private Material setAlphaMat;

	void Start() {
		if (!shadowLight) {
			Debug.LogWarning ("no shadow casting light set, disabling script");
			enabled = false;
		}
		if (!extrusionShader) {
			Debug.LogWarning ("no shadow casting shader set, disabling script");
			enabled = false;
		}
		if (!GetComponent<Camera>()) {
			Debug.LogWarning ("script must be attached to camera, disabling script");
			enabled = false;
		}
	}

	void OnPostRender() {
		if (!enabled)
			return;
			
		if (!setAlphaMat) {
			setAlphaMat = new Material (volumeAlphaShader);
			setAlphaMat.shader.hideFlags = HideFlags.HideAndDontSave;
			setAlphaMat.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!extrusionMat) {
			extrusionMat = new Material (extrusionShader);
			extrusionMat.hideFlags = HideFlags.HideAndDontSave;
		}
		
		// clear screen's alpha to 1/4
		GL.PushMatrix ();
		GL.LoadOrtho ();
		setAlphaMat.SetPass (0);
		DrawQuad();
		GL.PopMatrix ();

		// setup extrusion shader properties
		extrusionMat.SetFloat ("_Extrusion", extrusionDistance);
		Vector4 lightPos;
		if (shadowLight.type == LightType.Directional) {
			Vector4 dir = -shadowLight.transform.forward;
			dir = transform.InverseTransformDirection(dir);
			lightPos = new Vector4(dir.x,dir.y,-dir.z,0.0f);
		} else {
			Vector4 pos = shadowLight.transform.position;
			pos = transform.InverseTransformPoint(pos);
			lightPos = new Vector4(pos.x,pos.y,-pos.z,1.0f);
		}
		extrusionMat.SetVector("_LightPosition", lightPos);
	
		// render shadow volumes of all objects
		foreach(MeshFilter filter in objects ) {
			Mesh m = filter.sharedMesh;
			Transform tr = filter.transform;
			extrusionMat.SetPass(0);
			Graphics.DrawMeshNow (m, tr.localToWorldMatrix);
			extrusionMat.SetPass(1);
			Graphics.DrawMeshNow (m, tr.localToWorldMatrix);
		}

		// normalize and apply shadow mask
		GL.PushMatrix ();
		GL.LoadOrtho ();
		setAlphaMat.SetPass (1);
		DrawQuad();
		setAlphaMat.SetPass (2);
		DrawQuad();
		setAlphaMat.SetPass (3);
		DrawQuad();
		setAlphaMat.SetPass (4);
		DrawQuad();
		GL.PopMatrix ();

	}
	private static void DrawQuad() {
		GL.Begin (GL.QUADS);
		GL.Vertex3 (1, 0, 0.1f);
		GL.Vertex3 (0, 0, 0.1f);
		GL.Vertex3 (1, 1, 0.1f);
		GL.Vertex3 (0, 1, 0.1f);
		GL.End ();
	}

}