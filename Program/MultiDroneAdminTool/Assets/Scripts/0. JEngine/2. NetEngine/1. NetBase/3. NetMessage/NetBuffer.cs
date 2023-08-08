using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

namespace J2y.Network
{


	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// NetBuffer: 네트워크 스트림 버퍼
    //
	//		NetMessage(사용자 메시지)를 Serialization(+ Header)한 후 버퍼로 저장해서 네트워크 큐로 관리한다.
    //      NetworkStream으로 바로 쓰지 않는 이유는 쓰레드가 다르기 때문에 메인 쓰레드가 블락 되는 것을 방지한다.
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public class NetBuffer
	{
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 상수, Static
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [상수] 
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		// Number of bytes to overallocate for each message to avoid resizing
		protected const int c_overAllocateAmount = 4;
		#endregion

		


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Variable
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [Variable] 
		public MemoryStream _stream;
        public bool _use_memorypool = true;
        public int _size;                       // MemoryStream의 버퍼 사이즈보다 작거나 같다. (_stream.Length를 사용하지 않는 이유는 NetworkStream에서 읽을 경우 Buffer[]를 직접 넘기기 때문에 Length가 증가하지 않는다.) 
        public bool _immediate_send;            // 사용 X
		public IPEndPoint _sender_end_point;	// UDP

		// temp(header)
		public int _message_id;
        public INetMessageHeader Header;
		#endregion

		#region [Property]
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public byte[] _buffer
		{
			get { return _stream.GetBuffer(); }
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public int _position
		{
			get { return (int)_stream.Position; }
			set { _stream.Position = value; }
		}
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int _buffer_capacity
        {
            get { return _stream.Capacity; }            
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int _remain_buffer_capacity
        {
            get { return _buffer_capacity - _size; }
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public NetBuffer() { }
		public NetBuffer(int capacity)
		{
			_stream = new MemoryStream(capacity);
		}
		public NetBuffer(byte[] buffer)
		{
			_stream = new MemoryStream(buffer);
		}
        #endregion


        #region [복제] Copy
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Clone(NetBuffer src)
        {
            if (src._size > 0)
                Buffer.BlockCopy(src._buffer, 0, _buffer, 0, src._size);

            _size = src._size;
            _immediate_send = src._immediate_send;
            _sender_end_point = src._sender_end_point;
            _message_id = src._message_id;
            Header = src.Header;
        }
        #endregion



        #region [버퍼] 데이터 읽기 (+포지션 이동)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Read(MemoryStream out_stream, int data_size)
        {
            out_stream.Write(_buffer, _position, data_size);
            _position += data_size;
        }
        #endregion


        #region [버퍼] [Compact] 데이터 앞으로 이동
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Compact()
        {
            var remain_size = _size - _position;
            if(remain_size > 0)
                Buffer.BlockCopy(_buffer, _position, _buffer, 0, remain_size);
            _size = remain_size;
            _position = 0;
        }
        #endregion

        #region [버퍼] [Reset] 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Reset()
        {
            _size = 0;
            _position = 0;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 유틸
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [유틸] 해더와 데이터를 합친 메모리 스트림을 리턴한다.
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MemoryStream CombinePacketData(INetMessageHeader header, MemoryStream data_stream, int data_size)
        {
            var new_stream = new MemoryStream();
            var writer = new BinaryWriter(new_stream);
            header.Write(writer);
            writer.Write(data_stream.GetBuffer(), 0, data_size);
            new_stream.Position = 0;
            return new_stream;
        }
        #endregion

    }


}
