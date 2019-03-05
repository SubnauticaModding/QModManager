using QModManager.Utility;
using System.Collections.Generic;
using System.Reflection;

namespace QModManager
{
    public static class PatchManager
    {
        internal static List<Assembly> ErroredMods = new List<Assembly>();

        public static void MarkAsErrored(Assembly modAssembly = null)
        {
            modAssembly = modAssembly ?? Assembly.GetCallingAssembly();

            if (ErroredMods.Contains(modAssembly))
            {
                return;
            }

            ErroredMods.Add(modAssembly);
        }
    }
}
