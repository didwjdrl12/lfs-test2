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
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [Property] 
        #endregion





        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 스크립트
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [스크립트] 실행
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task RunScript(byte[] script_data, string file_path)
        {
            //----------------------------------------------------------------------------------
            // 1. (스크립트의) Rpc 서버 포트 변경
            //
            //ChangeScriptServerPort(NetData._script_server_port);

            var script_text = ByteToString(script_data);
            script_text = script_text.Replace("airsim.CarClient()", string.Format("airsim.CarClient('', {0})", NetData._script_server_port));
            script_text = script_text.Replace("airsim.MultirotorClient()", string.Format("airsim.MultirotorClient('', {0})", NetData._script_server_port));
            script_text = script_text.Replace("airsim.VehicleClient()", string.Format("airsim.VehicleClient('', {0})", NetData._script_server_port));


            //----------------------------------------------------------------------------------
            // 2. 스크립트 파일 쓰기
            //
            file_path = file_path.Replace(".py", string.Format("_{0}.py", NetData._script_server_port)); // 동시에 여러개 실행을 위해서
            var script_fullname = string.Format("{0}\\{1}", MdlBase._path_python_client, file_path);
            
            //File.WriteAllBytes(script_fullname, script_data);
            File.WriteAllText(script_fullname, script_text);

            while (!File.Exists(script_fullname))
            {
                await JScheduler.Wait(0.01f);
            }
            // 파일 쓰는 동안 잠시 대기


            //----------------------------------------------------------------------------------
            // 3. 스크립트 실행
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
     
        #region [스크립트] [유틸] Rpc 서버 포트 변경
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool ChangeScriptServerPort(int server_port)
        {
            try
            {
                //----------------------------------------------------------------------------------
                // 1. 원본 스크립트 파일 읽기
                //
                var origin_fullname = string.Format("{0}\\airsim\\client_origin.py", MdlBase._path_python_client);
                var client_fullname = string.Format("{0}\\airsim\\client.py", MdlBase._path_python_client);
                if (!File.Exists(origin_fullname)) // 최초 실행시 백업
                    File.Copy(client_fullname, origin_fullname);


                //----------------------------------------------------------------------------------
                // 2. 스크립트내 포트 변경
                //
                var airsim_script = File.ReadAllText(origin_fullname);
                airsim_script = airsim_script.Replace("port = 41451", string.Format("port = {0}", server_port));

                //----------------------------------------------------------------------------------
                // 3. 저장
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

        #region [유틸] ByteToString
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string ByteToString(byte[] strByte)
        {
            string str = Encoding.Default.GetString(strByte);
            return str;
        }
        #endregion

    }

}
