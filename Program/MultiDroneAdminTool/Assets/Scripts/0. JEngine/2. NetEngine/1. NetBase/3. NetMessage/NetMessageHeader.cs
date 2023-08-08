using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using static System.Net.IPAddress;


namespace J2y.Network
{
    namespace BigEndianExtension
    {
        public static class BigEndian
        {
            public static short ToBigEndian(this short value) => HostToNetworkOrder(value);
            public static int ToBigEndian(this int value) => HostToNetworkOrder(value);
            public static long ToBigEndian(this long value) => HostToNetworkOrder(value);
            public static short FromBigEndian(this short value) => NetworkToHostOrder(value);
            public static int FromBigEndian(this int value) => NetworkToHostOrder(value);
            public static long FromBigEndian(this long value) => NetworkToHostOrder(value);
        }
    }

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // [Interface] NetMessageHeader
    //		
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public interface INetMessageHeader
    {
        int HeaderSize { get; }
        int DataSize { get; }
        int PacketSize { get; }
        int MessageID { get; }
		int Acknowledgement { get; }

		bool HasStartTag { get; }
        byte StartTag { get; }

        void Write(BinaryWriter writer);
        void Write(byte[] buffer, int ptr);
        void Read(BinaryReader reader);
        void Read(byte[] buffer, int ptr);

    }


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // NetMessageHeader
    //		
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class NetMessageHeader : INetMessageHeader
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private static int c_header_marking = Convert.ToInt32("0xa5a5a5a5", 16);

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] Header
        public int _marking = c_header_marking;
        public int _data_size;
        public int _acknowledgement;
        public int _message_id;
        //public int _sequence_number;
        //public int _checksum;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] HeaderSize
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual int HeaderSize
		{
			get { return NetBase.HeaderByteSize; }
        }
		#endregion

		#region [Property] DataSize
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual int DataSize
		{
			get { return _data_size; }
		}
        #endregion

        #region [Property] PacketSize
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual int PacketSize
        {
            get { return DataSize + HeaderSize; }
            set { _data_size = value - HeaderSize; }
        }
        #endregion

        #region [Property] MessageID
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual int MessageID
        {
            get { return _message_id; }
        }
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual int Acknowledgement
		{
			get { return _acknowledgement; }
		}
		#endregion

		#region [Property] MessageID
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual bool HasStartTag
        {
            get { return false; }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual byte StartTag
        {
            get { return 0; }
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // IO
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [IO] [Binary] Write/Read
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Write(BinaryWriter writer)
		{
            //Write(s_header_raw_data, 0);
            //writer.Write(s_header_raw_data, 0, s_header_raw_data.Length);
            writer.Write(_marking.ToBigEndian());
            writer.Write(PacketSize.ToBigEndian());
            writer.Write(_acknowledgement.ToBigEndian());
            writer.Write(_message_id.ToBigEndian());
        }
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual void Read(BinaryReader reader)
		{
            //reader.Read(s_header_raw_data, 0, s_header_raw_data.Length);
            //Read(s_header_raw_data, 0);
            _marking = reader.ReadInt32().FromBigEndian();
            PacketSize = reader.ReadInt32().FromBigEndian();
			_acknowledgement = reader.ReadInt32().FromBigEndian();
            _message_id = reader.ReadInt32().FromBigEndian();
        }
        #endregion

        #region [IO] [Buffer] Write/Read
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Write(byte[] buffer, int ptr)
        {
            //SetInteger(buffer, ptr + 0, _marking);
            //SetInteger(buffer, ptr + 4, PacketSize);
            //SetInteger(buffer, ptr + 8, _acknowledgement);
            //SetInteger(buffer, ptr + 12, _message_id);
            CopyToByteArray(_marking.ToBigEndian(), buffer, ptr + 0);
            CopyToByteArray(PacketSize.ToBigEndian(), buffer, ptr + 4);
            CopyToByteArray(_acknowledgement.ToBigEndian(), buffer, ptr + 8);
            CopyToByteArray(_message_id.ToBigEndian(), buffer, ptr + 12);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Read(byte[] buffer, int ptr)
        {
            _marking = BitConverter.ToInt32(buffer, ptr).FromBigEndian();
            PacketSize = BitConverter.ToInt32(buffer, ptr + 4).FromBigEndian();
			_acknowledgement = BitConverter.ToInt32(buffer, ptr + 8).FromBigEndian();
            _message_id = BitConverter.ToInt32(buffer, ptr + 12).FromBigEndian();
            
            //_marking = GetInteger(buffer, ptr + 0);
            //var packet_size = GetInteger(buffer, ptr + 4);
            //_acknowledgement = GetInteger(buffer, ptr + 8);
            //_message_id = GetInteger(buffer, ptr + 12);
        }
        #endregion

        #region [IO] ShallowCopy
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        internal NetMessageHeader ShallowCopy()
		{
			return (NetMessageHeader)this.MemberwiseClone();
		}
        #endregion
        

        #region [유틸] CopyToByteArray (int32)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void CopyToByteArray(int source, byte[] destination, int offset)
        {
            //// check if there is enough space for all the 4 bytes we will copy
            //if (destination.Length < offset + 4)
            //    throw new ArgumentException("Not enough room in the destination array");

            destination[offset] = (byte)(source >> 24); // fourth byte
            destination[offset + 1] = (byte)(source >> 16); // third byte
            destination[offset + 2] = (byte)(source >> 8); // second byte
            destination[offset + 3] = (byte)source; // last byte is already in proper position
        }
        #endregion

        #region [백업] CNT 데이터 패킹
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static int GetInteger(byte[] bytes, int sourceStartIndex)
        //{
        //    Array.Copy(bytes, sourceStartIndex, s_int_raw_data, 0, 4);
        //    Array.Reverse(s_int_raw_data);
        //    return BitConverter.ToInt32(s_int_raw_data, 0);
        //}
        //public static void SetInteger(byte[] bytes, int sourceStartIndex, int setValue)
        //{
        //    byte[] temp = BitConverter.GetBytes(setValue);
        //    Array.Reverse(temp);
        //    Array.Copy(temp, 0, bytes, sourceStartIndex, 4);
        //}

        //private static byte[] s_header_raw_data = new byte[NetBase.HeaderByteSize];
        //private static byte[] s_int_raw_data = new byte[4];
        #endregion



    }

}
