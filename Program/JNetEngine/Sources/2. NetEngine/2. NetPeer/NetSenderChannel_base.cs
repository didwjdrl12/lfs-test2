using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace J2y.Network
{


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // NetSenderChannel_base (IO Thread)
    //      : NetConnection(NetPeer_base)당 하나씩 생성
    //
    //      1. [메인쓰레드] 메시지 송신
    //      2. [IO 쓰레드] 메시지 송신
    //      3. 바이너리 대용량 데이터(파일) 전송
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class NetSenderChannel_base
	{
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Variable
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


		#region [Variable] Base
		protected NetPeer_base _netpeer;
		private CancellationTokenSource _cancel_token;
		private bool _loop_stop;

        protected long _send_timer;     // IO Thread
        #endregion

        #region [Variable] Send Message(NetBuffer)
        protected NetQueue<NetBuffer> _queued_sends;
		protected INetMessageHeader _message_header;
		protected int _send_sequence_number;
		protected int _send_buffer_num_messages;          // 버퍼당 전송 메시지 개수
		#endregion
        


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 0. Base Methods
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [Init] 생성자
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		internal NetSenderChannel_base(NetPeer_base net_base, int windowSize)
		{
			_netpeer = net_base;
			_send_sequence_number = 0;
			_send_timer = JTimer.GetCurrentTick();

			_queued_sends = new NetQueue<NetBuffer>(8);
			_message_header = NetBase.MakeMessageHeader(net_base.MessageHeaderType, 0, 0);
			_cancel_token = new CancellationTokenSource();
		}
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. [메인쓰레드] 메시지 송신
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [메인 쓰레드] [송신] JNetMessage
        //------------------------------------------------------------------------------------------------------------------------------------------------------		
        public virtual eNetSendResult SendMessage(JNetMessage message)
        {
            switch (message._packing_type)
            {
                case eNetPackingType.Default:
                    {
                        var stream = new MemoryStream(message._capacity);// ★★★★버그: JMemoryPool.GetMemoryStream(max_buffer_size);
                        return send_message_impl(message._message_type, (writer) =>
                        {
                            message._fun_writer?.Invoke(writer);
                            foreach (var param in message._parameters)
                            {
                                if(param is JNetData)
                                {
                                    ((JNetData)param).Write(writer);
                                }
                                else
                                    JNet.PacketOutMsg(writer, param);
                            }
                        }, stream, message._immediate);
                    }

                case eNetPackingType.Buffer:
                    {
                        var stream = new MemoryStream(message._capacity);// ★★★★버그: JMemoryPool.GetMemoryStream(max_buffer_size);
                        return send_message_impl(message._message_type, (writer) =>
                        {
                            message._fun_writer?.Invoke(writer);
                            //writer.Write(message._data_size.ToBigEndian());
                            writer.Write(message._data_buffer, 0, message._data_size);
                        }, stream, message._immediate);
                    }
                    
                case eNetPackingType.NetData:
                    {
                        var stream = new MemoryStream(message._capacity);  // JMemoryPool.GetMemoryStream(message._capacity);
                        return send_message_impl(message._message_type, (writer) =>
                        {
                            message._fun_writer?.Invoke(writer);
                            if (!string.IsNullOrEmpty(message._result))
                                writer.Write(message._result);
                            if (message._netdata != null)
							{
								//JNewSerialization.Serialize(writer, message.NetData);
								message._netdata.Write(writer);
							}
							JNet.PacketOutMsg(writer, message._parameters);
                        }, stream, message._immediate);
                    }
                    
                case eNetPackingType.UserWriter:
                    {
                        var stream = new MemoryStream(message._capacity);// ★★★★버그: JMemoryPool.GetMemoryStream(max_buffer_size);
                        return send_message_impl(message._message_type, message._fun_writer, stream, message._immediate);
                    }
            }

            return eNetSendResult.UnknownError;
        }
        #endregion

        #region [메인 쓰레드] [송신] [★★★] 메시지 생성 후 큐에 넣기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        protected virtual eNetSendResult send_message_impl(int message_type, Action<BinaryWriter> fun_write, MemoryStream stream, bool immediate = true)
        {
            using (var writer = new BinaryWriter(stream))
            {
                //--------------------------------------------------------------------------------------------
                // 1. Data(사이즈를 구하기 위해서 데이터를 먼저 쓴다)
                //
                writer.Seek(_message_header.HeaderSize, SeekOrigin.Begin);
                fun_write(writer);

                //--------------------------------------------------------------------------------------------
                // 2. header
                //
                var packet_size = (int)stream.Position;
                var data_size = packet_size - _message_header.HeaderSize;
                //MakeMessageHeader(stream.GetBuffer(), 0, packet_size, message_type, ref _message_header);
                _message_header = NetBase.MakeMessageHeader(_netpeer.MessageHeaderType, data_size, message_type);
                writer.Seek(0, SeekOrigin.Begin);
                _message_header.Write(writer);

                //--------------------------------------------------------------------------------------------
                // 3. enqueue
                //
                var net_buffer = new NetBuffer
                {
                    _stream = stream,
                    _use_memorypool = false,
                    _position = 0,
                    _size = packet_size,
                    _immediate_send = immediate
                };
                _queued_sends.Enqueue(net_buffer);
                _send_buffer_num_messages++;
            }

            return eNetSendResult.Sent;
        }
        #endregion



        #region [메인 쓰레드] [송신] 메시지 해더 없이 데이터를 직접 보내는 경우가 있음
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual eNetSendResult SendRawData(Action<BinaryWriter> fun_write)
        {
            var stream = new MemoryStream();
            return SendRawData(fun_write, new MemoryStream());
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual eNetSendResult SendRawData(Action<BinaryWriter> fun_write, MemoryStream stream)
        {
            using (var writer = new BinaryWriter(stream))
            {
                fun_write(writer);

                var packet_size = (int)stream.Position;
                var net_buffer = new NetBuffer
                {
                    _stream = stream,
                    _use_memorypool = false,
                    _position = 0,
                    _size = packet_size,
                    _immediate_send = true
                };
                
                _queued_sends.Enqueue(net_buffer);
                _send_buffer_num_messages++;
            }

            return eNetSendResult.Sent;
        }
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 2. [IO 쓰레드] 메시지 송신
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [IO 쓰레드] [송신] [★★★] 전송 프로세스
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual void WriteLoop()
		{
			while (true)
			{
				if (_loop_stop || _cancel_token.IsCancellationRequested || !_netpeer.IsConnected())
					break;

				var has_msg = need_write_message();
				if (has_msg)
					write_message();
				else
					Thread.Sleep(50);
			}
		}
		protected virtual void write_message() { }
		#endregion



		#region [IO 쓰레드] [송신] 전송할 메시지가 있는지 확인
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual bool need_write_message()
        {
            var send_message = false;
            var old_message = (JTimer.GetElaspedTick(_send_timer) > NetBase.SendMTUMaxTime);
            if ((_queued_sends.Count) > 0)
                send_message = true;

            return send_message;
        }
        #endregion

        #region [IO 쓰레드] [송신] [선언] 전송 프로세스
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual async Task write_message_async()
		{
			await Task.Delay(100);
		}
        #endregion

        #region [IO 쓰레드] [송신] Stop
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Stop()
		{
			_loop_stop = true;
			_cancel_token.Cancel();
		}
        #endregion

        #region [IO 쓰레드] Disconnect
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Disconnect()
        {
            _netpeer.AddMainThreadCommand(() =>
            {
                Stop();
                //JLogger.Write("IO ReadAsync Disconnected");
                _netpeer.Disconnect();
            });
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 3. 바이너리 대용량 데이터(파일) 전송 (10 MB 이하는 일반 메시지로 전송)
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [바이너리] 파일 전송
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public long SendFile(string filepath)
        {
            // TODO: 스트림 전송(파일에서 읽으면서 바로 전송)

            var file = File.ReadAllBytes(filepath);
            return SendBigData(file);
        }
        #endregion

        #region [바이너리] 데이터 전송 (4G 이하 데이터)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public long SendBigData(byte[] send_data)
        {
            //-------------------------------------------------------------------------------------------------
            // 1. 파일 전송 준비
            //
            var file_guid = JUtil.CreateUniqueId();
            var buffer_size = send_data.Length;
            var whole_chunks = (int)(buffer_size / NetBase.FileChunkSize);
            var last_chunk_size = (int)(buffer_size % NetBase.FileChunkSize);
            //JLogger.Write(string.Format("[바이너리] 전송시작, 크기:{0}, 청크{1}, 마지막청크{2}", dataSize, whole_chunks, last_chunk_size));


            //-------------------------------------------------------------------------------------------------
            // 2. 시작 메시지
            //
            send_message_impl(JNetMessageProtocol.FileTransfer, (writer) =>
            {
                writer.Write((int)eFileTransferState.Start);
                writer.Write(file_guid);
                writer.Write(buffer_size);
                writer.Write(whole_chunks);
                writer.Write(last_chunk_size);
            }, JMemoryPool.GetMemoryStream(NetBase.FileChunkSize + _message_header.HeaderSize), false);
            JLogger.Write("[바이너리] 시작 메시지");


            //-------------------------------------------------------------------------------------------------
            // 3. 파일 청크 전송
            //
            upload_binary_chunks(send_data, whole_chunks, last_chunk_size);


            //-------------------------------------------------------------------------------------------------
            // 4. 파일 전송 종료
            // 
            send_message_impl(JNetMessageProtocol.FileTransfer, (writer) =>
            {
                writer.Write((int)eFileTransferState.End);
            }, JMemoryPool.GetMemoryStream(4096), false);
            JLogger.Write("[바이너리] 파일 전송 완료 메시지");

            return file_guid;
        }
        #endregion

        #region [바이너리] [구현] 청크 분할 후 전송
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void upload_binary_chunks(byte[] send_data, int whole_chunks, int last_chunk_size)
        {
            try
            {
                for (var i = 0; i < whole_chunks; i++)
                {
                    var offset = i * NetBase.FileChunkSize;
                    if (false == upload_binary_piece(send_data, offset, NetBase.FileChunkSize))
                        return;

                    JLogger.Write(string.Format("[바이너리] 청크 전송, 청크({0}/{1})", (i + 1), whole_chunks));
                    //Thread.Sleep(5);
                }

                if (false == upload_binary_piece(send_data, whole_chunks * NetBase.FileChunkSize, last_chunk_size))
                    return;
            }
            catch
            {
                JLogger.Write("[ERROR] 파일 전송 실패");
            }
            // ReSharper restore EmptyGeneralCatchClause
        }
        #endregion

        #region [바이너리] [구현] 청크 전송
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private bool upload_binary_piece(byte[] send_data, int offset, int chunkSize)
        {
            var succeedSending = false;
            do
            {
                var result = send_message_impl(JNetMessageProtocol.FileTransfer, (writer) =>
                {
                    writer.Write((int)eFileTransferState.Chunks);
                    writer.Write(send_data, offset, chunkSize);
                }, JMemoryPool.GetMemoryStream(NetBase.FileChunkSize + _message_header.HeaderSize), false);
                
                if (result != eNetSendResult.Dropped)
                    succeedSending = true;
            }
            while (!succeedSending);

            //JLogger.Write("[바이너리 데이터 전송] :" + offset + ":" + chunkSize);

            return true;
        }
        #endregion

        

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 유틸
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        
        #region [백업] [NetOutgoingMessage] [구현] [송신] [IO 쓰레드] 메시지(1개) 전송
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public async Task write_message_async(NetworkStream net_stream, NetOutgoingMessage om)
        //{
        //	int seqNr = _send_sequence_number;
        //	_send_sequence_number = (_send_sequence_number + 1) % NetBase.NumSequenceNumbers;

        //	//--------------------------------------------------------------------
        //	// 1. 메시지 패킹(Message -> Byte[])
        //	//
        //	_send_write_position = om.Encode(m_sendBuffer, _send_write_position, seqNr);
        //	_send_buffer_num_messages++;

        //	if ((_send_write_position >= NetBase.NetworkMTU) || (JTimer.GetElaspedTick(_send_timer) > NetBase.SendMTUMaxTime))
        //	{
        //		//--------------------------------------------------------------------
        //		// 2. 비동기 메시지 전송
        //		// 
        //		try
        //		{
        //			await net_stream.WriteAsync(m_sendBuffer, 0, _send_write_position).ConfigureAwait(false);
        //		}
        //		catch (Exception e)
        //		{
        //			JLogger.WriteError("[ERROR]" + e.Message);
        //		}

        //		//m_statistics.PacketSent(_send_write_position, _send_buffer_num_messages);
        //		_send_write_position = 0;
        //		_send_buffer_num_messages = 0;
        //		_send_timer = JTimer.GetCurrentTick();
        //	}			
        //}
        #endregion

    }

}
