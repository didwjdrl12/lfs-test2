using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using NetEndPoint = System.Net.IPEndPoint;

namespace J2y.Network
{
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// NetMessagePool
	//
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public class NetMessagePool
	{
		public static NetMessagePool Instance;

//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// Variable
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


//		#region [Variable] Storage

//		internal List<byte[]> m_storagePool;
		
//		internal int m_storagePoolBytes;
//		internal int m_storageSlotsUsedCount;
//		private int m_maxCacheCount;

//		#endregion

//		#region [Variable] Outgoing/Incoming Messages

//		private NetQueue<NetOutgoingMessage> m_outgoingMessagesPool;
//		private NetQueue<NetIncomingMessage> m_incomingMessagesPool;

//		#endregion




//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// 0. Base Methods
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//		#region [Init] Pools
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		public void InitializePools()
//		{
//			Instance = this;
//			m_storageSlotsUsedCount = 0;

//			if (m_configuration.UseMessageRecycling)
//			{
//				m_storagePool = new List<byte[]>(16);
//				m_outgoingMessagesPool = new NetQueue<NetOutgoingMessage>(4);
//				m_incomingMessagesPool = new NetQueue<NetIncomingMessage>(4);
//			}
//			else
//			{
//				m_storagePool = null;
//				m_outgoingMessagesPool = null;
//				m_incomingMessagesPool = null;
//			}

//			m_maxCacheCount = m_configuration.RecycledCacheMaxCount;
//		}
//		#endregion


//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// Storage
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//		#region [Storage] Get
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		internal byte[] GetStorage(int minimumCapacityInBytes)
//		{
//			if (m_storagePool == null)
//				return new byte[minimumCapacityInBytes];

//			lock (m_storagePool)
//			{
//				for (int i = 0; i < m_storagePool.Count; i++)
//				{
//					byte[] retval = m_storagePool[i];
//					if (retval != null && retval.Length >= minimumCapacityInBytes)
//					{
//						m_storagePool[i] = null;
//						m_storageSlotsUsedCount--;
//						m_storagePoolBytes -= retval.Length;
//						return retval;
//					}
//				}
//			}
//			m_statistics.m_bytesAllocated += minimumCapacityInBytes;
//			return new byte[minimumCapacityInBytes];
//		}
//		#endregion

//		#region [Storage] Recycle
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		internal void Recycle(byte[] storage)
//		{
//			if (m_storagePool == null || storage == null)
//				return;

//			lock (m_storagePool)
//			{
//				int cnt = m_storagePool.Count;
//				for (int i = 0; i < cnt; i++)
//				{
//					if (m_storagePool[i] == null)
//					{
//						m_storageSlotsUsedCount++;
//						m_storagePoolBytes += storage.Length;
//						m_storagePool[i] = storage;
//						return;
//					}
//				}

//				if (m_storagePool.Count >= m_maxCacheCount)
//				{
//					// pool is full; replace randomly chosen entry to keep size distribution
//					var idx = NetRandom.Instance.Next(m_storagePool.Count);

//					m_storagePoolBytes -= m_storagePool[idx].Length;
//					m_storagePoolBytes += storage.Length;
					
//					m_storagePool[idx] = storage; // replace
//				}
//				else
//				{
//					m_storageSlotsUsedCount++;
//					m_storagePoolBytes += storage.Length;
//					m_storagePool.Add(storage);
//				}
//			}
//		}
//		#endregion



//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// [Message] Create
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//		#region [OutgoingMessage] Create()
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		/// <summary>
//		/// Creates a new message for sending
//		/// </summary>
//		public NetOutgoingMessage CreateMessage()
//		{
//			return CreateMessage(m_configuration.m_defaultOutgoingMessageCapacity);
//		}
//		#endregion

//		#region [OutgoingMessage] Create(string)
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		/// <summary>
//		/// Creates a new message for sending and writes the provided string to it
//		/// </summary>
//		public NetOutgoingMessage CreateMessage(string content)
//	    {
//	        NetOutgoingMessage om;
	
//	        // Since this could be null.
//	        if (string.IsNullOrEmpty(content))
//	        {
//	            om = CreateMessage(1); // One byte for the internal variable-length zero byte.
//	        }
//	        else
//	        {
//	            om = CreateMessage(2 + content.Length); // Fair guess.
//	        }
	
//	        om.Write(content);
//	        return om;
//	    }
//		#endregion

//		#region [OutgoingMessage] Create(capacity)
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		/// <summary>
//		/// Creates a new message for sending
//		/// </summary>
//		/// <param name="initialCapacity">initial capacity in bytes</param>
//		public NetOutgoingMessage CreateMessage(int initialCapacity)
//		{
//			NetOutgoingMessage retval;
//			if (m_outgoingMessagesPool == null || !m_outgoingMessagesPool.TryDequeue(out retval))
//				retval = new NetOutgoingMessage();

//			NetException.Assert(retval.m_recyclingCount == 0, "Wrong recycling count! Should be zero" + retval.m_recyclingCount);

//			if (initialCapacity > 0)
//				retval.m_data = GetStorage(initialCapacity);

//			return retval;
//		}
//		#endregion

//		#region [IncomingMessage] Create(byte[])
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		internal NetIncomingMessage CreateIncomingMessage(byte[] useStorageData)
//		{
//			NetIncomingMessage retval;
//			if (m_incomingMessagesPool == null || !m_incomingMessagesPool.TryDequeue(out retval))
//				retval = new NetIncomingMessage(tp);
//			else
//				retval.m_incomingMessageType = tp;
//			retval.m_data = useStorageData;
//			return retval;
//		}
//		#endregion

//		#region [IncomingMessage] Create(minimumByteSize)
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		internal NetIncomingMessage CreateIncomingMessage(int minimumByteSize)
//		{
//			NetIncomingMessage retval;
//			if (m_incomingMessagesPool == null || !m_incomingMessagesPool.TryDequeue(out retval))
//				retval = new NetIncomingMessage(tp);
//			else
//				retval.m_incomingMessageType = tp;
//			retval.m_data = GetStorage(minimumByteSize);
//			return retval;
//		}
//		#endregion

//		#region [IncomingMessage] Create(string)
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		/// <summary>
//		/// Creates an incoming message with the required capacity for releasing to the application
//		/// </summary>
//		internal NetIncomingMessage CreateIncomingMessage(string text)
//		{
//			NetIncomingMessage retval;
//			if (string.IsNullOrEmpty(text))
//			{
//				retval = CreateIncomingMessage(tp, 1);
//				retval.Write(string.Empty);
//				return retval;
//			}

//			int numBytes = System.Text.Encoding.UTF8.GetByteCount(text);
//			retval = CreateIncomingMessage(tp, numBytes + (numBytes > 127 ? 2 : 1));
//			retval.Write(text);

//			return retval;
//		}
//		#endregion


//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// [Message] Recycle
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//		#region [IncomingMessage] Recycle
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		/// <summary>
//		/// Recycles a NetIncomingMessage instance for reuse; taking pressure off the garbage collector
//		/// </summary>
//		public void Recycle(NetIncomingMessage msg)
//		{
//			if (m_incomingMessagesPool == null || msg == null)
//				return;

//			NetException.Assert(m_incomingMessagesPool.Contains(msg) == false, "Recyling already recycled incoming message! Thread race?");

//			byte[] storage = msg.m_data;
//			msg.m_data = null;
//			Recycle(storage);
//			msg.Reset();

//			if (m_incomingMessagesPool.Count < m_maxCacheCount)
//				m_incomingMessagesPool.Enqueue(msg);
//		}
//		#endregion

//		#region [IncomingMessage] Recycle(list)
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		/// <summary>
//		/// Recycles a list of NetIncomingMessage instances for reuse; taking pressure off the garbage collector
//		/// </summary>
//		public void Recycle(IEnumerable<NetIncomingMessage> toRecycle)
//		{
//			if (m_incomingMessagesPool == null)
//				return;
//			foreach (var im in toRecycle)
//				Recycle(im);
//		}
//		#endregion

//		#region [OutgoingMessage] Recycle
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		internal void Recycle(NetOutgoingMessage msg)
//		{
//			if (m_outgoingMessagesPool == null)
//				return;
//#if DEBUG
//			NetException.Assert(m_outgoingMessagesPool.Contains(msg) == false, "Recyling already recycled outgoing message! Thread race?");
//			if (msg.m_recyclingCount != 0)
//				LogWarning("Wrong recycling count! should be zero; found " + msg.m_recyclingCount);
//#endif
//			// setting m_recyclingCount to zero SHOULD be an unnecessary maneuver, if it's not zero something is wrong
//			// however, in RELEASE, we'll just have to accept this and move on with life
//			msg.m_recyclingCount = 0;

//			byte[] storage = msg.m_data;
//			msg.m_data = null;

//			// message fragments cannot be recycled
//			// TODO: find a way to recycle large message after all fragments has been acknowledged; or? possibly better just to garbage collect them
//			if (msg.m_fragmentGroup == 0)
//				Recycle(storage);

//			msg.Reset();
//			if (m_outgoingMessagesPool.Count < m_maxCacheCount)
//				m_outgoingMessagesPool.Enqueue(msg);
//		}
//		#endregion

		
	}

}
