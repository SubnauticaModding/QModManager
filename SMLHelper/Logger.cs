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
            string configPath = "./QMods/Modding Helper/LogLevel.txt";

            File.WriteAllText(configPath, value ? "All" : "Important");

            EnableDebugging = value;
        }

        internal static void Initialize()
        {
            string configPath = "./QMods/Modding Helper/LogLevel.txt";

            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, "Important");
                EnableDebugging = false;

                return;
            }

            string fileContents = File.ReadAllText(configPath);

            try
            {
                EnableDebugging = bool.Parse(fileContents);

                Log($"Log level set to: {(EnableDebugging ? "All" : "Important")}", LogLevel.Info);
            }
            catch (Exception)
            {
                EnableDebugging = false;

                File.WriteAllText(configPath, "Important");

                Log("Error reading log level configuration file. Defaulted to Important", LogLevel.Warn);
            }
        }

        internal static void Log(string text, LogLevel level = LogLevel.Error)
        {
            if (level >= LogLevel.Info || EnableDebugging)
                Console.WriteLine($"[SMLHelper/{level.ToString()}] {text}");
        }

        internal static void Log(string text, LogLevel level = LogLevel.Error, params object[] args)
        {
            if (args != null && args.Length > 0)
                text = string.Format(text, args);

            if (level >= LogLevel.Info || EnableDebugging)
                Console.WriteLine($"[SMLHelper/{level.ToString()}] {text}");
        }

        internal static void Announce(string text, LogLevel level, bool logToFile = false)
        {
            ErrorMessage.AddMessage(text);

            if (logToFile)
                Log(text, level);
        }

        internal static void Announce(string text, LogLevel level, bool logToFile = false, params object[] args)
        {
            ErrorMessage.AddMessage(string.Format(text, args));

            if (logToFile)
                Log(text, level, args);
        }
    }
}
