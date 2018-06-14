using Harmony;
using SMLHelper.Patchers;
using System;

namespace SMLHelper
{
    public class Initializer
    {
        private static HarmonyInstance harmony;

        public static void Patch()
        {
            harmony = HarmonyInstance.Create("com.ahk1221.smlhelper");

            TechTypePatcher.AddTechType("Test!", "TEAT", "ASD");

            try
            {
                TechTypePatcher.Patch(harmony);
                CraftTreeTypePatcher.Patch(harmony);
                CraftDataPatcher.Patch(harmony);
                CraftTreePatcher.Patch(harmony);
                DevConsolePatcher.Patch(harmony);
                LanguagePatcher.Patch(harmony);
                ResourcesPatcher.Patch(harmony);
                PrefabDatabasePatcher.Patch(harmony);
                SpritePatcher.Patch(harmony);
                KnownTechPatcher.Patch(harmony);
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
    }
}
