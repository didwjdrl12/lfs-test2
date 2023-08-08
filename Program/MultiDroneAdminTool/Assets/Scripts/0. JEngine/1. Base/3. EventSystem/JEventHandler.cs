using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JEventHandler
    //		Adds a generic event system. The event system allows objects to register, unregister, and execute events on a particular object.
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JEventHandler
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [Variable] EventTable

        private static Dictionary<object, Dictionary<string, Delegate>> s_EventTable = new Dictionary<object, Dictionary<string, Delegate>>();
        private static Dictionary<string, Delegate> s_GlobalEventTable = new Dictionary<string, Delegate>();

        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] »ý¼ºÀÚ
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private JEventHandler()
        {
            ClearTable();
        }
        #endregion

        #region [Init] ClearTable
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ClearTable()
        {
            s_EventTable.Clear();
            JEventHandler.ExecuteEvent("OnEventHandlerClear");
        }
        #endregion

        #region [Delegate] Get
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static Delegate GetDelegate(string eventName)
        {
            Delegate handler;
            if (s_GlobalEventTable.TryGetValue(eventName, out handler))
            {
                return handler;
            }
            return null;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static Delegate GetDelegate(object obj, string eventName)
        {
            Dictionary<string, Delegate> handlers;
            if (s_EventTable.TryGetValue(obj, out handlers))
            {
                Delegate handler;
                if (handlers.TryGetValue(eventName, out handler))
                {
                    return handler;
                }
            }
            return null;
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Event] Register/Unregister
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Event] Register
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static void RegisterEvent(string eventName, Delegate handler)
        {
            Delegate prevHandlers;
            if (s_GlobalEventTable.TryGetValue(eventName, out prevHandlers))
            {
                s_GlobalEventTable[eventName] = Delegate.Combine(prevHandlers, handler);
            }
            else
            {
                s_GlobalEventTable.Add(eventName, handler);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static void RegisterEvent(object obj, string eventName, Delegate handler)
        {
            if (obj == null)
            {
                JLogger.Write("EventHandler.RegisterEvent error: target object cannot be null.");
                return;
            }

            Dictionary<string, Delegate> handlers;
            if (!s_EventTable.TryGetValue(obj, out handlers))
            {
                handlers = new Dictionary<string, Delegate>();
                s_EventTable.Add(obj, handlers);
            }

            Delegate prevHandlers;
            if (handlers.TryGetValue(eventName, out prevHandlers))
            {
                handlers[eventName] = Delegate.Combine(prevHandlers, handler);
            }
            else
            {
                handlers.Add(eventName, handler);
            }
        }
        #endregion

        #region [Event] Register<T>
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RegisterEvent(string eventName, Action handler) { RegisterEvent(eventName, (Delegate)handler); }
        public static void RegisterEvent(object obj, string eventName, Action handler) { RegisterEvent(obj, eventName, (Delegate)handler); }
        public static void RegisterEvent<T>(string eventName, Action<T> handler) { RegisterEvent(eventName, (Delegate)handler); }
        public static void RegisterEvent<T>(object obj, string eventName, Action<T> handler) { RegisterEvent(obj, eventName, (Delegate)handler); }
        public static void RegisterEvent<T, U>(string eventName, Action<T, U> handler) { RegisterEvent(eventName, (Delegate)handler); }
        public static void RegisterEvent<T, U>(object obj, string eventName, Action<T, U> handler) { RegisterEvent(obj, eventName, (Delegate)handler); }
        public static void RegisterEvent<T, U, V>(string eventName, Action<T, U, V> handler) { RegisterEvent(eventName, (Delegate)handler); }
        public static void RegisterEvent<T, U, V>(object obj, string eventName, Action<T, U, V> handler) { RegisterEvent(obj, eventName, (Delegate)handler); }
        public static void RegisterEvent<T, U, V, W>(string eventName, Action<T, U, V, W> handler) { RegisterEvent(eventName, (Delegate)handler); }
        public static void RegisterEvent<T, U, V, W>(object obj, string eventName, Action<T, U, V, W> handler) { RegisterEvent(obj, eventName, (Delegate)handler); }
        #endregion

        #region [Event] Unregister
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static void UnregisterEvent(string eventName, Delegate handler)
        {
            Delegate prevHandlers;
            if (s_GlobalEventTable.TryGetValue(eventName, out prevHandlers))
            {
                s_GlobalEventTable[eventName] = Delegate.Remove(prevHandlers, handler);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static void UnregisterEvent(object obj, string eventName, Delegate handler)
        {
            if (obj == null)
            {
                JLogger.Write("EventHandler.UnregisterEvent error: target object cannot be null.");
                return;
            }

            Dictionary<string, Delegate> handlers;
            if (s_EventTable.TryGetValue(obj, out handlers))
            {
                Delegate prevHandlers;
                if (handlers.TryGetValue(eventName, out prevHandlers))
                {
                    handlers[eventName] = Delegate.Remove(prevHandlers, handler);
                }
            }
        }
        #endregion

        #region [Event] Unregister<T>
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void UnregisterEvent(string eventName, Action handler) { UnregisterEvent(eventName, (Delegate)handler); }
        public static void UnregisterEvent(object obj, string eventName, Action handler) { UnregisterEvent(obj, eventName, (Delegate)handler); }
        public static void UnregisterEvent<T>(string eventName, Action<T> handler) { UnregisterEvent(eventName, (Delegate)handler); }
        public static void UnregisterEvent<T>(object obj, string eventName, Action<T> handler) { UnregisterEvent(obj, eventName, (Delegate)handler); }
        public static void UnregisterEvent<T, U>(string eventName, Action<T, U> handler) { UnregisterEvent(eventName, (Delegate)handler); }
        public static void UnregisterEvent<T, U>(object obj, string eventName, Action<T, U> handler) { UnregisterEvent(obj, eventName, (Delegate)handler); }
        public static void UnregisterEvent<T, U, V>(string eventName, Action<T, U, V> handler) { UnregisterEvent(eventName, (Delegate)handler); }
        public static void UnregisterEvent<T, U, V>(object obj, string eventName, Action<T, U, V> handler) { UnregisterEvent(obj, eventName, (Delegate)handler); }
        public static void UnregisterEvent<T, U, V, W>(string eventName, Action<T, U, V, W> handler) { UnregisterEvent(eventName, (Delegate)handler); }
        public static void UnregisterEvent<T, U, V, W>(object obj, string eventName, Action<T, U, V, W> handler) { UnregisterEvent(obj, eventName, (Delegate)handler); }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Event] Execute
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Event] Execute
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ExecuteEvent(string eventName)
        {
            var handler = GetDelegate(eventName) as Action;
            if (handler != null)
            {
                handler();
            }
        }
        public static void ExecuteEvent(object obj, string eventName)
        {
            var handler = GetDelegate(obj, eventName) as Action;
            if (handler != null)
            {
                handler();
            }
        }
        public static void ExecuteEvent<T>(string eventName, T arg1)
        {
            var handler = GetDelegate(eventName) as Action<T>;
            if (handler != null)
            {
                handler(arg1);
            }
        }
        public static void ExecuteEvent<T>(object obj, string eventName, T arg1)
        {
            var handler = GetDelegate(obj, eventName) as Action<T>;
            if (handler != null)
            {
                handler(arg1);
            }
        }
        public static void ExecuteEvent<T, U>(string eventName, T arg1, U arg2)
        {
            var handler = GetDelegate(eventName) as Action<T, U>;
            if (handler != null)
            {
                handler(arg1, arg2);
            }
        }
        public static void ExecuteEvent<T, U>(object obj, string eventName, T arg1, U arg2)
        {
            var handler = GetDelegate(obj, eventName) as Action<T, U>;
            if (handler != null)
            {
                handler(arg1, arg2);
            }
        }
        public static void ExecuteEvent<T, U, V>(string eventName, T arg1, U arg2, V arg3)
        {
            var handler = GetDelegate(eventName) as Action<T, U, V>;
            if (handler != null)
            {
                handler(arg1, arg2, arg3);
            }
        }

        public static void ExecuteEvent<T, U, V>(object obj, string eventName, T arg1, U arg2, V arg3)
        {
            var handler = GetDelegate(obj, eventName) as Action<T, U, V>;
            if (handler != null)
            {
                handler(arg1, arg2, arg3);
            }
        }
        public static void ExecuteEvent<T, U, V, W>(string eventName, T arg1, U arg2, V arg3, W arg4)
        {
            var handler = GetDelegate(eventName) as Action<T, U, V, W>;
            if (handler != null)
            {
                handler(arg1, arg2, arg3, arg4);
            }
        }
        public static void ExecuteEvent<T, U, V, W>(object obj, string eventName, T arg1, U arg2, V arg3, W arg4)
        {
            var handler = GetDelegate(obj, eventName) as Action<T, U, V, W>;
            if (handler != null)
            {
                handler(arg1, arg2, arg3, arg4);
            }
        }
        #endregion




    }
}
