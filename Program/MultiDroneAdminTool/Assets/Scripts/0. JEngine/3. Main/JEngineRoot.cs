using J2y.Network;
using System;
using System.Collections.Generic;
using System.IO;

public enum eNetPeerType
{
    Client,
    MainServer,
    FieldServer,
    // todo: AI, Chat
    Max,
    Unknown,
}



namespace J2y
{


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JEngineRoot
    //      ��ü ������ �����ϴ� Ŭ����, �� �ý����� �� Ŭ������ ��� �޾� ������ ��ɵ��� �����Ѵ�.
    //
    //      1. RPC (Send, Recv)
    //      2. Spawn Actor
    //      3. Create Component
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JEngineRoot : JObject
    {
        public static JEngineRoot Instance;

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [Variable] 0. �⺻ ����
        public static eNetPeerType _net_peer_type;
        #endregion

        #region [Variable] 1. Manager
        public JRpcMediator _rpc_mediator;
        public JAssetManager _asset_manager;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] [Actor] NetRole
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual bool IsServer
        {
            get { return !IsClient; }
            //get { return (_net_role == eNetRole.Authority); }            
        }
        public virtual bool IsClient
        {
            get { return (_net_peer_type == eNetPeerType.Client); }
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] Awake
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Awake()
        {
            Instance = this;
            MakeRpcMediator();
                        
        }
        #endregion

        #region [Init] ���� Init
        public virtual void InitAssets()
        {
            JNewSerialization._on_add_new_type_name = OnAddNewSerializeType;
        }
        #endregion

        //#region [������Ʈ]
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public override void UpdateInternal()
        //{
        //    base.UpdateInternal();
        //    JScheduler.Instance.Update();
        //}
        //#endregion

        #region [����] OnDestroy
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void OnDestroy()
        {
            JObjectManager.Destroy(_asset_manager);
            _asset_manager = null;

            base.OnDestroy();
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. RPC
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [RPC] ��û
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // remarks: �ڽ� Ŭ�������� ����
        //
        public virtual void NetSendRPC(int rpc_target, JObject caller, JObject receiver, string fun_name, params dynamic[] args)
        {            
        }
        #endregion

        #region [RpcMediator] ����
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void MakeRpcMediator()
        {
            _rpc_mediator = new JRpcMediator();
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 2. Actor
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [�ڡڡڡ�] [Actor] Spawn
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public virtual JActor RPCSpawnActor(int rpc_target, JObject caller, long dataInfoId)
        //{
        //    JActor dest_actor = null;

        //    switch (rpc_target)
        //    {
        //        case eRpcTarget.Client:
        //            {
        //                //--------------------------------------------------------------------
        //                // [S->C] Ŭ�� ���� ��û
        //                if (_net_role == eNetRole.Authority)
        //                {                            
        //                    // 1. ���� ����
        //                    dest_actor = SpawnActor(dataInfoId);

        //                    // 2. Ŭ�� ���� �˸�
        //                    var net_message = JRpcMediator.MakeMessage(rpc_target, caller, (writer) =>
        //                    {
        //                        writer.Write((int)eReplicationType.Actor);
        //                        dest_actor.Serialize(writer);
        //                    });
        //                    caller.NetSendMessage(net_message);
        //                }
        //                //--------------------------------------------------------------------
        //                // [C->C] ��� ����
        //                else
        //                {
        //                    dest_actor = SpawnActor(dataInfoId);
        //                }
        //            }
        //            break;

        //        case eRpcTarget.Server:
        //            {
        //                //--------------------------------------------------------------------
        //                // [S->S] �������� ����
        //                if (_net_role == eNetRole.Authority)
        //                {
        //                    dest_actor = SpawnActor(dataInfoId);
        //                }
        //                //--------------------------------------------------------------------
        //                // [C->S] ������ ���� ��û
        //                else
        //                {
        //                    //NetSendRPC(rpc_target, this, "OnRPCSpawnActor", caller._guid, dataInfoId);
        //                }
        //            }
        //            break;

        //        case eRpcTarget.Multicast:
        //            {
        //                //--------------------------------------------------------------------
        //                // [S->Noti] ������ ���� �� 
        //                //
        //                if (_net_role == eNetRole.Authority)
        //                {
        //                    // 1. ���� ����
        //                    dest_actor = SpawnActor(dataInfoId);

        //                    // 2. Ŭ�� ���� �˸�
        //                    var net_message = JRpcMediator.MakeMessage(rpc_target, caller, (writer) =>
        //                    {
        //                        writer.Write((int)eReplicationType.Actor);
        //                        dest_actor.Serialize(writer);
        //                    });
        //                    caller.NetBroadcast(net_message);
        //                }
        //                //--------------------------------------------------------------------
        //                // [C->S] ������ ���� ��û
        //                else
        //                {
        //                    //NetSendRPC(rpc_target, this, "OnRPCSpawnActor", caller._guid, dataInfoId);
        //                }
        //            }
        //            break;
        //    }

        //    //if (_net_role == eNetRole.Authority)
        //    //{
        //    //    // 1. ���������� ���� �� ����
        //    //    var actor = SpawnActor(dataInfoId);
        //    //    RPCSpawnActor(rpc_type, caller, actor);
        //    //}
        //    //else
        //    //{
        //    //    // 2. Ŭ�󿡼��� ������ ��û
        //    //    RPC(rpc_type, "RPCSpawnActor", dataInfoId);
        //    //}

        //    return dest_actor;
        //}
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public virtual void OnRPCSpawnActor(int rpc_target, long caller_guid, long dataInfoId)
        //{
        //    var caller = JObjectManager.Find(caller_guid);
        //    if (null == caller)
        //        return;
        //    RPCSpawnActor(rpc_target, caller, dataInfoId);
        //}
        #endregion

        #region [������] [Actor] Spawn (�ַ� �������� ȣ��)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void RPCSpawnActor(int rpc_target, JObject caller, JActor src_actor)
        {

        }
        #endregion
        
        #region [Actor] [Abstarct] Spawn 
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        public virtual JActor SpawnActor(long dataInfoId)
        {
            // remarks: �ڽ� Ŭ�������� Actor �� Component ���� 
            return JObjectManager.Create<JActor>();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        public virtual JActor SpawnActor(string prefab_name)
        {
            return JObjectManager.Create<JActor>();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual JActor SpawnActor(JActor src_actor)
        {
            // remarks: �ڽ� Ŭ�������� Actor �� Component ���� 
            return JObjectManager.Create<JActor>();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        // �ַ� �ʵ�, �������� ���
        //
        public virtual JActor SpawnActor<T>(string prefab_name) where T : JComponent
        {
            var actor = JObjectManager.Create<JActor>();
            actor.AddComponent<T>();
            return actor;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 4. Network
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Network] ���� Ŭ�������� ����
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override eNetSendResult NetSendMessage(JNetMessage message) { return eNetSendResult.UnknownError;  }
        public override eNetSendResult NetSendMessage(eNetPeerType server_type, JNetMessage message) { return eNetSendResult.UnknownError; }
        public override void NetBroadcast(JNetMessage message, NetPeer_base except = null) { }
        public override void NetBroadcast(eNetPeerType server_type, JNetMessage message, NetPeer_base except = null) { }
        public override void RegisterMessageHandler(int id, Action<NetPeer_base, BinaryReader> handler) { }
        public override void RegisterMessageHandler(eNetPeerType server_type, int id, Action<NetPeer_base, BinaryReader> handler) { }
        #endregion

        #region [RPC] Serialize Type Update
        protected virtual void RPC_OnAddNewSerializeType(string type_name)
        {
            JNewSerialization.UpdateTypeHash(type_name, JObjectManager.GetSerializeType(type_name));
        }

        public virtual void OnAddNewSerializeType(string type_name) { }
        #endregion

    }
}
