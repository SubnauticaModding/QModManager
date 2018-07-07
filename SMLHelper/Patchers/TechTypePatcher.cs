namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
                bannedIndices: ExtBannedIdManager.GetBannedIdsFor(TechTypeEnumName, bannedIndices, PreRegisteredTechTypes()));

        internal static TechType AddTechType(string name)
        {
            var cache = cacheManager.GetCacheForTypeName(name);

            if (cache == null)
            {
                cache = new EnumTypeCache()
                {
                    Name = name,
                    Index = cacheManager.GetNextFreeIndex()
                };
            }

            if (cacheManager.IsIndexConflicting(cache.Index))
                cache.Index = cacheManager.GetNextFreeIndex();

            var techType = (TechType)cache.Index;

            cacheManager.customEnumTypes.Add(techType, cache);

            var techTypeExtensions = typeof(TechTypeExtensions);
            var traverse = Traverse.Create(techTypeExtensions);

            var stringsNormal = traverse.Field("stringsNormal").GetValue<Dictionary<TechType, string>>();
            var stringsLowercase = traverse.Field("stringsLowercase").GetValue<Dictionary<TechType, string>>();
            var techTypesNormal = traverse.Field("techTypesNormal").GetValue<Dictionary<string, TechType>>();
            var techTypesIgnoreCase = traverse.Field("techTypesIgnoreCase").GetValue<Dictionary<string, TechType>>();
            var techTypeKeys = traverse.Field("techTypeKeys").GetValue<Dictionary<TechType, string>>();
            var keyTechTypes = traverse.Field("keyTechTypes").GetValue<Dictionary<string, TechType>>();

            stringsNormal[techType] = name;
            stringsLowercase[techType] = name.ToLowerInvariant();
            techTypesNormal[name] = techType;
            techTypesIgnoreCase[name] = techType;

            var intKey = cache.Index.ToString();
            techTypeKeys[techType] = intKey;
            keyTechTypes[intKey] = techType;

            Logger.Log("Successfully added Tech Type: \"{0}\" to Index: \"{1}\"", name, cache.Index);

            cacheManager.SaveCache();

            return techType;
        }

        private static List<int> PreRegisteredTechTypes()
        {
            // Make sure to exclude already registered TechTypes.
            // Be aware that this approach is still subject to race conditions.
            // Any mod that patches after this one will not be picked up by this method.
            // For those cases, there are additional ways of excluding these IDs.

            var bannedIndices = new List<int>();

            FieldInfo keyTechTypesField = typeof(TechTypeExtensions).GetField("keyTechTypes", BindingFlags.NonPublic | BindingFlags.Static);
            Dictionary<string, TechType> knownTechTypes = keyTechTypesField.GetValue(null) as Dictionary<string, TechType>;
            foreach (KeyValuePair<string, TechType> knownTechType in knownTechTypes)
            {
                int currentTechTypeKey = Convert.ToInt32(knownTechType.Key);
                if (currentTechTypeKey > startingIndex)
                {
                    if (!bannedIndices.Contains(currentTechTypeKey))
                    {
                        bannedIndices.Add(currentTechTypeKey);
                    }
                }
            }

            Logger.Log($"Finished known TechTypes exclusion. {bannedIndices.Count} IDs were added in ban list.");

            return bannedIndices;
        }

        #region Patches

        internal static void Patch(HarmonyInstance harmony)
        {
            var enumType = typeof(Enum);
            var thisType = typeof(TechTypePatcher);
            var techTypeType = typeof(TechType);

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
                var listArray = new List<TechType>();
                foreach (var obj in __result)
                {
                    listArray.Add((TechType)obj);
                }

                __result = listArray
                    .Concat(cacheManager.customEnumTypes.Keys)
                    .ToArray();
            }
        }

        private static bool Prefix_IsDefined(Type enumType, object value, ref bool __result)
        {
            if (enumType.Equals(typeof(TechType)))
            {
                if (cacheManager.customEnumTypes.Keys.Contains((TechType)value))
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
                foreach (var techType in cacheManager.customEnumTypes)
                {
                    if (value.Equals(techType.Value.Name, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
                    {
                        __result = techType.Key;
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool Prefix_ToString(Enum __instance, ref string __result)
        {
            if (__instance.GetType().Equals(typeof(TechType)))
            {
                foreach (var techType in cacheManager.customEnumTypes)
                {
                    if (__instance.Equals(techType.Key))
                    {
                        __result = techType.Value.Name;
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

    }
}
