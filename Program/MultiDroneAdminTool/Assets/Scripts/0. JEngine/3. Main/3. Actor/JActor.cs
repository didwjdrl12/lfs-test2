using J2y.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;



namespace J2y
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JActor: 액터 
    //      유니티의 GameObject 같은 객체, 하지만 클라이언트에서는 MonoBehavior를 상속 받음
    //
    //      1. Spawn
    //      2. Components
    //      3. Serialize
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JActor : JObject
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] 0. 기본 정보
        public new static string _default_path = "DataInfo/Assets/";
        [JSerializable]
        public string _actor_name; // for unity property
        #endregion

        #region [Variable] 1. Components
        public IList<JComponent> _components = new List<JComponent>();
        private Dictionary<Type, JComponent> _cache_components = new Dictionary<Type, JComponent>();
        #endregion

        #region [Variable] 2. Default Components
        [HideInInspector]
        public JActorTransform _transform;
        #endregion
        
        #region [Property] Asset Name
        public override string _asset_name { get { return _actor_name; } set { _actor_name = value; } }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Init
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] AwakeInternal
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void AwakeInternal()
        {
            InitializeActor();

            base.AwakeInternal();            
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void InitializeActor()
        {
#if NET_SERVER
            _transform = AddComponent<JActorTransform>();
#else
            _transform = (JActorTransform)AddComponent(gameObject.GetOrAddComponent<JActorTransform>());
            var coms = gameObject.GetComponents<JComponent>();
            foreach (var com in coms)
                AddComponent(com);
#endif
        }
        #endregion

        #region [Actor] DestroyInternal
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void DestroyInternal()
        {
            ClearComponents();
            base.DestroyInternal();
        }
        #endregion

        #region [업데이트] UpdateInternal
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void UpdateInternal()
        {
            base.UpdateInternal();
            _components.ForEach(c => c.UpdateInternal());
        }
        #endregion
        


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. [Spawn]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Spawn] Empty
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JActor New()
        {
            return CreateObject<JActor>();
        }
        #endregion

        #region [static] [Spawn] Load Asset -> Spawn
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JActor SpawnActor(string asset_name, string path = null, string extension = "jasset")
        {
            var asset = JAssetManager.Instance.Load(asset_name, path, extension);
            if (asset == null)
                throw new Exception(string.Format("JActor::Spawn - Asset Load Failed. [ {0} ]", asset_name));

            var actor = asset.Instantiate<JActor>();
            if (actor == null)
                throw new Exception(string.Format("JActor::Spawn - Actor Instantiate Failed. [ {0} ]", asset_name));

            return actor;
        }
        #endregion

        

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 2. [Component]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Component] Add
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual JComponent AddComponent(JComponent com)
        {
            var com1 = FindComponent(com.GetType());
            if (com1 != null)
                return com1;
            //var findCom = FindComponent(com.GetType().FullName);
            //if (findCom != null)
            //    return findCom as T;

            _components.Add(com);
            com._actor = this;
            com.OnAddComponent();

            return com;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual T AddComponent<T>() where T : JComponent
        {
            var com1 = GetComponent<T>();
            if (com1 != null)
                return com1;
            
            return JObjectManager.CreateComponent<T>(this) as T;
        }
        #endregion

        #region [Component] Add/Get
        //------------------------------------------------------------------------------------------------------------------------------------------------------
#if NET_SERVER
        public virtual T GetComponent<T>() where T : JComponent
#else
        public virtual new T GetComponent<T>() where T : JComponent
#endif
        {
            // 1. cache check
            var ct = typeof(T);
            if (_cache_components.ContainsKey(ct))
                return _cache_components[ct] as T;

            // 2. list search
            var com = _components.FirstOrDefault(c => (c.GetType() == ct) || c.GetType().IsSubclassOf(ct));
            if (com != null)
                _cache_components[ct] = com;
            return com as T;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual JComponent FindComponent(Type type)
        {
            if (_cache_components.ContainsKey(type))
                return _cache_components[type];

            var com = _components.FirstOrDefault(c => (c.GetType() == type) || c.GetType().IsSubclassOf(type));
            if (com != null)
                _cache_components[type] = com;
            return com;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual JComponent FindComponent(string com_type_name)
        {
            return FindComponent(JUtil.TryGetType(com_type_name));
        }
        #endregion

        #region [Component] Clear        
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ClearComponents()
        {
#if NET_SERVER
            foreach (var com in _components)
                JObjectManager.Destroy(com);
#endif
            _components.Clear();
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 3. Serialize
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        static public void NetSerializeActor(BinaryWriter writer, JActor actor)
        {
            writer.Write(actor._guid);
            writer.Write(actor._asset_name);
            actor.Serialize(writer);
        }
        static public JActor NetDeserializeActor(BinaryReader reader)
        {
            var guid = reader.ReadInt64();
            var actor_name = reader.ReadString();

            var actor = JObjectManager.Find(guid) as JActor;
            if (actor == null)
                actor = SpawnActor(actor_name);

            actor.Deserialize(reader);
            return actor;
        }


        #region [Serialize] 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(_components.Count);
            foreach (var com in _components)
            {
                writer.Write(JObjectManager.GetSerializeName(com.GetType()));
                com.Serialize(writer);
            }
        }
        #endregion

        #region [Deserialize] 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            int component_count = reader.ReadInt32();
            for (int i = 0; i < component_count; ++i)
            {
                var serialize_name = reader.ReadString();
                var serialize_type = JObjectManager.GetSerializeType(serialize_name);
                if (serialize_type == null)
                {
                    JLogger.Write(string.Format("JActor::Deserialize - JComponet Load Failed. [ {0} ]", serialize_name));
                    continue;
                }

                var component = FindComponent(serialize_type);
                if (component == null)
                {
                    component = JObjectManager.CreateComponent(this, serialize_type);
                    if (component == null)
                    {
                        JLogger.Write(string.Format("JActor::Deserialize - JComponet Load Failed. [ {0} ]", serialize_name));
                        continue;
                    }
                }
                component.Deserialize(reader);
            }

            _transform = GetComponent<JActorTransform>();
        }
        #endregion


        #region [File] Export/Import
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Export(string fullpath)
        {
            JObjectManager.SaveToFile(fullpath, this);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Import(string fullpath)
        {
            JObjectManager.LoadFromFile(fullpath, this);
        }
        #endregion



        #region [백업] [NetSerialize] 
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public override void NetSerialize(BinaryWriter writer)
        //{
        //    writer.Write(_guid);
        //    writer.Write(_asset_name);
        //    writer.Write(_components.Count);
        //    foreach (var com in _components)
        //    {
        //        var serialize_name = JObjectManager.GetSerializeName(com.GetType());
        //        writer.Write(serialize_name);
        //        com.NetSerialize(writer);
        //        writer.Write(com._guid);
        //    }
        //}
        //#endregion

        //#region [NetDeserialize] 
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public override void NetDeserialize(BinaryReader reader)
        //{
        //    ResetGuid(reader.ReadInt64());
        //    _asset_name = reader.ReadString();
        //    int component_count = reader.ReadInt32();
        //    for (int i = 0; i < component_count; ++i)
        //    {
        //        var serialize_name = reader.ReadString();
        //        var serialize_type = JObjectManager.GetSerializeType(serialize_name);
        //        if (serialize_type == null)
        //        {
        //            JLogger.Write(string.Format("JActor::NetDeserialize - JComponet Load Failed. [ {0} ]", serialize_name));
        //            continue;
        //        }

        //        var component = FindComponent(serialize_type);
        //        if (component == null)
        //        {
        //            component = JObjectManager.CreateComponent(this, serialize_type);

        //            if (component == null)
        //            {
        //                JLogger.Write(string.Format("JActor::NetDeserialize - JComponet Load Failed. [ {0} ]", serialize_name));
        //                continue;
        //            }
        //        }
        //        component.NetDeserialize(reader);
        //        component.ResetGuid(reader.ReadInt64());
        //    }
        //}

    
        #endregion

    }
}
