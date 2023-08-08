using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using J2y.Network;
using System.Net;
using System.IO;
using System;
using System.Threading.Tasks;


namespace J2y.MultiDrone
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MdaDataCenter
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static class MdlDataCenter
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] NetData
        public static NetTcpClient NetConnector { get; set; }
        #endregion





        #region [변수] NetData
        public static MdNetData_Computer NetComputer { get; set; }        
        public static List<MdlDroneClient> DroneSimulClients { get; set; }
        public static MdNetData_SimulResult NetSimulResult { get; set; }
        #endregion


        #region [변수] Task
        public static List<Task<MdlDroneClient>> _executing_tasks = new List<Task<MdlDroneClient>>();
        public static Task<MdlDroneClient> _current_executing_task;
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메인 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        static MdlDataCenter()
        {
            NetConnector = new NetTcpClient();
            NetComputer = new MdNetData_Computer();
            NetSimulResult = new MdNetData_SimulResult();
            DroneSimulClients = new List<MdlDroneClient>();

            NetComputer._guid = JUtil.CreateUniqueId();
        }
        #endregion

        #region [업데이트] 메인
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Update()
        {
            NetConnector.Update();
            DroneSimulClients.ForEach(c => c.UpdateInternal());

            //if (_current_executing_task != null && _current_executing_task.IsCompleted)
            //    _current_executing_task = null;

            //if (_current_executing_task == null && _executing_tasks.Count != 0)
            //{
            //    var task = _executing_tasks.First();
            //    _current_executing_task = task;
            //    _current_executing_task.Start();
            //    _executing_tasks.Remove(task);
            //}
        }
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // DroneSimulClient
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [DroneSimulClient] Add/Remove
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void AddDroneSimulClient(MdlDroneClient client)
        {
            NetComputer._clients.Add(client.NetData);
            DroneSimulClients.Add(client);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RemoveDroneSimulClient(MdlDroneClient client)
        {
            NetComputer._clients.Remove(client.NetData);
            JObjectManager.Destroy(client);
            DroneSimulClients.Remove(client);
        }
		//------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RemoveAllDroneSimulClients()
        {
            NetComputer._clients.Clear();
            DroneSimulClients.ForEach(c => JObjectManager.Destroy(c));
            DroneSimulClients.Clear();
        }
        #endregion

        #region [DroneSimulClient] Find
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MdlDroneClient FindDroneSimulClient(long guid)
        {
            return DroneSimulClients.FirstOrDefault(c => c.NetData._guid == guid);
        }
        #endregion

        #region [DroneSimulClient] Set KML Data
        public static void SetKML(FPlacemark[] Placemarks)
        {
            foreach(var cli in DroneSimulClients)
            {
                
            }
        }
        #endregion
    }

}
