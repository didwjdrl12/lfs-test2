using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NetEndPoint = System.Net.IPEndPoint;

namespace J2y.Network
{
	#region [Structure] NetPeerReader
	//------------------------------------------------------------------------------------------------------------------------------------------------------
	public struct NetPeerReader
	{
		public NetPeer_base Peer;
		public BinaryReader Reader;
		public static NetPeerReader Create(NetPeer_base peer, BinaryReader reader)
		{
			return new NetPeerReader() { Peer = peer, Reader = reader };
		}
	}
    #endregion


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // NetPeer_base
    //
    //      1. Network Connections
    //      2. �޽��� �۽�
    //      3. �޽��� ����
    //      4. [���ξ�����] Commands
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public partial class NetPeer_base : INetMessageHandler
	{
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] Base
        public long _keep_alive_timer;

        #endregion

        #region [Variable] Connections
        public NetPeer_base _parent_peer;                     // �������� NetClient�� ��� ���ӵ� �θ𼭹�
		public readonly List<NetPeer_base> m_connections;
		protected readonly Dictionary<NetEndPoint, NetPeer_base> m_connectionLookup;
		#endregion

		#region [Variable] �޽��� �۽�/����
		public NetSenderChannel_base _sender_channel;
		public NetReceiverChannel_base _recv_channel;
		public NetMessage_dispatcher_base _message_dispatcher;
		public NetBuffer _last_recv_buffer;     // �ʿ��ϳ�??
		#endregion

		#region [Variable] Commands
		internal readonly NetQueue<Action> _commands_main_thred;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] Tag/IpAddress

        public Object Tag { get; set; }
		public virtual string _ip_address { get; }
		public virtual int Port { get; }
		#endregion

		#region [Property] MessageHeaderType

		public virtual int MessageHeaderType { get; set; } = 0;

        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] ������
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public NetPeer_base()
		{
			_commands_main_thred = new NetQueue<Action>(8);
			m_connectionLookup = new Dictionary<NetEndPoint, NetPeer_base>();
			m_connections = new List<NetPeer_base>();
		}
		#endregion

		#region [Update] 
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual void Update()
		{
			if(_recv_channel != null)
				_recv_channel.DispatchMessagess(_message_dispatcher);

			execute_mainThreadCommands();
		}
		#endregion



		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 1. ��Ʈ��ũ Connections
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	
		#region [��Ʈ��ũ] KeepAliveTimer
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public void UpdateKeepAliveTimer()
		{
			_keep_alive_timer = JTimer.GetCurrentTick();
		}
		#endregion
		
		#region [��Ʈ��ũ] IsConnected/Disconnect
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual bool IsConnected() { return false; }
		public virtual void Disconnect() { }
        #endregion

       
		#region [��Ʈ��ũ] DisconnectAll
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public void DisconnectAll()
		{
			foreach (var client in m_connections)
				client.Disconnect();
			m_connections.Clear();
		}

        #endregion

        

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 2. �޽��� �۽�
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [�۽�] ������ ������
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual eNetSendResult SendMessage(int message_type, params dynamic[] args) { return SendMessage(JNetMessage.Make(message_type, args)); }
        public virtual eNetSendResult SendMessage(int message_type, byte[] data_buffer, int data_size) { return SendMessage(JNetMessage.Make(message_type, data_buffer, data_size)); }
        public virtual eNetSendResult SendMessage(int message_type, JNetData netdata, params dynamic[] args) { return SendMessage(JNetMessage.Make(message_type, netdata, args)); }
        public virtual eNetSendResult SendMessage(int message_type, string result, JNetData netdata, params dynamic[] args) { return SendMessage(JNetMessage.Make(message_type, netdata, args).SetResult(result)); }
        public virtual eNetSendResult SendMessage(int message_type, Action<BinaryWriter> fun_write) { return SendMessage(JNetMessage.Make(message_type, fun_write)); }
        #endregion

        #region [�۽�] [Simple] ������ ������
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual eNetSendResult SendSimpleMessage(string message_type, params dynamic[] args) { return SendMessage(JNetMessage.MakeSimple(message_type, args)); }
        public virtual eNetSendResult SendSimpleMessage(string message_type, byte[] data_buffer, int data_size) { return SendMessage(JNetMessage.MakeSimple(message_type, data_buffer, data_size)); }
        public virtual eNetSendResult SendSimpleMessage(string message_type, JNetData netdata, params dynamic[] args) { return SendMessage(JNetMessage.MakeSimple(message_type, netdata, args)); }
        public virtual eNetSendResult SendSimpleMessage(string message_type, string result, JNetData netdata, params dynamic[] args) { return SendMessage(JNetMessage.MakeSimple(message_type, netdata, args).SetResult(result)); }
        public virtual eNetSendResult SendSimpleMessage(string message_type, Action<BinaryWriter> fun_write) { return SendMessage(JNetMessage.MakeSimple(message_type, fun_write)); }
        #endregion

        #region [�۽�] [Short] ������ ������
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual eNetSendResult SendShortMessage(int message_type, params dynamic[] args) { return SendMessage(JNetMessage.MakeShort(message_type, args)); }
        public virtual eNetSendResult SendShortMessage(int message_type, byte[] data_buffer, int data_size) { return SendMessage(JNetMessage.MakeShort(message_type, data_buffer, data_size)); }
        public virtual eNetSendResult SendShortMessage(int message_type, JNetData netdata, params dynamic[] args) { return SendMessage(JNetMessage.MakeShort(message_type, netdata, args)); }
        public virtual eNetSendResult SendShortMessage(int message_type, string result, JNetData netdata, params dynamic[] args) { return SendMessage(JNetMessage.MakeShort(message_type, netdata, args).SetResult(result)); }
        public virtual eNetSendResult SendShortMessage(int message_type, Action<BinaryWriter> fun_write) { return SendMessage(JNetMessage.MakeShort(message_type, fun_write)); }
        #endregion


        #region [�۽�] JNetMessage
        //------------------------------------------------------------------------------------------------------------------------------------------------------		
        public virtual eNetSendResult SendMessage(JNetMessage message)
        {
            if (_sender_channel == null)
                return eNetSendResult.UnknownError;
            return _sender_channel.SendMessage(message);
        }
        #endregion

        #region [�۽�] Broadcast
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // ����: �޽��� ��Ȱ�� �ʿ�(NetBuffer.MemoryStream�� ��� ��Ƽ�����忡�� ���ư��� ������ �ܼ� ����Ʈ�� �����ؼ��� �ȵ� ��)
        //
        public virtual void Broadcast(JNetMessage message, NetPeer_base except = null)
		{
            foreach (var c in m_connections)
            {
                if (c != except)
                    c.SendMessage(message);
            }
		}
        #endregion

        #region [�۽�] ���̳ʸ� ��뷮 ������(����) ���� (10 MB ���ϴ� �Ϲ� �޽����� ����)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public long SendFile(string filepath)
        {
            return _sender_channel.SendFile(filepath);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public long SendBigData(byte[] send_data)
        {
            return _sender_channel.SendBigData(send_data);
        }
        #endregion

        #region [�۽�] �޽��� �ش� ���� �����͸� ���� ������ ��찡 ����
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual eNetSendResult SendRawData(Action<BinaryWriter> fun_write)
        {
            if (_sender_channel == null)
                return eNetSendResult.UnknownError;
            return _sender_channel.SendRawData(fun_write);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual eNetSendResult SendRawData(byte[] rawdata)
        {
            return SendRawData((writer) => { writer.Write(rawdata); });
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual eNetSendResult SendRawData(byte[] rawdata, int offset, int count)
        {
            return SendRawData((writer) => { writer.Write(rawdata, offset, count); });
        }
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 3. �޽��� ����
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [����] �޽��� �ڵ鷯 ���
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void RegisterMessageHandler(int id, Action<NetPeer_base, BinaryReader> handler, bool overwrite = false)
        {
            if (null == _message_dispatcher)
                return;
            _message_dispatcher.RegisterMessageHandler(id, handler, overwrite);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void RegisterSimpleMessageHandler(string id, Action<NetPeer_base, BinaryReader> handler, bool overwrite = false)
        {
            if (null == _message_dispatcher)
                return;
            _message_dispatcher.RegisterSimpleMessageHandler(id, handler, overwrite);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void RegisterShortMessageHandler(int id, Action<NetPeer_base, BinaryReader> handler, bool overwrite = false)
        {
            if (null == _message_dispatcher)
                return;
            _message_dispatcher.RegisterShortMessageHandler(id, handler, overwrite);
        }
        #endregion

        #region [����] [�񵿱�] aysnc, await
        //---------------------------------------------------------------------------------------------------------------------------------
        public async Task<(NetPeer_base, BinaryReader)> WaitMessage(int message_type, bool overwrite = true)
		{
            if (null == _message_dispatcher)
                return (null, null);
            return await _message_dispatcher.WaitMessage(message_type, overwrite);
		}
        //---------------------------------------------------------------------------------------------------------------------------------
        public async Task<(NetPeer_base, BinaryReader)> WaitSimpleMessage(string message_type, bool overwrite = true)
        {
            if (null == _message_dispatcher)
                return (null, null);
            return await _message_dispatcher.WaitSimpleMessage(message_type, overwrite);
        }
        #endregion

        #region [����] ���̳ʸ� ��뷮 ������(����)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual bool OnRecv_FileTransfer(BinaryReader reader)
        {
            return _recv_channel.OnRecv_FileTransfer(reader);
        }
        public byte[] GetFileTransferData(long file_guid, bool removeFromRepository = true)
        {
            return _recv_channel.GetFileTransferData(file_guid, removeFromRepository);
        }
        public bool SaveFileTransferData(long file_guid, string filepath, bool removeFromRepository = true)
        {
            return _recv_channel.SaveFileTransferData(file_guid, filepath, removeFromRepository);
        }
        #endregion

        #region [MessageDispatcher] Set
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetMessageDispatcher(NetMessage_dispatcher_base dispatcher)
        {
            _message_dispatcher = dispatcher;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 4. [���ξ�����] Commands
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [���ξ�����] [Commands] �߰�
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddMainThreadCommand(Action fun)
		{
			_commands_main_thred.Enqueue(fun);
		}
		#endregion

		#region [���ξ�����] [Commands] ����
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		protected void execute_mainThreadCommands()
		{
			while(_commands_main_thred.Count > 0)
			{
				if(_commands_main_thred.TryDequeue(out Action command))
					command();
			}			
		}
		#endregion


        

       

		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// ���
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


		#region [���] [NetStream] [WorkThread] RequestAsyncWorks
		////------------------------------------------------------------------------------------------------------------------------------------------------------
		//public void RequestAsyncWorks()
		//{
		//	//Task.Run(async () =>
		//	//{
		//	//	NetBase.AddThreadID(Thread.CurrentThread.ManagedThreadId);												
		//	//	await _sender_channel.RequestWriteAsync();				
		//	//});

		//	//Task.Run(async () =>
		//	//{
		//	//	NetBase.AddThreadID(Thread.CurrentThread.ManagedThreadId);
		//	//	await _recv_channel.RequestReadAsync();
		//	//});

		//	//var task_write = _sender_channel.RequestWriteAsync();
		//	//var task_read = _recv_channel.RequestReadAsync();

		//}
		#endregion
	}

}
