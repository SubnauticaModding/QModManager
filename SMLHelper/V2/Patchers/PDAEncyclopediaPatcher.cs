namespace SMLHelper.V2.Patchers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using Harmony;

    internal class PDAEncyclopediaPatcher
    {
        internal static void Patch(HarmonyInstance harmony)
        {
            var initMethod = typeof(PDAEncyclopedia).GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);
            var initPostfix = typeof(PDAEncyclopediaPatcher).GetMethod("InitializePostfix", BindingFlags.Static | BindingFlags.NonPublic);

            harmony.Patch(initMethod, null, new HarmonyMethod(initPostfix));
        }

        internal static void InitializePostfix()
        {
            var mapping = (Dictionary<string, PDAEncyclopedia.EntryData>)typeof(PDAEncyclopedia).GetField("mapping", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            foreach(var entry in mapping)
            {
                Console.WriteLine("Key: " + entry.Key);
                Console.WriteLine("EntryData key: " + entry.Value.key);
                Console.WriteLine("EntryData Path: " + entry.Value.path);
            }
        }
    }
}
