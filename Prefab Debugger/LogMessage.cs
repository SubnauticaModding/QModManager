using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueFire.Debugger
{
    public class LogMessage
    {
        public readonly string logString;
        public readonly string stackTrace;
        public readonly LogType type;

        public LogMessage(string _logString, string _stackTrace, LogType _type)
        {
            logString = _logString;
            stackTrace = _stackTrace;
            type = _type;
        }
    }
}