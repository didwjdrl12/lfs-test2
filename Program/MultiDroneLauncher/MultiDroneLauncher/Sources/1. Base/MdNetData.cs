using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace J2y.MultiDrone
{
    #region [enum] SpawnAreaType
    public enum eSpawnAreaType
    {
        Area, Point, Sphere,
    }
    #endregion

    #region [enum] MapName
    public enum eMapName
    {
        CesiumEtri,
        Shintanjin,
        ETRI,
        ArenaBlocks,
        ArenaNeighborhood,
        Max,
    }
    #endregion

    #region [enum] ClientState
    public enum eClientState
    {
        None,
        Execute,
        Connect,
        Spawn,
        Playing,
    }
    #endregion


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // Computer (Launcher)
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class MdNetData_Computer : JNetData
    {
        #region [변수] Base
        public long _guid;
        public string _ip_address = "";
        public string _cpu_info = "";
        public string _memory_info = "";
        public Dictionary<string, int> _ports = new Dictionary<string, int>();
        #endregion

        #region [변수] Clients
        public List<MdNetData_Client> _clients = new List<MdNetData_Client>();
        #endregion





        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [IO 쓰레드] 메시지 패킹/파싱
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [IO 쓰레드] 메시지 패킹
        //----------------------------------------------------------------
        public override void Read(BinaryReader reader)
        {
            _guid = reader.ReadInt64();
            _ip_address = reader.ReadString();
            _cpu_info = reader.ReadString();
            _memory_info = reader.ReadString();
            ParsePorts(reader);
            ParseList(reader, _clients);
        }
        #endregion

        #region [IO 쓰레드] 메시지 파싱
        //----------------------------------------------------------------
        public override void Write(BinaryWriter writer)
        {
            //writer.Write(_guid);
            writer.Write(_ip_address);
            writer.Write(_cpu_info);
            writer.Write(_memory_info);
            PackingPorts(writer);
            PackingList(writer, _clients);
        }
        #endregion

        #region [Util] 포트목록 파싱 & 패킹
        //----------------------------------------------------------------
        public void ParsePorts(BinaryReader reader)
        {
            var json = reader.ReadString();
            _ports = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
        }
        //----------------------------------------------------------------
        public void PackingPorts(BinaryWriter writer)
        {
            var json = JsonConvert.SerializeObject(_ports);
            writer.Write(json);
        }
        #endregion

    }


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // Client (Unreal)
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class MdNetData_Client : JNetData
    {
        #region [변수] Config File
        public long _guid;
        public int _processId;
        public eMapName MapName = eMapName.ArenaNeighborhood;
        public string _config_filename = "";
        public string _vehicle_type = "Car";
        public int _script_server_port = 41451;
        public DateTime ConnectTime = DateTime.Now;
        public string VehicleVisibleName = "Car";
        #endregion

        #region [변수] Spawn Area
        public eSpawnAreaType SpawnAreaType = eSpawnAreaType.Area;
        public int SpawnAreaIndex = -1;
        public Vector3 SpawnPoint;
        public float SpawnSphereRadius = 100;
        #endregion

        public int _batch_processId;
        public eClientState _state = eClientState.None;


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [IO 쓰레드] 메시지 패킹/파싱
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [IO 쓰레드] 메시지 패킹
        //----------------------------------------------------------------
        public override void Read(BinaryReader reader)
        {
            _guid = reader.ReadInt64();
            _processId = reader.ReadInt32();
            MapName = (eMapName)reader.ReadInt32();
            _config_filename = reader.ReadString();
            _vehicle_type = reader.ReadString();
            _script_server_port = reader.ReadInt32();
            ConnectTime = new DateTime(reader.ReadInt64());
            VehicleVisibleName = reader.ReadString();
            SpawnAreaType = (eSpawnAreaType)reader.ReadInt32();
            SpawnAreaIndex = reader.ReadInt32();
            SpawnPoint.x = reader.ReadSingle();
            SpawnPoint.y = reader.ReadSingle();
            SpawnPoint.z = reader.ReadSingle();
            SpawnSphereRadius = reader.ReadSingle();
            _state = (eClientState)reader.ReadInt32();
        }
        #endregion

        #region [IO 쓰레드] 메시지 파싱
        //----------------------------------------------------------------
        public override void Write(BinaryWriter writer)
        {
            writer.Write(_guid);
            writer.Write(_processId);
            writer.Write((int)MapName);
            writer.Write(_config_filename);
            writer.Write(_vehicle_type);
            writer.Write(_script_server_port);
            writer.Write(ConnectTime.Ticks);
            writer.Write(VehicleVisibleName);
            writer.Write((int)SpawnAreaType);
            writer.Write(SpawnAreaIndex);
            writer.Write(SpawnPoint.x);
            writer.Write(SpawnPoint.y);
            writer.Write(SpawnPoint.z);
            writer.Write(SpawnSphereRadius);
            writer.Write((int)_state);
        }
        #endregion
    }




    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // Client Simulation Result
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class MdNetData_SimulResult : JNetData
    {
        #region [변수] Image Buffer
        public byte[] _rgb_image;
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [IO 쓰레드] 메시지 패킹/파싱
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [IO 쓰레드] 메시지 패킹
        //----------------------------------------------------------------
        public override void Read(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            _rgb_image = new byte[length];
            reader.Read(_rgb_image, 0, length);
        }
        #endregion

        #region [IO 쓰레드] 메시지 파싱
        //----------------------------------------------------------------
        public override void Write(BinaryWriter writer)
        {
            writer.Write(_rgb_image.Length);
            writer.Write(_rgb_image);
        }
        #endregion
    }

}
