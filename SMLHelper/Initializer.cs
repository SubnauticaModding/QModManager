#pragma warning disable CS0618 // Type or member is obsolete
namespace SMLHelper.V2
{
    using Harmony;
    using Patchers;
    using System;
    using UnityEngine;

    public class Initializer
    {
        private static HarmonyInstance harmony;

        public static void Patch()
        {
            harmony = HarmonyInstance.Create("com.ahk1221.smlhelper");

            try
            {
                InitializeOld(); // Some patch methods add values/call methods to V2 patchers, and so they need to called first.
                Initialize();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught" + (!String.IsNullOrEmpty(e.Message) ? ", Message: " + e.Message : ""));
                Console.WriteLine("StackTrace: " + e.StackTrace);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception caught" + (!String.IsNullOrEmpty(e.InnerException.Message) ? ", Message: " + e.InnerException.Message : ""));
                    Console.WriteLine("Inner StackTrace: " + e.InnerException.StackTrace);
                }
            }

        }

        internal static void InitializeOld()
        {
            // Some classes only have methods, no lists/dictionaries, and so no need to patch them
            // Some other classes, like PrefabDatabasePatcher, get data from other classes.
            SMLHelper.CustomPrefabHandler.Patch();
            SMLHelper.CustomSpriteHandler.Patch();

            SMLHelper.Patchers.CraftDataPatcher.Patch();
            SMLHelper.Patchers.CraftTreePatcher.Patch();
            SMLHelper.Patchers.DevConsolePatcher.Patch();
            SMLHelper.Patchers.LanguagePatcher.Patch();
            SMLHelper.Patchers.KnownTechPatcher.Patch();
        }

        internal static void Initialize()
        {
            TechTypePatcher.Patch(harmony);
            CraftTreeTypePatcher.Patch(harmony);
            CraftDataPatcher.Patch(harmony);
            CraftTreePatcher.Patch(harmony);
            DevConsolePatcher.Patch(harmony);
            PDAEncyclopediaPatcher.Patch(harmony);
            LanguagePatcher.Patch(harmony);
            ResourcesPatcher.Patch(harmony);
            PrefabDatabasePatcher.Patch(harmony);
            SpritePatcher.Patch(harmony);
            KnownTechPatcher.Patch(harmony);
            BioReactorPatcher.Patch(harmony);
        }
    }
}
