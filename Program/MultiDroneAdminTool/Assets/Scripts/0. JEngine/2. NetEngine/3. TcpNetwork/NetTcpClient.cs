using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
#if !OLD_DOT_NET
using System.Threading.Tasks;
#endif



namespace J2y.Network
{
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// NetTcpClient
	//
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public class NetTcpClient : NetTcpPeer_base, IDisposable
	{
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Variable
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
		#region Variable

		public TcpClient _net_client;


        #endregion

        #region [Property] [Events] OnConnected/OnDisconnected

        public event Action OnEvent_ServerConnected;
        public event Action OnEvent_ServerDisconnected;

        #endregion
        
        #region [Property]

        public override string _ip_address
		{
			get { return _net_client.Client.RemoteEndPoint.ToString(); }
		}
		public int _remote_port
		{
			get; set;
		}
		public override int Port { get { return _remote_port; }  }

		#endregion



		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 0. Base Methods
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [Init] 생성자
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public NetTcpClient()
		{ }
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public NetTcpClient(TcpClient net_client)
			: this()
		{
			initialize(net_client);
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public NetTcpClient(string server_ip, int port)
			: this()
		{
			Connect(server_ip, port);
		}
		#endregion

		#region [Init] initialize
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual void initialize(TcpClient net_client)
		{
			_net_client = net_client;
			_net_stream = net_client.GetStream();

			initialize(_net_stream);

			//var end_point = _net_client.Client.RemoteEndPoint as IPEndPoint;
		}
		#endregion



		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// [네트워크]
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [네트워크] 연결
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual bool Connect(string server_ip, int port, Action<int> fun_connected = null)
		{
			if(IsConnected())
			{
                if (_ip_address != (server_ip+':'+port))
                    Disconnect();
                else
                {
                    fun_connected?.Invoke(1);
                    return true;
                }
			}

			try
			{
				var net_client = new TcpClient(server_ip, port);
				if (null == net_client)
					return false;
				if(null == _message_dispatcher)
					_message_dispatcher = new NetMessage_dispatcher_default();
				initialize(net_client);
				OnServerConnected();
				_remote_port = port;
                fun_connected?.Invoke(1);
            }
			catch (SocketException e)
			{
				JLogger.WriteFormat(@"[TCP] Server Connect Fail!!!:({0}:{1})-{2}", server_ip, port, e.Message);

				return false;
			}

			return true;
		}
		#endregion

		#region [네트워크] 종료
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public void Dispose()
		{
			Disconnect();
		}

		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public override void Disconnect()
		{
			var conn = IsConnected();
			if (!conn)
				return;
			
			if (conn)
				OnServerDisconnected();

			var ip_address = _ip_address;
			try
			{
				_net_client.Client.Disconnect(false);
				_net_stream.Close();
				_net_client.Close();
			}
			catch(SocketException e)
			{
				JLogger.WriteFormat(@"[SocketException] [Disconnect]({0} {1}).", ip_address, e.Message);
			}
			//var conn2 = IsConnected();
			//JLogger.WriteFormat(@"Disconnect Clients({0} {1}).", ip_address, conn2);

		}
        #endregion

        #region [네트워크] [이벤트] OnConnected/OnDisconnected (서버에 연결됨)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void OnServerConnected()
        {
            OnEvent_ServerConnected?.Invoke();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void OnServerDisconnected()
        {
            if (_parent_peer is NetTcpServer server)
            {
                server.OnClientDisconnected(this);
                server.m_connections.Remove(this);
            }

            if (_sender_channel != null)
                _sender_channel.Stop();
            if (_recv_channel != null)
                _recv_channel.Stop();

            _recv_channel = null;
            _sender_channel = null;

            OnEvent_ServerDisconnected?.Invoke();
        }
        #endregion

        #region [네트워크] IsConnected
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override bool IsConnected()
		{
			return (_net_client != null) && _net_client.Connected;

			#region [백업]
			//if (_net_client.Connected)
			//{
			//	return IsConnected(_net_client.Client);
			//	//if ((_net_client.Client.Poll(0, SelectMode.SelectWrite)) && (!_net_client.Client.Poll(0, SelectMode.SelectError)))
			//	//{
			//	//	byte[] buffer = new byte[1];
			//	//	if (_net_client.Client.Receive(buffer, SocketFlags.Peek) == 0)
			//	//	{
			//	//		return false;
			//	//	}
			//	//	else
			//	//	{
			//	//		return true;
			//	//	}
			//	//}
			//	//else
			//	//{
			//	//	return false;
			//	//}
			//}
			//else
			//{
			//	return false;
			//}
			#endregion
		}
		public static bool IsConnected(Socket socket)
		{
			try
			{
				return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
			}
			catch (SocketException) { return false; }
		}
		#endregion
        


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 유틸
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [유틸] 패킷 유효성 검사
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual bool ValidPacket(byte command)
		{
			return true;
		}
		#endregion

		#region [유틸] Check Sum 계싼
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static byte MakeChecksum(byte[] send_buf)
		{
			byte checkSum = 0;
			foreach (var ch in send_buf)
				checkSum += (byte)ch;

			return checkSum;
		}
		#endregion



		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 작업중...
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++






	}

}

