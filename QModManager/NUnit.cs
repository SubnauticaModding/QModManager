namespace QModManager
{
    internal partial class QModPatcher
    {
        internal static void ClearModLists()
        {
            loadedMods.Clear();
            foundMods.Clear();
            sortedMods.Clear();
            erroredMods.Clear();
        }
    }
}
