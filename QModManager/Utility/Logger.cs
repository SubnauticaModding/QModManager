namespace QModManager.Utility
{
    using System;

    /// <summary>A simple logging class. Can be used for basic logging or to know which logging level is enabled.</summary>
    public static class Logger
    {
        /// <summary>Possible logging levels.</summary>
        public enum Level
        {
            /// <summary>Debugging log level</summary>
            Debug,
            /// <summary>Informational log level</summary>
            Info,
            /// <summary>Warning log level</summary>
            Warn,
            /// <summary>Error log level</summary>
            Error,
            /// <summary>Fatal log level</summary>
            Fatal
        }

        private const string AssemblyName = "QModManager";

        #region Private functions (used by the Logger)

        private static string GetCallingAssemblyName() => ReflectionHelper.CallingAssemblyByStackTrace()?.GetName().Name;

        private static void Debug(string msg, bool showOnScreen = false, string callingAssembly = null, bool force = false)
        {
            if (!force && !Config.EnableDebugLogs)
                return;

            Console.WriteLine($"[{callingAssembly ?? GetCallingAssemblyName()}:DEBUG] {msg}");

            if (showOnScreen)
                ErrorMessage.AddDebug(msg);
        }

        private static void Info(string msg, bool showOnScreen = false, string callingAssembly = null)
        {
            Console.WriteLine($"[{callingAssembly ?? GetCallingAssemblyName()}:INFO] {msg}");

            if (showOnScreen)
                ErrorMessage.AddMessage(msg);
        }

        private static void Warn(string msg, bool showOnScreen = false, string callingAssembly = null)
        {
            Console.WriteLine($"[{callingAssembly ?? GetCallingAssemblyName()}:WARN] {msg}");

            if (showOnScreen)
                ErrorMessage.AddWarning(msg);
        }

        private static void Error(string msg = null, Exception ex = null, bool showOnScreen = false, string callingAssembly = null)
        {
            if (ex != null)
                msg = (string.IsNullOrEmpty(msg) ? ex.ToString() : msg + Environment.NewLine + ex.ToString());

            Console.WriteLine($"[{callingAssembly ?? GetCallingAssemblyName()}:ERROR] {msg}");

            if (showOnScreen && !string.IsNullOrEmpty(msg))
                ErrorMessage.AddError(msg);
        }

        private static void Exception(Exception e, bool selfAssembly = false) => Error(null, e, false, selfAssembly ? AssemblyName : GetCallingAssemblyName());

        private static void Fatal(string msg = null, Exception ex = null, bool showOnScreen = false, string callingAssembly = null)
        {
            if (ex != null)
                msg = (string.IsNullOrEmpty(msg) ? ex.ToString() : msg + Environment.NewLine + ex.ToString());
            Console.WriteLine($"[{callingAssembly ?? GetCallingAssemblyName()}:FATAL] {msg}");

            if (showOnScreen && !string.IsNullOrEmpty(msg))
                ErrorMessage.AddError(msg);
        }

        #endregion

        #region Internal functions (used by QModManager)

        internal static void Debug(string msg) => Debug(msg, false, AssemblyName, false);

        internal static void DebugForce(string msg) => Debug(msg, false, AssemblyName, true);

        internal static void Info(string msg) => Info(msg, false, AssemblyName);

        internal static void Warn(string msg) => Warn(msg, false, AssemblyName);

        internal static void Error(string msg) => Error(msg, null, false, AssemblyName);

        internal static void Exception(Exception e) => Exception(e, true);

        internal static void Fatal(string msg) => Fatal(msg, null, false, AssemblyName);

        internal static void Log(Level logLevel, string msg)
        {
            if (msg == null) // Return if there is no messages
                return;

            switch (logLevel)
            {
                case Level.Debug:
                    Debug(msg);
                    break;
                case Level.Warn:
                    Warn(msg);
                    break;
                case Level.Error:
                    Error(msg);
                    break;
                case Level.Fatal:
                    Fatal(msg);
                    break;
                default: // Defaults to informational logging
                    Info(msg);
                    break;
            }
        }

        #endregion

        #region Public functions (used by mods)

        /// <summary>Used to know if debug logging is enabled or not.</summary>
        public static bool DebugLogsEnabled => Config.EnableDebugLogs;

        /// <summary>
        /// This function will log given message and/or exception. It can optionally show the message on screen.
        /// You need to provide a message and/or an exception (this function will do nothing if both are set to null).
        /// Warning: You can call this function from any mod but don't call it from QModManager (<see cref="GetCallingAssemblyName"/> would fail).
        /// </summary>
        /// <param name="logLevel">The level of the log.</param>
        /// <param name="msg">Optional: The message that needs to be logged.</param>
        /// <param name="ex">Optional: The exception that needs to be logged.</param>
        /// <param name="showOnScreen">Optional: Whether to show the message on screen or not.</param>
        public static void Log(Level logLevel, string msg = null, Exception ex = null, bool showOnScreen = false)
        {
            if (ex != null)
            {
                // If exception was provided, concatenate its message to the log message
                if (logLevel != Level.Error && logLevel != Level.Fatal)
                    msg = (msg == null) ? ex.Message : msg + Environment.NewLine + ex.Message;
            }
            else if (msg == null) // Return if both given message and exception were null
                return;

            switch (logLevel)
            {
                case Level.Debug:
                    Debug(msg, showOnScreen, null, false);
                    break;
                case Level.Warn:
                    Warn(msg, showOnScreen, null);
                    break;
                case Level.Error:
                    Error(msg, ex, showOnScreen, null);
                    break;
                case Level.Fatal:
                    Fatal(msg, ex, showOnScreen, null);
                    break;
                default: // Defaults to informational logging
                    Info(msg, showOnScreen, null);
                    break;
            }
        }

        #endregion
    }
}
