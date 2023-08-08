using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace J2y.Network
{
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// NetMessage_dispatcher_base
	//
	//
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public class NetMessage_dispatcher_base
	{

		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Variable
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [Variable] 메시지 핸들러
            
		protected Dictionary<int, Action<NetPeer_base, BinaryReader>> _message_handlers;
        protected Dictionary<string, Action<NetPeer_base, BinaryReader>> _simple_message_handlers;
        protected Dictionary<int, Action<NetPeer_base, BinaryReader>> _short_message_handlers;

        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public NetMessage_dispatcher_base()
		{
			_message_handlers = new Dictionary<int, Action<NetPeer_base, BinaryReader>>();
            _simple_message_handlers = new Dictionary<string, Action<NetPeer_base, BinaryReader>>();
            _short_message_handlers = new Dictionary<int, Action<NetPeer_base, BinaryReader>>();

            register_base_message_handlers();
        }
        #endregion

        #region [핸들러] 기본 메시지 핸들러 등록(파일, RPC)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        protected virtual void register_base_message_handlers()
        {
            // 기본 파일 수신 핸들러 등록
            RegisterMessageHandler(JNetMessageProtocol.FileTransfer, (peer, reader) =>
            {
                peer.OnRecv_FileTransfer(reader);
            });
            RegisterMessageHandler(JNetMessageProtocol.RPC, (peer, reader) =>
            {
                JRpcMediator.OnNetRecvRPC(peer, reader);
            });
            RegisterMessageHandler(JNetMessageProtocol.RPCResponse, (peer, reader) =>
            {
                JRpcMediator.OnNetRecvRPCResponse(peer, reader);
            });
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++led++++++++++++++++++++++++++++++++++++++++++
        // 넷메시지 디스패치
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [메시지 핸들러] 등록
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void RegisterMessageHandler(int id, Action<NetPeer_base, BinaryReader> handler, bool overwrite = false)
		{
			if (_message_handlers.ContainsKey(id))
			{
				if (overwrite)
					_message_handlers[id] = handler;
            }
			else
			{
				_message_handlers.Add(id, handler);
			}
		}
        #endregion

        #region [메시지 핸들러] [Simple] 등록
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void RegisterSimpleMessageHandler(string id, Action<NetPeer_base, BinaryReader> handler, bool overwrite = false)
        {
            if (_simple_message_handlers.ContainsKey(id))
            {
                if (overwrite)
                    _simple_message_handlers[id] = handler;
            }
            else
            {
                _simple_message_handlers.Add(id, handler);
            }
        }
        #endregion

        #region [메시지 핸들러] [Short] 등록
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void RegisterShortMessageHandler(int id, Action<NetPeer_base, BinaryReader> handler, bool overwrite = false)
        {
            if (_short_message_handlers.ContainsKey(id))
            {
                if (overwrite)
                    _short_message_handlers[id] = handler;
            }
            else
            {
                _short_message_handlers.Add(id, handler);
            }
        }
        #endregion


        #region [디스패치] 메시지
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DispatchMessages(NetPeer_base sender, int message_type, NetBuffer net_buffer, BinaryReader reader)
		{
			if (_message_handlers.TryGetValue(message_type, out Action<NetPeer_base, BinaryReader> handler))
			{
				sender._last_recv_buffer = net_buffer;
				sender.UpdateKeepAliveTimer();

				try
				{
					handler(sender, reader);
				}
				catch (EndOfStreamException e)
				{
					JLogger.Write("[Exception][DispatchMessages][EndOfStreamException]:" + e.Message);
				}
				catch (ObjectDisposedException e)
				{
					JLogger.Write("[Exception][DispatchMessages][ObjectDisposedException]:" + e.Message);
				}
				catch (IOException e)
				{
					JLogger.Write("[Exception][DispatchMessages][IOException]:" + e.Message);
				}
			}
			else
			{
				//# JLogger.Write("No handler for message id " + message_type);
			}
		}
        #endregion
        
        #region [디스패치] [Simple] 메시지
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DispatchSimpleMessages(NetPeer_base sender, string message_type, NetBuffer net_buffer, BinaryReader reader)
        {
            if (_simple_message_handlers.TryGetValue(message_type, out Action<NetPeer_base, BinaryReader> handler))
            {
                sender._last_recv_buffer = net_buffer;
                sender.UpdateKeepAliveTimer();

                try
                {
                    handler(sender, reader);
                }
                catch (EndOfStreamException e)
                {
                    JLogger.Write("[Exception][DispatchMessages][EndOfStreamException]:" + e.Message);
                }
                catch (ObjectDisposedException e)
                {
                    JLogger.Write("[Exception][DispatchMessages][ObjectDisposedException]:" + e.Message);
                }
                catch (IOException e)
                {
                    JLogger.Write("[Exception][DispatchMessages][IOException]:" + e.Message);
                }
            }
            else
            {
                JLogger.Write("No handler for message id " + message_type);
            }
        }
        #endregion

        #region [디스패치] [Short] 메시지
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DispatchShortMessages(NetPeer_base sender, int message_type, NetBuffer net_buffer, BinaryReader reader)
        {
            if (_short_message_handlers.TryGetValue(message_type, out Action<NetPeer_base, BinaryReader> handler))
            {
                sender._last_recv_buffer = net_buffer;
                sender.UpdateKeepAliveTimer();

                try
                {
                    handler(sender, reader);
                }
                catch (EndOfStreamException e)
                {
                    JLogger.Write("[Exception][DispatchMessages][EndOfStreamException]:" + e.Message);
                }
                catch (ObjectDisposedException e)
                {
                    JLogger.Write("[Exception][DispatchMessages][ObjectDisposedException]:" + e.Message);
                }
                catch (IOException e)
                {
                    JLogger.Write("[Exception][DispatchMessages][IOException]:" + e.Message);
                }
            }
            else
            {
                JLogger.Write("No handler for message id " + message_type);
            }
        }
        #endregion
        

        #region [async] Wait Message
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<(NetPeer_base, BinaryReader)> WaitMessage(int message_type, bool overwrite = false)
        {
            var tcs = new TaskCompletionSource<(NetPeer_base, BinaryReader)>();
            RegisterMessageHandler(message_type, (peer, reader) =>
            {
                tcs.SetResult((peer, reader));
            }, overwrite);
            var result = await tcs.Task;
            return result;
        }
        #endregion

        #region [async] Wait Simple Message
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<(NetPeer_base, BinaryReader)> WaitSimpleMessage(string message_type, bool overwrite = false)
        {
            var tcs = new TaskCompletionSource<(NetPeer_base, BinaryReader)>();
            RegisterSimpleMessageHandler(message_type, (peer, reader) =>
            {
                tcs.SetResult((peer, reader));
            }, overwrite);
            var result = await tcs.Task;
            return result;
        }
        #endregion

        #region [async] Wait Short Message
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<(NetPeer_base, BinaryReader)> WaitShortMessage(int message_type, bool overwrite = false)
        {
            var tcs = new TaskCompletionSource<(NetPeer_base, BinaryReader)>();
            RegisterShortMessageHandler(message_type, (peer, reader) =>
            {
                tcs.SetResult((peer, reader));
            }, overwrite);
            var result = await tcs.Task;
            return result;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++led++++++++++++++++++++++++++++++++++++++++++
        // 유틸
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [유틸] ReadNetData
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T ReadNetData<T>(BinaryReader reader) where T : JNetData, new()
        {
        	var netdata = new T();
        	netdata.Read(reader);
        	return netdata;
        }
        #endregion

        #region [유틸] GetMessageHandlers
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Dictionary<int, Action<NetPeer_base, BinaryReader>> GetMessageHandlers()
        {
            return _message_handlers;
        }
        #endregion
    }


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // NetMessage_dispatcher_default
    //
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class NetMessage_dispatcher_default : NetMessage_dispatcher_base
	{
		#region [Init]
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public NetMessage_dispatcher_default()
		{ }
		#endregion
	}

}