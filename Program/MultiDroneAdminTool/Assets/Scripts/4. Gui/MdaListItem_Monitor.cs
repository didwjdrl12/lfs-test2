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

    public class MdaListItem_Monitor : JListItem_base
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] NetData

        #endregion

        #region [변수] UI
        public int _id;
        public Text _txt_id;
        public InputField _input_x;
        public InputField _input_y;
        public InputField _input_z;
        public InputField _input_smoothness;
        [HideInInspector]
        public Vector3 _pos;
        [HideInInspector]
        public string _ip;
        [HideInInspector]
        public float _sms;
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메인 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [초기화] Reset
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Reset(int id)
        {
            _txt_id.text = id.ToString();
            _input_x.text = "";
            _input_y.text = "";
            _input_z.text = "";
            _input_smoothness.text = "";
            _id = id;
            _pos = Vector3.zero;
            _ip = "";
            _sms = 0f;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Reset(int id, Vector3 pos, float sms, string ip)
        {
            Reset(id);
            SetPosition(pos);
            CheckStopSmoothness(sms);
            _ip = ip;
        }
        #endregion


        #region [초기화] Data
        //------------------------------------------------------------------------------------------------------------------------------------------------------    
        internal void SetData()
        {
            

        }
        #endregion

        #region [UI] Update
        //------------------------------------------------------------------------------------------------------------------------------------------------------    
        internal void UpdateUI()
        {

        }
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메인 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Button] 클릭
        //------------------------------------------------------------------------------------------------------------------------------------------------------    
        public void OnClick()
        {

        }
        #endregion

        #region [Position] Set
        //------------------------------------------------------------------------------------------------------------------------------------------------------    
        public void SetPosition(float x, float y, float z)
        {
            
            _pos = new Vector3(Mathf.Round(x), Mathf.Round(y), Mathf.Round(z));
            _input_x.text = _pos.x.ToString();
            _input_y.text = _pos.y.ToString();
            _input_z.text = _pos.z.ToString();
            
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------    
        public void SetPosition(Vector3 pos)
        {
            SetPosition(pos.x, pos.y, pos.z);
        }
        #endregion

        #region [Smoothness] Check
        //------------------------------------------------------------------------------------------------------------------------------------------------------    
        public void CheckStartSmoothness()
        {
            _input_smoothness.text = "...";
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------    
        public void CheckStopSmoothness(float smoothness)
        {
            _input_smoothness.text = smoothness.ToString();
            _sms = smoothness;
        }
        #endregion
    }


}
