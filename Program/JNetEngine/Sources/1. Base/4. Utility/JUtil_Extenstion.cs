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
using System.Diagnostics;

//namespace J2y
//{

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// JUtil. Extensions
//
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

#region [Extensions] Dictionary
public static class DictionaryExtensions
    {
        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            Func<TValue, bool> predicate)
        {
            var keys = dic.Keys.Where(k => predicate(dic[k])).ToList();
            foreach (var key in keys)
            {
                dic.Remove(key);
            }
        }
    }
    #endregion
    
    #region [Extensions] List
    public static class ListExtensions
    {
        //    list: List<T> to resize
        //    size: desired new size
        // element: default value to insert

        public static void Resize<T>(this List<T> list, int size, T element = default(T))
        {
            int count = list.Count;

            if (size < count)
            {
                list.RemoveRange(size, count - size);
            }
            else if (size > count)
            {
                if (size > list.Capacity)   // Optimization
                    list.Capacity = size;

                list.AddRange(Enumerable.Repeat(element, size - count));
            }
        }
    }
    #endregion


    #region [Extensions] GameObject
    public static class GameObjectExtensions
    {
        #region [GameObject] GetOrAddComponent
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T GetOrAddComponent<T>(this GameObject parentGO) where T : Component
        {
            var com = parentGO.GetComponent<T>();
            if (null == com)
                com = parentGO.AddComponent<T>();
            return com;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Component GetOrAddComponent(this GameObject parentGO, Type type)
        {
            var com = parentGO.GetComponent(type);
            if (null == com)
                com = parentGO.AddComponent(type);
            return com;
        }
        #endregion
    }
    #endregion

    #region [Interface] Prototype
    public abstract class Prototype
    {
        public abstract Prototype Clone();
    }
#endregion

    #region [Process]

public static class ProcessExtensions
{
    public static bool IsRunning(this Process process)
    {
        var id = process.Id;
        try
        {
            Process.GetProcessById(id);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
#endregion



//}




#region [Extensions] Linq
public static class LinqExtensions
{
    public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
    {
        foreach (var x in @this)
            action(x);
    }
}
#endregion