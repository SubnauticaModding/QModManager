namespace QModManager.API.ModLoading
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using QModManager.API.ModLoading.Internal;
    using QModManager.API.SMLHelper.Patchers;
    using QModManager.DataStructures;
    using QModManager.Utility;

    internal class Initializer
    {
        private readonly QModGame currentGame;        

        internal Initializer(QModGame currentlyRunningGame)
        {
            currentGame = currentlyRunningGame;
        }

        internal void InitializeMods(PairedList<QMod, ModStatus> modsToInitialize)
        {
            InitializeMods(modsToInitialize, PatchingOrder.PreInitialize);
            InitializeMods(modsToInitialize, PatchingOrder.NormalInitialize);
            InitializeMods(modsToInitialize, PatchingOrder.PostInitialize);
            FinalInitialize();

            //LogResults(ModLoadingResults.Failure, "The following mods failed to load to the errors during their initialization");
            //LogResults(ModLoadingResults.AlreadyLoaded, "The following mods encountered duplicate initialization attempts");
            //LogResults(ModLoadingResults.CurrentGameNotSupported, $"The following mods for '{GetOtherGame()}' were skipped");
        }

        private void FinalInitialize()
        {
            if (currentGame == QModGame.None)
                return; // Test mode

            UpdateSMLHelper();
            PatchSMLHelper();
        }

        private void InitializeMods(PairedList<QMod, ModStatus> modsToInitialize, PatchingOrder order)
        {
            foreach (Pair<QMod, ModStatus> pair in modsToInitialize)
            {
                QMod mod = pair.Key;
                ModLoadingResults result = mod.TryLoading(order, currentGame);
                switch (result)
                {
                    case ModLoadingResults.Success:
                        Logger.Info($"Successfully completed {order}Patch for [{mod.Id}]");
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

        private static void UpdateSMLHelper()
        {
            Logger.Info($"Checking for legacy SMLHelper files");
            try
            {
                string oldPath = IOUtilities.Combine(".", "QMods", "Modding Helper");
                if (File.Exists(Path.Combine(oldPath, "SMLHelper.dll")))
                    File.Delete(Path.Combine(oldPath, "SMLHelper.dll"));
                if (File.Exists(Path.Combine(oldPath, "mod.json")))
                    File.Delete(Path.Combine(oldPath, "mod.json"));

                // TODO in #81
            }
            catch (Exception e)
            {
                Logger.Error($"Caught an exception while trying to update legacy SMLHelper");
                Logger.Exception(e);
            }
        }

        private static void PatchSMLHelper()
        {
            Logger.Info($"Loading SMLHelper...");
            try
            {
                CustomFishPatcher.Patch(Patcher.Harmony);
                TechTypePatcher.Patch(Patcher.Harmony);
                CraftTreeTypePatcher.Patch(Patcher.Harmony);
                CraftDataPatcher.Patch(Patcher.Harmony);
                CraftTreePatcher.Patch(Patcher.Harmony);
                DevConsolePatcher.Patch(Patcher.Harmony);
                LanguagePatcher.Patch(Patcher.Harmony);
                ResourcesPatcher.Patch(Patcher.Harmony);
                PrefabDatabasePatcher.Patch(Patcher.Harmony);
                SpritePatcher.Patch();
                KnownTechPatcher.Patch(Patcher.Harmony);
                BioReactorPatcher.Patch(Patcher.Harmony);
                OptionsPanelPatcher.Patch(Patcher.Harmony);
                ItemsContainerPatcher.Patch(Patcher.Harmony);
                PDAPatcher.Patch(Patcher.Harmony);
                ItemActionPatcher.Patch(Patcher.Harmony);
                TooltipPatcher.Patch(Patcher.Harmony);
            }
            catch (Exception e)
            {
                Logger.Error($"Caught an exception while trying to initialize SMLHelper");
                Logger.Exception(e);
            }
        }

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
