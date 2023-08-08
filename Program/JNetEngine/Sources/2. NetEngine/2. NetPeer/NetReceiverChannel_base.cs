using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace J2y.Network
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // NetReceiverChannel_base
    //
    //      1. [메인쓰레드] 메시지 수신  
    //      2. [IO 쓰레드] (비동기) 데이터 수신
    //      3. 바이너리 데이터 수신
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class NetReceiverChannel_base
	{
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Variable
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
		#region [Variable] Base
		protected NetPeer_base _netpeer;
        protected CancellationTokenSource _cancel_token;
        protected bool _loop_stop;
		#endregion

		#region [Variable] Recv Message(NetBuffer)(Main Thread)
		protected NetQueue<NetBuffer> _queued_recvs;
		protected INetMessageHeader _message_header;
		protected bool _read_header;
		#endregion

		#region [Variable] Recv Buffer(IO Thread)
		protected NetBuffer _recv_buffer;
        #endregion

        #region [Variable] File
        protected Dictionary<long, byte[]> _file_recv_datas = new Dictionary<long, byte[]>();
        private long _file_current_guid;
        private int _file_total_chunk_count;
        private int _file_current_chunk_index;
        private int _file_last_chunk_size;
        #endregion

        public Action<NetPeer_base, INetMessageHeader, NetBuffer, BinaryReader> CustomDispatch;
		public Func<INetMessageHeader, MemoryStream, bool, bool> CustomMessageParser;


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 0. Base Methods
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [Init] 생성자
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public NetReceiverChannel_base(NetPeer_base net_base)
		{
			_netpeer = net_base;
			
			_message_header = NetBase.MakeMessageHeader(net_base.MessageHeaderType, 0, 0);
            _recv_buffer = new NetBuffer(NetBase.NetworkMTU);
			_queued_recvs = new NetQueue<NetBuffer>(8);
			_cancel_token = new CancellationTokenSource();
		}
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. [메인쓰레드] 메시지 수신  
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [메인쓰레드] 메시지 디스패치
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        internal virtual void DispatchMessagess(NetMessage_dispatcher_base dispatcher)
		{
			if(null == dispatcher)
			{
				_queued_recvs.Clear();
				return;
			}

			while (_queued_recvs.Count > 0)
			{
				if (_queued_recvs.TryDequeue(out NetBuffer recv_msg))
				{
					using (var reader = new BinaryReader(recv_msg._stream))
					{
                        CustomDispatch?.Invoke(_netpeer, recv_msg.Header, recv_msg, reader);

                        switch (recv_msg._message_id)
                        {
                            case JNetMessageProtocol.SimpleMessage:
                            {
								if (CustomDispatch == null) // 임시
								{
									var message_type = reader.ReadString();
									dispatcher.DispatchSimpleMessages(_netpeer, message_type, recv_msg, reader);
								}
                                break;
                            }

                            case JNetMessageProtocol.ShortMessage:
                            {
                                var message_type = reader.ReadInt32();
                                dispatcher.DispatchShortMessages(_netpeer, message_type, recv_msg, reader);
                                break;
                            }

                            default:
                            {
                                dispatcher.DispatchMessages(_netpeer, recv_msg._message_id, recv_msg, reader);
                                break;
                            }
                        }

                        JMemoryPool.ReturnMemoryStream(recv_msg._stream);
                    }
				}
			}
		}
		#endregion



		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 2. [IO 쓰레드] (비동기) 데이터 수신
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [IO 쓰레드] [수신] 데이터 읽기 루프
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual void ReadLoop() { }
		#endregion


		#region [IO 쓰레드] 비동기 데이터 읽기
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual async Task LoopAsyncRead()
		{
			while (!_loop_stop && !_cancel_token.IsCancellationRequested && _netpeer.IsConnected())
			{
				//while (!net_stream.DataAvailable)
				//{
				//	await Task.Delay(100);
				//}

				//int timeout = 60000; // 60초
				//var task_recv = read_message_async();

				//if (await Task.WhenAny(task_recv, Task.Delay(timeout, _cancel_token.Token)) == task_recv)
				//{					
				//}
				//else
				//{
				//	// 접속 끊김??
				//	// 클라이언트 제거???
				//	JLogger.Write("[LoopAsyncRead] Timeout");
				//}

				await read_message_async();
			}
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual async Task read_message_async()
		{
			await Task.Delay(100);
		}
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
        // 3. 바이너리 데이터 수신
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        // TODO: 스트림 수신(네트워크 수신하면서 파일에 즉시 쓰기)
        // FIXME: 수신 중간에 네트워크 끊기면 기존 데이터 삭제?

        #region [바이너리] 데이터 수신
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual bool OnRecv_FileTransfer(BinaryReader reader)
        {
            var tranState = (eFileTransferState)reader.ReadInt32();

            if (tranState == eFileTransferState.Start)
            {
                _file_current_guid = reader.ReadInt64();
                var dataSize = reader.ReadInt32();
                _file_total_chunk_count = reader.ReadInt32();
                _file_last_chunk_size = reader.ReadInt32();
                _file_current_chunk_index = 0;
                _file_recv_datas[_file_current_guid] = new byte[dataSize];

                JLogger.Write(string.Format("[바이너리] 수신시작, 크기:{0}, 청크{1}, 마지막청크{2}", dataSize, _file_total_chunk_count, _file_last_chunk_size));
            }

            else if (tranState == eFileTransferState.Chunks)
            {
                var recvData = _file_recv_datas[_file_current_guid];

                // Last Chunk
                if (_file_current_chunk_index == _file_total_chunk_count)
                {
                    var offset = _file_total_chunk_count * NetBase.FileChunkSize;
                    reader.Read(recvData, offset, _file_last_chunk_size);

                    JLogger.Write(string.Format("[바이너리] 마지막 청크 수신-{0}", _file_current_chunk_index));
                }
                else
                {
                    var offset = _file_current_chunk_index * NetBase.FileChunkSize;
                    reader.Read(recvData, offset, NetBase.FileChunkSize);

                    ++_file_current_chunk_index;
                    JLogger.Write(string.Format("[바이너리] 청크 수신, 청크({0}/{1})", _file_current_chunk_index, _file_total_chunk_count));
                }
            }

            else if (tranState == eFileTransferState.End)
            {
                var recvData = _file_recv_datas[_file_current_guid];

                // todo: 이벤트 발생
                JLogger.Write(string.Format("[파일 전송] 성공, 크기={0}, 청크={1}", recvData.Length, _file_total_chunk_count));

                //JFile.SaveBinaryFile("test3.txt", _fileTran_recvDatas[_fileTran_guid]);
            }

            return false;
        }
        #endregion

        #region [바이너리] 수신된 데이터 얻기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public byte[] GetFileTransferData(long file_guid, bool removeFromRepository = true)
        {
            if (!_file_recv_datas.ContainsKey(file_guid))
                return null;
            var recvData = _file_recv_datas[file_guid];
            if (removeFromRepository)
                _file_recv_datas.Remove(file_guid);
            return recvData;
        }
        #endregion

        #region [바이너리] 수신된 데이터 파일 저장
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool SaveFileTransferData(long file_guid, string filepath, bool removeFromRepository = true)
        {
            var recv_data = GetFileTransferData(file_guid, removeFromRepository);
            if (null == recv_data)
                return false;

            File.WriteAllBytes(filepath, recv_data);
            return true;
        }
        #endregion
    }

}
