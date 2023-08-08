using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using J2y.Network;
using System.Linq;
using System.Diagnostics;

namespace J2y.MultiDrone
{
    #region [NestedClass] AirsimSetting

    [Serializable]
    public class VehicleParam
    {
        public int NAV_RCL_ACT = 0;
        public int NAV_DLL_ACT = 0;
        public float LPE_LAT = 47.641468f;
        public float LPE_LON = -122.140165f;
        public int COM_OBL_ACT = 1;
    }
    [Serializable]
    public class Geopoint
    {
        public double Longitude = 47.641468;
        public double Latitude = -122.140165;
        public double Altitude = 122;

        public Geopoint(double longi, double lati, double alti)
        {
            Longitude = longi;
            Latitude = lati;
            Altitude = alti;
        }
    }

    [Serializable]
    public class SensorInfo
    {
        public int SensorType;
        public bool Enabled;

        public SensorInfo(int type, bool enabled)
        {
            SensorType = type;
            Enabled = enabled;
        }
    }

    [Serializable]
    public class AirsimSetting
    {
        public float SettingsVersion = 1.2f;
        public string SimMode = "Multirotor";
        public bool IsVRMode = false;

        public Dictionary<string,DroneVehicleSetting> Vehicles = new Dictionary<string, DroneVehicleSetting>();

        public Geopoint OriginGeopoint;
    }
    #endregion

    #region [NestedClass] DroneVehicleSetting
    [Serializable]
    public class DroneVehicleSetting
    {
        public string VehicleType;
    }
    #endregion

    #region [NestedClass] DronePX4Setting
    [Serializable]
    public class DronePX4Setting : DroneVehicleSetting
    {
        public string Model;
        public string ControlIP = "0.0.0.0";
        public int ControlPort = 0;
        public string QgcHostIp = "0.0.0.0";
        public int QgcPort = 0;
        public string LocalHostIp = "0.0.0.0";
        public bool UseSerial;
        public bool UseTcp;
        public string UdpIp = "0.0.0.0";
        public int UdpPort = 0;
        public int TcpPort = 0;
        public float X;
        public float Y;
        public float Z;
        public string SerialPort = "COM3";
        public bool LockStep;        
        public VehicleParam @params;
        //public Dictionary<string, SensorInfo> Sensors;

        //public DronePX4Setting(int idx, string qgcip)
        //{
            
        //}

        public void SettingSITL(int idx, string qgcip, bool lockstep)
        {
            string ipaddress = "127.0.0.1";
            var ipentry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in ipentry.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipaddress = ip.ToString();
                    break;
                }
            }

            Model = "Generic";
            ControlIP = ipaddress;
            ControlPort = 14580 + idx;
            LocalHostIp = "0.0.0.0";
            QgcHostIp = qgcip;
            QgcPort = 14550;
            UseSerial = false;
            UseTcp = true;
            UdpIp = ipaddress;
            UdpPort = 14560 + idx;
            TcpPort = 4560 + idx;
            LockStep = lockstep;
            @params = new VehicleParam();
        }

        public void SettingHITL(int idx, string qgcip, string serialport)
        {
            string ipaddress = "127.0.0.1";
            var ipentry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in ipentry.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipaddress = ip.ToString();
                    break;
                }
            }

            Model = "Generic";
            LocalHostIp = ipaddress;
            QgcHostIp = qgcip;
            QgcPort = 14550;
            UseSerial = true;
            UseTcp = false;
            SerialPort = serialport;
            @params = new VehicleParam();
        }

        public void SettingCPS(int idx, string cpsip)
        {
            string ipaddress = "127.0.0.1";
            var ipentry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in ipentry.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipaddress = ip.ToString();
                    break;
                }
            }

            Model = "Generic";
            LocalHostIp = ipaddress;
            ControlIP = cpsip;
            ControlPort = 14580;
            UseSerial = false;
            UseTcp = false;

            @params = new VehicleParam()
            {
                LPE_LAT = 36.383560f,
                LPE_LON = 127.366554f
            };
        }
    }

    
    #endregion

    #region [NestedClass] DroneSimpleFlightSetting
    [Serializable]
    public class DroneSimpleFlightSetting : DroneVehicleSetting
    {
        
    }
    #endregion

    #region [NestedClass] DroneCPSSetting
    [Serializable]
    public class DroneCPSSetting : DroneVehicleSetting
    {

    }
    #endregion

    #region [NestedClass] DroneCarSetting
    [Serializable]
    public class DroneCarSetting : DroneVehicleSetting
    {

    }
    #endregion



    #region [NestedClass] DroneClientSetting
    [Serializable]
    public class DroneClientSetting
    {
        public string StartMap = "ArenaBlocks";
        public string DedicatedServerIP = "127.0.0.1";
        public string VehicleType = "Car"; // Car, MultiRotor, ComputerVision
        public int ClientGuid;
        public int ScriptServerPort = 41451;
        public string VehicleVisibleName = "Car";

        // spawn area
        public eSpawnAreaType SpawnAreaType = eSpawnAreaType.Area;
        public int SpawnAreaIndex = -1;
        public float SpawnPointX;
        public float SpawnPointY;
        public float SpawnPointZ;
        public float SpawnSphereRadius = 100;

        public float SyncTransformInterval = 0.5f;

        public bool UsePhysx = true;
        public bool UseServer = true;

        public bool UseCPS = false;
        public string Local_IP = "";
        public int CPS_LocalPort = 0;
        public bool IsVRMode = false;
        
    }
    #endregion




    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MdlDroneClient
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public partial class MdlDroneClient : JObject
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // NestedClass
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [NestedClass] CaptureImage
        public class CaptureImage
        {
            public JSharedMemory _sharedmemory;
            public byte[] _result_buf;
            public DateTime _open_timer = DateTime.Now.AddSeconds(-3);
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [Property] Capture Images
        private bool _capturing;
        private long _req_capture_admin_guid;
        private CaptureImage[] _capture_images = new CaptureImage[(int)eCaptureImageType.Count];
        #endregion

        #region [Property] Base SharedMemory(Command)
        //private JSharedMemory _sharedmemory_command;
        #endregion

        public Process _process;

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] Base
        public MdNetData_Client NetData { get; set; } = new MdNetData_Client();
        public IntPtr WindowHandle { get; set; }
        
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메인 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] Awake
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Awake()
        {
            NetData._guid = _guid;
            NetData._state = eClientState.Execute;
            if(MdlDataCenter.DroneSimulClients.Count > 0)
                NetData._script_server_port = MdlDataCenter.DroneSimulClients.Max(c => c.NetData._script_server_port) + 1;
            NetData.ConnectTime = DateTime.Now;
            MdlDataCenter.AddDroneSimulClient(this);
            //_sharedmemory_command = JSharedMemory.Create(MakeCommandSharedMemoryName((int)NetData._guid), 1024, false);


        }
        #endregion
        
        #region [초기화] Initialize
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Initialize(int processId)
        {
            NetData._processId = processId;
            
            //NetData._state = ReadClientState();
            asyncRenderOff();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void asyncRenderOff()
        {
            //for(int i = 0; i < 200; ++i)
            //{
                //var succeed = WriteSharedMemoryCommand_Rendering(2, false); // Default Off
                //if(succeed)
                //    break;
                //await JScheduler.Wait(0.1f);

                //if (i == 199)
                //{
                //    JSharedMemory.Create(MakeCommandSharedMemoryName((int)NetData._guid), 1024, true);
                //    succeed = WriteSharedMemoryCommand_Rendering(2, false);
                //}
            //}
        }
        #endregion


        bool _first = true;
        DateTime timer = DateTime.Now;
        TimeSpan previousCpuTime;
        DateTime previousTime;
        #region [업데이트] 메인
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Update()
        {
            if (_capturing)
            {
                for (var i = 0; i < (int)eCaptureImageType.Count; ++i)
                    ReadCaptureImage((eCaptureImageType)i);
            }

            if (_process != null)
            {
                if (_first)
                {
                    previousCpuTime = _process.TotalProcessorTime;
                    previousTime = DateTime.Now;
                    _first = false;
                }

                if ((DateTime.Now - timer).Seconds > 10)
                {
                    timer = DateTime.Now;
                    MdlDataCenter.NetConnector.SendMessage(JNetMessage.Make((int)ePacketProtocol.LS_USAGE, NetData._guid, GetCpuUsage(), GetMemoryUsage()));
                }
            }
            //Console.WriteLine("CPU : " + GetCpuUsage().ToString() + ", RAM : " + GetMemoryUsage().ToString());
        }
        #endregion


        
        public float GetCpuUsage()
        {
            //TimeSpan currentCpuTime = _process.TotalProcessorTime;
            //DateTime currentTime = DateTime.Now;

            //// 경과 시간 계산
            //TimeSpan elapsedTime = currentTime - previousTime;
            //previousTime = currentTime;

            //// CPU 사용량 계산
            //TimeSpan cpuUsage = currentCpuTime - previousCpuTime;
            //double cpuUsagePercentage = cpuUsage.TotalMilliseconds / elapsedTime.TotalMilliseconds * 100.0;
            //previousCpuTime = currentCpuTime;

            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            return cpuCounter.NextValue();
        }

        public long GetMemoryUsage()
        {
            long memoryUsage = _process.WorkingSet64;
            return memoryUsage;
        }


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 이미지캡처
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [이미지캡처] Start
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void StartImageCapture(long req_capture_admin_guid)
        {
            _req_capture_admin_guid = req_capture_admin_guid;
            if (_capturing)
                return;
            _capturing = true;
            //WriteSharedMemoryCommand_Recording(true);
        }
        #endregion

        #region [이미지캡처] Stop
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void StopImageCapture()
        {
            if (!_capturing)
                return;
            _capturing = false;
            //WriteSharedMemoryCommand_Recording(false);
        }
        #endregion

        #region [이미지캡처] 찾기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public CaptureImage GetCaptureImage(eCaptureImageType type)
        {
            var log = false;
            var ci = _capture_images[(int)type];
            if (null == ci)
            {
                ci = _capture_images[(int)type] = new CaptureImage();
                log = true;
            }
            if ((null == ci._sharedmemory) && ((DateTime.Now - ci._open_timer).TotalSeconds > 3f))
            {
                ci._sharedmemory = JSharedMemory.Open(MakeSharedMemoryName((int)NetData._guid, type), 1920 * 1080 * 3, log); // 최대 해상도
                ci._open_timer = DateTime.Now;
            }
            return ci;
        }
        #endregion

        #region [이미지캡처] SharedMemory에서 읽기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public CaptureImage ReadCaptureImage(eCaptureImageType type)
        {
            var ci = GetCaptureImage(type);
            var sm = ci._sharedmemory;
            if (null == sm)
                return null;
            sm.ReadFromSharedMemory(0, 16);
            var state = (eSharedMemoryState)sm.ReadInt32(0);
            var width = sm.ReadInt32(4);
            var height = sm.ReadInt32(8);
            var size = sm.ReadInt32(12);

            if (state == eSharedMemoryState.WriteDone)
            {
                sm.WriteTo(0, (int)eSharedMemoryState.Reading);
                sm.ReadFromSharedMemory(16, size);
                sm.WriteTo(0, (int)eSharedMemoryState.ReadDone);

                if ((null == ci._result_buf) || (ci._result_buf.Length != size))
                    ci._result_buf = new byte[size];
                Buffer.BlockCopy(sm._buffer, 16, ci._result_buf, 0, size);

                //File.WriteAllBytes("test.png", ci._result_buf);

                // 결과 전송
                if (MdlDataCenter.NetConnector != null)
                {
                    MdlDataCenter.NetConnector.SendMessage(JNetMessage.MakeSimple("lc2s_client_capture_image", (writer) =>
                    {
                        writer.Write(_req_capture_admin_guid);
                        writer.Write(NetData._guid);
                        writer.Write((int)type);
                        writer.Write((int)state);
                        writer.Write(width);
                        writer.Write(height);
                        writer.Write(size);
                        writer.Write(ci._result_buf, 0, ci._result_buf.Length);
                    }).SetCapacity(1920 * 1080 * 3));
                }
            }

            return ci;
        }
        #endregion


        #region [SharedMemory] 클라이언트와 연결됐는지 체크
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool CheckConnectClient()
        {
            //_sharedmemory_command = JSharedMemory.Open(MakeCommandSharedMemoryName((int)NetData._guid), 1024, false);
            //var read = ReadClientState();

            //if (read != 0)
            //return true;

            //return false;

            return NetData._state == eClientState.Spawn;
        }
        #endregion

        #region [SharedMemory] 클라이언트 상태
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //public int ReadClientState()
        //{
        //    //return _sharedmemory_command.ReadInt32(16);
        //}

        //public bool IsSpawnError()
        //{
            //return ReadClientState() == 2 || ReadClientState() == 3;
        //}
        #endregion



        #region [렌더링] OnOff
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //public void SetRendering(int render_option)
        //{
        //    WriteSharedMemoryCommand_Rendering(render_option);
        //}
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // DroneClient SharedMemory
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [DroneClient] [SharedMemory] WriteCommand
        //------------------------------------------------------------------------------------------------------------------------------------------------------	
        //public bool WriteSharedMemoryCommand(int index, int cmd, bool log = true)
        //{
            //if (null == _sharedmemory_command)
            //    _sharedmemory_command = JSharedMemory.Open(MakeCommandSharedMemoryName((int)NetData._guid), 1024, log);

            //if (_sharedmemory_command != null)
            //    _sharedmemory_command.WriteTo(index, cmd);

            //return (_sharedmemory_command != null);
        //}
        #endregion

        #region [DroneClient] [SharedMemory] Rendering/Rendering/Exit
        //------------------------------------------------------------------------------------------------------------------------------------------------------	
        //public bool WriteSharedMemoryCommand_Recording(bool start_recording)
        //{
            //return WriteSharedMemoryCommand(0, start_recording ? 1 : 2); // 1: Start, 2: Stop
        //}
        //------------------------------------------------------------------------------------------------------------------------------------------------------	
        //public bool WriteSharedMemoryCommand_Rendering(int render_option, bool log = true)
        //{
        //    return WriteSharedMemoryCommand(4, render_option, log);
        //}
        ////------------------------------------------------------------------------------------------------------------------------------------------------------	
        //public bool WriteSharedMemoryCommand_Exit(int option)
        //{
        //    return WriteSharedMemoryCommand(8, option);
        //}
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 유틸
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [유틸] Sync RealCamera
        //------------------------------------------------------------------------------------------------------------------------------------------------------	
        public static string MakeSharedMemoryName(int client_id, eCaptureImageType type)
        {
            return string.Format("SM_{0}_{1}", client_id, (int)type);
            //return string.Format("SM_1_{0}", (int)type);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------	
        public static string MakeCommandSharedMemoryName(int client_id)
        {
            return string.Format("SM_COMMAND_{0}", client_id);
            //return "SM_COMMAND_1";
        }
        #endregion


    }

}
