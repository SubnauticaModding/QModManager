namespace SMLHelper.V2.Handlers
{
    using Interfaces;
    using Patchers;

    /// <summary>
    /// Handles Encyclopedia.
    /// </summary>
    public class PDAEncyclopediaHandler : IPDAEncyclopediaHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static IPDAEncyclopediaHandler Main { get; } = new PDAEncyclopediaHandler();

        private PDAEncyclopediaHandler()
        {
            // Hide constructor
        }

        void IPDAEncyclopediaHandler.AddCustomEntry(PDAEncyclopedia.EntryData entry)
        {
            if (PDAEncyclopediaPatcher.CustomEntryData.ContainsKey(entry.key))
                Logger.Log($"{entry.key} already has custom PDAEncyclopedia.EntryData. Replacing with latest.", LogLevel.Debug);

            PDAEncyclopediaPatcher.CustomEntryData[entry.key] = entry;
        }
        
        /// <summary>
        /// Adds custom entry.
        /// </summary>
        /// <param name="entry">The <see cref="PDAEncyclopedia.EntryData"/> entry.</param>
        public static void AddCustomEntry(PDAEncyclopedia.EntryData entry)
        {
            Main.AddCustomEntry(entry);
        }
    }
}
