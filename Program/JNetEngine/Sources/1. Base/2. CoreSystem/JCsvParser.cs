using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Reflection;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JCsvParser
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JCsvParser
    {
        public static string s_data_path = "DataInfo/";
        public static Dictionary<string, string[]> _textfile_attributes = new Dictionary<string, string[]>();

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 파일 파싱
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [파일파싱] [Load]
        //------------------------------------------------------------------------------------------------------------------------------------------------------

        #region [내부]
        public static IDictionary<T1, T2> Load<T1, T2>(string fileName, string path = null, bool auto = false) where T2 : JCsvData_base, new()
        {
            var fileFullPath = (path ?? s_data_path) + fileName;
            //var txtFile = VcResourceManager.Instance.LoadAsset<TextAsset>("db.unity3d", fileName, s_data_path);
            var full_text = "";
#if NET_SERVER
            fileFullPath += ".txt";
            full_text = File.ReadAllText(fileFullPath);
#else
            var txtFile = Resources.Load(fileFullPath, typeof(TextAsset)) as TextAsset;
			full_text = txtFile.text;
#endif

            var reader = new StringReader(full_text);
            if (reader == null)
            {
                JLogger.WriteError("File not found or not readable : " + fileFullPath);
                return null;
            }

            int lineCount = 0;
            var inputData = reader.ReadLine();
            var res_datas = new Dictionary<T1, T2>();

            while (inputData != null)
            {

                //-------------------------------------------------------------------
                // 1. 토큰 분리 및 해더 확인

                var stringList = inputData.Split('\t');
                if (stringList.Length == 0)
                    continue;
                if (lineCount == 0)
                {
                    var header = inputData;
                    if (_textfile_attributes.ContainsKey(fileName) == false)
                        _textfile_attributes.Add(fileName, header.Split('\t'));
                    lineCount++;
                    inputData = reader.ReadLine();                   
                    continue;
                }
                for (int i = 0; i < stringList.Length; i++)
                    stringList[i] = stringList[i].Replace("\\n", "\n");     // \n문자를 \\n으로 넣는 TextAsset을 위한 수정..


                //-------------------------------------------------------------------
                // 2. 데이터 라인 파싱

                var data_line = new T2();
                if (data_line.VarifyKey(stringList[0]) == false)
                {
                    JLogger.Write("VarifyKey fail : " + inputData[0]);
                    continue;
                }

                if(!auto)
                    data_line.Parse(stringList);
                else
                    data_line.AutoParse(stringList);

                res_datas.Add((T1)Convert.ChangeType(stringList[0], typeof(T1)), data_line);
                inputData = reader.ReadLine();
                lineCount++;
            }

            JLogger.WriteFormat("[DataTable] [{0} Count]{1}", lineCount, fileName);

            reader.Dispose();
            reader.Close();

            //foreach (var data in res_datas.Values)
            //    data.OnLoad();

            return res_datas;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static IDictionary<T1, T2> LoadAttribute<T1, T2>(string fileName, string path = null) where T2 : JCsvData_base, new()
        {
            var fileFullPath = path ?? s_data_path + fileName;
            //var txtFile = VcResourceManager.Instance.LoadAsset<TextAsset>("db.unity3d", fileName, s_data_path);
            var full_text = "";
#if NET_SERVER
            fileFullPath += ".txt";
            full_text = File.ReadAllText(fileFullPath);
#else
            var txtFile = Resources.Load(fileFullPath, typeof(TextAsset)) as TextAsset;
			full_text = txtFile.text;
#endif

            var reader = new StringReader(full_text);
            if (reader == null)
            {
                JLogger.WriteError("File not found or not readable : " + fileFullPath);
                return null;
            }

            int lineCount = 0;
            var inputData = reader.ReadLine();
            var res_datas = new Dictionary<T1, T2>();
            string[] attribute = null;
            while (inputData != null)
            {

                //-------------------------------------------------------------------
                // 1. 토큰 분리 및 해더 확인

                var stringList = inputData.Split('\t');
                if (stringList.Length == 0)
                    continue;
                if (lineCount == 0)
                {
                    var header = inputData;
                    lineCount++;
                    inputData = reader.ReadLine();
                    attribute = header.Split('\t');
                    continue;
                }
                for (int i = 0; i < stringList.Length; i++)
                    stringList[i] = stringList[i].Replace("\\n", "\n");     // \n문자를 \\n으로 넣는 TextAsset을 위한 수정..


                //-------------------------------------------------------------------
                // 2. 데이터 라인 파싱

                var data_line = new T2();
                if (data_line.VarifyKey(stringList[0]) == false)
                {
                    JLogger.Write("VarifyKey fail : " + inputData[0]);
                    continue;
                }

                data_line.AutoParse(attribute, stringList);
                res_datas.Add((T1)Convert.ChangeType(stringList[0], typeof(T1)), data_line);
                inputData = reader.ReadLine();
                lineCount++;
            }

            JLogger.WriteFormat("[DataTable] [{0} Count]{1}", lineCount, fileName);

            reader.Dispose();
            reader.Close();

            //foreach (var data in res_datas.Values)
            //    data.OnLoad();

            return res_datas;
        }
        #endregion

        #region [외부]
        public static IDictionary<long, T> Load<T>(string fileName, string path = null, bool auto = false) where T : JCsvData_base, new()
        {
            return Load<long, T>(fileName, path, auto);
        }

        public static IDictionary<long, T> LoadAttribute<T>(string fileName, string path = null) where T : JCsvData_base, new()
        {
            return LoadAttribute<long, T>(fileName, path);
        }
        #endregion

        #endregion

        #region [파일파싱] [Save]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        #region [내부]
        // 바로 Save 할 시 attribute 값 입력
        public static bool Save<T1, T2>(IDictionary<T1, T2> dict, string fileName, string[] attribute = null, string path = null) where T2 : JCsvData_base, new()
        {
            var writer = new StringWriter();
            if (writer == null)
            {
                return false;
            }
            var fileFullPath = path ?? s_data_path + fileName + ".txt";               
            var attri = attribute == null ? _textfile_attributes[fileName] : attribute;
            var length = attri.Length;

            if(attri != null)
            {
                // 헤더
                writer.Write(attri[0]);
                for (int i = 1; i < length; ++i)
                {
                    writer.Write('\t');
                    writer.Write(attri[i]);
                }
                writer.Write('\n');

                // 데이터
                List<string> _data_list = new List<string>();
                foreach (var obj in dict.Values)
                {
                    _data_list.Clear();

                    // 클래스 내의 데이터를 문자열로 가져와야함
                    var objType = obj.GetType();
                    if (objType.IsAssignableFrom(obj.GetType()) == false)
                        return false;

                    FieldInfo[] fields = objType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (FieldInfo field in fields)
                    {
                        object fieldValue = field.GetValue(obj);
                        if (fieldValue != null)
                        {
                             //field.SetValue(obj, JUtil.CloneProcedure(fieldValue));
                             _data_list.Add(fieldValue.ToString());
                        }
                    }

                    // 역순으로 읽기, 인자로 bool 변수를 받아 온오프 할 수 있음
                    _data_list.Reverse();
                    int count = 0;
                    foreach (var data in _data_list)
                    {
                        writer.Write(data);
                        writer.Write('\t');
                        count++;
                        if(count == attri.Length)
                        {
                            count = 0;
                            writer.Write('\n');
                        }
                    }
                }
               
                File.WriteAllText(fileFullPath, writer.ToString());
            }
            

            writer.Dispose();
            writer.Close();
        
            return true;
        }

        public static bool SaveAttribute<T1, T2>(ref IDictionary<T1, T2> dict, string[] attribute, string fileName, string path = null) where T2 : JCsvData_base, new()
        {
            //var fileFullPath = path ?? s_data_path + fileName + ".txt";

            //var writer = new StringWriter();

            //var length = attribute.Length;
            //writer.Write(attribute[0]);
            //for (int i = 1; i < length; ++i)
            //{
            //    writer.Write('\t');
            //    writer.Write(attribute[i]);
            //}
            //writer.Write('\n');

            //foreach (var item in dict)
            //{
            //    writer.Write(item.Key);
            //    writer.Write('\t');

            //    writer.Write(item.Value);
            //}

            //File.WriteAllText(fileFullPath, writer.ToString());

            return true;
        }

        #endregion

        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 백업
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        


        #region [백업]
        //protected static string LoadFileToString(string fileName)
        //{
        //    string fileFullPath = "Vendetta/DataTable/" + fileName;
        //    TextAsset _txtFile = Resources.Load(fileFullPath, typeof(TextAsset)) as TextAsset;
        //    TextReader reader = new StringReader(_txtFile.text);
        //    string _info = reader.ReadToEnd();
        //    reader.Close();
        //    byte[] buffers = StringToUTF8ByteArray(_info);
        //    return _info;
        //}

        //private static byte[] StringToUTF8ByteArray(string _string)
        //{
        //    UTF8Encoding encoding = new UTF8Encoding();
        //    byte[] byteArray = encoding.GetBytes(_string);
        //    return byteArray;
        //} 
        #endregion

    }

}
