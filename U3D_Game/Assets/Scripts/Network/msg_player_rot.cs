using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class msg_sc_player_rot : msg_handler{
	public ushort type = Events.MSG_SC_PLAYER_ROT;
	public int uid;
	public float rotx;
	public float roty;
	
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
		AppendParam (bytes, rotx);
		AppendParam (bytes, roty);
		
		return bytes.ToArray ();
	}
	
	public void unmarshal (Byte[] data)
	{
		int index = 0;
		type = unmarshalushort (data, ref index);
		uid = unmarshalint (data, ref index);
		rotx = unmarshalfloat (data, ref index);
		roty = unmarshalfloat (data, ref index);
		
		//Console.WriteLine ("%d", BitConverter.ToInt32(data, 10+len));
		
	}

}

public class msg_cs_player_rot : msg_handler{
	public ushort type = Events.MSG_CS_PLAYER_ROT;
	public int uid;
	public float rotx;
	public float roty;
	
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
		AppendParam (bytes, rotx);
		AppendParam (bytes, roty);
		
		return bytes.ToArray ();
	}
	
	public void unmarshal (Byte[] data)
	{
		int index = 0;
		type = unmarshalushort (data, ref index);
		uid = unmarshalint (data, ref index);
		rotx = unmarshalfloat (data, ref index);
		roty = unmarshalfloat (data, ref index);
		
		//Console.WriteLine ("%d", BitConverter.ToInt32(data, 10+len));
		
	}
}

