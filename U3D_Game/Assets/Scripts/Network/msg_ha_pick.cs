using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class msg_sc_ha_pick : msg_handler {
	
	private ushort type = Events.MSG_SC_HA_PICK;

	public int hid;

	public int uid;

	
	
	public Byte[] marshal ()
	{
		//消息类型, 各字段的数据
		//长度由最后发送的时候添加
		
		List<Byte> bytes = new List<Byte> ();
		
		//消息类型
		//传入服务器端是不需要传入字符串长度
		AppendParam (bytes, type);
		AppendParam (bytes, hid);
		AppendParam (bytes, uid);
		
		return bytes.ToArray ();
	}
	
	public void unmarshal (Byte[] data)
	{
		int index = 0;
		type = unmarshalushort (data, ref index);
		hid = unmarshalint (data, ref index);
		uid = unmarshalint (data, ref index);
		
		//Console.WriteLine ("%d", BitConverter.ToInt32(data, 10+len));
		
	}
	
}
