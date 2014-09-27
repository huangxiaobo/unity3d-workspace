using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[Serializable]

public class Game : MonoBehaviour
{

	//显示面板
	static Hud m_Hud;

	public static Hud _Hud { 
		get { 
			if (m_Hud == null) 
				m_Hud = (Hud)MonoBehaviour.FindObjectOfType (typeof(Hud));
			return m_Hud;
		} 
	}

	static Game m_Game;

	public static Game _Game { 
		get { 
			if (m_Game == null) 
				m_Game = (Game)MonoBehaviour.FindObjectOfType (typeof(Game));
			return m_Game;
		} 
	}

	static User m_User;

	public static User _User {
		get {
			if (m_User == null) {
				m_User = new User ();
			}
			return m_User;
		}
	}
	
	internal Timer 		timer = new Timer ();
	public Transform 	Fx;		//用来挂在一些全局的对象，比如弹孔
	public Transform 	PlayerSpawn;
	private static Game instance;

	public static Game Instance {
		get {
			if (instance == null) {
				GameObject go = GameObject.Find ("/Game");
				instance = (Game)(go.GetComponent ("Game"));
			}
			return instance;
		}
	}

	internal float GameTime = TimeSpan.FromMinutes (18).Milliseconds;
	// Use this for initialization
	void Start ()
	{
		//Screen.lockCursor = true;
		//启动时,禁止hud
		_Hud.enabled = false;
		Application.runInBackground = true;
		//Screen.lockCursor = true;
	}

	void Awake() {
		_Hud.enabled = false;
		//Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_User.login) {
			_Hud.enabled = true;
		}	
		//释放鼠标
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Screen.lockCursor = false;
		}

		if (Input.GetKeyDown (KeyCode.Return)) {	//F1切换聊天
			Screen.lockCursor = false;
		}
		
		if (Input.GetKeyDown (KeyCode.F4)) {	//F1切换聊天
			Screen.lockCursor = true;
		}
 	}
	
	public void OnGUI ()
	{
		//按
		if (Game._User.login) {
			// we want to place the TextArea in a particular location - use BeginArea and provide Rect
			if (Screen.lockCursor)
				GUI.Box (new Rect (Screen.width / 2 - 50, 0, 100, 20), new GUIContent ("按ESC返回菜单"));
			else
				GUI.Box (new Rect (Screen.width / 2 - 50, 0, 100, 20), new GUIContent ("按F4返回游戏"));
		}
	}


	private void StartGame ()
	{
	}

	private void InstanciatePlayer ()
	{
		Vector3 v = (Random.onUnitSphere);
		v.y = 0;
		var vector3 = PlayerSpawn.position + v;
	}

	void DidLockCursor() {
		Debug.Log("Locking cursor");
	}
	void DidUnlockCursor() {
		Debug.Log("Unlocking cursor");
	}
	void OnMouseDown() {
		Screen.lockCursor = true;
	}
	private bool wasLocked = false;


	public static void RestartGame() {
		msg_cs_deluser mcd = new msg_cs_deluser ();
		mcd.uid = Game._User.uid;
		NetworkSocket.Instance.Send (mcd.marshal());
	}
}
