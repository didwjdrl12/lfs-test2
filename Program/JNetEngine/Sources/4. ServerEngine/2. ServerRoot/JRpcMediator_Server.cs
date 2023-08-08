using J2y.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;



namespace J2y
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JRpcMediator_Server
    //      
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JRpcMediator_Server : JRpcMediator
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] 
        #endregion

        #region [property]
        public override int _this_rpc_target => eRpcTarget.Server;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Init
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JRpcMediator_Server()
        {
            Instance = this;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // RPC
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [RPC] 요청
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override JRpcCallResult RpcRequest(JReplication replication)
        {
            if (null == replication._net_receiver)
                return new JRpcCallResult(eRpcResult.WrongArguments);

            // 1. 서버에서 먼저 실행
            var rpc_target = replication._rpc_target;
            if (!JBitMask.Compare(rpc_target, eRpcTarget.ExceptSelf))
                replication._caller.InvokeMethod(replication._fun_name, replication._args);
            

            // 2. 메시지 전송
            var net_message = MakeMessage(replication);
            var net_peer_type = JUtil.ToNetPeerType(rpc_target);
            if (net_peer_type == eNetPeerType.Unknown)
                return new JRpcCallResult(eRpcResult.NoAuthority);
            else
            {
                if (JBitMask.Compare(replication._rpc_target, eRpcTarget.Multicast))
                    replication._net_receiver.NetBroadcast(net_peer_type, net_message);
                else
                    replication._net_receiver.NetSendMessage(net_peer_type, net_message);
            }

            // 3. 결과 리턴
            var rpc_call_result = new JRpcCallResult(eRpcResult.Succeed)
            {
                _caller = replication._caller,
                _receiver = replication._net_receiver,
                _fun_name = replication._fun_name
            };
            return rpc_call_result;
        }
        #endregion


        #region [Replication] [Actor] 요청
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override JActorReplicationResult ActorReplication(JReplication replication)
        {
            if (null == replication._net_receiver)
                return new JActorReplicationResult(eRpcResult.WrongArguments);

            // 1. 메시지 생성
            var rpc_target = replication._rpc_target;
            var net_message = MakeMessage(replication);


            // 2. 메시지 전송
            var net_target = JUtil.ToNetPeerType(rpc_target);
            if (net_target == eNetPeerType.Unknown)
                return new JActorReplicationResult(eRpcResult.NoAuthority);
            else
            {
                if (JBitMask.Compare(rpc_target, eRpcTarget.Multicast))
                    replication._net_receiver.NetBroadcast(net_target, net_message);
                else if (!JBitMask.Compare(rpc_target, eRpcTarget.Client))
                    replication._net_receiver.NetSendMessage(net_target, net_message);
            }
            
            // 3. 결과 리턴
            var actor_rpc_result = new JActorReplicationResult(eRpcResult.Succeed)
            {
                _caller = replication._caller,
                _receiver = replication._net_receiver
            };
            return actor_rpc_result;
        }
        #endregion





        //#region [RPC] 네트워크 전송
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public override void NetSendRPC(eRpcTarget rpc_type, JObject caller, JObject receiver, string fun_name, params dynamic[] args)
        //{
        //    //JEngineRoot.Instance.NetSendRPC(rpc_type, caller, receiver, fun_name, args);
        //}
        //#endregion



  
    }


}
