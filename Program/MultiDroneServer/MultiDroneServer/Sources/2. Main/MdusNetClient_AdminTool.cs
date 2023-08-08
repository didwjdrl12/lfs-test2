using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using J2y.Network;

namespace J2y.MultiDroneUnity
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MdusNetClient_AdminTool
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class MdusNetClient_AdminTool : JObject
    {
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

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [변수] Base
        public NetPeer_base NetPeer { get; set; }
        public MdNetData_AdminTool NetData { get; set; }
        public byte[][] CaptureBuffer { get; set; }
        public bool ActiveMonitor { get; set; } = false;
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

            CaptureBuffer = new byte[(int)eCaptureImageType.Count][];
            NetData = new MdNetData_AdminTool();
            NetData._guid = _guid;
            MdusDataCenter.NetClient_AdminTools.Add(this);
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
            MdusDataCenter.NetClient_AdminTools.Remove(this);
        }
        #endregion


        #region [CaptureBuffer] Get
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public byte[] GetCaptureBuffer(eCaptureImageType image_type, int size)
        {
            var cti = (int)image_type;
            if ((null == CaptureBuffer[cti]) || (CaptureBuffer[cti].Length != size))
                CaptureBuffer[cti] = new byte[size];
            return CaptureBuffer[cti];
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
