using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace QModManager
{
    public static class QModAPI
    {
        internal static List<Assembly> ErroredMods = new List<Assembly>();
        public static void MarkAsErrored(Assembly modAssembly = null)
        {
            if (ErroredMods.Contains(modAssembly)) return;

            modAssembly = modAssembly ?? Assembly.GetCallingAssembly();
            ErroredMods.Add(modAssembly);
        }

        public static QMod GetMod(Assembly modAssembly = null)
        {
            modAssembly = modAssembly ?? Assembly.GetCallingAssembly();

            foreach (QMod mod in Patcher.sortedMods)
                if (mod.LoadedAssembly == modAssembly) return mod;
            return null;
        }
        public static QMod GetMod(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            foreach (QMod mod in Patcher.sortedMods)
                if (mod.Id == Regex.Replace(id, "[^0-9a-z_]", "", RegexOptions.IgnoreCase)) return mod;
            return null;
        }

        public static bool ModPresent(Assembly modAssembly)
        {
            return GetMod(modAssembly) != null;
        }
        public static bool ModPresent(string id)
        {
            return GetMod(id) != null;
        }
    }
}
