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
        private readonly IEnumerable<QMod> modsToLoad;
        private readonly Game currentGame;

        internal int FailedToLoad { get; private set; }

        internal Initializer(IEnumerable<QMod> modsToInitialize, Game currentlyRunningGame)
        {
            modsToLoad = modsToInitialize;
            currentGame = currentlyRunningGame;
        }

        internal void Initialize()
        {
            InitializeMods(PatchingOrder.PreInitialize);
            InitializeMods(PatchingOrder.NormalInitialize);
            InitializeMods(PatchingOrder.PostInitialize);
            FinalInitialize();

            this.FailedToLoad = CountModsFailedToLoad();
        }

        private int CountModsFailedToLoad()
        {
            int failedToLoad = 0;
            foreach (QMod mod in modsToLoad)
            {
                if (mod.IsLoaded)
                    failedToLoad++;
            }

            return failedToLoad;
        }

        private void FinalInitialize()
        {
            UpdateSMLHelper();
            PatchSMLHelper();
        }

        private void InitializeMods(PatchingOrder order)
        {
            foreach (QMod mod in modsToLoad)
            {
                ModLoadingResults results = mod.TryLoading(order, currentGame);
                switch (results)
                {
                    case ModLoadingResults.Success:                        
                        break;// TODO - Report status
                    case ModLoadingResults.NoMethodToExecute:
                        break;
                    case ModLoadingResults.Failure:
                        break;
                    case ModLoadingResults.AlreadyLoaded:
                        break;
                    case ModLoadingResults.CurrentGameNotSupported:
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
    }
}
