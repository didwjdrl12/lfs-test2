using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace J2y.Network
{

	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// NetUdpReceiverChannel
	//
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public class NetUdpReceiverChannel : NetReceiverChannel_base
	{
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Variable
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		
		#region [Variable] Base

		private NetUdpPeer_base _udp_peer;

		#endregion


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 0. Base Methods
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [Init]
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public NetUdpReceiverChannel(NetPeer_base netpeer)
			: base(netpeer)
		{
			_udp_peer = netpeer as NetUdpPeer_base;
		}
		#endregion
		

		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// [메시지] 수신  
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
		#region [구현] [수신] [IO 쓰레드] 메시지(1개) 수신
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public override async Task read_message_async()
		{
			var net_client = _udp_peer._net_client;
			var header_size = NetBase.HeaderByteSize;


			//-------------------------------------------------------------------------------------
			// 1. 비동기 데이터 읽기
			//
			try
			{
				var recv_result = await net_client.ReceiveAsync();
				var recv_buffer = recv_result.Buffer;
				var data_size = recv_buffer.Length - header_size;

				//-------------------------------------------------------------------------------------
				// 2. 패킷 분리 후 디스패치(Header + Data)
				//
				// Header
				_message_header.Read(recv_buffer, 0);		
				// Data
				var stream_buf = new MemoryStream(recv_buffer, header_size, data_size); // todo: MemoryPool				
				var net_buffer = new NetBuffer();
				net_buffer._stream = stream_buf;
				net_buffer._position = 0;
				net_buffer._sender_end_point = recv_result.RemoteEndPoint;
				net_buffer._size = data_size;
				_queued_recvs.Enqueue(net_buffer);				
			}
			catch(Exception e)
			{
				JLogger.Write("[UDP Exception]" + e.Message);				
			}			
		}
		#endregion

	}

}
