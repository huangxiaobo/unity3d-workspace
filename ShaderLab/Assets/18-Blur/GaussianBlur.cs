using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class GaussianBlur : MonoBehaviour {
	// 制定模糊shader
	public Shader GaussianBlurShader;
	// 模糊材质
	private Material GaussianBlurMaterial;
	// 模糊半径
	public float BlurSize = 0.3f;

	void OnEnable()
	{
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
	}

	private void CreateMaterial()
	{
		// 创建材质
		if (!GaussianBlurShader || GaussianBlurMaterial)
			return;
		GaussianBlurMaterial = new Material(GaussianBlurShader) {hideFlags = HideFlags.HideAndDontSave};
	}

	[ImageEffectOpaque]
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CreateMaterial();

		if(BlurSize != 0 && GaussianBlurMaterial != null){
			// 如果模糊范围和材质存在
			// 得到纹理的长宽1/8的大小
			int rtW = source.width/8;
			int rtH = source.height/8;


			// 创建一个只有原来渲染屏幕1/8的纹理
			RenderTexture rtTempA = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
			rtTempA.filterMode = FilterMode.Bilinear;

			// 将源纹理绘制到临时纹理
			Graphics.Blit (source, rtTempA);

			// 垂直模糊->水平模糊->垂直模糊->水平模糊
			for(int i = 0; i < 2; i++){

				float iteraionOffs = i * 1.0f;
				// 设置材质的模糊半径
				GaussianBlurMaterial.SetFloat("_blurSize", BlurSize + iteraionOffs);

				// 垂直模糊
				// 创建临时纹理
				RenderTexture rtTempB = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
				rtTempB.filterMode = FilterMode.Bilinear;
				// 将临时纹理A使用模糊材质渲染到临时纹理B，使用pass 0 （垂直模糊）
				Graphics.Blit (rtTempA, rtTempB, GaussianBlurMaterial,0);
				// 释放临时纹理A
				RenderTexture.ReleaseTemporary(rtTempA);
				// 将纹理A设置为垂直模糊后的纹理
				rtTempA = rtTempB;

				// 水平模糊
				// 创建临时纹理B
				rtTempB = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
				rtTempB.filterMode = FilterMode.Bilinear;
				// 将临时纹理A使用模糊材质渲染到临时纹理B，使用pass 1 （水平模糊）
				Graphics.Blit (rtTempA, rtTempB, GaussianBlurMaterial,1);
				RenderTexture.ReleaseTemporary(rtTempA);
				rtTempA = rtTempB;
			}
			// 将最终模糊后的纹理绘制到目标纹理
			Graphics.Blit(rtTempA, destination);
			// 释放临时纹理A
			RenderTexture.ReleaseTemporary(rtTempA);
		}
		else{
			// 默认绘制
			Graphics.Blit(source, destination);

		}
	}
}
