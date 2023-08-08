#if !NET_SERVER
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;



namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JPopup_MessageBox
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JPopup_MessageBox : JWindow_base
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] 설정
        public string _sound_button = "Sfx_Button_Touch";
        #endregion


        #region [변수] 타입
        private eMessageBoxType _type;
        #endregion

        #region [변수] [UI] 버튼/메시지
        public Text _message;
        public GameObject _button_yes;
        public GameObject _button_no;
        public GameObject _button_ok;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메인 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
              
        #region [속성] 윈도우 타입
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetType(eMessageBoxType type)
        {
            _type = type;

            _button_yes.SetActive(type == eMessageBoxType.YESNO);
            _button_no.SetActive(type == eMessageBoxType.YESNO);
            _button_ok.SetActive(type == eMessageBoxType.OK);
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 버튼 이벤트
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [버튼] OK
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_OK()
        {
            if(!string.IsNullOrEmpty(_sound_button))
                JSoundManager.Instance.PlayEffect(_sound_button);
            JWindowManager.Instance.CloseMessageBox(eMessageBoxButton.OK);
        }
        #endregion


        #region [버튼] yes
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_Yes()
        {
            if (!string.IsNullOrEmpty(_sound_button))
                JSoundManager.Instance.PlayEffect(_sound_button);
            JWindowManager.Instance.CloseMessageBox(eMessageBoxButton.YES);
        }
        #endregion

        #region [버튼] no
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnClick_No()
        {
            if (!string.IsNullOrEmpty(_sound_button))
                JSoundManager.Instance.PlayEffect(_sound_button);
            JWindowManager.Instance.CloseMessageBox(eMessageBoxButton.NO);
        }
        #endregion

    }


}


#endif
