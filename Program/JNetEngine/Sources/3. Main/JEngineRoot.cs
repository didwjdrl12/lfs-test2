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
    //      전체 엔진을 관리하는 클래스, 각 시스템은 이 클래스를 상속 받아 각각의 기능들을 구현한다.
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
        
        #region [Variable] 0. 기본 정보
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

        #region [Init] 에셋 Init
        public virtual void InitAssets()
        {
            JNewSerialization._on_add_new_type_name = OnAddNewSerializeType;
        }
        #endregion

        //#region [업데이트]
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public override void UpdateInternal()
        //{
        //    base.UpdateInternal();
        //    JScheduler.Instance.Update();
        //}
        //#endregion

        #region [종료] OnDestroy
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

        #region [RPC] 요청
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // remarks: 자식 클래스에서 구현
        //
        public virtual void NetSendRPC(int rpc_target, JObject caller, JObject receiver, string fun_name, params dynamic[] args)
        {            
        }
        #endregion

        #region [RpcMediator] 생성
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

        #region [★★★★] [Actor] Spawn
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public virtual JActor RPCSpawnActor(int rpc_target, JObject caller, long dataInfoId)
        //{
        //    JActor dest_actor = null;

        //    switch (rpc_target)
        //    {
        //        case eRpcTarget.Client:
        //            {
        //                //--------------------------------------------------------------------
        //                // [S->C] 클라에 생성 요청
        //                if (_net_role == eNetRole.Authority)
        //                {                            
        //                    // 1. 액터 생성
        //                    dest_actor = SpawnActor(dataInfoId);

        //                    // 2. 클라에 생성 알림
        //                    var net_message = JRpcMediator.MakeMessage(rpc_target, caller, (writer) =>
        //                    {
        //                        writer.Write((int)eReplicationType.Actor);
        //                        dest_actor.Serialize(writer);
        //                    });
        //                    caller.NetSendMessage(net_message);
        //                }
        //                //--------------------------------------------------------------------
        //                // [C->C] 즉시 생성
        //                else
        //                {
        //                    dest_actor = SpawnActor(dataInfoId);
        //                }
        //            }
        //            break;

        //        case eRpcTarget.Server:
        //            {
        //                //--------------------------------------------------------------------
        //                // [S->S] 서버에만 생성
        //                if (_net_role == eNetRole.Authority)
        //                {
        //                    dest_actor = SpawnActor(dataInfoId);
        //                }
        //                //--------------------------------------------------------------------
        //                // [C->S] 서버에 생성 요청
        //                else
        //                {
        //                    //NetSendRPC(rpc_target, this, "OnRPCSpawnActor", caller._guid, dataInfoId);
        //                }
        //            }
        //            break;

        //        case eRpcTarget.Multicast:
        //            {
        //                //--------------------------------------------------------------------
        //                // [S->Noti] 서버에 생성 후 
        //                //
        //                if (_net_role == eNetRole.Authority)
        //                {
        //                    // 1. 액터 생성
        //                    dest_actor = SpawnActor(dataInfoId);

        //                    // 2. 클라에 생성 알림
        //                    var net_message = JRpcMediator.MakeMessage(rpc_target, caller, (writer) =>
        //                    {
        //                        writer.Write((int)eReplicationType.Actor);
        //                        dest_actor.Serialize(writer);
        //                    });
        //                    caller.NetBroadcast(net_message);
        //                }
        //                //--------------------------------------------------------------------
        //                // [C->S] 서버에 생성 요청
        //                else
        //                {
        //                    //NetSendRPC(rpc_target, this, "OnRPCSpawnActor", caller._guid, dataInfoId);
        //                }
        //            }
        //            break;
        //    }

        //    //if (_net_role == eNetRole.Authority)
        //    //{
        //    //    // 1. 서버에서는 생성 후 전파
        //    //    var actor = SpawnActor(dataInfoId);
        //    //    RPCSpawnActor(rpc_type, caller, actor);
        //    //}
        //    //else
        //    //{
        //    //    // 2. 클라에서는 서버에 요청
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

        #region [검토중] [Actor] Spawn (주로 서버에서 호출)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void RPCSpawnActor(int rpc_target, JObject caller, JActor src_actor)
        {

        }
        #endregion
        
        #region [Actor] [Abstarct] Spawn 
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        public virtual JActor SpawnActor(long dataInfoId)
        {
            // remarks: 자식 클래스에서 Actor 및 Component 생성 
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
            // remarks: 자식 클래스에서 Actor 및 Component 생성 
            return JObjectManager.Create<JActor>();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        // 주로 필드, 유저에서 사용
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

        #region [Network] 하위 클래스에서 구현
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
