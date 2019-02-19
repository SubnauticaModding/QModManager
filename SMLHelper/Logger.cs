namespace SMLHelper.V2
{
    using System;
    using System.IO;

    internal enum LogLevel
    {
        Debug = 0,
        Warn = 1,
        Error = 2,
        Info = 3
    }

    internal static class Logger
    {
        internal static LogLevel LoggingLevel { get; private set; }

        internal static void Initialize()
        {
            string configPath = "./QMods/Modding Helper/loglevel.txt";

            if(!File.Exists(configPath))
            {
                File.WriteAllText(configPath, "Warn");
                LoggingLevel = LogLevel.Warn;

                return;
            }

            string level = File.ReadAllText(configPath);

            try
            {
                LoggingLevel = (LogLevel)Enum.Parse(typeof(LogLevel), level);

                Log($"Log level set to: LogLevel.{LoggingLevel.ToString()}.", LogLevel.Info);
            }
            catch (Exception)
            {
                LoggingLevel = LogLevel.Warn;

                File.WriteAllText(configPath, "Warn");

                Log("Error reading log level configuration file. Defaulted to LogLevel.Warn.", LogLevel.Warn);
            }
        }

        internal static void Log(string text, LogLevel level = LogLevel.Error)
        {
            if(level >= LoggingLevel)
                Console.WriteLine($"[SMLHelper/{level.ToString()}] {text}");
        }

        internal static void Log(string text, LogLevel level = LogLevel.Error, params object[] args)
        {
            if (args != null && args.Length > 0)
                text = string.Format(text, args);

            if(level >= LoggingLevel)
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
