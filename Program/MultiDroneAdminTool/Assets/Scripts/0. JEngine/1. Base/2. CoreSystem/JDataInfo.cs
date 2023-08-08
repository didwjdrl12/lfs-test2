using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Collections;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JDataInfo_base
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //[Serializable]
    //public abstract class JDataInfo_base
    //{
    //}




    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JCsvData_base
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    [Serializable]
    public abstract class JCsvData_base// : JDataInfo_base
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] Base
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Property]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] 

        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [베이스]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [베이스] Clone
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //public override object Clone()
        //{
        //    var dataBase = (JCsvData_base)this.MemberwiseClone();
        //    dataBase._guid = _guid;
        //    return dataBase;
        //}

        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [CSV 파싱]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [CSV 파싱]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual bool VarifyKey(string KeyValue) { return true; }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Parse(string[] inputData)
        {
        }
        public virtual void AutoParse(string[] attribute, string[] inputData)
        {
            var length = attribute.Length;

            for (int i = 0; i < length; ++i)
                SetValue(this, attribute[i], inputData[i]);
        }

        public virtual void AutoParse(string[] inputData)
        {
            int i = 0;
            var _this_class = this;
            JUtil.DataCopy(ref _this_class, AutoConvert(GetType(), ref inputData, ref i));
        }

        public void SetValue(object obj, string item_name, string input)
        {
            Type type = obj.GetType();
            var field = type.GetField(item_name);

            if (field != null)
            {
                Type field_type = field.FieldType;
                if (field_type.IsPrimitive || (field_type == typeof(string)))
                {
                    field.SetValue(obj, Convert.ChangeType(input, field_type));
                    return;
                }
                // Enum
                if (field_type.IsEnum)
                {
                    field.SetValue(obj, ParseEnum(field_type, input));
                    return;
                }
                // Vector
                if (field_type == typeof(Vector3))
                {
                    field.SetValue(obj, ParseVector3(input));
                    return;
                }
                // List
                if (field_type.IsArray || typeof(IList).IsAssignableFrom(field_type))
                {
                    Type item_type;
                    if (field_type.IsArray)
                        item_type = field_type.GetElementType();
                    else
                        item_type = field_type.GetGenericArguments()[0];

                    if (item_type == typeof(int))
                    {
                        field.SetValue(obj, ParseIntArray(input));
                        return;
                    }
                    if (item_type == typeof(long))
                    {
                        field.SetValue(obj, ParseLongArray(input));
                        return;
                    }
                    if (item_type == typeof(float))
                    {
                        field.SetValue(obj, ParseFloatArray(input));
                        return;
                    }
                }
                // Dictionary
                if (typeof(IDictionary).IsAssignableFrom(field_type))
                {
                    var ga = field_type.GetGenericArguments();
                    var dict = Activator.CreateInstance(field_type) as IDictionary;
                    ParseDictionary(input, ref dict, ga[0], ga[1]);
                    field.SetValue(obj, dict);
                    return;
                }
            }
            else
            {
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                foreach (var class_field in fields)
                {
                    var field_type = class_field.FieldType;
                    if (!field_type.IsClass || !field_type.IsValueType)
                        continue;

                    var field_value = class_field.GetValue(obj);
                    SetValue(field_value, item_name, input);
                    class_field.SetValue(obj, field_value);
                    return;
                }
            }

            // error
            return;
        }
        public object AutoConvert(Type type, ref string[] input, ref int i)
        {
            if (type.IsPrimitive || (type == typeof(string)))
                return Convert.ChangeType(input[i++], type);
            // Enum
            if (type.IsEnum)
                return ParseEnum(type, input[i++]);
            // Vector
            if (type == typeof(Vector3))
                return ParseVector3(input[i++]);
            // List
            if (type.IsArray || typeof(IList).IsAssignableFrom(type))
            {
                Type item_type;
                if (type.IsArray)
                    item_type = type.GetElementType();
                else
                    item_type = type.GetGenericArguments()[0];
                if (item_type == typeof(int))
                    return ParseIntArray(input[i++]);
                if (item_type == typeof(long))
                    return ParseLongArray(input[i++]);
                if (item_type == typeof(float))
                    return ParseFloatArray(input[i++]);
            }
            // Dictionary
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var ga = type.GetGenericArguments();
                var dict = Activator.CreateInstance(type) as IDictionary;
                ParseDictionary(input[i++], ref dict, ga[0], ga[1]);
                return dict;
            }
            // Class
            return ParseClass(type, ref input, ref i);
        }
        #endregion

        #region [CSV 파싱] [유틸]
        public static Int32[] ParseInt32(string input)
        {
            var result = new Int32[input.Length];
            for (int i = 0; i < result.Length; ++i)
                Int32.TryParse(input, out result[i]);
            return result;
        }
        public static Int64 ParseInt64(string input)
        {
            var result = new Int64();
            Int64.TryParse(input, out result);
            return result;
        }


        #region string -> enum
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T ParseEnum<T>(string str_enum)
        {
            if (!Enum.IsDefined(typeof(T), str_enum))
                return default(T);
            return (T)Enum.Parse(typeof(T), str_enum);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object ParseEnum(Type type, string str_enum)
        {
            if (!Enum.IsDefined(type, str_enum))
                return null;
            return Enum.Parse(type, str_enum);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T[] ParseEnumArray<T>(string str_enum_array, char separator = ',')
        {
            var strs = str_enum_array.Split(',');
            if (strs.Length == 0)
                return null;
            var result = new T[strs.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                var str_enum = strs[i];
                if (!Enum.IsDefined(typeof(T), str_enum))
                    result[i] = default(T);
                result[i] = (T)Enum.Parse(typeof(T), str_enum);
            }
            return result;
        }
        #endregion

        #region string -> Vector3
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vector3 ParseVector3(string str_in)
        {
            var v = Vector3.zero;
            var strs = str_in.Split(',');
            if (strs.Length < 3)
                return v;

            v.x = Convert.ToSingle(strs[0]);
            v.y = Convert.ToSingle(strs[1]);
            v.z = Convert.ToSingle(strs[2]);
            return v;
        }
        #endregion

        #region List
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int[] ParseIntArray(string str_line, char separator = '#')
        {
            var strs = str_line.Split(separator);
            var result = new int[strs.Length];
            for (int i = 0; i < result.Length; ++i)
                int.TryParse(strs[i], out result[i]);
            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static long[] ParseLongArray(string str_line, char separator = '#')
        {
            var strs = str_line.Split(separator);
            var result = new long[strs.Length];
            for (int i = 0; i < result.Length; ++i)
                long.TryParse(strs[i], out result[i]);
            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static float[] ParseFloatArray(string str_line, char separator = '#')
        {
            var strs = str_line.Split(separator);
            var result = new float[strs.Length];
            for (int i = 0; i < result.Length; ++i)
                float.TryParse(strs[i], out result[i]);
            return result;
        }
        #endregion

        #region Dictionary
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Dictionary<T0, T1> ParseDictionary<T0, T1>(string str_line, char sep1 = '#', char sep2 = ',')
            where T0 : IConvertible
            where T1 : IConvertible
        {
            var groups = str_line.Split(sep1);
            var result = new Dictionary<T0, T1>();

            for (int i = 0; i < groups.Length; ++i)
            {
                var strs = groups[i].Split(sep2);

                if (strs.Length == 2)
                {
                    //double v0, v1;
                    //double.TryParse(strs[0], out v0);
                    //double.TryParse(strs[1], out v1);
                    var v0 = (T0)Convert.ChangeType(strs[0], typeof(T0));
                    var v1 = (T1)Convert.ChangeType(strs[1], typeof(T1));
                    result[v0] = v1;
                }
            }
            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ParseDictionary(string str_line, ref IDictionary dict, Type t1, Type t2, char sep1 = '#', char sep2 = ',')
        {
            var groups = str_line.Split(sep1);

            for (int i = 0; i < groups.Length; ++i)
            {
                var strs = groups[i].Split(sep2);

                if (strs.Length == 2)
                {
                    var key = Convert.ChangeType(strs[0], t1);
                    var value = Convert.ChangeType(strs[1], t2);
                    dict.Add(key, value);
                }
            }
        }
        #endregion

        #region Class
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public object ParseClass(Type type, ref string[] input, ref int i)
        {
            //// 예외 클래스
            //if (type == typeof(JCollisionMask))
            //    return JCollisionMask.Parse(input, ref i);

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var target = Activator.CreateInstance(type);
            var length = fields.Length;

            for (int cur_field_index = length - 1; cur_field_index >= 0; --cur_field_index)
            {
                var field = fields[cur_field_index];

                field.SetValue(target, AutoConvert(field.FieldType, ref input, ref i));
            }

            return target;
        }
        #endregion

        #endregion

        //#region [CSV 파싱] [이벤트] OnLoad
        //public virtual void OnLoad()
        //{
        //}
        //#endregion

    }
}
