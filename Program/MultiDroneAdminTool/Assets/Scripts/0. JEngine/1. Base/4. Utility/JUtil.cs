using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using UnityEngine;
//using Random = UnityEngine.Random;
using System.Xml.Serialization;
using System.Reflection;
using System.Collections;
using System.Runtime.CompilerServices;

//namespace J2y
//{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JUtil
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public partial class JUtil : MonoBehaviour
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Base
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [로그] Write
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void WriteLog(string log, params object[] args)
        {
            Console.WriteLine(string.Format(log, args));
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Log(string text, Vector3 savePosition)
        {
            Console.WriteLine(string.Format("{0}({1:0.0}, {2:0.0}, {3:0.0})", text, savePosition.x, savePosition.y, savePosition.z));
        }
        #endregion

        #region [유니크 ID]
        private static long s_uniqueIndex = 1;

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static long CreateUniqueId()
        {
            s_uniqueIndex++;
            var curTick = DateTime.Now.Ticks + s_uniqueIndex;
            return curTick;
        }
        #endregion



        #region [유틸] CreatePropertyInstance
        static public object CreatePropertyInstance(Type type, string property_name)
        {
            while (type != null)
            {
                var prop_type_info = type.GetProperty(property_name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (prop_type_info == null)
                {
                    type = type.BaseType;
                    continue;
                }
                var make_type = prop_type_info.PropertyType;
                if (make_type.IsAbstract)
                    return null;

                return Activator.CreateInstance(make_type);
            }
            return null;
        }
        #endregion

        #region [유틸] CloneProcedure
        public static T CloneProcedure<T>(T obj)
        {
            if (obj == null)
                return default(T);

            var type = obj.GetType();

            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            {
                return obj;
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                IList list = (IList)obj;
                var list_count = list.Count;

                if (type.IsArray)
                {
                    IList copiedArray = Array.CreateInstance(type.GetElementType(), list_count);
                    for (int i = 0; i < list_count; ++i)
                        copiedArray[i] = CloneProcedure(list[i]);

                    return (T)copiedArray;
                }
                else
                {
                    IList copiedList = (IList)Activator.CreateInstance(type);
                    for (int i = 0; i < list_count; ++i)
                        copiedList.Add(CloneProcedure(list[i]));

                    return (T)copiedList;
                }
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                IDictionary dict = (IDictionary)obj;
                IDictionary copiedDict = (IDictionary)Activator.CreateInstance(type);

                foreach (var key in dict.Keys)
                    copiedDict.Add(key, CloneProcedure(dict[key]));

                return (T)copiedDict;
            }
            //else if (typeof(Tuple).IsAssignableFrom(type))
            //{
            //    Tuple tuple = (Tuple)obj;
            //    var tuple_length = tuple.Length;
            //    var args = new object[tuple_length];

            //    for (int i = 0; i < tuple_length; ++i)
            //        args[i] = CloneProcedure(tuple[i]);

            //    return (T)Activator.CreateInstance(type, args);
            //}
            else if (typeof(ICloneable).IsAssignableFrom(type))
            {
                ICloneable cloneObj = (ICloneable)obj;
                return (T)cloneObj.Clone();
            }
            else if (type.IsClass || type.IsValueType)
            {
                object copiedObject = Activator.CreateInstance(type);

                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue != null)
                        field.SetValue(copiedObject, fieldValue); // warning! : shallow copy
                }
                return (T)copiedObject;
            }

            return default(T);
        }
        #endregion

        #region [유틸] DataCopy
        public static void DataCopy<T>(ref T copiedObject, object obj)
        {
            if (obj == null)
                return;

            var type = copiedObject.GetType();
            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            {
                var target = copiedObject as object;
                target = obj;
                return;
            }
            else if (type.IsArray)
            {
                Type typeElement = Type.GetType(type.FullName.Replace("[]", string.Empty));
                var array = obj as Array;
                var copiedArray = copiedObject as Array;
                for (int i = 0; i < array.Length; i++)
                {
                    copiedArray.SetValue(CloneProcedure(array.GetValue(i)), i);
                }
            }
            else if (type.IsClass || type.IsValueType)
            {
                var objType = obj.GetType();
                if (objType.IsAssignableFrom(type) == false)
                    return;

                FieldInfo[] fields = objType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue != null)
                    {
                        field.SetValue(copiedObject, CloneProcedure(fieldValue));
                    }
                }
            }
        }
        public static void DataCopy(object copiedObject, object obj) // for class
        {
            if (obj == null)
                return;

            var type = copiedObject.GetType();
            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            {
                var target = copiedObject as object;
                target = obj;
                return;
            }
            else if (type.IsArray)
            {
                Type typeElement = Type.GetType(type.FullName.Replace("[]", string.Empty));
                var array = obj as Array;
                var copiedArray = copiedObject as Array;
                for (int i = 0; i < array.Length; i++)
                {
                    copiedArray.SetValue(CloneProcedure(array.GetValue(i)), i);
                }
            }
            else if (type.IsClass || type.IsValueType)
            {
                var objType = obj.GetType();
                if (objType.IsAssignableFrom(type) == false)
                    return;

                FieldInfo[] fields = objType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue != null)
                    {
                        field.SetValue(copiedObject, CloneProcedure(fieldValue));
                    }
                }
            }
        }
        #endregion



        #region [보류] fade in/out

        //private Texture2D _fadeOutTexture;
        //public float _fadeSpeed = 0.6f;
        //private float _fadeAlpha = 0.0f;
        //private float _fadeDir = -1.0f;

        //private void loadImages()
        //{
        //    // 이미지 숫자
        //    _fadeOutTexture = Resources.Load("GUI/Black") as Texture2D;

        //    Console.WriteLine("Util.loadImages");
        //}


        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public void FadeOut()
        //{
        //    _fadeDir = 1.0f;
        //}
        //public void FadeIn()
        //{
        //    _fadeDir = -1.0f;
        //}
        //public void UpdateFadeInOut()
        //{
        //    int oldDepth = GUI.depth;
        //    GUI.depth = -10;
        //    _fadeAlpha += _fadeDir * _fadeSpeed * Time.deltaTime;
        //    _fadeAlpha = Mathf.Clamp01(_fadeAlpha);

        //    Color c = GUI.color;
        //    c.a = _fadeAlpha;
        //    GUI.color = c;
        //    if (_fadeAlpha > 0)
        //        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _fadeOutTexture);
        //    GUI.depth = oldDepth;
        //}
        #endregion


        #region [String To int] 문자열 해시
        public static int StringHash(string str)
        {
            int result = 0;
            int p_pow = 1;

            foreach (char c in str)
            {
                result = (result + (c - 'a' + 1) * p_pow) % 1000000009;
                p_pow = (31 * p_pow) % 1000000009;
            }

            return result;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변환
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [RpcTarget] ToNetPeerType
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static eNetPeerType ToNetPeerType(int rpc_target)
        {
            if (JBitMask.Compare(rpc_target, eRpcTarget.Client))
            {
                return eNetPeerType.Client;
            }
            if (JBitMask.Compare(rpc_target, eRpcTarget.MainServer))
            {
                return eNetPeerType.MainServer;                
            }
            if (JBitMask.Compare(rpc_target, eRpcTarget.FieldServer))
            {
                return eNetPeerType.FieldServer;
            }

            return eNetPeerType.Unknown;
        }
        #endregion

        #region [RpcTarget] Make
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static int Combine(params eRpcTarget[] flags)
        //{
        //    int sum = 0;
        //    foreach (var f in flags)
        //        JBitMask.Set(ref sum, f);
        //    return sum;
        //}
        #endregion


        #region [유틸] [레지스트리] 읽기
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T ReadRegistry<T>(string path, string key)
        {
            //opening the subkey  
            var regi_key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path);

            //if it does exist, retrieve the stored values  
            if (regi_key != null)
            {
                var res = (T)regi_key.GetValue(key);
                regi_key.Close();
                return res;
            }
            return default(T);
        }
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static string WriteRegistry(string path, string key, string value)
        //{
        //    //opening the subkey  
        //    var regi_key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path);

        //    //if it does exist, retrieve the stored values  
        //    if (regi_key != null)
        //    {
        //        var res = regi_key.GetValue("Setting1") as string;
        //        regi_key.Close();
        //        return res;
        //    }
        //    return "";
        //}
        #endregion

    }



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Helper
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region [Helper] Lerp

    public class LerpHelper
    {
        private float _time;
        private float _prevValue;
        public float _target;

        public void SetTarget(float startValue, float target)
        {
            _prevValue = startValue;
            _target = target;
            _time = 0.0f;
        }

        public float Lerp(float dt)
        {
            _time = Mathf.Clamp01(_time + dt);
            Value = Mathf.Lerp(_prevValue, _target, _time);
            return Value;
        }

        public float Value { get; private set; }
    }
    #endregion

    #region [Helper] Timer
    public class TimerHelper
    {
        private float _time;
        private float _interval;

        public TimerHelper(float interval)
        {
            _interval = interval;
        }

        public bool Update(float dt)
        {
            _time += dt;
            if (_time > _interval)
            {
                _time = _time % _interval;
                return true;
            }

            return false;
        }
    }
    #endregion

    #region [유틸] 비트 플래그
    public static class JBitMask
    {
        public static bool Compare<T>(int flags, T flag) where T : struct
        {
            int flagValue = (int)(object)flag;

            return (flags == flagValue) || ((flags & flagValue) > 0);
        }

        public static bool IsSet<T>(int flags, T flag) where T : struct
        {
            int flagValue = 1 << (int)(object)flag;

            return (flags & flagValue) != 0;
        }

        public static void Set<T>(ref int flags, T flag) where T : struct
        {
            int flagValue = 1 << (int)(object)flag;

            flags = (int)(object)(flags | flagValue);
        }

        public static void Unset<T>(ref int flags, T flag) where T : struct
        {
            int flagValue = 1 << (int)(object)flag;

            flags = (int)(object)(flags & (~flagValue));
        }
    }
    #endregion

    

       

//}
