// Taken from https://github.com/SMLHelper/SMLHelper/blob/DevMar2019/SMLHelper/Utility/ReflectionHelper.cs#L177-L190

using System.Diagnostics;
using System.Reflection;

namespace QModManager.Utility
{
    internal static class ReflectionHelper
    {
        internal static Assembly CallingAssemblyByStackTrace(bool skipSystemAssembly = true)
        {
            var stackTrace = new StackTrace();
            StackFrame[] frames = stackTrace.GetFrames();

            foreach (StackFrame stackFrame in frames)
            {
                Assembly ownerAssembly = stackFrame.GetMethod().DeclaringType.Assembly;

                if (skipSystemAssembly && ownerAssembly.GetName().Name == "mscorlib") // in case reflection is used
                    continue;

                if (ownerAssembly != Assembly.GetExecutingAssembly())
                    return ownerAssembly;
            }

            return Assembly.GetExecutingAssembly();
        }
    }
}
