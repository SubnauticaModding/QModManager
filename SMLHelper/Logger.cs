namespace SMLHelper.V2
{
    using System;

    internal static class Logger
    {
        internal static void Log(string text)
        {
            Console.WriteLine($"[SMLHelper] {text}");
        }

        internal static void Log(string text, params object[] args)
        {
            if (args != null && args.Length > 0)
                text = string.Format(text, args);

            Console.WriteLine($"[SMLHelper] {text}");
        }

        internal static void Announce(string text, bool logToFile = false)
        {
            ErrorMessage.AddMessage(text);

            if (logToFile)
                Log(text);
        }

        internal static void Announce(string text, bool logToFile = false, params object[] args)
        {
            ErrorMessage.AddMessage(string.Format(text, args));

            if (logToFile)
                Log(text, args);
        }
    }
}
