using UnityEngine;
using System.Collections;

[ExecuteInEditMode]  
[RequireComponent(typeof(Camera))]
public class AE_GroupBloom : MonoBehaviour {
	public Material m_groupBloomMaterial;

	void OnEnable()
	{
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
	}

	void start() {
		if (SystemInfo.supportsImageEffects == false) {  
			enabled = false;  
			return;  
		}  
	}

	[ImageEffectOpaque]
	void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture){
		// 使用指定的材质来渲染屏幕
		Graphics.Blit (sourceTexture, destTexture, m_groupBloomMaterial);
	}
}