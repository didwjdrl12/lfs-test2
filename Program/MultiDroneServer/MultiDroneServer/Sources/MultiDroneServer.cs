using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using J2y;
using J2y.Network;



namespace J2y.MultiDroneUnity
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // VosRoot
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class MultiDroneServer : JObject
    {
        public static MultiDroneServer Instance;

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ����
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [����] Base
        public bool _running = true;
        #endregion


        #region [����] Manager
        public MdusNetServer _net_server;
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // �ʱ�ȭ
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [�ʱ�ȭ] Awake
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Awake()
        {
            Instance = this;
        }
        #endregion

        #region [�ʱ�ȭ] Start
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Start()
        {
            _net_server = new MdusNetServer();
            _net_server.StartServer(MdusBase._launcher_server_port);

            Console.WriteLine("Launcher Server Connected.");

            //_net_server.OnClientConnected

            MdusNetMessageDispatcher.RegisterMessageHandlers(_net_server);
        }
        #endregion

        #region [����]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void OnDestory()
        {
            _net_server.DisconnectAll();
            _net_server = null;
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ������Ʈ
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [������Ʈ]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Update()
        {
            _net_server.Update();

            #region [Test]

            //if (Console.KeyAvailable)
            //{
            //    var keyinfo = Console.ReadKey(true);
            //    int idx = 0;
            //    switch (keyinfo.Key)
            //    {
            //        case ConsoleKey.S:
            //            //Console.WriteLine("Start!");

            //            break;
            //        case ConsoleKey.A:
            //            //var user = MdusDataCenter.NetClient_Users[0];
            //            //user.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.NOTI_USER_ENTER, user.NetData));
            //            break;
            //        case ConsoleKey.D0:
            //            idx = 0;
            //            var net_client = JObjectManager.Create<MdusNetClient_User>();
            //            net_client.NetData = new MdNetData_User();
            //            net_client.NetData._guid = idx;
            //            net_client.NetData._vehicle_type = eVehicleType.Multirotor;
            //            MdusDataCenter.NetClient_Users[idx] = net_client;
            //            break;
            //        case ConsoleKey.D1:
            //            idx = 1;
            //            var net_client1= JObjectManager.Create<MdusNetClient_User>();
            //            net_client1.NetData = new MdNetData_User();
            //            net_client1.NetData._guid = idx;
            //            net_client1.NetData._vehicle_type = eVehicleType.Multirotor;
            //            MdusDataCenter.NetClient_Users[idx] = net_client1;
            //            break;
            //        case ConsoleKey.D2:
            //            idx = 2;
            //            var net_client2 = JObjectManager.Create<MdusNetClient_User>();
            //            net_client2.NetData = new MdNetData_User();
            //            net_client2.NetData._guid = idx;
            //            net_client2.NetData._vehicle_type = eVehicleType.Multirotor;
            //            MdusDataCenter.NetClient_Users[idx] = net_client2;
            //            break;
            //    }
            //}
            #endregion
        }
        #endregion

    }

}
