namespace QModManager.Utility
{
    using QModManager.Patching;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using QModManager.API;
    using QModManager.Checks;

    internal static class SummaryLogger
    {
        internal static void LogSummaries(List<QMod> mods)
        {
            CheckOldHarmony(mods);
            //LogStatus(mods, ModStatus.CanceledByAuthor, "The following mods have been disabled internally by the mod author:", Logger.Level.Info);
            LogStatus(mods, ModStatus.CanceledByUser, "The following mods are disabled by user configuration and have been skipped:", Logger.Level.Info);
            LogStatus(mods, ModStatus.PatchMethodFailed, "The following mods failed during patching:", Logger.Level.Error);
            LogStatus(mods, ModStatus.TooManyPatchMethods, "The following mods had too many patch methods and were canceled:", Logger.Level.Error);
            LogStatus(mods, ModStatus.MissingPatchMethod, "The following mods had no patch methods to run:", Logger.Level.Error);
            //LogStatus(mods, ModStatus.CircularLoadOrder, "The following mods could not be loaded due to circular load order:", Logger.Level.Error);
            //LogStatus(mods, ModStatus.CircularDependency, "The following mods could not be loaded due to circular dependencies:", Logger.Level.Error);
            LogStatus(mods, ModStatus.MissingDependency, "The following mods could not be loaded due to missing dependencies:", Logger.Level.Error);
            LogStatus(mods, ModStatus.OutOfDateDependency, "The following mods could not be loaded due to outdated dependencies:", Logger.Level.Error);
            LogStatus(mods, ModStatus.CurrentGameNotSupported, "The following mods do not support the current game:", Logger.Level.Error);
            LogStatus(mods, ModStatus.FailedIdentifyingGame, "Could not identify the supported game for the following mods:", Logger.Level.Error);
            LogStatus(mods, ModStatus.DuplicateIdDetected, "Found the following duplicate mods:", Logger.Level.Error);
            LogStatus(mods, ModStatus.DuplicatePatchAttemptDetected, "Found the following mods attempted duplicate patching:", Logger.Level.Error);
            LogStatus(mods, ModStatus.MissingCoreInfo, "The following mods could not be loaded for patching due to a missing mod.json file:", Logger.Level.Error);
            LogStatus(mods, ModStatus.InvalidCoreInfo, "The following mods could not be loaded for patching due to an invalid mod.json file:", Logger.Level.Error);
            LogStatus(mods, ModStatus.MissingAssemblyFile, "The following mods had no DLL file to load:", Logger.Level.Error);
            LogStatus(mods, ModStatus.FailedLoadingAssemblyFile, "The following mods failing loading their DLL files:", Logger.Level.Error);
            LogStatus(mods, ModStatus.UnidentifiedMod, "The following mods could not be identified for loading:", Logger.Level.Error);
            LogStatus(mods, ModStatus.BannedID, "The following mods could not be loaded because they are using a banned ID:", Logger.Level.Error);
            LogStatus(mods, ModStatus.Merged, "The following mods have been merged with QModManager and have been skipped:", Logger.Level.Warn);
        }

        private static void LogStatus(List<QMod> mods, ModStatus statusToReport, string summary, Logger.Level logLevel)
        {
            List<QMod> specificMods = mods.FindAll(mod => mod.Status == statusToReport);

            if (specificMods.Count == 0)
                return;

            Logger.Log(logLevel, summary);
            foreach (QMod mod in specificMods)
            {
                switch (statusToReport)
                {
                    case ModStatus.MissingDependency:
                    {
                        if (mod.HasDependencies)
                        {
                            Console.WriteLine($"- {mod.DisplayName} ({mod.Id}) is missing these dependencies:");
                            foreach (RequiredQMod dependency in mod.RequiredMods)
                            {
                                if (!QModServices.Main.ModPresent(dependency.Id))
                                    Console.WriteLine($"   - {dependency.Id}{(dependency.RequiresMinimumVersion ? $" at version {dependency.MinimumVersion} or newer" : string.Empty)}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"- {mod.DisplayName} ({mod.Id}) is missing a dependency but none are listed in mod.json, Please check Nexusmods for list of Dependencies.");
                        }
                        break;
                    }

                    case ModStatus.OutOfDateDependency:
                        Console.WriteLine($"- {mod.DisplayName} ({mod.Id}) requires a newer version of these dependencies:");
                        foreach (RequiredQMod dependency in mod.RequiredMods)
                        {
                            if (dependency.RequiresMinimumVersion)
                            {
                                IQMod dependencyDetails = QModServices.Main.FindModById(dependency.Id);

                                if (dependencyDetails == null || dependencyDetails.ParsedVersion < dependency.MinimumVersion)                                
                                    Console.WriteLine($"   - {dependency.Id} at version {dependency.MinimumVersion} or newer");                                
                            }
                        }
                        break;

                    default:
                        Console.WriteLine($"- {mod.DisplayName} ({mod.Id})");
                        break;
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

        internal static void ReportIssues(List<QMod> mods)
        {
            List<QMod> loadedMods = mods.FindAll(m => m.IsLoaded);
            List<QMod> skippedMods = mods.FindAll(m => !m.IsLoaded && m.Status < 0);
            List<QMod> erroredMods = mods.FindAll(m => !m.IsLoaded && m.Status > 0);

            Logger.Info($"Finished loading QModManager. Loaded {loadedMods.Count} mods.");

            if (skippedMods.Count > 0)
                Logger.Info($"A total of {skippedMods.Count} mods were skipped");

            if (erroredMods.Count > 0)
            {
                Logger.Error($"A total of {erroredMods.Count} mods failed to load");

                string message;

                switch (erroredMods.Count)
                {
                    case 1:
                        message = $"The following mod could not be loaded: {erroredMods[0].DisplayName}.";
                        break;
                    case 2:
                        message = $"The following mods could not be loaded: {erroredMods[0].DisplayName} and {erroredMods[1].DisplayName}.";
                        break;
                    case 3:
                        message = $"The following mods could not be loaded: {erroredMods[0].DisplayName}, {erroredMods[1].DisplayName} and {erroredMods[2].DisplayName}.";
                        break;
                    default:
                        message = $"The following mods could not be loaded: {erroredMods[0].DisplayName}, {erroredMods[1].DisplayName}, {erroredMods[2].DisplayName} and {erroredMods.Count - 3} others.";
                        break;
                }

                message += " Check the log for more information.";

                new Dialog()
                {
                    message = message,
                    leftButton = Dialog.Button.SeeLog,
                    rightButton = Dialog.Button.Close,
                    color = Dialog.DialogColor.Red
                }.Show();
            }
            else if (VersionCheck.result != null)
            {
                new Dialog()
                {
                    message = $"There is a newer version of QModManager available: {VersionCheck.result.ToStringParsed()} (current version: {Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()})",
                    leftButton = Dialog.Button.Download,
                    rightButton = Dialog.Button.Close,
                    color = Dialog.DialogColor.Blue
                }.Show();
            }
        }
    }
}
