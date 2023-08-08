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
    // JObject : ��� �����Ǵ� ��ü�� �θ�
    //      Ŭ���̾�Ʈ�� MonoBehaviour�� ���
    //
    //      1. �����ֱ�
    //		    todo: Awake, Start, Update, OnDestroy �� ���÷����� �̿��Ͽ� �ʿ��� ��츸 ȣ��
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
        
        #region [���] �⺻ ����
        public const float _current_engine_version = 1.0f;
        #endregion

        #region [Variable] 1. �����ֱ�
#if NET_SERVER
        private bool _enabled;
        private bool _first_update = true;
        private bool _destroyed;
#endif
        #endregion

        #region [Variable] 2. Base
        [HideInInspector] public GameObject _gameObject;


        #endregion Daejeon, South Korea



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] Path
        public static string _default_path;        // �⺻ ���� ��� ���� 
        public static string _default_extension;   // �⺻ Ȯ���� ���� 

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
        public bool Destroyed
        {
            get { return _destroyed; }
            set { _destroyed = value; }
        }
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. �����ֱ�
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [�����ֱ�] Awake/Start/OnDestroy/Update
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

        #region [����] [�����ֱ�] Internal
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

        #region [����] [�����ֱ�] Enabled
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

        #region [���] [����] �Ҹ�
        //   //------------------------------------------------------------------------------------------------------------------------------------------------------
        //   ~JObject()
        //{
        //       // todo: ���� �ʿ�(�Ҹ��ڿ����� �����Լ� ȣ���� �ȵ�.. �̹� ���� ���� �߿� �Ǵٸ� ������ �߻�..)
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
