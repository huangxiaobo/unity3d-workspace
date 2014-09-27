using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerService : MonoBehaviour {

	public GameObject playerPrefab;

	private string debugstring = "";

	private Dictionary<int, Player> playerDict = new Dictionary<int, Player>();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Handler(int msg_type, byte[] msg) {
		Debug.Log ("PlayerService---msg_type: " + BitConverter.GetBytes(msg_type).ToString());
		switch (msg_type) {
		case Events.MSG_SC_PLAYER_CREATE:
			Debug.Log ("MSG_SC_PLAYER_CREATE!!!!!!!!!!!!!!!!!!!!");
			{
				msg_sc_player_create mpc = new msg_sc_player_create ();
				mpc.unmarshal (msg);
				CreatePlayer (mpc.uid, new Vector3 (mpc.x, mpc.y, mpc.z));
			}
		
			break;
		case Events.MSG_SC_PLAYER_MOV:
			Debug.Log ("MSG_SC_PLAYER_MOV!!!!!!!!!!!!!!!!!!!!");
			break;
		case Events.MSG_SC_DELUSER:
			Debug.Log ("MSG_SC_DELUSER!!!!!!!!!!!!!!!!!!!!");
			{
				msg_sc_deluser msd= new msg_sc_deluser ();
				msd.unmarshal (msg);
				DelPlayer(msd.uid);
			}
			break;
		case Events.MSG_SC_PLAYER_POS:
		{
			msg_sc_player_pos mspp = new msg_sc_player_pos();
			mspp.unmarshal(msg);
			DownloadPlayerPos(mspp.uid, mspp.x, mspp.y, mspp.z);
		}
			break;
		case Events.MSG_SC_PLAYER_ROT:
		{
			msg_sc_player_rot mspr = new msg_sc_player_rot();
			mspr.unmarshal(msg);
			DownloadPlayerRot(mspr.uid, mspr.rotx, mspr.roty);
		}
			break;
		case Events.MSG_CS_PLAYER_VEL:
		{
			msg_sc_player_vel mspv = new msg_sc_player_vel();
			mspv.unmarshal(msg);
			DownloadPlayerVel(mspv.uid, mspv.x, mspv.y, mspv.z);
		}
			break;
		case Events.MSG_SC_PLAYER_LIFE:
		{
			msg_sc_player_life mspl = new msg_sc_player_life();
			mspl.unmarshal(msg);
			DownloadPlayerLife(mspl.uid, mspl.life);
		}
			break;
		case Events.MSG_SC_SHOOT_STATUS:
		{
			msg_sc_shoot_status msss = new msg_sc_shoot_status();
			msss.unmarshal(msg);
			DownloadShootStatus(msss.uid, 
			                    new Vector3(msss.ray_org_x, msss.ray_org_y, msss.ray_org_z),
			                    new Vector3(msss.ray_dir_x, msss.ray_dir_y, msss.ray_dir_z));
		}
			break;

		case Events.MSG_SC_PLAYER_KILLCOUNT:
		{
			msg_sc_player_killcount mspk = new msg_sc_player_killcount();
			mspk.unmarshal(msg);
			Debug.Log("uid============" + mspk.uid + "uiserid = " + Game._User.uid);
			if (Game._User.uid == mspk.uid && playerDict.ContainsKey(mspk.uid))
			{
				playerDict[mspk.uid].KillCount = mspk.killcount;
			}
		}
			break;
		default:
			break;
		}
	}

	public void CreatePlayer(int uid, Vector3 position) {
		//Debug.Log ("CreatePlayer-----uid: " + uid);
		GameObject player  = (GameObject)Instantiate (playerPrefab, position, transform.rotation);
		Player pl = (Player) player.GetComponent(typeof(Player));
		pl.uid = uid;
		player.transform.parent = this.transform;

		//加入字典
		playerDict.Add (uid, pl);
	
		if (uid != Game._User.uid) {
			pl.isMine = false;
			Transform camTr = player.transform.Find ("Cam/CamRnd/Camera");
			camTr.gameObject.SetActive(false);
			Camera camera = camTr.gameObject.GetComponent<Camera>();;
			camera.enabled = false;

			Transform tr;
			tr = player.transform.Find("Cam");
			foreach(Transform tran in tr.GetComponentsInChildren<Transform>()){//遍历当前物体及其所有子物体  
				tran.gameObject.layer = 9;//更改物体的Layer层  
			}

			tr = player.transform.Find("Animations");
			foreach(Transform tran in tr.GetComponentsInChildren<Transform>()){//遍历当前物体及其所有子物体  
				tran.gameObject.layer = 8;//更改物体的Layer层  
			}
		} else {
			pl.isMine = true;

			Transform camTr = player.transform.Find ("Cam/CamRnd/Camera");
			camTr.gameObject.SetActive(true);
			Camera camera = camTr.gameObject.GetComponent<Camera>();;
			camera.enabled = true;
			camera.cullingMask &= ~(1 << 9);	//关闭9层
			camera.cullingMask |=  (1 << 8);	//打开8层

			Transform tr;
			tr = player.transform.Find("Cam");
			foreach(Transform tran in tr.GetComponentsInChildren<Transform>()){//遍历当前物体及其所有子物体  
				tran.gameObject.layer = 8;//更改物体的Layer层  
			}
			
			tr = player.transform.Find("Animations");
			foreach(Transform tran in tr.GetComponentsInChildren<Transform>()){//遍历当前物体及其所有子物体  
				tran.gameObject.layer = 9;//更改物体的Layer层  
			}
		}



		//摄像机
		GameObject uicam = GameObject.Find("/Game/Login/UICam");
		if (uicam == null)
			return;
		uicam.SetActive (false);

		GameObject login = GameObject.Find("/Game/Login");
		if (login != null) {
			login.SetActive(false);
		}
	}

	private void DelPlayer(int uid) {
		//Debug.Log ("DelPlayer.........uid: " + uid);
		Transform[] trs = gameObject.GetComponentsInChildren<Transform> ();
		foreach (Transform tr in trs) {
			GameObject go = tr.gameObject;
			Player pc = (Player) go.GetComponent(typeof(Player));
			if (pc &&  pc.uid == uid) {
				Destroy(go);
			}
		}
		playerDict.Remove (uid);
	}

	private void DownloadPlayerPos(int uid, float x, float y, float z) {
		//Debug.Log ("DownloadPlayerPos.........uid: " + uid + " " + x + " " + y + " " + z);
		Player player = playerDict [uid];
		Vector3 pos = new Vector3(x, y, z);
		player.SyncDowPos(pos);
	}

	public static void UploadPlayerPos(int uid, float x, float y, float z) {
		msg_cs_player_pos mcpp = new msg_cs_player_pos ();
		mcpp.uid = uid;
		mcpp.x = x;
		mcpp.y = y;
		mcpp.z = z;
		NetworkSocket.Instance.Send (mcpp.marshal ());
	}

	
	private void DownloadPlayerMov(int uid, float x, float y, float z) {
		//Debug.Log ("DownloadPlayerMov.........uid: " + uid + " " + x + " " + y + " " + z);
		Player player = playerDict [uid];
		Vector3 pos = new Vector3(x, y, z);
		player.SyncDowMov(pos);
	}
	
	public static void UploadPlayerMov(int uid, float x, float y, float z) {
		msg_cs_player_mov mcpm = new msg_cs_player_mov ();
		mcpm.uid = uid;
		mcpm.x = x;
		mcpm.y = y;
		mcpm.z = z;
		NetworkSocket.Instance.Send (mcpm.marshal ());
	}

	private void DownloadPlayerVel(int uid, float x, float y, float z) {
		//Debug.Log ("DownloadPlayerVel.........uid: " + uid + " " + x + " " + y + " " + z);
		Player player = playerDict [uid];
		Vector3 pos = new Vector3(x, y, z);
		player.SyncDowVel(pos);
	}
	
	public static void UploadPlayerVel(int uid, float x, float y, float z) {
		msg_cs_player_vel mcpv = new msg_cs_player_vel ();
		mcpv.uid = uid;
		mcpv.x = x;
		mcpv.y = y;
		mcpv.z = z;
		NetworkSocket.Instance.Send (mcpv.marshal ());
	}
	
	public static void UploadPlayerRot(int uid, float rotx, float roty) {
		msg_cs_player_rot mcpr = new msg_cs_player_rot ();
		mcpr.uid = uid;
		mcpr.rotx = rotx;
		mcpr.roty = roty;
		NetworkSocket.Instance.Send (mcpr.marshal());
	}

	public void DownloadPlayerRot(int uid, float rotx, float roty) {
		//Debug.Log ("<color=red>DownloadPlayerRot</color>");
		Player player = playerDict [uid];
		player.SyncDownRot (rotx, roty);
	}

	public void DownloadPlayerLife(int uid, int life) {
		Debug.Log ("<color=red>DownloadPlayerLife</color> life: " + life);
		if (playerDict.ContainsKey (uid)) {
			playerDict[uid].SyncDownLife(life);
		}
	}

	private void DownloadShootStatus(int uid, Vector3 ray_org, Vector3 ray_dir) {
		Debug.Log ("<color=red>DownloadShootStatus</color>: " + (playerDict.ContainsKey (uid)) + " uid: " + uid);
		if (playerDict.ContainsKey (uid)) {
			Player pl = playerDict[uid];
			Transform gunTr = pl.transform.Find ("Cam/CamRnd/ak47");

			Gun gun = (Gun)gunTr.gameObject.GetComponent(typeof(Gun));
			gun.DownloadShootStatus(ray_org, ray_dir);
		}
	}


	public void OnGUI() {
		GUI.Label (new Rect (20, 100, 200, 20), debugstring);  
	}
}
