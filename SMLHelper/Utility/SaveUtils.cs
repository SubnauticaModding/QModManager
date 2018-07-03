namespace SMLHelper.V2.Utility
{
    using System.IO;

    /* TODO: Maybe add more saving related functions here,
       such as those related to serializing? */
    public class SaveUtils
    {
        public static string GetCurrentSaveDataDir()
        {
            return Path.Combine(@"./SNAppData/SavedGames/", Utils.GetSavegameDir());
        }
    }
}
