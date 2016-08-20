using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GameObjectGlow : MonoBehaviour {
	private const string NODE = "Glow Camera";

	void OnEnable()
	{
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
	}

	private Camera glowCamera
	{
		get
		{
			if (null == m_GlowCamera)
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

				m_GlowCamera = node.GetComponent<Camera>();
				if (null == m_GlowCamera)
				{
					m_GlowCamera = node.gameObject.AddComponent<Camera>();
				}

				m_GlowCamera.enabled = false;
				m_GlowCamera.clearFlags = CameraClearFlags.SolidColor;
				m_GlowCamera.backgroundColor = new Color(0, 0, 0, 0);
				m_GlowCamera.renderingPath = RenderingPath.VertexLit;
				m_GlowCamera.hdr = false;
				m_GlowCamera.useOcclusionCulling = false;
				m_GlowCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
			}

			return m_GlowCamera;
		}
	}
	private Camera m_GlowCamera;
 
 	private Shader glowShader
	{
		get
		{
			if (m_GlowShader == null)
			{
				m_GlowShader = Shader.Find("ShaderLib/17/Glow");
			}

			return m_GlowShader;
		}
	}
	private Shader m_GlowShader = null;

	private Material glowMaterial
	{
	 get
	 {
		 if (m_GlowMaterial == null)
		 {
			 m_GlowMaterial = new Material(Shader.Find("ShaderLib/17/GlowEffect"));
			 m_GlowMaterial.hideFlags = HideFlags.HideAndDontSave;
		 }

		 return m_GlowMaterial;
	 }
	}
	private Material m_GlowMaterial = null;

	private Material blurMaterial
	{
	 get
	 {
		 if (m_BlurMaterial == null)
		 {
			 m_BlurMaterial = new Material(Shader.Find("ShaderLib/18/GaussianBlurEffect"));
			 m_BlurMaterial.hideFlags = HideFlags.HideAndDontSave;
		 }

		 return m_BlurMaterial;
	 }
	}
	private Material m_BlurMaterial = null;



	private RenderTexture glowMap;

	void OnPreRender()
	{
		glowMap = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);

		glowCamera.fieldOfView = GetComponent<Camera>().fieldOfView;
		glowCamera.orthographic = GetComponent<Camera>().orthographic;
		glowCamera.nearClipPlane = GetComponent<Camera>().nearClipPlane;
		glowCamera.farClipPlane = GetComponent<Camera>().farClipPlane;
		glowCamera.cullingMask = GetComponent<Camera>().cullingMask;
		glowCamera.targetTexture = glowMap;
		glowCamera.RenderWithShader(glowShader, "RenderType");

	}

	private void OnPostRender()
	{
		RenderTexture.ReleaseTemporary(glowMap);
	}
 
	[ImageEffectOpaque]
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		BlurRenderTexture(glowMap);
		glowMaterial.SetTexture("_GlowMap", glowMap);
		Graphics.Blit(source, destination, glowMaterial);
	}

	private int BlurSize = 2;
	void BlurRenderTexture(RenderTexture source)
	{
		// 如果模糊范围和材质存在
		// 得到纹理的长宽1/8的大小
		int rtW = source.width;
		int rtH = source.height;


		// 创建一个只有原来渲染屏幕1/8的纹理
		RenderTexture rtTempA = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
		rtTempA.filterMode = FilterMode.Bilinear;

		// 将源纹理绘制到临时纹理
		Graphics.Blit (source, rtTempA);

		// 垂直模糊->水平模糊->垂直模糊->水平模糊
		for(int i = 0; i < 2; i++){

			float iteraionOffs = i * 1.0f;
			// 设置材质的模糊半径
			blurMaterial.SetFloat("_blurSize", BlurSize + iteraionOffs);

			// 垂直模糊
			// 创建临时纹理
			RenderTexture rtTempB = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
			rtTempB.filterMode = FilterMode.Bilinear;
			// 将临时纹理A使用模糊材质渲染到临时纹理B，使用pass 0 （垂直模糊）
			Graphics.Blit (rtTempA, rtTempB, blurMaterial,0);
			// 释放临时纹理A
			RenderTexture.ReleaseTemporary(rtTempA);
			// 将纹理A设置为垂直模糊后的纹理
			rtTempA = rtTempB;

			// 水平模糊
			// 创建临时纹理B
			rtTempB = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
			rtTempB.filterMode = FilterMode.Bilinear;
			// 将临时纹理A使用模糊材质渲染到临时纹理B，使用pass 1 （水平模糊）
			Graphics.Blit (rtTempA, rtTempB, blurMaterial,1);
			RenderTexture.ReleaseTemporary(rtTempA);
			rtTempA = rtTempB;
		}
		// 将最终模糊后的纹理绘制到目标纹理
		Graphics.Blit(rtTempA, source);
		// 释放临时纹理A
		RenderTexture.ReleaseTemporary(rtTempA);
	}
}
