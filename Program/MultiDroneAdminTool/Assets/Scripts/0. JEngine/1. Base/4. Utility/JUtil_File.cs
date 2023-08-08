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
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

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
        // Path
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region 경로 문제
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public string pathForDocumentsFile(string filename)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(Path.Combine(path, "Documents"), filename);
            }

            else if (Application.platform == RuntimePlatform.Android)
            {
                string path = Application.persistentDataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(path, filename);
            }

            else
            {
                string path = Application.dataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(path, filename);
            }
        }
        #endregion

        #region [폴더]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void CreateFolder(string folder)
        {
            var di = new DirectoryInfo(folder);
            if (di.Exists == false)
                di.Create();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool ExistsFolder(string folder)
        {
            var di = new DirectoryInfo(folder);
            return di.Exists;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void DeleteFolder(string folder, bool recursive)
        {
            var di = new DirectoryInfo(folder);
            if (di.Exists)
                di.Delete(recursive);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        #endregion


        #region [Path] Current
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string CurrentDirectory
        {
            get { return Environment.CurrentDirectory; }
            set { Environment.CurrentDirectory = value; }
        }
        #endregion

        #region [Path] Combine
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string CombinePath(params string[] paths)
        {
            return Path.Combine(paths);
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Ini File
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [INI] DllImport
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
                                                        int size, string filePath);
        #endregion

        #region [INI] Read/Write
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        public static void WriteIniString(string section, string key, string val, string filePath)
        {
            WritePrivateProfileString(section, key, val, filePath);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        public static string ReadIniString(string section, string key, string def, string filePath)
        {
            var sb = new StringBuilder(256);
            GetPrivateProfileString(section, key, def, sb, sb.Capacity, filePath);
            return sb.ToString();
        }
        #endregion

        
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Process
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Process] 실행
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Process ExecuteProcess(string path, string program_name, bool check_duplicate_run = true, string arguments = "", ProcessWindowStyle window_style = ProcessWindowStyle.Normal) // ProcessWindowStyle.Hidden, Normal
        {
            if (check_duplicate_run && IsRunningProcess(program_name))
                return null;

            var process_fullname = string.Format("{0}\\{1}.exe", path, program_name);

            var info = new ProcessStartInfo(process_fullname);                           //, argument);
            info.WorkingDirectory = path;
            info.WindowStyle = window_style;
            info.Arguments = arguments;
            var process = Process.Start(info);
            return process;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Process ExecuteBatchFile(string path, string program_name, string arguments = "", ProcessWindowStyle window_style = ProcessWindowStyle.Normal) // ProcessWindowStyle.Hidden, Normal
        {
            // if (IsRunningProcess(program_name, true))
            //    return GetRunningProcess(program_name);

            var process_fullname = string.Format("{0}\\{1}.bat", path, program_name);

            var info = new ProcessStartInfo(process_fullname);                           //, argument);
            info.WorkingDirectory = path;
            info.WindowStyle = window_style;
            info.Arguments = arguments;
            return Process.Start(info);
        }
        #endregion

    #region [Process] 실행중인가
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public static bool IsRunningProcess(string program_name, bool duplicate_running = false)
        {
            var process_list = Process.GetProcessesByName(program_name);
            if (process_list.Length > 0)
            {
                if (duplicate_running)                                                 // 디버깅을 위해 실행되어 있으면 실행하는 부분을 건너 띈다.
                    return true;
                else
                {
                    foreach (var process in process_list)
                        process.Kill();
                }
            }
            return false;
        } 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsRunningProcess(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                return process != null;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
        #endregion

        #region [Process] 종료
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void KillProcess(string program_name)
        {
            var process_list = Process.GetProcessesByName(program_name);
            if (process_list.Length > 0)
            {
                foreach (var process in process_list)
                    process.Kill();
            }
        }
        public static void KillProcess(int pid)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "taskkill",
                Arguments = string.Format("/pid {0} /f /t", pid),
                CreateNoWindow = true,
                UseShellExecute = false
            }).WaitForExit();
        }
        #endregion




}



//}
