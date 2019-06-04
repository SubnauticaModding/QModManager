namespace QModManager.API.ModLoading
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Harmony;
    using QModManager.API.ModLoading.Internal;
    using QModManager.API.SMLHelper.Patchers;
    using QModManager.Utility;

    internal class Initializer
    {
        private readonly Game currentGame;
        internal readonly IDictionary<string, ModLoadingResults> NonLoadedMods = new Dictionary<string, ModLoadingResults>();
        internal readonly IDictionary<ModLoadingResults, int> ErrorTotals = new Dictionary<ModLoadingResults, int>
        {
            { ModLoadingResults.Failure, 0 },
            { ModLoadingResults.AlreadyLoaded, 0 },
            { ModLoadingResults.CurrentGameNotSupported, 0 },
        };

        internal int FailedToLoad { get; private set; }

        internal Initializer(Game currentlyRunningGame)
        {
            currentGame = currentlyRunningGame;
        }

        internal void InitializeMods<Q>(ICollection<Q> modsToInitialize)
            where Q : IQModLoadable
        {
            InitializeMods(modsToInitialize, PatchingOrder.PreInitialize);
            InitializeMods(modsToInitialize, PatchingOrder.NormalInitialize);
            InitializeMods(modsToInitialize, PatchingOrder.PostInitialize);
            FinalInitialize();

            LogResults(ModLoadingResults.Failure, "The following mods failed to load to the errors during their initialization");
            LogResults(ModLoadingResults.AlreadyLoaded, "The following mods encountered duplicate initialization attempts");
            LogResults(ModLoadingResults.CurrentGameNotSupported, $"The following mods for '{GetOtherGame()}' were skipped");

            this.FailedToLoad = CountModsFailedToLoad(modsToInitialize);
        }

        private int CountModsFailedToLoad<Q>(ICollection<Q> mods)
            where Q : IQModLoadable
        {
            int failedToLoad = 0;
            foreach (IQModLoadable mod in mods)
            {
                if (!mod.IsLoaded)
                {
                    failedToLoad++;
                }
            }

            return failedToLoad;
        }

        private void FinalInitialize()
        {
            if (currentGame == Game.None)
                return; // Test mode

            UpdateSMLHelper();
            PatchSMLHelper();
        }

        private void InitializeMods<Q>(ICollection<Q> modsToInitialize, PatchingOrder order)
            where Q : IQModLoadable
        {
            foreach (IQModLoadable mod in modsToInitialize)
            {
                ModLoadingResults result = mod.TryLoading(order, currentGame);
                switch (result)
                {
                    case ModLoadingResults.Success:
                        Logger.Info($"Successfully completed {order}Patch for [{mod.Id}]");
                        break;                    
                    case ModLoadingResults.Failure:
                        NonLoadedMods[mod.Id] = ModLoadingResults.Failure;
                        ErrorTotals[ModLoadingResults.Failure]++;
                        break;
                    case ModLoadingResults.AlreadyLoaded:
                        NonLoadedMods[mod.Id] = ModLoadingResults.AlreadyLoaded;
                        ErrorTotals[ModLoadingResults.AlreadyLoaded]++;
                        break;
                    case ModLoadingResults.CurrentGameNotSupported:
                        NonLoadedMods[mod.Id] = ModLoadingResults.CurrentGameNotSupported;
                        ErrorTotals[ModLoadingResults.CurrentGameNotSupported]++;
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
                HarmonyInstance Harmony = Patcher.Harmony;

                CustomFishPatcher.Patch(Harmony);
                TechTypePatcher.Patch(Harmony);
                CraftTreeTypePatcher.Patch(Harmony);
                CraftDataPatcher.Patch(Harmony);
                CraftTreePatcher.Patch(Harmony);
                DevConsolePatcher.Patch(Harmony);
                LanguagePatcher.Patch(Harmony);
                ResourcesPatcher.Patch(Harmony);
                PrefabDatabasePatcher.Patch(Harmony);
                SpritePatcher.Patch();
                KnownTechPatcher.Patch(Harmony);
                BioReactorPatcher.Patch(Harmony);
                OptionsPanelPatcher.Patch(Harmony);
                ItemsContainerPatcher.Patch(Harmony);
                PDAPatcher.Patch(Harmony);
                ItemActionPatcher.Patch(Harmony);
                TooltipPatcher.Patch(Harmony);
            }
            catch (Exception e)
            {
                Logger.Error($"Caught an exception while trying to initialize SMLHelper");
                Logger.Exception(e);
            }
        }

        internal void LogResults(ModLoadingResults result, string firstLogLine)
        {
            if (ErrorTotals[result] > 0)
            {
                var toWrite = new List<string> { firstLogLine };

                foreach (KeyValuePair<string, ModLoadingResults> mod in NonLoadedMods)
                {
                    if (mod.Value != result)
                        continue;

                    toWrite.Add($"[{mod.Key}]");
                }

                Logger.Warn(toWrite.ToArray());
            }
        }

        private string GetOtherGame()
        {
            switch (currentGame)
            {
                case Game.Subnautica:
                    return "BelowZero";
                case Game.BelowZero:
                    return "Subnautica";
                default:
                    return "Unknown";
            }
        }
    }
}
