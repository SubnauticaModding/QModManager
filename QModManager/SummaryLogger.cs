namespace QModManager
{
    using System;
    using System.Collections.Generic;
    using QModManager.API.ModLoading;
    using QModManager.API.ModLoading.Internal;
    using QModManager.DataStructures;
    using Logger = Utility.Logger;

    internal static class SummaryLogger
    {
        internal static void LogSummaries(PairedList<QMod, ModStatus> mods)
        {
            CheckOldHarmony(mods.Keys);
            LogStatus(mods, ModStatus.CanceledByAuthor, "The following mods have been disabled internally by the mod author", Logger.Level.Info);
            LogStatus(mods, ModStatus.CanceledByUser, "The following mods have been disabled by user configuration", Logger.Level.Info);
            LogStatus(mods, ModStatus.PatchMethodFailed, "The following mods failed during patching", Logger.Level.Error);
            LogStatus(mods, ModStatus.TooManyPatchMethods, "The following mods had too many patch methods and were canceled", Logger.Level.Warn);
            LogStatus(mods, ModStatus.MissingPatchMethod, "The following mods had no patch methods to run", Logger.Level.Warn);
            LogStatus(mods, ModStatus.CircularLoadOrder, "The following mods could not be loaded due to circular load order", Logger.Level.Warn);
            LogStatus(mods, ModStatus.CircularDependency, "The following mods could not be loaded due to circular dependencies", Logger.Level.Warn);
            LogStatus(mods, ModStatus.MissingDependency, "The following mods could not be loaded due to missing dependencies", Logger.Level.Warn);
            LogStatus(mods, ModStatus.CurrentGameNotSupported, "The following mods do not support the current game", Logger.Level.Warn);
            LogStatus(mods, ModStatus.FailedIdentifyingGame, "Could not identify the supported game for the following mods did not ", Logger.Level.Warn);
            LogStatus(mods, ModStatus.DuplicateIdDetected, "Found the following duplicate mods", Logger.Level.Warn);
            LogStatus(mods, ModStatus.DuplicatePatchAttemptDetected, "Found the following mods attempted duplicate patching", Logger.Level.Error);
            LogStatus(mods, ModStatus.MissingCoreInfo, "The following mods could not be loaded for patching due to missing core data", Logger.Level.Warn);
            LogStatus(mods, ModStatus.InvalidCoreInfo, "The following mods could not be loaded for patching due to invalid core data", Logger.Level.Warn);
            LogStatus(mods, ModStatus.MissingAssemblyFile, "The following mods had no DLL file to load", Logger.Level.Warn);
            LogStatus(mods, ModStatus.FailedLoadingAssemblyFile, "The following mods failing loading their DLL files", Logger.Level.Warn);
            LogStatus(mods, ModStatus.UnidentifiedMod, "The following mods could not be identified for loading", Logger.Level.Warn);
        }

        private static void LogStatus(PairedList<QMod, ModStatus> mods, ModStatus statusToReport, string summary, Logger.Level logLevel)
        {
            if (mods.ValueCount(statusToReport) == 0)
                return;

            Logger.Log(logLevel, summary);
            foreach (Pair<QMod, ModStatus> pair in mods)
            {
                if (pair.Value == statusToReport)
                {
                    QMod mod = pair.Key;
                    Console.WriteLine($"- {mod.DisplayName} ({mod.Id})");
                }
            }
        }

        private static void CheckOldHarmony(IEnumerable<QMod> mods)
        {
            var modsThatUseOldHarmony = new List<QMod>();

            foreach (QMod mod in mods)
            {
                if (mod.IsLoaded && mod.HarmonyOutdated)
                    modsThatUseOldHarmony.Add(mod);
            }

            if (modsThatUseOldHarmony.Count > 0)
            {
                Logger.Warn($"Some mods are using an old version of harmony! This will NOT cause any problems, but it's not recommended:");
                foreach (QMod mod in modsThatUseOldHarmony)
                {
                    Console.WriteLine($"- {mod.DisplayName} ({mod.Id})");
                }
            }
        }
    }
}
