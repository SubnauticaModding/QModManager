namespace QModManager.API.SMLHelper
{
    using Harmony;
    using Patchers;
    using QModManager.Utility;
    using System;

    internal static class Initializer
    {
        private static HarmonyInstance harmony { get => Patcher.harmony; }

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
            CustomFishPatcher.Patch(harmony);
            TechTypePatcher.Patch(harmony);
            CraftTreeTypePatcher.Patch(harmony);
            CraftDataPatcher.Patch(harmony);
            CraftTreePatcher.Patch(harmony);
            DevConsolePatcher.Patch(harmony);
            LanguagePatcher.Patch(harmony);
            ResourcesPatcher.Patch(harmony);
            PrefabDatabasePatcher.Patch(harmony);
            SpritePatcher.Patch();
            KnownTechPatcher.Patch(harmony);
            BioReactorPatcher.Patch(harmony);
            OptionsPanelPatcher.Patch(harmony);
            ItemsContainerPatcher.Patch(harmony);
            PDAPatcher.Patch(harmony);
            ItemActionPatcher.Patch(harmony);
            TooltipPatcher.Patch(harmony);
        }
    }
}
