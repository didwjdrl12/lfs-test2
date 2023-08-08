using J2y;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2y.MultiDroneUnity
{
    class Program
    {
        private static MultiDroneServer _server_root;

        static void Main(string[] args)
        {
            Console.WriteLine("MultiDrone Unity Server On");

            MdusBase.LoadConfig();
            var path = Environment.CurrentDirectory;
            _server_root = JObjectManager.Create<MultiDroneServer>();

            while (_server_root._running)
            {
                var start_time = JTimer.GetCurrentTime();
                _server_root.UpdateInternal();
                JTimer.SleepIdleTime(start_time, 5);
            }

            JObjectManager.Destroy(_server_root);
        }
    }
}
