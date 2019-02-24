namespace SMLHelper.V2
{
    using System;
    using System.IO;

    internal enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3,
    }

    internal static class Logger
    {
        internal static bool EnableDebugging { get; private set; }
        internal static void SetDebugging(bool value)
        {
            string configPath = "./QMods/Modding Helper/EnableDebugLogs.txt";

            File.WriteAllText(configPath, value.ToString());
            EnableDebugging = value;
        }

        internal static void Initialize()
        {
            string configPath = "./QMods/Modding Helper/EnableDebugLogs.txt";

            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, "False");
                EnableDebugging = false;

                return;
            }

            string fileContents = File.ReadAllText(configPath);

            try
            {
                EnableDebugging = bool.Parse(fileContents);

                Log($"Enable debug logs set to: {EnableDebugging.ToString()}", LogLevel.Info);
            }
            catch (Exception)
            {
                File.WriteAllText(configPath, "False");
                EnableDebugging = false;

                Log("Error reading EnableDebugLogs.txt configuration file. Defaulted to false", LogLevel.Warn);
            }
        }

        internal static void Log(string text, LogLevel level = LogLevel.Info)
        {
            if (level >= LogLevel.Info || EnableDebugging)
                Console.WriteLine($"[SMLHelper/{level.ToString()}] {text}");
        }

        internal static void Log(string text, LogLevel level = LogLevel.Info, params object[] args)
        {
            if (args != null && args.Length > 0)
                text = string.Format(text, args);

            if (level >= LogLevel.Info || EnableDebugging)
                Console.WriteLine($"[SMLHelper/{level.ToString()}] {text}");
        }

        internal static void Announce(string text, LogLevel level = LogLevel.Info, bool logToFile = false)
        {
            ErrorMessage.AddMessage(text);

            if (logToFile)
                Log(text, level);
        }

        internal static void Announce(string text, LogLevel level = LogLevel.Info, bool logToFile = false, params object[] args)
        {
            ErrorMessage.AddMessage(string.Format(text, args));

            if (logToFile)
                Log(text, level, args);
        }
    }
}
