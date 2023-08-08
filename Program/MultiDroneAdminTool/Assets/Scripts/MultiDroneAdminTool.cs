using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using J2y.Network;
using System.Linq;
using Newtonsoft.Json;

namespace J2y.MultiDrone
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MultiDroneAdminTool
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class MultiDroneAdminTool : MonoBehaviour
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [변수] Manager
        public JWindowManager _window_manager;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메인 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] Awake
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Awake()
        {
            MdaBase.LoadConfig();
        }
        #endregion
        
        #region [초기화] Start
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Start()
        {
            MdaDataCenter.NetConnector = new NetTcpClient();
            StartCoroutine(coroutine_tryconnect());
        }

        IEnumerator coroutine_tryconnect()
        {
            while(true)
            {
                if (!MdaDataCenter.NetConnector.IsConnected())
                    TryConnect();



                yield return new WaitForSeconds(5f);
            }
        }
        #endregion
        
        #region [종료] OnDisable
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnDisable()
        {
            MdaDataCenter.NetConnector.Disconnect();
        }
        #endregion

        public void RegistMessageHandlers()
        {
            Debug.Log("Launcher Server Connected.");

            MdaDataCenter.NetConnector.RegisterMessageHandler((int)ePacketProtocol.SA_CLIENT_LIST, (peer, reader) =>
            {
                MdaDataCenter.NetComputers.Clear();
                var pc_count = reader.ReadInt32();
                for (int i = 0; i < pc_count; ++i)
                {
                    var net_pc = new MdNetData_Computer();
                    net_pc.Read(reader);

                    MdaDataCenter.NetComputers.Add(net_pc);

                    if (null == MdaDataCenter.SelectedNetComputer)
                        MdaDataCenter.SelectedNetComputer = net_pc;
                }

                MdaWindow_Main.Instance.UpdateComputerListUI();
            });

            MdaDataCenter.NetConnector.RegisterMessageHandler((int)ePacketProtocol.SA_HITL_PORTS, (peer, reader) =>
            {
                var id = reader.ReadInt64();

                var com = MdaDataCenter.NetComputers.Where(w => w._guid == id).FirstOrDefault();

                if (com == null)
                    return;

                var json = reader.ReadString();
                var info = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);

                com._ports = info;
                MdaWindow_Main.Instance.UpdateComputerListUI();
                //MdaWindow_Main.Instance.UpdateHITLPosition();
            });

            MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.CLIENT_TYPE, (int)eClientType.AdminTool);
            MdaDataCenter.NetConnector.RegisterMessageHandler((int)ePacketProtocol.CLIENT_TYPE, (sender, reader) =>
            {
                var res = reader.ReadString();
                Console.WriteLine("[MSG] sc_client_type:" + res);
                var map = reader.ReadInt32();
                if (map != -1)
                {
                    MdaWindow_Main.Instance._dropdown_map_name.value = map;
                    MdaWindow_Main.Instance._dropdown_map_name.interactable = false;
                }
                MdaWindow_Main.Instance.OnClick_RefreshClientList();
            });

            MdaDataCenter.NetConnector.RegisterMessageHandler((int)ePacketProtocol.SA_SPAWN_ERROR, (sender, reader) =>
            {
                var idx = reader.ReadInt32();
                MdaWindow_Main.Instance.ShowPopupMessage(5, idx.ToString());
            });


            MdaDataCenter.NetConnector.RegisterMessageHandler((int)ePacketProtocol.SA_SMS, (sender, reader) =>
            {
                var id = reader.ReadInt64();
                var sms = reader.ReadSingle();

                foreach (var com in MdaDataCenter.NetComputers)
                {
                    var find = com._clients.Where(w => w._guid == id).FirstOrDefault();
                    if (find != null)
                        find.Smoothness = sms;
                }


                var monitor = MdaWindow_Main.Instance._monitor_item_list.Where(w => w._id == id).FirstOrDefault();
                if (monitor == null)
                    return;

                monitor.CheckStopSmoothness(sms);

                //MdaWindow_Main.Instance.OnClick_Monitor_Sms(false);
            });

            MdaDataCenter.NetConnector.RegisterMessageHandler((int)ePacketProtocol.NOTI_USER_MOVE, (sender, reader) =>
            {
                var id = reader.ReadInt64();
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                var z = reader.ReadSingle();

                if (MdaWindow_Main.Instance._dropdown_map_name.value == 1)
                {
                    x -= 6740;
                    y -= -790;
                    z -= 150;
                }

                var find = MdaWindow_Main.Instance._monitor_item_list.Where(w => w._id == id).FirstOrDefault();
                if (find == null)
                    return;

                find.SetPosition(x, y, z);

            });

            MdaDataCenter.NetConnector.RegisterMessageHandler((int)ePacketProtocol.SA_PX4_OUTPUT, (sender, reader) =>
            {
                var px_id = reader.ReadInt32();
                var output = reader.ReadString();
                MdaWindow_Main.Instance._console_output.text += string.Format("\n{0}\n{1}", px_id, output);
                


            });

            MdaDataCenter.NetConnector.RegisterMessageHandler((int)ePacketProtocol.SA_USAGE, (sender, reader) => 
            {
                var guid = reader.ReadInt64();
                var cpu_usage = reader.ReadSingle();
                var ram_usage = reader.ReadInt64();

                foreach(var com in MdaDataCenter.NetComputers)
                {
                    foreach(var cli in com._clients)
                    {
                        if(cli._guid == guid)
                        {
                            cli.cpu_usage = cpu_usage;
                            cli.ram_usage = ram_usage;
                        }
                    }
                }

            });
        }

        #region [업데이트] 메인
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Update()
        {
            MdaDataCenter.NetConnector.Update();
        }

        public void TryConnect()
        {
            if (MdaDataCenter.NetConnector.IsConnected())
                return;
            MdaDataCenter.NetConnector.Connect(MdaBase._launcher_server_ip, MdaBase._launcher_server_port, (r) => RegistMessageHandlers());
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 내부 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    }

}
