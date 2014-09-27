using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class Events
{

	public const ushort		MSG_CS_LOGIN = 0x1001;
	public const ushort		MSG_SC_CONFIRM = 0x1002;
	public const ushort		MSG_SC_WAIT = 0x1003;
	public const ushort		MSG_SC_START = 0x1004;
			
	public const ushort		MSG_CS_MOVETO = 0x2001;
	public const ushort		MSG_SC_MOVETO = 0x2002;
			
	public const ushort		MSG_CS_CHAT = 0x3001;
	public const ushort		MSG_SC_CHAT = 0x3002;
			
	public const ushort		MSG_SC_ADDUSER = 0x4001;
	public const ushort		MSG_SC_DELUSER = 0x4002;
	
	public const ushort 	MSG_CS_DELUSER = 0x4003;			
			
			
	public const ushort		MSG_SC_ENEMY_CREATE = 0X5003;
			
	public const ushort		MSG_SC_PLAYER_CREATE = 0X6001;
			
	public const ushort		MSG_SC_PLAYER_POS = 0X7001;
	public const ushort		MSG_CS_PLAYER_POS = 0X7002;
	public const ushort		MSG_SC_PLAYER_MOV = 0X7003;
	public const ushort		MSG_CS_PLAYER_MOV = 0X7004;
	public const ushort		MSG_SC_PLAYER_ROT = 0X7005;
	public const ushort		MSG_CS_PLAYER_ROT = 0X7006;
	public const ushort		MSG_SC_PLAYER_VEL = 0X7007;
	public const ushort		MSG_CS_PLAYER_VEL = 0X7008;
	public const ushort		MSG_SC_PLAYER_LIFE = 0X7009;
	public const ushort		MSG_CS_PLAYER_LIFE = 0X700A;
	public const ushort		MSG_SC_PLAYER_KILLCOUNT = 0X700B;
	public const ushort		MSG_CS_PLAYER_KILLCOUNT = 0X700C;
	public const ushort		MSG_SC_PLAYER_DAMAGE = 0X700D;
	public const ushort		MSG_CS_PLAYER_DAMAGE = 0X700E;

			
	public const ushort		MSG_SC_ENEMY_ADD = 0X8001;
	public const ushort		MSG_CS_ENEMY_ADD = 0X8002;
	public const ushort		MSG_SC_ENEMY_DIE  = 0X8003;
	public const ushort		MSG_CS_ENEMY_DIE = 0X8004;
	public const ushort		MSG_CS_ENEMY_MOVE = 0x8005;
	public const ushort		MSG_SC_ENEMY_MOVE = 0X8006;
	public const ushort		MSG_SC_ENEMY_DAMAGE = 0X8007;
	public const ushort		MSG_CS_ENEMY_DAMAGE = 0X8008;
	public const ushort		MSG_SC_ENEMY_STATUS = 0X8009;
	public const ushort		MSG_CS_ENEMY_STATUS = 0X800A;
	public const ushort		MSG_CS_ENEMY_POS = 0x800B;
	public const ushort		MSG_SC_ENEMY_POS = 0X800C;
	public const ushort		MSG_CS_ENEMY_ROT = 0x800D;
	public const ushort		MSG_SC_ENEMY_ROT = 0X800E;

	public const ushort		MSG_SC_SHOOT_STATUS = 0X9001;
	public const ushort		MSG_CS_SHOOT_STATUS = 0X9002;

	public const ushort 	MSG_SC_HA_PICK = 0XA001;
	public const ushort		MSG_CS_HA_PICK = 0XA002;
	public const ushort 	MSG_SC_HA_CREATE = 0XA003;
	public const ushort		MSG_CS_HA_CREATE = 0XA004;
}

public class msg_handler
{

	public msg_handler ()
	{
	}

	public static void AppendParam (List<Byte> bytes, int value)
	{
		Byte[] b = new byte[4];
		b = BitConverter.GetBytes (value);
		
		for (int i = 0; i < b.Length; ++i)
			bytes.Add (b [i]);
	}
	
	public static void AppendParam (List<Byte> bytes, ushort value)
	{
		Byte[] b = new byte[2];
		b = BitConverter.GetBytes (value);
		
		for (int i = 0; i < b.Length; ++i)
			bytes.Add (b [i]);
	}

	public static void AppendParam (List<Byte> bytes, float value)
	{
		Byte[] b = new byte[4];
		b = BitConverter.GetBytes (value);
		
		for (int i = 0; i < b.Length; ++i)
			bytes.Add (b [i]);
	}
	
	public static void AppendParam (List<Byte> bytes, bool value)
	{
		bytes.Add (BitConverter.GetBytes (value) [0]);
	}
	
	
	
	public static void AppendParam (List<Byte> bytes, string value)
	{
		AppendParam (bytes, value.Length);
		
		
		for (int i = 0; i < value.Length; ++i)
			bytes.Add (BitConverter.GetBytes (value [i]) [0]);
	}
	
	public string unmarshalstring (Byte[] buf, ref int index)
	{
		int len = BitConverter.ToInt32 (buf, index);
		index += 4;

		Byte[] str_byte = new byte[len];
		for (int i = 0; i < len; ++i) {
			str_byte [i] = buf [index+i];
		}
		string result = System.Text.Encoding.Default.GetString (str_byte);
		index += len;
		return result;
	}
	
	public static int unmarshalint(Byte[] buf, ref int index) {
		int result = BitConverter.ToInt32 (buf, index);
		index += 4;
		return result;
	}
	
	
	public static ushort unmarshalushort(Byte[] buf, ref int index) {
		ushort result = BitConverter.ToUInt16 (buf, index);
		index += 2;
		return result;
	}

	public static float unmarshalfloat(Byte[] buf, ref int index) {
		float result = BitConverter.ToSingle (buf, index);
		index += 4;
		return result;
	}
}


public class msg_sc_wait : msg_handler {
	public ushort type = Events.MSG_SC_WAIT;
	public int uid;
	
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
		
		return bytes.ToArray ();
	}
	
	public void unmarshal (Byte[] data)
	{
		int index = 0;
		type = unmarshalushort (data, ref index);
		uid = unmarshalint (data, ref index);	
	}
}


public class msg_sc_start : msg_handler {
	public ushort type = Events.MSG_SC_START;
	public int uid;
	
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
		
		return bytes.ToArray ();
	}
	
	public void unmarshal (Byte[] data)
	{
		int index = 0;
		type = unmarshalushort (data, ref index);
		uid = unmarshalint (data, ref index);	
	}
}
