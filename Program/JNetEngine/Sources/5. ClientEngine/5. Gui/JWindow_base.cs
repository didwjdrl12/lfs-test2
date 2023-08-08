#if !NET_SERVER
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JWindow_base
    //
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JWindow_base : JGui_base
    {
         //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] 버튼
        private bool _overlap_button_check;
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 기본 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [창열기]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Open()
        {
            gameObject.SetActive(true);
            //if (_openAnimEnabled)
            //    JSoundManager.Instance.PlayEffect("Window_open5");
            //else
            //    JSoundManager.Instance.PlayEffect("Window_open3");
        }
        #endregion

        #region [창닫기]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Close()
        {
            gameObject.SetActive(false);
            //JSoundManager.Instance.PlayEffect("Sfx_Button_Touch");
        }
        #endregion

        #region [버튼] 중복 클릭 방지
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual IEnumerator OverlapButtonCheck(bool IsDisactiveButtons = false, float time = 3.0f)
        {
            _overlap_button_check = !_overlap_button_check;

            if (!_overlap_button_check)
            {
                _overlap_button_check = true;
                yield return null;
            }

            if (IsDisactiveButtons)
            {
                var buttons = gameObject.GetComponentsInChildren<Button>();
                foreach (var button in buttons)
                    button.interactable = false;

                yield return new WaitForSeconds(time);

                foreach (var button in buttons)
                    button.interactable = true;
            }
            else
                yield return new WaitForSeconds(time);

            _overlap_button_check = false;
        }
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 리스트
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [리스트] 초기화
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ListItem_clear(Transform parentTM)
        {
            JUtil.DestroyAllChildren(parentTM);
        }
        #endregion

        #region [리스트] 항목 추가
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T ListItem_add<T>(GameObject prefab, Transform parentTM, bool resetTM = true) where T : JListItem_base
        {
            var list_item_go = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            var list_item = list_item_go.GetComponent<T>();
            list_item_go.transform.SetParent(parentTM);
            if (resetTM)
            {
                list_item_go.transform.localPosition = Vector3.zero;
                list_item_go.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            return list_item;
        }
        #endregion

        #region [리스트] 항목 순회
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static IEnumerable<T> GetListItems<T>(Transform parentTM) where T : JListItem_base
        {
            for (int i = 0; i < parentTM.childCount; ++i)
            {
                var child = parentTM.GetChild(i);
                var obj = child.GetComponent<T>();
                if (obj != null)
                    yield return obj;
            }
        }
        #endregion

        #region [리스트] 항목 삭제
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ListItem_Remove<T>(Transform parentTM, Func<T, bool> pred) where T : JListItem_base
        {
            for(int i = parentTM.childCount - 1; i >= 0; --i)
            {
                var child = parentTM.GetChild(i);
                var obj = child.GetComponent<T>();
                if((obj != null) && pred(obj))
                    GameObject.Destroy(obj.gameObject);
            }
        }
        #endregion


    }

}
#endif
