using System;

#if !NET_SERVER
using UnityEngine;
#endif

public class JLogger
{
#if NET_SERVER
    public static void Write(string log, int log_level = 0)
    {
        Console.WriteLine(log);
    }
    public static void WriteWarning(string log)
    {
        Console.WriteLine(log);
    }
    public static void WriteError(string log)
    {
        Console.WriteLine(log);
    }
    public static void WriteFormat(string log, params object[] args)
    {
        Console.WriteLine(string.Format(log, args));
    }
#else
    public static void Write(string log, int log_level = 0)
    {
        Debug.Log(log);
    }
    public static void WriteWarning(string log)
    {
        Debug.Log(log);
    }
    public static void WriteError(string log, params object[] args)
    {
		Debug.LogErrorFormat(log, args);
	}
    public static void WriteFormat(string log, params object[] args)
    {
        Debug.Log(string.Format(log, args));
    }
#endif


}




