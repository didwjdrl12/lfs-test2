using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
#if !OLD_DOT_NET
using System.Threading.Tasks;
#endif


//public enum eNetTaskType
//{
//	Accept, Read, Write,
//}

//public class JNetTask
//{
//	public eNetTaskType _type;
//	public Task<JNetTask> _task;
//	public Task<TcpClient> _task_accept;	
//}



namespace J2y.Network
{

	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// NetTcpServer
	//
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public class NetTcpServer : NetTcpPeer_base
	{
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] Tcp Server

        public eNetServerState _server_state;
        protected TcpListener _net_listener;
		protected string _serverName;

		#endregion


		#region [Variable] �̺�Ʈ

		public Action OnEvent_ServerStarted;
        public Action OnEvent_ServerStopped;
        public Action<NetTcpClient> OnEvent_ClientConnected;
        public Action<NetTcpClient> OnEvent_ClientDisconncted;

        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] Address
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public string Address
        {
            get {
                if (null == _net_listener)
                    return "";
                return ((IPEndPoint)_net_listener.LocalEndpoint).Address.ToString(); }
        }
        #endregion

        #region [Property] Port
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override int Port
        {
            get {
                if (null == _net_listener)
                    return 0;
                return ((IPEndPoint)_net_listener.LocalEndpoint).Port;
            }
        }
        #endregion

        



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public NetTcpServer(string serverName = "Server")
		{
			_serverName = serverName;
		}
		#endregion

		#region [Update]
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public override void Update()
		{
			base.Update();

			var idx = m_connections.Count;
			while (idx > 0) // ���ο��� �߰�/���� ����
			{
				--idx;
				if (m_connections.Count <= idx)
					continue;

				var c = m_connections[idx];
				c.Update();				
			}
		}
		#endregion


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// �񵿱� ����
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


		#region [����] ����
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual void StartServer(int server_port)
		{
            if (_server_state == eNetServerState.Running)
                return;

            if (_message_dispatcher == null)
				_message_dispatcher = new NetMessage_dispatcher_default();

			var task_accept = AysncEchoServer(server_port);
            _server_state = eNetServerState.Running;
            OnStartServer();
        }
        #endregion

        #region [����] ����
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void StopServer()
		{
            if (_server_state == eNetServerState.Stop)
                return;

            _net_listener.Stop();
            // todo: ���� ����
            JLogger.Write("[TODO][Server] Stop Implements");

            OnStopServer();
            _server_state = eNetServerState.Stop;
        }
        #endregion


        #region [����] �񵿱� ���� ����
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task AysncEchoServer(int server_port)
		{
			_net_listener = new TcpListener(IPAddress.Any, server_port);
			_net_listener.Start();
						
			while (true)
			{
				var net_client = await _net_listener.AcceptTcpClientAsync().ConfigureAwait(true);				
				var tcp_client = MakeNetClient(net_client);
				tcp_client.SetMessageDispatcher(_message_dispatcher); // ������ �Ѱ����� �޽��� ��� ó��
				tcp_client._parent_peer = this;

				AddMainThreadCommand(() =>
				{
					OnClientConnected(tcp_client);
					m_connections.Add(tcp_client);
				});
			}
		}
		#endregion

		#region [����] Ŭ���̾�Ʈ ����
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual NetTcpClient MakeNetClient(TcpClient net_client)
		{
			var tcp_client = new NetTcpClient(net_client);
			return tcp_client;
		}
		#endregion




		#region [��Ʈ��ũ] [�̺�Ʈ] OnStartServer/OnStopServer
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual void OnStartServer()
        {
            JLogger.WriteFormat("[{0}] OnStartServer(Port:{1})", _serverName, Port);
            OnEvent_ServerStarted?.Invoke();
        }
		public virtual void OnStopServer()
        {
            JLogger.WriteFormat("[{0}] OnStopServer", _serverName);
            OnEvent_ServerStopped?.Invoke();
        }
        #endregion

        #region [��Ʈ��ũ] [�̺�Ʈ] Client Connected/Disonnected
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void OnClientConnected(NetTcpClient net_client)
        {
            JLogger.Write("[NetTcpServer] OnClientConnected:" + net_client._ip_address);
            OnEvent_ClientConnected?.Invoke(net_client);
        }
		public virtual void OnClientDisconnected(NetTcpClient net_client)
        {
            JLogger.Write("[NetTcpServer] OnClientDisconnected:" + net_client._ip_address);

            OnEvent_ClientDisconncted?.Invoke(net_client);
        }
		#endregion


		#region [��ƿ] LocalIPAddress
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static string GetLocalIPAddress()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					return ip.ToString();
				}
			}
			throw new Exception("No network adapters with an IPv4 address in the system!");
		}
		#endregion




		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Connections
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		//#region [��Ʈ��ũ] [Connections] ����(������, ����, ���Ͻü���)
		////------------------------------------------------------------------------------------------------------------------------------------------------------
		//public virtual void NetClient_change(NetTcpClient from, NetTcpClient to)
		//{
		//	to.Tag = to;
		//	m_connections.Add(to);
		//	to._parent_peer = this;
		//	to.OnConnected();

		//	//m_connections.Remove(from);
		//	//from.OnDisconnected();			
		//}
		//#endregion



		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// ���
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [���]
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		//public ServerSocket(IPAddress ipAddress, int port)
		//{
		//	_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		//	_server.Bind(new IPEndPoint(ipAddress, port));
		//	_server.Listen(20);
		//	JLogger.WriteFormat("Wait connection");
		//	Accept();
		//}
		//private async Task StartSend(int i)
		//{
		//	try
		//	{
		//		CodingImage img = new CodingImage();
		//		var buffer = img.CodingImages("C:\\image\\file" + i + ".bmp");
		//		Task<int> senTask = SendAsync.AsyncSend(client, buffer, 0, buffer.Length);
		//	}
		//	catch (Exception ex)
		//	{

		//	}
		//}
		//private async void Accept()
		//{
		//	for (int i = 0; i < 14; i++)
		//	{
		//		client = await Task.Factory.FromAsync<socket>(_server.BeginAccept, _server.EndAccept, true);
		//		JLogger.WriteFormat("Connected");
		//		await StartSend(i);

		//	}
		//}

		//public static Task<int> AsyncSend(this Socket socket, byte[] buffer, int offset, int count)
		//{
		//	return Task.Factory.FromAsync<int>(
		//		socket.BeginSend(buffer, offset, count, SocketFlags.None, null, socket),
		//		socket.EndSend);
		//}
		#endregion


	}

}
