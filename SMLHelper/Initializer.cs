namespace SMLHelper.V2
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
                InitializeOld(); // Some patch methods add values/call methods to V2 patchers, and so they need to called first.
                Initialize();
            }
            catch (Exception e)
            {
                Logger.Error($"Caught exception while trying to initialize SMLHelper{Environment.NewLine}{e}");
            }
        }

        internal static void InitializeOld()
        {
            // Some classes only have methods, no lists/dictionaries, and so no need to patch them
            // Some other classes, like PrefabDatabasePatcher, get data from other classes.
#pragma warning disable CS0618 // Type or member is obsolete
            SMLHelper.CustomPrefabHandler.Patch();
            SMLHelper.CustomSpriteHandler.Patch();

            SMLHelper.Patchers.CraftDataPatcher.Patch();
            SMLHelper.Patchers.CraftTreePatcher.Patch();
            SMLHelper.Patchers.DevConsolePatcher.Patch();
            SMLHelper.Patchers.LanguagePatcher.Patch();
            SMLHelper.Patchers.KnownTechPatcher.Patch();
#pragma warning restore CS0618 // Type or member is obsolete
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
