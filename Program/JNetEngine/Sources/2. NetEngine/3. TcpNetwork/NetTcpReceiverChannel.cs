using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace J2y.Network
{

	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// NetTcpReceiverChannel
	//
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public class NetTcpReceiverChannel : NetReceiverChannel_base
	{
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Variable
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


		#region [Variable] Base

		private NetTcpPeer_base _tcp_peer;
		#endregion


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 0. Base Methods
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [Init] 생성자
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public NetTcpReceiverChannel(NetPeer_base netpeer)
			: base(netpeer)
		{
			_tcp_peer = netpeer as NetTcpPeer_base;
		}
		#endregion



		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// [메시지] 수신  
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [IO 쓰레드] [수신] 데이터 읽기 루프
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public override void ReadLoop()
		{
			while (!_loop_stop && !_cancel_token.IsCancellationRequested && _netpeer.IsConnected())
			{
				if (!read_header())
					return;

				if (!read_data())
					return;
			}
		}
		#endregion

        #region [구현] [수신] [IO 쓰레드] 버퍼 읽기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private bool recv_buffer(int read_buffer_size)
        {
            var net_stream = _tcp_peer._net_stream;
           
            //-------------------------------------------------------------------------------------
            // 1. 비동기 데이터 읽기(최대한 많이 읽는다)
            //
            try
            {
                while (read_buffer_size > 0)
                {
                    // _recv_buffer._buffer_capacity - _recv_buffer._size
                    var nbytes = net_stream.Read(_recv_buffer._buffer, _recv_buffer._size, read_buffer_size);//.ConfigureAwait(false);
                    if (nbytes == 0)
                    {
                        Disconnect();
                        return false;
                    }

                    _recv_buffer._size += nbytes;
                    read_buffer_size -= nbytes;
                }
            }
            //-------------------------------------------------------------------------------------
            // 2. 예외처리
            //
            catch (Exception e)
            {
                JLogger.WriteFormat("Force Disconnected:[Port:{0}][{1}]", _tcp_peer.Port, e.Message);
                Disconnect();
                #region [백업] Disconnect 대신 OnDisconnected 호출???
                // _netpeer.AddMainThreadCommand(() =>
                //{
                //	_netpeer.OnDisconnected();
                //	//_net_base.Disconnect();
                //});				
                #endregion

                return false;
            }

            return true;
        }
        #endregion

        #region [구현] [수신] [IO 쓰레드] [★★★] 해더 읽기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private bool read_header()
        {
            var net_stream = _tcp_peer._net_stream;
            var recv_stream = _recv_buffer._stream;
            var read_result = false;

            //-------------------------------------------------------------------------------------
            // 1. 특정 서버의 경우 패킷의 시작 태그가 붙어 있다.
            //
            _message_header = NetBase.MakeMessageHeader(_netpeer.MessageHeaderType, 0, 0);

            if (_message_header.HasStartTag)
            {
                while (true)
                {
                    read_result = recv_buffer(1);
                    if (!read_result)
                        return false;
                    if (_message_header.StartTag == _recv_buffer._buffer[0])
                        break;
                    _recv_buffer.Reset();
                }
            }

            //-------------------------------------------------------------------------------------
            // 2. 해더 읽기 (TCP 특성상 부분적으로 데이터 수신시 기다린후 반복해서 읽기)
            //
            var read_header_size = _message_header.HeaderSize;
            if (_message_header.HasStartTag)
                --read_header_size;

            read_result = recv_buffer(read_header_size);
            if (!read_result || (_recv_buffer._size < _message_header.HeaderSize))
                return false;

            _message_header.Read(_recv_buffer._buffer, _recv_buffer._position);
            _recv_buffer.Reset();


            //-------------------------------------------------------------------------------------
            // 3. 해킹 또는 잘못된 데이터인 경우임
            //
            if ((_message_header.PacketSize < _message_header.HeaderSize) || (_message_header.PacketSize > NetBase.MaxReceiveBufferSize))
            {
                if(_message_header.PacketSize < _message_header.HeaderSize)
                    JLogger.Write("[Network] Tool Small Data Size:" + _message_header.PacketSize);
                else
                    JLogger.Write("[Network] Big Data Size:" + _message_header.PacketSize);

                Disconnect();
                return false;
            }
            return true;
        }
        #endregion

        #region [구현] [수신] [IO 쓰레드] [★★★★★] 데이터 읽기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // 서버의 경우 RecvBuffer의 경우 사용자별로 생성되기 때문에 버퍼를 크게할 수 없다. 또한 TCP에 한번에 전송되는 데이터 사이즈도 크지 않기 때문에
        // 대용량 데이터를 수신할 경우 반복적으로 데이터를 읽을 필요가 있다.
        //
        //      1. 비동기 데이터 수신
        //      2. (작은용량) -> 수신큐에 추가
        //      3. (대용량) -> 큰버퍼에 저장 후 수신큐에 추가
        //
        private bool read_data()
        {
            // todo(검토): 작은 데이터를 여러번 수신하면 느려지지 않나??? 
            // 최대한 많이 받고 메시지 잘라서 써야 될 듯 한데... 의미가 있는지 모르겠네...

            var net_stream = _tcp_peer._net_stream;
            var recv_stream = _recv_buffer._stream;
            var data_size = _message_header.DataSize;
            var res_data_stream = JMemoryPool.GetMemoryStream(_message_header.PacketSize);


            //-------------------------------------------------------------------------------------
            // 1. 비동기 데이터 수신
            //            
            var remain_data_size = data_size;
            while (remain_data_size > 0)
            {
                // (대용량) 데이터의 경우 분할 수신
                var read_data_size = Math.Min(remain_data_size, _recv_buffer._buffer_capacity);
                var read_result = recv_buffer(read_data_size);
                if (!read_result)
                    return false;
                
                _recv_buffer.Read(res_data_stream, read_data_size);
                _recv_buffer.Reset();
                remain_data_size -= read_data_size;
            }


			//-------------------------------------------------------------------------------------
			// 2. 결과 저장
			//
			var customParsing = false;
			res_data_stream.Position = 0;
			if (CustomMessageParser != null)
				customParsing = CustomMessageParser(_message_header, res_data_stream, true);
		
			if(!customParsing)
				enqueue_recv_message(res_data_stream);
            _recv_buffer.Reset();
            return true;

        }
        #endregion

        #region [구현] [수신] [IO 쓰레드] MemoryStream -> NetBuffer(생성) -> RecvQueue(저장)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void enqueue_recv_message(MemoryStream out_stream)
        {
            //-------------------------------------------------------------------------------------
            // todo: 유효성 검사
            //
            //if (!NetBase.ValidPacket(_message_header))
            //{
            //	JLogger.WriteFormat(@"Invalid Packet ID({0}).", _message_header._message_id);
            //	return;
            //}

            var net_buffer = new NetBuffer();
            net_buffer._stream = out_stream;
            net_buffer._position = 0;
            net_buffer._size = _message_header.PacketSize;
            net_buffer._message_id = _message_header.MessageID;
            net_buffer.Header = _message_header;
            _queued_recvs.Enqueue(net_buffer);
            //_read_header = false;
        }
        #endregion

       
    }

}
