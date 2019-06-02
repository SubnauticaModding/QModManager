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

        internal int FailedToLoad { get; private set; }

        internal Initializer(IEnumerable<QMod> modsToInitialize)
        {
            modsToLoad = modsToInitialize;
        }

        internal void Initialize()
        {
            PreInitialize();
            NormalInitialize();
            PostInitialize();
            FinalInitialize();

            int failedToLoad = 0;
            foreach (QMod mod in modsToLoad)
            {
                if (mod.IsLoaded)
                    failedToLoad++;
            }
            this.FailedToLoad = failedToLoad;
        }

        private void PreInitialize()
        {
            InitializeMods(modsToLoad, PatchingOrder.PreInitialize);
        }

        private void NormalInitialize()
        {
            InitializeMods(modsToLoad, PatchingOrder.NormalInitialize);
        }

        private void PostInitialize()
        {
            InitializeMods(modsToLoad, PatchingOrder.PostInitialize);
        }

        private void FinalInitialize()
        {
            try
            {
                UpdateSMLHelper();
            }
            catch (Exception e)
            {
                Logger.Error($"Caught an exception while trying to update SMLHelper");
                Logger.Exception(e);
            }

            Logger.Info($"Loading SMLHelper...");

            try
            {
                PatchSMLHelper();
            }
            catch (Exception e)
            {
                Logger.Error($"Caught an exception while trying to initialize SMLHelper");
                Logger.Exception(e);
            }
        }

        private static void InitializeMods(IEnumerable<QMod> mods, PatchingOrder order)
        {
            foreach (QMod mod in mods)
            {
                ModLoadingResults results = mod.TryLoading(order);
                switch (results)
                {
                    case ModLoadingResults.Success:
                        // TODO - Report status
                        break;
                    case ModLoadingResults.NoMethodToExecute:
                        break;
                    case ModLoadingResults.Failure:
                        break;
                    case ModLoadingResults.AlreadyLoaded:
                        break;
                }
            }
        }

        private static void UpdateSMLHelper()
        {
            string oldPath = IOUtilities.Combine(".", "QMods", "Modding Helper");
            if (File.Exists(Path.Combine(oldPath, "SMLHelper.dll")))
                File.Delete(Path.Combine(oldPath, "SMLHelper.dll"));
            if (File.Exists(Path.Combine(oldPath, "mod.json")))
                File.Delete(Path.Combine(oldPath, "mod.json"));

            // TODO in #81
        }

        private static void PatchSMLHelper()
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
    }
}
