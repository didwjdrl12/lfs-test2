using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace J2y.MultiDrone
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MdlBase
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public partial class MdlBase
    {
        public static string _launcher_server_ip = "127.0.0.1";
        public static int _launcher_server_port = 46711;
        public static bool _unreal_editor_mode = false;          // 언리얼 실행 후 런처를 실행해야 함(Shared Memory Open)


        // 클라이언트 실행파일
        public static string _path_root = ".";
        public static string _path_client { get { return _path_root + "\\DroneClientBin"; } }
        public static string _path_python_client { get { return _path_client + "\\PythonClient"; } }
        //public static string _filename_client = "test_client";
        public static string _filename_client = "blocks";
        public static string _filename_setting = "settings.json";

        // PX4
        public static string _path_px4_root { get { return _path_root + "\\PX4"; } }
        //public static string _path_px4_script { get { return _path_px4_root + "\\home\\Firmware\\Tools"; } }
        public static string _px4_batch = "run-console";
        //public static string _px4_sitl_script = "sitl_single_run_for_multiple.sh";

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 공통 설정
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Config
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region 설정 파일 읽기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void LoadConfig()
        {
            var path_root = System.Environment.CurrentDirectory;
            var d = new SimpleDictionary();
            d.Load(path_root + "\\ConfigBase.ini");

            _launcher_server_ip = d.Get("Server_IP");

        }
        #endregion

    }



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // enum
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

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

        #endregion
    };

    public enum eClientType
    {
        Client, Launcher, AdminTool, Max
    }

    public enum eVehicleType
    {
        Car, Multirotor, ComputerVision, Max,
    }

    public enum eSharedMemoryState 
    {
        ReadDone, Writing, WriteDone, Reading,
    }

    public enum eCaptureImageType 
    { //this indexes to array, -1 is special to indicate main camera
        Scene = 0, 
        DepthPlanner, 
        DepthPerspective,
        DepthVis, 
        DisparityNormalized,
        Segmentation,
        SurfaceNormals,
        Infrared,
        Count //must be last
    };

}
