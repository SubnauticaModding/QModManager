namespace SMLHelper.V2.Interfaces
{
    /// <summary>
    /// Handles Encyclopedia.
    /// </summary>
    public interface IPDAEncyclopediaHandler
    {
        /// <summary>
        /// Adds custom entry.
        /// </summary>
        /// <param name="entry"></param>
        void AddCustomEntry(PDAEncyclopedia.EntryData entry);
    }
}
