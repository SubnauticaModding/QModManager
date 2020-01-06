namespace QModManager.API
{
    using System.Collections.ObjectModel;
    using System.Reflection;

    /// <summary>
    /// An set of services provided by QModManager for mods to use.
    /// </summary>
    public interface IQModAPI
    {
        /// <summary>
        /// Gets a list all mods being tracked by QModManager.
        /// </summary>
        /// <returns>A read only list of mods containing all of the loaded mods</returns>
        ReadOnlyCollection<IQMod> GetAllMods();

        /// <summary>
        /// Returns the mod from the assembly which called this method
        /// </summary>
        IQMod GetMyMod();

        /// <summary>
        /// Returns a mod from an <see cref="Assembly"/>
        /// </summary>
        /// <param name="modAssembly"></param>
        IQMod GetMod(Assembly modAssembly);

        /// <summary>
        /// Finds a specific mod by its unique <see cref="IQMod.Id"/> value.
        /// </summary>
        /// <param name="id">The mod ID.</param>
        /// <returns>The <see cref="IQMod"/> instance of the mod if found; otherwise returns <c>null</c>.</returns>
        IQMod GetMod(string id);

        /// <summary>
        /// Checks whether or not a mod is present based on its unique <see cref="IQMod.Id"/> value.
        /// </summary>
        /// <param name="id">The mod ID.</param>
        /// <returns><c>True</c> if the mod is in the collection of mods to load; Otherwise <c>false</c>.</returns>
        bool ModPresent(string id);
    }
}
