using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class msg_sc_enemy_create : msg_handler{
	public ushort msg_type = Events.MSG_SC_ENEMY_CREATE;
	public int eid;
	public float x;
	public float y;
	public float z;
	public int life;
	public int enemy_type;
	
	public Byte[] marshal ()
	{
		//消息类型, 各字段的数据
		//长度由最后发送的时候添加
		
		List<Byte> bytes = new List<Byte> ();
		
		//消息类型
		//传入服务器端是不需要传入字符串长度
		AppendParam (bytes, msg_type);
		//AppendParam (bytes, name.Length);
		AppendParam (bytes, eid);
		AppendParam (bytes, x);
		AppendParam (bytes, y);
		AppendParam (bytes, z);
		AppendParam (bytes, life);
		AppendParam (bytes, enemy_type);
		
		return bytes.ToArray ();
	}
	
	public void unmarshal (Byte[] data)
	{
		int index = 0;
		msg_type = unmarshalushort (data, ref index);
		eid = unmarshalint (data, ref index);
		x = unmarshalfloat (data, ref index);
		y = unmarshalfloat (data, ref index);
		z = unmarshalfloat (data, ref index);
		life = unmarshalint (data, ref index);
		enemy_type = unmarshalint (data, ref index);

		//Console.WriteLine ("%d", BitConverter.ToInt32(data, 10+len));
		
	}
}

