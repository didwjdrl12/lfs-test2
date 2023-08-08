//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace J2y.Network
//{
//	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//	//
//	// (백업) JTcpNetManager
//	//
//	//
//	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//	public class JTcpNetManager
//	{
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// 변수
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


//		#region 변수

//		internal static NetQueue<JTcpNetBase>[] _net_bases;
//		private static int _thread_seq_number;

//		#endregion



//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// TcpNetBase
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//		#region [TcpNetBase] Add/Remove
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		public static void Add(JTcpNetBase net)
//		{
//			lock (_net_bases)
//			{
//				var thread_idx = (_thread_seq_number++) % _net_bases.Length;
//				_net_bases[thread_idx].Enqueue(net);
//			}
//		}
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		public static void Remove(JTcpNetBase net)
//		{
//			lock (_net_bases)
//			{

//			}
//		}
//		#endregion



//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// ThreadPool
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//		#region [초기화] IO ThreadPool
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		public static void Initialize(int thread_count)
//		{
//			_net_bases = new NetQueue<JTcpNetBase>[thread_count];
//			for (int i = 0; i < _net_bases.Length; ++i)
//				_net_bases[i] = new NetQueue<JTcpNetBase>(100);

//			// IO 쓰레드풀 생성
//			for (int i = 0; i < thread_count; ++i)
//			{
//				int thread_index = i;
//				Task.Run(() =>
//				{
//					work_NetStream(thread_index);
//				});
//			}
//		}
//		#endregion


//		#region [WorkThread] IO Work
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		public static void work_NetStream(int thread_index)
//		{
//			while (true)
//			{
//				var start_time = JTimer.GetCurrentTime();

//				//---------------------------------------------------------------------------------------
//				// 1. 비동기 IO 작업들을 요청한다.
//				//	  내부적으로 루핑하고 있기 때문에 계속 추적할 필요는 없다.
//				//
//				lock (_net_bases)
//				{
//					var net_bases = _net_bases[thread_index];
//					while (net_bases.Count > 0)
//					{
//						JTcpNetBase net_base;
//						if (!net_bases.TryDequeue(out net_base))
//							continue;
//						net_base.RequestAsyncWorks();
//					}
//				}

//				JTimer.SleepIdleTime(start_time, 50);
//			}
//		}
//		#endregion

//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// 내부 함수
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



//	}

//}
