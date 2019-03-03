namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    // Special thanks to Gorillazilla9 for sharing this method of fragment count patching
    // https://github.com/Gorillazilla9/SubnauticaFragReqBoost/blob/master/PDAScannerPatcher.cs
    internal class PDAPatcher
    {
        internal static readonly Dictionary<TechType, int> FragmentCount = new Dictionary<TechType, int>();
        internal static readonly Dictionary<TechType, float> FragmentScanTime = new Dictionary<TechType, float>();

        private static readonly Type pdaScannerType = typeof(PDAScanner);
        private static readonly Dictionary<TechType, PDAScanner.EntryData> BlueprintToFragment = new Dictionary<TechType, PDAScanner.EntryData>();

        internal static void Patch(HarmonyInstance harmony)
        {
            MethodInfo initializeInfo = pdaScannerType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
            MethodInfo postfixInfo = typeof(PDAPatcher).GetMethod("InitializePostfix", BindingFlags.NonPublic | BindingFlags.Static);

            harmony.Patch(initializeInfo, null, new HarmonyMethod(postfixInfo));

            Logger.Log($"PDAPatcher is ready.", LogLevel.Debug);
        }

        private static void InitializePostfix()
        {
            BlueprintToFragment.Clear();

            Dictionary<TechType, PDAScanner.EntryData> mapping = PDAScanner.mapping;

            // Populate BlueprintToFragment for reverse lookup
            foreach (KeyValuePair<TechType, PDAScanner.EntryData> entry in mapping)
            {
                TechType blueprintTechType = entry.Value.blueprint;

                BlueprintToFragment[blueprintTechType] = entry.Value;
            }

            // Update fragment totals
            foreach (KeyValuePair<TechType, int> fragmentEntry in FragmentCount)
            {
                if (mapping.ContainsKey(fragmentEntry.Key)) // Lookup by techtype of fragment
                {
                    mapping[fragmentEntry.Key].totalFragments = fragmentEntry.Value;
                }
                else if (BlueprintToFragment.TryGetValue(fragmentEntry.Key, out PDAScanner.EntryData entryData)) // Lookup by blueprint techtype
                {
                    entryData.totalFragments = fragmentEntry.Value;
                }
                else
                {
                    Logger.Log($"Warning: TechType {fragmentEntry.Key} not known in PDAScanner.EntryData", LogLevel.Warn);
                }
            }

            // Update scan times
            foreach (KeyValuePair<TechType, float> fragmentEntry in FragmentScanTime)
            {
                if (mapping.ContainsKey(fragmentEntry.Key)) // Lookup by techtype of fragment
                {
                    mapping[fragmentEntry.Key].scanTime = fragmentEntry.Value;
                }
                else if (BlueprintToFragment.TryGetValue(fragmentEntry.Key, out PDAScanner.EntryData entryData)) // Lookup by blueprint techtype
                {
                    entryData.scanTime = fragmentEntry.Value;
                }
                else
                {
                    Logger.Log($"Warning: TechType {fragmentEntry.Key} not known in PDAScanner.EntryData", LogLevel.Warn);
                }
            }

            Logger.Log($"PDAPatcher is done.", LogLevel.Debug);
        }
    }
}
