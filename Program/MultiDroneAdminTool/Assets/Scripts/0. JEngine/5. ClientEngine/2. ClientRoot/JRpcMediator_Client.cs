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
    // JRpcMediator_Client
    //      
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JRpcMediator_Client : JRpcMediator
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] 
        #endregion

        #region [property]
        public override int _this_rpc_target => eRpcTarget.Client;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Init
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] ������
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JRpcMediator_Client()
        {
            Instance = this;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // RPC
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [RPC] ��û
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override JRpcCallResult RpcRequest(JReplication replication)
        {
            if (null == replication._net_receiver)
                return new JRpcCallResult(eRpcResult.WrongArguments);

            // 1. Ŭ�󿡼� ���� ����
            var rpc_target = replication._rpc_target;
            if (!JBitMask.Compare(rpc_target, eRpcTarget.ExceptSelf) && !JBitMask.Compare(rpc_target, eRpcTarget.Multicast)) // Multicast�� ���� �޽��� ���Ž� ȣ��ȴ�.
                replication._caller.InvokeMethod(replication._fun_name, replication._args);


            // 2. �޽��� ����
            var net_message = MakeMessage(replication);
            var net_target = JUtil.ToNetPeerType(rpc_target);
            if(net_target == eNetPeerType.Unknown)
                return new JRpcCallResult(eRpcResult.NoAuthority);
            else
                replication._net_receiver.NetSendMessage(net_target, net_message);
            
            // 3. ��� ����
            var rpc_call_result = new JRpcCallResult(eRpcResult.Succeed)
            {
                _caller = replication._caller,
                _receiver = replication._net_receiver,
                _fun_name = replication._fun_name
            };
            return rpc_call_result;
        }
        #endregion


        #region [Replication] [Actor] ��û
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override JActorReplicationResult ActorReplication(JReplication replication)
        {
            if (null == replication._net_receiver)
                return new JActorReplicationResult(eRpcResult.WrongArguments);

            // 1. �޽��� ����
            var rpc_target = replication._rpc_target;
            var net_message = MakeMessage(replication);

            // 2. �޽��� ����
            var net_target = JUtil.ToNetPeerType(rpc_target);
            if (net_target == eNetPeerType.Unknown)
                return new JActorReplicationResult(eRpcResult.NoAuthority);
            else
                replication._net_receiver.NetSendMessage(net_target, net_message);

            // 3. ��� ����
            var actor_rep_result = new JActorReplicationResult(eRpcResult.Succeed)
            {
                _caller = replication._caller,
                _receiver = replication._net_receiver
            };
            return actor_rep_result;
        }
        #endregion





        //#region [RPC] ��Ʈ��ũ ����
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public override void NetSendRPC(eRpcTarget rpc_type, JObject caller, JObject receiver, string fun_name, params dynamic[] args)
        //{
        //    //JEngineRoot.Instance.NetSendRPC(rpc_type, caller, receiver, fun_name, args);
        //}
        //#endregion

    }


}
