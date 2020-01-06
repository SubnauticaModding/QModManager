namespace QModManager.API
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;

    /// <summary>
    /// Minimal legacy support only.
    /// </summary>
    /// <seealso cref="IQModAPI" />
    [Obsolete("This is kept as legacy support only. Please switch to QModServices.", true)]
    public class QModAPI : IQModAPI
    {
        /// <summary>
        /// Minimal legacy support only.
        /// </summary>
        [Obsolete("This is kept as legacy support only. Please switch to QModServices.", true)]
        public static IQModAPI Main { get; } = new QModAPI();

        private QModAPI()
        {
        }

        /// <summary>
        /// Gets a list all mods being tracked by QModManager.
        /// </summary>
        /// <returns>A read only list of mods containing all of the loaded mods</returns>
        [Obsolete("This is kept as legacy support only. Please switch to QModServices.", true)]
        public static ReadOnlyCollection<IQMod> GetAllMods()
        {
            return QModServices.Main.GetAllMods();
        }

        /// <summary>
        /// Returns the mod from the assembly which called this method
        /// </summary>
        [Obsolete("This is kept as legacy support only. Please switch to QModServices.", true)]
        public static IQMod GetMyMod()
        {
            var modAssembly = Assembly.GetCallingAssembly();
            return QModServices.Main.FindModByAssembly(modAssembly);
        }

        /// <summary>
        /// Returns a mod from an <see cref="Assembly"/>
        /// </summary>
        /// <param name="modAssembly"></param>
        [Obsolete("This is kept as legacy support only. Please switch to QModServices.", true)]
        public static IQMod GetMod(Assembly modAssembly)
        {
            return QModServices.Main.FindModByAssembly(modAssembly);
        }

        /// <summary>
        /// Finds a specific mod by its unique <see cref="IQMod.Id"/> value.
        /// </summary>
        /// <param name="id">The mod ID.</param>
        /// <returns>The <see cref="IQMod"/> instance of the mod if found; otherwise returns <c>null</c>.</returns>
        [Obsolete("This is kept as legacy support only. Please switch to QModServices.", true)]
        public static IQMod GetMod(string id)
        {
            return QModServices.Main.FindModById(id);
        }

        /// <summary>
        /// Checks whether or not a mod is present based on its unique <see cref="IQMod.Id"/> value.
        /// </summary>
        /// <param name="id">The mod ID.</param>
        /// <returns><c>True</c> if the mod is in the collection of mods to load; Otherwise <c>false</c>.</returns>
        [Obsolete("This is kept as legacy support only. Please switch to QModServices.", true)]
        public static bool ModPresent(string id)
        {
            return QModServices.Main.ModPresent(id);
        }

        [Obsolete("This is kept as legacy support only. Please switch to QModServices.", true)]
        ReadOnlyCollection<IQMod> IQModAPI.GetAllMods()
        {
            return QModServices.Main.GetAllMods();
        }

        [Obsolete("This is kept as legacy support only. Please switch to QModServices.", true)]
        IQMod IQModAPI.GetMod(Assembly modAssembly)
        {
            return QModServices.Main.FindModByAssembly(modAssembly);
        }

        [Obsolete("This is kept as legacy support only. Please switch to QModServices.", true)]
        IQMod IQModAPI.GetMod(string id)
        {
            return QModServices.Main.FindModById(id);
        }

        [Obsolete("This is kept as legacy support only. Please switch to QModServices.", true)]
        IQMod IQModAPI.GetMyMod()
        {
            var modAssembly = Assembly.GetCallingAssembly();
            return QModServices.Main.FindModByAssembly(modAssembly);
        }

        [Obsolete("This is kept as legacy support only. Please switch to QModServices.", true)]
        bool IQModAPI.ModPresent(string id)
        {
            return QModServices.Main.ModPresent(id);
        }
    }
}
