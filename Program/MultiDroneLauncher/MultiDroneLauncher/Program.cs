using J2y;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace J2y.MultiDrone
{
    class Program
    {
        private static MultiDroneLauncher _drone_launcher;

        static void Main(string[] args)
        {
            Console.WriteLine("MultiDrone Lacuncher On");

            MdlBase.LoadConfig();
            MdlBase._path_root = Environment.CurrentDirectory;
            _drone_launcher = JObjectManager.Create<MultiDroneLauncher>();

            while (_drone_launcher._running)
            {
                var start_time = JTimer.GetCurrentTime();
                _drone_launcher.UpdateInternal();
                JScheduler.Instance.Update();
                JTimer.SleepIdleTime(start_time, 5);
            }

            JObjectManager.Destroy(_drone_launcher);
        }
    }
}
