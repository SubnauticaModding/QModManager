#if BELOWZERO
namespace SMLHelper.V2.Patchers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using HarmonyLib;

    internal partial class CraftDataPatcher
    {
        internal static readonly IDictionary<TechType, JsonValue> CustomTechData = new SelfCheckingDictionary<TechType, JsonValue>("CustomTechData", AsStringFunction);

        private static void PatchForBelowZero(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(TechData), nameof(TechData.TryGetValue)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(CraftDataPatcher), nameof(CheckPatchRequired))));

            harmony.Patch(AccessTools.Method(typeof(TechData), nameof(TechData.Cache)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(CraftDataPatcher), nameof(AddCustomTechDataToOriginalDictionary))));
        }

        private static void CheckPatchRequired(TechType techType)
        {
            if (CustomTechData.TryGetValue(techType, out JsonValue smlTechData))
            {
                if (!TechData.entries.TryGetValue(techType, out JsonValue techData) ||
                    smlTechData != techData)
                {
                    AddCustomTechDataToOriginalDictionary();
                }
            }
        }

        private static void AddCustomTechDataToOriginalDictionary()
        {
            var added = new List<TechType>();
            var updated = new List<TechType>();
            foreach (KeyValuePair<TechType, JsonValue> customTechData in CustomTechData)
            {
                JsonValue smlTechData = customTechData.Value;
                TechType techType = customTechData.Key;
                if (TechData.entries.TryGetValue(techType, out JsonValue techData))
                {
                    if (techData != smlTechData)
                    {
                        updated.Add(techType);
                        foreach (int key in smlTechData.Keys)
                        {
                            techData[key] = smlTechData[key];
                        }
                    }
                }
                else
                {
                    TechData.entries.Add(techType, smlTechData);
                    added.Add(techType);
                }
            }

            for (int i = 0; i < updated.Count; i++)
            {
                TechType updatedTechData = updated[i];
                CustomTechData[updatedTechData] = TechData.entries[updatedTechData];
            }

            if (added.Count > 0)
            {
                Logger.Log($"Added {added.Count} new entries to the TechData.entries dictionary.", LogLevel.Info);
                LogEntries("Added the following TechTypes", added);
            }

            if (updated.Count > 0)
            {
                Logger.Log($"Updated {updated.Count} existing entries to the TechData.entries dictionary.", LogLevel.Info);
                LogEntries("Updated the following TechTypes", updated);
            }
        }

        private static void LogEntries(string log, List<TechType> updated)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < updated.Count; i++)
            {
                builder.AppendLine($"{updated[i]}");
            }

            Logger.Log($"{log}:{Environment.NewLine}{builder}", LogLevel.Debug);
        }
    }
}
#endif
