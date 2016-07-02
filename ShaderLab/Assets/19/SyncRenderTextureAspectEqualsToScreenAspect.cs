using UnityEngine;
using System.Collections;
public class SyncRenderTextureAspectEqualsToScreenAspect : MonoBehaviour {
	public RenderTexture m_rt;
	// Use this for initialization
	void Start () {
		// 设置RenderTarget的屏幕比例和屏幕比例相同
		float screenAspect = (float)(Screen.width) / Screen.height;
		////Debug.Log (screenAspect);
		m_rt.width = (int)(m_rt.height*screenAspect);
	}
}
