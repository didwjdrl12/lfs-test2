using J2y.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JObject
    //
    //      3. [Replication] RPC(Function)
    //      4. [Replication] Actor
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

#if NET_SERVER
    public partial class JObject
#else
    public partial class JObject : MonoBehaviour
#endif
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] RPC
        public Dictionary<string, Action<BinaryReader>> _rpc_response_handlers;
        protected TaskCompletionSource<JActor> _tcs_actor_replication;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] Replication
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private JReplicationHelper _replication_helper_impl;
        public JReplicationHelper _replication_helper
        {
            get
            {
                if (null == _replication_helper_impl)
                    _replication_helper_impl = new JReplicationHelper();

                return _replication_helper_impl;
            }
        }
        #endregion



        #region [Property] NetRole
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual eNetRole _net_role { get; set; }
        public virtual eNetRole _net_remote_role { get; set; }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 2. [Replication] [RPC(Function)]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Replication] [RPC] 요청
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JRpcCallResult RpcRequest(NetTcpClient tcp_client, JActor actor)                             { return RpcRequest(tcp_client, JReplication.Make(this, this, actor)); }
        public JRpcCallResult RpcRequest(NetTcpClient tcp_client, string fun_name, params dynamic[] args)   { return RpcRequest(tcp_client, JReplication.Make(this, this, fun_name, args)); }
        public JRpcCallResult RpcRequest(int rpc_target, JActor actor)                                      { return RpcRequest(JReplication.Make(rpc_target, this, this, actor)); }
        public JRpcCallResult RpcRequest(int rpc_target, string fun_name, params dynamic[] args)            { return RpcRequest(JReplication.Make(rpc_target, this, this, fun_name, args)); }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JRpcCallResult RpcRequest(NetTcpClient tcp_client, JReplication replication)                 { return JRpcMediator.Instance.RpcRequest(tcp_client, replication); }
        public JRpcCallResult RpcRequest(JReplication replication)                                          { return JRpcMediator.Instance.RpcRequest(replication); }
        #endregion

        #region [Replication] [RPC] 응답
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual eRpcResult RpcResponse(params dynamic[] args)
        {
            _replication_helper._net_sender.SendMessage(JNetMessage.Make(JNetMessageProtocol.RPCResponse, (writer) =>
            {
                writer.Write(_guid);
                writer.Write(_replication_helper._rpc_func_name);
                JNewSerialization.Serialize(writer, args);
            }));
            return eRpcResult.Succeed;
        }
        #endregion

        #region [Replication] [RPC] 응답 메시지핸들러 등록
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void RegisterRpcResponseHandler(string fun_name, Action<BinaryReader> handler)
        {
            if (null == _rpc_response_handlers)
                _rpc_response_handlers = new Dictionary<string, Action<BinaryReader>>();

            _rpc_response_handlers[fun_name] = handler;
        }
        #endregion

        #region [Replication] [RPC] [async] Wait RPC Message
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<BinaryReader> WaitRpcMessage(string fun_name)
        {
            var tcs = new TaskCompletionSource<BinaryReader>();
            RegisterRpcResponseHandler(fun_name, (reader) =>
            {
                tcs.SetResult(reader);
            });
            var result = await tcs.Task;
            return result;
        }
        #endregion
        

        #region [Reflection] Invoke
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void InvokeMethod(string methodName, params object[] argus)
        {
            var method = FindMethod(methodName);
            if (method != null)
                method.Invoke(this, argus);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public MethodInfo FindMethod(string methodName)
        {
            return JReflection.FindMethod(GetType(), methodName);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool HasMethod(string methodName)
        {
            return FindMethod(methodName) != null;
        }
        // public void CancelInvoke();
        // public void CancelInvoke(string methodName);
        #endregion


        #region [Network] 해당 객체(유저 또는 유닛)에 메시지 전송
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual eNetSendResult NetSendMessage(JNetMessage message)
        {
            return JEngineRoot.Instance.NetSendMessage(message); // default method
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual eNetSendResult NetSendMessage(eNetPeerType server_type, JNetMessage message)
        {
            return JEngineRoot.Instance.NetSendMessage(server_type, message); // default method
        }
        #endregion

        #region [Network] 해당 객체(유저, 필드, 유닛) 주변으로 메시지 전파
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void NetBroadcast(JNetMessage message, NetPeer_base except = null)
        {
            JEngineRoot.Instance.NetBroadcast(message, except); // default method
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void NetBroadcast(eNetPeerType server_type, JNetMessage message, NetPeer_base except = null)
        {
            JEngineRoot.Instance.NetBroadcast(server_type, message, except); // default method
        }
        #endregion

        #region [Network] 메시지핸들러 등록
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void RegisterMessageHandler(int id, Action<NetPeer_base, BinaryReader> handler)
        {
            JEngineRoot.Instance.RegisterMessageHandler(id, handler); // default method
        }
        public virtual void RegisterMessageHandler(eNetPeerType server_type, int id, Action<NetPeer_base, BinaryReader> handler)
        {
            JEngineRoot.Instance.RegisterMessageHandler(server_type, id, handler); // default method
        }
        #endregion
         


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 3. [Replication] [Actor]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Spawn] JActor
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual JActor Spawn(string asset_name)
        {
            return JActor.SpawnActor(asset_name);
        }
        #endregion

        #region [Replication] [Actor]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual JActorReplicationResult Spawn(int rpc_target, string asset_name)
        {
            var actor = Spawn(asset_name);
            return Spawn(rpc_target, actor);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual JActorReplicationResult Spawn(int rpc_target, JActor actor)
        {
            var replication = JReplication.Make(rpc_target, this, this, actor);
            
            var result = JRpcMediator.Instance.ActorReplication(replication);
            return result;
        }
        #endregion

        #region [Replication] [RPC] 응답
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public eRpcResult ActorReplicationResponse(params dynamic[] args)
        //{
        //    NetSendMessage(JNetMessage.Make(JNetMessageProtocol.RPCResponse, (writer) =>
        //    {
        //        writer.Write(JReflection.GetCurrentMethod());
        //        JNewSerialization.Serialize(writer, args);
        //    }));
        //    return eRpcResult.Succeed;
        //}
        #endregion

        #region [Replication] [RPC] [async] Wait RPC Message
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        public async Task<JActor> WaitActorReplication()
        {
            _tcs_actor_replication = new TaskCompletionSource<JActor>();            
            var result = await _tcs_actor_replication.Task;
            return result;
        }
        #endregion

        #region [Replication] [Actor] [이벤트] 객체 생성됨
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void OnSpwanActorInternal(JActor actor)
        {
            OnSpwanActor(actor);

            if (_tcs_actor_replication != null)
                _tcs_actor_replication.SetResult(actor);
        }
        public virtual void OnSpwanActor(JActor actor) { }
        #endregion

        #region [Actor] [Spawn] 검토중
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static async Task<JActor> SpawnWait(eRpcTarget rep, string asset_name)
        //{
        //    var actor = Spawn(rep, asset_name);
        //    return actor;
        //}
        //#region [Actor] [Spawn] 2. Type
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static T Spawn<T>(eReplicationType rep) where T : JComponent
        //{
        //    var actor = Spawn(rep);
        //    return actor.AddComponent<T>();
        //}
        //#endregion

        //#region [Actor] [Spawn] 3. ActorType (PrefabName)
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static JActor Spawn(eReplicationType rep, string prefab_name)
        //{
        //    var actor = Spawn(rep);
        //    return actor;
        //}
        //#endregion

        //#region [Actor] [Spawn] 4. DataInfo ID
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static JActor Spawn(eReplicationType rep, long dataInfoId)
        //{
        //    var actor = Spawn(rep);
        //    return actor;
        //}
        //#endregion

        #endregion

            

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 백업
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [백업] GUID
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //internal GUID m_GUID;
        //public GUID guid { get { return m_GUID; } }
        //internal long m_LocalIdentifierInFile;
        //public long localIdentifierInFile { get { return m_LocalIdentifierInFile; } }

        //[NativeName("fileType")]
        //internal FileType m_FileType;
        //public FileType fileType { get { return m_FileType; } }

        //internal string m_FilePath;
        //public string filePath { get { return m_FilePath; } }

        //public override string ToString()
        //{
        //	return UnityString.Format("{{guid: {0}, fileID: {1}, type: {2}, path: {3}}}", m_GUID, m_LocalIdentifierInFile, m_FileType, m_FilePath);
        //}


        //public override int GetHashCode()
        //{
        //	unchecked
        //	{
        //		var hashCode = m_GUID.GetHashCode();
        //		hashCode = (hashCode * 397) ^ m_LocalIdentifierInFile.GetHashCode();
        //		hashCode = (hashCode * 397) ^ (int)m_FileType;
        //		if (!string.IsNullOrEmpty(m_FilePath))
        //			hashCode = (hashCode * 397) ^ m_FilePath.GetHashCode();
        //		return hashCode;
        //	}
        //}
        #endregion


    }
}
