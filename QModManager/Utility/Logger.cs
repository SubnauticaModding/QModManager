using System;

namespace QModManager.Utility
{
    internal static class Logger
    {
        private static bool EnableDebugging
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

        private static void Log(string prefix, string text) => Console.WriteLine($"[QModManager{prefix}] {text}");
        internal static void Log(string text) => Log("", text);
        internal static void Debug(string text) { if (EnableDebugging) Log("/DEBUG", text); }
        internal static void Info(string text) => Log("/INFO", text);
        internal static void Warn(string text) => Log("/WARN", text);
        internal static void Error(string text) => Log("/ERROR", text);
        internal static void Fatal(string text) => Log("/FATAL", text);
    }
}
