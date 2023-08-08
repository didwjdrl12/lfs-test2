using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using J2y;
using J2y.Network;
using System.Net;
using System.Diagnostics;
using Microsoft.Win32;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace J2y.MultiDrone
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MultiDroneLauncher
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    
    [Serializable]
    public class PortInfo
    {
        public string _name;
        public int _id;
    }

    public class MultiDroneLauncher : JObject
    {        
        public static MultiDroneLauncher Instance;

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [변수] Base
        public bool _running = true;
        private long _network_connecting_timer;
        private bool _first_connected = true;
        public static bool _executing = false;
        #endregion

        #region [변수] Process
        private long _process_check_timer;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 초기화
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] Awake
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Awake()
        {
            Instance = this;

        }
        #endregion

      
        #region [초기화] Start
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override async void Start()
        {
            var cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            var ram = new PerformanceCounter("Memory", "Available MBytes");
            MdlDataCenter.NetComputer._cpu_info = string.Format("{0}", CPUSpeed());
            MdlDataCenter.NetComputer._memory_info = string.Format("{0}", ram.NextValue().ToString());
            CheckPort();
            //MdlDataCenter.NetComputer._memory_info = string.Format("RAM:{0:0,0}MB, VRAM:{1:0,0}MB)", SystemInfo.systemMemorySize, SystemInfo.graphicsMemorySize);

            TryConnectServer();
            
            if (MdlBase._unreal_editor_mode)
                await ExecuteDroneClient( true, 
                                        eMapName.ArenaBlocks,  
                                        "Car", 
                                        0, 
                                        "Car1", 
                                        eSpawnAreaType.Area, 
                                        -1, 
                                        Vector3.zero, 
                                        100f, 0.5f, 
                                        0,                  // idx
                                        "192.168.0.1",       // qgcIP
                                        false,              // isVRMode
                                        0, 
                                        0, 
                                        0, 
                                        "", 
                                        0, 
                                        false, 
                                        false);
        }
        #endregion

        public override void UpdateInternal()
        {
            base.UpdateInternal();

            if (Console.KeyAvailable)
            {
                var keyinfo = Console.ReadKey(true);
                switch (keyinfo.Key)
                {
                    case ConsoleKey.A:

                        break;
                }
            }
        }


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 네트워크
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [네트워크] Try Connect
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public void TryConnectServer()
        {
            if (!MdlDataCenter.NetConnector.IsConnected() && (JTimer.GetElaspedTime(_network_connecting_timer) > 5f))
            {
                Console.WriteLine("[ClientRecording] Try Connect.");
                ConnectToServer();                
            }
        }
        #endregion

        #region [네트워크] 접속
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ConnectToServer()
        {
            _network_connecting_timer = JTimer.GetCurrentTick();

            MdlDataCenter.NetConnector.Connect(MdlBase._launcher_server_ip, MdlBase._launcher_server_port, (r) =>
            {
                Console.WriteLine("Launcher Server Connected.");

                if (_first_connected)
                {
                    MdlNetMessageDispatcher.RegisterMessageHandlers(MdlDataCenter.NetConnector);
                    _first_connected = false;
                }

                MdlDataCenter.NetComputer._ip_address = NetTcpServer.GetLocalIPAddress(); // MdlDataCenter.NetConnector._ip_address;
                MdlDataCenter.NetConnector.SendMessage((int)ePacketProtocol.CLIENT_TYPE, (int)eClientType.Launcher, MdlDataCenter.NetComputer);
            });
        }
        #endregion


        #region [DroneClient] 실행
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<MdlDroneClient> ExecuteDroneClient(bool editor_client, 
                                                        eMapName map_name, 
                                                        string vehicle_type, 
                                                        int vehicle_model, 
                                                        string vehicle_name, 
                                                        eSpawnAreaType spawn_area, 
                                                        int spawn_area_index, 
                                                        Vector3 spawn_point, 
                                                        float spawn_sphere_radius, 
                                                        float sync_transform_interval, 
                                                        int id, 
                                                        string qgcip, 
                                                        bool isVRMode,
                                                        double gio_longi, double gio_lati, double gio_alti, 
                                                        string hitl_port, 
                                                        int hitl_idx, 
                                                        bool use_physx, 
                                                        bool lockstep)
        {
            while (_executing == true)
            {
                Console.WriteLine("[" + (vehicle_type == "HITL" ? hitl_idx.ToString() : id.ToString()) + "] Waiting for another client...");
                await JScheduler.Wait(1f);
            }
            _executing = true;
            var drone_client = CreateObject<MdlDroneClient>();
            if (editor_client)
                drone_client.NetData._guid = 0;
            drone_client.NetData.MapName = map_name;
            drone_client._guid = id;

            // 1. 세팅 파일 쓰기
            var setting = new DroneClientSetting();
            setting.StartMap = map_name.ToString();
            setting.DedicatedServerIP = MdlBase._launcher_server_ip;
            setting.VehicleType = vehicle_type;// todo: 관리툴에서 변경
            setting.ClientGuid = (int)drone_client.NetData._guid;
            setting.ScriptServerPort = drone_client.NetData._script_server_port;
            setting.SpawnAreaType = spawn_area;   
            setting.SpawnAreaIndex = spawn_area_index;
            setting.SpawnPointX = spawn_point.x;
            setting.SpawnPointY = spawn_point.y;
            setting.SpawnPointZ = spawn_point.z;
            setting.SpawnSphereRadius = spawn_sphere_radius;
            setting.VehicleVisibleName = drone_client.NetData.VehicleVisibleName = vehicle_name;
            setting.SyncTransformInterval = sync_transform_interval;
            setting.UsePhysx = use_physx;
            setting.IsVRMode = isVRMode;
            setting.VehicleType = vehicle_model.ToString();

            if (vehicle_type == "CPS")
            {
                setting.UseCPS = true;
                setting.Local_IP = MdlDataCenter.NetComputer._ip_address;
                setting.CPS_LocalPort = int.Parse(hitl_port);
            }

            if(vehicle_type != "Car")
            {
                //setting.VehicleType = vehicle_model.ToString();
            }

            drone_client.NetData.SpawnAreaType = spawn_area;
            drone_client.NetData.SpawnAreaIndex = spawn_area_index;
            drone_client.NetData.SpawnPoint = spawn_point;
            drone_client.NetData.SpawnSphereRadius = spawn_sphere_radius;
            drone_client.NetData._guid = id;

            if(!Directory.Exists(string.Format("{0}\\Settings", MdlBase._path_client)))
            {
                Directory.CreateDirectory(string.Format("{0}\\Settings", MdlBase._path_client));
                Directory.CreateDirectory(string.Format("{0}\\Settings\\Airsim", MdlBase._path_client));
                Directory.CreateDirectory(string.Format("{0}\\Settings\\MultiDrone", MdlBase._path_client));
            }
            else
            {
                if (!Directory.Exists(string.Format("{0}\\Settings\\Airsim", MdlBase._path_client)))
                    Directory.CreateDirectory(string.Format("{0}\\Settings\\Airsim", MdlBase._path_client));

                if (!Directory.Exists(string.Format("{0}\\Settings\\MultiDrone", MdlBase._path_client)))
                    Directory.CreateDirectory(string.Format("{0}\\Settings\\MultiDrone", MdlBase._path_client));
            }



            var json_str = JsonConvert.SerializeObject(setting);
            //File.WriteAllText(string.Format("{0}\\Settings\\MultiDrone\\MultiDroneSettings{1}.json", MdlBase._path_client, id), json_str);


            Console.WriteLine("[" + (vehicle_type == "HITL" ? hitl_idx.ToString() : id.ToString()) + "] Write MultiDroneSetting");

            var airsimsetting = new AirsimSetting();
            airsimsetting.IsVRMode = isVRMode;
            DroneVehicleSetting vehiclesetting = null;
            if(vehicle_type == "SimpleFlight")
            {
                vehiclesetting = new DroneSimpleFlightSetting();
                vehiclesetting.VehicleType = vehicle_type;

            }
            else if(vehicle_type == "Car")
            {
                vehiclesetting = new DroneCarSetting();
                vehiclesetting.VehicleType = vehicle_type;
                airsimsetting.SimMode = "Car";
            }
            else if(vehicle_type == "SITL")
            {
                vehiclesetting = new DronePX4Setting();
                ((DronePX4Setting)vehiclesetting).SettingSITL(id - 1, qgcip, lockstep);
                vehiclesetting.VehicleType = "PX4MultiRotor";
                airsimsetting.OriginGeopoint = new Geopoint(gio_longi, gio_lati, gio_alti);

                // PX4 실행
                var batch_arguments = string.Format("{0} {1}", MdlBase._path_px4_root, id-1);
                var batch = JUtil.ExecuteBatchFile(MdlBase._path_px4_root, MdlBase._px4_batch, batch_arguments);
                if (batch != null)
                    drone_client.NetData._batch_processId = batch.Id;
            }
            else if(vehicle_type == "HITL")
            {
                vehiclesetting = new DronePX4Setting();
                ((DronePX4Setting)vehiclesetting).SettingHITL(hitl_idx, qgcip, hitl_port);
                drone_client._guid = hitl_idx;
                drone_client.NetData._guid = hitl_idx;
                id = hitl_idx;
                vehiclesetting.VehicleType = "PX4MultiRotor";
                airsimsetting.OriginGeopoint = new Geopoint(gio_longi, gio_lati, gio_alti);
            }
            else if(vehicle_type == "CPS")
            {
                vehiclesetting = new DroneCPSSetting();
                vehiclesetting.VehicleType = "SimpleFlight";
                id = hitl_idx;
                drone_client.NetData._guid = hitl_idx;
                airsimsetting.OriginGeopoint = new Geopoint(gio_longi, gio_lati, gio_alti);
            }

            airsimsetting.Vehicles.Add(id.ToString(), vehiclesetting);
            var jsonsetting = JsonConvert.SerializeObject(airsimsetting);

            File.WriteAllText(string.Format("{0}\\Settings\\MultiDrone\\MultiDroneSettings{1}.json", MdlBase._path_client, id), json_str);

            File.WriteAllText(string.Format("{0}\\Settings\\Airsim\\settings{1}.json", MdlBase._path_client, id), jsonsetting);

            Console.WriteLine("[" + id.ToString() + "] Write AirsimSetting");

            // 2. 드론 클라이언트 실행
            if (editor_client)
            {
                drone_client.Initialize(0);
            }
            else
            {
                var process = ExecuteClientProcess(id);
                
                //SharedMemory 연결 확인까지 대기
                //while (!drone_client.CheckConnectClient())
                //{
                //    await JScheduler.Wait(1f);
                //    if (!JUtil.IsRunningProcess(process.Id))
                //        break;

                //    Console.WriteLine("[" + id.ToString() + "] Waiting For connect client to server...");
                //}
                if (process != null)
                    drone_client.Initialize(process.Id);

                if (!process.HasExited)
                {
                    drone_client.WindowHandle = process.MainWindowHandle;
                    drone_client._process = process;
                }
            }
            _executing = false;
            //await JScheduler.Wait(10f);
            return drone_client;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 업데이트
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [업데이트]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Update()
        {
            JTimer.Update();
            TryConnectServer();
            CheckClientAlive();
            CheckClientStatus();
            //CheckPort();
            MdlDataCenter.Update();

            if (Console.KeyAvailable)
            {
                var keyinfo = Console.ReadKey(true);
                switch (keyinfo.Key)
                {
                    case ConsoleKey.H:                        
                        break;
                }
            }
        }
        #endregion





        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Process
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Process] 실행
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Process ExecuteClientProcess(int id)
        {
            return JUtil.ExecuteProcess(MdlBase._path_client, MdlBase._filename_client, false, $"-vid {id}");
        }
        #endregion

        #region [Process] 종료
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void KillClientProcess(long client_guid)
        {
            var drone_client = MdlDataCenter.FindDroneSimulClient(client_guid);
            if (null == drone_client)
                return;
            //JUtil.KillProcess(MdlBase._filename_client); // todo: ProcessID 기억해서 종료
            JUtil.KillProcess(drone_client.NetData._processId);
            MdlDataCenter.RemoveDroneSimulClient(drone_client);
        }
        #endregion
		
		#region [Process] 모두종료
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void KillClientAllProcess()
        {
            JUtil.KillProcess(MdlBase._filename_client);
            MdlDataCenter.RemoveAllDroneSimulClients();
        }
        #endregion

        #region [Process] 프로세스가 살아 있는지 지속적 검사
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckClientAlive()
        {
            if (JTimer.GetElaspedTime(_process_check_timer) < 5f)
                return;

            _process_check_timer = JTimer.GetCurrentTick();
            var remove_clients = MdlDataCenter.DroneSimulClients.Where(client =>
            {
                if(!JUtil.IsRunningProcess(client.NetData._processId))
                {
                    Console.WriteLine("[Info] Process Termination Detected." + client.NetData._processId);
                    JUtil.KillProcess(client.NetData._batch_processId);
                    return true;
                }                
                return false;
            }).ToList();

            remove_clients.ForEach(c => MdlDataCenter.RemoveDroneSimulClient(c));
            if(remove_clients.Count > 0)
                MdlDataCenter.NetConnector.SendMessage((int)ePacketProtocol.LS_COMPUTER_INFO, MdlDataCenter.NetComputer);
        }
        #endregion

        #region [Process] PX4 모두 종료
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void KillAllPX4()
        {
            JUtil.KillProcess("px4");
        }
        #endregion

        #region [Status]
        public void CheckClientStatus()
        {
            
        }

        #endregion

        #region [유틸] CPUSpeed
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string CPUSpeed()
        {
            var processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);
            if ((processor_name != null) && (processor_name.GetValue("ProcessorNameString") != null))
            {
                string value = processor_name.GetValue("ProcessorNameString").ToString();
                //string freq = value.Split('@')[1];
                return value;
            }
            return "";
        }
        #endregion

        #region [포트] CheckPort
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async void CheckPort()
        {
                var temp_names = GetPortInfo();
                var port_infos = MdlDataCenter.NetComputer._ports;
                var exc_info = port_infos.Keys.Except(temp_names);
                if (port_infos.Count < temp_names.Count || exc_info != null && exc_info.Count() != 0)
                {
                    port_infos.Clear();
                    foreach (var name in temp_names)
                    {
                        if (name == null)
                            continue;
                        if (port_infos.ContainsKey(name))
                            return;
                        var id =  await GetMavSysIDByPortName(name);
                        if (id <= 0)
                        {
                            Console.WriteLine("[Error] Get MavSysID : " + name);
                            continue;
                        }
                        if (port_infos.ContainsKey(name))
                            port_infos[name] = id;
                        else
                            port_infos.Add(name, id);
                    }
                    MdlDataCenter.NetConnector.SendMessage((int)ePacketProtocol.LS_PORTS, JsonConvert.SerializeObject(port_infos));
                }                
        }
        #endregion

        #region [포트] SendPortInfo
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SendPortInfo()
        {
            var data = JsonConvert.SerializeObject(MdlDataCenter.NetComputer._ports);

            MdlDataCenter.NetConnector.SendMessage((int)ePacketProtocol.LS_PORTS, data);
        }
        #endregion

        #region [포트] GetPortInfo
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public List<string> GetPortInfo()
        {
            return SerialPort.GetPortNames().Distinct().ToList();
            //var search = new ManagementObjectSearcher(@"SELECT * FROM Win32_PnPEntity WHERE DESCRIPTION = 'MindPX'");
            //var g = search.Get();
            //var temp_names = new List<string>();
            //foreach (ManagementObject o in g)
            //{
            //    var match = Regex.Match((string)o.GetPropertyValue("Name"), @"COM\d");
            //    if(match.Success)
            //        temp_names.Add(match.Value);
            //}
            //g.Dispose();

            //return temp_names;
        }
        #endregion


        #region [유틸] ExecuteCommandToMavConsole
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ExecuteCommandToMavConsole(int id, string port, string command)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = @"./MavConsole.exe";
            info.Arguments = string.Format("{0} {1} {2}", port, 5, command);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.CreateNoWindow = true;
            var ps = Process.Start(info);

            using (var sr = ps.StandardOutput)
            {
                var output = sr.ReadToEnd();
                MdlDataCenter.NetConnector.SendMessage((int)ePacketProtocol.LS_PX4_OUTPUT, id, output);
                Console.WriteLine("[MSG] Execute Command");
            }
        }
        #endregion

        #region [유틸] ResetPixhawk
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RebootPixhawk(string port)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = @"./MavConsole.exe";
            info.Arguments = string.Format("{0} {1} {2}", port, 5, "reboot");
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.CreateNoWindow = true;
            var ps = Process.Start(info);
        }
        #endregion

        #region [유틸] GetMavSysIDByPortName
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<int> GetMavSysIDByPortName(string name)
        {
            //JUtil.KillProcess("MavConsole");
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = @"./MavConsole.exe";
            info.Arguments = name + " 5 1";
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.CreateNoWindow = true;
            var ps = Process.Start(info);
            var begin = JTimer.GetCurrentTick();
            using (var sr = ps.StandardOutput)
            {
                var task = Task.Run(()=>sr.ReadToEnd());

                while (!(task.IsCompleted && JTimer.GetElaspedTime(begin) > 10f))
                    await Task.Delay(1000);

                if (task.Status != TaskStatus.RanToCompletion)
                {
                    ps.Kill();
                    return -1;
                }

                Console.WriteLine(task.Result);

                var match = Regex.Match(task.Result, @"SYSTEM ID: \d+");
                var mv = match.Value;
                var id = Regex.Match(mv, @"\d+").Value;

                task.Dispose();

                if (match.Success)
                    return int.Parse(id);
                else
                    return -1;
            }
        }
        #endregion


        #region [유틸] OrganizeWindows
        public void OrganizeWindows()
        {
            var pss = Process.GetProcessesByName("Blocks").Select(s => s.MainWindowHandle).Where(ww => ww != IntPtr.Zero).ToArray();

            if (pss.Length < 2)
                return;

            int gridsize = 2;

            while(Math.Pow(gridsize,2) < pss.Length)
                gridsize++;

            PInvoke.DEVMODE mode = new PInvoke.DEVMODE();
            PInvoke.User32.EnumDisplaySettings(null, PInvoke.User32.ENUM_CURRENT_SETTINGS, ref mode);

            var h = ((int)(mode.dmPelsHeight * 0.9)) / gridsize;
            var w = (int)mode.dmPelsWidth / gridsize;
            
            int idx = 0;
            for (int i = 0; i < gridsize; ++i)
            {
                for (int j = 0; j < gridsize; ++j)
                {
                    if (idx >= pss.Count())
                        return;

                    PInvoke.User32.MoveWindow(pss[idx++], i * w, j * h, w, h, false);
                }
            }
        }
        #endregion


        [DllImport(@"E:\work\IPCDll\x64\Debug\IPCDll.dll", EntryPoint="fnIpcSender", CallingConvention = CallingConvention.StdCall)]
        public static extern int IpcSender(char[] data, uint dataLen);

        [DllImport(@"E:\work\IPCDll\x64\Debug\IPCDll.dll", EntryPoint = "fnIpcSenderClose", CallingConvention = CallingConvention.StdCall)]
        public static extern int IpcSenderClose();

    }

}
