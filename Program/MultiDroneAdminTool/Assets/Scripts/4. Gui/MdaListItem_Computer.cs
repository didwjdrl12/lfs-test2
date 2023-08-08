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
    // VtListItem_Container
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class MdaListItem_Computer : JListItem_base
    {
        public static MdaListItem_Computer Selected;

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] NetData
        public MdNetData_Computer NetData { get; set; }
        #endregion

        #region [변수] UI
        public Text _title;
        public GameObject _selected_mark;
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
        internal void SetData(MdNetData_Computer netdata)
        {
            NetData = netdata;
            _title.text = netdata._ip_address;

            //UpdateUI();
        }
        #endregion

        #region [UI] Update
        //------------------------------------------------------------------------------------------------------------------------------------------------------    
        internal void UpdateUI()
        {
            var txt = string.Format("ID:{0}\t\tIP:{1}\nCPU:{2}\t\tMemory:{3}\n", NetData._guid, NetData._ip_address, NetData._cpu_info, NetData._memory_info);
            var txt_ports = "Ports : ";
            foreach(var p in NetData._ports)
            {
                txt_ports += p.Key + " : " + p.Value.ToString() + "    ";
            }
            if (NetData._ports.Count == 0)
                txt_ports += "NONE";
            MdaWindow_Main.Instance._text_computer_info.text = txt + txt_ports;
        }
        #endregion

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

            MdaWindow_Main.Instance.CommitSpawnPoint();
            MdaDataCenter.SelectedNetComputer = NetData;

            MdaWindow_Main.Instance.OnClick_CreateSpawnSetting();
            MdaWindow_Main.Instance.UpdateSpawnPointList();

            MdaWindow_Main.Instance._input_add_count.text = NetData._saved_spawn_points.Count.ToString();

            MdaWindow_Main.Instance.UpdateClientListUI(NetData);
            _selected_mark.SetActive(true);
            UpdateUI();
            Selected = this;

            MdaWindow_Main.Instance.UpdateComportList();
            //MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_REQ_PORTS, NetData._guid);

            //MdaWindow_Main.Instance.ClearHITLInfo();

            //foreach(var d in NetData._dic_hitl_info)
            //    MdaWindow_Main.Instance.CreateHITLInfo(d.Key, d.Value);
        }        
        #endregion
    }


}
