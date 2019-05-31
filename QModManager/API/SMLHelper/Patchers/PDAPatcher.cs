namespace QModManager.API.SMLHelper.Patchers
{
    using Harmony;
    using QModManager.Utility;
    using System.Collections.Generic;

    // Special thanks to Gorillazilla9 for sharing this method of fragment count patching
    // https://github.com/Gorillazilla9/SubnauticaFragReqBoost/blob/master/PDAScannerPatcher.cs
    internal static class PDAPatcher
    {
        internal static readonly Dictionary<TechType, int> FragmentCount = new Dictionary<TechType, int>();
        internal static readonly Dictionary<TechType, float> FragmentScanTime = new Dictionary<TechType, float>();

        private static readonly Dictionary<TechType, PDAScanner.EntryData> BlueprintToFragment = new Dictionary<TechType, PDAScanner.EntryData>();

        internal static void Patch(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(PDAScanner), "Initialize"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(PDAPatcher), "InitializePostfix")));

            Logger.Debug($"PDAPatcher is done.");
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
                    Logger.Warn($"Warning: TechType {fragmentEntry.Key} not known in PDAScanner.EntryData");
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
                    Logger.Warn($"Warning: TechType {fragmentEntry.Key} not known in PDAScanner.EntryData");
                }
            }
        }
    }
}
