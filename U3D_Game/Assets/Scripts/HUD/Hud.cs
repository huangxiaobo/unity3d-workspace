using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {
	public string UserName;
	public string KillText;
	public string LifeText;

	public string PatronText;
	public string GrenadeText;

	public GUIStyle	uiStyle;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private bool toggleBool = true;

	public void OnGUI() {

		
		if (GUI.Button (new Rect (Screen.width - 100, 20, 100, 20), "离开游戏")) {
			print ("Moving to GameEnd");
			if (GUI.Button (new Rect (Screen.width / 2 - 30, Screen.height / 2 - 15, 60, 30), "确定结束？")) {
				Application.Quit();
			}
		} 
		if (GUI.Button (new Rect (Screen.width - 100, 65, 100, 20), "重新开始")) {
			Game.RestartGame();
			Application.LoadLevel ("EmptyScene");
		}


		GUI.Box (new Rect (Screen.width - 100, 100, 100, 80), new GUIContent ("F1:\t开启聊天\n\nESC:\t解锁光标\n\nF4:\t锁定光标"));

		GUI.Box (new Rect (Screen.width - 100, 200, 100, 20), new GUIContent (Game._User.login ?  "当前在线" : "当前离线"));

		//用户信息

		GUI.Box (new Rect (0, Screen.height - 100, 100.0f, 20), new GUIContent ("F1聊天"));

		GUI.Box (new Rect (0, Screen.height - 75, 100.0f, 20), new GUIContent (UserName));
		
		GUI.Box (new Rect (0, Screen.height - 50, 100.0f, 20), new GUIContent (LifeText));

		GUI.Box (new Rect (0, Screen.height - 25, 100.0f, 20), new GUIContent (KillText));

		//武器状态
		GUI.Box (new Rect (Screen.width - 100, Screen.height - 50, 100.0f, 20), new GUIContent (GrenadeText));
		
		GUI.Box (new Rect (Screen.width - 100, Screen.height - 25, 100.0f, 20), new GUIContent (PatronText));


		//
	}

	private void RestartGame() {
		enabled = false;
		Game._User.login = false;

		//退成服务器


	}
}
