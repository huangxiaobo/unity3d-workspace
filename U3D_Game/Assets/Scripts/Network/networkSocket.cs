using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;

public class NetworkSocket : MonoBehaviour
{
	public String host = "localhost";
	public Int32 port = 2000;
	internal Boolean socket_ready = false;
	internal String input_buffer = "";
	TcpClient tcp_socket;
	NetworkStream net_stream;
	Queue<Byte> sendBuf = new Queue<Byte>();


	public static NetworkSocket network;
	
	public static NetworkSocket Instance {
		get {
			if (network == null) {
				GameObject go = GameObject.Find("/Game/NetworkManager");
				network = (NetworkSocket)(go.GetComponent("NetworkSocket"));
			}
			return network;
		}
	}
	
	public static User _User = new User();


	void Update ()
	{
		if (net_stream == null) {
			Game._User.login = false;

		}
		try {
		readSocket ();
		if (sendBuf.Count != 0) {
			Byte[] data = sendBuf.ToArray();
			//Debug.Log("Send: " + BitConverter.ToString(data).Replace('-', ' '));
			writeSocket (data);
			sendBuf.Clear();
		}
		} catch (SocketException se) {
			Game._User.login = false;
		} catch(IOException io) {
		}
	}

	void Awake ()
	{
		setupSocket ();

		//StartCoroutine ("process");\


	}

	void OnApplicationQuit ()
	{
		closeSocket ();
	}

	public void setupSocket ()
	{
		Debug.Log ("setupSocket");
		try {
			tcp_socket = new TcpClient (host, port);

			net_stream = tcp_socket.GetStream ();

			socket_ready = true;
		} catch (Exception e) {
			// Something went wrong
			Debug.Log ("Socket error: " + e);
		}
	}

	public void writeSocket (Byte[] bytes)
	{
		if (!socket_ready || bytes.Length == 0)
			return;
		if (net_stream.CanWrite)
			net_stream.Write (bytes, 0, bytes.Length);
	}

	static int count = 0;

	public void readSocket ()
	{
		//Debug.Log ("readSocket");
		if (!socket_ready)
			return;

		Byte[] readByte = new byte[4];
		if (net_stream != null && net_stream.DataAvailable && readByte.Length == net_stream.Read (readByte, 0, readByte.Length)) {//读取长度
			//长度
			int len = BitConverter.ToInt32 (readByte, 0);
			Byte[] rcvBuf = new byte[len];

			net_stream.Read (rcvBuf, 0, rcvBuf.Length);

			process (rcvBuf);
		}

	}

	public void closeSocket ()
	{
		if (!socket_ready)
			return;

		tcp_socket.Close ();
		socket_ready = false;
	}

	public void  process (Byte[] rcvBuf)
	{
		Debug.Log ("<color=blue>process</color> rcvBuf.Len: " + rcvBuf.Length);

		if (rcvBuf.Length <= 2) {//消息类型长度至少两个字节
			return;
		}
		//先读两个字节,获取消息类型
			
		ushort type = BitConverter.ToUInt16 (rcvBuf, 0);
		//string hex = BitConverter.ToString(rcvBuf).Replace("-", string.Empty);
		//Debug.Log ("rcvBuf: " + hex);

		GameObject go;
		switch (type) {
		case Events.MSG_CS_LOGIN:
		case Events.MSG_SC_CONFIRM:
		case Events.MSG_SC_WAIT:
		case Events.MSG_SC_START:
			Debug.Log("Login!!!!!!!!!!!!!");
			go = GameObject.Find("/Game/Login");
			if (go == null) {
				throw new NullReferenceException();
			}
			Login login = (Login) go.GetComponent(typeof(Login));
			login.Handler(type, rcvBuf);
			break;
		case Events.MSG_CS_CHAT:
		case Events.MSG_SC_CHAT:
		{
			Debug.Log("Chat!!!!!!!!!!!!!");
			go = GameObject.Find("/Game/Chat");
			if (go == null) {
				throw new NullReferenceException();
			}
			Chat chat = (Chat) go.GetComponent(typeof(Chat));
			chat.Handler(type, rcvBuf);
		}
			break;
		case Events.MSG_SC_PLAYER_CREATE:
		case Events.MSG_SC_DELUSER:
		case Events.MSG_SC_PLAYER_POS:
		case Events.MSG_CS_PLAYER_POS:
		case Events.MSG_SC_PLAYER_MOV:
		case Events.MSG_CS_PLAYER_MOV:
		case Events.MSG_SC_PLAYER_ROT:
		case Events.MSG_CS_PLAYER_VEL:
		case Events.MSG_SC_PLAYER_VEL:
		case Events.MSG_SC_PLAYER_LIFE:
		case Events.MSG_SC_SHOOT_STATUS:
		case Events.MSG_SC_PLAYER_KILLCOUNT:
		{
			go = GameObject.Find("PlayerCreator");
			if (go == null) {
				throw new NullReferenceException();
			}
			PlayerService ps = (PlayerService) go.GetComponent(typeof(PlayerService));
			ps.Handler(type, rcvBuf);
		}
			break;

		case Events.MSG_SC_ENEMY_CREATE:
		case Events.MSG_SC_ENEMY_MOVE:
		case Events.MSG_SC_ENEMY_POS:
		case Events.MSG_SC_ENEMY_ROT:
		case Events.MSG_SC_ENEMY_DIE:
		case Events.MSG_SC_ENEMY_DAMAGE:
		{
			go = GameObject.Find("Game/Enemy");
			if (go == null) {
				throw new NullReferenceException();
			}
			EnemyService es = (EnemyService) go.GetComponent(typeof(EnemyService));
			es.Handler(type, rcvBuf);
		}
			break;
		case Events.MSG_SC_HA_CREATE:
		case Events.MSG_SC_HA_PICK:
		{		
			go = GameObject.Find("Game/HealthAmmo");
			Debug.Log("MSG_SC_HA_CREATE---------go: " + go);
			if (go == null) {
				throw new NullReferenceException();
			}
			HealthAmmoService ps = (HealthAmmoService) go.GetComponent(typeof(HealthAmmoService));
			ps.Handler(type, rcvBuf);

		}
			break;
		default:
			throw new Exception("Unhandlerable message: " + type);
			Debug.Log ("<color=red>Unhandlerable message</color> type: " + type);
			break;
		}

	}	

	public void Send(Byte[] data) {
		//Debug.Log ("<color=blue>Network Send</color>");

		int len = data.Length;

		Byte[] buf = BitConverter.GetBytes (len);
		for (int i = 0; i < buf.Length; ++i) {
			sendBuf.Enqueue (buf [i]);
		}
		for (int i = 0; i < data.Length; ++i) {
			sendBuf.Enqueue (data [i]);
		}
	}
}