namespace QModManager.API.SMLHelper
{
    using System;
    using System.Reflection;
    using Harmony;
    using Patchers;

    public class Initializer
    {
        private static HarmonyInstance harmony { get => Patcher.harmony; }

        public static void PrePreInit()
        {
            Logger.Initialize();
        }

        public static void PreInit()
        {

        }

        public static void Init()
        {

        }

        public static void PostInit()
        {

        }

        public static void PostPostInit()
        {
            Logger.Log($"Loading SMLHelper v{Assembly.GetExecutingAssembly().GetName().Version}...", LogLevel.Info);

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
