namespace SMLHelper.V2.Patchers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Harmony;

    // Special thanks to Gorillazilla9 for sharing this method of fragment count patching
    // https://github.com/Gorillazilla9/SubnauticaFragReqBoost/blob/master/PDAScannerPatcher.cs
    internal class PdaPatcher
    {
        internal static Dictionary<TechType, int> FragmentCount = new Dictionary<TechType, int>();
        internal static Dictionary<TechType, float> FragmentScanTime = new Dictionary<TechType, float>();

        private static Dictionary<TechType, PDAScanner.EntryData> BlueprintToFragment = new Dictionary<TechType, PDAScanner.EntryData>();

        internal static void Patch(HarmonyInstance harmony)
        {
            MethodInfo initializeInfo = typeof(PDAScanner).GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
            MethodInfo postfixInfo = typeof(PdaPatcher).GetMethod("InitializePostfix", BindingFlags.NonPublic | BindingFlags.Static);

            harmony.Patch(initializeInfo, null, new HarmonyMethod(postfixInfo));
        }

        private static void InitializePostfix()
        {
            PopulateReverseLookup();

            UpdateFragmentTotals();

            UpdateScanTimes();
        }

        private static void UpdateFragmentTotals()
        {
            foreach (KeyValuePair<TechType, int> fragmentEntry in FragmentCount)
            {
                if (PDAScanner.HasEntry(fragmentEntry.Key)) // Lookup by techtype of fragment
                {
                    PDAScanner.GetEntryData(fragmentEntry.Key).totalFragments = fragmentEntry.Value;
                }
                else if (BlueprintToFragment.TryGetValue(fragmentEntry.Key, out PDAScanner.EntryData entryData)) // Lookup by blueprint techtype
                {
                    entryData.totalFragments = fragmentEntry.Value;
                }
                else
                {
                    Logger.Log($"Warning: TechType {fragmentEntry.Key} not known in PDAScanner.EntryData");
                }
            }
        }

        private static void UpdateScanTimes()
        {
            foreach (KeyValuePair<TechType, float> fragmentEntry in FragmentScanTime)
            {
                if (PDAScanner.HasEntry(fragmentEntry.Key)) // Lookup by techtype of fragment
                {
                    PDAScanner.GetEntryData(fragmentEntry.Key).scanTime = fragmentEntry.Value;
                }
                else if (BlueprintToFragment.TryGetValue(fragmentEntry.Key, out PDAScanner.EntryData entryData)) // Lookup by blueprint techtype
                {
                    entryData.scanTime = fragmentEntry.Value;
                }
                else
                {
                    Logger.Log($"Warning: TechType {fragmentEntry.Key} not known in PDAScanner.EntryData");
                }
            }
        }

        private static void PopulateReverseLookup()
        {
            BlueprintToFragment.Clear();

            Dictionary<TechType, PDAScanner.EntryData>.Enumerator entries = PDAScanner.GetAllEntriesData();

            do
            {
                KeyValuePair<TechType, PDAScanner.EntryData> entry = entries.Current;

                TechType blueprintTechType = entry.Value.blueprint;

                BlueprintToFragment[blueprintTechType] = entry.Value;

            } while (entries.MoveNext());
        }
    }
}
