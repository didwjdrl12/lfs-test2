using J2y.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JObject : 모든 관리되는 객체의 부모
    //      클라이언트는 MonoBehaviour를 상속
    //
    //      1. 생명주기
    //		    todo: Awake, Start, Update, OnDestroy 등 리플렉션을 이용하여 필요한 경우만 호출
    //      2. ObjectPool (GUID)
    //      3. [Replication] RPC(Function)
    //      4. [Replication] Actor
    //      5. Serialize
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

#if NET_SERVER
    public partial class JObject // : ICloneable
#else
    public partial class JObject : MonoBehaviour
#endif
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [상수] 기본 정보
        public const float _current_engine_version = 1.0f;
        #endregion

        #region [Variable] 1. 생명주기
#if NET_SERVER
        private bool _enabled;
        private bool _first_update = true;
        private bool _destroyed;
#endif
        #endregion

        #region [Variable] 2. Base
        [HideInInspector] public GameObject _gameObject;


        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] Path
        public static string _default_path;        // 기본 저장 경로 지정 
        public static string _default_extension;   // 기본 확장자 지정 

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [JSerializable] public virtual string _asset_name { get; set; }
        [JSerializable] public virtual string _asset_path { get; set; } = _default_path;
        #endregion


        #region [Property] JObject
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        protected string _log_tag_internal;
        public virtual string _log_tag
        {
            get
            {
                if (null == _log_tag_internal)
                    _log_tag_internal = GetType().Name;
                return _log_tag_internal;
            }
            set { _log_tag_internal = value; }
        }
        #endregion

        #region [Property] GUID
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [JSerializable]
        public virtual long _guid { get; set; } = JUtil.CreateUniqueId();
        #endregion

        #region [Property] Destroyed
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //public bool Destroyed
        //{
            //get { return _destroyed; }
            //set { _destroyed = value; }
        //}
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. 생명주기
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [생명주기] Awake/Start/OnDestroy/Update
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Awake() {
#if !NET_SERVER
            _gameObject = gameObject;
#endif
        }
        public virtual void Start() { }
        public virtual void OnDestroy() { }
        public virtual void Update() { }
        #endregion

        #region [서버] [생명주기] Internal
#if NET_SERVER
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void AwakeInternal()
        {
            Awake();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void UpdateInternal()
        {
            if (_first_update)
            {
                Start();
                _first_update = false;
            }
            Update();
        }
        public virtual void DestroyInternal()
        {
            if (_destroyed)
                return;
            Enabled = false;
            OnDestroy();
            _destroyed = true;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Destroy()
        {
            JObjectManager.Destroy(this);
        }
#else
        public virtual void AwakeInternal() { }
        public virtual void UpdateInternal() { }
        public virtual void DestroyInternal() { }
#endif
        #endregion

        #region [서버] [생명주기] Enabled
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool Enabled
        {
#if NET_SERVER
            get { return _enabled; }
            set
            {
                if (_enabled == value)
                    return;
                _enabled = value;
                if (_enabled)
                    OnEnable();
                else
                    OnDisable();
            }
#else
            get { return enabled; }
            set { enabled = value; }
#endif
        }

        public virtual void OnEnable() { }
        public virtual void OnDisable() { }

        #endregion

        #region [백업] [메인] 소멸
        //   //------------------------------------------------------------------------------------------------------------------------------------------------------
        //   ~JObject()
        //{
        //       // todo: 검토 필요(소멸자에서는 가상함수 호출이 안됨.. 이미 삭제 과정 중에 또다른 삭제가 발생..)
        //       if (!_destroyed)
        //           DestroyObject(this);
        //       Dispose(false);
        //   }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 2. ObjectPool
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [ObjectPool] Create/Destroy
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T CreateObject<T>() where T : JObject
        {
            return JObjectManager.Create<T>();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void DestroyObject(JObject obj)
        {
            JObjectManager.Destroy(obj);
        }
        #endregion

        #region [ObjectPool] Reset Guid
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ResetGuid(long new_guid)
        {
            JObjectManager.ResetGuid(this, new_guid);
        }
        #endregion
    }
}
