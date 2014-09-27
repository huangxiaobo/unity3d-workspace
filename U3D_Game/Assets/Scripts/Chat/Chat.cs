using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chat : MonoBehaviour
{

	private string chat_str = "";
	private bool chat = false;
	List<string>  chatBuf = new List<string> ();

	// Use this for initialization
	void Start ()
	{
	}

	void Awake() {
		chat = false;
		scrollPosition = new Vector2(scrollPosition.x, Mathf.Infinity);

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (chat) {
			Debug.Log("chat update");
		}

		if (Input.GetKeyDown (KeyCode.F1)) {	//F1切换聊天
			Debug.Log("F1 Down.....");
			Screen.lockCursor = false;
			chat = true;
			preFocusControl = GUI.GetNameOfFocusedControl();
		}

		if (!chat) {
			return;
		}
		scrollPosition = new Vector2(scrollPosition.x, Mathf.Infinity);
	}

	private string histroy_str = "";
	private string preFocusControl;
	private Vector2 scrollPosition;
	void OnGUI ()
	{
		if (chat) {
			//GUI.SetNextControlName ("InputTextField");
			chat_str = GUI.TextField (new Rect (0, Screen.height - 200, 200, 30), chat_str);
			//EditorGUI.FocusTextInControl ("InputTextField");	

			//if (GUI.GetNameOfFocusedControl() == string.Empty) {
			//	// Set focus to control Text1.
			//	GUI.FocusControl("InputTextField");
			//}

			if (GUI.Button (new Rect (210, Screen.height - 200, 50, 30), "发送")) {
				chat = false;
				GUI.FocusControl(preFocusControl);
				Screen.lockCursor = true;
				//Debug.Log ("input: " + chat_str);
				if (chat_str.Length != 0) {
					SendChat ();
					chat_str = "";
				}
								
			}
		}

		//显示历史消息



		// we want to place the TextArea in a particular location - use BeginArea and provide Rect
		GUILayout.BeginArea(new Rect(0, 0, 250, Screen.height - 200));
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width (250), GUILayout.Height (250));
		
		// We just add a single label to go inside the scroll view. Note how the
		// scrollbars will work correctly with wordwrap.
		GUILayout.Label (histroy_str);
		
		// End the scrollview we began above.
		GUILayout.EndScrollView ();
		GUILayout.EndArea();

				
	}

	private void SendChat ()
	{
		msg_cs_chat mcc = new msg_cs_chat ();
		mcc.uid = Game._User.uid;
		mcc.text = chat_str;

		NetworkSocket.Instance.Send (mcc.marshal ());
	}

	public void Handler (int msg_type, byte[] msg)
	{
		switch (msg_type) {
		case Events.MSG_CS_CHAT:
			//客户端不会遇到
			
			break;
		case Events.MSG_SC_CHAT:
			Debug.Log ("MSG_SC_CHAT!!!!!!!!!!!!!!!!!!!!");
			msg_sc_chat msc = new msg_sc_chat ();
			msc.unmarshal (msg);
			//chatBuf.Add (string.Format ("ID（{0}）say： {1}", msc.uid, msc.text));
			histroy_str += "\n\n";
			histroy_str += string.Format ("ID（{0}）say： {1}", msc.uid, msc.text);
			break;
		default:
			break;
		}
	}

}
