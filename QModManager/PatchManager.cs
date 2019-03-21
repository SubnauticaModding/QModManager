using System.Collections.Generic;
using System.Reflection;

namespace QModManager
{
    public static class PatchManager
    {
        internal static List<Assembly> ErroredMods = new List<Assembly>();

        public static void MarkAsErrored(Assembly modAssembly = null)
        {
            if (ErroredMods.Contains(modAssembly)) return;

            modAssembly = modAssembly ?? Assembly.GetCallingAssembly();
            ErroredMods.Add(modAssembly);
        }
    }
}
