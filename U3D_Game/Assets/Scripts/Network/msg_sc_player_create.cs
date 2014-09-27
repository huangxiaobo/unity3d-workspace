using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class msg_sc_player_create : msg_handler{
	public ushort type = Events.MSG_SC_PLAYER_CREATE;
	public int uid;
	public float x;
	public float y;
	public float z;

	public Byte[] marshal ()
	{
		//消息类型, 各字段的数据
		//长度由最后发送的时候添加
		
		List<Byte> bytes = new List<Byte> ();
		
		//消息类型
		//传入服务器端是不需要传入字符串长度
		AppendParam (bytes, type);
		//AppendParam (bytes, name.Length);
		AppendParam (bytes, uid);
		AppendParam (bytes, x);
		AppendParam (bytes, y);
		AppendParam (bytes, z);
		
		return bytes.ToArray ();
	}
	
	public void unmarshal (Byte[] data)
	{
		Debug.Log ("data len: " + data.Length);
		int index = 0;
		type = unmarshalushort (data, ref index);
		uid = unmarshalint (data, ref index);
		x = unmarshalfloat (data, ref index);
		y = unmarshalfloat (data, ref index);
		z = unmarshalfloat (data, ref index);
		
		//Console.WriteLine ("%d", BitConverter.ToInt32(data, 10+len));
		
	}
	
	public void print() {
		Debug.Log ("msg_type: " + type
		           + " x: " + x + " y: " + y + " z: " + z
		          );
	}

}
