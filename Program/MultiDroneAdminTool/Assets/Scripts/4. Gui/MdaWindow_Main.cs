using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using J2y.Network;
using System.IO;
using TMPro;
using System.Linq;
using PygmyMonkey.FileBrowser;

namespace J2y.MultiDrone
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MdaWindow_Main
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public partial class MdaWindow_Main : JWindow_base
    {
        public static MdaWindow_Main Instance;

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // inner class
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        #region [class] SpawnPointInfo
        public class SpawnPointInfo
        {
            public SpawnPointInfo(GameObject obj, InputField[] inputs, Button btn_remove)
            {
                _obj = obj;
                _inputs = inputs;
                _btn_remove = btn_remove;
            }
            public GameObject _obj;
            public InputField[] _inputs;
            public Button _btn_remove;

            public int GetID()
            {
                return int.Parse(_inputs[0].text);
            }
            public Vector3 GetVectorPos()
            {
                float x = 0f;
                float y = 0f;
                float z = 0f;

                float.TryParse(_inputs[1].text, out x);
                float.TryParse(_inputs[2].text, out y);
                float.TryParse(_inputs[3].text, out z);

                return new Vector3(x, y, z);
            }

            public void Destroy()
            {
                GameObject.Destroy(_obj);
            }
        }
        #endregion
        
        #region [class] Geopoint
        public class Geopoint
        {
            public string Longitute;
            public string Latitute;
            public string Altitute;
        }
        #endregion
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // enum
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [enum] Tab
        public enum eTab
        {
            Config, CaptureResult, Max
        }
        #endregion

               
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [변수] Computer List
        public GameObject _listitem_computer_prefab;
        public Transform _listitem_computer_root;
        #endregion

        #region [변수] Client List
        public GameObject _listitem_client_prefab;
        public Transform _listitem_client_root;

        public InputField _input_add_count;
        public Dropdown _dropdown_map_name;
        public Dropdown _dropdown_vehicle_type;
        #endregion

        #region [변수] Spawn Option        
        public Dropdown _dropdown_spawn_type;
        public InputField _input_spawn_area_index;
        public InputField[] _input_spawn_points = new InputField[3];
        public InputField _input_spawn_sphere_radius;
        public InputField _input_spawn_name;

        public GameObject[] _spawntype_menus;
        public InputField _input_spawn_setting_path;

        private List<SpawnPointInfo> _list_spawn_point = new List<SpawnPointInfo>();
        //private Dictionary<string, long> _dic_hitl = new Dictionary<string, long>();
        public Transform _spawn_list_root;
        public GameObject _spawn_point_prefab;

        public InputField _input_sync_transform_interval;

        public Button _btn_all_pc;

        public InputField _input_geo_longi;
        public InputField _input_geo_lati;
        public InputField _input_geo_alti;

        public Button[] _btns_unused_hitl;

        public InputField _input_cps_idx;
        public InputField _input_cps_port;
		
		public Toggle _toggle_vrmode;

        #endregion

        #region [변수] PC and Client Info
        public Text _text_computer_info;
        public Text _text_client_info;

        public InputField _input_qgc_ip;
        public InputField _input_qgc_port;
        #endregion

        #region [변수] Monitor
        public GameObject _monitor_item_prefab;
        public Transform _monitor_item_root;
        public List<MdaListItem_Monitor> _monitor_item_list;
        public Toggle _monitor_autostop;
        public InputField _monitor_timer;

        #endregion

        #region [변수] Popup

        public enum ePopupMessageType
        {

        }

        public GameObject _popup_root;
        public Text _popup_text;
        #endregion

        #region [변수] Console
        public Transform _comports_group;
        public InputField _console_command;
        public InputField _console_output;
        #endregion

        public Button _btn_select_model;
        private int _selected_drone_model = 0;

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메인 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] Awake
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Awake()
        {
            Instance = this;
            _input_qgc_ip.text = PlayerPrefs.GetString("QGCIP", "127.0.0.1");
            _input_geo_alti.text = PlayerPrefs.GetString("alti", "20");
            _input_geo_lati.text = PlayerPrefs.GetString("lati", "36.383560");
            _input_geo_longi.text = PlayerPrefs.GetString("long", "127.366554"); 
        }
        #endregion

        #region [초기화] Start
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Start()
        {
          
        }
        #endregion

        public override void OnDisable()
        {
            SaveGeopoint();
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) && _console_command.isFocused)
                SendConsoleCommand();

        }


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // PC, Client List
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Client] [버튼] Refresh
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_RefreshClientList()
        {
            //Debug.Log(_input_qgc_ip.text);
            MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_CLIENT_LIST);
            

            
        }
        #endregion

        #region [Client] [버튼] 추가
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_AddClient()
        {
            if (null == MdaDataCenter.SelectedNetComputer)
            {
                ShowPopupMessage(0);
                return;
            }

            var com_guid = MdaDataCenter.SelectedNetComputer._guid;
            var count = 1;
            var map_name = (eMapName)_dropdown_map_name.value;
            var vehicle_type = _dropdown_vehicle_type.options[_dropdown_vehicle_type.value].text;
            var spawn_area = (eSpawnAreaType)_dropdown_spawn_type.value;
            var spawn_area_index = -1;
            var spawn_point = Vector3.zero;
            var spawn_sphere_radius = 100f;
            var vehicle_name = _input_spawn_name.text;
            //var sync_transform_interval = 0f;
            //float.TryParse(_input_sync_transform_interval.text, out sync_transform_interval);
            var qgc_ip = _input_qgc_ip.text;
            bool isVRMode = _selected_drone_model == 3; //_toggle_vrmode != null ? _toggle_vrmode.isOn : false;
            //Debug.Log(sync_transform_interval);


            if (_dropdown_vehicle_type.value == 4)
            {
                int idx;
                int.TryParse(_input_cps_idx.text, out idx);

                AddClient(com_guid, 1, map_name, vehicle_type, vehicle_name, spawn_area, spawn_area_index, spawn_point, spawn_sphere_radius, qgc_ip,isVRMode, _input_cps_port.text, idx);
            }
            else if (_dropdown_spawn_type.value == 0 || _dropdown_spawn_type.value == 1)
            {
                int.TryParse(_input_add_count.text, out count);
                int.TryParse(_input_spawn_area_index.text, out spawn_area_index);
                float.TryParse(_input_spawn_sphere_radius.text, out spawn_sphere_radius);

                if (MdaBase._temp_minimum_version)
                {
                    map_name = eMapName.ArenaNeighborhood;
                    spawn_area = eSpawnAreaType.Area;
                    spawn_area_index = -1;
                }

                if (_dropdown_vehicle_type.value == 3)
                {
                    if (MdaDataCenter.SelectedNetComputer._ports.Count == 0)
                    {
                        ShowPopupMessage(1, MdaDataCenter.SelectedNetComputer._ip_address);
                        return;
                    }

                    foreach (var hitl in MdaDataCenter.SelectedNetComputer._ports)
                    {
                        if (hitl.Value == 0)
                        {
                            ShowPopupMessage(2, MdaDataCenter.SelectedNetComputer._ip_address, hitl.Key);
                            return;
                        }
                    }

                    foreach (var hitl in MdaDataCenter.SelectedNetComputer._ports)
                    {
                        AddClient(com_guid, 1, map_name, vehicle_type, vehicle_name, spawn_area, spawn_area_index, spawn_point, spawn_sphere_radius, qgc_ip,isVRMode, hitl.Key, hitl.Value);
                    }

                }
                else
                    AddClient(com_guid, count, map_name, vehicle_type, vehicle_name, spawn_area, spawn_area_index, spawn_point, spawn_sphere_radius, qgc_ip, isVRMode);
            }
            else if(_dropdown_spawn_type.value == 2)
            {
                if (_list_spawn_point == null || _list_spawn_point.Count == 0)
                {
                    ShowPopupMessage(3);
                    return;
                }

                if (_dropdown_vehicle_type.value == 3)
                {
                    if (MdaDataCenter.SelectedNetComputer._ports.Count == 0)
                    {
                        ShowPopupMessage(1, MdaDataCenter.SelectedNetComputer._ip_address);
                        return;
                    }

                    foreach (var hitl in MdaDataCenter.SelectedNetComputer._ports)
                    {
                        if (hitl.Value == 0)
                        {
                            ShowPopupMessage(2, MdaDataCenter.SelectedNetComputer._ip_address, hitl.Key);
                            return;
                        }
                    }

                    foreach (var sp in _list_spawn_point)
                    {
                        var hitl = MdaDataCenter.SelectedNetComputer._ports.Where(w => w.Value.ToString() == sp._inputs[0].text.Replace("#", "")).FirstOrDefault();
                        spawn_point = sp.GetVectorPos();
                        AddClient(com_guid, 1, map_name, vehicle_type, vehicle_name, spawn_area, spawn_area_index, spawn_point, spawn_sphere_radius, qgc_ip,isVRMode, hitl.Key, hitl.Value);
                    }

                }
                else
                    foreach (var sp in _list_spawn_point)
                    {
                        spawn_point = sp.GetVectorPos();
                        AddClient(com_guid, count, map_name, vehicle_type, vehicle_name, spawn_area, spawn_area_index, spawn_point, spawn_sphere_radius, qgc_ip, isVRMode, "", sp.GetID());
                    }
            }

            _dropdown_map_name.interactable = false;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Onclick_AddClientAllPC()
        {
            //if (_dropdown_spawn_type.value != 0 && _dropdown_spawn_type.value != 1)
            //{
            //    ShowPopupMessage(4, _dropdown_spawn_type.itemText.text);
            //    return;
            //}

            var count = 1;

            var map_name = (eMapName)_dropdown_map_name.value;
            var vehicle_type = _dropdown_vehicle_type.options[_dropdown_vehicle_type.value].text;
            var spawn_area_index = -1;
            var spawn_point = Vector3.zero;
            var spawn_sphere_radius = 100f;
            var vehicle_name = _input_spawn_name.text;

            var qgc_ip = _input_qgc_ip.text;
            var qgc_port = 0;
            int.TryParse(_input_qgc_port.text, out qgc_port);

            bool isVRMode = _toggle_vrmode!=null ?  _toggle_vrmode.isOn : false;

            if (_dropdown_spawn_type.value == 2)
            {
                //if (_list_spawn_point == null || _list_spawn_point.Count == 0)
                //{
                //    ShowPopupMessage(3);
                //    return;
                //}

                foreach (var com in MdaDataCenter.NetComputers)
                {
                    foreach(var p in com._saved_spawn_points)
                    {
                        spawn_point = p.Value;

                        if (_dropdown_vehicle_type.value == 3)
                        {
                            //hitl
                            var hitl = com._ports.Where(w => w.Value == p.Key).FirstOrDefault();
                            AddClient(com._guid, 1, map_name, vehicle_type, vehicle_name, eSpawnAreaType.Point, spawn_area_index, spawn_point, spawn_sphere_radius, qgc_ip,isVRMode, hitl.Key, hitl.Value);
                        }
                        else
                        {
                            AddClient(com._guid, count, map_name, vehicle_type, vehicle_name, eSpawnAreaType.Point, spawn_area_index, spawn_point, spawn_sphere_radius, qgc_ip, isVRMode);
                        }
                    }
                }
            }
            else
            {
                int.TryParse(_input_add_count.text, out count);
                var spawn_area = (eSpawnAreaType)_dropdown_spawn_type.value;
                //var sync_transform_interval = 0f;
                int.TryParse(_input_spawn_area_index.text, out spawn_area_index);
                float.TryParse(_input_spawn_points[0].text, out spawn_point.x);
                float.TryParse(_input_spawn_points[1].text, out spawn_point.y);
                float.TryParse(_input_spawn_points[2].text, out spawn_point.z);
                float.TryParse(_input_spawn_sphere_radius.text, out spawn_sphere_radius);
                //float.TryParse(_input_sync_transform_interval.text, out sync_transform_interval);                

                foreach (var com in MdaDataCenter.NetComputers)
                {
                    AddClient(com._guid, count, map_name, vehicle_type, vehicle_name, spawn_area, spawn_area_index, spawn_point, spawn_sphere_radius, qgc_ip, isVRMode);
                }

                _dropdown_map_name.interactable = false;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddClient(	long com_guid, int count, eMapName map_name, string vehicle_type, string vehicle_name, eSpawnAreaType spawn_area, 
								int spawn_area_index, Vector3 spawn_point, float spawn_sphere_radius, string qgc_ip, bool isVRMode, string port = "", long idx = 0)
        {
            for(int i = 0; i < count; ++i)
                MdaDataCenter.NetConnector.SendMessage(
					(int)ePacketProtocol.AS_CLIENT_ADD, 
					com_guid, 
					1, 
					(int)map_name, 
					vehicle_type, 
					_selected_drone_model,
					vehicle_name, 
					(int)spawn_area, 
					spawn_area_index, 
					spawn_point.x, spawn_point.y, spawn_point.z, spawn_sphere_radius, 
					qgc_ip, 
                    isVRMode,
					_input_geo_longi.text, _input_geo_lati.text, _input_geo_alti.text, 
					port, 
					idx);
        }
        #endregion

        #region [Client] [버튼] 종료
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_KillClient()
        {
            var select_type = Array.IndexOf(_toggles, _toggles.Where(w => w.isOn).FirstOrDefault());
            var com_guid = MdaDataCenter.SelectedNetComputer._guid;

            switch (select_type)
            {
                case 0:
                    if (null == MdaDataCenter.SelectedNetClient)
                        return;

                    var client_guid = MdaDataCenter.SelectedNetClient._guid;
                    MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_CLIENT_KILL, com_guid, client_guid);

                    MdaDataCenter.SelectedNetClient = null;
                    break;
                case 1:
                    if (null == MdaDataCenter.SelectedNetComputer)
                        return;

                    MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_CLIENT_KILL_ALL, com_guid);
                    MdaDataCenter.SelectedNetClient = null;
                    break;
                case 2:
                    foreach (var com in MdaDataCenter.NetComputers)
                        MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_CLIENT_KILL_ALL, com._guid);
                    MdaDataCenter.SelectedNetClient = null;
                    break;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_KillErrorClient()
        {
            //1 : wait 2 : error 3 : error 4: ok
            if (null == MdaDataCenter.SelectedNetComputer)
                return;

            //var com_guid = MdaDataCenter.SelectedNetComputer._guid;

            foreach (var com in MdaDataCenter.NetComputers)
            {
                foreach (var client in com._clients)
                {
                    if (client._state != eClientState.Spawn)
                    {
                        MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_CLIENT_KILL, com._guid, client._guid);
                        MdaDataCenter.SelectedNetClient = null;
                    }
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_KillPX4()
        {
            var select_type = Array.IndexOf(_toggles, _toggles.Where(w => w.isOn).FirstOrDefault());

            switch(select_type)
            {
                case 1:
                    if (null == MdaDataCenter.SelectedNetComputer)
                        return;

                    MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_PX4_KILL, MdaDataCenter.SelectedNetComputer._guid);
                    break;
                case 2:
                    foreach(var com in MdaDataCenter.NetComputers)
                        MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_PX4_KILL, com._guid);
                    break;

                default:
                    break;
            }

            

        }

        #endregion

        #region [Client] [버튼] 재시작
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async void OnClick_RestartClient()
        {
            var select_type = Array.IndexOf(_toggles, _toggles.Where(w => w.isOn).FirstOrDefault());
            OnClick_KillClient();
            await new WaitForSeconds(1f);

            switch (select_type)
            {
                case 0:
                case 1:        
                    OnClick_AddClient();
                    break;
                case 2:
                    Onclick_AddClientAllPC();
                    break;

            }
            
        }
        #endregion

        #region [Client] [버튼] 모두 재시작
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async void OnClick_RestartClientAll()
        {
            var net_coms = new List<MdNetData_Computer>();
            foreach (var net_com in MdaDataCenter.NetComputers)
            {
                net_coms.Add(net_com.Clone() as MdNetData_Computer);
                MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_CLIENT_KILL_ALL, net_com._guid);                
            }
            //var sync_transform_interval = 0f;
            //float.TryParse(_input_sync_transform_interval.text, out sync_transform_interval);
            MdaDataCenter.SelectedNetClient = null;
            await new WaitForSeconds(3f);

            var qgc_ip = _input_qgc_ip.text;
            var qgc_port = 0;
            int.TryParse(_input_qgc_port.text, out qgc_port);
            bool isVRMode = _toggle_vrmode!=null ? _toggle_vrmode.isOn : false;

            // 재실행
            foreach (var net_com in net_coms)
            {
                foreach (var net_client in net_com._clients)
                {
                    AddClient(net_com._guid, 1, net_client.MapName, net_client._vehicle_type, net_client.VehicleVisibleName, net_client.SpawnAreaType, net_client.SpawnAreaIndex, net_client.SpawnPoint, net_client.SpawnSphereRadius, qgc_ip, isVRMode);
                    await new WaitForSeconds(4f);                    
                }
            }
        }
        #endregion

        #region [Client] [버튼] Create SpawnSetting
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_CreateSpawnSetting()
        {
            for (int i = _list_spawn_point.Count-1; i >= 0; --i)
                OnClick_RemoveSpawnPoint(i);

            _input_spawn_setting_path.text = "";
            //MdaDataCenter.SelectedNetComputer._saved_spawn_points.Clear();
        }
        #endregion

        #region [Client] [버튼] Add Spawn Point
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_AddSpawnPoint()
        {
            CreateSpawnPoint(_list_spawn_point.Count);
            _input_add_count.text = _list_spawn_point.Count.ToString();
        }
        #endregion

        #region [Client] [버튼] Remove Spawn Point
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_RemoveSpawnPoint(int index)
        {
            for (int i = index + 1; i < _list_spawn_point.Count; ++i)
            {
                //_list_spawn_point[i]._label.text = "POS " + (i).ToString("D2");
                _list_spawn_point[i]._btn_remove.onClick.RemoveAllListeners();
                var num = i - 1;
                _list_spawn_point[i]._btn_remove.onClick.AddListener(() =>
                {
                    OnClick_RemoveSpawnPoint(num);
                });
            }
            Destroy(_list_spawn_point[index]._obj);
            _list_spawn_point.RemoveAt(index);
            _input_add_count.text = _list_spawn_point.Count.ToString();
            UpdateSpawnPointLabel();
            //MdaDataCenter.SelectedNetComputer._saved_spawn_points.Remove(index + 1);
        }
        #endregion

        #region [Client] [InputField] ChangeSpawnPointList
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void UpdateSpawnPointList()
        {
            if (MdaDataCenter.SelectedNetComputer == null)
                return; 

            var dic = MdaDataCenter.SelectedNetComputer._saved_spawn_points;
            if (dic.Count <= 0 || dic == null)
                OnClick_CreateSpawnSetting();
            else
            {
                if (_dropdown_vehicle_type.value == 3)
                    foreach (var d in dic)
                        CreateSpawnPointForHITL(d.Key.ToString(), d.Value.x, d.Value.y, d.Value.z);
                else
                    for (int i = 0; i < dic.Count; ++i)
                    {
                        var key = dic.Keys.ToArray()[i];
                        CreateSpawnPoint(i, dic[key].x, dic[key].y, dic[key].z);
                    }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CommitSpawnPoint()
        {
            var dic_sp = _list_spawn_point.ToDictionary(k => int.Parse(k._inputs[0].text.Replace("#", "")));
            var sps = MdaDataCenter.SelectedNetComputer._saved_spawn_points;
            sps.Clear();
            foreach(var sp in dic_sp)
            {
                var id = sp.Key;
                var pos = new Vector3(float.Parse(sp.Value._inputs[1].text), float.Parse(sp.Value._inputs[2].text), float.Parse(sp.Value._inputs[3].text));
                sps.Add(id, pos);
            }
        }
        #endregion

        #region [Client] [버튼] SpawnSetting Load
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_OpenSpawnSetting()
        {
            FileBrowser.OpenFilePanel("Setting load", Environment.CurrentDirectory, new string[] { "json", }, null, (bool canceled, string filePath) =>
            {
                if (canceled)
                    return;
                
                var json = File.ReadAllText(filePath);
                var info = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, string>>>(json);

                foreach(var com in MdaDataCenter.NetComputers)
                    com._saved_spawn_points.Clear();

                foreach(var com in info)
                {
                    var find_com = MdaDataCenter.NetComputers.Where(w => w._ip_address == com.Key).FirstOrDefault();
                    if (find_com == null)
                        continue;

                    if (_dropdown_vehicle_type.value == 3)
                    {
                        foreach(var port in find_com._ports)
                            find_com._saved_spawn_points.Add(port.Value, Vector3.zero);

                        foreach (var p in com.Value)
                            if (find_com._ports.ContainsValue(p.Key))
                                find_com._saved_spawn_points[p.Key] = p.Value.ToVector3();
                    }
                    else
                        find_com._saved_spawn_points = com.Value.Select((s) => new { s.Key, v = com.Value[s.Key].ToVector3() }).ToDictionary(x => x.Key, x => x.v);
                }

                OnClick_CreateSpawnSetting();
                UpdateSpawnPointList();
                UpdateSpawnPointLabel();
                _input_spawn_setting_path.text = filePath;
                
                _input_add_count.text = _list_spawn_point.Count.ToString();
            });
        }

        #endregion

        #region [Client] [버튼] SpawnSetting Save
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_SaveSpawnSetting()
        {
            FileBrowser.SaveFilePanel("Setting save", "Setting Save", Environment.CurrentDirectory, "spawnsetting.json", new string[] { "json", }, null, (bool canceled, string filePath) =>
            {
                if (canceled)
                    return;

                CommitSpawnPoint();

                var obj = new Dictionary<string, Dictionary<int, string>>();
                foreach(var com in MdaDataCenter.NetComputers)
                {
                    obj.Add(com._ip_address, new Dictionary<int, string>());
                    foreach(var pos in com._saved_spawn_points)
                    {
                        obj[com._ip_address].Add(pos.Key, pos.Value.ToString());
                    }
                }
                
                if (!filePath.EndsWith(".json"))
                    filePath += ".json";

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                File.WriteAllText(filePath, json);
            });
        }
        #endregion

        #region [Client] [버튼] GeoPoint Load
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_OpenGeoPointSetting()
        {
            FileBrowser.OpenFilePanel("Setting load", Environment.CurrentDirectory, new string[] { "json", }, null, (bool canceled, string filePath) =>
            {
                if (canceled)
                    return;

                var json = File.ReadAllText(filePath);
                var point = Newtonsoft.Json.JsonConvert.DeserializeObject<Geopoint>(json);

                _input_geo_longi.text = point.Longitute;
                _input_geo_lati.text = point.Latitute;
                _input_geo_alti.text = point.Altitute;
            });
        }

        #endregion

        #region [Client] [버튼] GeoPoint Save
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_SaveGeoPointSetting()
        {
            FileBrowser.SaveFilePanel("Setting save", "Setting Save", Environment.CurrentDirectory, "OriginGeopoint.json", new string[] { "json", }, null, (bool canceled, string filePath) =>
            {
                if (canceled)
                    return;

                Geopoint point = new Geopoint();
                point.Longitute = _input_geo_longi.text;
                point.Latitute = _input_geo_lati.text;
                point.Altitute = _input_geo_alti.text;

                if (!filePath.EndsWith(".json"))
                    filePath += ".json";

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(point);
                File.WriteAllText(filePath, json);
            });
        }
        #endregion
        
        #region [Setting] Save QGCIP
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SaveQGCIP()
        {
            PlayerPrefs.SetString("QGCIP", _input_qgc_ip.text);
        }
        #endregion

        #region [Setting] Save Geopoint
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SaveGeopoint()
        {
            PlayerPrefs.SetString("alti", _input_geo_alti.text);
            PlayerPrefs.SetString("lati", _input_geo_lati.text);
            PlayerPrefs.SetString("long", _input_geo_longi.text);
        }
        #endregion

        //#region [Client] [버튼] Remove Spawn Point
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public void OnClick_RemoveSpawnPoint(int index)
        //{
        //    for(int i = index+1; i < _list_spawn_point.Count; ++i)
        //    {
        //        _list_spawn_point[index]._label.text = "POS " + (i + 1).ToString("D2");
        //        _list_spawn_point[index]._btn_remove.onClick.RemoveAllListeners();
        //        _list_spawn_point[index]._btn_remove.onClick.AddListener(() =>
        //        {
        //            OnClick_RemoveSpawnPoint(i);
        //        });
        //    }
        //    Destroy(_list_spawn_point[index]._obj);
        //    _list_spawn_point.RemoveAt(index);
        //}
        //#endregion

        #region [Client] [버튼] CreateSpawnPoint
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CreateSpawnPoint(int index, float x = 0f, float y = 0f, float z = 0f)
        {
            var obj = Instantiate(_spawn_point_prefab, _spawn_list_root.transform);

            var inputs = obj.GetComponentsInChildren<InputField>();
            inputs[0].text = (index+1).ToString("D2");

            inputs[1].text = x.ToString();
            inputs[2].text = y.ToString();
            inputs[3].text = z.ToString();
            var btn_remove = obj.GetComponentInChildren<Button>();

            btn_remove.onClick.AddListener(() =>
            {
                OnClick_RemoveSpawnPoint(index);
            });

            _list_spawn_point.Add(new SpawnPointInfo(obj, inputs, btn_remove));
            UpdateSpawnPointLabel();
            //MdaDataCenter.SelectedNetComputer._saved_spawn_points.Add(index + 1, new Vector3(x, y, z));

        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CreateSpawnPointForHITL(string name, float x = 0f, float y = 0f, float z = 0f)
        {
            var obj = Instantiate(_spawn_point_prefab, _spawn_list_root.transform);

            var inputs = obj.GetComponentsInChildren<InputField>();
            inputs[0].text = name;

            inputs[1].text = x.ToString();
            inputs[2].text = y.ToString();
            inputs[3].text = z.ToString();

            var btn_remove = obj.GetComponentInChildren<Button>();
            btn_remove.gameObject.SetActive(false);

            _list_spawn_point.Add(new SpawnPointInfo(obj, inputs, btn_remove));

        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void UpdateSpawnPointLabel()
        {
            if (_dropdown_spawn_type.value != 2 || _dropdown_vehicle_type.value == 3)
                return;

            List<long> clients = new List<long>();

            foreach (var com in MdaDataCenter.NetComputers)
                clients.AddRange(com._clients.Select(s => s._guid).ToArray());

            clients.Sort();

            long idx = 1;

            for (int i = 0; i < _list_spawn_point.Count; ++i)
            {
                foreach (var c in clients)
                {
                    if (idx > c)
                        continue;

                    if (c != idx)
                    {
                            break;
                    }
                    idx++;
                }
                _list_spawn_point[i]._inputs[0].text = idx.ToString();
                idx++;
            }
        }
        #endregion

        #region
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void UpdateHITLPosition()
        {
            if (MdaDataCenter.SelectedNetComputer == null)
                return;

            if (_dropdown_vehicle_type.value != 3 || _dropdown_spawn_type.value != 2)
                return;

            var info = MdaDataCenter.SelectedNetComputer._ports;

            var removelist = _list_spawn_point.Where(w => !info.ContainsKey(w._inputs[0].text.Replace("#", ""))).ToList();
            foreach (var r in removelist)
            {
                Destroy(r._obj);
                _list_spawn_point.Remove(r);
            }

            //foreach(var pair in info)
            //{
            //    var find = _list_spawn_point.Where(w => w._label.text.Replace("#", "") == pair.Key).FirstOrDefault();
            //    if (find == null)
            //        CreateSpawnPointForHITL(pair.Value.ToString());
            //}

        }

        #endregion

        #region [UI] [Dropdown] Vehicle Type 변경
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnDropdown_VehicleType(int value)
        {
            _input_spawn_name.text = _dropdown_vehicle_type.options[_dropdown_vehicle_type.value].text;

            _btn_select_model.interactable = (value >= 1 && value <= 4);

            var active = !(value == 3 || value == 4 || _dropdown_spawn_type.value == 2);
            _input_add_count.interactable = active;
            _btn_all_pc.interactable = active;

            foreach (var btn in _btns_unused_hitl)
                btn.interactable = value != 3;

            _dropdown_spawn_type.interactable = value != 4;

            if (value == 3)
            {
                OnDropdown_SpawnType(_dropdown_spawn_type.value);
                if (_dropdown_spawn_type.value == 2)
                {
                    UpdateHITLPosition();
                    foreach(var com in MdaDataCenter.NetComputers)
                    {
                        com._saved_spawn_points.Clear();
                        for(int i = 0; i < com._ports.Count; ++i)
                        {
                            com._saved_spawn_points.Add(com._ports.Values.ToArray()[i], Vector3.zero);
                        }
                    }
                    UpdateSpawnPointList();
                }
                else
                {
                    if (MdaDataCenter.SelectedNetComputer == null)
                        _input_add_count.text = "0";
                    else
                        _input_add_count.text = MdaDataCenter.SelectedNetComputer._ports.Count.ToString();
                }
            }
            else if(value == 4)
            {
                _spawntype_menus[0].SetActive(false);
                _spawntype_menus[1].SetActive(false);
                _spawntype_menus[2].SetActive(true);
            }
            else
            {
                OnDropdown_SpawnType(_dropdown_spawn_type.value);
                OnClick_CreateSpawnSetting();
                UpdateSpawnPointLabel();
            }
        }
        #endregion

        #region [UI] [Dropdown] Spawn Type 변경
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnDropdown_SpawnType(int value)
        {
            foreach(var menu in _spawntype_menus)
                menu.SetActive(false);

            var active = !(_dropdown_vehicle_type.value == 3);
            _input_add_count.interactable = active;
            _btn_all_pc.interactable = active;
            _dropdown_spawn_type.interactable = true;
            switch (value)
            {
                case 0:
                case 1:
                    _spawntype_menus[0].SetActive(true);
                    
                    break;
                case 2:
                    _spawntype_menus[1].SetActive(true);
                    _input_add_count.interactable = false;

                    if (_dropdown_vehicle_type.value == 3)
                    {
                        UpdateHITLPosition();
                        foreach (var com in MdaDataCenter.NetComputers)
                        {
                            com._saved_spawn_points.Clear();
                            for (int i = 0; i < com._ports.Count; ++i)
                            {
                                com._saved_spawn_points.Add(com._ports.Values.ToArray()[i], Vector3.zero);
                            }
                        }
                        UpdateSpawnPointList();
                    }
                    else
                        UpdateSpawnPointLabel();
                    _input_add_count.text = _list_spawn_point.Count.ToString();
                    //_btn_all_pc.interactable = false;
                    break;
                //case 3:
                //    _spawntype_menus[2].SetActive(true);
                //    _input_add_count.interactable = false;
                //    _btn_all_pc.interactable = false;
                //    break;
                default:
                    break;
            }
        }
        #endregion
        
        #region [UI] Computer 정보
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void UpdateComputerListUI()
        {
            var selected_com = MdaDataCenter.SelectedNetComputer;
            var selected_com_guid = (selected_com != null) ? selected_com._guid : 0;
            
            ListItem_clear(_listitem_computer_root);
            ListItem_clear(_listitem_client_root);
            foreach (var net_pc in MdaDataCenter.NetComputers)
            {
                var item = ListItem_add<MdaListItem_Computer>(_listitem_computer_prefab, _listitem_computer_root);
                item.SetData(net_pc);

                if (selected_com_guid == 0) // 첫번째 선택
                {
                    selected_com = net_pc;
                    selected_com_guid = net_pc._guid;
                    item.OnClick();
                }
                else if (selected_com_guid == net_pc._guid) // 기존 선택된 객체
                {
                    selected_com = net_pc;
                    item.OnClick();
                }
            }
			
			UpdateClientListUI(selected_com);
            UpdateHITLPosition();
        }
        #endregion
        
        #region [UI] Client 정보
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void UpdateClientListUI(MdNetData_Computer net_pc)
        {
            var selected_client = MdaDataCenter.SelectedNetClient;
            var selected_client_guid = (selected_client != null) ? selected_client._guid : 0;
            
            ListItem_clear(_listitem_client_root);
			if(net_pc != null)
			{
				foreach (var net_client in net_pc._clients)
				{
					var item = ListItem_add<MdaListItem_Client>(_listitem_client_prefab, _listitem_client_root);
                    UpdateRenderingStatus(net_pc._guid, net_client._guid);
					item.SetData(net_client);

                    if (selected_client_guid == 0) // 첫번째 선택
                    {
                        selected_client = net_client;
                        selected_client_guid = net_client._guid;
                        item.OnClick();
                    }
                    else if (selected_client_guid == net_client._guid) // 기존 선택된 객체
                    {
                        MdaDataCenter.SelectedNetClient = selected_client = net_client;
                        item.OnClick();
                    }

                    if(net_client._state != eClientState.Spawn)
                    {
                        item._background.color *= Color.yellow;
                        item._selected_mark.GetComponent<Image>().color = Color.yellow;
                    }

                    //switch (net_client._state)
                    //{
                    //    case 1:
                    //        item._background.color *= Color.yellow;
                    //        item._selected_mark.GetComponent<Image>().color = Color.yellow;
                    //        break;
                    //    case 2:
                    //    case 3:
                    //        item._background.color *= Color.red;
                    //        //item._selected_mark.SetActive(true);
                    //        item._selected_mark.GetComponent<Image>().color = Color.red;
                    //        //item._selected_mark.SetActive(false);
                    //        break;
                    //    default:
                    //        break;
                    //}
                }
			}
            
            for (int i = 0; i < _capture_buttons.Length; ++i)
                _capture_buttons[i].SetActive(0 == i);
        }
        #endregion

        #region [Monitor] CreateMonitorItem
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CreateMonitorItem()
        {
            if (!_tab_toggles[3].isOn)
                return;
            ListItem_clear(_monitor_item_root);
            foreach(var com in MdaDataCenter.NetComputers)
            {
                foreach(var cl in com._clients)
                    AddMonitorItem((int)cl._guid, cl.Pos, cl.Smoothness, com._ip_address);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddMonitorItem(int id, Vector3 pos, float sms, string ip)
        {
            var item = ListItem_add<MdaListItem_Monitor>(_monitor_item_prefab, _monitor_item_root);
            item.Reset(id, pos, sms, ip);
            _monitor_item_list.Add(item);
        }
        #endregion

        #region [Monitor] PosOn/Off
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_Monitor_Pos(bool active)
        {
            if (active)
            {
                MdaDataCenter.NetConnector.RegisterMessageHandler((int)ePacketProtocol.SA_POS, (sender, reader) =>
                {
                    var id = reader.ReadInt64();
                    var x = reader.ReadSingle();
                    var y = reader.ReadSingle();
                    var z = reader.ReadSingle();

                    foreach (var com in MdaDataCenter.NetComputers)
                    {
                        var find = com._clients.Where(w => w._guid == id).FirstOrDefault();
                        if (find != null)
                            find.Pos = new Vector3(x, y, z);

                        var item =_monitor_item_list.Where(w => w._id == (int)id).FirstOrDefault();
                        if (item != null)
                            item.SetPosition(find.Pos);
                    }
                });
                MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_MONITOR_START_POS);
                
            }
            else
            {
                MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_MONITOR_STOP_POS);
                //MdaDataCenter.NetConnector.RegisterMessageHandler
            }

        }
        #endregion

        #region [Monitor] SavePos
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_SaveMonitorPos()
        {
            FileBrowser.SaveFilePanel("Monitor save", "Monitor Save", Environment.CurrentDirectory, "monitoringpos.json", new string[] { "json", }, null, (bool canceled, string filePath) =>
            {
                if (canceled)
                    return;

                var obj = new Dictionary<string, Dictionary<int, string>>();

                foreach(var monitor in _monitor_item_list)
                {
                    if (!obj.ContainsKey(monitor._ip))
                        obj.Add(monitor._ip, new Dictionary<int, string>());

                    obj[monitor._ip].Add(monitor._id, monitor._pos.ToString());
                }

                if (!filePath.EndsWith(".json"))
                    filePath += ".json";

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                File.WriteAllText(filePath, json);
            });
        }
        #endregion

        #region [Monitor] Sms On/Off
        public void OnClick_Monitor_Sms(bool active)
        {
            if (active)
            {
                MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_MONITOR_START_SMS, 0f);
            }
            else
            {
                MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_MONITOR_STOP_SMS);
            }
        }
        #endregion

        #region [Monitor] SavePos
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_SaveMonitorSms()
        {
            FileBrowser.SaveFilePanel("Smoothness save", "Smoothness Save", Environment.CurrentDirectory, "smoothness.csv", new string[] { "csv", }, null, (bool canceled, string filePath) =>
            {
                if (canceled)
                    return;

                if (!filePath.EndsWith(".csv"))
                    filePath += ".csv";

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("ID,Smoothness");
                foreach (var monitor in _monitor_item_list)
                    sb.AppendLine(string.Format("{0},{1}", monitor._id, monitor._sms));

                Stream file = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(file, Encoding.UTF8);
                sw.WriteLine(sb);
                sw.Close();  
            });
        }
        #endregion

        #region [Popup] Debug Message
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ShowPopupMessage(int type, params string[] args)
        {
            if (_popup_root.activeSelf)
                return;

            _popup_root.SetActive(true);
            string msg = "";
            switch(type)
            {
                case 0:
                    msg = "PC가 선택되지 않았습니다.";
                    break;
                case 1:
                    msg = "연결된 HITL 모듈이 없습니다.";
                    break;
                case 2:
                    msg = "ID가 입력되지 않은 Port가 존재합니다.";
                    break;
                case 3:
                    msg = "SpawnPoint가 존재하지 않습니다.";
                    break;
                case 4:
                    msg = "현재 설정에서는 할 수 없습니다.";
                    break;
                case 5:
                    msg = "이미 사용중인 ID입니다.";
                    break;
                case 6:
                    msg = "확장자가 올바르지 않습니다.";
                    break;
            }

            _popup_text.text = msg;
            foreach(var p in args)
            {
                _popup_text.text += "\n" + p;
            }
        }

        #endregion



        #region MdaWindow_Main_Client.cs -> Partial class debug 문제로 인해 이동

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // NestedClass
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [NestedClass] CaptureImage
        public class CaptureImage
        {
            private byte[] _capture_buffer;
            private Texture2D _capture_texture;
            public RawImage _capture_screen;


            #region [CaptureBuffer] Get
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public byte[] GetCaptureBuffer(int size)
            {
                if ((null == _capture_buffer) || (_capture_buffer.Length != size))
                    _capture_buffer = new byte[size];
                return _capture_buffer;
            }
            #endregion

            #region [CaptureBuffer] LoadCaptureBuffer
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public void LoadCaptureBuffer(BinaryReader reader, int width, int height, int img_size)
            {
                var buf = GetCaptureBuffer(img_size);
                reader.Read(buf, 0, img_size);

                if ((null == _capture_texture) && (_capture_screen != null))
                    _capture_screen.texture = _capture_texture = new Texture2D(width, height, TextureFormat.RGB24, false);
                if (_capture_texture != null)
                    _capture_texture.LoadImage(buf);
            }
            #endregion
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++        

        #region [변수] Tab
        public Toggle[] _tab_toggles = new Toggle[(int)eTab.Max];
        public GameObject[] _tab_roots = new GameObject[(int)eTab.Max];
        #endregion

        #region [변수] Setting
        public TMP_InputField _input_setting;
        public Toggle[] _toggles;

        public Toggle _filemode;
        #endregion

        #region [변수] ImageCapture
        public GameObject[] _capture_buttons;
        public CaptureImage[] CaptureImages = new CaptureImage[(int)eCaptureImageType.Count];
        public RawImage[] CaptureScreens = new RawImage[3];
        #endregion

        #region [변수] Script (Python)
        public InputField _input_script_path;
        public TMP_InputField _input_script;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 탭
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [탭] 변경
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnTab_Menu(bool isOn)
        {
            if (!isOn)
                return;

            for (int i = 0; i < _tab_roots.Length; ++i)
                _tab_roots[i].SetActive(_tab_toggles[i].isOn);
        }
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. Client Setting File
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [SettingFile] [버튼] Load
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void LoadSettingFromServer()
        {
            if (_filemode.isOn)
                return;

            if (null == MdaDataCenter.SelectedNetClient)
                return;
            var com_guid = MdaDataCenter.SelectedNetComputer._guid;
            var client_guid = MdaDataCenter.SelectedNetClient._guid;

            MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_CLIENT_SETTING_LOAD, com_guid, client_guid);
            MdaDataCenter.NetConnector.RegisterMessageHandler((int)ePacketProtocol.SA_CLIENT_SETTING_LOAD, (peer, reader) =>
            {
                var data_length = reader.ReadInt32();
                var setting_data = new byte[data_length];
                reader.Read(setting_data, 0, data_length);

                var str = Encoding.Default.GetString(setting_data);
                _input_setting.text = str;
            });
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_LoadSettingFromFile()
        {
            FileBrowser.OpenFilePanel("Select a json to load", Environment.CurrentDirectory, new string[] { "json", }, null, (bool canceled, string filePath) =>
            {
                if (canceled)
                    return;
                _input_setting.text = File.ReadAllText(filePath);
            });
        }
        #endregion

        #region [SettingFile] [버튼] Send
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_SendSettingFile(bool everyone)
        {
            var setting_data = Encoding.Default.GetBytes(_input_setting.text);

            if (everyone)
            {
                foreach (var net_com in MdaDataCenter.NetComputers)
                {
                    SendSettingFile(net_com._guid, 0, setting_data);
                }
            }
            else
            {
                if (null == MdaDataCenter.SelectedNetClient)
                    return;
                var com_guid = MdaDataCenter.SelectedNetComputer._guid;
                var client_guid = MdaDataCenter.SelectedNetClient._guid;

                SendSettingFile(com_guid, client_guid, setting_data);


            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SendSettingFile(long com_guid, long client_guid, byte[] setting_data)
        {
            MdaDataCenter.NetConnector.SendMessage(JNetMessage.Make((int)ePacketProtocol.AS_CLIENT_SETTING_SAVE, (writer) =>
            {
                writer.Write(com_guid);
                writer.Write(client_guid);
                writer.Write(setting_data.Length);
                writer.Write(setting_data, 0, setting_data.Length);
            }).SetCapacity(64 * 1024));

            MdaDataCenter.NetConnector.RegisterMessageHandler((int)ePacketProtocol.SA_CLIENT_SETTING_SAVE, (peer, reader) =>
            {
                var result = reader.ReadString();
            });
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 2. 이미지캡처
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [이미지캡처] 시작/종료
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_StartImageCapture(int capture)
        {
            if (null == MdaDataCenter.SelectedNetClient)
                return;
            var com_guid = MdaDataCenter.SelectedNetComputer._guid;
            var client_guid = MdaDataCenter.SelectedNetClient._guid;
            var setting_data = Encoding.Default.GetBytes(_input_setting.text);

            for (int i = 0; i < _capture_buttons.Length; ++i)
                _capture_buttons[i].SetActive(capture == i);


            MdaDataCenter.NetConnector.SendMessage(JNetMessage.Make((int)ePacketProtocol.AS_REQ_CLIENT_CAPTURE_IMAGE, (writer) =>
            {
                writer.Write(com_guid);
                writer.Write(client_guid);
                writer.Write(capture);
            }));

            MdaDataCenter.NetConnector.RegisterMessageHandler((int)ePacketProtocol.SA_REQ_CLIENT_RESULT_IMAGE, (peer, reader) =>
            {
                var image_type = reader.ReadInt32();
                client_guid = reader.ReadInt64();
                var state = reader.ReadInt32();
                var width = reader.ReadInt32();
                var height = reader.ReadInt32();
                var size = reader.ReadInt32();
                if (size <= 0)
                    return;

                var capture_image = CaptureImages[image_type];
                if (null == capture_image)
                    capture_image = CaptureImages[image_type] = new CaptureImage();

                // 최대 3개 스크린만 사용
                if (null == capture_image._capture_screen)
                {
                    for (int i = 0; i < CaptureScreens.Length; ++i)
                    {
                        if (CaptureScreens[i] == null)
                            continue;
                        capture_image._capture_screen = CaptureScreens[i];
                        CaptureScreens[i] = null;
                        break;
                    }
                }

                capture_image.LoadCaptureBuffer(reader, width, height, size);
            });
        }
        #endregion

        #region [렌더링] On/Off
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_Rendering(int render_option)
        {
            var select_type = Array.IndexOf(_toggles, _toggles.Where(w => w.isOn).FirstOrDefault());

            switch (select_type)
            {
                case 0:
                    if (null == MdaDataCenter.SelectedNetClient)
                        return;
                    StartCoroutine(SetRendering(MdaDataCenter.SelectedNetComputer, MdaDataCenter.SelectedNetClient, render_option));
                    break;
                case 1:
                    if (null == MdaDataCenter.SelectedNetComputer)
                        return;
                    StartCoroutine(SetRendering(MdaDataCenter.SelectedNetComputer._clients, render_option));
                    break;
                case 2:
                    StartCoroutine(SetRendering(MdaDataCenter.NetComputers, render_option));
                    break;

            }

        }

        public IEnumerator SetRendering(List<MdNetData_Computer> computers, int render_option)
        {
            foreach (var computer in computers)
                foreach (var client in computer._clients)
                    yield return SetRendering(computer, client, render_option);
        }

        public IEnumerator SetRendering(List<MdNetData_Client> clients, int render_option)
        {
            foreach (var client in clients)
            {
                yield return SetRendering(MdaDataCenter.SelectedNetComputer, client, render_option);
                yield return new WaitForSeconds(0.1f);
            }
        }

        public IEnumerator SetRendering(MdNetData_Computer computer, MdNetData_Client client, int render_option)
        {
            var com_guid = computer._guid;
            var client_guid = client._guid;
            bool complate = false;
            MdaDataCenter.NetConnector.SendMessage(JNetMessage.Make((int)ePacketProtocol.AS_CLIENT_RENDERING, (writer) =>
            {
                writer.Write(com_guid);
                writer.Write(client_guid);
                writer.Write(render_option);
                complate = true;
            }));

            while (!complate)
                yield return null;

            UpdateRenderingStatus(com_guid, client_guid);
        }
        #endregion


        #region [UI] 렌더링상태정보
        public void UpdateRenderingStatus(long com_guid, long client_guid)
        {
            MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_CLIENT_RENDERING_INFO, com_guid, client_guid);
            MdaDataCenter.NetConnector.RegisterMessageHandler((int)ePacketProtocol.SA_REQ_CLIENT_IS_RENDERING, (peer, reader) =>
            {
                var recv_com_guid = reader.ReadInt64();
                long recv_client_guid = reader.ReadInt64();
                JLogger.Write(recv_client_guid.ToString());
                var is_rendering = reader.ReadInt32();
                var is_lidar = reader.ReadInt32();

                MdaListItem_Client item = null;

                var items = gameObject.GetComponentsInChildren<MdaListItem_Client>();
                foreach (var i in items)
                {
                    if (i.NetData._guid == recv_client_guid)
                        item = i;
                }

                if (item != null)
                {
                    item.SetRenderingMark(is_rendering == 1);
                    item.SetLidarMark(is_lidar == 3);
                }
            });
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 3. Python Script
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Python] [버튼] Load
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_LoadPythonFile()
        {
            if (null == MdaDataCenter.SelectedNetClient)
                return;
            var com_guid = MdaDataCenter.SelectedNetComputer._guid;
            var client_guid = MdaDataCenter.SelectedNetClient._guid;

            FileBrowser.OpenFilePanel("Select a python to load", Environment.CurrentDirectory, new string[] { "py", }, null, (bool canceled, string filePath) =>
            {
                if (canceled)
                    return;
                _input_script.text = File.ReadAllText(filePath);
                _input_script_path.text = Path.GetFileName(filePath);
            });
        }
        #endregion

        #region [Python] [버튼] Send
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_SendPythonFile(int client_type)
        {
            var script_data = Encoding.Default.GetBytes(_input_script.text);

            if (client_type == 1) // Single
            {
                if (null == MdaDataCenter.SelectedNetClient)
                    return;
                var com_guid = MdaDataCenter.SelectedNetComputer._guid;
                var client_guid = MdaDataCenter.SelectedNetClient._guid;

                SendPythonFile(com_guid, client_guid, script_data);
            }
            else if (client_type == 2) // 1 PC (all clients)
            {
                if (null == MdaDataCenter.SelectedNetComputer)
                    return;
                var net_com = MdaDataCenter.SelectedNetComputer;
                foreach (var net_client in net_com._clients)
                    SendPythonFile(net_com._guid, net_client._guid, script_data);
            }
            else if (client_type == 3) // ALL PC (all clients)
            {
                foreach (var net_com in MdaDataCenter.NetComputers)
                {
                    foreach (var net_client in net_com._clients)
                        SendPythonFile(net_com._guid, net_client._guid, script_data);
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SendPythonFile(long com_guid, long client_guid, byte[] script_data)
        {
            if (null == MdaDataCenter.SelectedNetClient)
                return;

            MdaDataCenter.NetConnector.SendMessage(JNetMessage.Make((int)ePacketProtocol.AS_CLIENT_PYTHON_SEND, (writer) =>
            {
                writer.Write(com_guid);
                writer.Write(client_guid);
                writer.Write(_input_script_path.text);
                writer.Write(script_data.Length);
                writer.Write(script_data, 0, script_data.Length);
            }).SetCapacity(1024 * 1024));

            MdaDataCenter.NetConnector.RegisterMessageHandler((int)ePacketProtocol.SA_CLIENT_PYTHON_SEND, (peer, reader) =>
            {
                var result = reader.ReadString();
            });
        }
        #endregion

        #region [Console]

        public void UpdateComportList()
        {
            if (_comports_group.transform.childCount > 2)
                for (int i = 2; i < _comports_group.transform.childCount; ++i)
                    Destroy(_comports_group.transform.GetChild(i).gameObject);

            foreach(var port in MdaListItem_Computer.Selected.NetData._ports)
            {
                GameObject copy = Instantiate(_comports_group.transform.GetChild(0).gameObject, _comports_group.transform);
                copy.name = port.Value.ToString();
                copy.GetComponentInChildren<Text>().text = port.Value.ToString();
            }
        }

        public void RebootPX4()
        {
            var temp = _console_command.text;
            _console_command.text = "reboot";
            SendConsoleCommand();
            _console_command.text = temp;
        }

        public void SendConsoleCommand()
        {
            var cmd = _console_command.text;

            if (_comports_group.transform.GetChild(0).GetComponent<Toggle>().isOn)
            {
                foreach(var com in MdaDataCenter.NetComputers)
                    foreach(var id in com._ports)
                        MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_PX4_CMD, com._guid, id.Value, id.Key, cmd);
            }
            else if (_comports_group.transform.GetChild(1).GetComponent<Toggle>().isOn)
            {
                foreach(var id in MdaDataCenter.SelectedNetComputer._ports)
                    MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_PX4_CMD, MdaDataCenter.SelectedNetComputer._guid, id.Value, id.Key, cmd);
            }
            else
            {
                for (int i = 2; i < _comports_group.transform.childCount; ++i)
                {
                    var tgls = _comports_group.transform.GetChild(i);
                    if (!tgls.GetComponent<Toggle>().isOn)
                        continue;
                    var port = MdaDataCenter.SelectedNetComputer._ports.Where(w => w.Value == int.Parse(tgls.name)).FirstOrDefault();
                    MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_PX4_CMD, MdaDataCenter.SelectedNetComputer._guid, port.Value, port.Key, cmd);
                }
            }

            _console_command.text = "";
        }

        public void OnClick_DroneModel(int type)
        {
            _selected_drone_model = type;
        }

        public void OnClick_OrganizeWindows()
        {
            MdaDataCenter.NetConnector.SendMessage((int)ePacketProtocol.AS_ORGANIZE_WINDOW);
        }
        #endregion

        #endregion






    }

}
