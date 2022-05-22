using System;
using System.Collections.Generic;
using System.Text;
namespace Atavism
{

    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4

    }

    public class AtavismLogger
    {
        public static LogLevel logLevel = LogLevel.Debug;
        public static void LogTraceMessage(object message)
        {
            if (logLevel <= LogLevel.Trace)
            {
                UnityEngine.Debug.Log("TRACE: " + message);
            }
        }

        public static void LogTraceMessage(object message, UnityEngine.GameObject go)
        {
            if (logLevel <= LogLevel.Trace)
            {
                UnityEngine.Debug.Log("TRACE: " + message, go);

            }
        }


        public static void LogDebugMessage(object message)
        {
            if (logLevel <= LogLevel.Debug)
            {
                UnityEngine.Debug.Log(message);
            }
        }

        public static void LogDebugMessage(object message, UnityEngine.GameObject go)
        {
            if (logLevel <= LogLevel.Debug)
            {
                UnityEngine.Debug.Log(message, go);
            }
        }

        public static void LogInfoMessage(object message)
        {
            if (logLevel <= LogLevel.Info)
            {
                UnityEngine.Debug.Log(message);
            }
        }

        public static void LogInfoMessage(object message, UnityEngine.GameObject go)
        {
            if (logLevel <= LogLevel.Info)
            {
                UnityEngine.Debug.Log(message, go);
            }
        }

        public static void LogWarning(object message)
        {
            if (logLevel <= LogLevel.Warning)
            {
                UnityEngine.Debug.LogWarning(message);
            }
        }

        public static void LogWarning(object message, UnityEngine.GameObject go)
        {
            if (logLevel <= LogLevel.Warning)
            {
                UnityEngine.Debug.LogWarning(message, go);
            }
        }

        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public static void LogError(object message, UnityEngine.GameObject go)
        {
            UnityEngine.Debug.LogError(message, go);
        }
    }
}