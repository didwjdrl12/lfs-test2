using J2y.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;




namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JComponent : JObject
    //
    //      2. Coroutine
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JComponent : JObject
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] 0. 기본정보
        public JActor _actor { get; set; }
        #endregion

        #region [Variable] 1. RPC

        #endregion

        #region [Variable] 2. Coroutines

        protected JCoroutines _coroutine;

        #endregion

        

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [Property] Base
        public long _actor_guid { get { return _actor._guid; } }
        #endregion

        #region [Property] Default Components
        public JActorTransform _transform { get { return _actor._transform; } }
        #endregion

        

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. 생명주기
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

#if NET_SERVER
        #region [생명주기] Internal Update & Destroy
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void UpdateInternal()
        {
            base.UpdateInternal();

            if (null != _coroutine)
                _coroutine.Update();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void DestroyInternal()
        {
            _coroutine = null;
            base.DestroyInternal();
        }
        #endregion
#endif

        #region [Event] OnAddComponent
        public virtual void OnAddComponent() {}
        #endregion

        

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 3. Coroutine
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

#if NET_SERVER
        #region [Coroutine] Start
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void StartCoroutine(IEnumerator routine)
        {
			if (null == _coroutine)
                _coroutine = new JCoroutines();
            _coroutine.Start(routine);
        }
        #endregion

        #region [Coroutine] StopAll
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void StopAllCoroutines()
        {
            if (null == _coroutine)
                return;
            _coroutine.StopAll();
        }
        #endregion
#endif

        #region [Coroutine] Pause
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static IEnumerator Pause(float time)
        {
			yield return JCoroutines.Pause(time);
        }
        #endregion


    }

}