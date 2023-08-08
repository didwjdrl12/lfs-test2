using J2y.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using UnityEngine;


namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JAssetManager
    //
    //		1. JAsset Load
    //      2. JAsset Caching
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JAssetManager : JObject
    {
        #region [Singleton]
        public static JAssetManager s_instance;

        public static JAssetManager Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = JObjectManager.Create<JAssetManager>();
                return s_instance;
            }
            set
            {
                s_instance = value;
            }
        }

        #endregion
        


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] JAsset Caching

        public IDictionary<string, JAsset> _caching_data = new Dictionary<string, JAsset>();

        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [Init] Awake
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Awake()
        {
            base.Awake();
            s_instance = this;
        }
        #endregion

        #region [종료] OnDestroy
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void OnDestroy()
        {
            Clear();
            s_instance = null;
            base.OnDestroy();            
        }
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 데이터 읽기(파일에서)
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Load] Make Asset Data
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual JAsset Load(string assetName, string path = null, string extension = null, bool caching_data = true)
        {
            lock (this)
            {
                JAsset asset = null;

                var load_asset = !caching_data || !_caching_data.ContainsKey(assetName);
                if (load_asset)
                {
                    asset = new JAsset(assetName, path, extension);
                    if (caching_data)
                        _caching_data.Add(assetName, asset);
                }
                else if (caching_data)
                    asset = _caching_data[assetName];

                return asset;
            }
        }
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public JAsset Load<T>(string assetName, string path = null, string extension = null, bool caching_data = true)    { return Load(assetName, typeof(T), path, extension, caching_data); }
        //public JAsset Load<T>(string assetName, string path, bool caching_data = true)                      { return Load(assetName, typeof(T), path, null, caching_data); }
        //public JAsset Load<T>(string assetName, bool caching_data = true)                                   { return Load(assetName, typeof(T), null, null, caching_data); }
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public JAsset Load(string assetName, Type type, string path, bool caching_data = true)              { return Load(assetName, type, path, null, caching_data); }
        //public JAsset Load(string assetName, Type type, bool caching_data = true)                           { return Load(assetName, type, null, null, caching_data); }
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public JAsset Load(string assetName, string path, string extension, bool caching_data = true)       { return Load(assetName, null, path, null, caching_data); }
        //public JAsset Load(string assetName, string path, bool caching_data = true)                         { return Load(assetName, null, path, null, caching_data); }
        //public JAsset Load(string assetName, bool caching_data = true)                                      { return Load(assetName, null, null, null, caching_data); }
        #endregion

        #region [Load] PreLoad
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual bool PreLoad(string path, string extension = ".jasset")
        {
            var di = new DirectoryInfo(path);

            foreach (var dir in di.GetDirectories())
                PreLoad(dir.FullName, extension);

            foreach (var file in di.GetFiles())
            {
                if (extension == Path.GetExtension(file.FullName))
                    Load(Path.GetFileNameWithoutExtension(file.FullName), di.FullName, extension, true);
            }

            return true;
        }
        #endregion

        // todo : ReExport() => read prefabs & exports (unity) - 파일 구조 변경 시 데이터 형식 갱신을 위한 기능


        #region [Clear] 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Clear()
        {
            _caching_data.Clear();

#if !NET_SERVER
            Resources.UnloadUnusedAssets();
#endif
        }
        #endregion
    }
}