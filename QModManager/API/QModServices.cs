namespace QModManager.API
{
    using System.Collections.Generic;
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
        /// Gets a list all mods being tracked by QModManager.
        /// </summary>
        /// <returns>
        /// A <see cref="List{IQMod}" /> containing all mods recognized by the mod loader.
        /// </returns>
        public List<IQMod> GetAllMods()
        {
            return new List<IQMod>(knownMods.Values);
        }

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
    }
}
