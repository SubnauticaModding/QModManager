namespace SMLHelper.V2.Utility
{
    using System.IO;

    /* TODO: Maybe add more saving related functions here,
       such as those related to serializing? */
    public static class SaveUtils
    {
        public static string GetCurrentSaveDataDir()
        {
            return Path.Combine(SNUtils.savedGamesDir, Utils.GetSavegameDir());
        }
    }
}
