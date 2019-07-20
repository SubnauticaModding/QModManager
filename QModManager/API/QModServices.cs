namespace QModManager.API
{
    using System.Collections.Generic;

    /// <summary>
    /// Services offered to mods.
    /// </summary>
    /// <seealso cref="IQModServices" />
    public class QModServices : IQModServices
    {
        internal static List<IQMod> successfullyLoadedMods = new List<IQMod>();

        /// <summary>
        /// Gets the main entry point into the QMod Services.
        /// </summary>
        /// <value>
        /// The main.
        /// </value>
        public static IQModServices Main { get; } = new QModServices();

        private QModServices()
        {
        }

        /// <summary>
        /// Gets a list successfully loaded mods.
        /// </summary>
        /// <returns>
        /// The loaded mods.
        /// </returns>
        public List<IQMod> GetLoadedMods()
        {
            return new List<IQMod>(successfullyLoadedMods);
        }
    }
}
