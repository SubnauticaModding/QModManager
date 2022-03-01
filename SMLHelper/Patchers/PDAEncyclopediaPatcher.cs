namespace SMLHelper.V2.Patchers
{
    using HarmonyLib;
    using SMLHelper.V2.Utility;
    using System.Collections.Generic;

    internal class PDAEncyclopediaPatcher
    {
        internal static readonly SelfCheckingDictionary<string, PDAEncyclopedia.EntryData> CustomEntryData = new SelfCheckingDictionary<string, PDAEncyclopedia.EntryData>("CustomEntryData");

        internal static void Patch(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(PDAEncyclopedia), nameof(PDAEncyclopedia.Initialize)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(PDAEncyclopediaPatcher), nameof(PDAEncyclopediaPatcher.InitializePostfix))));
        }

        private static void InitializePostfix()
        {
            Dictionary<string, PDAEncyclopedia.EntryData> mapping = PDAEncyclopedia.mapping;

            // Add custom entry data
            foreach(KeyValuePair<string, PDAEncyclopedia.EntryData> customEntry in CustomEntryData)
            {
                if (!mapping.ContainsKey(customEntry.Key))
                {
                    mapping.Add(customEntry.Key, customEntry.Value);
                    Logger.Debug($"Adding PDAEncyclopedia EntryData for Key Value: {customEntry.Key}.");
                }
                else
                {
                    mapping[customEntry.Key] = customEntry.Value;
                    Logger.Debug($"PDAEncyclopedia already Contains EntryData for Key Value: {customEntry.Key}, Overwriting Original.");
                }
            }
        }
    }
}
