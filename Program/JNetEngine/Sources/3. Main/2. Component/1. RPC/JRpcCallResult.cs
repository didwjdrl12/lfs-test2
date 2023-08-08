using J2y.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JRpcCallResult
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JRpcCallResult
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [Variable] Base
        public eRpcResult _result;
        public string _fun_name;
        public JObject _caller;
        public JObject _receiver;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] 持失切
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JRpcCallResult(eRpcResult result)
        {
            _result = result;
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [RpcResponse] [Async]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [RpcResponse] [async] BinaryReader
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<BinaryReader> ReturnWait()
        {
            var reader = await _receiver.WaitRpcMessage(_fun_name);
            return reader;
        }
        #endregion

        #region [RpcResponse] [async] T1
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<T1> ReturnWait<T1>()
        {
            var reader = await _receiver.WaitRpcMessage(_fun_name);
            var objs = JNewSerialization.Deserialize(reader) as object[];
            var t1 = (T1)objs[0];
            return t1;
        }
        #endregion

        #region [RpcResponse] [async] T1, T2
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<(T1, T2)> ReturnWait<T1, T2>()
        {
            var reader = await _receiver.WaitRpcMessage(_fun_name);
            var objs = JNewSerialization.Deserialize(reader) as object[];
            var t1 = (T1)objs[0];
            var t2 = (T2)objs[1];
            return (t1, t2);
        }
        #endregion

        #region [RpcResponse] [async] T1, T2, T3
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<(T1, T2, T3)> ReturnWait<T1, T2, T3>()
        {
            var reader = await _receiver.WaitRpcMessage(_fun_name);
            var objs = JNewSerialization.Deserialize(reader) as object[];
            var t1 = (T1)objs[0];
            var t2 = (T2)objs[1];
            var t3 = (T3)objs[2];
            return (t1, t2, t3);
        }
        #endregion

        #region [RpcResponse] [async] T1, T2, T3, T4
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<(T1, T2, T3, T4)> ReturnWait<T1, T2, T3, T4>()
        {
            var reader = await _receiver.WaitRpcMessage(_fun_name);
            var objs = JNewSerialization.Deserialize(reader) as object[];
            var t1 = (T1)objs[0];
            var t2 = (T2)objs[1];
            var t3 = (T3)objs[2];
            var t4 = (T4)objs[3];
            return (t1, t2, t3, t4);
        }
        #endregion

        #region [RpcResponse] [async] T1, T2, T3, T4, T5
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<(T1, T2, T3, T4, T5)> ReturnWait<T1, T2, T3, T4, T5>()
        {
            var reader = await _receiver.WaitRpcMessage(_fun_name);
            var objs = JNewSerialization.Deserialize(reader) as object[];
            var t1 = (T1)objs[0];
            var t2 = (T2)objs[1];
            var t3 = (T3)objs[2];
            var t4 = (T4)objs[3];
            var t5 = (T5)objs[4];
            return (t1, t2, t3, t4, t5);
        }
        #endregion

        #region [RpcResponse] [async] T1, T2, T3, T4, T5, T6
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<(T1, T2, T3, T4, T5, T6)> ReturnWait<T1, T2, T3, T4, T5, T6>()
        {
            var reader = await _receiver.WaitRpcMessage(_fun_name);
            var objs = JNewSerialization.Deserialize(reader) as object[];
            var t1 = (T1)objs[0];
            var t2 = (T2)objs[1];
            var t3 = (T3)objs[2];
            var t4 = (T4)objs[3];
            var t5 = (T5)objs[4];
            var t6 = (T6)objs[5];
            return (t1, t2, t3, t4, t5, t6);
        }
        #endregion

        #region [RpcResponse] [async] T1, T2, T3, T4, T5, T6, T7
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<(T1, T2, T3, T4, T5, T6, T7)> ReturnWait<T1, T2, T3, T4, T5, T6, T7>()
        {
            var reader = await _receiver.WaitRpcMessage(_fun_name);
            var objs = JNewSerialization.Deserialize(reader) as object[];
            var t1 = (T1)objs[0];
            var t2 = (T2)objs[1];
            var t3 = (T3)objs[2];
            var t4 = (T4)objs[3];
            var t5 = (T5)objs[4];
            var t6 = (T6)objs[5];
            var t7 = (T7)objs[6];
            return (t1, t2, t3, t4, t5, t6, t7);
        }
        #endregion

        #region [RpcResponse] [async] T1, T2, T3, T4, T5, T6, T7, T8
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<(T1, T2, T3, T4, T5, T6, T7, T8)> ReturnWait<T1, T2, T3, T4, T5, T6, T7, T8>()
        {
            var reader = await _receiver.WaitRpcMessage(_fun_name);
            var objs = JNewSerialization.Deserialize(reader) as object[];
            var t1 = (T1)objs[0];
            var t2 = (T2)objs[1];
            var t3 = (T3)objs[2];
            var t4 = (T4)objs[3];
            var t5 = (T5)objs[4];
            var t6 = (T6)objs[5];
            var t7 = (T7)objs[6];
            var t8 = (T8)objs[7];
            return (t1, t2, t3, t4, t5, t6, t7, t8);
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [RpcResponse] [Async] Actor
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [RpcResponse] [async] Actor
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<T1> ReturnActorWait<T1>() where T1 : JComponent
        {
            var actor = await _receiver.WaitActorReplication();
            return actor.GetComponent<T1>();
        }
        #endregion
        



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [RpcResponse] [Sync]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [RpcResponse] BinaryReader
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Return(Action<BinaryReader> handler)
        {
            _receiver.RegisterRpcResponseHandler(_fun_name, handler);
        }
        #endregion

        #region [RpcResponse] T1
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Return<T1>(Action<T1> handler)
        {
            _receiver.RegisterRpcResponseHandler(_fun_name, (reader) =>
            {
                var objs = JNewSerialization.Deserialize(reader) as object[];
                handler((T1)objs[0]);
            });
        }
        #endregion

        #region [RpcResponse] T1, T2
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Return<T1, T2>(Action<T1, T2> handler)
        {
            _receiver.RegisterRpcResponseHandler(_fun_name, (reader) =>
            {
                var objs = JNewSerialization.Deserialize(reader) as object[];
                handler((T1)objs[0], (T2)objs[1]);
            });
        }
        #endregion

        #region [RpcResponse] T1, T2, T3
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Return<T1, T2, T3>(Action<T1, T2, T3> handler)
        {
            _receiver.RegisterRpcResponseHandler(_fun_name, (reader) =>
            {
                var objs = JNewSerialization.Deserialize(reader) as object[];
                handler((T1)objs[0], (T2)objs[1], (T3)objs[2]);
            });
        }
        #endregion

        #region [RpcResponse] T1, T2, T3, T4
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Return<T1, T2, T3, T4>(Action<T1, T2, T3, T4> handler)
        {
            _receiver.RegisterRpcResponseHandler(_fun_name, (reader) =>
            {
                var objs = JNewSerialization.Deserialize(reader) as object[];
                handler((T1)objs[0], (T2)objs[1], (T3)objs[2], (T4)objs[3]);
            });
        }
        #endregion

        #region [RpcResponse] T1, T2, T3, T4, T5
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Return<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> handler)
        {
            _receiver.RegisterRpcResponseHandler(_fun_name, (reader) =>
            {
                var objs = JNewSerialization.Deserialize(reader) as object[];
                handler((T1)objs[0], (T2)objs[1], (T3)objs[2], (T4)objs[3], (T5)objs[4]);
            });
        }
        #endregion

        #region [RpcResponse] T1, T2, T3, T4, T5, T6
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Return<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> handler)
        {
            _receiver.RegisterRpcResponseHandler(_fun_name, (reader) =>
            {
                var objs = JNewSerialization.Deserialize(reader) as object[];
                handler((T1)objs[0], (T2)objs[1], (T3)objs[2], (T4)objs[3], (T5)objs[4], (T6)objs[5]);
            });
        }
        #endregion

        #region [RpcResponse] T1, T2, T3, T4, T5, T6, T7
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Return<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> handler)
        {
            _receiver.RegisterRpcResponseHandler(_fun_name, (reader) =>
            {
                var objs = JNewSerialization.Deserialize(reader) as object[];
                handler((T1)objs[0], (T2)objs[1], (T3)objs[2], (T4)objs[3], (T5)objs[4], (T6)objs[5], (T7)objs[6]);
            });
        }
        #endregion        

        #region [RpcResponse] T1, T2, T3, T4, T5, T6, T7, T8
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Return<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> handler)
        {
            _receiver.RegisterRpcResponseHandler(_fun_name, (reader) =>
            {
                var objs = JNewSerialization.Deserialize(reader) as object[];
                handler((T1)objs[0], (T2)objs[1], (T3)objs[2], (T4)objs[3], (T5)objs[4], (T6)objs[5], (T7)objs[6], (T8)objs[7]);
            });
        }
        #endregion        
    }



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JActorReplicationResult
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JActorReplicationResult
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [Variable] Base
        public eRpcResult _result;
        public string _fun_name;
        public JObject _caller;
        public JObject _receiver;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] 持失切
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JActorReplicationResult(eRpcResult result)
        {
            _result = result;
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [RpcResponse] [Async]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [RpcResponse] [async] BinaryReader
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<BinaryReader> ReturnWait()
        {
            var reader = await _receiver.WaitRpcMessage(_fun_name);
            return reader;
        }
        #endregion

    }

}
