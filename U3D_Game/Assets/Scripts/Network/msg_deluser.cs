using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class msg_sc_deluser : msg_handler {
	public static ushort type = Events.MSG_SC_DELUSER;
	public int uid;
	
	public Byte[] marshal ()
	{
		//消息类型, 各字段的数据
		//长度由最后发送的时候添加
		
		List<Byte> bytes = new List<Byte> ();
		
		//消息类型
		//传入服务器端是不需要传入字符串长度
		AppendParam (bytes, type);
		AppendParam (bytes, uid);
		return bytes.ToArray ();
	}
	
	public void unmarshal (Byte[] data)
	{
		//Debug.Log ("data len: " + data.Length);
		int index = 0;
		type = unmarshalushort (data, ref index);
		uid = unmarshalint (data, ref index);
	}
}



public class msg_cs_deluser : msg_handler {
	public static ushort type = Events.MSG_CS_DELUSER;
	public int uid;
	
	public Byte[] marshal ()
	{
		//消息类型, 各字段的数据
		//长度由最后发送的时候添加
		
		List<Byte> bytes = new List<Byte> ();
		
		//消息类型
		//传入服务器端是不需要传入字符串长度
		AppendParam (bytes, type);
		AppendParam (bytes, uid);
		return bytes.ToArray ();
	}
	
	public void unmarshal (Byte[] data)
	{
		//Debug.Log ("data len: " + data.Length);
		int index = 0;
		type = unmarshalushort (data, ref index);
		uid = unmarshalint (data, ref index);
	}
}
