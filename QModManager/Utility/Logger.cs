namespace QModManager.Utility
{
    using System;
    using System.Diagnostics;
    using System.IO;

    internal static class Logger
    {
        internal enum Level
        {
            Debug,
            Info,
            Warn,
            Error,
            Fatal
        }

        /// <summary>
        /// Gets a value indicating whether debug logs are enabled.
        /// To enable debug logs, simple create an empty file named <c>"QModDebug.txt"</c> within the Subnautica folder.
        /// </summary>
        /// <value>
        ///   <c>true</c> if debug logs are enabled; otherwise, <c>false</c>.
        /// </value>
        internal static bool EnableDebugLogging { get; } = File.Exists(Path.Combine(Environment.CurrentDirectory, "QModDebug.txt"));

        private static void Log(string logLevel, params string[] text)
        {
            if (text == null || text.Length < 1)
                return;

            string from;
            Type classType = GetCallingClass();

            if (classType == null)
                from = null;
            else if (classType.Namespace.Contains("SMLHelper"))
                from = "SMLHelper";
            else
                from = classType.Name;

            string toWrite = "[QModManager] ";
            if (!string.IsNullOrEmpty(from))
                toWrite += $"[{from}] ";
            if (!string.IsNullOrEmpty(logLevel))
                toWrite += $"[{logLevel}] ";

            int length = toWrite.Length;

            Console.WriteLine($"{toWrite}{text[0]}");

            for (int i = 1; i < text.Length; i++)
                Console.WriteLine($"{text[i]}");
        }

        internal static void Log(params string[] text)
        {
            Log("", text);
        }

        internal static void Log(Level logLevel, params string[] text)
        {
            switch (logLevel)
            {
                case Level.Debug:
                    Debug(text);
                    break;
                case Level.Info:
                    Info(text);
                    break;
                case Level.Warn:
                    Warn(text);
                    break;
                case Level.Error:
                    Error(text);
                    break;
                case Level.Fatal:
                    Fatal(text);
                    break;
            }
        }

        internal static void Debug(params string[] text)
        {
            if (EnableDebugLogging)
                Log("Debug", text);
        }

        internal static void DebugForce(params string[] text)
        {
            Log("Debug", text);
        }

        internal static void Info(params string[] text)
        {
            Log("Info", text);
        }

        internal static void Warn(params string[] text)
        {
            Log("Warn", text);
        }

        internal static void Error(params string[] text)
        {
            Log("Error", text);
        }

        internal static void Exception(Exception e)
        {
            Log("Exception", e.ToString());
        }

        internal static void Fatal(params string[] text)
        {
            Log("Fatal", text);
        }

        private static Type GetCallingClass()
        {
            var stackTrace = new StackTrace();
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
