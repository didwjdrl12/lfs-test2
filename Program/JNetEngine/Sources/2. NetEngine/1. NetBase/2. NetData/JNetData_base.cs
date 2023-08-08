using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using J2y.Network;
using System.Text;
using System.Linq;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JNetData
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    [Serializable]
    public class JNetData : ICloneable
    {
       public static readonly Encoding Encoding = Encoding.UTF8;
		public static readonly Encoding EncodingUnicode = Encoding.Unicode;
		public static readonly Encoding EncodingASCII = Encoding.ASCII;

		#region [베이스] Clone
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual object Clone()
		{
			return MemberwiseClone();
		}
		#endregion



		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// [IO 쓰레드] 메시지 패킹/파싱
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [메시지] Read/Write
		//----------------------------------------------------------------
		public virtual void Read(BinaryReader reader)
        {
			ReadPlaneStructure(reader);
		}
        //----------------------------------------------------------------
        public virtual void Write(BinaryWriter writer)
        {
			WritePlaneStructure(writer);
		}
		#endregion


		#region [메시지] PlaneStructure
		//----------------------------------------------------------------
		public virtual void ReadPlaneStructure(BinaryReader reader)
		{
			// 보류: (todo)모든 필드들 순회하면서 값 쓰기
			var instance = this;
			var fields = JReflection
				.FindAllFields(instance)
				.Where(f => !Attribute.IsDefined(f, typeof(NonSerializedAttribute)))
				.OrderBy(field => field.MetadataToken);

			foreach (var fi in fields)
			{
				var type = fi.FieldType;
				if (type == typeof(string))
				{
					fi.SetValue(instance, ReadString(reader));
				}
				else if (type == typeof(int))
				{
					fi.SetValue(instance, ReadInt32(reader));
				}
				else if (type == typeof(short))
				{
					fi.SetValue(instance, ReadInt16(reader));
				}
				else if (type == typeof(double))
				{
					fi.SetValue(instance, ReadDouble(reader));
				}
				else if (type == typeof(float))
				{
					fi.SetValue(instance, ReadSingle(reader));
				}
				else if (type.IsEnum)
				{
					var enum_inst = Activator.CreateInstance(type);
					enum_inst = type.GetEnumValues().GetValue(ReadInt32(reader));
					fi.SetValue(instance, enum_inst);
				}
			}
		}
		#endregion

		#region [IO 쓰레드] 메시지 파싱
		//----------------------------------------------------------------
		public virtual void WritePlaneStructure(BinaryWriter writer)
		{
			// 보류: JSerialization.Serialize(writer, this);

			// 모든 필드들 순회하면서 값 쓰기
			var instance = this;
			var fields = JReflection
				.FindAllFields(instance)
				.Where(f => !Attribute.IsDefined(f, typeof(NonSerializedAttribute)))
				.OrderBy(field => field.MetadataToken);

			foreach (var fi in fields)
			{
				var type = fi.FieldType;
				if (type == typeof(string))
				{
					Write(writer, (string)fi.GetValue(instance));
				}
				else if (type == typeof(int))
				{
					Write(writer, (int)fi.GetValue(instance));
				}
				else if (type == typeof(short))
				{
					Write(writer, (short)fi.GetValue(instance));
				}
				else if (type == typeof(double))
				{
					Write(writer, (double)fi.GetValue(instance));
				}
				else if (type == typeof(float))
				{
					Write(writer, (float)fi.GetValue(instance));
				}
				else if (type.IsEnum)
				{
					Write(writer, (int)fi.GetValue(instance));
				}

			}
		}
		#endregion

		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 유틸
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [유틸] TempBuffer
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		private static byte[] s_tempBuffer4 = new byte[4];
		private static byte[] s_tempBuffer8 = new byte[8];
		#endregion

		#region [유틸] [Stream] String
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static string ReadString(BinaryReader reader)
		{
			//try
			{
				var stringLength = ReadInt32(reader);
				
				var buffer = new byte[stringLength];
				reader.Read(buffer, 0, stringLength);
				return Encoding.GetString(buffer);
			}
			//catch (Exception)
			//{
			//	return "";
			//}
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static string ReadStringAscii(BinaryReader reader)
		{
			//try
			{
				var stringLength = ReadInt32(reader);

				var buffer = new byte[stringLength];
				reader.Read(buffer, 0, stringLength);
				return EncodingASCII.GetString(buffer);
			}
			//catch (Exception)
			//{
			//	return "";
			//}
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static void Write(BinaryWriter writer, string text)
		{
			if (null == text)
				text = "";

			var buffer = Encoding.GetBytes(text);
				Write(writer, buffer.Length);

			writer.Write(buffer, 0, buffer.Length);
		}
		#endregion

		#region [유틸] [Stream] FixedString
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static string ReadFixedString(BinaryReader reader, int size)
		{
			//try
			{				
				var buffer = new byte[size];
				reader.Read(buffer, 0, size);
				return EncodingUnicode.GetString(buffer);
			}
			//catch (Exception)
			//{
			//	return "";
			//}
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static void WriteFixedString(BinaryWriter writer, string text, int size)
		{
			var buffer = new byte[size];
			EncodingUnicode.GetBytes(text, 0, text.Length, buffer, 0);
			writer.Write(buffer, 0, buffer.Length);
		}
		#endregion

		


		#region [유틸] [Stream] Int16
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static short ReadInt16(BinaryReader reader)
		{
			int first = reader.ReadByte();
			return (short)(reader.ReadByte() | first << 8);
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static void Write(BinaryWriter writer, short int16)
		{
			byte[] bytes = BitConverter.GetBytes(int16);
			writer.Write(bytes, 1, 1);
			writer.Write(bytes, 0, 1);
		}
		#endregion

		#region [유틸] [Stream] Int32
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static int ReadInt32(BinaryReader reader)
		{
			int first = reader.ReadByte();
			int second = reader.ReadByte();
			int third = reader.ReadByte();
			return (reader.ReadByte() | third << 8 | second << 16 | first << 24);
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static void Write(BinaryWriter writer, int int32)
		{
			var bytes = BitConverter.GetBytes(int32);
			writer.Write(bytes, 3, 1);
			writer.Write(bytes, 2, 1);
			writer.Write(bytes, 1, 1);
			writer.Write(bytes, 0, 1);
		}
		#endregion

		#region [유틸] [Stream] Int64
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static int ReadInt64(BinaryReader reader)
		{
			int b1 = reader.ReadByte();
			int b2 = reader.ReadByte();
			int b3 = reader.ReadByte();
			int b4 = reader.ReadByte();
			int b5 = reader.ReadByte();
			int b6 = reader.ReadByte();
			int b7 = reader.ReadByte();
			int b8 = reader.ReadByte();
			return (b8 | b7 << 8 | b6 << 16 | b5 << 24 | b4 << 32 | b3 << 40 | b2 << 48 | b1 << 56);
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static void Write(BinaryWriter writer, long int64)
		{
			var bytes = BitConverter.GetBytes(int64);
			writer.Write(bytes, 7, 1);
			writer.Write(bytes, 6, 1);
			writer.Write(bytes, 5, 1);
			writer.Write(bytes, 4, 1);
			writer.Write(bytes, 3, 1);
			writer.Write(bytes, 2, 1);
			writer.Write(bytes, 1, 1);
			writer.Write(bytes, 0, 1);
		}
		#endregion

		#region [유틸] [Stream] Single
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static float ReadSingle(BinaryReader reader)
		{
			s_tempBuffer4[3] = reader.ReadByte();
			s_tempBuffer4[2] = reader.ReadByte();
			s_tempBuffer4[1] = reader.ReadByte();
			s_tempBuffer4[0] = reader.ReadByte();
			return BitConverter.ToSingle(s_tempBuffer4, 0);
		}

		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static void Write(BinaryWriter writer, float real)
		{
			var bytes = BitConverter.GetBytes(real);
			writer.Write(bytes, 3, 1);
			writer.Write(bytes, 2, 1);
			writer.Write(bytes, 1, 1);
			writer.Write(bytes, 0, 1);
		}
		#endregion

		#region [유틸] [Stream] Double
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static double ReadDouble(BinaryReader reader)
		{
			s_tempBuffer8[7] = reader.ReadByte();
			s_tempBuffer8[6] = reader.ReadByte();
			s_tempBuffer8[5] = reader.ReadByte();
			s_tempBuffer8[4] = reader.ReadByte();
			s_tempBuffer8[3] = reader.ReadByte();
			s_tempBuffer8[2] = reader.ReadByte();
			s_tempBuffer8[1] = reader.ReadByte();
			s_tempBuffer8[0] = reader.ReadByte();
			return BitConverter.ToDouble(s_tempBuffer8, 0);
		}

		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static void Write(BinaryWriter writer, double doub)
		{
			var bytes = BitConverter.GetBytes(doub);
			writer.Write(bytes, 7, 1);
			writer.Write(bytes, 6, 1);
			writer.Write(bytes, 5, 1);
			writer.Write(bytes, 4, 1);
			writer.Write(bytes, 3, 1);
			writer.Write(bytes, 2, 1);
			writer.Write(bytes, 1, 1);
			writer.Write(bytes, 0, 1);
		}
		#endregion
		
		#region [유틸] Array
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public void ReadArray<T>(BinaryReader reader, ref T[] buffer) where T : JNetData, new()
		{
			var size = ReadInt32(reader);
			buffer = new T[size];
			for (int i = 0; i < buffer.Length; i++)
			{
				var data = new T();
				data.Read(reader);
				buffer[i] = data;
			}
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public void WriteArray<T>(BinaryWriter writer, T[] buffer) where T : JNetData
		{
			var size = (buffer != null) ? buffer.Length : 0;
			writer.Write(size.ToBigEndian());
			for (int i = 0; i < size; i++)
			{
				buffer[i].Write(writer);
			}
		}
		#endregion

		#region [유틸] List
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static void ReadList<T>(BinaryReader reader, ref List<T> list) where T : JNetData, new()
		{
			var size = ReadInt32(reader);
			list = new List<T>();
			for (int i = 0; i < size; i++)
			{
				var data = new T();
				data.Read(reader);
				list.Add(data);
			}
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static void WriteList<T>(BinaryWriter writer, IList<T> buffer) where T : JNetData
		{
			var size = (buffer != null) ? buffer.Count : 0;
			writer.Write(size.ToBigEndian());
			for (int i = 0; i < size; i++)
			{
				buffer[i].Write(writer);
			}
		}
		#endregion


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 유틸
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [유틸] 파싱
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ParseArray<T>(BinaryReader reader, ref T[] buffer, bool null_check = false) where T : JNetData, new()
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                bool read_data = true;
                if (null_check)
                    read_data = reader.ReadBoolean();

                if (read_data)
                {
                    var data = new T();
                    data.Read(reader);
                    buffer[i] = data;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ParseList<T>(BinaryReader reader, List<T> buffer) where T : JNetData, new()
        {
            buffer.Clear();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var data = new T();
                data.Read(reader);
                buffer.Add(data);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ParseDictionaryLong<T2>(BinaryReader reader, out IDictionary<long, T2> buffer) where T2 : JNetData, new()
        {
            buffer = new Dictionary<long, T2>();
            buffer.Clear();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var key = reader.ReadInt64();
                var data = new T2();
                data.Read(reader);
                buffer.Add(key, data);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ParseDictionaryInt<T2>(BinaryReader reader, out IDictionary<int, T2> buffer) where T2 : JNetData, new()
        {
            buffer = new Dictionary<int, T2>();
            buffer.Clear();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var key = reader.ReadInt32();
                var data = new T2();
                data.Read(reader);
                buffer.Add(key, data);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ParseDictionaryString<T2>(BinaryReader reader, out IDictionary<string, T2> buffer) where T2 : JNetData, new()
        {
            buffer = new Dictionary<string, T2>();
            buffer.Clear();
            int count = reader.ReadInt16();
            for (int i = 0; i < count; i++)
            {
                var key = reader.ReadString();
                var data = new T2();
                data.Read(reader);
                buffer.Add(key, data);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ParseIntArray(BinaryReader reader, ref int[] buffer)
        {
            for (int i = 0; i < buffer.Length; ++i)
                buffer[i] = reader.ReadInt32();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ParseLongArray(BinaryReader reader, ref long[] buffer)
        {
            for (int i = 0; i < buffer.Length; ++i)
                buffer[i] = reader.ReadInt64();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ParseIntList(BinaryReader reader, List<int> buffer)
        {
            buffer.Clear();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                buffer.Add(reader.ReadInt32());
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ParseLongList(BinaryReader reader, List<long> buffer)
        {
            buffer.Clear();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                buffer.Add(reader.ReadInt64());
        }
        #endregion

        #region [유틸] 패킹
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void PackingArray(BinaryWriter writer, JNetData[] buffer, bool null_check = false)
        {
            foreach (var data in buffer)
            {
                if (null_check)
                    writer.Write(data != null);
                if (data != null)
                    data.Write(writer);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void PackingList<T>(BinaryWriter writer, List<T> buffer) where T : JNetData
        {
            writer.Write(buffer.Count);
            foreach (var data in buffer)
                data.Write(writer);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void PackingDictionaryLong<T2>(BinaryWriter writer, IDictionary<long, T2> buffer)
            where T2 : JNetData
        {
            writer.Write(buffer.Count);
            foreach (var data in buffer)
            {
                writer.Write(data.Key);
                data.Value.Write(writer);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void PackingDictionaryInt<T2>(BinaryWriter writer, IDictionary<int, T2> buffer)
            where T2 : JNetData
        {
            writer.Write(buffer.Count);
            foreach (var data in buffer)
            {
                writer.Write(data.Key);
                data.Value.Write(writer);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void PackingDictionaryString<T2>(BinaryWriter writer, IDictionary<string, T2> buffer)
            where T2 : JNetData
        {
            writer.Write(buffer.Count);
            foreach (var data in buffer)
            {
                writer.Write(data.Key);
                data.Value.Write(writer);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void PackingIntArray(BinaryWriter writer, int[] buffer)
        {
            foreach (var data in buffer)
                writer.Write(data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void PackingLongArray(BinaryWriter writer, long[] buffer)
        {
            foreach (var data in buffer)
                writer.Write(data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void PackingIntList(BinaryWriter writer, List<int> buffer)
        {
            writer.Write(buffer.Count);
            foreach (var data in buffer)
                writer.Write(data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void PackingLongList(BinaryWriter writer, List<long> buffer)
        {
            writer.Write(buffer.Count);
            foreach (var data in buffer)
                writer.Write(data);
        }
        #endregion


        #region [유틸] Vector2, Vector3
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Write(BinaryWriter writer, Vector3 v)
        {
            writer.Write(v.x);
            writer.Write(v.y);
            writer.Write(v.z);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Write(BinaryWriter writer, Vector2 v)
        {
            writer.Write(v.x);
            writer.Write(v.y);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Write(BinaryWriter writer, Quaternion q)
        {
            writer.Write(q.x);
            writer.Write(q.y);
            writer.Write(q.z);
            writer.Write(q.w);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vector3 ReadVector3(BinaryReader recv_msg)
        {
            var v = new Vector3();
            v.x = recv_msg.ReadSingle();
            v.y = recv_msg.ReadSingle();
            v.z = recv_msg.ReadSingle();
            return v;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vector2 ReadVector2(BinaryReader recv_msg)
        {
            var v = new Vector2();
            v.x = recv_msg.ReadSingle();
            v.y = recv_msg.ReadSingle();
            return v;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Quaternion ReadQuaternion(BinaryReader recv_msg)
        {
            var q = new Quaternion();
            q.x = recv_msg.ReadSingle();
            q.y = recv_msg.ReadSingle();
            q.z = recv_msg.ReadSingle();
            q.w = recv_msg.ReadSingle();
            return q;
        }
        #endregion

	}


}
