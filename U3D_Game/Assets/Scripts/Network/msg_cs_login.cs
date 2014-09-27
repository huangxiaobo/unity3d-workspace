using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class msg_cs_login : msg_handler
{
	public ushort type = Events.MSG_CS_LOGIN;		//msg type
	private string _name;

	public string name {
		get {
			return _name;
		}
		set {
			_name = value;
		}
	}

	private string _password;

	public string password {
		get {
			return _password;
		}

		set {
			_password = value;
		}
	}

	public static string fmt = "=HI%ds%ds";

	public msg_cs_login ()
	{

	}

	public Byte[] marshal ()
	{
		//消息类型, 各字段的数据
		//长度由最后发送的时候添加

		List<Byte> bytes = new List<Byte> ();

		//消息类型
		//传入服务器端是不需要传入字符串长度
		AppendParam (bytes, type);
		//AppendParam (bytes, name.Length);
		AppendParam (bytes, name);
		AppendParam (bytes, password);

		return bytes.ToArray ();
	}

	public void unmarshal (Byte[] data)
	{
		Debug.Log ("data len: " + data.Length);
		int index = 0;
		type = unmarshalushort (data, ref index);
		name = unmarshalstring (data, ref index);
		password = unmarshalstring(data, ref index);

		//Console.WriteLine ("%d", BitConverter.ToInt32(data, 10+len));

	}

	public void print() {
		Debug.Log ("msg_type: " + type
		           + " name: " + name
		           + " password: " + password);
	}



}


