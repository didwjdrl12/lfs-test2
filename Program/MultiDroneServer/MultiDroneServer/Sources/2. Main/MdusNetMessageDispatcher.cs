using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using J2y.Network;
using System.Linq;
using Newtonsoft.Json;
using System.Text;

namespace J2y.MultiDroneUnity
{
    public class NetPeer_base_Comp : IEqualityComparer<NetPeer_base>
    {
        bool IEqualityComparer<NetPeer_base>.Equals(NetPeer_base x, NetPeer_base y)
        {
            return x._ip_address.Split(':')[0] == y._ip_address.Split(':')[0];
        }
        int IEqualityComparer<NetPeer_base>.GetHashCode(NetPeer_base obj)
        {
            if (obj == null || !obj._ip_address.Contains(':'))
                return 0;
            int hashName = obj._ip_address.Split(':')[0].GetHashCode();
            return hashName;
        }
    }
    enum EAltitudeMode : byte
    {
        relativeToGround,
        clampToGround,
        absolute,
        clampToSeaFloor,
        relativeToSeaFloor
    };
    struct FPlacemark
    {
        public double LookLongitude;  // 2
        public double LookLatitude;   // 2
        public double LookAltitude;   //2
        public float LookRange;       //1
        public float LookTilt;        //1
        public float LookHeading;     //1
        public EAltitudeMode AltitudeMode;

        public double Logitude;  //2
        public double Latitude;  //2
        public double Altitude;  //2

        public float Heading;    //1
        public float Tilt;       //1
        public float Roll;       //1

        public Vector3 Scale;    //3
        public string FbxName;

        public int ToBytes(ref byte[] arr, int StartArrIndex)
        {
            int dstOffset = StartArrIndex;
            Buffer.BlockCopy(BitConverter.GetBytes(LookLongitude), 0, arr, dstOffset, sizeof(double)); dstOffset += sizeof(double);
            Buffer.BlockCopy(BitConverter.GetBytes(LookLatitude), 0, arr, dstOffset, sizeof(double)); dstOffset += sizeof(double);
            Buffer.BlockCopy(BitConverter.GetBytes(LookAltitude), 0, arr, dstOffset, sizeof(double)); dstOffset += sizeof(double);
            Buffer.BlockCopy(BitConverter.GetBytes(LookRange), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);
            Buffer.BlockCopy(BitConverter.GetBytes(LookTilt), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);
            Buffer.BlockCopy(BitConverter.GetBytes(LookHeading), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);
            Buffer.BlockCopy(BitConverter.GetBytes((byte)AltitudeMode), 0, arr, dstOffset, sizeof(byte)); dstOffset += sizeof(byte);

            Buffer.BlockCopy(BitConverter.GetBytes(Logitude), 0, arr, dstOffset, sizeof(double)); dstOffset += sizeof(double);
            Buffer.BlockCopy(BitConverter.GetBytes(Latitude), 0, arr, dstOffset, sizeof(double)); dstOffset += sizeof(double);
            Buffer.BlockCopy(BitConverter.GetBytes(Altitude), 0, arr, dstOffset, sizeof(double)); dstOffset += sizeof(double);

            Buffer.BlockCopy(BitConverter.GetBytes(Heading), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);
            Buffer.BlockCopy(BitConverter.GetBytes(Tilt), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);
            Buffer.BlockCopy(BitConverter.GetBytes(Roll), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);

            if (Scale == null)
                Scale = Vector3.one;
            Buffer.BlockCopy(BitConverter.GetBytes(Scale.x), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);
            Buffer.BlockCopy(BitConverter.GetBytes(Scale.y), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);
            Buffer.BlockCopy(BitConverter.GetBytes(Scale.z), 0, arr, dstOffset, sizeof(float)); dstOffset += sizeof(float);

            if (FbxName == null)
                FbxName = "NM";
            byte[] strBytes = Encoding.UTF8.GetBytes(FbxName);
            Buffer.BlockCopy(strBytes, 0, arr, dstOffset, sizeof(byte) * strBytes.Length); dstOffset += sizeof(byte) * strBytes.Length;

            return dstOffset - StartArrIndex;
        }
        public bool ToStruct(byte[] Src, int StartSrcOffset, int EndSrcOffset)
        {
            int srcOffset = StartSrcOffset;

            LookLongitude = BitConverter.ToDouble(Src, srcOffset); srcOffset += sizeof(double);
            if (srcOffset > EndSrcOffset)
                return false;
            LookLatitude = BitConverter.ToDouble(Src, srcOffset); srcOffset += sizeof(double);
            if (srcOffset > EndSrcOffset)
                return false;
            LookAltitude = BitConverter.ToDouble(Src, srcOffset); srcOffset += sizeof(double);
            if (srcOffset > EndSrcOffset)
                return false;

            LookRange = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
            if (srcOffset > EndSrcOffset)
                return false;
            LookTilt = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
            if (srcOffset > EndSrcOffset)
                return false;
            LookHeading = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
            if (srcOffset > EndSrcOffset)
                return false;
            AltitudeMode = (EAltitudeMode)Src[srcOffset]; srcOffset += sizeof(byte);
            if (srcOffset > EndSrcOffset)
                return false;


            Logitude = BitConverter.ToDouble(Src, srcOffset); srcOffset += sizeof(double);
            if (srcOffset > EndSrcOffset)
                return false;
            Latitude = BitConverter.ToDouble(Src, srcOffset); srcOffset += sizeof(double);
            if (srcOffset > EndSrcOffset)
                return false;
            Altitude = BitConverter.ToDouble(Src, srcOffset); srcOffset += sizeof(double);
            if (srcOffset > EndSrcOffset)
                return false;

            Heading = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
            if (srcOffset > EndSrcOffset)
                return false;
            Tilt = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
            if (srcOffset > EndSrcOffset)
                return false;
            Roll = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
            if (srcOffset > EndSrcOffset)
                return false;

            Scale = Vector3.one;
            Scale.x = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
            if (srcOffset > EndSrcOffset)
                return false;
            Scale.y = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
            if (srcOffset > EndSrcOffset)
                return false;
            Scale.z = BitConverter.ToSingle(Src, srcOffset); srcOffset += sizeof(float);
            if (srcOffset > EndSrcOffset)
                return false;

            int strlen = EndSrcOffset - srcOffset;
            if (strlen <= 3)
                return false;
            byte[] nameArr = new byte[strlen];
            Buffer.BlockCopy(Src, srcOffset, nameArr, 0, strlen);

            FbxName = Encoding.UTF8.GetString(nameArr);

            return true;
        }
    };

    public enum ePacketProtocol
    {
        CLIENT_TYPE = 8,
        CLIENT_STATE = 9,

        #region [Client]
        // 입장, 퇴장
        CS_ENTER_ROOM = 10,
        SC_ENTER_ROOM = 11,
        NOTI_USER_ENTER = 12,
        NOTI_USER_LEAVE = 13,
        CS_SPAWN_USER = 14,
        NOTI_USER_SPAWN = 15,

        // 이동
        CS_USER_MOVE = 20,
        NOTI_USER_MOVE = 21,

        // 기타
        SC_RENDERING = 22,
        SC_LIDAR = 23,
        SC_NAME = 24,
        SC_START_MONITOR = 25,
        SC_STOP_MONITOR = 26,
        CS_SMOOTHNESS = 27,

        #endregion

        #region [Launcher]

        SL_CLIENT_ADD = 40,
        LS_COMPUTER_INFO = 41,
        SL_CLIENT_KILL = 42,
        SL_CLIENT_KILL_ALL = 43,

        SL_CLIENT_SETTING_LOAD = 44,
        SL_CLIENT_SETTING_SAVE = 45,

        LS_CLIENT_SETTING_LOAD = 46,
        LS_CLIENT_SETTING_SAVE = 47,

        SL_CLIENT_PYTHON_SEND = 48,
        LS_CLIENT_PYTHON_SEND = 49,

        SL_REQ_CLIENT_CAPTURE_IMAGE = 50,
        LS_CLIENT_CAPTURE_IMAGE = 51,

        SL_CLIENT_RENDERING = 52,

        SL_PX4_KILL = 53,

        LS_PORTS = 54,

        SL_REQ_PORTS = 55,

        SL_PX4_CMD = 56,
        LS_PX4_OUTPUT = 57,

        SL_ORGANIZE_WINDOW = 58,
        LS_ORGANIZE_WINDOW = 59,

        SL_KML = 60,

        LS_USAGE = 61,

        #endregion

        #region [AdminTool]

        AS_CLIENT_ADD = 80,
        AS_CLIENT_KILL = 81,
        AS_CLIENT_KILL_ALL = 82,
        AS_CLIENT_LIST = 83,
        SA_CLIENT_LIST = 84,

        AS_CLIENT_SETTING_LOAD = 85,
        SA_CLIENT_SETTING_LOAD = 86,
        AS_CLIENT_SETTING_SAVE = 87,
        SA_CLIENT_SETTING_SAVE = 88,

        AS_CLIENT_RENDERING = 89,
        AS_CLIENT_RENDERING_INFO = 90,
        SA_REQ_CLIENT_IS_RENDERING = 91,

        AS_REQ_CLIENT_CAPTURE_IMAGE = 92,
        SA_REQ_CLIENT_RESULT_IMAGE = 93,

        AS_CLIENT_PYTHON_SEND = 94,
        SA_CLIENT_PYTHON_SEND = 95,

        AS_PX4_KILL = 96,

        SA_HITL_PORTS = 97,
        AS_REQ_PORTS = 98,

        SA_SPAWN_ERROR = 99,

        AS_MONITOR_START_POS = 100,
        AS_MONITOR_STOP_POS = 101,
        SA_POS = 102,

        AS_MONITOR_START_SMS = 103,
        AS_MONITOR_STOP_SMS = 104,
        SA_SMS = 105,

        AS_PX4_CMD = 106,
        SA_PX4_OUTPUT = 107,

        AS_ORGANIZE_WINDOW = 108,

        SA_USAGE = 109,
        #endregion

        #region [KML]
        AS_KML = 110,
        SC_KML_UPDATED = 111,
        CS_CUSTOM_STRING = 112,
        SC_KML_REMOVEALL = 113,
        #endregion
        SC_MODEL_DATA = 114,
        SC_MODEL_DATA_BEZIER = 115,

        OTHER_PROJECTION = 201,
    };



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MdusNetMessageDispatcher
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static class MdusNetMessageDispatcher
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] Temp
        private static long _temp_admin_guid;

        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 유저
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [유저] Broadcast
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void BroadcastUsers(MdusNetServer net_server, JNetMessage net_message, MdusNetClient_User except = null)
        {
            foreach (var user in MdusDataCenter.NetClient_Users)
            {
                if ((null == user) || (user == except) || null == user.NetPeer)
                    continue;
                user.NetSendMessage(net_message);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static void BroadcastAdmins(MdusNetServer net_server, JNetMessage net_message, MdusNetClient_AdminTool except = null)
        //{
        //    foreach (var admin in MdusDataCenter.NetClient_AdminTools)
        //    {
        //        if ((null == admin) || (admin == except) || null == admin.NetPeer)
        //            continue;
        //        admin.NetSendMessage(net_message);
        //    }
        //}
        #endregion



        #region [유저] Find
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MdusNetClient_User FindUser(MdusNetServer net_server, long guid)
        {

            var type = net_server.GetType();


            foreach (var client in net_server.m_connections)
            {
                var user = client.Tag as MdusNetClient_User;
                if (null == user)
                    continue;
                if (user._guid == guid)
                    return user;
            }
            return null;
        }
        #endregion

        #region [네트워크] Broadcast
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void BroadcastAdminTools(MdusNetServer net_server, JNetMessage net_message)
        {
            foreach (var client in net_server.m_connections)
            {
                var admin = client.Tag as MdusNetClient_AdminTool;
                if (null == admin)
                    continue;
                admin.NetSendMessage(net_message);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void BroadcastLaunchers(MdusNetServer net_server, JNetMessage net_message)
        {
            foreach (var client in net_server.m_connections)
            {
                var client_launcher = client.Tag as MdusNetClient_Launcher;
                if (null == client_launcher)
                    continue;
                client_launcher.NetSendMessage(net_message);
            }
        }
        #endregion

        #region [네트워크] [AdminTool] Find
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MdusNetClient_AdminTool FindAdminTool(MdusNetServer net_server, long guid)
        {
            foreach (var client in net_server.m_connections)
            {
                var admin = client.Tag as MdusNetClient_AdminTool;
                if (null == admin)
                    continue;
                if (admin._guid == guid)
                    return admin;
            }
            return null;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MdusNetClient_Launcher FindLaunchers(MdusNetServer net_server, long guid)
        {
            foreach (var client in net_server.m_connections)
            {
                var client_launcher = client.Tag as MdusNetClient_Launcher;
                if (null == client_launcher)
                    continue;
                if (client_launcher.NetData._guid == guid)
                    return client_launcher;
            }
            return null;
        }
        #endregion

        #region [메시지] [만들기] 컴퓨터(+클라이언트) 목록
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JNetMessage MakeMessage_ComputerList()
        {
            return JNetMessage.Make((int)ePacketProtocol.SA_CLIENT_LIST, (writer) =>
            {
                writer.Write(MdusDataCenter.NetClient_Launchers.Count);
                MdusDataCenter.NetClient_Launchers.ForEach(c =>
                {
                    c.NetData.Write(writer);
                });
            }).SetCapacity(1024 * 128);
        }
        #endregion

        //static long[] timer = { JTimer.GetCurrentTick(), JTimer.GetCurrentTick() };
        public static void RegisterMessageHandlers(MdusNetServer net_server)
        {
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 인증
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [인증] [C->S] 
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.CLIENT_TYPE, (sender, reader) =>
            {
                var client_type = (eClientType)reader.ReadInt32();
                switch (client_type)
                {
                    case eClientType.Client:
                        {
                            //var net_client = JObjectManager.Create<MdusNetClient_User>();
                            //sender.Tag = net_client;
                            //net_client.NetPeer = sender;
                        }
                        break;
                    case eClientType.Launcher:
                        {
                            var net_client = JObjectManager.Create<MdusNetClient_Launcher>();

                            //reader.ReadInt64();
                            //net_client.NetData._ip_address = reader.ReadString();
                            //net_client.NetData._cpu_info = reader.ReadString();
                            //net_client.NetData._memory_info = reader.ReadString();
                            net_client.NetData.Read(reader);
                            sender.Tag = net_client;
                            net_client.NetPeer = sender;
                        }
                        break;
                    case eClientType.AdminTool:
                        {
                            var net_client = JObjectManager.Create<MdusNetClient_AdminTool>();
                            sender.Tag = net_client;
                            net_client.NetPeer = sender;

                            sender.SendMessage((int)ePacketProtocol.CLIENT_TYPE, "AdminTool", MdusDataCenter.s_map);
                        }
                        break;
                }
                JLogger.WriteFormat("cs_client_type:" + client_type);


                //----------------------------------------------------------------------
                // 2. 응답 패킷
                //

                //sender.SendSimpleMessage("sc_client_type", "ok");
            });
            #endregion

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 유저
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


            #region [유저] [C->S] 방 접속
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.CS_ENTER_ROOM, (sender, reader) =>
            {
                var id = reader.ReadInt64();
                MdusNetClient_User user = null;

                if (MdusBase._editor_mode)
                {
                    user = JObject.CreateObject<MdusNetClient_User>();
                    user.NetData = new MdNetData_User();
                    user.NetData._vehicle_type = (eVehicleType)reader.ReadInt32();
                    user.NetData._vehicle_model = 0;
                    MdusDataCenter.NetClient_Users[id - 1] = user;

                }
                else
                {
                    reader.ReadInt32();
                    user = MdusDataCenter.FindUser(id);
                }

                sender.Tag = user;
                user.NetPeer = sender;
                user.NetData._guid = id;

                JLogger.Write("Recv Enter Message " + user.NetData._guid);

                //----------------------------------------------------------------------
                // 1. 기존 유저 목록 전송
                // 
                sender.SendMessage((int)ePacketProtocol.SC_ENTER_ROOM, (writer) =>
                {
                    MdNetData_EnterRoom data = new MdNetData_EnterRoom();
                    data._users = MdusDataCenter.NetClient_Users.Where(w => w != null && w.NetData._guid != id && w._is_spawned == true).ToList();
                    data._size = data._users.Count;

                    data.Write(writer);

                    foreach (var us in data._users)
                        JLogger.Write("Send User List " + us._guid);
                });
            });
            #endregion

            #region [유저] [C->S] 스폰
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.CS_SPAWN_USER, (sender, reader) =>
            {
                var user = sender.Tag as MdusNetClient_User;
                MdNetData_User data = new MdNetData_User();

                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //            
                data._guid = reader.ReadInt64();
                data._vehicle_type = (eVehicleType)reader.ReadInt32();
                data._vehicle_model = reader.ReadInt32();
                data.Read(reader);
                #region [Editor mode]
                if (MdusBase._editor_mode)
                {
                    var fuser = MdusDataCenter.FindUser(data._guid);
                    fuser.NetData._visible_name = data._visible_name;
                }
                #endregion
                //----------------------------------------------------------------------
                // 2. Broadcast(스폰)
                // 
                BroadcastUsers(net_server, JNetMessage.Make((int)ePacketProtocol.NOTI_USER_SPAWN, data), user);
                user._is_spawned = true;
                JLogger.Write("Broadcast Spawn Message");
                //----------------------------------------------------------------------
                // 3. 클라이언트 상태 전달
                // 
                if (user.Launcher != null)
                {
                    user.Launcher.NetData._clients.ForEach((c) =>
                    {
                        if (c._guid == data._guid)
                            c._state = eClientState.Spawn;
                    });
                    user.Launcher.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.CLIENT_STATE, data._guid));
                }
            });
            #endregion

            #region [유저] [C->S] custom string data
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.CS_CUSTOM_STRING, (sender, reader) =>
            {
                var user = sender.Tag as MdusNetClient_User;
                MdNetData_CustomString data = new MdNetData_CustomString();
                data.Read(reader);
                string str = data.ToString();
                int i = str.IndexOf('@');
                str = str.Substring(i + 1);
                Console.WriteLine("ePacketProtocol.CS_CUSTOM_STRING = " + str);
                if (str.Contains("test"))
                {
                    BroadcastUsers(net_server, JNetMessage.Make((int)ePacketProtocol.SC_KML_UPDATED, 1004));
                }
            });
            #endregion

            #region [유저] [C->S] 위치 이동
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.CS_USER_MOVE, (sender, reader) =>
            {
                var user = sender.Tag as MdusNetClient_User;
                MdNetData_Move data = new MdNetData_Move();

                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //            
                data.Read(reader);
                user.NetData._position = data._position;
                user.NetData._rotation = data._rotation;

                //----------------------------------------------------------------------
                // 3. Broadcast(접속 알림)
                // 
                BroadcastUsers(net_server, JNetMessage.Make((int)ePacketProtocol.NOTI_USER_MOVE, (writer) =>
                {
                    data.Write(writer);
                }), user);

                //var time = JTimer.GetElaspedTime(timer[data._guid-1]);

                //if (time > 1f)
                //{
                //    Console.WriteLine(string.Format("ID : {0}", data._guid));
                //    Console.WriteLine(string.Format("Position : {0}, {1}, {2}", data._position[0], data._position[1], data._position[2]));
                //    Console.WriteLine(string.Format("Rotation : {0}, {1}, {2}, {3}", data._rotation[0], data._rotation[1], data._rotation[2], data._rotation[3]));
                //    Console.WriteLine(string.Format("Velocity : {0}, {1}, {2}", data._velocity[0], data._velocity[1], data._velocity[2]));
                //    Console.WriteLine(string.Format("RotorCount : {0}", data._rotor_count));
                //    Console.WriteLine(string.Format("RotorRotation : {0}, {1}, {2}, {3}", data._rotor_rotation[0], data._rotor_rotation[1], data._rotor_rotation[2], data._rotor_rotation[3]));
                //    Console.WriteLine();
                //    timer[data._guid-1] = JTimer.GetCurrentTick();
                //}

                //----------------------------------------------------------------------
                // 4. Monitoring(AdminTool에 위치 전달)
                // 
                foreach (var tool in MdusDataCenter.NetClient_AdminTools)
                {
                    if (tool.ActiveMonitor)
                        tool.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.NOTI_USER_MOVE, data._guid, data._position[0], data._position[1], data._position[2]));
                }
            });
            #endregion

            #region #region [유저] [S->C] Smoothness
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.CS_SMOOTHNESS, (sender, reader) =>
            {
                var user = sender.Tag as MdusNetClient_User;
                var smoothness = reader.ReadSingle();
                BroadcastAdminTools(net_server, JNetMessage.Make((int)ePacketProtocol.SA_SMS, user.NetData._guid, smoothness));

            });
            #endregion

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 런처
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [런처] [LC->S] 목록 보내기(갱신)
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.LS_COMPUTER_INFO, (sender, reader) =>
            {


                var client_launcher = sender.Tag as MdusNetClient_Launcher;

                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //            
                var net_com = new MdNetData_Launcher();
                net_com.Read(reader);

                net_com._guid = client_launcher.NetData._guid;
                client_launcher.NetData = net_com;

                //----------------------------------------------------------------------
                // 2. 목록 갱신 알림
                // 
                BroadcastAdminTools(net_server, MakeMessage_ComputerList());
            });
            #endregion

            #region [런처] [LC->S] 포트 목록 보내기(갱신)
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.LS_PORTS, (sender, reader) =>
            {
                var client_launcher = sender.Tag as MdusNetClient_Launcher;

                var guid = (sender.Tag as MdusNetClient_Launcher).NetData._guid;
                var json = reader.ReadString();
                client_launcher.NetData._ports = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);

                BroadcastAdminTools(net_server, JNetMessage.Make((int)ePacketProtocol.SA_HITL_PORTS, guid, json));
            });
            #endregion


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 운영툴
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [운영툴] [AC->S] 클라이언트 목록 요청
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_CLIENT_LIST, (sender, reader) =>
            {
                var client_admin = sender.Tag as MdusNetClient_AdminTool;

                //----------------------------------------------------------------------
                // 1. 리스트 전송
                //  
                sender.SendMessage(MakeMessage_ComputerList());
            });
            #endregion

            #region [운영툴] [AC->S] 클라이언트 추가
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_CLIENT_ADD, (sender, reader) =>
            {
                Console.WriteLine("[MSG] ac2s_client_add");

                var client_admin = sender.Tag as MdusNetClient_AdminTool;
                //----------------------------------------------------------------------
                // 1. 입력 패킷

                #region [패킷 파싱]
                Vector3 spawn_point;
                var com_guid = reader.ReadInt64();
                var count = reader.ReadInt32();
                var map_name = reader.ReadInt32();
                var vehicle_type = reader.ReadString();
                var vehicle_model = reader.ReadInt32();
                var vehicle_name = reader.ReadString();
                var spawn_area = reader.ReadInt32();
                var spawn_area_index = reader.ReadInt32();
                spawn_point.x = reader.ReadSingle();
                spawn_point.y = reader.ReadSingle();
                spawn_point.z = reader.ReadSingle();
                var spawn_sphere_radius = reader.ReadSingle();

                var qgcip = reader.ReadString();
                bool isVRMode = reader.ReadBoolean();
                var gio_longi = reader.ReadString();
                var gio_lati = reader.ReadString();
                var gio_alti = reader.ReadString();
                var hitl_port = reader.ReadString();
                var hitl_idx = reader.ReadInt32();
                #endregion
                var sync_transform_interval = MdusBase._sync_interval;

                var launcher = MdusDataCenter.FindLauncher(com_guid);
                if (null == launcher)
                {
                    Console.WriteLine("[ERROR] Launcher Not Found:-" + com_guid);
                    return;
                }

                MdusDataCenter.s_map = map_name;

                int idx = 0;

                if (vehicle_type == "HITL")
                {
                    var user = MdusDataCenter.FindUser(hitl_idx);
                    if (user != null)
                    {
                        sender.SendMessage((int)ePacketProtocol.SA_SPAWN_ERROR, hitl_idx);
                        return;
                    }
                    var net_client = JObjectManager.Create<MdusNetClient_User>();
                    idx = hitl_idx;
                    net_client.NetData = new MdNetData_User();
                    net_client.NetData._vehicle_type = eVehicleType.Multirotor;
                    net_client.NetData._vehicle_model = vehicle_model;
                    net_client.NetData._guid = idx;
                    net_client.Launcher = launcher;
                    net_client.NetData._visible_name = vehicle_name;
                    var client = new MdNetData_Client();
                    client._guid = idx;
                    client._state = eClientState.Execute;
                    launcher.NetData._clients.Add(client);
                    MdusDataCenter.NetClient_Users[idx - 1] = net_client;
                }
                else if (vehicle_type == "CPS")
                {
                    var user = MdusDataCenter.FindUser(hitl_idx);
                    if (user != null)
                    {
                        sender.SendMessage((int)ePacketProtocol.SA_SPAWN_ERROR, hitl_idx);
                        return;
                    }
                    var net_client = JObjectManager.Create<MdusNetClient_User>();
                    idx = hitl_idx;
                    net_client.NetData = new MdNetData_User();
                    net_client.NetData._vehicle_type = eVehicleType.Multirotor;
                    net_client.NetData._guid = idx;
                    net_client.Launcher = launcher;
                    net_client.NetData._vehicle_model = vehicle_model;
                    net_client.NetData._visible_name = vehicle_name;
                    var client = new MdNetData_Client();
                    client._guid = idx;
                    client._state = eClientState.Execute;
                    launcher.NetData._clients.Add(client);
                    MdusDataCenter.NetClient_Users[idx - 1] = net_client;
                }
                else
                {
                    idx = MdusDataCenter.FindEmptyIndex();
                    var net_client = JObjectManager.Create<MdusNetClient_User>();
                    net_client.NetData = new MdNetData_User();
                    net_client.NetData._guid = idx + 1;
                    if (vehicle_type == "Car")
                        net_client.NetData._vehicle_type = eVehicleType.Car;
                    else if (vehicle_type == "SimpleFlight" || vehicle_type == "SITL" || vehicle_type == "HITL" || vehicle_type == "CPS")
                    {
                        net_client.NetData._vehicle_type = eVehicleType.Multirotor;
                        net_client.NetData._vehicle_model = vehicle_model;
                    }
                    else if (vehicle_type == "ComputerVision")
                        net_client.NetData._vehicle_type = eVehicleType.ComputerVision;
                    net_client.NetData._visible_name = vehicle_name;

                    net_client.Launcher = launcher;
                    var client = new MdNetData_Client();
                    client._guid = idx + 1;
                    client._state = eClientState.Execute;
                    launcher.NetData._clients.Add(client);
                    MdusDataCenter.NetClient_Users[idx] = net_client;
                }

                //----------------------------------------------------------------------
                // 2. 런처에 요청

                launcher.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SL_CLIENT_ADD,
                                                    count, map_name,
                                                    vehicle_type,
                                                    vehicle_model,
                                                    vehicle_name,
                                                    spawn_area,
                                                    spawn_area_index,
                                                    spawn_point.x, spawn_point.y, spawn_point.z, spawn_sphere_radius,
                                                    sync_transform_interval,
                                                    idx + 1,
                                                    qgcip,
                                                    isVRMode,
                                                    gio_longi, gio_lati, gio_alti,
                                                    hitl_port, hitl_idx,
                                                    MdusBase._use_phyxs,
                                                    MdusBase._lockstepsitl));
            });
            #endregion


            #region [운영툴] [AC->S] AS_KML        
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_KML, (sender, reader) =>
            {
                Console.WriteLine("[KJS] ePacketProtocol.AS_KML");

                var client_admin = sender.Tag as MdusNetClient_AdminTool;

                // 1. 배열갯수.
                byte[] buffer = reader.ReadBytes(4);
                int numArr = BitConverter.ToInt32(buffer, 0);

                if(numArr == 666666)
                {
                    Console.WriteLine("[KJS] send Remove All Kml Models");
                    BroadcastUsers(net_server, JNetMessage.Make((int)ePacketProtocol.SC_KML_REMOVEALL, 1004));
                    return;
                }
                
                // 2. 헤더정보.
                int headSize = numArr * 2 * 4;
                buffer = reader.ReadBytes(headSize);

                FPlacemark[] arr = new FPlacemark[numArr];
                Tuple<int,int>[] head = new Tuple<int,int>[numArr];
                int offset = 0;
                int i = 0;
                int start = 0;
                int end = 0;
                for (; i < numArr; ++i)
                {
                    start = BitConverter.ToInt32(buffer, offset); 
                    offset += 4;
                    end   = BitConverter.ToInt32(buffer, offset); 
                    offset += 4;
                    
                    Debug.Assert(end > start);
                    head[i] = new Tuple<int, int>(start, end);                    
                }

                // 3. FPlacemark 정보.
                start = head[0].Item1;
                int placemarkSize = head[numArr - 1].Item2 - start;
                buffer = reader.ReadBytes(placemarkSize);
                FPlacemark[] temp = new FPlacemark[numArr];
                i = 0;
                for (;i <numArr; ++i)
                {
                    Debug.Assert(head[i].Item1 >= start);

                    temp[i] = new FPlacemark();                    
                    temp[i].ToStruct(buffer, head[i].Item1-start, head[i].Item2-start);
                }

                reader.BaseStream.Position = 0;
                int dataSize = 4 + headSize + placemarkSize;
                buffer = reader.ReadBytes(dataSize);

                //BroadcastLaunchers(net_server, JNetMessage.Make((int)ePacketProtocol.SL_KML, buffer, dataSize));
                var net_message = JNetMessage.Make((int)ePacketProtocol.SL_KML, buffer, dataSize);
                var uniqueList = net_server.m_connections.Distinct(new NetPeer_base_Comp()).ToList();
                Console.WriteLine("[KJS] Unique By IPAddress, Num Launcher=" + uniqueList.Count);
                foreach (var client in uniqueList)
                {
                    var client_launcher = client.Tag as MdusNetClient_Launcher;
                    if (null == client_launcher)
                        continue;
                    client_launcher.NetSendMessage(net_message);
                }

                BroadcastUsers(net_server, JNetMessage.Make((int)ePacketProtocol.SC_KML_UPDATED, 1004));
            });
            #endregion

            #region [운영툴] [AC->S] 클라이언트 종료
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_CLIENT_KILL, (sender, reader) =>
            {
                Console.WriteLine("[MSG] ac2s_client_kill");

                var client_admin = sender.Tag as MdusNetClient_AdminTool;


                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var com_guid = reader.ReadInt64();
                var client_guid = reader.ReadInt64();
                var launcher = MdusDataCenter.FindLauncher(com_guid);
                if (null == launcher)
                {
                    Console.WriteLine("[ERROR] Launcher Not Found:-" + com_guid);
                    return;
                }

                //----------------------------------------------------------------------
                // 2. 런처에 요청
                //  
                launcher.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SL_CLIENT_KILL, client_guid));

                var info = MdusDataCenter._rendering_info.Where(w => w._computer_guid == com_guid && w._client_guid == client_guid).FirstOrDefault();
                if (info != null)
                    MdusDataCenter._rendering_info.Remove(info);

                var lidar_info = MdusDataCenter._lidar_info.Where(w => w._computer_guid == com_guid && w._client_guid == client_guid).FirstOrDefault();
                if (info != null)
                    MdusDataCenter._lidar_info.Remove(info);
            });
            #endregion

            #region [운영툴] [AC->S] 클라이언트 모두 종료
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_CLIENT_KILL_ALL, (sender, reader) =>
            {
                Console.WriteLine("[MSG] ac2s_client_kill_all");

                var client_admin = sender.Tag as MdusNetClient_AdminTool;


                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var com_guid = reader.ReadInt64();
                var launcher = MdusDataCenter.FindLauncher(com_guid);
                if (null == launcher)
                {
                    Console.WriteLine("[ERROR] Launcher Not Found:-" + com_guid);
                    return;
                }

                //----------------------------------------------------------------------
                // 2. 런처에 요청
                //  
                launcher.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SL_CLIENT_KILL_ALL));
            });
            #endregion

            #region [운영툴] [AC->S] PX4 종료
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_PX4_KILL, (sender, reader) =>
            {
                Console.WriteLine("[MSG] ac2s_client_kill_all");

                var client_admin = sender.Tag as MdusNetClient_AdminTool;


                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var com_guid = reader.ReadInt64();
                var launcher = MdusDataCenter.FindLauncher(com_guid);
                if (null == launcher)
                {
                    Console.WriteLine("[ERROR] Launcher Not Found:-" + com_guid);
                    return;
                }

                //----------------------------------------------------------------------
                // 2. 런처에 요청
                //  
                launcher.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SL_PX4_KILL));
            });
            #endregion

            #region [운영툴] [AC->S] 포트 목록 요청
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_REQ_PORTS, (sender, reader) =>
            {
                var client_admin = sender.Tag as MdusNetClient_AdminTool;

                //----------------------------------------------------------------------
                // 1. 리스트 전송
                //  
                var id = reader.ReadInt64();
                var launcher = MdusDataCenter.FindLauncher(id);
                if (launcher == null)
                    return;
                launcher.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SL_REQ_PORTS));
            });
            #endregion

            #region [운영툴] [AC->S] 명령어
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_PX4_CMD, (sender, reader) =>
            {
                var client_admin = sender.Tag as MdusNetClient_AdminTool;

                //----------------------------------------------------------------------
                // 1. 리스트 전송
                //  
                var com_id = reader.ReadInt64();
                var px_id = reader.ReadInt32();
                var port = reader.ReadString();
                var cmd = reader.ReadString();
                var launcher = MdusDataCenter.FindLauncher(com_id);
                if (launcher == null)
                    return;
                launcher.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SL_PX4_CMD, px_id, port, cmd));
            });
            #endregion

            #region [운영툴] [AC->S] 콘솔아웃풋 
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.LS_PX4_OUTPUT, (sender, reader) =>
            {
                //----------------------------------------------------------------------
                // 1. 리스트 전송
                //  
                var px_id = reader.ReadInt32();
                var output = reader.ReadString();

                BroadcastAdminTools(net_server, JNetMessage.Make((int)ePacketProtocol.SA_PX4_OUTPUT, px_id, output));
            });
            #endregion
            #region [운영툴] Position
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_MONITOR_START_POS, (sender, reader) =>
            {
                var client_admin = sender.Tag as MdusNetClient_AdminTool;
                client_admin.ActiveMonitor = true;
                
            });
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_MONITOR_STOP_POS, (sender, reader) =>
            {
                var client_admin = sender.Tag as MdusNetClient_AdminTool;
                client_admin.ActiveMonitor = false;
            });
            #endregion

            #region [운영툴] Smoothness
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_MONITOR_START_SMS, (sender, reader) =>
            {
                var timer = reader.ReadSingle();
                BroadcastUsers(net_server, JNetMessage.Make((int)ePacketProtocol.SC_START_MONITOR, timer));
            });
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_MONITOR_STOP_SMS, (sender, reader) =>
            {
                BroadcastUsers(net_server, JNetMessage.Make((int)ePacketProtocol.SC_STOP_MONITOR));
            });
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_ORGANIZE_WINDOW, (sender, reader) =>
            {
                BroadcastLaunchers(net_server, JNetMessage.Make((int)ePacketProtocol.SL_ORGANIZE_WINDOW));
            });
            #endregion

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // [운영툴->런처->운영툴] 세팅 정보
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [운영툴] [AC->S] [SettingFile] 읽기 요청
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_CLIENT_SETTING_LOAD, (sender, reader) =>
            {
                Console.WriteLine("[MSG] ac2s_client_setting_load");

                var client_admin = sender.Tag as MdusNetClient_AdminTool;


                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var com_guid = reader.ReadInt64();
                var client_guid = reader.ReadInt64();
                var launcher = MdusDataCenter.FindLauncher(com_guid);
                if (null == launcher)
                {
                    Console.WriteLine("[ERROR] Launcher Not Found:-" + com_guid);
                    return;
                }

                //----------------------------------------------------------------------
                // 2. 런처에 요청
                //  
                _temp_admin_guid = client_admin._guid;
                launcher.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SL_CLIENT_SETTING_LOAD, client_admin._guid, client_guid));
            });
            #endregion

            #region [운영툴] [AC->S] [SettingFile] 저장 요청
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_CLIENT_SETTING_SAVE, (sender, reader) =>
            {
                Console.WriteLine("[MSG] ac2s_client_setting_save");

                var client_admin = sender.Tag as MdusNetClient_AdminTool;


                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var com_guid = reader.ReadInt64();
                var client_guid = reader.ReadInt64();
                var launcher = MdusDataCenter.FindLauncher(com_guid);
                if (null == launcher)
                {
                    Console.WriteLine("[ERROR] Launcher Not Found:-" + com_guid);
                    return;
                }
                var data_length = reader.ReadInt32();
                var setting_data = new byte[data_length];
                reader.Read(setting_data, 0, data_length);

                //----------------------------------------------------------------------
                // 2. 런처에 요청
                //  
                _temp_admin_guid = client_admin._guid;
                launcher.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SL_CLIENT_SETTING_SAVE, (writer) =>
                {
                    writer.Write(client_admin._guid);
                    writer.Write(client_guid);
                    writer.Write(setting_data.Length);
                    writer.Write(setting_data, 0, setting_data.Length);
                }).SetCapacity(data_length * 2));
            });
            #endregion

            #region [런처] [LC->S->AC] [SettingFile] 읽기 응답
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.LS_CLIENT_SETTING_LOAD, (sender, reader) =>
            {
                Console.WriteLine("[MSG] lc2s_client_setting_load");

                var client_launcher = sender.Tag as MdusNetClient_Launcher;


                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var admin = FindAdminTool(net_server, _temp_admin_guid);
                if (null == admin)
                {
                    Console.WriteLine("[ERROR] AdminTool Not Found:-" + _temp_admin_guid);
                    return;
                }
                var data_length = reader.ReadInt32();
                var setting_data = new byte[data_length];
                reader.Read(setting_data, 0, data_length);


                //----------------------------------------------------------------------
                // 2. 운영툴에 전송
                //  
                admin.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SA_CLIENT_SETTING_LOAD, setting_data, data_length));
            });
            #endregion            

            #region [런처] [LC->S->AC] [SettingFile] 저장 응답
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.LS_CLIENT_SETTING_SAVE, (sender, reader) =>
            {
                Console.WriteLine("[MSG] lc2s_client_setting_save");

                var client_launcher = sender.Tag as MdusNetClient_Launcher;


                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var admin = FindAdminTool(net_server, _temp_admin_guid);
                if (null == admin)
                {
                    Console.WriteLine("[ERROR] AdminTool Not Found:-" + _temp_admin_guid);
                    return;
                }
                var result = reader.ReadString();


                //----------------------------------------------------------------------
                // 2. 운영툴에 전송
                //  
                admin.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SA_CLIENT_SETTING_SAVE, result));
            });
            #endregion



            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // [운영툴->런처->운영툴] 스크립트 데이터
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [운영툴] [AC->S] [스크립트] 저장 요청
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_CLIENT_PYTHON_SEND, (sender, reader) =>
            {
                Console.WriteLine("[MSG] ac2s_client_python_send");

                var client_admin = sender.Tag as MdusNetClient_AdminTool;


                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var com_guid = reader.ReadInt64();
                var client_guid = reader.ReadInt64();
                var launcher = MdusDataCenter.FindLauncher(com_guid);
                if (null == launcher)
                {
                    Console.WriteLine("[ERROR] Launcher Not Found:-" + com_guid);
                    return;
                }
                var file_path = reader.ReadString();
                var data_length = reader.ReadInt32();
                var setting_data = new byte[data_length];
                reader.Read(setting_data, 0, data_length);

                //----------------------------------------------------------------------
                // 2. 런처에 요청
                //  
                _temp_admin_guid = client_admin._guid;
                launcher.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SL_CLIENT_PYTHON_SEND, (writer) =>
                {
                    writer.Write(client_admin._guid);
                    writer.Write(client_guid);
                    writer.Write(file_path);
                    writer.Write(setting_data.Length);
                    writer.Write(setting_data, 0, setting_data.Length);
                }).SetCapacity(data_length * 2));
            });
            #endregion

            #region [런처] [LC->S->AC] [스크립트] 저장 응답
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.LS_CLIENT_PYTHON_SEND, (sender, reader) =>
            {
                Console.WriteLine("[MSG] lc2s_client_python_send");

                var client_launcher = sender.Tag as MdusNetClient_Launcher;


                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var admin = FindAdminTool(net_server, _temp_admin_guid);
                if (null == admin)
                {
                    Console.WriteLine("[ERROR] AdminTool Not Found:-" + _temp_admin_guid);
                    return;
                }
                var result = reader.ReadString();


                //----------------------------------------------------------------------
                // 2. 운영툴에 전송
                //  
                admin.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SA_CLIENT_PYTHON_SEND, result));
            });
            #endregion




            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // [운영툴->런처->운영툴] 영상 정보 캡처
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [운영툴] [AC->S] 영상 정보 캡처 시작/종료
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_REQ_CLIENT_CAPTURE_IMAGE, (sender, reader) =>
            {
                Console.WriteLine("[MSG] s2lc_req_client_capture_image");

                var client_admin = sender.Tag as MdusNetClient_AdminTool;


                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var com_guid = reader.ReadInt64();
                var client_guid = reader.ReadInt64();
                var capture = reader.ReadInt32();
                var launcher = MdusDataCenter.FindLauncher(com_guid);
                if (null == launcher)
                {
                    Console.WriteLine("[ERROR] Launcher Not Found:-" + com_guid);
                    return;
                }


                //----------------------------------------------------------------------
                // 2. 런처에 요청
                //  
                launcher.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SL_REQ_CLIENT_CAPTURE_IMAGE, (writer) =>
                {
                    writer.Write(client_admin._guid);
                    writer.Write(client_guid);
                    writer.Write(capture);
                }));
            });
            #endregion

            #region [런처] [LC->S] 영상 정보 보내기
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int) ePacketProtocol.LS_CLIENT_CAPTURE_IMAGE, (sender, reader) =>
            {
                var client_launcher = sender.Tag as MdusNetClient_Launcher;

                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //
                var admin_guid = reader.ReadInt64();
                var client_guid = reader.ReadInt64();
                var admin_tool = MdusDataCenter.FindAdminTool(admin_guid);
                if (null == admin_tool)
                {
                    // Console.WriteLine("[ERROR] AdminTool Not Found:-" + admin_guid);
                    return;
                }

                //----------------------------------------------------------------------
                // 2. 운영툴에 전송
                //  
                var image_type = reader.ReadInt32();
                var state = reader.ReadInt32();
                var width = reader.ReadInt32();
                var height = reader.ReadInt32();
                var size = reader.ReadInt32();
                if (size <= 0)
                    return;
                var capture_buf = admin_tool.GetCaptureBuffer((MdusNetClient_AdminTool.eCaptureImageType)image_type, size);
                reader.Read(capture_buf, 0, size);

                admin_tool.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SA_REQ_CLIENT_RESULT_IMAGE, (writer) =>
                {
                    writer.Write(image_type);
                    writer.Write(client_guid);
                    writer.Write(state);
                    writer.Write(width);
                    writer.Write(height);
                    writer.Write(size);
                    writer.Write(capture_buf, 0, size);
                }).SetCapacity(1920 * 1080 * 3));
            });
            #endregion

            #region [운영툴] [AC->S] 렌더링 On/Off
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_CLIENT_RENDERING, (sender, reader) =>
            {
                Console.WriteLine("[MSG] ac2s_client_rendering");

                var client_admin = sender.Tag as MdusNetClient_AdminTool;


                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var com_guid = reader.ReadInt64();
                var client_guid = reader.ReadInt64();
                var render_option = reader.ReadInt32();
                var launcher = MdusDataCenter.FindLauncher(com_guid);
                if (null == launcher)
                {
                    Console.WriteLine("[ERROR] Launcher Not Found:-" + com_guid);
                    return;
                }

                client_guid--;

                switch(render_option)
                {
                    case 1:
                        MdusDataCenter.NetClient_Users[client_guid].NetPeer.SendMessage((int)ePacketProtocol.SC_RENDERING, true);
                        break;

                    case 2:
                        MdusDataCenter.NetClient_Users[client_guid].NetPeer.SendMessage((int)ePacketProtocol.SC_RENDERING, false);
                        break;

                    case 3:
                        MdusDataCenter.NetClient_Users[client_guid].NetPeer.SendMessage((int)ePacketProtocol.SC_LIDAR, true);
                        break;

                    case 4:
                        MdusDataCenter.NetClient_Users[client_guid].NetPeer.SendMessage((int)ePacketProtocol.SC_LIDAR, false);
                        break;

                    case 5:
                        MdusDataCenter.NetClient_Users[client_guid].NetPeer.SendMessage((int)ePacketProtocol.SC_NAME, true);
                        break;

                    case 6:
                        MdusDataCenter.NetClient_Users[client_guid].NetPeer.SendMessage((int)ePacketProtocol.SC_NAME, false);
                        break;


                }

                //----------------------------------------------------------------------
                // 2. 런처에 요청
                //  
                //launcher.NetSendMessage(JNetMessage.Make((int)ePacketProtocol.SL_CLIENT_RENDERING, (writer) =>
                //{
                //    writer.Write(client_admin._guid);
                //    writer.Write(client_guid);
                //    writer.Write(render_option);
                //}));

                var info = MdusDataCenter._rendering_info.Where(w => w._computer_guid == com_guid && w._client_guid == client_guid).FirstOrDefault();
                var lidar_info = MdusDataCenter._lidar_info.Where(w => w._computer_guid == com_guid && w._client_guid == client_guid).FirstOrDefault();

                //switch (render_option)
                //{
                //    case 1:
                //        if (info == null)
                //            MdusDataCenter._rendering_info.Add(new MdusDataCenter.MdusClientInfo(com_guid, client_guid));
                //        break;
                //    case 2:
                //        if (info != null)
                //            MdusDataCenter._rendering_info.Remove(info);
                //        break;
                //    case 3:
                //        if (lidar_info == null)
                //            MdusDataCenter._lidar_info.Add(new MdusDataCenter.MdusClientInfo(com_guid, client_guid));
                //        break;
                //    case 4:
                //        if (lidar_info != null)
                //            MdusDataCenter._lidar_info.Remove(lidar_info);
                //        break;
                //}
            });

            #endregion

            #region [운영툴] [AC->S] 렌더링 정보 요청
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            net_server.RegisterMessageHandler((int)ePacketProtocol.AS_CLIENT_RENDERING_INFO, (sender, reader) =>
            {
                Console.WriteLine("[MSG] ac2s_client_rendering_info");

                var com_guid = reader.ReadInt64();
                var client_guid = reader.ReadInt64();

                sender.SendMessage(JNetMessage.Make((int)ePacketProtocol.SA_REQ_CLIENT_IS_RENDERING, (writer) =>
                {
                    writer.Write(com_guid);
                    writer.Write(client_guid);
                    var info = MdusDataCenter._rendering_info.Where(w => w._computer_guid == com_guid && w._client_guid == client_guid).FirstOrDefault();
                    writer.Write(info != null ? 1 : 2);

                    var lidar_info = MdusDataCenter._lidar_info.Where(w => w._computer_guid == com_guid && w._client_guid == client_guid).FirstOrDefault();
                    writer.Write(lidar_info != null ? 3 : 4);
                }));
            });
            #endregion

            net_server.RegisterMessageHandler((int)ePacketProtocol.SC_MODEL_DATA, (sender, reader) =>
            {
                byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);

                BroadcastUsers(net_server, JNetMessage.Make((int)ePacketProtocol.SC_MODEL_DATA, data, data.Length));

                Console.WriteLine("[KJS] SC_MODEL_DATA");
            });
            net_server.RegisterMessageHandler((int)ePacketProtocol.SC_MODEL_DATA_BEZIER, (sender, reader) =>
            {
                byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);

                //test.
                //int offset = 0;
                //int stringLen = 0;
                //string str;
                
                //int id = BitConverter.ToInt32(data, offset);                     offset += sizeof(int);

                //for (int i = 0; i < 4; ++i)
                //{
                //    // 경도.
                //    stringLen = BitConverter.ToInt32(data, offset);              offset += sizeof(int);
                //    str = Encoding.UTF8.GetString(data, offset, stringLen);       offset += stringLen;
                //    // 위도.
                //    stringLen = BitConverter.ToInt32(data, offset);              offset += sizeof(int);
                //    str = Encoding.UTF8.GetString(data, offset, stringLen);       offset += stringLen;
                //}

                BroadcastUsers(net_server, JNetMessage.Make((int)ePacketProtocol.SC_MODEL_DATA_BEZIER, data, data.Length));

                Console.WriteLine("[KJS] SC_MODEL_DATA_BEZIER");
            });

            net_server.RegisterMessageHandler((int)ePacketProtocol.OTHER_PROJECTION, (sender, reader) =>
            {
                var data = reader.ReadBytes((int)reader.BaseStream.Length);
                var json = Encoding.UTF8.GetString(data).Replace("\0", "");

                var info = JsonConvert.DeserializeObject<MdNetData_ProjectinoInfo>(json);
                Console.WriteLine(json);
            });

            net_server.RegisterMessageHandler((int)ePacketProtocol.LS_USAGE, (sender, reader) =>
            {
                var guid = reader.ReadInt64();
                var cpu_usage = reader.ReadSingle();
                var ram_usage = reader.ReadInt64();

                BroadcastAdminTools(net_server, JNetMessage.Make((int)ePacketProtocol.SA_USAGE, guid, cpu_usage, ram_usage));
            });
        }
    }
}
