namespace QModManager.API
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface of services that other mods can make use of.
    /// </summary>
    public interface IQModServices
    {
        /// <summary>
        /// Gets a list all mods being tracked by QModManager.
        /// </summary>
        /// <returns>
        /// A <see cref="List{IQMod}"/> containing all mods recognized by the mod loader.
        /// </returns>
        List<IQMod> GetAllMods();

        /// <summary>
        /// Finds a specific mod by its unique <see cref="IQMod.Id"/> value.
        /// </summary>
        /// <param name="modId">The mod ID.</param>
        /// <returns>The <see cref="IQMod"/> instance of the mod if found; otherwise returns <c>null</c.></returns>
        IQMod FindModById(string modId);
    }
}