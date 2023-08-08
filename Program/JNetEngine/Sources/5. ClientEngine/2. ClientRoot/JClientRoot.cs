using UnityEngine;
using System;
using System.Collections.Generic;
using J2y.Network;
using System.IO;
using System.Threading.Tasks;

namespace J2y
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // LucaServerRoot_base
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JClientRoot : JEngineRoot
    {
        public new static JClientRoot Instance;

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Variable

        public NetTcpClient[] _tcp_connectors;
        public NetTcpClient _default_tcp_connector;

        #endregion

        #region [Variable] Custom Events
        public Action CustomStart;
        public Action CustomUpdate;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Init
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] Awake
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Awake()
        {
            base.Awake();
            Instance = this;
        }
        #endregion
        
        #region [업데이트] Update
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Update()
        {
            base.Update();
            if(_tcp_connectors != null)
                _tcp_connectors.ForEach(c => c?.Update());
        }
        #endregion

        #region [업데이트] [Test] RunLoop
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void TestRunLoop(bool thread_run = false)
        {
            if (thread_run)
            {
                Task.Run(() => run_loop_impl());
            }
            else
            {
                run_loop_impl();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        protected virtual void run_loop_impl()
        {
            CustomStart?.Invoke();

            while (true)
            {
                var start_time = JTimer.GetCurrentTime();
                JScheduler.Instance.Update();
                UpdateInternal();

                CustomUpdate?.Invoke();
                JTimer.SleepIdleTime(start_time, 5);
            }
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // RPC
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //#region [RPC] 요청
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public override void NetSendRPC(int rpc_type, JObject caller, JObject receiver, string fun_name, params dynamic[] args)
        //{
        //    var net_message = JRpcMediator.MakeMessage(rpc_type, caller, fun_name, args);

        //    switch (rpc_type)
        //    {
        //        case eRpcTarget.Client:
        //            {
        //                if (null == caller)
        //                    return;
        //                JRpcMediator.MethodInvoke(caller, fun_name, args);                        
        //            }
        //            return;
        //        case eRpcTarget.Server:
        //            {
        //                NetSendMessage(net_message);
        //            }
        //            return;
        //        case eRpcTarget.Multicast:
        //            {
        //                // todo: 서버에 전파를 요청
        //                NetBroadcast(net_message);
        //            }
        //            return;
        //    }

        //    base.NetSendRPC(rpc_type, caller, receiver, fun_name, args);
        //}
        //#endregion

        #region [RpcMediator] 생성
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void MakeRpcMediator()
        {
            _rpc_mediator = new JRpcMediator_Client();
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [네트워크] 클라이언트
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [네트워크] [클라이언트] 연결
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual bool Connect(eNetPeerType server_type, string server_ip, int port, Action<int> fun_connected = null)
        {
            if (null == _tcp_connectors)
            {
                var max = (int)eNetPeerType.Max;
                _tcp_connectors = new NetTcpClient[max];
                for (int i = 0; i < max; ++i)
                    _tcp_connectors[i] = new NetTcpClient();
            }

            var connector = _tcp_connectors[(int)server_type];

            if (null == _default_tcp_connector)
                _default_tcp_connector = connector;

            return connector.Connect(server_ip, port, fun_connected);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual bool Connect(string server_ip, int port, Action<int> fun_connected = null)
        {
            return Connect(eNetPeerType.MainServer, server_ip, port, fun_connected);
        }
        #endregion

        #region [네트워크] [클라이언트] 해제
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void DisconnectServer(eNetPeerType server_type)
        {
            if (null == _tcp_connectors)
                return;
            var connector = _tcp_connectors[(int)server_type];
            if (_default_tcp_connector == connector)
                _default_tcp_connector = null;

            if (null != connector)
                connector.Disconnect();
            _tcp_connectors[(int)server_type] = null;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void DisconnectServers()
        {
            _tcp_connectors.ForEach(c => c?.Disconnect());
            _tcp_connectors = null;
            _default_tcp_connector = null;
        }
        #endregion

        #region [네트워크] [커넥터] 찾기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual NetTcpClient GetConnector(eNetPeerType server_type = eNetPeerType.MainServer)
        {
            if (null == _tcp_connectors || server_type == eNetPeerType.Unknown)
                return null;
            return _tcp_connectors[(int)server_type];
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [네트워크] [송신]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [네트워크] [송신] 데이터 보내기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override eNetSendResult NetSendMessage(eNetPeerType server_type, JNetMessage message)
        {
            var connector = GetConnector(server_type);
            if (null == connector)
                return eNetSendResult.FailedNotConnected;
            return connector.SendMessage(message);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override eNetSendResult NetSendMessage(JNetMessage message)
        {
            if (null == _default_tcp_connector)
                return eNetSendResult.FailedNotConnected;
            return _default_tcp_connector.SendMessage(message);
        }

        #endregion

        #region [네트워크] 메시지핸들러 등록
        public override void RegisterMessageHandler(eNetPeerType server_type, int id, Action<NetPeer_base, BinaryReader> handler)
        {
            GetConnector(server_type)?.RegisterMessageHandler(id, handler);
        }
        #endregion

        #region [백업] [네트워크] [송신] [Simple] 데이터 보내기
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public virtual eNetSendResult NetSendMessage(int message_type, params dynamic[] args) { return NetSendMessage(JNetMessage.Make(message_type, args)); }
        //public virtual eNetSendResult NetSendMessage(int message_type, byte[] data_buffer, int data_size, bool immediate = true) { return NetSendMessage(JNetMessage.Make(message_type, data_buffer, data_size, immediate)); }
        //public virtual eNetSendResult NetSendMessage(int message_type, JNetData_base netdata, bool immediate = true, params dynamic[] args) { return NetSendMessage(JNetMessage.Make(message_type, netdata, immediate, args)); }
        //public virtual eNetSendResult NetSendMessage(int message_type, string result, JNetData_base netdata, bool immediate = true, params dynamic[] args) { return NetSendMessage(JNetMessage.Make(message_type, result, netdata, immediate, args)); }
        //public virtual eNetSendResult NetSendMessage(int message_type, Action<BinaryWriter> fun_write, bool immediate = true) { return NetSendMessage(JNetMessage.Make(message_type, fun_write, immediate)); }
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public virtual eNetSendResult NetSendSimpleMessage(string message_type, params dynamic[] args) { return NetSendMessage(JNetMessage.MakeSimple(message_type, args)); }
        //public virtual eNetSendResult NetSendSimpleMessage(string message_type, byte[] data_buffer, int data_size, bool immediate = true) { return NetSendMessage(JNetMessage.MakeSimple(message_type, data_buffer, data_size, immediate)); }
        //public virtual eNetSendResult NetSendSimpleMessage(string message_type, JNetData_base netdata, bool immediate = true, params dynamic[] args) { return NetSendMessage(JNetMessage.MakeSimple(message_type, netdata, immediate, args)); }
        //public virtual eNetSendResult NetSendSimpleMessage(string message_type, string result, JNetData_base netdata, bool immediate = true, params dynamic[] args) { return NetSendMessage(JNetMessage.MakeSimple(message_type, result, netdata, immediate, args)); }
        //public virtual eNetSendResult NetSendSimpleMessage(string message_type, Action<BinaryWriter> fun_write, bool immediate = true) { return NetSendMessage(JNetMessage.MakeSimple(message_type, fun_write, immediate)); }
        #endregion

        #region [네트워크] [송신] Broadcast
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void NetBroadcast(eNetPeerType server_type, JNetMessage message, NetPeer_base except = null)
        {
            var connector = GetConnector(server_type);
            if (null == connector)
                return;
            connector.SendMessage(message);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void NetBroadcast(JNetMessage message, NetPeer_base except = null)
        {
            _default_tcp_connector.SendMessage(message);
        }
        #endregion
    }

}
