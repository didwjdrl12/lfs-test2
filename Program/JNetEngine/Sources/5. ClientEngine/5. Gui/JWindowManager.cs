using UnityEngine;
using System.Collections.Generic;
using System;
#if !NET_SERVER
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;


namespace J2y
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JWindowManager
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JWindowManager : MonoBehaviour
    {
        public static JWindowManager Instance;

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] Serialize Fields
        public Image _fade_image;
        #endregion

        #region [변수] 윈도우
        protected Canvas _canvas_root;
        protected List<JWindow_base> _windows = new List<JWindow_base>();
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메인 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] Awake
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _canvas_root = GetComponent<Canvas>();
        }
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 윈도우 일반
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [윈도우] 열기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public T OpenWindow<T>() where T : JWindow_base
        {
            var win = FindWindow<T>();
            if (null == win)
                win = LoadWindow<T>();
            if (null != win)
            {
                win.gameObject.SetActive(true);
                win.transform.SetParent(_canvas_root.transform);
                win.GetComponent<RectTransform>().offsetMin = Vector3.zero;
                win.GetComponent<RectTransform>().offsetMax = Vector3.zero;
                win.GetComponent<RectTransform>().localScale = Vector3.one;
                //JUtil.ResetTransform(win.transform);			
            }
            return win;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool IsOpend<T>() where T : JWindow_base
        {
            var win = FindWindow<T>();
            if (null != win)
                return win.gameObject.activeSelf;
            return false;
        }
        #endregion

        #region [윈도우] 닫기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CloseWindow<T>(bool destroy = false) where T : JWindow_base
        {
            var win = FindWindow<T>();
            if (null != win)
            {
                if (destroy)
                {
                    _windows.Remove(win);
                    Destroy(win.gameObject);
                }
                else
                    win.gameObject.SetActive(false);
            }
        }
        #endregion

        #region [윈도우] 찾기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public T FindWindow<T>() where T : JWindow_base
        {
            foreach (var win in _windows)
            {
                if (win is T)
                    return win as T;
            }
            return null;
        }
        #endregion

        #region [윈도우] 모두 삭제/닫기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DestroyAllWindows()
        {
            var wins = new List<JWindow_base>();
            foreach (var win in _windows)            
                Destroy(win.gameObject);            
            _windows = wins;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CloseAllWindows()
        {
            foreach (var win in _windows)
                win.Close();
        }
        #endregion
        
        #region [윈도우] 프리팹 로딩
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual T LoadWindow<T>() where T : JWindow_base
        {
            var win_name = typeof(T).Name;
            GameObject gui_res = null;

            foreach (var path in GuiPaths)
            {
                var win_path = string.Format("{0}/{1}", path, win_name);
                gui_res = Resources.Load(win_path) as GameObject;
                //JAssetBundleManager.Instance.LoadAsset(path, win_name);
                if (gui_res != null)
                    break;
            }
            if (gui_res == null)
                return null;


            var win_go = (GameObject)Instantiate(gui_res);
            if (null == win_go)
                return null;
            var win = win_go.GetComponent<T>();
            if (win != null)
                _windows.Add(win);
            return win;
        }
        public virtual string[] GuiPaths { get { return new string[] { "GUI/Prefabs" }; } }
        #endregion


        #region [Fade] In/Out
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public void FadeOutIn()
        //{
        //	_fade_image.gameObject.SetActive(true);
        //	_fade_image.DOFade(1f, 1f);
        //	Invoke("FadeOut", 1f);
        //}
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void FadeOutIn(float duration, Action fun_next)
        {
            if (duration <= 0f)
            {
                fun_next();
                return;
            }
            StartCoroutine(corotuine_fadeOutIn(duration, fun_next));
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public IEnumerator corotuine_fadeOut(float duration)
        {
            FadeOut(duration);
            yield return new WaitForSeconds(duration);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public IEnumerator corotuine_fadeIn(float duration)
        {
            FadeIn(duration);
            yield return new WaitForSeconds(duration);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public IEnumerator corotuine_fadeOutIn(float duration, Action fun_next)
        {
            FadeOut(duration);
            yield return new WaitForSeconds(duration);

            fun_next();
            yield return new WaitForSeconds(1f);
            FadeIn(duration);
            yield return new WaitForSeconds(duration);

        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void FadeIn(float duration = 1f)
        {
            _fade_image.DOFade(0f, duration);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void FadeOut(float duration = 1f)
        {
            _fade_image.gameObject.SetActive(true);
            _fade_image.DOFade(1f, duration);
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메시지 박스
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [윈도우] 메시지 박스

        private JPopup_MessageBox _popup_message_box;

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ShowMessageBox(eMessageBoxType mbtype, string text, Action<eMessageBoxButton> fun_res = null)
        {
            var popup = _popup_message_box = OpenWindow<JPopup_MessageBox>();
            popup.gameObject.SetActive(true);
            popup.SetType(mbtype);
            popup._message.text = text;

            if (fun_res != null)            
                JEventHandler.RegisterEvent(popup.gameObject, "CloseWindow", fun_res);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CloseMessageBox(eMessageBoxButton btnType = eMessageBoxButton.OK)
        {
            //CloseWindow<KePopup_messageBox>();
            _popup_message_box.gameObject.SetActive(false);

            JEventHandler.ExecuteEvent(_popup_message_box.gameObject, "CloseWindow", btnType);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ShowErrorMessage(string text, Action<eMessageBoxButton> fun_res = null)
        {
            ShowMessageBox(eMessageBoxType.None, text, fun_res);
        }
        #endregion

        #region [윈도우] Result
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public void ShowResultWindow(string text, int candy)
        //{
        //	FadeOutIn(1f, () =>
        //	{
        //		var popup = OpenWindow<KePopup_result>();
        //		popup._message.text = text;
        //	});		
        //}
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 팝업
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    
    }

}

#endif
