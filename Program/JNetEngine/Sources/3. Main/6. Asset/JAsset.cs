using J2y.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JAsset
    //
    //      @목적: 유니티에서 저장된 어셋을 서버에서 읽기 위해서는 적절한 파일 포맷으로 변환할 필요가 있다. 
    //             여러 컴포넌트들을 포함한 액터를 저장하고 읽을 수 있도록 지원한다.
    //
    //		@todo: JReflection 사용 하도록 수정
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JAsset
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [Variable] Asset Info(Name, Path)        
        public string _asset_name { get; }
        public string _path { get; }
        public string _extension { get; }
        #endregion
        
        #region [Variable] Asset        
#if NET_SERVER
        private JObject _impl_asset;
#else
        UnityEngine.Object _impl_asset;
#endif
        #endregion

        #region [Property] Type
        public Type _type {
            get { return _impl_asset.GetType(); }
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Init
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                
        #region [Init] JAsset
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JAsset(string asset_name, string path = null, string extension = null)
        {
            _asset_name = asset_name;
            _path = path ?? "";
            _extension = extension ?? "";

            var fullpath = MakePath(_asset_name, path, extension);

#if NET_SERVER
            _impl_asset = JObjectManager.LoadFromFile(fullpath);
#else
            _impl_asset = Resources.Load(fullpath);
#endif
        }
        #endregion

        #region [Init] [ClientOnly] JAsset
        //------------------------------------------------------------------------------------------------------------------------------------------------------
#if !NET_SERVER
        public JAsset(UnityEngine.Object impl_asset, string asset_name, string path = null, string extension = null)
        {
            _asset_name = asset_name;
            _path = path ?? "";
            _extension = extension ?? "";
            _impl_asset = impl_asset;
        }
#endif
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // JAsset Instantiate
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [JAsset] Instantiate
#if NET_SERVER
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public object Instantiate()
        {
            if (_impl_asset == null)
                return null;
            var new_obj = _impl_asset.Clone<JObject>();
            new_obj.ResetGuid(JUtil.CreateUniqueId());
            return new_obj;
        }
#else
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public object Instantiate()
        {
            if (_impl_asset == null)
                return null;
            var new_obj = UnityEngine.Object.Instantiate(_impl_asset);
            return new_obj;
        }
#endif
        #endregion

        #region [JAsset] Instantiate<T>
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public object Instantiate(Type type)
        {
            return Instantiate();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public T Instantiate<T>() where T : JObject
        {
            return Instantiate() as T;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Path
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Path] Make
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string MakePath(string asset_name, string path = null, string extension = null)
        {
            return string.Format("{0}{1}.{2}", path ?? "", asset_name, extension ?? "");
        }
        #endregion
    }
}