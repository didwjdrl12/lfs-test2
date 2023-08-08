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
        // ���, Static
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [���] 
        #endregion

        #region [property]
        public virtual int _this_rpc_target => eRpcTarget.None;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. [RPC] [NetMessage] ��û
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

        #region [RPC] ��Ʈ��ũ ����
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void NetSendRPC(int rpc_target, JObject caller, JObject receiver, string fun_name, params dynamic[] args)
        {
            JEngineRoot.Instance.NetSendRPC(rpc_target, caller, receiver, fun_name, args);
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 2. [RPC] ����
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [RPC] ��û ���� (��Ʈ��ũ ����)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void OnNetRecvRPC(NetPeer_base sender, BinaryReader reader)
        {
            //-------------------------------------------------------------------------------------
            // 1. �⺻ RPC �Ӽ�
            //
            var rpc_target = reader.ReadInt32();
            var obj_guid = reader.ReadInt64();
            var rep_type = (eReplicationType)reader.ReadInt32();
            var obj = JObjectManager.Find(obj_guid);
            if (null == obj)
                return;

            obj._replication_helper._net_sender = sender;


            //-------------------------------------------------------------------------------------
            // 2. RPC �Լ� ȣ��
            //
            var fun_name = "";
            object[] args = null;
            JActor actor = null;

            if (rep_type == eReplicationType.Function)
            {
                //-------------------------------------------------------------------------------------
                // 2.1. �Ķ���� ����
                //
                fun_name = reader.ReadString();
                args = JNewSerialization.Deserialize(reader) as object[];


                //-------------------------------------------------------------------------------------
                // 2.2. RPC ȣ��
                //
                obj._replication_helper._rpc_func_name = fun_name;
                obj.InvokeMethod(fun_name, args);
            }
            //-------------------------------------------------------------------------------------
            // 3. ���� ����
            //
            else if (rep_type == eReplicationType.Actor)
            {
                var asset_name = reader.ReadString();
                actor = JActor.SpawnActor(asset_name);
                actor.Deserialize(reader);
                obj.OnSpwanActorInternal(actor);
            }


            //-------------------------------------------------------------------------------------
            // 3. Multicast�� ��� ����
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


            // fixme : Message ��/���� ���� ���� �ڵ� ����. ���� ���� �ѹ� ������ �ʿ伺 ����...?
        }
        #endregion

        #region [RPC] ��û ���� (��Ʈ��ũ ���)
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

        #region [RPC] �Լ� ȣ��
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

            // 1. �޽��� ����
            var net_message = MakeMessage(replication);
            tcp_client.SendMessage(net_message);

            // 2. ��� ����
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

            // 1. �޽��� ����
            var net_message = MakeMessage(replication);
            tcp_client.SendMessage(net_message);

            // 2. ��� ����
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
        // ���
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [���] [RPC] [��ƿ] �Ķ���� ����
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

        #region [���] [RPC] [��ƿ] �Ķ���� Serialization
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
