namespace QModManager.API
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface of services that other mods can make use of.
    /// </summary>
    public interface IQModServices
    {
        /// <summary>
        /// Gets a list successfully loaded mods.
        /// </summary>
        /// <returns>
        /// The loaded mods.
        /// </returns>
        List<IQMod> GetLoadedMods();
    }
}