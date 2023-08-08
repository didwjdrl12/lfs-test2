using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using J2y.Network;
using System.IO;
using System.Threading.Tasks;
using System.Text;

namespace J2y.MultiDrone
{
    public enum EAltitudeMode : byte
    {
        relativeToGround,
        clampToGround,
        absolute,
        clampToSeaFloor,
        relativeToSeaFloor
    };
    public struct FPlacemark
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
        public void RegWrite(Microsoft.Win32.RegistryKey Key)
        {
            Key.SetValue("LookLongitude", LookLongitude.ToString());
            Key.SetValue("LookLatitude", LookLatitude.ToString());
            Key.SetValue("LookAltitude", LookAltitude.ToString());
            Key.SetValue("LookRange", LookRange);
            Key.SetValue("LookTilt", LookTilt);        //1
            Key.SetValue("LookHeading", LookHeading);     //1
            Key.SetValue("AltitudeMode", AltitudeMode);

            Key.SetValue("Logitude", Logitude.ToString());  //2
            Key.SetValue("Latitude", Latitude.ToString());  //2
            Key.SetValue("Altitude", Altitude.ToString());  //2

            Key.SetValue("Heading", Heading);    //1
            Key.SetValue("Tilt", Tilt);       //1
            Key.SetValue("Roll",Roll);       //1

            Key.SetValue("Scale.x", Scale.x);
            Key.SetValue("Scale.y", Scale.y);
            Key.SetValue("Scale.z", Scale.z);
            Key.SetValue("FbxName", FbxName);
        }
        public void RegRead(Microsoft.Win32.RegistryKey Key)
        {
            string str;
            str             = (string)Key.GetValue("LookLongitude");
            LookLongitude    = double.Parse(str);
            str             = (string)Key.GetValue("LookLatitude");
            LookLatitude     = double.Parse(str);
            str             = (string)Key.GetValue("LookAltitude");
            LookAltitude     = double.Parse(str);
            LookRange        = (float)Key.GetValue("LookRange");
            LookTilt         = (float)Key.GetValue("LookTilt");
            LookHeading      = (float)Key.GetValue("LookHeading");
            AltitudeMode     = (EAltitudeMode)Key.GetValue("AltitudeMode");

            str             = (string)Key.GetValue("Logitude");  //2
            Logitude         = double.Parse(str);
            str             = (string)Key.GetValue("Latitude");  //2
            Latitude        = double.Parse(str);
            str             = (string)Key.GetValue("Altitude");  //2
            Altitude        = double.Parse(str);

            Heading         = (float)Key.GetValue("Heading");    //1
            Tilt            = (float)Key.GetValue("Tilt");       //1
            Roll            = (float)Key.GetValue("Roll");       //1

            Scale = Vector3.one;
            Scale.x         = (float)Key.GetValue("Scale.x");
            Scale.y         = (float)Key.GetValue("Scale.y");
            Scale.z         = (float)Key.GetValue("Scale.z");
            FbxName         = (string)Key.GetValue("FbxName");
        }
    };
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MdlNetMessageDispatcher
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static class MdlNetMessageDispatcher
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [변수] Base

        #endregion



        public static void RegisterMessageHandlers(NetTcpClient tcp_client)
        {
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // [인증] 
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [인증] 추가
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            tcp_client.RegisterMessageHandler((int)ePacketProtocol.CLIENT_TYPE, (sender, reader) =>
            {
                var res = reader.ReadString();
                Console.WriteLine("[MSG] sc_client_type:" + res);

                sender.SendMessage((int)ePacketProtocol.LS_COMPUTER_INFO, MdlDataCenter.NetComputer);
            });
            #endregion


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // [클라이언트] 
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [클라이언트] 상태
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            tcp_client.RegisterMessageHandler((int)ePacketProtocol.CLIENT_STATE, (sender, reader) =>
            {
                var id = reader.ReadInt64();
                var client = MdlDataCenter.FindDroneSimulClient(id);
                client.NetData._state = eClientState.Spawn;
            });
            #endregion


            #region [클라이언트] 추가
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            tcp_client.RegisterMessageHandler((int)ePacketProtocol.SL_CLIENT_ADD, async (sender, reader) =>
            {
                Console.WriteLine("[MSG] s2lc_client_add");

                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                try
                {
                    Vector3 spawn_point;
                    var count = reader.ReadInt32();
                    var map_name = (eMapName)reader.ReadInt32();
                    var vehicle_type = reader.ReadString();
                    var vehicle_model = reader.ReadInt32();
                    var vehicle_name = reader.ReadString();
                    var spawn_area = (eSpawnAreaType)reader.ReadInt32();
                    var spawn_area_index = reader.ReadInt32();
                    spawn_point.x = reader.ReadSingle();
                    spawn_point.y = reader.ReadSingle();
                    spawn_point.z = reader.ReadSingle();
                    var spawn_sphere_radius = reader.ReadSingle();
                    var sync_transform_interval = reader.ReadSingle();
                    var idx = reader.ReadInt32();
                    var qgcip = reader.ReadString();
                    bool isVRMode = reader.ReadBoolean();

                    var gio_longi = double.Parse(reader.ReadString());
                    var gio_lati = double.Parse(reader.ReadString());
                    var gio_alti = double.Parse(reader.ReadString());
                    var hitl_port = reader.ReadString();
                    var hitl_idx = reader.ReadInt32();

                    var use_physx = reader.ReadBoolean();
                    var lockstep_sitl = reader.ReadBoolean();

                    for (int i = 0; i < count; ++i)
                    {
                        await MultiDroneLauncher.Instance.ExecuteDroneClient(false, 
                                                                        map_name, 
                                                                        vehicle_type, 
                                                                        vehicle_model, 
                                                                        vehicle_name, 
                                                                        spawn_area, 
                                                                        spawn_area_index, 
                                                                        spawn_point, 
                                                                        spawn_sphere_radius, 
                                                                        sync_transform_interval, 
                                                                        idx, 
                                                                        qgcip,
                                                                        isVRMode,
                                                                        gio_longi, gio_lati, gio_alti, 
                                                                        hitl_port, 
                                                                        hitl_idx, 
                                                                        use_physx, 
                                                                        lockstep_sitl);

                        JLogger.Write("[Log] : " + (i + 1) + " of the " + count + " executed.");
                        sender.SendMessage(JNetMessage.Make((int)ePacketProtocol.LS_COMPUTER_INFO, MdlDataCenter.NetComputer).SetCapacity(1024 * 128));
                    }
                }
                catch (NullReferenceException nullException)
                {
                    Console.WriteLine("[Exception] : " + nullException.ToString());
                }
                catch(FormatException formatException)
                {
                    Console.WriteLine("[Exception] : " + formatException.ToString());
                }
                catch(Exception e)
                {
                    Console.WriteLine("[Exception] : " + e.ToString());
                }
                
            });
            #endregion

            #region [S->L] SL_KML
            tcp_client.RegisterMessageHandler((int)ePacketProtocol.SL_KML, (sender, reader) =>
            {
                Console.WriteLine("kjs : ePacketProtocol.SL_KML");

                byte[] buffer = reader.ReadBytes(4);
                int numArr = BitConverter.ToInt32(buffer, 0);                                

                // 2. 헤더정보.
                int headSize = numArr * 2 * 4;
                buffer = reader.ReadBytes(headSize);

                Tuple<int, int>[] head = new Tuple<int, int>[numArr];
                int offset = 0;
                int i = 0;
                int start = 0;
                int end = 0;
                for (; i < numArr; ++i)
                {
                    start = BitConverter.ToInt32(buffer, offset);
                    offset += 4;
                    end = BitConverter.ToInt32(buffer, offset);
                    offset += 4;

                    Debug.Assert(end > start);
                    head[i] = new Tuple<int, int>(start, end);
                }

                // 3. FPlacemark 정보.
                start = head[0].Item1;
                int placemarkSize = head[numArr - 1].Item2 - start;
                buffer = reader.ReadBytes(placemarkSize);
                FPlacemark[] placemarkArr = new FPlacemark[numArr];
                i = 0;
                for (; i < numArr; ++i)
                {
                    Debug.Assert(head[i].Item1 >= start); 

                    placemarkArr[i] = new FPlacemark();
                    placemarkArr[i].ToStruct(buffer, head[i].Item1 - start, head[i].Item2 - start);
                }

                // 이전 제거.
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("KML",true);
                if (key != null)
                {
                    Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("KML");
                }  
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("KML");
                key.SetValue("Num Placemark", numArr);

                for (i = 0; i < numArr; ++i)
                {
                    key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"KML\Placemark" + i);
                    Debug.Assert(key != null);
                    placemarkArr[i].RegWrite(key);
                }
            });
            #endregion

            #region [클라이언트] 종료
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            tcp_client.RegisterMessageHandler((int)ePacketProtocol.SL_CLIENT_KILL, (sender, reader) =>
            {
                Console.WriteLine("[MSG] s2lc_client_kill");
                var client_guid = reader.ReadInt64();
                var client = MdlDataCenter.FindDroneSimulClient(client_guid);
                if (null == client)
                    return;
                MultiDroneLauncher.KillClientProcess(client_guid);
                sender.SendMessage((int)ePacketProtocol.LS_COMPUTER_INFO, MdlDataCenter.NetComputer);
            });
            #endregion

            #region [클라이언트] 모두 종료
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            tcp_client.RegisterMessageHandler((int)ePacketProtocol.SL_CLIENT_KILL_ALL, (sender, reader) =>
            {
                Console.WriteLine("[MSG] s2lc_client_kill_all");

                MultiDroneLauncher.KillClientAllProcess();
                sender.SendMessage((int)ePacketProtocol.LS_COMPUTER_INFO, MdlDataCenter.NetComputer);
            });
            #endregion


            #region [클라이언트] [SettingFile] Load
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            tcp_client.RegisterMessageHandler((int)ePacketProtocol.SL_CLIENT_SETTING_LOAD, (sender, reader) =>
            {
                Console.WriteLine("[MSG] s2lc_client_setting_load");

                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var req_admin_guid = reader.ReadInt64();
                var client_guid = reader.ReadInt64();
                var client = MdlDataCenter.FindDroneSimulClient(client_guid);
                if (null == client)
                    return;

                // todo: 클라이언트 ID에 따른 세팅 파일 다르게
                var config_fullname = string.Format("{0}\\{1}", MdlBase._path_client, MdlBase._filename_setting);
                if (File.Exists(config_fullname))
                {
                    var setting_data = File.ReadAllBytes(config_fullname);
                    sender.SendMessage((int)ePacketProtocol.LS_CLIENT_SETTING_SAVE, setting_data, setting_data.Length);
                }
                else
                {
                    //string msg = "kjs : SL_CLIENT_SETTING_LOAD, 파일없음 : " + config_fullname;
                    //Console.WriteLine(msg);
                }
            });
            #endregion

            #region [클라이언트] [SettingFile] Save
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            tcp_client.RegisterMessageHandler((int)ePacketProtocol.SL_CLIENT_SETTING_SAVE, (sender, reader) =>
            {
                Console.WriteLine("[MSG] s2lc_client_setting_save");

                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var req_admin_guid = reader.ReadInt64();
                var client_guid = reader.ReadInt64();
                var data_length = reader.ReadInt32();
                var client = MdlDataCenter.FindDroneSimulClient(client_guid);
                if (null == client)
                    return;

                var setting_data = new byte[data_length];
                reader.Read(setting_data, 0, data_length);

                // todo: 클라이언트 ID에 따른 세팅 파일 다르게
                var config_fullname = string.Format("{0}\\{1}", MdlBase._path_client, MdlBase._filename_setting);                
                File.WriteAllBytes(config_fullname, setting_data);

                sender.SendMessage((int)ePacketProtocol.LS_CLIENT_SETTING_SAVE, "ok");
            });
            #endregion

            #region [클라이언트] [스크립트] 저장 및 실행
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            tcp_client.RegisterMessageHandler((int)ePacketProtocol.SL_CLIENT_PYTHON_SEND, async (sender, reader) =>
            {
                Console.WriteLine("[MSG] s2lc_client_python_send");

                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var req_admin_guid = reader.ReadInt64();
                var client_guid = reader.ReadInt64();
                var file_path = reader.ReadString();
                var data_length = reader.ReadInt32();
                var client = MdlDataCenter.FindDroneSimulClient(client_guid);
                if (null == client)
                    return;
                var script_data = new byte[data_length];
                reader.Read(script_data, 0, data_length);

                await client.RunScript(script_data, file_path);

                sender.SendMessage((int)ePacketProtocol.LS_CLIENT_PYTHON_SEND, "ok");
            });
            #endregion


            #region [클라이언트] [PX4] 모두 종료
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            tcp_client.RegisterMessageHandler((int)ePacketProtocol.SL_PX4_KILL, (sender, reader) =>
            {
                MultiDroneLauncher.KillAllPX4();
                Console.WriteLine("[MSG] KillAllPX4");

            });
            #endregion

            #region [클라이언트] [PX4] 콘솔
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            tcp_client.RegisterMessageHandler((int)ePacketProtocol.SL_PX4_CMD, (sender, reader) =>
            {
                var px_id = reader.ReadInt32();
                var port = reader.ReadString();
                var cmd = reader.ReadString();
                MultiDroneLauncher.ExecuteCommandToMavConsole(px_id, port, cmd);
                

            });
            #endregion

            #region [유틸] [창 정렬]
            tcp_client.RegisterMessageHandler((int)ePacketProtocol.SL_ORGANIZE_WINDOW, (sender, reader) =>
            {
                MultiDroneLauncher.Instance.OrganizeWindows();
            });
            #endregion

            #region [클라이언트] 포트정보
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            tcp_client.RegisterMessageHandler((int)ePacketProtocol.SL_REQ_PORTS, (sender, reader) =>
            {
                MultiDroneLauncher.Instance.SendPortInfo();
            });
            #endregion
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // [클라이언트] 영상
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            // TODO : Client에 직접 요청하는 것으로 변경 예정

            #region [클라이언트] [영상] 요청
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            tcp_client.RegisterSimpleMessageHandler("s2lc_req_client_capture_image", (sender, reader) =>
            {
                Console.WriteLine("[MSG] s2lc_req_client_capture_image");

                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var req_admin_guid = reader.ReadInt64();
                var client_guid = reader.ReadInt64();
                var capture = reader.ReadInt32();
                var client = MdlDataCenter.FindDroneSimulClient(client_guid);
                if (null == client)
                    return;

                if (capture != 0)
                    client.StartImageCapture(req_admin_guid);
                else
                    client.StopImageCapture();
            });
            #endregion


            #region [클라이언트] [렌더링] 옵션 변경
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            tcp_client.RegisterSimpleMessageHandler("s2lc_client_rendering", (sender, reader) =>
            {
                Console.WriteLine("[MSG] s2lc_client_rendering");

                //----------------------------------------------------------------------
                // 1. 입력 패킷
                //    
                var req_admin_guid = reader.ReadInt64();
                var client_guid = reader.ReadInt64();
                var render_option = reader.ReadInt32();
                var client = MdlDataCenter.FindDroneSimulClient(client_guid);
                if (null == client)
                    return;

                //client.SetRendering(render_option);
            });
            #endregion
        }

    }
}
