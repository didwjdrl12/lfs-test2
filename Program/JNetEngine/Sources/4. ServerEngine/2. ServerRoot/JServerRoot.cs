using UnityEngine;
using System;
using System.Collections.Generic;
using J2y.Network;
using System.Threading.Tasks;
using System.IO;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JServerRoot
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JServerRoot : JEngineRoot
    {
        public new static JServerRoot Instance;

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] 서버
        public NetTcpServer _net_server;
		public bool _server_running;
		#endregion

		#region [Variable] Timer
		protected long _startTick;
		protected long _serverTick;
		protected float _deltaTime;
		protected float _currentTime;
		#endregion

		#region [Variable] Custom Events
		public Action CustomStart;
        public Action CustomUpdate;
		#endregion



		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Property
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [Property] State
		public float DeltaTime => _deltaTime;
		public float Time => _currentTime;
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
            _server_running = true;
			_startTick = _serverTick = JTimer.GetCurrentTick();

			CreateManagers();
        }
        #endregion

        #region [Init] 1. 매니저들 Init
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void CreateManagers()
        {            
        }
        #endregion


        #region [종료] OnDestroy
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void OnDestroy()
        {
            StopServer();
            base.OnDestroy();         
        }
        #endregion

        #region [업데이트] Update
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Update()
        {
            base.Update();
            _net_server?.Update();

			updateTimer();
		}
		#endregion

		#region [Update] Timer
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		private void updateTimer()
		{
			_currentTime = JTimer.GetElaspedTime(_startTick);
			_deltaTime = JTimer.GetElaspedTime(_serverTick);
			_serverTick = JTimer.GetCurrentTick();
		}
		#endregion


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 서버
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [서버] 시작
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual void StartServer(int server_port)
        {
            if (null == _net_server)
                _net_server = new NetTcpServer();
            _net_server.StartServer(server_port);
        }
        #endregion

        #region [서버] 종료
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void StopServer()
        {
            if(_net_server != null)
                _net_server.StopServer();
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 업데이트
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [업데이트] + 예외처리
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void UpdateInternal()
        {
            if (JBase._final_release)
            {
                try
                {
                    base.UpdateInternal();
                    JScheduler.Instance.Update();
                }
                catch (Exception e)
                {
                    Console.WriteLine("[ServerRoot]" + e.Message);
                    // DB 기록시 Exception 생기는 듯....
                    // MdsSystem_gameLog.System_field_exception(e.Message);
                }
            }
            else
            {
                base.UpdateInternal();
                JScheduler.Instance.Update();
            }
        }
        #endregion

        #region [업데이트] RunLoop
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void RunLoop(bool thread_run = false)
        {
            if(thread_run)
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

            while (_server_running)
            {
                var start_time = JTimer.GetCurrentTime();
                UpdateInternal();

                CustomUpdate?.Invoke();
                JTimer.SleepIdleTime(start_time, 1);
            }
        }
        #endregion


         




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // RPC
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //#region [RPC] 요청
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public override void NetSendRPC(eRpcTarget rpc_type, JObject caller, JObject receiver, string fun_name, params dynamic[] args)
        //{
        //    switch (rpc_type)
        //    {
        //        case eRpcTarget.Client: break;
        //        case eRpcTarget.Server:
        //            {
        //                if (null == caller)
        //                    return;
        //                JRpcMediator.MethodInvoke(caller, fun_name, args);
        //            }
        //            return;
        //        case eRpcTarget.Multicast: break;
        //    }

        //    base.NetSendRPC(rpc_type, caller, receiver, fun_name, args);
        //}
        //#endregion

        #region [RpcMediator] 생성
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void MakeRpcMediator()
        {
            _rpc_mediator = new JRpcMediator_Server();
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [네트워크] [송신]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Network] 해당 객체(유저 또는 유닛)에 메시지 전송
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override eNetSendResult NetSendMessage(JNetMessage message)
        {
            // 디폴트는 모두에게 전파 (하위 클래스에서 실구현)
            NetBroadcast(message);
            return eNetSendResult.Sent;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override eNetSendResult NetSendMessage(eNetPeerType peer_type, JNetMessage message)
        {
            // 디폴트는 모두에게 전파 (하위 클래스에서 실구현)
            NetBroadcast(peer_type, message);
            return eNetSendResult.Sent;
        }
        #endregion
        
        #region [네트워크] [송신] Broadcast
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void NetBroadcast(eNetPeerType peer_type, JNetMessage message, NetPeer_base except = null)
        {
            NetBroadcast(message, except); // 디폴트는 서버든 클라든 모두에게 전파
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void NetBroadcast(JNetMessage message, NetPeer_base except = null)
        {
            if (null == _net_server)
                return;
            _net_server.Broadcast(message, except);
        }
        #endregion

    }

}
