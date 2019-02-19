namespace SMLHelper.V2
{
    using System;

    internal enum LogLevel
    {
        Debug,
        Warn,
        Error,
        Info
    }

    internal static class Logger
    {
        internal static LogLevel LoggingLevel { get; private set; } = LogLevel.Warn;

        internal static void Initialize(LogLevel level)
        {
            LoggingLevel = level;
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
