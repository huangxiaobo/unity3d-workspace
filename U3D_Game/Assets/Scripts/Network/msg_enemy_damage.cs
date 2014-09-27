using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class msg_sc_enemy_damage : msg_handler{
	public ushort type = Events.MSG_CS_ENEMY_DAMAGE;
	public int eid;
	public int uid;
	public int damage;

	public Byte[] marshal ()
	{
		//消息类型, 各字段的数据
		//长度由最后发送的时候添加
		
		List<Byte> bytes = new List<Byte> ();
		
		//消息类型
		//传入服务器端是不需要传入字符串长度
		AppendParam (bytes, type);
		AppendParam (bytes, eid);
		AppendParam (bytes, uid);
		AppendParam (bytes, damage);
		
		return bytes.ToArray ();
	}
	
	public void unmarshal (Byte[] data)
	{
		int index = 0;
		type = unmarshalushort (data, ref index);
		eid = unmarshalint(data, ref index);
		uid = unmarshalint(data, ref index);
		damage = unmarshalint (data, ref index);
	}
	
}

public class msg_cs_enemy_damage : msg_handler{
	public ushort type = Events.MSG_CS_ENEMY_DAMAGE;
	public int eid;
	public int uid;
	public int damage;
	
	public Byte[] marshal ()
	{
		//消息类型, 各字段的数据
		//长度由最后发送的时候添加
		
		List<Byte> bytes = new List<Byte> ();
		
		//消息类型
		AppendParam (bytes, type);
		AppendParam (bytes, eid);
		AppendParam (bytes, uid);
		AppendParam (bytes, damage);
		
		return bytes.ToArray ();
	}
	
	public void unmarshal (Byte[] data)
	{
		int index = 0;
		type = unmarshalushort (data, ref index);
		eid = unmarshalint(data, ref index);
		uid = unmarshalint(data, ref index);
		damage = unmarshalint (data, ref index);
		
		//Console.WriteLine ("%d", BitConverter.ToInt32(data, 10+len));
		
	}
}

