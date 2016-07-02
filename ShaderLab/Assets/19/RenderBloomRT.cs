using UnityEngine;
using System.Collections;

// 如果场景中有的GameObject的shader是GroupBLoom,那么就正常渲染纹理
// 如果是其他类型，那么就渲染成黑色
// 最后渲染的纹理应该是只显示使用GroupBloom的物体,其它都是黑色
// 然后在摄像机上再加一个内置的Blur脚本, 把得到的纹理模糊下
// 存入RenderTargeth
// 将摄像机的RenderTarget指向RenderBlommRT
public class RenderBloomRT : MonoBehaviour {

	public Camera m_mainCameraRef;
	public Shader m_renderBloomTexShader;
	// Use this for initialization
	void Start () {
		// 设置摄像机不可用，不然场景中有两个摄像机，会渲染两次
		GetComponent<Camera> ().enabled = false;//it is equals to uncheck Camera component in Inspector
		// 同步主摄像机的信息到当前摄像机,用于在主摄像机的位置渲染一张BloomRenderTarget
		synchronizePosAndRotWithMainCamera ();
		synchronizeProjModeAndFrustomWithMainCamera ();
	}
	void LateUpdate () {
		// 只需要同步位置参数
		synchronizePosAndRotWithMainCamera ();
		//Rendering with Replaced Shaders: http://www.cnblogs.com/wantnon/p/4528677.html
		// 根据不同的RenderType替换掉场景中的对象的渲染shader
		GetComponent<Camera>().RenderWithShader(m_renderBloomTexShader, "RenderType");
	}
	void synchronizePosAndRotWithMainCamera(){
		// 将主摄像机的位置和旋转参数同步到当前摄像机上
		transform.position=m_mainCameraRef.transform.position;
		transform.rotation = m_mainCameraRef.transform.rotation;
	}
	void synchronizeProjModeAndFrustomWithMainCamera(){
		// 将主摄像机的投影参数同步到当前摄像机上
		GetComponent<Camera>().orthographic=m_mainCameraRef.orthographic;
		GetComponent<Camera> ().orthographicSize = m_mainCameraRef.orthographicSize;
		GetComponent<Camera> ().nearClipPlane = m_mainCameraRef.nearClipPlane;
		GetComponent<Camera> ().farClipPlane = m_mainCameraRef.farClipPlane;
		GetComponent<Camera> ().fieldOfView = m_mainCameraRef.fieldOfView;
	}
}

