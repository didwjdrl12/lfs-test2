using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;



#region [유틸] BigEndian

public static class BigEndian
{
	public static short ToBigEndian(this short value)
	{
		return System.Net.IPAddress.HostToNetworkOrder(value);
	}
	public static int ToBigEndian(this int value)
	{
		return System.Net.IPAddress.HostToNetworkOrder(value);
	}
	public static long ToBigEndian(this long value)
	{
		return System.Net.IPAddress.HostToNetworkOrder(value);
	}
	public static short FromBigEndian(this short value)
	{
		return System.Net.IPAddress.NetworkToHostOrder(value);
	}
	public static int FromBigEndian(this int value)
	{
		return System.Net.IPAddress.NetworkToHostOrder(value);
	}
	public static long FromBigEndian(this long value)
	{
		return System.Net.IPAddress.NetworkToHostOrder(value);
	}
}

#endregion




#region [유틸] JTcpMessageHeader

public class JTcpMessageHeader
{
	public byte _command;
	
	#region [초기화]
	//------------------------------------------------------------------------------------------------------------------------------------------------------
	public JTcpMessageHeader()
	{
	}
	#endregion

	#region [HeaderSize]
	//------------------------------------------------------------------------------------------------------------------------------------------------------
	public virtual int HeaderSize
	{
		get { return 0; }
	}
	//------------------------------------------------------------------------------------------------------------------------------------------------------
	public virtual int DataSize
	{
		get { return 0; }
		set { }
	}
	//------------------------------------------------------------------------------------------------------------------------------------------------------
	public virtual int CheckSumSize
	{
		get { return 0; }
	}
	#endregion


	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	// IO
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	#region [IO] Write
	//------------------------------------------------------------------------------------------------------------------------------------------------------
	public virtual void Write(BinaryWriter writer)
	{		
	}
	//------------------------------------------------------------------------------------------------------------------------------------------------------
	public virtual void WritePost(BinaryWriter writer)
	{
	}
	#endregion
	
	#region [IO] Read
	//------------------------------------------------------------------------------------------------------------------------------------------------------
	public virtual void Read(BinaryReader reader)
	{		
	}
	#endregion
	
}
#endregion



//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//
// JTcpMessage
//
//
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

public class JTcpMessage
{

	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	// 변수
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


	#region 변수

	public JTcpMessageHeader _header;
	
	#endregion
	
	#region Property	
	public int _data_size
	{
		get { return _header.DataSize; }
		set { _header.DataSize = value; }
	}
	public int _packet_size
	{
		get { return _header.DataSize + _header.HeaderSize + _header.CheckSumSize; }
	}
	#endregion


	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	// 메인 함수
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	#region [초기화]
	//------------------------------------------------------------------------------------------------------------------------------------------------------
	public JTcpMessage()
	{
	}
	public static short ToShort(int i)
	{
		return (short)i;
	}
	#endregion


	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	// IO
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	#region [IO] Write
	//------------------------------------------------------------------------------------------------------------------------------------------------------
	public virtual void Write(BinaryWriter writer)
	{
		_header.Write(writer);
	}
	#endregion

	#region [IO] Read
	//------------------------------------------------------------------------------------------------------------------------------------------------------
	public virtual void Read(BinaryReader reader)
	{
	}
	#endregion


}

