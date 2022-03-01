namespace SMLHelper.V2.Handlers
{
    using Interfaces;
    using Json;

    /// <summary>
    /// A handler class for registering your <see cref="SaveDataCache"/>.
    /// </summary>
    public class SaveDataHandler : ISaveDataHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static ISaveDataHandler Main { get; } = new SaveDataHandler();

        private SaveDataHandler()
        {
            // Hide Constructor
        }

        T ISaveDataHandler.RegisterSaveDataCache<T>()
        {
            T cache = new();

            IngameMenuHandler.Main.RegisterOnLoadEvent(() => cache.Load());
            IngameMenuHandler.Main.RegisterOnSaveEvent(() => cache.Save());

            return cache;
        }
    }
}
