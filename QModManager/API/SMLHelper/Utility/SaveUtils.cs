namespace QModManager.API.SMLHelper.Utility
{
    using System.IO;

    /// <summary>
    /// Utilities for saved games
    /// </summary>
    public static class SaveUtils
    {
        /// <summary>
        /// Returns the path to the current save slot's directory.
        /// </summary>
        public static string GetCurrentSaveDataDir()
        {
            if (Patcher.game == Patcher.Game.Subnautica)
                return Path.Combine(SNUtils.savedGamesDir, Utils.GetSavegameDir());
            else
                return SaveLoadManager.temporarySavePath;
        }
    }
}
