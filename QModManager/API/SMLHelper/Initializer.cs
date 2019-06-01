namespace QModManager.API.SMLHelper
{
    using Harmony;
    using Patchers;
    using QModManager.Utility;
    using System;

    internal static class Initializer
    {
        private static HarmonyInstance Harmony => Patcher.Harmony;

        internal static void PostPostInit()
        {
            Logger.Info($"Loading SMLHelper...");

            try
            {
                Initialize();
            }
            catch (Exception e)
            {
                Logger.Error($"Caught an exception while trying to initialize SMLHelper");
                Logger.Exception(e);
            }
        }

        internal static void Initialize()
        {
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
