using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Threading;

namespace J2y
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JObjectManager
    //		
    //      1. All Objects (모든 객체 자동 등록)
    //      2. ObjectPool (객체 재사용에 사용, 수동등록)
    //      3. Object Serialization <Name, Type> 매핑
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static class JObjectManager
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] 1. JObjects (All Objects)
        private static IDictionary<long, JObject> _objects = new Dictionary<long, JObject>();
        #endregion

        #region [Variable] 2. Object Pool (Type, Objects) 
        private static Dictionary<Type, Stack<JObject>> m_GenericPool = new Dictionary<Type, Stack<JObject>>();
        #endregion

        #region [Variable] 3. SerializeType <Name, Type[eNetPeer]>
        public static Dictionary<string, Type[]> _serialize_types = new Dictionary<string, Type[]>();
        #endregion



        
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. Object Create/Destroy
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [ObjectPool] Create
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JObject Create(Type instType, Action<JObject> pre_init = null)
        {
            if (instType == null)
                return null;

            JObject obj;

#if NET_SERVER
            //var obj = Get<T>(); // ObjectPool을 사용하면 Variable 초기화가 적절히 안될 수 있으므로 직접 생성(방법 연구 필요)
            obj = Activator.CreateInstance(instType) as JObject;
            if (obj == null)
                throw new Exception(string.Format("JObjectManager::Create - instType은 JObject를 상속받는 형식이어야 합니다. [ {0} ]", instType.Name));
#else
            obj = (new GameObject(instType.Name).AddComponent(instType)) as JObject;
#endif

            ObjectInit_Internal(obj, pre_init);
            return obj;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T Create<T>(Action<JObject> pre_init = null) where T : JObject
        {
            return (T)Create(typeof(T), pre_init);
        }
        public static JObject Create(string serialize_name, Action<JObject> pre_init = null)
        {
            return Create(GetSerializeType(serialize_name), pre_init);
        }
        #endregion

        #region [ObjectPool] CreateComponent
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JComponent CreateComponent(JActor actor, Type type)
        {
            JComponent com;
#if NET_SERVER
            com = Activator.CreateInstance(type) as JComponent;
            if (com == null)
                throw new Exception(string.Format("JObjectManager::CreateComponent - type JComponent 상속받는 형식이어야 합니다. [ {0} ]", type.Name));

            Add(com);
            com.AwakeInternal();
            com.Enabled = true;
#else
            com = actor.gameObject.GetOrAddComponent(type) as JComponent;
#endif
            actor.AddComponent(com);
            com._actor = actor;
            com.OnAddComponent();

            return com;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T CreateComponent<T>(JActor actor) where T : JComponent
        {
            return (T)CreateComponent(actor, typeof(T));
        }
        public static JComponent CreateComponent(JActor actor, string serialize_name)
        {
            return CreateComponent(actor, GetSerializeType(serialize_name));
        }
#endregion

        #region [ObjectPool] Destroy
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Destroy<T>(T obj) where T : JObject
        {
            if (null == obj)
                return;
#if NET_SERVER
            // todo: 검토중..
            // JScheduler.CancelUpdatable(obj.UpdateInternal);
            obj.DestroyInternal();
#else
            GameObject.Destroy(obj);
#endif

            Remove(obj);
        }
        #endregion

        #region [ObjectPool] ObjectInit (Awake/Enable)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ObjectInit_Internal(JObject obj, Action<JObject> pre_init = null)
        {
            pre_init?.Invoke(obj);

#if NET_SERVER
            Add(obj);
            obj.AwakeInternal();
            obj.Enabled = true;
            // todo: Update 함수가 있는 경우만 추가
            // todo: 검토중..
            //JScheduler.AddUpdatable(obj.UpdateInternal);

#else
            obj._gameObject = obj.gameObject;
#endif
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. JObject 추가/삭제
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [JObject] 추가/제거	
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool Add(JObject obj)
        {
            if (Exist(obj._guid))
                return false;
            _objects[obj._guid] = obj;
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool Remove(long guid)
        {
            var obj = Find(guid);
            if (obj != null)
            {
                _objects.Remove(guid);
                return true;
            }
            return false;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool Remove(JObject obj)
        {
            return Remove(obj._guid);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Clear()
        {
            _objects.Clear();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ResetGuid(JObject obj, long new_guid)
        {
            if (obj._guid == new_guid)
                return;
            if (Exist(new_guid))
            {
                JLogger.WriteError("Duplicated Object GUID.");
                //throw new Exception("Duplicated Object GUID.");
            }
            Remove(obj);
            obj._guid = new_guid;
            Add(obj);
        }
        #endregion

        #region [JObject] 찾기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static IEnumerable<JObject> ObjectList()
        {
            return _objects.Values;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JObject Find(long guid)
        {
            if (Exist(guid))
                return _objects[guid];
            return null;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool Exist(long guid)
        {
            return _objects.ContainsKey(guid);
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 2. ObjectPool
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [ObjectPool] Get
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T Get<T>() where T : JObject
        {
            return GetInternal<T>();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static T GetInternal<T>() where T : JObject
        {
            if (m_GenericPool.TryGetValue(typeof(T), out Stack<JObject> pooledObjects))
            {
                if (pooledObjects.Count > 0)
                {
                    return pooledObjects.Pop() as T;
                }
            }
            return Create<T>();
        }
        #endregion

        #region [ObjectPool] Return
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Return<T>(T obj) where T : JObject
        {
            ReturnInternal(obj);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static void ReturnInternal(JObject obj)
        {
            if (m_GenericPool.TryGetValue(obj.GetType(), out Stack<JObject> pooledObjects))
            {
                pooledObjects.Push(obj);
            }
            else
            {
                pooledObjects = new Stack<JObject>();
                pooledObjects.Push(obj);
                m_GenericPool.Add(obj.GetType(), pooledObjects);
            }
        }
        #endregion

        #region [JObjectManager] 정적 생성자/소멸자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        static JObjectManager()
        {
            AppDomain.CurrentDomain.ProcessExit += Destructor;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        static void Destructor(object sender, EventArgs e)
        {
            _objects.Values.Reverse().ForEach(o => Destroy(o));
            Clear();
        }
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 3. SerializeType
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [SerializeType] Type->Name 찾기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string GetSerializeName(Type obj_type)
        {
            foreach (var types_item in _serialize_types)
            {
                foreach (var type in types_item.Value)
                {
                    if (type == null)
                        continue;

                    if (type == obj_type)
                        return types_item.Key;
                }
            }

            return obj_type.FullName;
        }
        #endregion
        
        #region [SerializeType] Name->Type 찾기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Type GetSerializeType(string serialize_name)
        {
            if (_serialize_types.ContainsKey(serialize_name))
            {
                Type type = _serialize_types[serialize_name][(int)JEngineRoot._net_peer_type];
                if (type != null)
                    return type;
            }
            return JUtil.TryGetType(serialize_name);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Type GetSerializeType(string serialize_name, eNetPeerType client_type)
        {
            if (_serialize_types.ContainsKey(serialize_name))
            {
                Type type = _serialize_types[serialize_name][(int)client_type];
                if (type != null)
                    return type;
            }
            return JUtil.TryGetType(serialize_name);
        }
        #endregion
        
        #region [SerializeType] Name->Type 등록 (serialize_name, type, net_peer_type)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RegisterSerializeType(string serialize_name, Type type, eNetPeerType client_type)
        {
            if (_serialize_types.ContainsKey(serialize_name))
            {
                _serialize_types[serialize_name][(int)client_type] = type;
                return;
            }

            var types = new Type[(int)eNetPeerType.Max];
            types[(int)client_type] = type;
            _serialize_types.Add(serialize_name, types);
        }
        public static void RegisterSerializeType(string serialize_name, Type type)
        {
            RegisterSerializeType(serialize_name, type, JEngineRoot._net_peer_type);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RegisterSerializeType(string serialize_name, string type_name, eNetPeerType client_type)
        {
            RegisterSerializeType(serialize_name, JUtil.NameToType(type_name), client_type);
        }
        public static void RegisterSerializeType(string serialize_name, string type_name)
        {
            RegisterSerializeType(serialize_name, JUtil.NameToType(type_name), JEngineRoot._net_peer_type);
        }
        #endregion

        #region [SerializeType] Name->Type 등록 (serialize_name, client, main_server, field_server)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RegisterSerializeType(string serialize_name, Type client_type, Type main_server_type, Type field_server_type)
        {
            if (_serialize_types.ContainsKey(serialize_name))
                return;

            _serialize_types.Add(serialize_name, new Type[] { client_type, main_server_type, field_server_type });
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RegisterSerializeType(string serialize_name, Type client_type, Type server_type)
        {
            RegisterSerializeType(serialize_name, client_type, server_type, server_type);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RegisterSerializeType(string serialize_name, string client_name, string server_name)
        {
            Type client_type = JUtil.NameToType(client_name);
            Type server_type = JUtil.NameToType(server_name);
            RegisterSerializeType(serialize_name, client_type, server_type, server_type);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RegisterSerializeType(string serialize_name, string client_name, string main_server_name, string field_server_name)
        {
            Type client_type = JUtil.NameToType(client_name);
            Type main_server_type = JUtil.NameToType(main_server_name);
            Type field_server_type = JUtil.NameToType(field_server_name);

            RegisterSerializeType(serialize_name, client_type, main_server_type, field_server_type);
        }
        #endregion


        #region [SerializeType] AutoRegistration
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RegistrateAllSerializeType(eNetPeerType peer_type)
        {            
            JReflection.FindAllClasses()
                .Where(c => c
                    .GetCustomAttributes()
                    .Any(attr => attr is JSerializable))
                .ForEach(type =>
                {
                    var attr = type.GetCustomAttributes().FirstOrDefault(att => att is JSerializable) as JSerializable;
                    var serialize_name = attr.Name;
                    var peer_type2 = (attr.NetPeerType == eNetPeerType.Unknown) ? peer_type : attr.NetPeerType;
                    RegisterSerializeType(attr.Name, type, peer_type2);
                });
        }
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 4. Object Serialization
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

       
        #region [Serialization] Serialize / Deserialize
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Serialize(BinaryWriter writer, JObject target)
        {
            writer.Write(GetSerializeName(target.GetType()));
            target.Serialize(writer);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JObject Deserialize(BinaryReader reader, JObject target = null)
        {
            string serialize_name = reader.ReadString();
            if (target == null)
                target = Create(serialize_name);

            if (target == null)
                throw new Exception(string.Format("JObject::Deserialize - Deserialize Failed [ {0} ]", serialize_name));

            target.Deserialize(reader);
            return target;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T Deserialize<T>(BinaryReader reader, JObject target = null) where T : JObject
        {
            return Deserialize(reader, target) as T;
        }
        #endregion

        #region [Serialization] LoadFromFile
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // @target 이미 생성된 객체에 데이터만 읽고 싶을때 사용, null이면 새로 생성
        //
        public static JObject LoadFromFile(string full_path, JObject target = null)
        {
            JObject result = null;

            try
            {
                var fs = new FileStream(full_path, FileMode.Open, FileAccess.Read);
                if (fs.CanRead)
                {
                    var br = new BinaryReader(fs);
                    result = Deserialize(br, target);
                    br.Close();
                }
                fs.Close();
            }
            catch (FileNotFoundException)
            {
                // nothing.
            }
            catch (DirectoryNotFoundException)
            {
                JUtil.CreateFolder(Path.GetDirectoryName(full_path));
                JUtil.WriteLog("JObject::Import() - [ DirectoryNotFoundException ] Create Directory, {0}", full_path);
                result = LoadFromFile(full_path);
            }

            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T LoadFromFile<T>(string full_path, JObject target = null) where T : JObject
        {
            return LoadFromFile(full_path, target) as T;
        }

        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static T LoadFromFile<T>(string asset_name, string path = null, string extension = null) where T : JObject
        //{
        //    var obj = LoadFromFile(MakePath(typeof(T), asset_name, path, extension)) as T;
        //    if (obj != null)
        //        obj._asset_name = asset_name;
        //    return obj;
        //}
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static JObject LoadFromFile(string asset_name, string path, string extension, Type type = null)
        //{
        //    string full_path;
        //    if (type != null)
        //        full_path = MakePath(type, asset_name, path, extension);
        //    else
        //        full_path = MakePath(asset_name, path, extension);

        //    var obj = LoadFromFile(full_path);
        //    if (obj != null)
        //        obj._asset_name = asset_name;
        //    return obj;
        //}
        

        //public virtual void LoadFromFile(string asset_name, string path, string extension = null)
        //{
        //    LoadFromFile(MakePath(GetType(), asset_name ?? _asset_name, path, extension), this);
        //}

        //public virtual void LoadFromFile()
        //{
        //    LoadFromFile(MakePath(GetType(), _asset_name), this);
        //}
        #endregion

        #region [Serialization] SaveToFile
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool SaveToFile(string full_path, JObject target)
        {
            bool result = false;

            try
            {
                var fs = new FileStream(full_path, FileMode.OpenOrCreate, FileAccess.Write);
                if (fs.CanWrite)
                {
                    var bw = new BinaryWriter(fs);

                    Serialize(bw, target);
                    result = true;

                    bw.Close();
                }
                fs.Close();
            }
            catch (FileNotFoundException)
            {
                // nothing.
            }
            catch (DirectoryNotFoundException)
            {
                // nothing.
            }

            return result;
        }


        //public virtual bool SaveToFile(string asset_name, string path, string extension = null)
        //{
        //    return SaveToFile(MakePath(GetType(), asset_name ?? _asset_name, path, extension));
        //}

        //public virtual bool SaveToFile()
        //{
        //    return SaveToFile(MakePath(GetType(), _asset_name));
        //}
        #endregion


        #region [Serialization] [유틸] LoadSerializationType
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Type LoadSerializationType(string full_path)
        {
            Type result = null;

            try
            {
                var fs = new FileStream(full_path, FileMode.Open, FileAccess.Read);
                if (fs.CanRead)
                {
                    var br = new BinaryReader(fs);
                    result = GetSerializeType(br.ReadString());

                    br.Close();
                }
                fs.Close();
            }
            catch (FileNotFoundException)
            {
                // nothing.
            }
            catch (DirectoryNotFoundException)
            {
                JUtil.CreateFolder(Path.GetDirectoryName(full_path));
                JUtil.WriteLog("JObject::Import() - [ DirectoryNotFoundException ] Create Directory, {0}", full_path);
                result = GetSerializeType(full_path);
            }

            return result;
        }
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static Type LoadSerializationType(string asset_name, string path, string extension, Type type = null)
        //{
        //    return LoadSerializationType(MakePath(asset_name, path, extension));
        //}
        #endregion


        //#region [Serialization] [유틸] MakePath
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static string MakePath(Type type, string asset_name, string path = null, string extension = null)
        //{
        //    extension = extension ?? GetDefaultExtension(type);
        //    if (extension.Length > 0 && extension[0] != '.')
        //        extension = "." + extension;

        //    return (path ?? GetDefaultPath(type)) + "/" + asset_name + extension;
        //}
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static string MakePath(string asset_name, string path = null, string extension = null)
        //{
        //    if (extension != null && extension.Length > 0 && extension[0] != '.')
        //        extension = "." + extension;

        //    return (path ?? "") + "/" + asset_name + (extension ?? ".jasset");
        //}
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public string MakePath(string path = null, string extension = null)
        //{
        //    return MakePath(GetType(), _asset_name, path, extension);
        //}
        //#endregion

        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static string GetDefaultExtension(Type type)
        //{
        //    var field_info = type?.GetField("_default_extension", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        //    var val = (string)field_info?.GetValue(null);
        //    return val ?? ".jasset";
        //}
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static string GetDefaultPath(Type type)
        //{
        //    var field_info = type?.GetField("_default_path", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        //    var val = (string)field_info?.GetValue(null);
        //    return val ?? "";
        //}
    }

}
