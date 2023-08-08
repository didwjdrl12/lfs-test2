//using System;
//using System.Diagnostics;
//using System.Net;

//namespace J2y.Network
//{
//	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//	//
//	// NetBuffer
//	//		
//	//
//	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//	//
//	// (Backup) NetBuffer
//	//		
//	//
//	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	
//	public partial class NetBuffer_lid
//	{
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// 상수, Static
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//		#region [상수] 
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		// Number of bytes to overallocate for each message to avoid resizing
//		protected const int c_overAllocateAmount = 4;
//		#endregion

//		#region [Static] ReadMethods, WriteMethods(RPC 구현시 필요)
//		private static readonly Dictionary<Type, MethodInfo> s_readMethods;
//		private static readonly Dictionary<Type, MethodInfo> s_writeMethods;
//		#endregion


//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// 변수
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//		#region [변수] Data, Length, Position
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		internal NetMessageHeader _header;
//		internal byte[] m_data;
//		internal int m_readPosition;
//		//internal int m_bitLength;
//		#endregion


//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// Property
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//		#region [Property] Data
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		/// <summary>
//		/// Gets or sets the internal data buffer
//		/// </summary>
//		public byte[] Data
//		{
//			get { return m_data; }
//			set { m_data = value; }
//		}
//		#endregion

//		#region [Property] Length
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		/// <summary>
//		/// Gets or sets the length of the used portion of the buffer in bytes
//		/// </summary>
//		public int LengthBytes
//		{
//			get { return ((m_bitLength + 7) >> 3); }
//			set
//			{
//				m_bitLength = value * 8;
//				InternalEnsureBufferSize(m_bitLength);
//			}
//		}

//		/// <summary>
//		/// Gets or sets the length of the used portion of the buffer in bits
//		/// </summary>
//		public int LengthBits
//		{
//			get { return m_bitLength; }
//			set
//			{
//				m_bitLength = value;
//				InternalEnsureBufferSize(m_bitLength);
//			}
//		}
//		#endregion

//		#region [Property] Position
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		/// <summary>
//		/// Gets or sets the read position in the buffer, in bits (not bytes)
//		/// </summary>
//		public long Position
//		{
//			get { return (long)m_readPosition; }
//			set { m_readPosition = (int)value; }
//		}

//		/// <summary>
//		/// Gets the position in the buffer in bytes; note that the bits of the first returned byte may already have been read - check the Position property to make sure.
//		/// </summary>
//		public int PositionInBytes
//		{
//			get { return (int)(m_readPosition / 8); }
//		}
//		#endregion


//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// 메인 함수
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//		#region [StaticConstructor] Read, Write 함수들 추출(RPC에서 사용)
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		static NetBuffer_lid()
//		{
//			s_readMethods = new Dictionary<Type, MethodInfo>();
//			MethodInfo[] methods = typeof(NetIncomingMessage).GetMethods(BindingFlags.Instance | BindingFlags.Public);
//			foreach (MethodInfo mi in methods)
//			{
//				if (mi.GetParameters().Length == 0 && mi.Name.StartsWith("Read", StringComparison.InvariantCulture) && mi.Name.Substring(4) == mi.ReturnType.Name)
//				{
//					s_readMethods[mi.ReturnType] = mi;
//				}
//			}

//			s_writeMethods = new Dictionary<Type, MethodInfo>();
//			methods = typeof(NetOutgoingMessage).GetMethods(BindingFlags.Instance | BindingFlags.Public);
//			foreach (MethodInfo mi in methods)
//			{
//				if (mi.Name.Equals("Write", StringComparison.InvariantCulture))
//				{
//					ParameterInfo[] pis = mi.GetParameters();
//					if (pis.Length == 1)
//						s_writeMethods[pis[0].ParameterType] = mi;
//				}
//			}
//		}
//		#endregion

//	}
//}

