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
    // JReplication
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JReplication
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] Common
        public eReplicationType _type;
        public int _rpc_target;         // eRpcTarget 
        public JObject _caller;
        public JObject _net_receiver;   // 焊烹篮 _caller
        public string _fun_name;
        public dynamic[] _args;
        #endregion

        #region [Variable] ActorReplication
        public JActor _src_actor;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 皋矫瘤 积己 
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [FunctionReplication] 积己
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JReplication Make(string fun_name, params dynamic[] args) { return new JReplication() { _type = eReplicationType.Function, _fun_name = fun_name, _args = args }; }
        public static JReplication Make(JObject caller, JObject net_receiver, string fun_name, params dynamic[] args) { return new JReplication() { _type = eReplicationType.Function, _caller = caller, _net_receiver = net_receiver, _fun_name = fun_name, _args = args }; }
        public static JReplication Make(int rpc_target, string fun_name, params dynamic[] args) { return new JReplication() { _type = eReplicationType.Function, _rpc_target = rpc_target, _fun_name = fun_name, _args = args }; }
        public static JReplication Make(int rpc_target, JObject caller, JObject net_receiver, string fun_name, params dynamic[] args) { return new JReplication() { _type = eReplicationType.Function, _rpc_target = rpc_target, _caller = caller, _net_receiver = net_receiver, _fun_name = fun_name, _args = args }; }
        #endregion

        #region [ActorReplication] 积己
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JReplication Make(JObject caller, JObject net_receiver, JActor actor) { return new JReplication() { _type = eReplicationType.Actor, _caller = caller, _net_receiver = net_receiver, _src_actor = actor }; }
        public static JReplication Make(int rpc_target, JObject caller, JObject net_receiver, JActor actor) { return new JReplication() { _type = eReplicationType.Actor, _rpc_target = rpc_target, _caller = caller, _net_receiver = net_receiver, _src_actor = actor}; }
        #endregion

    }

}


