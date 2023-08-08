using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using J2y.Network;

namespace J2y.MultiDroneUnity
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MdusDataCenter
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static class MdusDataCenter
    {
        public static int MAX_USER = 255;
        public static int s_map = -1;

        public class MdusClientInfo
        {
            public long _computer_guid;
            public long _client_guid;

            public MdusClientInfo(long com_guid, long client_guid)
            {
                _computer_guid = com_guid;
                _client_guid = client_guid;
            }
        }
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] NetClient

        public static List<MdusNetClient_AdminTool> NetClient_AdminTools { get; set; }
        public static List<MdusNetClient_Launcher> NetClient_Launchers { get; set; }
        public static MdusNetClient_User[] NetClient_Users { get; set; }
        #endregion

        #region [변수] 렌더링상태 저장
        public static List<MdusClientInfo> _rendering_info = new List<MdusClientInfo>();
        #endregion

        #region [변수] 라이다상태 저장
        public static List<MdusClientInfo> _lidar_info = new List<MdusClientInfo>();
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메인 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        static MdusDataCenter()
        {
            NetClient_AdminTools = new List<MdusNetClient_AdminTool>();
            NetClient_Launchers = new List<MdusNetClient_Launcher>();

            NetClient_Users = new MdusNetClient_User[MAX_USER];
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // PC
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [PC] 찾기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MdusNetClient_Launcher FindComputer(long guid)
        {
            return NetClient_Launchers.FirstOrDefault(l => l.NetData._guid == guid);
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 유저
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [유저] 찾기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int FindEmptyIndex()
        {
            int count = 0;
            for(count = 0; count < MAX_USER; ++count)
                if (NetClient_Users[count] == null)
                    break;

            return count;
        }

        public static MdusNetClient_User FindUser(long guid)
        {
            return NetClient_Users.Where(w=>w != null).FirstOrDefault(l =>l.NetData._guid == guid );
        }
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Launcher
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Launcher] 찾기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MdusNetClient_Launcher FindLauncher(long guid)
        {
            return NetClient_Launchers.FirstOrDefault(l => l.NetData._guid == guid);
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // AdminTool
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [AdminTool] 찾기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MdusNetClient_AdminTool FindAdminTool(long guid)
        {
            return NetClient_AdminTools.FirstOrDefault(l => l.NetData._guid == guid);
        }
        #endregion

    }

}
