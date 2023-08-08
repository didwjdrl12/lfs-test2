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
    // JRPCMediator
    //      
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JRpcMediator
    {
        public static JRpcMediator Instance;

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 상수, Static
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [상수] 
        #endregion

        #region [property]
        public virtual int _this_rpc_target => eRpcTarget.None;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. [RPC] [NetMessage] 요청
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [RPC] [NetMessage] Make        
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JNetMessage MakeMessage(JReplication replication)
        {
            if(replication._type == eReplicationType.Function)
            {
                var net_message = JNetMessage.Make(JNetMessageProtocol.RPC, (writer) =>
                {
                    writer.Write(replication._rpc_target);
                    writer.Write(replication._caller._guid);
                    writer.Write((int)replication._type);
                    writer.Write(replication._fun_name);
                    JNewSerialization.Serialize(writer, replication._args);
                });

                return net_message;
            }
            else if (replication._type == eReplicationType.Actor)
            {
                var net_message = JNetMessage.Make(JNetMessageProtocol.RPC, (writer) =>
                {
                    writer.Write(replication._rpc_target);
                    writer.Write(replication._caller._guid);
                    writer.Write((int)replication._type);
                    writer.Write(replication._src_actor._asset_name);
                    replication._src_actor.Serialize(writer);
                });

                return net_message;
            }

            return null;               
        }
        #endregion

        #region [RPC] 네트워크 전송
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void NetSendRPC(int rpc_target, JObject caller, JObject receiver, string fun_name, params dynamic[] args)
        {
            JEngineRoot.Instance.NetSendRPC(rpc_target, caller, receiver, fun_name, args);
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 2. [RPC] 수신
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [RPC] 요청 받음 (네트워크 수신)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void OnNetRecvRPC(NetPeer_base sender, BinaryReader reader)
        {
            //-------------------------------------------------------------------------------------
            // 1. 기본 RPC 속성
            //
            var rpc_target = reader.ReadInt32();
            var obj_guid = reader.ReadInt64();
            var rep_type = (eReplicationType)reader.ReadInt32();
            var obj = JObjectManager.Find(obj_guid);
            if (null == obj)
                return;

            obj._replication_helper._net_sender = sender;


            //-------------------------------------------------------------------------------------
            // 2. RPC 함수 호출
            //
            var fun_name = "";
            object[] args = null;
            JActor actor = null;

            if (rep_type == eReplicationType.Function)
            {
                //-------------------------------------------------------------------------------------
                // 2.1. 파라미터 추출
                //
                fun_name = reader.ReadString();
                args = JNewSerialization.Deserialize(reader) as object[];


                //-------------------------------------------------------------------------------------
                // 2.2. RPC 호출
                //
                obj._replication_helper._rpc_func_name = fun_name;
                obj.InvokeMethod(fun_name, args);
            }
            //-------------------------------------------------------------------------------------
            // 3. 액터 스폰
            //
            else if (rep_type == eReplicationType.Actor)
            {
                var asset_name = reader.ReadString();
                actor = JActor.SpawnActor(asset_name);
                actor.Deserialize(reader);
                obj.OnSpwanActorInternal(actor);
            }


            //-------------------------------------------------------------------------------------
            // 3. Multicast의 경우 전파
            //
            
            if ((Instance._this_rpc_target == eRpcTarget.Client) 
                || (JBitMask.Compare(rpc_target, eRpcTarget.Multicast) == false))
                return;
            var except_self = JBitMask.Compare(rpc_target, eRpcTarget.ExceptSelf) ? sender : null;


            if (rep_type == eReplicationType.Function)
            {
                var net_message = JNetMessage.Make(JNetMessageProtocol.RPC, (writer) => 
                {
                    writer.Write(rpc_target);
                    writer.Write(obj_guid);
                    writer.Write((int)rep_type);
                    writer.Write(fun_name);
                    JNewSerialization.Serialize(writer, args);
                });

                obj.NetBroadcast(net_message, except_self);
            }
            else if (rep_type == eReplicationType.Actor)
            {
                var net_message = JNetMessage.Make(JNetMessageProtocol.RPC, (writer) =>
                {
                    writer.Write(rpc_target);
                    writer.Write(obj_guid);
                    writer.Write((int)rep_type);
                    writer.Write(actor._asset_name);
                    actor.Serialize(writer);
                });

                obj.NetBroadcast(net_message, except_self);
            }


            // fixme : Message 송/수신 관련 동일 코드 산재. 관련 내용 한번 감싸줄 필요성 있음...?
        }
        #endregion

        #region [RPC] 요청 받음 (네트워크 답신)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void OnNetRecvRPCResponse(NetPeer_base sender, BinaryReader reader)
        {
            var obj_guid = reader.ReadInt64();
            var obj = JObjectManager.Find(obj_guid);
            if (null == obj)
                return;

            var fun_name = reader.ReadString();

            if (obj._rpc_response_handlers.ContainsKey(fun_name))
                obj._rpc_response_handlers[fun_name](reader);
        }
        #endregion

        #region [RPC] 함수 호출
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void MethodInvoke(JObject obj, string fun_name, params object[] args)
        {
            if (null == obj)
                return;
            obj.InvokeMethod(fun_name, args);
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 2. [Replication]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Replication] [RPC(Function)]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual JRpcCallResult RpcRequest(NetTcpClient tcp_client, JReplication replication)
        {
            if (null == replication._net_receiver)
                return new JRpcCallResult(eRpcResult.WrongArguments);

            // 1. 메시지 전송
            var net_message = MakeMessage(replication);
            tcp_client.SendMessage(net_message);

            // 2. 결과 리턴
            var result = new JRpcCallResult(eRpcResult.Succeed);
            result._caller = replication._caller;
            result._receiver = replication._net_receiver;
            result._fun_name = replication._fun_name;
            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual JRpcCallResult RpcRequest(JReplication replication)
        {
            return new JRpcCallResult(eRpcResult.NoAuthority);
        }
        #endregion
        
        #region [Replication] [Actor]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual JActorReplicationResult ActorReplication(NetTcpClient tcp_client, JReplication replication)
        {
            if (null == replication._net_receiver)
                return new JActorReplicationResult(eRpcResult.WrongArguments);

            // 1. 메시지 전송
            var net_message = MakeMessage(replication);
            tcp_client.SendMessage(net_message);

            // 2. 결과 리턴
            var result = new JActorReplicationResult(eRpcResult.Succeed)
            {
                _caller = replication._caller,
                _receiver = replication._net_receiver
            };
            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual JActorReplicationResult ActorReplication(JReplication replication)
        {
            return new JActorReplicationResult(eRpcResult.NoAuthority);
        }
        #endregion
        


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 백업
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [백업] [RPC] [유틸] 파라미터 추출
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static object ReadArgument(Type type, BinaryReader reader)
        //{
        //    object value;

        //    var primitive_type = JReplication.HasReadMethod(type);
        //    if (primitive_type)
        //    {
        //        JReplication.Read(reader, type, out value);                
        //    }
        //    // NetData, List, Dictionary
        //    else
        //    {
        //        //var is_netdata = type.IsClass && typeof(JNetData).IsSubclassOf(type);

        //        var netdata = Activator.CreateInstance(type) as JNetData;
        //        value = JSerialization.Deserialize(reader);
        //    }

        //    return value;            
        //}
        #endregion

        #region [백업] [RPC] [유틸] 파라미터 Serialization
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static object WriteArgument(object value, BinaryWriter writer)
        //{
        //    var type = value.GetType();
        //    var primitive_type = JReplication.HasWriteMethod(type);

        //    if (primitive_type)
        //    {
        //        JReplication.Write(writer, type, value);
        //    }
        //    // NetData, List, Dictionary
        //    else
        //    {
        //        //var is_netdata = type.IsClass && typeof(JNetData).IsSubclassOf(type);

        //        var netdata = Activator.CreateInstance(type) as JNetData;
        //        JSerialization.Serialize(writer, value);
        //    }

        //    return value;
        //}
        #endregion
   
    }
}
