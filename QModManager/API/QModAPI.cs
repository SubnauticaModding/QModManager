using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace QModManager.API
{
    public partial class QModAPI : IQModAPI
    {
        public static IQModAPI Main { get; } = new QModAPI();
        private QModAPI() { }

        internal static List<Assembly> ErroredMods = new List<Assembly>();

        public static void MarkAsErrored(Assembly modAssembly = null)
            => Main.MarkAsErrored(modAssembly);

        public static ReadOnlyCollection<IQMod> GetAllMods(bool includeUnloaded = false, bool includeErrored = false)
            => Main.GetAllMods(includeUnloaded, includeErrored);

        public static IQMod GetMyMod()
            => Main.GetMyMod();
        public static IQMod GetMod(Assembly modAssembly, bool includeUnloaded = false, bool includeErrored = false)
            => Main.GetMod(modAssembly, includeUnloaded, includeErrored);
        public static IQMod GetMod(string id, bool includeUnloaded = false, bool includeErrored = false)
            => Main.GetMod(id, includeUnloaded, includeErrored);

        public static bool ModPresent(Assembly modAssembly, bool includeUnloaded = false, bool includeErrored = false)
            => Main.ModPresent(modAssembly, includeUnloaded, includeErrored);
        public static bool ModPresent(string id, bool includeUnloaded = false, bool includeErrored = false)
            => Main.ModPresent(id, includeUnloaded, includeErrored);

        #region Non-static

        void IQModAPI.MarkAsErrored()
            => MarkAsErrored(null);
        void IQModAPI.MarkAsErrored(Assembly modAssembly)
        {
            modAssembly = modAssembly ?? Assembly.GetCallingAssembly();

            if (ErroredMods.Contains(modAssembly)) return;

            ErroredMods.Add(modAssembly);
        }

        ReadOnlyCollection<IQMod> IQModAPI.GetAllMods()
            => GetAllMods(false, false);
        ReadOnlyCollection<IQMod> IQModAPI.GetAllMods(bool includeUnloaded)
            => GetAllMods(includeUnloaded, false);
        ReadOnlyCollection<IQMod> IQModAPI.GetAllMods(bool includeUnloaded, bool includeErrored)
        {
            if (includeErrored)
                return Patcher.foundMods.Select(qmod => (IQMod)qmod).ToList().AsReadOnly();
            else if (includeUnloaded)
                return Patcher.sortedMods.Select(qmod => (IQMod)qmod).ToList().AsReadOnly();
            else
                return Patcher.loadedMods.Select(qmod => (IQMod)qmod).ToList().AsReadOnly();
        }

        IQMod IQModAPI.GetMyMod()
            => GetMod(Assembly.GetCallingAssembly(), true, true);

        IQMod IQModAPI.GetMod(Assembly modAssembly)
            => GetMod(modAssembly, false, false);
        IQMod IQModAPI.GetMod(Assembly modAssembly, bool includeUnloaded)
            => GetMod(modAssembly, includeUnloaded, false);
        IQMod IQModAPI.GetMod(Assembly modAssembly, bool includeUnloaded, bool includeErrored)
        {
            if (modAssembly == null) return null;

            foreach (QMod mod in GetAllMods(includeUnloaded, includeErrored))
            {
                if (mod.LoadedAssembly == modAssembly)
                    return mod;
            }

            return null;
        }

        IQMod IQModAPI.GetMod(string id)
            => GetMod(id, false, false);
        IQMod IQModAPI.GetMod(string id, bool includeUnloaded)
            => GetMod(id, includeUnloaded, false);
        IQMod IQModAPI.GetMod(string id, bool includeUnloaded, bool includeErrored)
        {
            if (string.IsNullOrEmpty(id)) return null;

            foreach (QMod mod in GetAllMods(includeUnloaded, includeErrored))
            {
                if (mod.Id == Regex.Replace(id, Patcher.IDRegex, "", RegexOptions.IgnoreCase))
                    return mod;
            }

            return null;
        }

        bool IQModAPI.ModPresent(Assembly modAssembly)
            => ModPresent(modAssembly, false, false);
        bool IQModAPI.ModPresent(Assembly modAssembly, bool includeUnloaded)
            => ModPresent(modAssembly, includeUnloaded, false);
        bool IQModAPI.ModPresent(Assembly modAssembly, bool includeUnloaded, bool includeErrored)
            => GetMod(modAssembly, includeUnloaded, includeErrored) != null;

        bool IQModAPI.ModPresent(string id)
            => ModPresent(id, false, false);
        bool IQModAPI.ModPresent(string id, bool includeUnloaded)
            => ModPresent(id, includeUnloaded, false);
        bool IQModAPI.ModPresent(string id, bool includeUnloaded, bool includeErrored)
            => GetMod(id, includeUnloaded, includeErrored) != null;
        
        #endregion
    }

    public partial interface IQModAPI
    {
        void MarkAsErrored();
        void MarkAsErrored(Assembly modAssembly);

        ReadOnlyCollection<IQMod> GetAllMods();
        ReadOnlyCollection<IQMod> GetAllMods(bool includeUnloaded);
        ReadOnlyCollection<IQMod> GetAllMods(bool includeUnloaded, bool includeErrored);

        IQMod GetMyMod();

        IQMod GetMod(Assembly modAssembly);
        IQMod GetMod(Assembly modAssembly, bool includeUnloaded);
        IQMod GetMod(Assembly modAssembly, bool includeUnloaded, bool includeErrored);

        IQMod GetMod(string id);
        IQMod GetMod(string id, bool includeUnloaded);
        IQMod GetMod(string id, bool includeUnloaded, bool includeErrored);

        bool ModPresent(Assembly modAssembly);
        bool ModPresent(Assembly modAssembly, bool includeUnloaded);
        bool ModPresent(Assembly modAssembly, bool includeUnloaded, bool includeErrored);

        bool ModPresent(string id);
        bool ModPresent(string id, bool includeUnloaded);
        bool ModPresent(string id, bool includeUnloaded, bool includeErrored);
    }
}
