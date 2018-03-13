using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SMLHelper.Patchers;

namespace SMLHelper
{
    public class Initializer
    {
        private static HarmonyInstance harmony;

        public static void Patch()
        {
            harmony = HarmonyInstance.Create("com.ahk1221.smlhelper");

            try
            {
                TechTypePatcher.Patch(harmony);
                CraftDataPatcher.Patch(harmony);
                CraftTreePatcher.Patch(harmony);
                DevConsolePatcher.Patch(harmony);
                LanguagePatcher.Patch(harmony);
                ResourcesPatcher.Patch(harmony);
                PrefabDatabasePatcher.Patch(harmony);
                SpritePatcher.Patch(harmony);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            harmony.Patch(typeof(MainMenuMusic).GetMethod("Stop"), null,
                new HarmonyMethod(typeof(Initializer).GetMethod("Postpatch")));
        }

        public static void Postpatch()
        {
            TechTypePatcher.Postpatch();
        }
    }
}
