using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using J2y.Network;

namespace J2y.MultiDroneUnity
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MdusNetClient_User
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class MdusNetClient_User : JObject
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [변수] Base
        public NetPeer_base NetPeer { get; set; }
        public MdNetData_User NetData { get; set; }
        public MdusNetClient_Launcher Launcher { get; set; }
		public bool _is_spawned = false;
		#endregion



		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 메인 함수
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [초기화] 생성자
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public override void Awake()
        {
            base.Awake();

            //NetData = new MdNetData_User();
            //NetData._guid = JUtil.CreateUniqueId();
            //MdusDataCenter.NetClient_Users.Add(this);

            //_movement = new JSyncMovement(null, JSyncMovement.eInterpolationType.Time, JSyncMovement.eRotationType.None);
        }
        #endregion

        //#region [업데이트] Update    
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public void FixedUpdate()
        //{
        //    _movement.Update(true);
        //}
        //#endregion


        #region [종료] 삭제
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void OnDestroy()
        {
            if(NetData._guid < 0 || NetData._guid > 255)
                return;

            MdusNetMessageDispatcher.BroadcastUsers(MdusNetServer.Instance, JNetMessage.Make((int)ePacketProtocol.NOTI_USER_LEAVE, (writer) =>
            {
                NetData.Write(writer);
            }), this);

            MdusDataCenter.NetClient_Users[NetData._guid-1] = null;
        }
        #endregion





        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 네트워크
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [네트워크] 전송
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override eNetSendResult NetSendMessage(JNetMessage message)
        {
            return NetPeer.SendMessage(message);
        }
        public override void NetBroadcast(JNetMessage message, NetPeer_base except = null)
        {
            NetPeer.Broadcast(message);
        }
        #endregion



    }

}
