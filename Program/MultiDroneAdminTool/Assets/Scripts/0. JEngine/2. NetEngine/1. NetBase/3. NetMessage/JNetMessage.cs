using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace J2y.Network
{
    #region [enum] NetPackingType
    public enum eNetPackingType
    {
        Default, Buffer, NetData, UserWriter, 
    }
    #endregion


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JNetMessage: 네트워크 메시지 
    //
    //      1. 사용자가 만들고 전파
    //      2. Normal or Simple Message
    //
    // [PackingType]
    //      1. Default (params dynamic args[])
    //      2. Buffer (byte[])
    //      3. NetData
    //      4. UserWriter (Action)
    //
    // [검토]
    //      1. Broadcast시 메시지 복사가 아닌 포인트만 재사용중이다 -> result, buffer, buffer_size등이 문제 없는지 확인한다.
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JNetMessage
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] Base
        public eNetPackingType _packing_type;
        public int _message_type;
        public bool _immediate;
        public string _result;
        public int _capacity = 128;

        // Packing Type
        public object[] _parameters;
        public byte[] _data_buffer;
        public int _data_size;
        public JNetData _netdata;
        
        public Action<BinaryWriter> _fun_writer;

        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메시지 생성 
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [메시지] 생성
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JNetMessage Make(int message_type, params dynamic[] args) { return new JNetMessage() { _packing_type = eNetPackingType.Default, _message_type = message_type, _parameters = args }; }
        public static JNetMessage Make(int message_type, byte[] data_buffer, int data_size) { return new JNetMessage() { _packing_type = eNetPackingType.Buffer, _message_type = message_type, _data_buffer = data_buffer, _data_size = data_size, _capacity = data_size + NetBase.HeaderByteSize }; }
        public static JNetMessage Make(int message_type, JNetData netdata, params dynamic[] args) { return new JNetMessage() { _packing_type = eNetPackingType.NetData, _message_type = message_type, _netdata = netdata, _parameters = args }; }        
        public static JNetMessage Make(int message_type, Action<BinaryWriter> fun_write) { return new JNetMessage() { _packing_type = eNetPackingType.UserWriter, _message_type = message_type, _fun_writer = fun_write }; }
        #endregion

        #region [메시지] [Simple] 생성
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JNetMessage MakeSimple(string message_type, params dynamic[] args) { return new JNetMessage() { _packing_type = eNetPackingType.Default, _message_type = JNetMessageProtocol.SimpleMessage, _fun_writer = (writer) => { writer.Write(message_type); }, _parameters = args }; }
        public static JNetMessage MakeSimple(string message_type, byte[] data_buffer, int data_size) { return new JNetMessage() { _packing_type = eNetPackingType.Buffer, _message_type = JNetMessageProtocol.SimpleMessage, _fun_writer = (writer) => { writer.Write(message_type); }, _data_buffer = data_buffer, _data_size = data_size, _capacity = data_size + NetBase.HeaderByteSize }; }
        public static JNetMessage MakeSimple(string message_type, JNetData netdata, params dynamic[] args) { return new JNetMessage() { _packing_type = eNetPackingType.NetData, _message_type = JNetMessageProtocol.SimpleMessage, _fun_writer = (writer) => { writer.Write(message_type); }, _netdata = netdata, _parameters = args }; }
        public static JNetMessage MakeSimple(string message_type, Action<BinaryWriter> fun_write) { return new JNetMessage() { _packing_type = eNetPackingType.UserWriter, _message_type = JNetMessageProtocol.SimpleMessage, _fun_writer = (writer) => { writer.Write(message_type); fun_write.Invoke(writer); } }; }
        #endregion

        #region [메시지] [Short] 생성
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JNetMessage MakeShort(int message_type, params dynamic[] args) { return new JNetMessage() { _packing_type = eNetPackingType.Default, _message_type = JNetMessageProtocol.ShortMessage, _fun_writer = (writer) => { writer.Write(message_type); }, _parameters = args, _capacity = 32 }; }
        public static JNetMessage MakeShort(int message_type, byte[] data_buffer, int data_size) { return new JNetMessage() { _packing_type = eNetPackingType.Buffer, _message_type = JNetMessageProtocol.ShortMessage, _fun_writer = (writer) => { writer.Write(message_type); }, _data_buffer = data_buffer, _data_size = data_size, _capacity = data_size + NetBase.HeaderByteSize }; }
        public static JNetMessage MakeShort(int message_type, JNetData netdata, params dynamic[] args) { return new JNetMessage() { _packing_type = eNetPackingType.NetData, _message_type = JNetMessageProtocol.ShortMessage, _fun_writer = (writer) => { writer.Write(message_type); }, _netdata = netdata, _parameters = args, _capacity = 32 }; }
        public static JNetMessage MakeShort(int message_type, Action<BinaryWriter> fun_write) { return new JNetMessage() { _packing_type = eNetPackingType.UserWriter, _message_type = JNetMessageProtocol.ShortMessage, _fun_writer = (writer) => { writer.Write(message_type); fun_write.Invoke(writer); }, _capacity = 32 }; }
        #endregion

        // todo: int[], float[] 등 Array, List의 전송이 자동으로 되도록 한다.

        #region [유틸] 속성 변경
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JNetMessage SetCapacity(int capacity) { _capacity = capacity; return this; }
        public JNetMessage SetImmediate(bool immediate) { _immediate = immediate; return this; }
        public JNetMessage SetResult(string result) { _result = result; return this; }
        #endregion
    }



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // (사용 X) JNet
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public partial class JNet
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 유틸
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [백업] [Read/Write] NetData
        //#region [메시지] [Read] NetData
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static T Read<T>(BinaryReader recv_msg) where T : JNetData, new()
        //{
        //    var netdata = new T();
        //    netdata.Read(recv_msg);
        //    return netdata;
        //}
        //#endregion

        //#region [메시지] [Read/Write] List<NetData>
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static void Write<T>(BinaryWriter writer, List<T> data) where T : JNetData
        //{
        //    writer.Write(data.Count);
        //    foreach (var d in data)
        //        d.Write(writer);
        //}
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static void Read<T>(BinaryReader reader, List<T> buffer) where T : JNetData, new()
        //{
        //    buffer.Clear();
        //    int count = reader.ReadInt32();
        //    for (int i = 0; i < count; i++)
        //    {
        //        var data = new T();
        //        data.Read(reader);
        //        buffer.Add(data);
        //    }
        //}
        //#endregion
               
        //#region [메시지] [Read/Write] Array<NetData>
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static void Write<T>(BinaryWriter writer, T[] data) where T : JNetData
        //{
        //    writer.Write(data.Length);
        //    foreach (var d in data)
        //        d.Write(writer);
        //}
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static void Read<T>(BinaryReader reader, T[] buffer) where T : JNetData, new()
        //{
        //    int count = reader.ReadInt32();
        //    for (int i = 0; i < buffer.Length; i++)
        //    {
        //        var data = new T();
        //        data.Read(reader);
        //        buffer[i] = data;
        //    }
        //}
        //#endregion

        #endregion

        #region [메시지] [Read/Write] Vector2, Vector3
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Packing(BinaryWriter writer, Vector3 v)
        {
            writer.Write(v.x);
            writer.Write(v.y);
            writer.Write(v.z);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Packing(BinaryWriter writer, Vector2 v)
        {
            writer.Write(v.x);
            writer.Write(v.y);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vector3 ParseVector3(BinaryReader recv_msg)
        {
            var v = new Vector3
            {
                x = recv_msg.ReadSingle(),
                y = recv_msg.ReadSingle(),
                z = recv_msg.ReadSingle()
            };
            return v;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vector2 ParseVector2(BinaryReader recv_msg)
        {
            var v = new Vector2
            {
                x = recv_msg.ReadSingle(),
                y = recv_msg.ReadSingle()
            };
            return v;
        }
        #endregion


        #region [메시지] [Write] params dynamic[] args
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void PacketOutMsg(BinaryWriter writer, params dynamic[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                if (typeof(JObject).IsAssignableFrom(args[i].GetType()))
                    JNewSerialization.Serialize(writer, args[i]);
                else
                    writer.Write(args[i]);
            }
            #region [Backup]
            //var arguType = args[i].GetType();
            //if (typeof(int) == arguType)
            //	writer.Write((int)args[i]);
            //else if (typeof(string) == arguType)
            //	writer.Write((string)args[i]);
            //else if (typeof(bool) == arguType)
            //	writer.Write((bool)args[i]);
            //else if (typeof(float) == arguType)
            //	writer.Write((float)args[i]);
            //else if (typeof(double) == arguType)
            //	writer.Write((double)args[i]);
            //else if (typeof(long) == arguType)
            //	writer.Write((long)args[i]);
            //else if (typeof(byte) == arguType)
            //	writer.Write((byte)args[i]);
            //else if (typeof(short) == arguType)
            //	writer.Write((short)args[i]);
            //else if (typeof(sbyte) == arguType)
            //	writer.Write((sbyte)args[i]);
            //else if (typeof(ushort) == arguType)
            //	writer.Write((ushort)args[i]);
            //else if (typeof(uint) == arguType)
            //	writer.Write((uint)args[i]);
            //else if (typeof(ulong) == arguType)
            //	writer.Write((ulong)args[i]);
            #endregion
        }
        #endregion
    }


}
