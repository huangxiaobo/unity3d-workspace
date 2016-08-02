using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class OcclusionEffect : MonoBehaviour
{
	private const string NODE = "Occlusion Camera";

	[Range(0.0f, 10.0f)]
	public float intensity = 1.0f;
	public Vector4 tiling = new Vector4(1, 1, 0, 0);
	public Texture2D occlusionMap;
	public LayerMask cullingMask;

	private Camera occlusionCamera
	{
		get
		{
			if (null == m_OcclusionCamera)
			{
				Transform node = transform.FindChild(NODE);
				if (null == node)
				{
					node = new GameObject(NODE).transform;
					node.parent = transform;
					node.localPosition = Vector3.zero;
					node.localRotation = Quaternion.identity;
					node.localScale = Vector3.one;
				}

				m_OcclusionCamera = node.GetComponent<Camera>();
				if (null == m_OcclusionCamera)
				{
					m_OcclusionCamera = node.gameObject.AddComponent<Camera>();
				}

				m_OcclusionCamera.enabled = false;
				m_OcclusionCamera.clearFlags = CameraClearFlags.SolidColor;
				m_OcclusionCamera.backgroundColor = new Color(0, 0, 0, 0);
				m_OcclusionCamera.renderingPath = RenderingPath.VertexLit;
				m_OcclusionCamera.hdr = false;
				m_OcclusionCamera.useOcclusionCulling = false;
				m_OcclusionCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
			}

			return m_OcclusionCamera;
		}
	}
	private Camera m_OcclusionCamera;

	private Material occlusionMaterial
	{
		get
		{
			if (m_OcclusionMaterial == null)
			{
				m_OcclusionMaterial = new Material(Shader.Find("ShaderLib/22/Occlusion"));
				m_OcclusionMaterial.hideFlags = HideFlags.HideAndDontSave;
			}

			return m_OcclusionMaterial;
		}
	}
	private Material m_OcclusionMaterial = null;

	private RenderTexture depthMap;

	void Start()
	{
	}

	private void OnPreRender()
	{
		depthMap = RenderTexture.GetTemporary(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);

		GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;

		occlusionCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
		occlusionCamera.fieldOfView = GetComponent<Camera>().fieldOfView;
		occlusionCamera.orthographic = GetComponent<Camera>().orthographic;
		occlusionCamera.nearClipPlane = GetComponent<Camera>().nearClipPlane;
		occlusionCamera.farClipPlane = GetComponent<Camera>().farClipPlane;
		occlusionCamera.cullingMask = cullingMask; // 这个地方要只留下角色相关的图层，其余图层要剔除掉
		occlusionCamera.targetTexture = depthMap;
		occlusionCamera.RenderWithShader(Shader.Find("Hidden/Camera-DepthNormalTexture"), string.Empty);
	}

	private void OnPostRender()
	{
		RenderTexture.ReleaseTemporary(depthMap);
	}

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		occlusionMaterial.SetTexture("_DepthMap", depthMap);
		occlusionMaterial.SetTexture("_OcclusionMap", occlusionMap);
		occlusionMaterial.SetFloat("_Intensity", intensity);
		occlusionMaterial.SetVector("_Tiling", tiling);
		Graphics.Blit(sourceTexture, destTexture, occlusionMaterial);
	}

	private void OnDisable()
	{
		OnDestroy();
	}

	private void OnDestroy()
	{
		if (null != m_OcclusionCamera)
		{
			if (Application.isPlaying)
			{
				Destroy(m_OcclusionCamera.gameObject);
			}
			else
			{
				DestroyImmediate(m_OcclusionCamera.gameObject);
			}
		}
	}
}