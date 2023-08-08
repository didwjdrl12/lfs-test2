using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace J2y.MultiDrone
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MdaBase
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public partial class MdaBase
    {
        public static string _launcher_server_ip = "127.0.0.1";
        public static int _launcher_server_port = 46711;

        public static bool _temp_minimum_version = false;


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

        #endregion

        #region [AdminTool]

        AS_CLIENT_ADD = 80,
        AS_CLIENT_KILL = 81,
        AS_CLIENT_KILL_ALL = 82,
        AS_CLIENT_LIST = 83,
        SA_CLIENT_LIST = 84,

        AS_CLIENT_SETTING_LOAD = 85,
        SA_CLIENT_SETTING_LOAD = 86,
        AS_CLIENT_SETTING_SAVE = 87,
        SA_CLIENT_SETTING_SAVE = 88,

        AS_CLIENT_RENDERING = 89,
        AS_CLIENT_RENDERING_INFO = 90,
        SA_REQ_CLIENT_IS_RENDERING = 91,

        AS_REQ_CLIENT_CAPTURE_IMAGE = 92,
        SA_REQ_CLIENT_RESULT_IMAGE = 93,

        AS_CLIENT_PYTHON_SEND = 94,
        SA_CLIENT_PYTHON_SEND = 95,

        AS_PX4_KILL = 96,

        SA_HITL_PORTS = 97,
        AS_REQ_PORTS = 98,

        SA_SPAWN_ERROR = 99,

        AS_MONITOR_START_POS = 100,
        AS_MONITOR_STOP_POS = 101,
        SA_POS = 102,

        AS_MONITOR_START_SMS = 103,
        AS_MONITOR_STOP_SMS = 104,
        SA_SMS = 105,

        AS_PX4_CMD = 106,
        SA_PX4_OUTPUT = 107,

        AS_ORGANIZE_WINDOW = 108,

        SA_USAGE = 109,
        #endregion

        #region [KML]
        AS_KML = 110,
        SC_KML_UPDATED = 111,
        CS_CUSTOM_STRING = 112,
        SC_KML_REMOVEALL = 113,
        #endregion

        SC_MODEL_DATA = 114,
        SC_MODEL_DATA_BEZIER = 115
    };

    public enum eClientType
    {
        Client, Launcher, AdminTool, Max
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
