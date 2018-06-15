using Harmony;
using SMLHelper.V2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SMLHelper.V2.Patchers
{
    public class TechTypePatcher
    {
        private static readonly FieldInfo CachedEnumString_valueToString =
            typeof(CachedEnumString<TechType>).GetField("valueToString", BindingFlags.NonPublic | BindingFlags.Instance);

        internal const int startingIndex = 11010;
        internal static string CallerName = null;
        internal static List<int> bannedIndices = new List<int> // Can't make it constant, dunno why
        {
            11110, //AutosortLocker 
            11111, //AutosortTarget
            11112, //AutosortTargetStanding
            11120, //HabitatControlPanel
            11130, //DockedVehicleStorageAccess
            11140  //BaseTeleporter (not released)
        };

        public static void Patch(HarmonyInstance harmony)
        {
            var enumType = typeof(Enum);
            var thisType = typeof(TechTypePatcher);
            var techTypeType = typeof(TechType);

            harmony.Patch(enumType.GetMethod("GetValues", BindingFlags.Public | BindingFlags.Static), null,
                new HarmonyMethod(thisType.GetMethod("Postfix_GetValues", BindingFlags.Public | BindingFlags.Static)));

            harmony.Patch(enumType.GetMethod("IsDefined", BindingFlags.Public | BindingFlags.Static),
                new HarmonyMethod(thisType.GetMethod("Prefix_IsDefined", BindingFlags.Public | BindingFlags.Static)), null);

            harmony.Patch(enumType.GetMethod("Parse", new Type[] { typeof(Type), typeof(string), typeof(bool) }),
                new HarmonyMethod(thisType.GetMethod("Prefix_Parse", BindingFlags.Public | BindingFlags.Static)), null);

            harmony.Patch(techTypeType.GetMethod("ToString", new Type[0]),
                new HarmonyMethod(thisType.GetMethod("Prefix_ToString", BindingFlags.Public | BindingFlags.Static)), null);

            Logger.Log("TechTypePatcher is done.");
        }

        internal static readonly EnumCacheManager<TechType> cacheManager = new EnumCacheManager<TechType>("TechType", startingIndex, bannedIndices);

        #region Adding TechTypes

        public static TechType AddTechType(string name, string languageName, string languageTooltip)
        {
            CallerName = Assembly.GetCallingAssembly().GetName().Name;
            return AddTechType(name, languageName, languageTooltip, true);
        }

        public static TechType AddTechType(string name, string languageName, string languageTooltip, bool unlockOnGameStart)
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

            if (cacheManager.IsIndexConflicting(cache.Index) || cacheManager.IsIndexBanned(cache.Index))
                cache.Index = cacheManager.GetNextFreeIndex();

            var techType = (TechType)cache.Index;

            cacheManager.customEnumTypes.Add(techType, cache);

            LanguagePatcher.customLines.Add(name, languageName);
            LanguagePatcher.customLines.Add("Tooltip_" + name, languageTooltip);
            var valueToString = CachedEnumString_valueToString.GetValue(TooltipFactory.techTypeTooltipStrings) as Dictionary<TechType, string>;
            valueToString[techType] = "Tooltip_" + name;

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

            if (unlockOnGameStart)
                KnownTechPatcher.unlockedAtStart.Add(techType);

            CallerName = CallerName ?? Assembly.GetCallingAssembly().GetName().Name;
            Logger.Log("Successfully added Tech Type: \"{0}\" to Index: \"{1}\" for mod \"{2}\"", name, cache.Index, CallerName);
            CallerName = null;

            cacheManager.SaveCache();

            return techType;
        }

        #endregion

        #region Patches

        public static void Postfix_GetValues(Type enumType, ref Array __result)
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

        public static bool Prefix_IsDefined(Type enumType, object value, ref bool __result)
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

        public static bool Prefix_Parse(Type enumType, string value, bool ignoreCase, ref object __result)
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

        public static bool Prefix_ToString(Enum __instance, ref string __result)
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
