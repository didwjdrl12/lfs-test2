using J2y.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JObject : 5. Serialize
    //
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

#if NET_SERVER
    public partial class JObject
#else
    public partial class JObject : MonoBehaviour
#endif
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. [Serialization]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Serialization] Serialize/Deserialize
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Serialize(BinaryWriter writer)
        {
            SerializeJSerializableFields(writer);
            SerializeJSerializableProperties(writer);
        }
        public virtual void Deserialize(BinaryReader reader)
        {
            DeserializeJSerializableFields(reader);
            DeserializeJSerializableProperties(reader);
        }
        #endregion

        #region [JSerializable] Fields
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SerializeJSerializableFields(BinaryWriter writer)
        {
            JReflection.FindAllFields(this)
                .Where(f => f
                    .GetCustomAttributes()
                    .Any(attr => attr is JSerializable))
                .ForEach(fi =>
                {
                    //var attr = fi.GetCustomAttributes().FirstOrDefault(att => att is JSerializable) as JSerializable;
                    //if(!attr.Deprecated)
                    JNewSerialization.Serialize(writer, fi.GetValue(this));
                });
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DeserializeJSerializableFields(BinaryReader reader)
        {
            JReflection.FindAllFields(this)
                .Where(f => f
                    .GetCustomAttributes()
                    .Any(attr => attr is JSerializable))
                .ForEach(fi =>
                {
                    //var attr = fi.GetCustomAttributes().FirstOrDefault(att => att is JSerializable) as JSerializable;
                    // if (attr.Version >= JObject._current_engine_version)
                    fi.SetValue(this, JNewSerialization.Deserialize(reader));
                });
        }
        #endregion

        #region [JSerializable] Properties
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SerializeJSerializableProperties(BinaryWriter writer)
        {
            JReflection.FindAllProperties(this)
                .Where(f => f
                    .GetCustomAttributes()
                    .Any(attr => attr is JSerializable))
                .ForEach(fi =>
                {
                    JNewSerialization.Serialize(writer, fi.GetValue(this));
                });
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DeserializeJSerializableProperties(BinaryReader reader)
        {
            JReflection.FindAllProperties(this)
                .Where(f => f
                    .GetCustomAttributes()
                    .Any(attr => attr is JSerializable))
                .ForEach(fi =>
                {
                    fi.SetValue(this, JNewSerialization.Deserialize(reader));
                });
        }
        #endregion

                        
        #region [Serialization] [유틸] GetSerializeName/Path/Extension
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public string GetSerializeName() { return JObjectManager.GetSerializeName(GetType()); }       
        //public string GetDefaultPath() { return JObjectManager.GetDefaultPath(GetType()); }       
        //public string GetDefaultExtension() { return JObjectManager.GetDefaultExtension(GetType()); }
        #endregion

        


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 2. Clone / DeepCopy
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Clone] Server Only        
#if NET_SERVER
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public object Clone()
        {
            using (var ms = new MemoryStream())
            {
                var writer = new BinaryWriter(ms);
                JObjectManager.Serialize(writer, this);

                ms.Position = 0;
                var reader = new BinaryReader(ms);
                return JObjectManager.Deserialize(reader, null);
            }
        }
        public static T Clone<T>(T obj) where T : JObject { return obj?.Clone() as T; }
        public T Clone<T>() where T : JObject { return Clone() as T; }
#endif
        #endregion





        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 3. [NetSerialization]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [백업] [NetSerialization]
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public virtual void NetSerialize(BinaryWriter writer) { }

        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static void NetSerialize<T>(BinaryWriter writer, T obj) where T : JObject
        //{
        //    writer.Write(obj._guid);
        //    obj.NetSerialize(writer);
        //}
        //#endregion

        //#region [NetDeserialization]
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public virtual void NetDeserialize(BinaryReader reader) { }

        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static T NetDeserialize<T>(BinaryReader reader, T target = null) where T : JObject
        //{
        //    var read_guid = reader.ReadInt64();
        //    if (target == null)
        //    {
        //        var find_obj = JObjectManager.Find(read_guid);
        //        if (find_obj == null)
        //        {
        //            target = JObjectManager.Create<T>();
        //            JObjectManager.ResetGuid(target, read_guid);
        //        }
        //        else
        //        {
        //            target = find_obj as T;
        //        }
        //    }
        //    else
        //    {
        //        JObjectManager.ResetGuid(target, read_guid);
        //    }

        //    target.NetDeserialize(reader);
        //    return target;
        //}
        #endregion
    }
}
