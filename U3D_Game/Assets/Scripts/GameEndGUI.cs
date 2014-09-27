using UnityEngine;
using System.Collections;

public class GameEndGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		GUI.BeginGroup (new Rect (Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 100));
		
		if (GUI.Button (new Rect (0, 0, 100, 100), "游戏结束")) {
			// 创建一个定位到屏幕中央的组
			Application.Quit();
		}
		
		// 完成组的创建，这非常重要
		GUI.EndGroup ();
	}
}
