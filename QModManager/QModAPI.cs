using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace QModManager
{
    public static partial class QModAPI
    {
        internal static List<Assembly> ErroredMods = new List<Assembly>();
        public static void MarkAsErrored(Assembly modAssembly = null)
        {
            if (ErroredMods.Contains(modAssembly)) return;

            modAssembly = modAssembly ?? Assembly.GetCallingAssembly();
            ErroredMods.Add(modAssembly);
        }

        public static ReadOnlyCollection<QMod> GetAllMods(bool includeUnloaded = false, bool includeErrored = false)
        {
            if (includeErrored)
                return Patcher.foundMods.AsReadOnly();
            else if (includeUnloaded)
                return Patcher.sortedMods.AsReadOnly();
            else
                return Patcher.loadedMods.AsReadOnly();
        }

        public static QMod GetMod(Assembly modAssembly = null, bool includeUnloaded = false, bool includeErrored = false)
        {
            modAssembly = modAssembly ?? Assembly.GetCallingAssembly();

            foreach (QMod mod in GetAllMods(includeUnloaded, includeErrored))
                if (mod.LoadedAssembly == modAssembly) return mod;
            return null;
        }
        public static QMod GetMod(string id, bool includeUnloaded = false, bool includeErrored = false)
        {
            if (string.IsNullOrEmpty(id)) return null;
            foreach (QMod mod in GetAllMods(includeUnloaded, includeErrored))
                if (mod.Id == Regex.Replace(id, "[^0-9a-z_]", "", RegexOptions.IgnoreCase)) return mod;
            return null;
        }

        public static bool ModPresent(Assembly modAssembly, bool includeUnloaded = false, bool includeErrored = false)
            => GetMod(modAssembly, includeUnloaded, includeErrored) != null;
        public static bool ModPresent(string id, bool includeUnloaded = false, bool includeErrored = false)
            => GetMod(id, includeUnloaded, includeErrored) != null;
    }
}
