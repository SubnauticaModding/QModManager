using QModManager.API.SMLHelper.Utility;
using System;
using System.Diagnostics;
using System.Reflection;

namespace QModManager.Utility
{
    internal static class Logger
    {
        internal static bool EnableDebugging
        {
            get
            {
                return PlayerPrefsExtra.GetBool("QModManager_EnableDebugLogs", false);
            }
            set
            {
                PlayerPrefsExtra.SetBool("QModManager_EnableDebugLogs", value);
            }
        }

        internal static void Log(string logLevel, string text)
        {
            string from;
            Type classType = GetCallingClass();

            if (classType == null) from = null;
            else if (classType.Namespace.Contains("SMLHelper")) from = "SMLHelper";
            else from = classType.Name;

            if (string.IsNullOrEmpty(from))
                Console.WriteLine($"[QModManager] [{logLevel}] {text}");
            else
                Console.WriteLine($"[QModManager] [{from}] [{logLevel}] {text}");
        }

        internal static void Log(string text)
        {
            Log("", text);
        }

        internal static void Debug(string text, bool force = false)
        {
            if (EnableDebugging || force)
                Log("Debug", text);
        }
        internal static void Info(string text)
        {
            Log("Info", text);
        }
        internal static void Warn(string text)
        {
            Log("Warn", text);
        }
        internal static void Error(string text)
        {
            Log("Error", text);
        }
        internal static void Exception(Exception e)
        {
            Log("Exception", e.ToString());
        }
        internal static void Fatal(string text)
        {
            Log("Fatal", text);
        }

        internal static Type GetCallingClass()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] frames = stackTrace.GetFrames();

            foreach (StackFrame stackFrame in frames)
            {
                Type declaringClass = stackFrame.GetMethod().DeclaringType;
                if (declaringClass != typeof(Logger))
                    return declaringClass;
            }

            return null;
        }
    }
}
