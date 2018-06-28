namespace SMLHelper.V2.Patchers
{
    using System;
    using System.Collections.Generic;
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
        }
    }
}
