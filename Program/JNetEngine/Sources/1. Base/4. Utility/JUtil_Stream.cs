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
        // Serialize
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Serialize] XML
        //------------------------------------------------------------------------------------------------------------------------------------------------------	
        public static void Serialize(string filename, System.Object o)
        {
            var writer = new XmlSerializer(o.GetType());
            var wfile = new System.IO.StreamWriter(filename);
            writer.Serialize(wfile, o);
            wfile.Close();
        }
        #endregion

        #region [Deserialize] XML
        //------------------------------------------------------------------------------------------------------------------------------------------------------	
        public static T Deserialize<T>(string filename)
        {
            var reader = new XmlSerializer(typeof(T));
            var file = new StreamReader(filename);
            var obj = (T)reader.Deserialize(file);
            file.Close();
            return obj;
        }
        #endregion

        #region EMail 전송
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SendEMail(string mail_sender, string mail_receiver, string subject, string body)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.To.Add(mail_receiver);
            message.Subject = subject;
            message.From = new System.Net.Mail.MailAddress(mail_sender);
            message.Body = body;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("yoursmtphost");
            smtp.Send(message);
        }
        #endregion

        #region TryGetType
        public static Type TryGetType(string typeName)
        {
            var getType = Type.GetType(typeName);

            if (getType == null)
                getType = FindType(typeName);

            return getType;
        }

        public static Type FindType(string typeName)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var getType = a.GetType(typeName);
                if (getType != null)
                    return getType;
            }
            return null;
        }
        #endregion

        #region NameToType
        public static Type NameToType(string typeName)
        {
            var getType = Type.GetType(typeName);

            if (getType == null)
                getType = FindTypeByName(typeName);

            return getType;
        }

        public static Type FindTypeByName(string typeName)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = a.GetTypes();
                foreach (var type in types)
                {
                    if (type.Name == typeName)
                        return type;
                }
            }
            return null;
        }
        #endregion

        #region MakeGenericTypeName
        public static Type MakeGenericTypeName(Type type)
        {
            return JUtil.TryGetType(type.Namespace + '.' + type.Name);
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [StreamHelper]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [StreamHelper] SerializableStream
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static byte[] MakeSerializableStream(Action<MemoryStream> fun)
        {
            //var formatter = new BinaryFormatter();
            byte[] result_buffer;
            using (var stream = new MemoryStream(1024 * 1024))
            {
                fun(stream);
                result_buffer = stream.GetBuffer();
                //Console.WriteLine("serializable:" + stream.Position);
            }
            return result_buffer;
        }
        #endregion

        #region [StreamHelper] DeserializableStream
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void MakeDeserializableStream(byte[] buffer, Action<MemoryStream> fun)
        {
            //var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(buffer))
            {
                fun(stream);
                //Console.WriteLine("serializable:" + stream.Position);
            }
        }
        #endregion
        
        #region [StreamHelper] SerializableStream
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static byte[] MakeSerializableWriter(Action<BinaryWriter> fun)
        {
            //var formatter = new BinaryFormatter();
            byte[] result_buffer;
            using (var stream = new MemoryStream(1024 * 1024))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    fun(writer);
                }

                result_buffer = stream.GetBuffer();
            }
            return result_buffer;
        }
        #endregion

        #region [StreamHelper] DeserializableStream
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void MakeDeserializableReader(byte[] buffer, Action<BinaryReader> fun)
        {
            //var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(buffer))
            {
                using (var reader = new BinaryReader(stream))
                {
                    fun(reader);
                }
                //Console.WriteLine("serializable:" + stream.Position);
            }
        }
        #endregion

    }



//}
