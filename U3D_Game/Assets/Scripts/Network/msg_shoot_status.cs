using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class msg_sc_shoot_status : msg_handler{
	public ushort msg_type = Events.MSG_SC_SHOOT_STATUS;
	public int uid;
	public float ray_org_x;
	public float ray_org_y;
	public float ray_org_z;
	public float ray_dir_x;
	public float ray_dir_y;
	public float ray_dir_z;

	
	public Byte[] marshal ()
	{
		//消息类型, 各字段的数据
		//长度由最后发送的时候添加
		
		List<Byte> bytes = new List<Byte> ();
		
		//消息类型
		//传入服务器端是不需要传入字符串长度
		AppendParam (bytes, msg_type);
		//AppendParam (bytes, name.Length);
		AppendParam (bytes, uid);
		AppendParam (bytes, ray_org_x);
		AppendParam (bytes, ray_org_y);
		AppendParam (bytes, ray_org_z);
		AppendParam (bytes, ray_dir_x);
		AppendParam (bytes, ray_dir_y);
		AppendParam (bytes, ray_dir_z);

		return bytes.ToArray ();
	}
	
	public void unmarshal (Byte[] data)
	{
		int index = 0;
		msg_type = unmarshalushort (data, ref index);
		uid = unmarshalint (data, ref index);
		ray_org_x = unmarshalfloat (data, ref index);
		ray_org_y = unmarshalfloat (data, ref index);
		ray_org_z = unmarshalfloat (data, ref index);
		ray_dir_x = unmarshalfloat (data, ref index);
		ray_dir_y = unmarshalfloat (data, ref index);
		ray_dir_z = unmarshalfloat (data, ref index);
	}
}


public class msg_cs_shoot_status : msg_handler{
	public ushort msg_type = Events.MSG_CS_SHOOT_STATUS;
	public int uid;
	public float ray_org_x;
	public float ray_org_y;
	public float ray_org_z;
	public float ray_dir_x;
	public float ray_dir_y;
	public float ray_dir_z;
	
	
	public Byte[] marshal ()
	{
		//消息类型, 各字段的数据
		//长度由最后发送的时候添加
		
		List<Byte> bytes = new List<Byte> ();
		
		//消息类型
		//传入服务器端是不需要传入字符串长度
		AppendParam (bytes, msg_type);
		//AppendParam (bytes, name.Length);
		AppendParam (bytes, uid);
		AppendParam (bytes, ray_org_x);
		AppendParam (bytes, ray_org_y);
		AppendParam (bytes, ray_org_z);
		AppendParam (bytes, ray_dir_x);
		AppendParam (bytes, ray_dir_y);
		AppendParam (bytes, ray_dir_z);
		
		return bytes.ToArray ();
	}
	
	public void unmarshal (Byte[] data)
	{
		int index = 0;
		msg_type = unmarshalushort (data, ref index);
		uid = unmarshalint (data, ref index);
		ray_org_x = unmarshalfloat (data, ref index);
		ray_org_y = unmarshalfloat (data, ref index);
		ray_org_z = unmarshalfloat (data, ref index);
		ray_dir_x = unmarshalfloat (data, ref index);
		ray_dir_y = unmarshalfloat (data, ref index);
		ray_dir_z = unmarshalfloat (data, ref index);
	}
}



