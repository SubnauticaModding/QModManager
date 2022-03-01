namespace SMLHelper.V2.Interfaces
{
    using Json;

    /// <summary>
    /// A handler class for registering your <see cref="SaveDataCache"/>.
    /// </summary>
    public interface ISaveDataHandler
    {
        /// <summary>
        /// Registers your <see cref="SaveDataCache"/> to be automatically loaded and saved whenever the game is.
        /// </summary>
        /// <typeparam name="T">A class derived from <see cref="SaveDataCache"/> to hold your save data.</typeparam>
        /// <returns>An instance of the <typeparamref name="T"/> : <see cref="SaveDataCache"/> with values loaded
        /// from the json file on disk whenever a save slot is loaded.</returns>
        T RegisterSaveDataCache<T>() where T : SaveDataCache, new();
    }
}
