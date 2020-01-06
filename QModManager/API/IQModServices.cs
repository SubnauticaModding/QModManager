namespace QModManager.API
{
    using System.Reflection;

    /// <summary>
    /// An set of services provided by QModManager for mods to use.
    /// </summary>
    public interface IQModServices : IQModAPI
    {
        /// <summary>
        /// Finds a specific mod by its unique <see cref="IQMod.Id"/> value.
        /// </summary>
        /// <param name="modId">The mod ID.</param>
        /// <returns>The <see cref="IQMod"/> instance of the mod if found; otherwise returns <c>null</c>.</returns>
        IQMod FindModById(string modId);

        /// <summary>
        /// Finds a specific mod with a <see cref="IQMod.LoadedAssembly"/> that matches the provided one.
        /// </summary>
        /// <param name="modAssembly">The mod assembly.</param>
        /// <returns>The <see cref="IQMod"/> instance of the mod if found; otherwise returns <c>null</c>.</returns>
        IQMod FindModByAssembly(Assembly modAssembly);
    }
}