using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Reflection;
using System.Collections;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JSerialization
    //      새로운 Serialization을 만드는 이유는 다음과 같다.
    //          1. 범용 Serialization은 타입과 값을 모두 저장하기 때문에 용량이 크고 속도가 느리다.
    //          2. 클라이언트와 서버의 객체들은 비슷한 클래스를 사용하지만 정확히 같은 타입은 아니며(ex-LsfUser, LcUser) 어셈블리가 다르기 때문에 
    //             Serialization이 불가능하다.
    //          -> 1번의 경우는 많은 오픈소스가 존재하지만 2번의 이유로 직접 제작하도록 한다.
    //
    //
    // @JSerialization
    //      가장 기본적인 데이터 직렬화 방식(BinaryFormatter 기반)
    //
    // @JNewSerialization : Fast
    //      Reflection을 이용하여 기본 데이터만을 Serialize & Deserialize하는 방식으로, null이나 추상타입에 대한 지원을 하지 않고, 최소한의 데이터만을 변환하는 방식
    //
    // @JNewSerialization : TypeHashing
    //      Fast와 유사한 알고리즘으로 동작하나 데이터의 형식(Type FullName)과 함께 데이터를 변환하여 null이나 추상 타입에 대한 제약이 없는 방식.단, 데이터의 형식이 차지하는 데이터 크기를 최소화하기 위한 Hashing과정이 추가됨
    //      (※ Hashing 결과물을 해석하기 위한 데이터는 별도의 파일로 저장 되며, 이에 대한 경로 지정 필요)
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JSerialization
    //
    //      @SerializationSurrogate
    //          Serialization을 위해서는 class에 [Serializable] Attribute 추가가 필요하다.
    //          그러나 이미 개발된 유니티의 Vector3, Quaternion를 Serialization하기 위해서는 SerializationSurrogate이용한 타입 변환 대리자 필요하다.
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region [SerializationSurrogate] Vector3
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    sealed class Vector3SerializationSurrogate : ISerializationSurrogate
    {
        #region [Serialize] GetObjectData
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
        {
            var v3 = (Vector3)obj;
            info.AddValue("x", v3.x);
            info.AddValue("y", v3.y);
            info.AddValue("z", v3.z);
            //Debug.Log(v3);
        }
        #endregion

        #region [Deserialize] SetObjectData
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var v3 = (Vector3)obj;
            v3.x = (float)info.GetValue("x", typeof(float));
            v3.y = (float)info.GetValue("y", typeof(float));
            v3.z = (float)info.GetValue("z", typeof(float));
            obj = v3;
            return obj;   // Formatters ignore this return value //Seems to have been fixed!
        }
        #endregion

    }
    #endregion

    #region [SerializationSurrogate] Quaternion
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    sealed class QuaternionSerializationSurrogate : ISerializationSurrogate
    {
        #region [Serialize] GetObjectData
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
        {
            var q = (Quaternion)obj;
            info.AddValue("x", q.x);
            info.AddValue("y", q.y);
            info.AddValue("z", q.z);
            info.AddValue("w", q.w);
            //Debug.Log(v3);
        }
        #endregion

        #region [Deserialize] SetObjectData
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var q = (Quaternion)obj;
            q.x = (float)info.GetValue("x", typeof(float));
            q.y = (float)info.GetValue("y", typeof(float));
            q.z = (float)info.GetValue("z", typeof(float));
            q.w = (float)info.GetValue("w", typeof(float));
            obj = q;
            return obj;   // Formatters ignore this return value //Seems to have been fixed!
        }
        #endregion

    }
	#endregion

	//#region [SerializationSurrogate] String
	////------------------------------------------------------------------------------------------------------------------------------------------------------
	//sealed class StringSerializationSurrogate : ISerializationSurrogate
	//{
	//	#region [Serialize] GetObjectData
	//	//------------------------------------------------------------------------------------------------------------------------------------------------------
	//	public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
	//	{
	//		var text = (string)obj;
	//		var buffer = Encoding.UTF8.GetBytes(text);

	//		info.AddValue("len", ((short)buffer.Length).ToBigEndian());
	//		info.AddValue("buf", buffer);
	//	}
	//	#endregion

	//	#region [Deserialize] SetObjectData
	//	//------------------------------------------------------------------------------------------------------------------------------------------------------
	//	public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
	//	{
	//		var text = (string)obj;
	//		VtBase.ReadString
	//		q.x = (float)info.GetValue("x", typeof(float));
	//		q.y = (float)info.GetValue("y", typeof(float));
	//		q.z = (float)info.GetValue("z", typeof(float));
	//		q.w = (float)info.GetValue("w", typeof(float));
	//		obj = q;
	//		return obj;   // Formatters ignore this return value //Seems to have been fixed!
	//	}
	//	#endregion

	//}
	//#endregion

	public static class JSerialization
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //  [BinaryFormatter]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private static BinaryFormatter s_binary_formatter;

        #region [BinaryFormatter] 생성 (+ SerializationSurrogate)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static BinaryFormatter MakeBinaryFormatter()
        {
            var bf = new BinaryFormatter();

            var ss = new SurrogateSelector();
            ss.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
            ss.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), new QuaternionSerializationSurrogate());


            bf.SurrogateSelector = ss;
            return bf;
        }
        #endregion

        #region [BinaryFormatter] GET
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static BinaryFormatter GetBinaryFormatter()
        {
            if (null == s_binary_formatter)
                s_binary_formatter = MakeBinaryFormatter();

            return s_binary_formatter;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //  [Serialization]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Serialize] Stream/BinaryWriter
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Serialize(Stream serializationStream, object graph)
        {
            GetBinaryFormatter().Serialize(serializationStream, graph);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Serialize(BinaryWriter writer, object graph)
        {
            GetBinaryFormatter().Serialize(writer.BaseStream, graph);
        }
        #endregion

        #region [Deserialize] Stream/BinaryReader
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object Deserialize(Stream serializationStream)
        {
            return GetBinaryFormatter().Deserialize(serializationStream);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object Deserialize(BinaryReader reader)
        {
            return GetBinaryFormatter().Deserialize(reader.BaseStream);
        }
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static T Deserialize<T>(BinaryReader reader) where T : class
		{
			var obj = Deserialize(reader);
			return obj as T;
		}
		#endregion

		#region [Deserialize] NetData
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static T Deserialize<T>(Stream serializationStream) where T : class
        {
            var obj = Deserialize(serializationStream);
            return obj as T;
        }
        #endregion

    }


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JNewSerialization
    //
    //      1. Fast Serialization
    //      2. TypeHashing Serialization
    //      3. Network Serialization
    //      4. Variable Replication(Variable 리플리케이션)을 위한 객체 변환 감지
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region [Attribute] JSerialzable
    public class JSerializable : Attribute
    {
        public string Name;
        public eNetPeerType NetPeerType;
        public float Version = 1.0f;
        public bool Deprecated;

        public JSerializable(string name = "", eNetPeerType peer_type = eNetPeerType.Unknown) { Name = name; NetPeerType = peer_type; }
    }
    #endregion

    public static class JNewSerialization
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] BindingFlags
        static readonly BindingFlags _flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        #endregion

        // todo : fast & rpc - ValueTuple

        #region [Variable] [Serializer Optimize]
        public static bool _type_dict_dirty = true;
        public static Dictionary<int, Type> _type_dict = new Dictionary<int, Type>();
        public static List<string> _type_name_list = new List<string>();
        public static string _type_name_list_path = "type_name_list";
        public static Action<string> _on_add_new_type_name = null;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Serialization] [TypeHashing]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Serialize] [TypeHashing] object (+Type)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Serialize(BinaryWriter writer, object value, Type type = null)
        {
            if (value == null)
            {
                SerializeHash(writer, "NULL");
                return;
            }

            if (type == null)
                type = value.GetType();

            if (type.IsEnum)
            {
                SerializeHash(writer, type.FullName, type);
                writer.Write((int)value);
            }
            else if (JReflection.s_write_methods.TryGetValue(type, out MethodInfo writeMethod))
            {
                SerializeHash(writer, type.FullName, type);
                writeMethod.Invoke(writer, new object[] { value });
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                Type genericArgType;
                if (type.IsArray)
                    genericArgType = type.GetElementType();
                else
                    genericArgType = type.GetGenericArguments()[0];

                type = JUtil.MakeGenericTypeName(type);
                SerializeHash(writer, type.FullName, type);
                SerializeHash(writer, genericArgType.FullName, genericArgType);

                var list = value as IList;

                Serialize(writer, list.Count);
                foreach (var item in list)
                {
                    Serialize(writer, item);
                }
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var genericArgsType = type.GetGenericArguments();

                type = JUtil.MakeGenericTypeName(type);
                SerializeHash(writer, type.FullName, type);
                SerializeHash(writer, genericArgsType[0].FullName, genericArgsType[0]);
                SerializeHash(writer, genericArgsType[1].FullName, genericArgsType[1]);

                var dict = value as IDictionary;

                Serialize(writer, dict.Count);
                foreach (var key in dict.Keys)
                {
                    Serialize(writer, key);
                    Serialize(writer, dict[key]);
                }
            }
            //else if (typeof(Tuple).IsAssignableFrom(type))
            //{
            //    var genericArgsType = type.GetGenericArguments();

            //    type = JUtil.MakeGenericTypeName(type);
            //    SerializeHash(writer, type.FullName, type);

            //    var tuple = value as Tuple;

            //    int length = tuple.Length;
            //    Serialize(writer, length);
            //    for (int i = 0; i < length; ++i)
            //    {
            //        SerializeHash(writer, genericArgsType[i].FullName, genericArgsType[i]);
            //        Serialize(writer, tuple[i]);
            //    }
            //}
            else if (type.IsClass || type.IsValueType)
            {
                SerializeHash(writer, type.FullName, type);
                FieldInfo[] fields = type.GetFields(_flags);
                foreach (FieldInfo field in fields)
                {
                    Serialize(writer, field.GetValue(value));
                }
            }
        }

        #endregion

        #region [Serialize] [TypeHashing] Function Overloading
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Serialize(Stream stream, object value, Type type = null) { Serialize(new BinaryWriter(stream), value, type); }
        public static void Serialize(BinaryWriter writer, params object[] args) { Serialize(writer, (object)args); }
        public static void Serialize(Stream stream, params object[] args) { Serialize(new BinaryWriter(stream), (object)args); }
        #endregion

        #region [Deserialize] [TypeHashing] 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object Deserialize(BinaryReader reader)
        {
            Type type = DeserializeHash(reader);

            if (type == null)
                return null;

            if (type.IsEnum)
            {
                var enum_inst = Activator.CreateInstance(type);
                enum_inst = type.GetEnumValues().GetValue(reader.ReadInt32());
                return enum_inst;
            }
            else if (JReflection.s_read_methods.TryGetValue(type, out MethodInfo readMethod))
            {
                return readMethod.Invoke(reader, null);
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                Type item_type = DeserializeHash(reader);

                IList list;

                int list_count = (int)Deserialize(reader);
                if (type.IsArray)
                {
                    list = Array.CreateInstance(item_type, list_count);
                    for (int i = 0; i < list_count; ++i)
                        list[i] = Deserialize(reader);
                }
                else
                {
                    list = Activator.CreateInstance(type.MakeGenericType(item_type)) as IList;
                    for (int i = 0; i < list_count; ++i)
                        list.Add(Deserialize(reader));
                }

                return list;
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                Type key_type = DeserializeHash(reader);
                Type value_type = DeserializeHash(reader);

                var dict = Activator.CreateInstance(type.MakeGenericType(key_type, value_type)) as IDictionary;

                int dict_count = (int)Deserialize(reader);
                for (int i = 0; i < dict_count; ++i)
                {
                    var key = Deserialize(reader);
                    var item = Deserialize(reader);

                    dict.Add(key, item);
                }

                return dict;
            }
            else if (typeof(Tuple).IsAssignableFrom(type))
            {
                int list_count = (int)Deserialize(reader);
                var types = new Type[list_count];
                var args = new object[list_count];

                for (int i = 0; i < list_count; ++i)
                {
                    types[i] = DeserializeHash(reader);
                    args[i] = Deserialize(reader);
                }

                return Activator.CreateInstance(type.MakeGenericType(types), args);
            }
            else if (type.IsClass || type.IsValueType)
            {
                object inst = Activator.CreateInstance(type);
                FieldInfo[] fields = type.GetFields(_flags);
                foreach (FieldInfo field in fields)
                {
                    field.SetValue(inst, Deserialize(reader));
                }
                return inst;
            }

            return null;
        }
        #endregion

        #region [Deserialize] [TypeHashing] Function Overloading
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object Deserialize(Stream stream) { return Deserialize(new BinaryReader(stream)); }
        public static T Deserialize<T>(BinaryReader reader) { return (T)Deserialize(reader); }
        public static T Deserialize<T>(Stream stream) { return (T)Deserialize(stream); }
        #endregion

        #region [Deserialize] [TypeHashing] RPC (object[])
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object[] Deserialize(BinaryReader reader, MethodInfo mi)
        {
            var arguments = mi.GetGenericArguments();
            var targets = new object[arguments.Length];

            for (int i = 0; i < arguments.Length; ++i)
            {
                targets[i] = Deserialize(reader);
            }

            return targets;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object[] Deserialize(Stream stream, MethodInfo mi)
        {
            return Deserialize(new BinaryReader(stream), mi);
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Serialization] [Fast] (추상 타입 사용 불가, null 불가)
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Serialize] [Fast] object (+Type)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void FastSerialize(BinaryWriter writer, object value, Type type = null)
        {
            if (type == null)
                type = value.GetType();

            #region Native Data Serialize
            if (JReflection.s_write_methods.TryGetValue(type, out MethodInfo writeMethod))
            {
                writeMethod.Invoke(writer, new object[] { value });
                return;
            }

            if (type.IsEnum)
            {
                FastSerialize(writer, value, typeof(int));
                return;
            }
            #endregion

            #region List Data Serialize
            if (typeof(IList).IsAssignableFrom(type))
            {
                var list = value as IList;

                int count = list.Count;
                FastSerialize(writer, count);

                for (int i = 0; i < count; ++i)
                    FastSerialize(writer, list[i]);

                return;
            }
            #endregion

            #region Dictionary Data Serialize
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var dict = value as IDictionary;

                int count = dict.Count;
                FastSerialize(writer, count);

                var arg_type = dict.GetType().GetGenericArguments();

                foreach (var key in dict.Keys)
                {
                    FastSerialize(writer, key, arg_type[0]);
                    FastSerialize(writer, dict[key], arg_type[1]);
                }

                return;
            }
            #endregion

            #region Class & Struct Data Serialize
            var fields = type.GetFields(_flags);
            foreach (var fi in fields)
            {
                var target = fi.GetValue(value);
                if (target == null)
                    throw new Exception("JNewSerialization::Serialize - target is null");

                FastSerialize(writer, target);
            }
            #endregion
        }
        #endregion

        #region [Serialize] [Fast] Function Overloading
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void FastSerialize(Stream stream, object value, Type type = null) { FastSerialize(new BinaryWriter(stream), value, type); }
        public static void FastSerialize(Stream stream, params object[] args) { FastSerialize(new BinaryWriter(stream), args); }
        public static void FastSerialize(BinaryWriter writer, params object[] args)
        {
            for (int i = 0; i < args.Length; ++i)
                FastSerialize(writer, args[i]);
        }
        #endregion

        #region [Deserialize] [Fast]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object FastDeserialize(BinaryReader reader, Type type)
        {
            #region Native Data Deserialize
            if (JReflection.s_read_methods.TryGetValue(type, out MethodInfo readMethod))
            {
                return readMethod.Invoke(reader, null);
            }

            if (type.IsEnum)
            {
                return FastDeserialize(reader, typeof(int));
            }
            #endregion

            #region Array Data Deserialize
            if (type.IsArray == true)
            {

                int count = (int)FastDeserialize(reader, typeof(int));
                var array = Activator.CreateInstance(type, new object[] { count }) as IList;
                var array_itemType = type.GetElementType();

                for (int i = 0; i < count; ++i)
                {
                    array[i] = FastDeserialize(reader, array_itemType);
                }

                return array;
            }
            #endregion

            var target = Activator.CreateInstance(type); // create instance

            #region List Data Deserialize
            if (typeof(IList).IsAssignableFrom(type))
            {
                var list = target as IList;
                var list_itemType = type.GetGenericArguments()[0];

                int count = (int)FastDeserialize(reader, typeof(int));
                for (int i = 0; i < count; ++i)
                {
                    list.Add(FastDeserialize(reader, list_itemType));
                }

                return list;
            }
            #endregion

            #region Dictionary Data Deserialize
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var dict = target as IDictionary;

                var dict_keyType = type.GetGenericArguments()[0];
                var dict_itemType = type.GetGenericArguments()[1];

                int count = (int)FastDeserialize(reader, typeof(int));
                for (int i = 0; i < count; ++i)
                {
                    dict.Add(FastDeserialize(reader, dict_keyType), FastDeserialize(reader, dict_itemType));
                }

                return dict;
            }
            #endregion

            #region Class & Struct Deserialize
            var fields = type.GetFields(_flags);

            foreach (var fi in fields)
            {
                fi.SetValue(target, FastDeserialize(reader, fi.FieldType));
            }
            #endregion

            return target;
        }
        #endregion

        #region [Deserialize] [Fast] Function Overloading
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object FastDeserialize(Stream stream, Type type) { return FastDeserialize(new BinaryReader(stream), type); }
        public static T FastDeserialize<T>(BinaryReader reader) { return (T)FastDeserialize(reader, typeof(T)); }
        public static T FastDeserialize<T>(Stream stream) { return (T)FastDeserialize(new BinaryReader(stream), typeof(T)); }
        #endregion

        #region [Deserialize] [Fast] RPC (object[], fast, 추상 타입 사용 불가, null 불가)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object[] FastDeserialize(BinaryReader reader, MethodInfo mi)
        {
            var arguments = mi.GetGenericArguments();
            var targets = new object[arguments.Length];

            for (int i = 0; i < arguments.Length; ++i)
            {
                targets[i] = FastDeserialize(reader, arguments[i].GetType());
            }

            return targets;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object[] FastDeserialize(Stream stream, MethodInfo mi)
        {
            return FastDeserialize(new BinaryReader(stream), mi);
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Serializer Optimize] 
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Serializer Optimize] [TypeNameList] Save / Load
        static void SaveTypeNameList()
        {
            try
            {
                var fs = new FileStream(_type_name_list_path, FileMode.OpenOrCreate, FileAccess.Write);
                if (fs.CanWrite)
                {
                    BinaryWriter bw = new BinaryWriter(fs);

                    bw.Write(_type_name_list.Count);
                    foreach (string type_name in _type_name_list)
                        bw.Write(type_name);

                    bw.Close();
                }
                fs.Close();
            }
            catch (DirectoryNotFoundException)
            {
                //JUtil.CreateFolder(_type_name_list_path);
                //JUtil.WriteLog("JNewSerialization::SaveTypeNameList() - [ DirectoryNotFoundException ] Create Directory, {0}", _type_name_list_path);
                //
                //SaveTypeNameList();
            }
        }

        static bool LoadTypeNameList()
        {
            try
            {
                FileStream fs = new FileStream(_type_name_list_path, FileMode.Open, FileAccess.Read);
                if (fs.CanRead)
                {
                    BinaryReader br = new BinaryReader(fs);

                    _type_name_list.Clear();

                    int cnt = br.ReadInt32();
                    for (int i = 0; i < cnt; ++i)
                        _type_name_list.Add(br.ReadString());

                    br.Close();
                }
                fs.Close();

                return true;
            }
            catch (FileNotFoundException)
            {
                // nothing.
            }
            catch (DirectoryNotFoundException)
            {
                //JUtil.CreateFolder(_type_name_list_path);
                //JUtil.WriteLog("JNewSerialization::LoadTypeNameList() - [ DirectoryNotFoundException ] Create Directory, {0}", _type_name_list_path);
            }

            return false;
        }
        #endregion

        #region [Serializer Optimize] [갱신] Update TypeHash
        static void UpdateTypeHash()
        {
            if (_type_dict_dirty == false)
                return;
            _type_dict_dirty = false;

            _type_name_list.Clear();

            LoadTypeNameList();

            foreach (string type_name in _type_name_list)
            {
                int hash_idx = JUtil.StringHash(type_name);
                if (_type_dict.ContainsKey(hash_idx))
                    continue;

                var type = JUtil.TryGetType(type_name);
                if (type == null && type_name != "NULL")
                    continue;

                _type_dict.Add(hash_idx, type);
            }
        }

        public static int UpdateTypeHash(string str, Type type)
        {
            UpdateTypeHash();

            int hash_idx = JUtil.StringHash(str);
            if (_type_dict.ContainsKey(hash_idx) == false)
            {
                if (_type_name_list.Contains(str) == false)
                {
                    _type_name_list.Add(str);
                    SaveTypeNameList();
                    _on_add_new_type_name?.Invoke(str);
                }

                _type_dict.Add(hash_idx, type);
            }

            return hash_idx;
        }
        #endregion

        #region [Serializer Optimize] [Serialize] String To Hash & Save Type
        static void SerializeHash(BinaryWriter writer, string str, Type type = null)
        {
            UpdateTypeHash();

            int hash_idx = JUtil.StringHash(str);
            if (_type_dict.ContainsKey(hash_idx) == false)
            {
                _type_name_list.Add(str);
                _type_dict.Add(hash_idx, type);
                SaveTypeNameList();
            }

            writer.Write(hash_idx);
        }
        #endregion

        #region [Serializer Optimize] [Deserialize] Hash To Type
        static Type DeserializeHash(BinaryReader reader)
        {
            UpdateTypeHash();

            return _type_dict[reader.ReadInt32()];
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [NetSerialize] 
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //#region [Util] NetSerialize
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static void NetSerialize(BinaryWriter writer, object value, Type type = null)
        //{
        //    if (value == null)
        //    {
        //        SerializeHash(writer, "NULL", null);
        //        return;
        //    }

        //    if (type == null)
        //        type = value.GetType();

        //    if (type.IsEnum)
        //    {
        //        SerializeHash(writer, type.FullName, type);
        //        writer.Write((int)value);
        //    }
        //    else if (JReflection.s_write_methods.TryGetValue(type, out MethodInfo writeMethod))
        //    {
        //        SerializeHash(writer, type.FullName, type);
        //        writeMethod.Invoke(writer, new object[] { value });
        //    }
        //    else if (typeof(IList).IsAssignableFrom(type))
        //    {
        //        Type genericArgType;
        //        if (type.IsArray)
        //            genericArgType = type.GetElementType();
        //        else
        //            genericArgType = type.GetGenericArguments()[0];

        //        type = JUtil.MakeGenericTypeName(type);
        //        SerializeHash(writer, type.FullName, type);
        //        SerializeHash(writer, genericArgType.FullName, genericArgType);

        //        var list = value as IList;

        //        NetSerialize(writer, list.Count);
        //        foreach (var item in list)
        //        {
        //            NetSerialize(writer, item);
        //        }
        //    }
        //    else if (typeof(IDictionary).IsAssignableFrom(type))
        //    {
        //        var genericArgsType = type.GetGenericArguments();

        //        type = JUtil.MakeGenericTypeName(type);
        //        SerializeHash(writer, type.FullName, type);
        //        SerializeHash(writer, genericArgsType[0].FullName, genericArgsType[0]);
        //        SerializeHash(writer, genericArgsType[1].FullName, genericArgsType[1]);

        //        var dict = value as IDictionary;

        //        NetSerialize(writer, dict.Count);
        //        foreach (var key in dict.Keys)
        //        {
        //            NetSerialize(writer, key);
        //            NetSerialize(writer, dict[key]);
        //        }
        //    }
        //    else if (typeof(ITuple).IsAssignableFrom(type))
        //    {
        //        var genericArgsType = type.GetGenericArguments();

        //        type = JUtil.MakeGenericTypeName(type);
        //        SerializeHash(writer, type.FullName, type);

        //        var tuple = value as ITuple;

        //        int length = tuple.Length;
        //        NetSerialize(writer, length);
        //        for (int i = 0; i < length; ++i)
        //        {
        //            SerializeHash(writer, genericArgsType[i].FullName, genericArgsType[i]);
        //            NetSerialize(writer, tuple[i]);
        //        }
        //    }
        //    else if (typeof(JObject).IsAssignableFrom(type))
        //    {
        //        type = typeof(JObject);
        //        SerializeHash(writer, type.FullName, type);

        //        var obj = value as JObject;

        //        writer.Write(obj.GetSerializeName());
        //        obj.NetSerialize(writer);
        //    }
        //    else if (type.IsClass || type.IsValueType)
        //    {
        //        SerializeHash(writer, type.FullName, type);

        //        FieldInfo[] fields = type.GetFields(_flags);
        //        foreach (FieldInfo field in fields)
        //        {
        //            NetSerialize(writer, field.GetValue(value));
        //        }
        //    }
        //}
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static void NetSerialize(Stream stream, object value, Type type = null)
        //{
        //    NetSerialize(new BinaryWriter(stream), value, type);
        //}
        //#endregion

        //#region [Util] NetDeserialize
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static object NetDeserialize(BinaryReader reader)
        //{
        //    Type type = DeserializeHash(reader);

        //    if (type == null)
        //        return null;

        //    if (type.IsEnum)
        //    {
        //        var enum_inst = Activator.CreateInstance(type);
        //        enum_inst = type.GetEnumValues().GetValue(reader.ReadInt32());
        //        return enum_inst;
        //    }
        //    else if (JReflection.s_read_methods.TryGetValue(type, out MethodInfo readMethod))
        //    {
        //        return readMethod.Invoke(reader, null);
        //    }
        //    else if (typeof(IList).IsAssignableFrom(type))
        //    {
        //        Type item_type = DeserializeHash(reader);

        //        IList list;

        //        int list_count = (int)NetDeserialize(reader);
        //        if (type.IsArray)
        //        {
        //            list = Array.CreateInstance(item_type, list_count);
        //            for (int i = 0; i < list_count; ++i)
        //                list[i] = NetDeserialize(reader);
        //        }
        //        else
        //        {
        //            list = Activator.CreateInstance(type.MakeGenericType(item_type)) as IList;
        //            for (int i = 0; i < list_count; ++i)
        //                list.Add(NetDeserialize(reader));
        //        }

        //        return list;
        //    }
        //    else if (typeof(IDictionary).IsAssignableFrom(type))
        //    {
        //        Type key_type = DeserializeHash(reader);
        //        Type value_type = DeserializeHash(reader);

        //        var dict = Activator.CreateInstance(type.MakeGenericType(key_type, value_type)) as IDictionary;

        //        int dict_count = (int)NetDeserialize(reader);
        //        for (int i = 0; i < dict_count; ++i)
        //        {
        //            var key = NetDeserialize(reader);
        //            var item = NetDeserialize(reader);

        //            dict.Add(key, item);
        //        }

        //        return dict;
        //    }
        //    else if (typeof(ITuple).IsAssignableFrom(type))
        //    {
        //        int list_count = (int)NetDeserialize(reader);
        //        var types = new Type[list_count];
        //        var args = new object[list_count];

        //        for (int i = 0; i < list_count; ++i)
        //        {
        //            types[i] = DeserializeHash(reader);
        //            args[i] = NetDeserialize(reader);
        //        }

        //        return Activator.CreateInstance(type.MakeGenericType(types), args);
        //    }
        //    else if (typeof(JObject).IsAssignableFrom(type))
        //    {
        //        var serialize_name = reader.ReadString();
        //        var obj = JObjectManager.Create(serialize_name);

        //        if (obj == null)
        //            throw new Exception(string.Format("JSerialization::NetDeserialize - NetDeserialize Failed [ {0} ]", serialize_name));

        //        obj.NetDeserialize(reader);
        //        return obj;
        //    }
        //    else if (type.IsClass || type.IsValueType)
        //    {
        //        object inst = Activator.CreateInstance(type);
        //        FieldInfo[] fields = type.GetFields(_flags);
        //        foreach (FieldInfo field in fields)
        //        {
        //            field.SetValue(inst, NetDeserialize(reader));
        //        }
        //        return inst;
        //    }

        //    return null;
        //}

        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static object NetDeserialize(Stream stream)
        //{
        //    return NetDeserialize(new BinaryReader(stream));
        //}
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static T NetDeserialize<T>(BinaryReader reader)
        //{
        //    return (T)NetDeserialize(reader);
        //}
        //public static T NetDeserialize<T>(Stream stream)
        //{
        //    return (T)NetDeserialize(stream);
        //}
        //#endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 4. Variable Replication
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [VariableReplication] DiffSerialize
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int DiffSerialize(ref List<object> obj_list, object current_value, object prev_value, int parnet_field_index = -1)
        {
            if (current_value == null && prev_value == null)
                return 0;

            if (parnet_field_index != -1)
                obj_list.Add(parnet_field_index);

            if (current_value == null || prev_value == null)
            {
                obj_list.Add(-1);

                if (current_value == null)
                    obj_list.Add(UpdateTypeHash("NULL", null));
                else
                {
                    var curr_type = current_value.GetType();
                    obj_list.Add(UpdateTypeHash(curr_type.ToString(), curr_type));
                    obj_list.Add(current_value);
                }
                return -1;
            }

            Type type = current_value.GetType();
            if (type.IsEnum)
                type = typeof(int);

            int diff_count_index = obj_list.Count;
            int diff_count = 0;
            obj_list.Add(0);
            obj_list.Add(UpdateTypeHash(type.ToString(), type));

            #region Native Data Serialize
            if (JReflection.HasWriteMethod(type))
            {
                if (current_value.Equals(prev_value) == false)
                {
                    obj_list.Add(current_value);
                    diff_count++;
                }
            }
            #endregion

            #region List & Array Serialize
            else if (typeof(IList).IsAssignableFrom(type))
            {
                var curr_list = current_value as IList;
                var prev_list = prev_value as IList;

                if (curr_list.Count == prev_list.Count)
                {
                    obj_list.Add(true);

                    int list_diff_count_index = obj_list.Count;
                    int list_diff_count = 0;
                    obj_list.Add(0);

                    int count = curr_list.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        if (DiffSerialize(ref obj_list, curr_list[i], prev_list[i], i) != 0)
                            list_diff_count++;
                    }

                    if (list_diff_count > 0)
                    {
                        obj_list[list_diff_count_index] = list_diff_count;
                        diff_count++;
                    }
                    else
                    {
                        obj_list.RemoveAt(list_diff_count_index);
                        obj_list.RemoveAt(list_diff_count_index - 1);
                    }
                }
                else
                {
                    obj_list.Add(false);
                    obj_list.Add(curr_list);
                    diff_count++;
                }
            }
            #endregion

            #region Dictionary Serialize
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var curr_dict = current_value as IDictionary;
                var prev_dict = prev_value as IDictionary;

                int dict_diff_count_index = obj_list.Count;
                int dict_diff_count = 0;
                obj_list.Add(0);

                foreach (var key in curr_dict.Keys)
                {
                    if (prev_dict.Contains(key))
                    {
                        int flag_index = obj_list.Count;
                        obj_list.Add(0); // Fix Flag
                        obj_list.Add(key);
                        if (DiffSerialize(ref obj_list, curr_dict[key], prev_dict[key], -1) != 0)
                        {
                            dict_diff_count++;
                        }
                        else
                        {
                            obj_list.RemoveAt(flag_index + 1);
                            obj_list.RemoveAt(flag_index);
                        }
                        prev_dict.Remove(key);
                    }
                    else
                    {
                        obj_list.Add(1); // Add Flag
                        obj_list.Add(key);
                        obj_list.Add(curr_dict[key]);
                        dict_diff_count++;
                    }
                }

                foreach (var key in prev_dict.Keys)
                {
                    obj_list.Add(2); // Delete Flag
                    obj_list.Add(key);
                    dict_diff_count++;
                }

                if (dict_diff_count > 0)
                {
                    obj_list[dict_diff_count_index] = dict_diff_count;
                    diff_count++;
                }
                else
                {
                    obj_list.RemoveAt(dict_diff_count_index);
                }
            }
            #endregion

            #region Class & Struct Serialize
            else if (type.IsClass || type.IsValueType)
            {
                FieldInfo[] fields = type.GetFields(_flags);
                var fields_length = fields.Length;
                for (int i = 0; i < fields_length; ++i)
                {
                    object curr_field_value = fields[i].GetValue(current_value);
                    object prev_field_value = fields[i].GetValue(prev_value);

                    if (DiffSerialize(ref obj_list, curr_field_value, prev_field_value, i) != 0)
                        diff_count++;
                }
            }
            #endregion

            if (diff_count > 0)
                obj_list[diff_count_index] = diff_count;
            else
            {
                obj_list.RemoveAt(diff_count_index + 1);
                obj_list.RemoveAt(diff_count_index);
                if (parnet_field_index >= 0)
                    obj_list.RemoveAt(diff_count_index - 1);
            }

            return diff_count;
        }

        public static int DiffSerialize<T>(BinaryWriter writer, T current_value, ref T prev_value)
        {
            List<object> obj_list = new List<object>();
            DiffSerialize(ref obj_list, current_value, prev_value);

            foreach (var item in obj_list)
            {
                Serialize(writer, item);
            }

            prev_value = (T)JUtil.CloneProcedure(current_value);

            return obj_list.Count;
        }

        public static int DiffSerialize<T>(Stream stream, T current_value, ref T prev_value)
        {
            return DiffSerialize(new BinaryWriter(stream), current_value, ref prev_value);
        }
        #endregion

        #region [Variable Replication] DiffDeserialize
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object DiffDeserialize(BinaryReader reader, object target)
        {
            int diff_count = (int)Deserialize(reader);

            UpdateTypeHash();
            Type type = _type_dict[(int)Deserialize(reader)];

            if (diff_count == -1)
            {
                if (type == null)
                    return null;
                else
                    return Deserialize(reader);
            }

            if (type.IsEnum)
                type = typeof(int);

            #region Native Data Deserialize
            if (JReflection.HasReadMethod(type))
            {
                return Deserialize(reader);
            }
            #endregion

            #region List & Array Deserialize
            else if (typeof(IList).IsAssignableFrom(type))
            {
                bool isFix = (bool)Deserialize(reader);
                if (isFix)
                {
                    var target_list = target as IList;
                    int list_diff_count = (int)Deserialize(reader);
                    for (int li = 0; li < list_diff_count; ++li)
                    {
                        int list_index = (int)Deserialize(reader);
                        target_list[list_index] = DiffDeserialize(reader, target_list[list_index]);
                    }
                    return target_list;
                }
                else
                {
                    return Deserialize(reader);
                }
            }
            #endregion

            #region Dictionary Deserialize
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var target_dict = target as IDictionary;
                int dict_diff_count = (int)Deserialize(reader);
                for (int count = 0; count < dict_diff_count; ++count)
                {
                    switch ((int)Deserialize(reader))
                    {
                        case 0: // Fix
                            {
                                var key = Deserialize(reader);
                                target_dict[key] = DiffDeserialize(reader, target_dict[key]);
                            }
                            break;
                        case 1: // Add
                            {
                                target_dict.Add(Deserialize(reader), Deserialize(reader));
                            }
                            break;

                        case 2: // Delete
                            {
                                target_dict.Remove(Deserialize(reader));
                            }
                            break;

                        default:
                            {
                                throw new Exception("JNewSerialization::DiffDeserialize - IDictionary Flag Error");
                            }
                    }
                }
                return target_dict;
            }
            #endregion

            #region Class & Struct Deserialize
            else if (type.IsClass || type.IsValueType)
            {
                FieldInfo[] fields = type.GetFields(_flags);
                for (int i = 0; i < diff_count; ++i)
                {
                    int field_index = (int)Deserialize(reader);
                    var fi = fields[field_index];

                    fi.SetValue(target, DiffDeserialize(reader, fi.GetValue(target)));
                }
                return target;
            }
            #endregion

            return null;
        }

        public static object DiffDeserialize(Stream stream, object target)
        {
            return DiffDeserialize(new BinaryReader(stream), target);
        }

        public static void DiffDeserialize<T>(BinaryReader reader, ref T target)
        {
            target = (T)DiffDeserialize(reader, target);
        }

        public static void DiffDeserialize<T>(Stream stream, ref T target)
        {
            target = (T)DiffDeserialize(new BinaryReader(stream), target);
        }
        #endregion

        #region [Variable Replication] [Fast] DiffSerialize (object, fast, 추상 타입 사용 불가, null 불가)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static int FastDiffSerialize(ref List<object> obj_list, object current_value, object prev_value, Type type, int parnet_field_index = -1)
        {
            var fields = type.GetFields(_flags);
            int fields_length = fields.Length;

            if (current_value == null)
                throw new Exception("JNewSerialization::FastDiffSerialize - current_value is null");

            if (parnet_field_index != -1)
                obj_list.Add(parnet_field_index);

            int diff_count_index = obj_list.Count;
            int diff_count = 0;
            obj_list.Add(0);

            for (int field_index = 0; field_index < fields_length; ++field_index)
            {
                var curr_fieldValue = fields[field_index].GetValue(current_value);
                var prev_fieldValue = fields[field_index].GetValue(prev_value);
                var field_type = fields[field_index].FieldType;

                fields[field_index].SetValue(prev_value, curr_fieldValue);

                #region Native Data Serialize
                if (JReflection.HasWriteMethod(field_type) || field_type.IsEnum)
                {
                    if (curr_fieldValue == null)
                        throw new Exception("JNewSerialization::FastDiffSerialize - curr_fieldValue is null");

                    if (curr_fieldValue.Equals(prev_fieldValue) == false)
                    {
                        obj_list.Add(field_index);
                        obj_list.Add(curr_fieldValue);

                        diff_count++;
                    }

                    continue;
                }
                #endregion

                #region List Serialize
                // 추가, 삭제의 구분이 난해하므로, 변조에 대해서만 처리.
                // 추가/삭제 시 모든 데이터 전송.
                if (typeof(IList).IsAssignableFrom(field_type))
                {
                    int init_field_index = field_index;
                    var list_itemType = (field_type.IsArray == true) ? field_type.GetElementType() : field_type.GetGenericArguments()[0];

                    var curr_list = curr_fieldValue as IList;

                    int count = curr_list.Count;
                    if ((prev_fieldValue is IList prev_list) && (prev_list.Count == count))
                    {
                        obj_list.Add(field_index);

                        int list_diff_count_index = obj_list.Count;
                        int list_diff_count = 0;
                        obj_list.Add(0);

                        if (JReflection.HasWriteMethod(list_itemType))
                        {
                            for (int i = 0; i < count; ++i)
                            {
                                if (curr_list[i] == null)
                                    throw new Exception("JNewSerialization::FastDiffSerialize - curr_list[i] is null");

                                if (curr_list[i].Equals(prev_list[i]) == false)
                                {
                                    obj_list.Add(i);
                                    obj_list.Add(curr_list[i]);

                                    list_diff_count++;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < count; ++i)
                            {
                                if (FastDiffSerialize(ref obj_list, curr_list[i], prev_list[i], list_itemType, i) != 0)
                                    list_diff_count++;
                            }
                        }

                        if (list_diff_count > 0)
                        {
                            obj_list[list_diff_count_index] = list_diff_count;
                            diff_count++;
                        }
                        else
                        {
                            obj_list.RemoveAt(list_diff_count_index);
                            obj_list.RemoveAt(list_diff_count_index - 1);
                        }
                    }
                    else
                    {
                        obj_list.Add(field_index);
                        obj_list.Add(-1);
                        obj_list.Add(count);

                        for (int i = 0; i < count; ++i)
                        {
                            if (curr_list[i] == null)
                                throw new Exception("JNewSerialization::FastDiffSerialize - curr_list[i] is null");

                            obj_list.Add(curr_list[i]);
                        }

                        diff_count++;
                    }

                    continue;
                }
                #endregion

                #region Dictionary Serialize
                if (typeof(IDictionary).IsAssignableFrom(field_type))
                {
                    var curr_dict = curr_fieldValue as IDictionary;
                    var prev_dict = prev_fieldValue as IDictionary;

                    obj_list.Add(field_index);

                    int dict_diff_count_index = obj_list.Count;
                    int dict_diff_count = 0;
                    obj_list.Add(0);

                    var value_type = field_type.GetGenericArguments()[1];

                    if (JReflection.HasWriteMethod(value_type))
                    {
                        foreach (var key in curr_dict.Keys)
                        {
                            if (prev_dict.Contains(key))
                            {
                                if (curr_dict[key] == null)
                                    throw new Exception("JNewSerialization::FastDiffSerialize - curr_dict[key] is null");

                                int flag_index = obj_list.Count;
                                obj_list.Add(0); // Fix Flag

                                if (curr_dict[key].Equals(prev_dict[key]) == false)
                                {
                                    obj_list.Add(key);
                                    obj_list.Add(curr_dict[key]);

                                    dict_diff_count++;
                                }
                                else
                                {
                                    obj_list.RemoveAt(flag_index);
                                }
                                prev_dict.Remove(key);
                            }
                            else
                            {
                                obj_list.Add(1); // Add Flag

                                obj_list.Add(key);
                                obj_list.Add(curr_dict[key]);

                                dict_diff_count++;
                            }
                        }

                        foreach (var key in prev_dict.Keys)
                        {
                            obj_list.Add(2); // Delete Flag
                            obj_list.Add(key);
                            dict_diff_count++;
                        }
                    }
                    else
                    {
                        foreach (var key in curr_dict.Keys)
                        {
                            if (prev_dict.Contains(key))
                            {
                                int flag_index = obj_list.Count;
                                obj_list.Add(0); // Fix Flag
                                obj_list.Add(key);

                                if (FastDiffSerialize(ref obj_list, curr_dict[key], prev_dict[key], value_type, -1) != 0)
                                {
                                    dict_diff_count++;
                                }
                                else
                                {
                                    obj_list.RemoveAt(flag_index + 1);
                                    obj_list.RemoveAt(flag_index);
                                }
                                prev_dict.Remove(key);
                            }
                            else
                            {
                                obj_list.Add(1); // Add Flag

                                obj_list.Add(key);
                                obj_list.Add(curr_dict[key]);

                                dict_diff_count++;
                            }
                        }

                        foreach (var key in prev_dict.Keys)
                        {
                            obj_list.Add(2); // Delete Flag
                            obj_list.Add(key);
                            dict_diff_count++;
                        }
                    }


                    if (dict_diff_count > 0)
                    {
                        obj_list[dict_diff_count_index] = dict_diff_count;
                        diff_count++;
                    }
                    else
                    {
                        obj_list.RemoveAt(dict_diff_count_index);
                        obj_list.RemoveAt(dict_diff_count_index - 1);
                    }

                    continue;
                }
                #endregion

                #region Class & Struct Serialize
                if (FastDiffSerialize(ref obj_list, curr_fieldValue, prev_fieldValue, field_type, field_index) != 0)
                    diff_count++;
                #endregion
            }


            if (diff_count > 0)
            {
                obj_list[diff_count_index] = diff_count;
            }
            else
            {
                obj_list.RemoveAt(diff_count_index);
                if (parnet_field_index >= 0)
                    obj_list.RemoveAt(diff_count_index - 1);
            }

            return diff_count;
        }

        public static int FastDiffSerialize<T>(BinaryWriter writer, T current_value, ref T prev_value)
        {
            Type type = typeof(T);
            if (JReflection.HasWriteMethod(type) || type.IsEnum)
            {
                if (current_value.Equals(prev_value) == false)
                {
                    prev_value = current_value;
                    FastSerialize(writer, current_value);
                    return 1;
                }

                return 0;
            }

            List<object> obj_list = new List<object>();
            FastDiffSerialize(ref obj_list, current_value, prev_value, type);

            foreach (var item in obj_list)
            {
                FastSerialize(writer, item);
            }

            return obj_list.Count;
        }
        public static int FastDiffSerialize<T>(Stream stream, T current_value, ref T prev_value)
        {
            return FastDiffSerialize<T>(new BinaryWriter(stream), current_value, ref prev_value);
        }
        #endregion

        #region [Variable Replication] [Fast] DiffDeserialize (object, fast, 추상 타입 사용 불가, null 불가)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void FastDiffDeserialize<T>(BinaryReader reader, Type type, ref T target)
        {
            if (JReflection.HasReadMethod(type))
            {
                target = (T)FastDeserialize(reader, type);
                return;
            }

            var fields = type.GetFields(_flags);
            int diff_count = (int)FastDeserialize(reader, typeof(int));

            for (int i = 0; i < diff_count; ++i)
            {
                int field_index = (int)FastDeserialize(reader, typeof(int));

                var fi = fields[field_index];
                var field_type = fi.FieldType;

                #region Native Data Deserialize
                if (JReflection.HasReadMethod(field_type) || field_type.IsEnum)
                {
                    fi.SetValue(target, FastDeserialize(reader, field_type));
                    continue;
                }
                #endregion

                #region List & Array Deserialize
                if (typeof(IList).IsAssignableFrom(field_type))
                {
                    int list_diff_count = (int)FastDeserialize(reader, typeof(int));

                    var list_ref_target = fi.GetValue(target);
                    var target_list = list_ref_target as IList;
                    var list_itemType = (field_type.IsArray == true) ? field_type.GetElementType() : field_type.GetGenericArguments()[0];

                    if (list_diff_count != -1)
                    {
                        if (JReflection.HasReadMethod(field_type))
                        {
                            for (int li = 0; li < list_diff_count; ++li)
                            {
                                int list_index = (int)FastDeserialize(reader, typeof(int));
                                target_list[list_index] = FastDeserialize(reader, list_itemType);
                            }
                        }
                        else
                        {
                            for (int li = 0; li < list_diff_count; ++li)
                            {
                                int list_index = (int)FastDeserialize(reader, typeof(int));
                                object read_object = target_list[list_index];
                                FastDiffDeserialize(reader, list_itemType, ref read_object);
                                target_list[list_index] = read_object;
                            }
                        }
                    }
                    else
                    {
                        target_list.Clear();

                        int list_count = (int)FastDeserialize(reader, typeof(int));
                        for (int li = 0; li < list_count; ++li)
                        {
                            target_list.Add(FastDeserialize(reader, list_itemType));
                        }
                    }

                    continue;
                }
                #endregion

                #region Dictionary Deserialize
                if (typeof(IDictionary).IsAssignableFrom(field_type))
                {
                    int dict_diff_count = (int)FastDeserialize(reader, typeof(int));

                    var dict_ref_target = fi.GetValue(target);
                    var target_dict = dict_ref_target as IDictionary;

                    var key_type = field_type.GetGenericArguments()[0];
                    var value_type = field_type.GetGenericArguments()[1];

                    if (JReflection.HasWriteMethod(value_type))
                    {
                        for (int count = 0; count < dict_diff_count; ++count)
                        {
                            switch ((int)FastDeserialize(reader, typeof(int)))
                            {
                                case 0: // Fix
                                    {
                                        target_dict[FastDeserialize(reader, key_type)] = FastDeserialize(reader, value_type);
                                    }
                                    break;
                                case 1: // Add
                                    {
                                        target_dict.Add(FastDeserialize(reader, key_type), FastDeserialize(reader, value_type));
                                    }
                                    break;

                                case 2: // Delete
                                    {
                                        target_dict.Remove(FastDeserialize(reader, key_type));
                                    }
                                    break;

                                default:
                                    {
                                        throw new Exception("JNewSerialization::FastDiffDeserialize - IDictionary Flag Error");
                                    }
                            }
                        }
                    }
                    else
                    {
                        for (int count = 0; count < dict_diff_count; ++count)
                        {
                            switch ((int)FastDeserialize(reader, typeof(int)))
                            {
                                case 0: // Fix
                                    {
                                        var key = FastDeserialize(reader, key_type);
                                        var value = target_dict[key];
                                        FastDiffDeserialize(reader, value_type, ref value);
                                        target_dict[key] = value;
                                    }
                                    break;
                                case 1: // Add
                                    {
                                        target_dict.Add(FastDeserialize(reader, key_type), FastDeserialize(reader, value_type));
                                    }
                                    break;

                                case 2: // Delete
                                    {
                                        target_dict.Remove(FastDeserialize(reader, key_type));
                                    }
                                    break;

                                default:
                                    {
                                        throw new Exception("JNewSerialization::FastDiffDeserialize - IDictionary Flag Error");
                                    }
                            }
                        }
                    }

                    continue;
                }
                #endregion

                #region Class & Struct Deserialize
                object ref_target = fi.GetValue(target);
                FastDiffDeserialize(reader, field_type, ref ref_target);
                fi.SetValue(target, ref_target);
                #endregion
            }
        }

        public static void FastDiffDeserialize<T>(Stream stream, Type type, ref T target)
        {
            FastDiffDeserialize(new BinaryReader(stream), type, ref target);
        }

        public static void FastDiffDeserialize<T>(BinaryReader reader, ref T target)
        {
            FastDiffDeserialize(reader, typeof(T), ref target);
        }

        public static void FastDiffDeserialize<T>(Stream stream, ref T target)
        {
            FastDiffDeserialize(new BinaryReader(stream), typeof(T), ref target);
        }
        #endregion

    }
}
