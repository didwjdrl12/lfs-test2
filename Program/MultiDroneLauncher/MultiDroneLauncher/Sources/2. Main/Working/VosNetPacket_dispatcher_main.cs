//using UnityEngine;
//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.IO;
//using J2y.Network;


////++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
////
//// VosNetPacket_dispatcher_main
////
////
////
////++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//public partial class VosNetPacket_dispatcher_main : NetMessage_dispatcher_base
//{
//    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//    // 변수
//    //
//    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


//    #region 변수


//    #endregion


//    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//    // [Property]
//    //
//    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


//    #region [Property]
//    //------------------------------------------------------------------------------------------------------------------------------------------------------
//    public VosNetServer _net_server
//    {
//        get { return _netpeer as VosNetServer; }
//    }
//    #endregion




//    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//    // 메시지 핸들러들 등록
//    //
//    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//    #region [초기화]
//    //------------------------------------------------------------------------------------------------------------------------------------------------------
//    public VosNetPacket_dispatcher_main(VosNetServer net_server)
//        : base(net_server)
//    {
//        RegisterMessageHandler_user_common();
//    }
//    #endregion


//    #region [유틸] NetClientCapture / NetClientViewer
//    //------------------------------------------------------------------------------------------------------------------------------------------------------
//    public VosNetClientCapture ToNetClientCapture(NetPeer_base netpeer)
//    {
//        return netpeer.Tag as VosNetClientCapture;
//    }
//    //------------------------------------------------------------------------------------------------------------------------------------------------------
//    public VosNetClientViewer ToNetClientViewer(NetPeer_base netpeer)
//    {
//        return netpeer.Tag as VosNetClientViewer;
//    }
//    //------------------------------------------------------------------------------------------------------------------------------------------------------
//    public VosNetClientKinect ToNetClientKinect(NetPeer_base netpeer)
//    {
//        return netpeer.Tag as VosNetClientKinect;
//    }
//    //------------------------------------------------------------------------------------------------------------------------------------------------------
//    public int GetNetClientCaptureIndex(VosNetClientCapture capture)
//    {
//        int index = 0;
//        foreach (var cap in CollectNetClientCaptures())
//        {
//            if (cap == capture)
//                return index;
//            ++index;
//        }
//        return -1;
//    }
//    #endregion

//    #region [핸들러] [유저] 메시지 핸들러 등록
//    ////------------------------------------------------------------------------------------------------------------------------------------------------------
//    //public void RegisterMessageHandlerUser(string id, Action<MdsmUser, BinaryReader> handler, bool write_default_log = true)
//    //{
//    //	if (_message_handlers.ContainsKey(id))
//    //	{
//    //		JLogger.WriteFormat("[ERROR] RegisterMessageHandler{0}", id);
//    //	}
//    //	else
//    //	{
//    //		_message_handlers.Add(id, (sender, reader) =>
//    //		{
//    //			var user = ToUser(sender);
//    //			if (write_default_log)
//    //				JLogger.WriteFormat("{0}:{1}", id, user._netdata._nickname);
//    //			handler(user, reader);
//    //		});
//    //	}
//    //}
//    #endregion


//    #region [유틸] [Broadcast] 
//    //------------------------------------------------------------------------------------------------------------------------------------------------------
//    public IEnumerable<VosNetClientViewer> CollectNetClientViewers()
//    {
//        foreach (var conn in _net_server.m_connections)
//        {
//            var net_pc = ToNetClientViewer(conn);
//            if (null != net_pc)
//                yield return net_pc;
//        }
//    }
//    //------------------------------------------------------------------------------------------------------------------------------------------------------
//    public IEnumerable<VosNetClientCapture> CollectNetClientCaptures()
//    {
//        foreach (var conn in _net_server.m_connections)
//        {
//            var net_pc = ToNetClientCapture(conn);
//            if (null != net_pc)
//                yield return net_pc;
//        }
//    }
//    //------------------------------------------------------------------------------------------------------------------------------------------------------
//    public IEnumerable<VosNetClient_base> CollectNetClients()
//    {
//        foreach (var conn in _net_server.m_connections)
//        {
//            var net_pc = ToNetClientCapture(conn);
//            var net_viewer = ToNetClientViewer(conn);
//            if (net_pc != null)
//                yield return net_pc;
//            if (net_viewer != null)
//                yield return net_viewer;
//        }
//    }
//    //------------------------------------------------------------------------------------------------------------------------------------------------------
//    public void BroadcastViewer(JNetMessage net_message)
//    {
//        foreach (var net_client in CollectNetClientViewers())
//        {
//            net_client._net_peer.SendMessage(net_message);
//        }
//    }
//    #endregion



//    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//    // 메시지 핸들러들 등록
//    //
//    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//    private byte[] _image_buffer = new byte[2048 * 2048 * 3 * 3];

//    public void RegisterMessageHandler_user_common()
//	{
//        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//        // 인증
//        //
//        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//        #region [C->FS] [인증] 보내기
//        //------------------------------------------------------------------------------------------------------------------------------------------------------
//        RegisterSimpleMessageHandler("cs_client_type", (sender, reader) =>
//        {
//            //----------------------------------------------------------------------
//            // 1. 입력 패킷
//            //            
//            var client_type = (eNetClientType)reader.ReadInt32();
//            switch(client_type)
//            {
//                case eNetClientType.Capture: sender.Tag = new VosNetClientCapture(sender); break;
//                case eNetClientType.Viewer: sender.Tag = new VosNetClientViewer(sender); break;
//                case eNetClientType.Kinect: sender.Tag = new VosNetClientKinect(sender); break;
//            }

//            JLogger.WriteFormat("cs_client_type:" + client_type);


//            //----------------------------------------------------------------------
//            // 2. 응답 패킷
//            //
//            sender.SendSimpleMessage("sc_client_type", "ok");


//            if((client_type == eNetClientType.Viewer) && (VosMapManager.Instance._map_buffer != null))
//                sender.SendSimpleMessage("noti_all_point_clouds", VosMapManager.Instance._map_buffer, VosMapManager.Instance._map_buffer.Length);
//        });
//        #endregion



//        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//        // 맵데이터
//        //
//        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//        #region [C->S] 맵데이터 요청
//        //------------------------------------------------------------------------------------------------------------------------------------------------------
//        RegisterSimpleMessageHandler("cs_map_data", (sender, reader) =>
//        {
//            var net_android = ToNetClientCapture(sender);
//            if (null == net_android)
//                return;

//            //----------------------------------------------------------------------
//            // 1. 입력 패킷
//            //
//            JLogger.WriteFormat("cs_map_data:");
//            var map_name = reader.ReadString();
//            var map_data = VosMapManager.Instance.LoadMapData(map_name);
//            if (null == map_data)
//            {
//                JLogger.WriteError("[ERROR] MapFile Is Not Exist." + map_name);
//                return;
//            }


//            //----------------------------------------------------------------------
//            // 2. 응답 패킷
//            //
//            var file_guid = sender.SendBigData(map_data);
//            sender.SendSimpleMessage("sc_map_data", file_guid);
//        });
//        #endregion



//        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//        // 영상 
//        //
//        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//        #region [C->S] 카메라 영상
//        //------------------------------------------------------------------------------------------------------------------------------------------------------
//        RegisterSimpleMessageHandler("cs_camera_image", (sender, reader) =>
//        {
//            var net_android = ToNetClientCapture(sender);
//            if (null == net_android)
//                return;

//            //----------------------------------------------------------------------
//            // 1. 입력 패킷
//            //
//            JLogger.WriteFormat("cs_camera_image:");

//            // todo: 사용자가 여러명일 경우 NetClient에 데이터 저장
//            var buffer_size = reader.ReadInt32();
//            reader.Read(_image_buffer, 0, buffer_size);

//            //----------------------------------------------------------------------
//            // 2. 응답 패킷
//            //
//            sender.SendSimpleMessage("sc_camera_image_ack");

//            BroadcastViewer(JNetMessage.MakeSimple("noti_camera_image", (writer) =>
//            {
//                writer.Write(buffer_size);
//                writer.Write(_image_buffer, 0, buffer_size);
//            }));
//        });
//        #endregion


//        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//        // 카메라 좌표
//        //
//        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//        #region  [C->S] 카메라 좌표
//        //------------------------------------------------------------------------------------------------------------------------------------------------------
//        RegisterSimpleMessageHandler("cs_camera_matrix", (sender, reader) =>
//        {
//            var net_capture = ToNetClientCapture(sender);
//            if (null == net_capture)
//                return;
//            //JLogger.WriteFormat("cs_camera_matrix:");

//            // matrix? quaternion?
//            var user_index = GetNetClientCaptureIndex(net_capture);
//            var size = reader.ReadInt32();
//            if (null == net_capture._camera_info)
//                net_capture._camera_info = new double[size];
//            for (int i = 0; i < size; ++i)
//                net_capture._camera_info[i] = reader.ReadDouble();
            
//            BroadcastViewer(JNetMessage.MakeSimple("noti_camera_matrix", (writer) =>
//            {
//                writer.Write(user_index);
//                writer.Write(net_capture._camera_info.Length);
//                for (int i = 0; i < net_capture._camera_info.Length; ++i)
//                    writer.Write(net_capture._camera_info[i]);
//            }));
//        });
//        #endregion




//        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//        // 포인트 클라우드
//        //
//        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//        #region  [C->S] 포인트 클라우드
//        //------------------------------------------------------------------------------------------------------------------------------------------------------
//        RegisterSimpleMessageHandler("cs_all_point_clouds", (sender, reader) =>
//        {
//            var net_capture = ToNetClientCapture(sender);
//            //if ((null == net_capture) || (0 != GetNetClientCaptureIndex(net_capture))) // 현재 버전은 한명의 데이터만 수신하기
//            //    return;
//            if ((null == net_capture) || (null != VosMapManager.Instance._map_buffer)) // 현재 버전은 한명의 데이터만 수신하기
//                return;
                

//            //----------------------------------------------------------------------
//            // 1. 입력 패킷
//            //
//            //JLogger.WriteFormat("sc_point_clouds:");
//            var buf_size = reader.ReadInt32();
//            if ((null == VosMapManager.Instance._map_buffer) || (VosMapManager.Instance._map_buffer.Length != buf_size))
//                VosMapManager.Instance._map_buffer = new byte[buf_size];
//            reader.Read(VosMapManager.Instance._map_buffer, 0, buf_size);

//            //----------------------------------------------------------------------
//            // 2. 응답 패킷
//            //
//            BroadcastViewer(JNetMessage.MakeSimple("noti_all_point_clouds", VosMapManager.Instance._map_buffer, buf_size));
//        });
//        #endregion



//        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//        // 키넥트
//        //
//        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//        #region  [C->S] 카메라 좌표
//        //------------------------------------------------------------------------------------------------------------------------------------------------------
//        RegisterSimpleMessageHandler("cs_kinect_skeleton", (sender, reader) =>
//        {
//            var net_kinect = ToNetClientKinect(sender);
//            if (null == net_kinect)
//                return;
            
//            var recv_buf = sender._last_recv_buffer;
//            var send_buf = new byte[recv_buf._size];
//            Buffer.BlockCopy(recv_buf._buffer, 0, send_buf, 0, recv_buf._size);

//            foreach(var net_client in CollectNetClients())
//            {
//                net_client._net_peer.SendSimpleMessage("sc_kinect_skeleton", send_buf, send_buf.Length);
//            }
            

//        });
//        #endregion

//    }



//}



