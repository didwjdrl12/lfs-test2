using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NetEndPoint = System.Net.IPEndPoint;

namespace J2y.Network
{
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// NetUdpPeer_base
	//
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public partial class NetUdpPeer_base : NetPeer_base
	{
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Variable
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


		#region [Variable] 네트워크

		public UdpClient _net_client;


        #endregion

        #region [Property] [Events] OnConnected/OnDisconnected

        public event Action OnEvent_ServerConnected;
        public event Action OnEvent_ServerDisconnected;

        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public NetUdpPeer_base()
		{			
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

			if (_net_client != null)
				_net_client.Close();
			_net_client = null;
		}
		#endregion

		#region [Update] 
		////------------------------------------------------------------------------------------------------------------------------------------------------------
		//public override void Update()
		//{
		//	base.Update();
		//}
		#endregion







		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 네트워크
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [네트워크] Init
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public void initialize()
		{			
			_sender_channel = new NetUdpSenderChannel(this);
			_recv_channel = new NetUdpReceiverChannel(this);

			Task.Run(() => _sender_channel.WriteLoop());
			Task.Run(() => _recv_channel.ReadLoop());
		}
        #endregion


        #region [네트워크] 메시지 전송
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public void SendMessage(IPEndPoint endpoint, JUdpMessage udp_msg)
        //{
        //	var datagram = udp_msg.ToBinaryData();
        //	_net_udp_client.Send(datagram, datagram.Length, endpoint);
        //}
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
            if (_sender_channel != null)
                _sender_channel.Stop();
            if (_recv_channel != null)
                _recv_channel.Stop();

            _recv_channel = null;
            _sender_channel = null;

            OnEvent_ServerDisconnected?.Invoke();
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 백업
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    }

}
