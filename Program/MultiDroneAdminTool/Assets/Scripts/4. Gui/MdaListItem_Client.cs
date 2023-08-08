using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using J2y.Network;
using UnityEngine.UI;

namespace J2y.MultiDrone
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MdaListItem_Client
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class MdaListItem_Client : JListItem_base
    {
        public static MdaListItem_Client Selected;

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] NetData
        public MdNetData_Client NetData { get; set; }
        #endregion


        #region [변수] UI
        public Text _title;
        public Image _background;
        public GameObject _selected_mark;
        public GameObject _rendering_mark;
        public GameObject _lidar_mark;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메인 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [초기화] Reset
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Reset()
        {
           
        }
        #endregion


       
        #region [초기화] Data
        //------------------------------------------------------------------------------------------------------------------------------------------------------    
        internal void SetData(MdNetData_Client netdata)
        {
            NetData = netdata;
            _title.text = netdata._guid.ToString();
           
            UpdateUI();
        }
        #endregion

        #region [UI] Update
        //------------------------------------------------------------------------------------------------------------------------------------------------------    
        internal void UpdateUI()
        {
            MdaWindow_Main.Instance._text_client_info.text = string.Format("ID:{0}\nProcessID:{1}\nConnection Time:{2}\nUsage - CPU:{3},\tRAM:{4}", NetData._guid, NetData._processId, NetData.ConnectTime.ToString(), NetData.cpu_usage, NetData.ram_usage);
            MdaWindow_Main.Instance._dropdown_map_name.value = (int)NetData.MapName;
            //MdaWindow_Main.Instance._dropdown_vehicle_type.value = (int)NetData._vehicle_type;
            MdaWindow_Main.Instance._dropdown_spawn_type.value = (int)NetData.SpawnAreaType;
            MdaWindow_Main.Instance._input_spawn_area_index.text = NetData.SpawnAreaIndex.ToString();
            MdaWindow_Main.Instance._input_spawn_points[0].text = NetData.SpawnPoint.x.ToString();
            MdaWindow_Main.Instance._input_spawn_points[1].text = NetData.SpawnPoint.y.ToString();
            MdaWindow_Main.Instance._input_spawn_points[2].text = NetData.SpawnPoint.z.ToString();
            MdaWindow_Main.Instance._input_spawn_sphere_radius.text = NetData.SpawnSphereRadius.ToString();
        }
        #endregion

        public void SetRenderingMark(bool onoff)
        {
            _rendering_mark.SetActive(onoff);
        }

        public void SetLidarMark(bool onoff)
        {
            _lidar_mark.SetActive(onoff);
        }

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메인 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [토글] 클릭
        //------------------------------------------------------------------------------------------------------------------------------------------------------    
        public void OnClick()
        {
            if (Selected != null)
                Selected._selected_mark.SetActive(false);
            MdaDataCenter.SelectedNetClient = NetData;
            _selected_mark.SetActive(true);
            UpdateUI();
            Selected = this;

            MdaWindow_Main.Instance.LoadSettingFromServer();

        }
        #endregion
    }


}
