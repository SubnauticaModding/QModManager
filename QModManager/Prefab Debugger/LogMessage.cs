using UnityEngine;

namespace QModManager.Debugger
{
    internal class LogMessage
    {
        internal readonly string logString;
        internal readonly string stackTrace;
        internal readonly LogType type;

        internal LogMessage(string _logString, string _stackTrace, LogType _type)
        {
            logString = _logString;
            stackTrace = _stackTrace;
            type = _type;
        }
    }
}