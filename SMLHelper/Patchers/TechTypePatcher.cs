namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Utility;

    internal class TechTypePatcher
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

            TechType techType = (TechType)cache.Index;

            cacheManager.Add(techType, cache);

            Type techTypeExtensions = typeof(TechTypeExtensions);
            Traverse traverse = Traverse.Create(techTypeExtensions);

            Dictionary<TechType, string> stringsNormal = traverse.Field("stringsNormal").GetValue<Dictionary<TechType, string>>();
            Dictionary<TechType, string> stringsLowercase = traverse.Field("stringsLowercase").GetValue<Dictionary<TechType, string>>();
            Dictionary<string, TechType> techTypesNormal = traverse.Field("techTypesNormal").GetValue<Dictionary<string, TechType>>();
            Dictionary<string, TechType> techTypesIgnoreCase = traverse.Field("techTypesIgnoreCase").GetValue<Dictionary<string, TechType>>();
            Dictionary<TechType, string> techTypeKeys = traverse.Field("techTypeKeys").GetValue<Dictionary<TechType, string>>();
            Dictionary<string, TechType> keyTechTypes = traverse.Field("keyTechTypes").GetValue<Dictionary<string, TechType>>();

            stringsNormal[techType] = name;
            stringsLowercase[techType] = name.ToLowerInvariant();
            techTypesNormal[name] = techType;
            techTypesIgnoreCase[name] = techType;

            string intKey = cache.Index.ToString();
            techTypeKeys[techType] = intKey;
            keyTechTypes[intKey] = techType;

            cacheManager.SaveCache();

            Logger.Log($"Successfully added Tech Type: '{name}' to Index: '{cache.Index}'");
            return techType;
        }

        private static List<int> PreRegisteredTechTypes()
        {
            // Make sure to exclude already registered TechTypes.
            // Be aware that this approach is still subject to race conditions.
            // Any mod that patches after this one will not be picked up by this method.
            // For those cases, there are additional ways of excluding these IDs.

            List<int> bannedIndices = new List<int>();

            FieldInfo keyTechTypesField = typeof(TechTypeExtensions).GetField("keyTechTypes", BindingFlags.NonPublic | BindingFlags.Static);
            Dictionary<string, TechType> knownTechTypes = keyTechTypesField.GetValue(null) as Dictionary<string, TechType>;
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

            Logger.Log($"Finished known TechTypes exclusion. {bannedIndices.Count} IDs were added in ban list.");

            return bannedIndices;
        }

        #region Patches

        internal static void Patch(HarmonyInstance harmony)
        {
            Type enumType = typeof(Enum);
            Type thisType = typeof(TechTypePatcher);
            Type techTypeType = typeof(TechType);

            harmony.Patch(enumType.GetMethod("GetValues", BindingFlags.Public | BindingFlags.Static), null,
                new HarmonyMethod(thisType.GetMethod("Postfix_GetValues", BindingFlags.NonPublic | BindingFlags.Static)));

            harmony.Patch(enumType.GetMethod("IsDefined", BindingFlags.Public | BindingFlags.Static),
                new HarmonyMethod(thisType.GetMethod("Prefix_IsDefined", BindingFlags.NonPublic | BindingFlags.Static)), null);

            harmony.Patch(enumType.GetMethod("Parse", new Type[] { typeof(Type), typeof(string), typeof(bool) }),
                new HarmonyMethod(thisType.GetMethod("Prefix_Parse", BindingFlags.NonPublic | BindingFlags.Static)), null);

            harmony.Patch(techTypeType.GetMethod("ToString", new Type[0]),
                new HarmonyMethod(thisType.GetMethod("Prefix_ToString", BindingFlags.NonPublic | BindingFlags.Static)), null);

            Logger.Log("TechTypePatcher is done.");
        }

        private static void Postfix_GetValues(Type enumType, ref Array __result)
        {
            if (enumType.Equals(typeof(TechType)))
            {
                List<TechType> listArray = new List<TechType>();
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
