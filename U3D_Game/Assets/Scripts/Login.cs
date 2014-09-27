using UnityEngine;
using System;
using System.Collections;


public class Login : MonoBehaviour
{

	enum Status
	{
		LOGIN,
		WAIT_FOR_CONFIRM,
		WAIT_FOR_START,
		PWD_ERROR,
		START,
	};

	Status status = Status.LOGIN;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnGUI ()
	{  

		string info = "";
		GUI.BeginGroup (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));
			
		//用户名  
		GUI.Label (new Rect (20, 20, 50, 20), "用户名");  
		Game._User.username = GUI.TextField (new Rect (80, 20, 100, 20), Game._User.username, 15);//15为最大字符串长度  
		//密码  
		GUI.Label (new Rect (20, 50, 50, 20), "密  码");  
		Game._User.password = GUI.PasswordField (new Rect (80, 50, 100, 20), Game._User.password, '*');//'*'为密码遮罩  

		//登录按钮  
		 
		switch (status) {
		case Status.LOGIN:
			{
				if (GUI.Button (new Rect (80, 80, 50, 20), "登录")) { 
					Game._User.Verify ();
					//StartCoroutine(Wait(5.1f));
					//gameObject.SetActive(false);
					//enabled = false;

					status = Status.WAIT_FOR_CONFIRM;
				}  
			}
			break;
		case Status.WAIT_FOR_CONFIRM:
			info = "等待确认";
			break;
		case Status.PWD_ERROR:
			info = "密码错误,请重试";
			if (GUI.Button (new Rect (80, 80, 50, 20), "登录")) { 
				Game._User.Verify ();
				//StartCoroutine(Wait(5.1f));
				//gameObject.SetActive(false);
				//enabled = false;
			}
			break;
		case Status.WAIT_FOR_START:
			info = "登陆成功, 正在等待其他玩家";
			break;
		case Status.START:
			info = "开始游戏";
			break;

		default:
			break;
		}

		//信息  
		GUI.Label (new Rect (20, 100, 200, 20), info);  
		// 完成组的创建，这非常重要
		GUI.EndGroup ();
	
	}

	IEnumerator  Wait (float time)
	{
		while (true) {
			yield return new WaitForSeconds (time); 
			if (Game._User.login == true) {
				Debug.Log ("**********************");
				Application.LoadLevel ("Level01");
				gameObject.SetActive (true);
				enabled = true;
				yield return null;
			}
		}
	}

	public void Handler (ushort msg_type, byte[] data)
	{
		switch (msg_type) {
		case Events.MSG_CS_LOGIN:
			Debug.Log ("MSG_CS_LOGIN!!!!!!!!!!!!!!!!!!!!");
			{
				msg_cs_login mcl = new msg_cs_login ();
				mcl.unmarshal (data);
				
				mcl.print ();
			}
			break;
		case Events.MSG_SC_CONFIRM:
			Debug.Log ("MSG_SC_CONFIRM!!!!!!!!!!!!!!!!!!!!");
			{
				msg_sc_confirm msc = new msg_sc_confirm ();
				msc.unmarshal (data);
				
				if (msc.result != 0) {
					Game._User.uid =  msc.uid;
					if (Game._User.uid == 0)
						throw new ArgumentNullException ("Game._User.uid == 0");
					status = Status.WAIT_FOR_START;
				} else {
					status = Status.PWD_ERROR;
				}
			}
			break;
		case Events.MSG_SC_WAIT:
			Debug.Log ("MSG_SC_WAIT!!!!!!!!!!!!!!!!!!!!");
			status = Status.WAIT_FOR_START;
			break;
		case Events.MSG_SC_START:
			Debug.Log ("Login succuss");
			Game._User.login = true;
			enabled = false;
			Screen.lockCursor = true;
			break;
		default:
			Debug.Log ("login default!!!!!!!!!!!!!!!!!!!!");
			break;
		}
	}
}
