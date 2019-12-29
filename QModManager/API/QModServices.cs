namespace QModManager.API
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using QModManager.Patching;

    /// <summary>
    /// Services offered to mods.
    /// </summary>
    /// <seealso cref="IQModServices" />
    public class QModServices : IQModServices
    {
        private static readonly Dictionary<string, IQMod> knownMods = new Dictionary<string, IQMod>();

        internal static void LoadKnownMods(List<QMod> loadedMods)
        {
            foreach (QMod mod in loadedMods)
                knownMods.Add(mod.Id, mod);
        }

        private QModServices()
        {
        }

        /// <summary>
        /// Gets the main entry point into the QMod Services.
        /// </summary>
        /// <value>
        /// The main.
        /// </value>
        public static IQModServices Main { get; } = new QModServices();

        /// <summary>
        /// Finds the mod by identifier.
        /// </summary>
        /// <param name="modId">The mod identifier.</param>
        /// <returns></returns>
        public IQMod FindModById(string modId)
        {
            if (knownMods.TryGetValue(modId, out IQMod mod))
            {
                return mod;
            }

            return null;
        }

        /// <summary>
        /// Checks whether or not a mod is present based on its ID.
        /// </summary>
        /// <param name="modId">The mod ID.</param>
        /// <returns>
        ///   <c>True</c> if the mod is in the collection of mods to load; Otherwise <c>false</c>.
        /// </returns>
        public bool ModPresent(string modId)
        {
            return knownMods.ContainsKey(modId);
        }

        /// <summary>
        /// Finds the mod by assembly.
        /// </summary>
        /// <param name="modAssembly">The mod assembly.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IQMod FindModByAssembly(Assembly modAssembly)
        {
            foreach (IQMod mod in knownMods.Values)
            {
                if (mod.LoadedAssembly == modAssembly)
                    return mod;
            }

            return null;
        }

        /// <summary>
        /// Gets a list all mods being tracked by QModManager.
        /// </summary>
        /// <returns>
        /// A read only list of mods containing all of the loaded mods
        /// </returns>
        public ReadOnlyCollection<IQMod> GetAllMods()
        {
            return new ReadOnlyCollection<IQMod>(new List<IQMod>(knownMods.Values));
        }

        /// <summary>
        /// Returns the mod from the assembly which called this method
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IQMod GetMyMod()
        {
            var modAssembly = Assembly.GetCallingAssembly();
            return FindModByAssembly(modAssembly);
        }

        /// <summary>
        /// Returns a mod from an <see cref="Assembly" />
        /// </summary>
        /// <param name="modAssembly"></param>
        /// <returns></returns>
        public IQMod GetMod(Assembly modAssembly)
        {
            return FindModByAssembly(modAssembly);
        }

        /// <summary>
        /// Returns a mod from an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQMod GetMod(string id)
        {
            return FindModById(id);
        }
    }
}
