using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace J2y.Network
{
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// NetUdpClient
	//
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public class NetUdpClient : NetUdpPeer_base
	{
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region Variable

        #endregion

      



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public NetUdpClient()
		{
			_net_client = new UdpClient();
		}
		#endregion



		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 네트워크
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [네트워크] 연결
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public bool ConnectTo(string hostname, int port)
		{
			try
			{
				_net_client.Connect(IPAddress.Parse(hostname), port);
				initialize();
				OnServerConnected();
				return true;
			}
			catch (SocketException e)
			{
				JLogger.WriteFormat(string.Format("Connect Fail:{0}, {1}", hostname, e.Message));
				return false;
			}
		}
        #endregion
        


    }

}
