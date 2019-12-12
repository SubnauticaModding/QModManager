namespace QModManager.API.ModLoading
{
    using System;
    using System.IO;
    //using QModManager.API.SMLHelper.Patchers;
    using QModManager.DataStructures;
    using QModManager.Patching;
    using QModManager.Utility;

    internal class Initializer
    {
        private readonly QModGame currentGame;

        internal Initializer(QModGame currentlyRunningGame)
        {
            currentGame = currentlyRunningGame;
        }

        internal bool InitializeMods(PairedList<QMod, ModStatus> modsToInitialize)
        {
            InitializeMods(modsToInitialize, PatchingOrder.PreInitialize);
            InitializeMods(modsToInitialize, PatchingOrder.NormalInitialize);
            InitializeMods(modsToInitialize, PatchingOrder.PostInitialize);
            return true;
            //return FinalInitialize();
        }

        //private bool FinalInitialize()
        //{
        //    if (currentGame == QModGame.None)
        //        return true; // Test mode

        //    return UpdateSMLHelper() && PatchSMLHelper();
        //}

        private void InitializeMods(PairedList<QMod, ModStatus> modsToInitialize, PatchingOrder order)
        {
            foreach (Pair<QMod, ModStatus> pair in modsToInitialize)
            {
                if (pair.Value != ModStatus.Success)
                    continue;

                QMod mod = pair.Key;
                ModLoadingResults result = mod.TryLoading(order, currentGame);
                switch (result)
                {
                    case ModLoadingResults.Success:
                        break;
                    case ModLoadingResults.Failure:
                        pair.Value = ModStatus.PatchMethodFailed;
                        break;
                    case ModLoadingResults.AlreadyLoaded:
                        pair.Value = ModStatus.DuplicatePatchAttemptDetected;
                        break;
                    case ModLoadingResults.CurrentGameNotSupported:
                        pair.Value = ModStatus.CurrentGameNotSupported;
                        break;
                }
            }
        }

        //private static bool UpdateSMLHelper()
        //{
        //    Logger.Info($"Checking for legacy SMLHelper files");
        //    try
        //    {
        //        string oldPath = IOUtilities.Combine(".", "QMods", "Modding Helper");
        //        if (File.Exists(Path.Combine(oldPath, "SMLHelper.dll")))
        //            File.Delete(Path.Combine(oldPath, "SMLHelper.dll"));
        //        if (File.Exists(Path.Combine(oldPath, "mod.json")))
        //            File.Delete(Path.Combine(oldPath, "mod.json"));

        //        // TODO

        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new FatalPatchingException("Caught an exception while trying to update legacy SMLHelper", e);
        //    }
        //}

        private string GetOtherGame()
        {
            switch (currentGame)
            {
                case QModGame.Subnautica:
                    return "BelowZero";
                case QModGame.BelowZero:
                    return "Subnautica";
                default:
                    return "Unknown";
            }
        }
    }
}
