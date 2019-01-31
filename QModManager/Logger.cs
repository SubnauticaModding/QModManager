using System;

namespace QModManager
{
    internal class Logger
    {
        private static void Log(string prefix, string text) => Console.WriteLine($"[QModManager/{prefix}] {text}");
        internal static void Debug(string text) => Log("DEBUG", text);
        internal static void Info(string text) => Log("INFO", text);
        internal static void Warn(string text) => Log("WARN", text);
        internal static void Error(string text) => Log("ERROR", text);
        internal static void Fatal(string text) => Log("FATAL", text);
    }
}
