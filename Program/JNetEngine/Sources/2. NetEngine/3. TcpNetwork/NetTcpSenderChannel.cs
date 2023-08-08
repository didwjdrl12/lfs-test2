using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace J2y.Network
{


	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// NetTcpSenderChannel
	//
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public class NetTcpSenderChannel : NetSenderChannel_base
	{
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [상수] 설정
        private static bool c_using_final_buffer = false;
        #endregion


        #region [Variable] Base
        private NetTcpPeer_base _tcp_peer;
        #endregion


        #region [Variable] Final Buffer(IO Thread)
        protected NetBuffer _final_buffer;
        protected BinaryWriter _final_writer;        
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        internal NetTcpSenderChannel(NetPeer_base netpeer)
			: base(netpeer, 0)
		{
			_tcp_peer = netpeer as NetTcpPeer_base;

            if (c_using_final_buffer)
            {
                ResizeFianlBuffer(NetBase.NetworkMTU);
                _final_writer = new BinaryWriter(_final_buffer._stream);
            }
        }
		#endregion





		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// [IO 쓰레드] 메시지 송신
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [송신] [IO 쓰레드] 메시지(1개) 전송
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		protected override void write_message()
		{
			var net_stream = _tcp_peer._net_stream;
			var succeed = true;

			while (_queued_sends.Count > 0)
			{
				//--------------------------------------------------------------------
				// 1. 메시지큐에서 데이터를 추출한다.
				// 
				NetBuffer send_buffer = null;
				if (!_queued_sends.TryDequeue(out send_buffer))
					break;

				//--------------------------------------------------------------------
				// 2. 최대 버퍼 사이즈를 초과한 경우(대용량 데이터) 분할 전송
				//
				if (send_buffer._size > NetBase.NetworkMTU)
				{
					succeed = write_big_buffer(send_buffer._buffer, send_buffer._size);
				}
				//--------------------------------------------------------------------
				// 3. 일반 데이터 비동기 전송
				//
				else
				{
					succeed = write_buffer(send_buffer._buffer, 0, send_buffer._size);
				}

				if (send_buffer._use_memorypool)
					JMemoryPool.ReturnMemoryStream(send_buffer._stream);
				if (!succeed)
					break;
			}
		}
		#endregion

       
        #region [IO 쓰레드] [송신] 전송할 메시지가 있는지 확인
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override bool need_write_message()
        {
            var send_message = false;
            var old_message = (JTimer.GetElaspedTick(_send_timer) > NetBase.SendMTUMaxTime);
            if ((_queued_sends.Count) > 0)
                send_message = true;
            if (c_using_final_buffer && old_message && (_final_buffer._size > 0))
                send_message = true;

            return send_message;
        }
        #endregion

        #region [IO 쓰레드] [송신] [구현] 대용량 버퍼 (분할)전송
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private bool write_big_buffer(byte[] buffer, int total_buffer_size)
        { 
            var net_stream = _tcp_peer._net_stream;

            var buffer_pos = 0;
            while (buffer_pos < total_buffer_size)
            {
                var buffer_size = Math.Min(NetBase.NetworkMTU, total_buffer_size - buffer_pos);

                var succeed = write_buffer(buffer, buffer_pos, buffer_size);
                if (!succeed)
                    return false;
                buffer_pos += buffer_size;
            }

            return true;
        }
        #endregion

        #region [IO 쓰레드] [송신] [구현] 버퍼 실전송
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private bool write_buffer(byte[] buffer, int offset, int data_size)
        {
            var net_stream = _tcp_peer._net_stream;

            //--------------------------------------------------------------------
            // 1. 비동기 메시지 전송
            // 
            var exception_count = 0;
            var succeed_write = true;
            do
            {
                try
                {
                    net_stream.Write(buffer, offset, data_size);//.ConfigureAwait(false);                    
                }
                catch (Exception e)
                {
                    succeed_write = false;
                    JLogger.WriteError("[ERROR]" + e.Message);
					Thread.Sleep(1000);

					// 예외 발생시 어떻게 해야 되지?? -> 다시 시도.. 횟수가 많으면 접속 끊기
					++exception_count;
                    if (exception_count > 5)
                    {
                        Disconnect();
						Thread.Sleep(1000);
						return false;
                    }
                }

            } while (!succeed_write);

            //--------------------------------------------------------------------
            // 2. 버퍼 리셋
            // 
            _send_buffer_num_messages = 0;
            _send_timer = JTimer.GetCurrentTick();
            //m_statistics.PacketSent(_send_write_position, _send_buffer_num_messages);			

            return true;
        }
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [IO 쓰레드] [FinalBuffer] 메시지 송신
        //
        // [백업용] 사용 안할 듯..
        //      메시지를 모아서 한꺼번에 전송하기 위해 제작하였으나 임시 버퍼에 Write하는 작업이 한번 더 생김
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [★★★★★] [IO 쓰레드] [송신] [FinalBuffer] [구현] 메시지(1개) 전송 (FinalBuffer 사용)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // 메시지를 모아서 한꺼번에 전송하기 위해 버퍼에만 기록하다가 
        // 일정 크기 이상이 되면 데이터를 전송한다.
        //
        //      1. 메시지큐에서 데이터를 추출
        //      2. (대용량) -> 분할 전송
        //      3. (작은용량) -> 모아서 전송
        //
        public async Task vFinalBuffer_write_message_async()
		{
			var net_stream = _tcp_peer._net_stream;
			var final_stream = _final_buffer._stream;
            var old_message = (JTimer.GetElaspedTick(_send_timer) > NetBase.SendMTUMaxTime);
            

            while (_queued_sends.Count > 0)
			{
                //--------------------------------------------------------------------
                // 1. 메시지큐에서 데이터를 추출한다.
                // 
                NetBuffer send_buffer = null;
                if (!_queued_sends.TryDequeue(out send_buffer))
                    break;
                
                                        
                //--------------------------------------------------------------------
                // 2. 최대 버퍼 사이즈를 초과한 경우(대용량 데이터) 즉시 전송
                //
                if (send_buffer._size > _final_buffer._buffer_capacity)
                {
                    await vFinalBuffer_flush_buffer(); 
                    await vFinalBuffer_write_big_buffer(send_buffer);
                    continue;
                }
                //--------------------------------------------------------------------
                // 3. 현재 남은 버퍼 사이즈를 초과한 경우 버퍼를 먼저 비우고(Flush) 전송
                //
                else if (send_buffer._size > _final_buffer._remain_buffer_capacity)
                {
                    await vFinalBuffer_flush_buffer();                    
                }
                

                //--------------------------------------------------------------------
                // 4. (대부분) 버퍼에 여유가 있는 경우
                //                
                final_stream.Write(send_buffer._buffer, 0, send_buffer._size);
                JMemoryPool.ReturnMemoryStream(send_buffer._stream);
                var flush_net_stream = send_buffer._immediate_send;

                //--------------------------------------------------------------------
                // 5. 최종 데이터 전송
                // 
                var flush_data_size = _final_buffer._position;
				if ((flush_data_size >= NetBase.NetworkMTU) || old_message)
					flush_net_stream = true;

				if (flush_net_stream)
				{
                    await vFinalBuffer_flush_buffer();
                    old_message = false;
                }
			}
		}
        #endregion

        #region [IO 쓰레드] [송신] [FinalBuffer] [구현] 대용량 버퍼 (분할)전송
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private async Task vFinalBuffer_write_big_buffer(NetBuffer send_buffer)
        {
            var net_stream = _tcp_peer._net_stream;

            var buffer_pos = 0;
            var total_buffer_size = send_buffer._size;
            while (buffer_pos < total_buffer_size)
            {
                var buffer_size = Math.Min(NetBase.NetworkMTU, total_buffer_size - buffer_pos);

                await net_stream.WriteAsync(send_buffer._buffer, buffer_pos, buffer_size);
                buffer_pos += buffer_size;
            }

            JMemoryPool.ReturnMemoryStream(send_buffer._stream);            
        }
        #endregion

        #region [IO 쓰레드] [송신] [FinalBuffer] [구현] 버퍼 실전송
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private async Task vFinalBuffer_flush_buffer()
        {
            var net_stream = _tcp_peer._net_stream;
            var final_stream = _final_buffer._stream;
            var flush_data_size = (int)final_stream.Position;

            //--------------------------------------------------------------------
            // 1. 비동기 메시지 전송
            // 
            var exception_count = 0;
            while (true)
            {
                try
                {
                    await net_stream.WriteAsync(final_stream.GetBuffer(), 0, flush_data_size);//.ConfigureAwait(false);
                    break;
                }
                catch (Exception e)
                {
                    JLogger.WriteError("[ERROR]" + e.Message);
                    await Task.Delay(1000);

                    // 예외 발생시 어떻게 해야 되지?? -> 다시 시도.. 횟수가 많으면 접속 끊기
                    ++exception_count;
                    if (exception_count > 5)
                    {
                        Disconnect();
                        await Task.Delay(1000);
                    }
                }
            }           

            //--------------------------------------------------------------------
            // 2. 버퍼 리셋
            // 
            final_stream.Seek(0, SeekOrigin.Begin);
            _send_buffer_num_messages = 0;
            _send_timer = JTimer.GetCurrentTick();
            
            //m_statistics.PacketSent(_send_write_position, _send_buffer_num_messages);			
        }
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 유틸
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [유틸] [FinalBuffer] Resize 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        internal int ResizeFianlBuffer(int buffer_size)
        {
            var pool_buffer_size = JMemoryPool.GetMemoryPoolBufferSize(buffer_size);

            // 최초 생성시
            if (null == _final_buffer)
            {
                _final_buffer = new NetBuffer(JMemoryPool.GetBuffer(pool_buffer_size));
            }
            // 용량 증가 -> 기존 버퍼 복사
            else if (_final_buffer._buffer_capacity < pool_buffer_size)
            {
                var new_buffer = new NetBuffer(JMemoryPool.GetBuffer(pool_buffer_size));
                new_buffer.Clone(_final_buffer);

                JMemoryPool.ReturnMemoryStream(_final_buffer._stream);
                _final_buffer = new_buffer;
            }

            return _final_buffer._buffer_capacity;
        }
        #endregion
    }

}
