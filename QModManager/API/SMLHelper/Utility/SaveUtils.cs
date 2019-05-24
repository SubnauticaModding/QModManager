namespace QModManager.API.SMLHelper.Utility
{
    using System.IO;

    /* TODO: Maybe add more saving related functions here,
       such as those related to serializing? */
    public static class SaveUtils
    {
        /// <summary>
        /// Returns the path to the current save slot's directory.
        /// </summary>
        public static string GetCurrentSaveDataDir()
        {
            return Path.Combine(SNUtils.savedGamesDir, Utils.GetSavegameDir());
        }
    }
}
