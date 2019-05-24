namespace QModManager.API.SMLHelper
{
    using System;
    using System.Reflection;
    using Harmony;
    using Patchers;

    public class Initializer
    {
        private static HarmonyInstance harmony;

        public static void Patch()
        {
            Logger.Initialize();

            Logger.Log($"Loading v{Assembly.GetExecutingAssembly().GetName().Version}...", LogLevel.Info);

            harmony = HarmonyInstance.Create("com.ahk1221.smlhelper");

            try
            {
                Initialize();
            }
            catch (Exception e)
            {
                Logger.Error($"Caught exception while trying to initialize SMLHelper{Environment.NewLine}{e}");
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
            //TooltipPatcher.Patch(harmony);
        }
    }
}
