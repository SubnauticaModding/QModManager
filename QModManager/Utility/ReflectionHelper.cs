using System.Diagnostics;
using System.Reflection;

namespace QModManager.Utility
{
    internal static class ReflectionHelper
    {
        internal static Assembly CallingAssemblyByStackTrace()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] frames = stackTrace.GetFrames();

            foreach (StackFrame stackFrame in frames)
            {
                Assembly ownerAssembly = stackFrame.GetMethod().DeclaringType.Assembly;
                if (ownerAssembly != Assembly.GetExecutingAssembly())
                    return ownerAssembly;
            }

            return Assembly.GetExecutingAssembly();
        }
    }
}
