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

        private static void Debug(bool selfAssembly = false, params string[] msgs)
        {
            if (Config.EnableDebugLogs && msgs != null && msgs.Length > 0)
            {
                string callingAssembly = selfAssembly ? AssemblyName : GetCallingAssemblyName();
                foreach (string msg in msgs)
                    Debug(msg, false, callingAssembly, false);
            }
        }

        private static void DebugForce(bool selfAssembly = false, params string[] msgs)
        {
            if (msgs != null && msgs.Length > 0)
            {
                string callingAssembly = selfAssembly ? AssemblyName : GetCallingAssemblyName();
                foreach (string msg in msgs)
                    Debug(msg, false, callingAssembly, true);
            }
        }

        private static void Info(string msg, bool showOnScreen = false, string callingAssembly = null)
        {
            Console.WriteLine($"[{callingAssembly ?? GetCallingAssemblyName()}:INFO] {msg}");

            if (showOnScreen)
                ErrorMessage.AddMessage(msg);
        }

        private static void Info(bool selfAssembly = false, params string[] msgs)
        {
            if (msgs != null && msgs.Length > 0)
            {
                string callingAssembly = selfAssembly ? AssemblyName : GetCallingAssemblyName();
                foreach (string msg in msgs)
                    Info(msg, false, callingAssembly);
            }
        }

        private static void Warn(string msg, bool showOnScreen = false, string callingAssembly = null)
        {
            Console.WriteLine($"[{callingAssembly ?? GetCallingAssemblyName()}:WARN] {msg}");

            if (showOnScreen)
                ErrorMessage.AddWarning(msg);
        }

        private static void Warn(bool selfAssembly = false, params string[] msgs)
        {
            if (msgs != null && msgs.Length > 0)
            {
                string callingAssembly = selfAssembly ? AssemblyName : GetCallingAssemblyName();
                foreach (string msg in msgs)
                    Warn(msg, false, callingAssembly);
            }
        }

        private static void Error(string msg = null, Exception ex = null, bool showOnScreen = false, string callingAssembly = null)
        {
            if (ex != null)
                msg = (string.IsNullOrEmpty(msg) ? ex.ToString() : msg + Environment.NewLine + ex.ToString());

            Console.WriteLine($"[{callingAssembly ?? GetCallingAssemblyName()}:ERROR] {msg}");

            if (showOnScreen && !string.IsNullOrEmpty(msg))
                ErrorMessage.AddError(msg);
        }

        private static void Error(bool selfAssembly = false, params string[] msgs)
        {
            if (msgs != null && msgs.Length > 0)
            {
                string callingAssembly = selfAssembly ? AssemblyName : GetCallingAssemblyName();
                foreach (string msg in msgs)
                    Error(msg, null, false, callingAssembly);
            }
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

        private static void Fatal(bool selfAssembly = false, params string[] msgs)
        {
            if (msgs != null && msgs.Length > 0)
            {
                string callingAssembly = selfAssembly ? AssemblyName : GetCallingAssemblyName();
                foreach (string msg in msgs)
                    Fatal(msg, null, false, callingAssembly);
            }
        }

        #endregion

        #region Internal functions (used by QModManager)

        internal static void Debug(params string[] msgs) => Debug(true, msgs);

        internal static void DebugForce(params string[] msgs) => DebugForce(true, msgs);

        internal static void Info(params string[] msgs) => Info(true, msgs);

        internal static void Warn(params string[] msgs) => Warn(true, msgs);

        internal static void Error(params string[] msgs) => Error(true, msgs);

        internal static void Exception(Exception e) => Exception(e, true);

        internal static void Fatal(params string[] msgs) => Fatal(true, msgs);

        internal static void Log(Level logLevel, params string[] msgs)
        {
            if (msgs == null) // Return if there is no messages
                return;

            switch (logLevel)
            {
                case Level.Debug:
                    Debug(true, msgs);
                    break;
                case Level.Warn:
                    Warn(true, msgs);
                    break;
                case Level.Error:
                    Error(true, msgs);
                    break;
                case Level.Fatal:
                    Fatal(true, msgs);
                    break;
                default: // Defaults to informational logging
                    Info(true, msgs);
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
        /// Warning: Do not call this function from QModManager.
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
                    Debug(msg, showOnScreen);
                    break;
                case Level.Warn:
                    Warn(msg, showOnScreen);
                    break;
                case Level.Error:
                    Error(msg, ex, showOnScreen);
                    break;
                case Level.Fatal:
                    Fatal(msg, ex, showOnScreen);
                    break;
                default: // Defaults to informational logging
                    Info(msg, showOnScreen);
                    break;
            }
        }

        #endregion
    }
}
