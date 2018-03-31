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

            var techType = TechTypePatcher.AddTechType("Helno", "He l o no", "xD");

            var anotherSprite = SpriteManager.Get(TechType.Aerogel);
            CraftTreePatcher.customTabs.Add(new CustomCraftTab("BatteryMenu/Hmmz", "LLOZ", CraftScheme.Workbench, anotherSprite));
            CraftTreePatcher.customTabs.Add(new CustomCraftTab("BatteryMenu/Hmmz/Asd", "Hmm", CraftScheme.Workbench, anotherSprite));
            CraftTreePatcher.customNodes.Add(new CustomCraftNode(techType, CraftScheme.Workbench, "BatteryMenu/Hmmz/Asd/Helno"));

            var sprite = SpriteManager.Get(TechType.Titanium);
            CraftTreePatcher.customTabs.Add(new CustomCraftTab("Resources/TestMods", "Test Mods", CraftScheme.Fabricator, sprite));
            CraftTreePatcher.customNodes.Add(new CustomCraftNode(techType, CraftScheme.Fabricator, "Resources/TestMods/Helno"));

            CraftTreePatcher.customTabs.Add(new CustomCraftTab("hzad", "ASD", CraftScheme.SeamothUpgrades, anotherSprite));
            CraftTreePatcher.customNodes.Add(new CustomCraftNode(techType, CraftScheme.SeamothUpgrades, "hzad/Helno"));

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
                KnownTechPatcher.Patch(harmony);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

        }
    }
}
