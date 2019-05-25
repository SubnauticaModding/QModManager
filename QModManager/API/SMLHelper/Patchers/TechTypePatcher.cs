namespace QModManager.API.SMLHelper.Patchers
{
    using Harmony;
    using QModManager.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utility;

    internal static class TechTypePatcher
    {
        private const string TechTypeEnumName = "TechType";
        internal static readonly int startingIndex = 11010;
        internal static readonly List<int> bannedIndices = new List<int>
        {
            11110, //AutosortLocker 
            11111, //AutosortTarget
            11112, //AutosortTargetStanding
            11120, //HabitatControlPanel
            11130, //DockedVehicleStorageAccess
            11140  //BaseTeleporter (not released)
        };

        internal static readonly EnumCacheManager<TechType> cacheManager =
            new EnumCacheManager<TechType>(
                enumTypeName: TechTypeEnumName,
                startingIndex: startingIndex,
                bannedIDs: ExtBannedIdManager.GetBannedIdsFor(TechTypeEnumName, bannedIndices, PreRegisteredTechTypes()));

        internal static TechType AddTechType(string name)
        {
            EnumTypeCache cache = cacheManager.GetCacheForTypeName(name);

            if (cache == null)
            {
                cache = new EnumTypeCache()
                {
                    Name = name,
                    Index = cacheManager.GetNextFreeIndex()
                };
            }

            if (cacheManager.IsIndexValid(cache.Index))
                cache.Index = cacheManager.GetNextFreeIndex();

            var techType = (TechType)cache.Index;

            cacheManager.Add(techType, cache);

            // Direct access to private fields made possible by https://github.com/CabbageCrow/AssemblyPublicizer/
            // See README.md for details.
            TechTypeExtensions.stringsNormal[techType] = name;
            TechTypeExtensions.stringsLowercase[techType] = name.ToLowerInvariant();
            TechTypeExtensions.techTypesNormal[name] = techType;
            TechTypeExtensions.techTypesIgnoreCase[name] = techType;

            string intKey = cache.Index.ToString();
            TechTypeExtensions.techTypeKeys[techType] = intKey;
            TechTypeExtensions.keyTechTypes[intKey] = techType;

            cacheManager.SaveCache();

            Logger.Debug($"Successfully added Tech Type: '{name}' to Index: '{cache.Index}'");
            return techType;
        }

        private static List<int> PreRegisteredTechTypes()
        {
            // Make sure to exclude already registered TechTypes.
            // Be aware that this approach is still subject to race conditions.
            // Any mod that patches after this one will not be picked up by this method.
            // For those cases, there are additional ways of excluding these IDs.

            var bannedIndices = new List<int>();

            Dictionary<string, TechType> knownTechTypes = TechTypeExtensions.keyTechTypes;
            foreach (TechType knownTechType in knownTechTypes.Values)
            {
                int currentTechTypeKey = (int)knownTechType;

                if (currentTechTypeKey < startingIndex)
                    continue; // This is possibly a default TechType,
                // Anything below this range we won't ever assign

                if (bannedIndices.Contains(currentTechTypeKey))
                    continue; // Already exists in list

                bannedIndices.Add(currentTechTypeKey);
            }

            if (bannedIndices.Count > 0)
                Logger.Debug($"Finished known TechTypes exclusion. {bannedIndices.Count} IDs were added in ban list.");

            return bannedIndices;
        }

        #region Patches

        internal static void Patch(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(Enum), "GetValues"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(TechTypePatcher), "Postfix_GetValues")));

            harmony.Patch(AccessTools.Method(typeof(Enum), "IsDefined"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TechTypePatcher), "Prefix_IsDefined")));

            harmony.Patch(AccessTools.Method(typeof(Enum), "Parse", new Type[] { typeof(Type), typeof(string), typeof(bool) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TechTypePatcher), "Prefix_Parse")));

            harmony.Patch(AccessTools.Method(typeof(TechType), "ToString", new Type[] { }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TechTypePatcher), "Prefix_ToString")));

            Logger.Info($"Added {cacheManager.ModdedKeys.Count()} TechTypes succesfully into the game.");

            Logger.Debug("TechTypePatcher is done.");
        }

        private static void Postfix_GetValues(Type enumType, ref Array __result)
        {
            if (enumType.Equals(typeof(TechType)))
            {
                var listArray = new List<TechType>();
                foreach (object obj in __result)
                {
                    listArray.Add((TechType)obj);
                }

                listArray.AddRange(cacheManager.ModdedKeys);

                __result = listArray.ToArray();
            }
        }

        private static bool Prefix_IsDefined(Type enumType, object value, ref bool __result)
        {
            if (TooltipPatcher.DisableEnumIsDefinedPatch) return true;

            if (enumType.Equals(typeof(TechType)))
            {
                if (cacheManager.ContainsKey((TechType)value))
                {
                    __result = true;
                    return false;
                }
            }

            return true;
        }

        private static bool Prefix_Parse(Type enumType, string value, bool ignoreCase, ref object __result)
        {
            if (enumType.Equals(typeof(TechType)))
            {
                if (cacheManager.TryParse(value, out TechType techType))
                {
                    __result = techType;
                    return false;
                }
            }

            return true;
        }

        private static bool Prefix_ToString(Enum __instance, ref string __result)
        {
            if (__instance is TechType techType)
            {
                if (cacheManager.TryGetValue(techType, out EnumTypeCache techTypeCache))
                {
                    __result = techTypeCache.Name;
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
