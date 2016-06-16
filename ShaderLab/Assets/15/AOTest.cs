using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
public class AOTest : MonoBehaviour 
{
	public Shader shaderAO;
	public Shader shaderAOBlur;
	public Shader shaderAOBlit;
	private Material materialAO;
	private Material materialAOBlur;
	private Material materialAOBlit;

	private Camera cameraAO;

	private Texture2D noiseTexture;
	public Texture2D randTexture;
	
	public float radius = 2.0f;
	public float projScale = 500.0f;
	public float bias = 0.1f;
	public float intensity = 1.0f;

	void Awake()
	{
		if(shaderAO && shaderAOBlur && shaderAOBlit)
		{
			materialAO = new Material(shaderAO);
			materialAO.hideFlags = HideFlags.HideAndDontSave;
			
			materialAOBlur = new Material(shaderAOBlur);
			materialAOBlur.hideFlags = HideFlags.HideAndDontSave;
			
			materialAOBlit = new Material(shaderAOBlit);
			materialAOBlit.hideFlags = HideFlags.HideAndDontSave;			
		}

		cameraAO = GetComponent<Camera>();
		cameraAO.depthTextureMode |= DepthTextureMode.Depth;
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	private void GenerateNoiseTexture(int width, int height)
	{
		if (noiseTexture)
		{
			//those the texture need to be regenerated?
			if (noiseTexture.width != width && noiseTexture.height != height)
			{
				Destroy(noiseTexture);
				noiseTexture = null;
			}
			else
			{
				return;
			}
		}

		Debug.Log("Generating noise texture: " + width + " : " + height);

		noiseTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
		Color[] pixelsColor = new Color[width * height];
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				float pixelValue = (float)((i ^ j) / ((float)Mathf.Max(width, height) * 2.0f));

				Vector4 kEncodeMul = new Vector4(1.0f, 255.0f, 65025.0f, 160581375.0f);
				float kEncodeBit = 1.0f / 255.0f;
				Vector4 enc = kEncodeMul * pixelValue;
				enc = enc - new Vector4(Mathf.Floor(enc.x), Mathf.Floor(enc.y), Mathf.Floor(enc.z), Mathf.Floor(enc.w));
				enc -= new Vector4(enc.y, enc.z, enc.w, enc.w) * kEncodeBit;

				pixelsColor[i * width + j] = new Color(enc.x, enc.y, enc.z, enc.w);
			}
		}

		noiseTexture.SetPixels(pixelsColor);
		noiseTexture.Apply();
}


	[ImageEffectOpaque]
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (!materialAO || !materialAOBlur || !materialAOBlit) 
		{
			Graphics.Blit(source, destination);
			return;
		}

		GenerateNoiseTexture(Mathf.Min(source.width, 2048), Mathf.Min(source.height, 2048));

		float cameraNear = cameraAO.nearClipPlane;
		float cameraFar = cameraAO.farClipPlane;

		Vector3 clipInfo = new Vector3(cameraNear * cameraFar,  cameraNear - cameraFar, cameraFar);

		Matrix4x4 projMatrix = cameraAO.projectionMatrix;
		//From the Unity doc for Matrix4x4: Data is accessed as: row + (column*4).
		//Doing the math using double than casting to float come from the paper source code, and is required for the SSAO to work correctly
		Vector4 projInfo = new Vector4
			((float)(-2.0 / (Screen.width * projMatrix[0])), 
			 (float)(-2.0 / (Screen.height * projMatrix[5])),
			 (float)((1.0 - (double)projMatrix[2]) / projMatrix[0]), 
			 (float)((1.0 + (double)projMatrix[6]) / projMatrix[5]));

		materialAO.SetVector ("_ClipInfo", clipInfo);
		materialAO.SetVector ("_ProjInfo", projInfo);
		materialAO.SetFloat("_NoiseScale", (float)Mathf.Max(noiseTexture.width, noiseTexture.height) * 2.0f);
		materialAO.SetTexture("_NoiseTexture", noiseTexture);
		materialAO.SetTexture("_RandTexture", randTexture);
		
		materialAO.SetFloat("_ProjScale", projScale);
		materialAO.SetFloat("_Radius", radius);
		materialAO.SetFloat("_Radius2", radius * radius);
		materialAO.SetFloat("_Bias", bias);
		materialAO.SetFloat("_IntensityDivR6", intensity / Mathf.Pow(radius, 6.0f));

			
		RenderTexture AOTextureTmp1 = RenderTexture.GetTemporary (source.width, source.height, 0, RenderTextureFormat.ARGBHalf);
		RenderTexture AOTextureTmp2 = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBHalf);

		Graphics.Blit(source, AOTextureTmp1, materialAO);
			
		materialAOBlur.SetVector("_Axis", new Vector2(1.0f,0.0f));
		Graphics.Blit(AOTextureTmp1, AOTextureTmp2, materialAOBlur);
		
		materialAOBlur.SetVector("_Axis", new Vector2(0.0f,1.0f));
		Graphics.Blit(AOTextureTmp2, AOTextureTmp1, materialAOBlur);

		materialAOBlit.SetTexture("_AOTexture", AOTextureTmp1);
		Graphics.Blit(source, destination, materialAOBlit);
			
		RenderTexture.ReleaseTemporary(AOTextureTmp1);
		RenderTexture.ReleaseTemporary(AOTextureTmp2);
		
	}
}
