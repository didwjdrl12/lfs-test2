using UnityEngine;
using System;
using System.Collections.Generic;
using J2y.Network;
using System.Threading;
using System.Threading.Tasks;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JScheduledEvent
    //		Used by the Scheduler, the ScheduledEvent contains the delegate and arguments for the event that should execute at a time in the future.
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    public class JScheduledEvent
    {
        #region [Variable] Internal
        private Action m_Callback = null;
        private Action<object> m_CallbackArg = null;
        private object m_Argument;
        private long m_EndTime;
        #endregion

        #region [Property] 
        public Action Callback { get { return m_Callback; } set { m_Callback = value; } }
        public Action<object> CallbackArg { get { return m_CallbackArg; } set { m_CallbackArg = value; } }
        public object Argument { get { return m_Argument; } set { m_Argument = value; } }
        public long EndTime { get { return m_EndTime; } set { m_EndTime = value; } }
        #endregion

        #region [Reset]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Reset()
        {
            m_Callback = null;
            m_CallbackArg = null;
            m_Argument = null;
        }
        #endregion

    }




    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JScheduler
    //		1. 특정 함수(Action)를 n초 후에 호출
    //      2. 주기적 업데이트 함수
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JScheduler
    {
        public static JScheduler Instance = new JScheduler();

        #region [Variable] ScheduledEvent
        public List<JScheduledEvent> ActiveEvents { get { return m_ActiveEvents; } }

        private List<JScheduledEvent> m_ActiveEvents = new List<JScheduledEvent>();
        private List<Action> m_UpdateObjects = new List<Action>();
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // Assign the static variables and register for any events.
        //
        public JScheduler()
        {
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        ~JScheduler()
        {
            m_ActiveEvents.Clear();
        }
        #endregion


        #region [Update] 시간 검사해서 함수 호출
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Loop through the ScheduledEvents and execute the event when the current time is greater than or equal to the end time of the ScheduledEvent.
        /// </summary>
        public void Update()
        {
            for (int i = m_ActiveEvents.Count - 1; i > -1; --i)
            {
                if (m_ActiveEvents[i].EndTime <= JTimer.GetCurrentTick())
                    Execute(i);
            }

            for (int i = m_UpdateObjects.Count - 1; i > -1; --i)
                m_UpdateObjects[i]();

            execute_mainThreadCommands();
        }
        #endregion

        #region [구현] 이벤트 함수 실행
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes an event with the specified index.
        /// </summary>
        /// <param name="index">The index of the event to execute.</param>
        private void Execute(int index)
        {
            var activeEvent = m_ActiveEvents[index];
            // Remove the event from the list before the callback to prevent the callback from adding a new event and changing the order.
            m_ActiveEvents.RemoveAt(index);
            if (activeEvent.Callback != null)
            {
                activeEvent.Callback();
            }
            else
            {
                activeEvent.CallbackArg(activeEvent.Argument);
            }
            //ObjectPool.Return(activeEvent);
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Schedule
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [Schedule] 등록(delay, callback)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Schedule a new event to occur after the specified delay.
        /// </summary>
        /// <param name="delay">The time to wait before triggering the event.</param>
        /// <param name="callback">The event to occur.</param>
        /// <returns>The ScheduledEvent, allows for cancelling.</returns>
        public static JScheduledEvent Schedule(float delay, Action callback)
        {
            if (Instance == null)
            {
                return null;
            }

            return Instance.AddEventInternal(delay, callback);
        }
        #endregion

        #region [Schedule] 등록(delay, callback, arg)
        /// <summary>
        /// Add a new event with an argumentto be executed in the future.
        /// </summary>
        /// <param name="delay">The delay from the current time to execute the event.</param>
        /// <param name="callback">The delegate to execute after the specified delay.</param>
        /// <param name="arg">The argument of the delegate.</param>
        /// <returns>The ScheduledEvent instance, useful if the event should be cancelled.</returns>
        public static JScheduledEvent Schedule(float delay, Action<object> callback, object arg)
        {
            if (Instance == null)
            {
                return null;
            }

            return Instance.AddEventInternal(delay, callback, arg);
        }
        #endregion

        #region [Schedule] Cancel
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Cancels an event.
        /// </summary>
        /// <param name="scheduledEvent">The event to cancel.</param>
        public static void Cancel(ref JScheduledEvent scheduledEvent)
        {
            Instance.CancelEventInternal(ref scheduledEvent);
        }

        #endregion


        #region [Schedule] [구현] 등록
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Internal method to add a new event to be executed in the future.
        /// </summary>
        /// <param name="delay">The delay from the current time to execute the event.</param>
        /// <param name="callback">The delegate to execute after the specified delay.</param>
        /// <returns>The ScheduledEvent instance, useful if the event should be cancelled.</returns>
        private JScheduledEvent AddEventInternal(float delay, Action callback)
        {
            // Don't add the event if the game hasn't started.
            //if (enabled == false) {
            //    return null;
            //}

            if (delay == 0)
            {
                callback();
                return null;
            }
            else
            {
                //var scheduledEvent = ObjectPool.Get<JScheduledEvent>();
                var scheduledEvent = new JScheduledEvent();
                scheduledEvent.Reset();
                scheduledEvent.EndTime = JTimer.GetCurrentTick() + JTimer.TimeToTick(delay);
                scheduledEvent.Callback = callback;
                m_ActiveEvents.Add(scheduledEvent);
                return scheduledEvent;
            }
        }
        #endregion
        
        #region [Schedule] [구현] 등록(arg)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Internal event to add a new event with an argumentto be executed in the future.
        /// </summary>
        /// <param name="delay">The delay from the current time to execute the event.</param>
        /// <param name="callback">The delegate to execute after the specified delay.</param>
        /// <param name="arg">The argument of the delegate.</param>
        /// <returns>The ScheduledEvent instance, useful if the event should be cancelled.</returns>
        private JScheduledEvent AddEventInternal(float delay, Action<object> callbackArg, object arg)
        {
            if (delay == 0)
            {
                callbackArg(arg);
                return null;
            }
            else
            {
                //var scheduledEvent = ObjectPool.Get<JScheduledEvent>();
                var scheduledEvent = new JScheduledEvent();
                scheduledEvent.Reset();
                scheduledEvent.EndTime = JTimer.GetCurrentTick() + JTimer.TimeToTick(delay);
                scheduledEvent.CallbackArg = callbackArg;
                scheduledEvent.Argument = arg;
                m_ActiveEvents.Add(scheduledEvent);
                return scheduledEvent;
            }
        }

        #endregion

        #region [Schedule] [구현] Cancel
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Internal method to cancel an event.
        /// </summary>
        /// <param name="scheduledEvent">The event to cancel.</param>
        private void CancelEventInternal(ref JScheduledEvent scheduledEvent)
        {
            if (scheduledEvent != null && m_ActiveEvents.Contains(scheduledEvent))
            {
                m_ActiveEvents.Remove(scheduledEvent);
                //ObjectPool.Return(scheduledEvent);
                scheduledEvent = null;
            }
        }

        #endregion



        #region [Schedule] Async wait
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static async Task Wait(float duration)
        {
            var tcs = new TaskCompletionSource<bool>();
            Schedule(duration, () =>
            {
                tcs.SetResult(true);
            });
            var result = await tcs.Task;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Updatable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Updatable] 등록
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void AddUpdatable(Action callback)
        {
            if (Instance == null)
                return;
            Instance.m_UpdateObjects.Add(callback);
        }
        #endregion
        
        #region [Updatable] 취소
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void CancelUpdatable(Action callback)
        {
            if (Instance == null)
                return;
            Instance.m_UpdateObjects.Remove(callback);
        }
        #endregion


        #region [JObject] [Updatable] 등록
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void AddUpdatable(JObject obj)
        {
            if (Instance == null)
                return;
            Instance.m_UpdateObjects.Add(obj.UpdateInternal);
        }
        #endregion

        #region [JObject] [Updatable] 취소
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void CancelUpdatable(JObject obj)
        {
            if (Instance == null)
                return;
            Instance.m_UpdateObjects.Remove(obj.UpdateInternal);
        }
        #endregion





        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [메인쓰레드] Commands
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] Commands
        internal static readonly NetQueue<Action> _commands_main_thred = new NetQueue<Action>(8);
        #endregion


        #region [메인쓰레드] [Commands] 추가
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void AddMainThreadCommand(Action fun)
        {
            _commands_main_thred.Enqueue(fun);
        }
		#endregion

		#region [메인쓰레드] [Commands] 메인 쓰레드로 복귀 (디버깅 필요)
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static Task WaitMainThread()
		{
			var instance = Instance; // singleton 
			var tcs = new TaskCompletionSource<bool>();
			AddMainThreadCommand(() =>
			{
				tcs.SetResult(true);
			});
			return tcs.Task;
		}
		#endregion



		#region [메인쓰레드] [Commands] 실행
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		protected static void execute_mainThreadCommands()
        {
            while (_commands_main_thred.Count > 0)
            {
                if (_commands_main_thred.TryDequeue(out Action command))
                    command();
            }
        }
        #endregion

    }

}
