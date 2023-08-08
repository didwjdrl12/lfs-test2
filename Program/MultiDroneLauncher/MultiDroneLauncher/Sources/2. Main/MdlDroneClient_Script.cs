using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using J2y.Network;
using System.Threading.Tasks;
using System.Text;

namespace J2y.MultiDrone
{
   
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MdlDroneClient
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public partial class MdlDroneClient : JObject
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ����
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [Property] 
        #endregion





        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ��ũ��Ʈ
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [��ũ��Ʈ] ����
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task RunScript(byte[] script_data, string file_path)
        {
            //----------------------------------------------------------------------------------
            // 1. (��ũ��Ʈ��) Rpc ���� ��Ʈ ����
            //
            //ChangeScriptServerPort(NetData._script_server_port);

            var script_text = ByteToString(script_data);
            script_text = script_text.Replace("airsim.CarClient()", string.Format("airsim.CarClient('', {0})", NetData._script_server_port));
            script_text = script_text.Replace("airsim.MultirotorClient()", string.Format("airsim.MultirotorClient('', {0})", NetData._script_server_port));
            script_text = script_text.Replace("airsim.VehicleClient()", string.Format("airsim.VehicleClient('', {0})", NetData._script_server_port));


            //----------------------------------------------------------------------------------
            // 2. ��ũ��Ʈ ���� ����
            //
            file_path = file_path.Replace(".py", string.Format("_{0}.py", NetData._script_server_port)); // ���ÿ� ������ ������ ���ؼ�
            var script_fullname = string.Format("{0}\\{1}", MdlBase._path_python_client, file_path);
            
            //File.WriteAllBytes(script_fullname, script_data);
            File.WriteAllText(script_fullname, script_text);

            while (!File.Exists(script_fullname))
            {
                await JScheduler.Wait(0.01f);
            }
            // ���� ���� ���� ��� ���


            //----------------------------------------------------------------------------------
            // 3. ��ũ��Ʈ ����
            //
            var process = JUtil.ExecuteBatchFile(MdlBase._path_client, "RunScript", file_path);

            //await Task.Delay(000);
            if(!process.IsRunning())
                Console.WriteLine("Run Script Fail", process.IsRunning());

            await Task.Delay(1000);
            if(!process.IsRunning())
            {
                JUtil.ExecuteBatchFile(MdlBase._path_client, "RunScript", file_path);

                await Task.Delay(1000);
                if (!process.IsRunning())
                    Console.WriteLine("Run Script Retry Fail", process.IsRunning());
            }

            //Console.WriteLine("Running Process : {0}", process.IsRunning());
        }
        #endregion
     
        #region [��ũ��Ʈ] [��ƿ] Rpc ���� ��Ʈ ����
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool ChangeScriptServerPort(int server_port)
        {
            try
            {
                //----------------------------------------------------------------------------------
                // 1. ���� ��ũ��Ʈ ���� �б�
                //
                var origin_fullname = string.Format("{0}\\airsim\\client_origin.py", MdlBase._path_python_client);
                var client_fullname = string.Format("{0}\\airsim\\client.py", MdlBase._path_python_client);
                if (!File.Exists(origin_fullname)) // ���� ����� ���
                    File.Copy(client_fullname, origin_fullname);


                //----------------------------------------------------------------------------------
                // 2. ��ũ��Ʈ�� ��Ʈ ����
                //
                var airsim_script = File.ReadAllText(origin_fullname);
                airsim_script = airsim_script.Replace("port = 41451", string.Format("port = {0}", server_port));

                //----------------------------------------------------------------------------------
                // 3. ����
                //
                File.WriteAllText(client_fullname, airsim_script);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region [��ƿ] ByteToString
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string ByteToString(byte[] strByte)
        {
            string str = Encoding.Default.GetString(strByte);
            return str;
        }
        #endregion

    }

}
