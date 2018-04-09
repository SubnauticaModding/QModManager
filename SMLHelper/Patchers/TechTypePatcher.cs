using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SMLHelper.Patchers
{
    public class TechTypeCache
    {
        public int Index;
        public string Name;
    }

    public class TechTypePatcher
    {
        private static readonly FieldInfo CachedEnumString_valueToString =
            typeof(CachedEnumString<TechType>).GetField("valueToString", BindingFlags.NonPublic | BindingFlags.Instance);

        private static Dictionary<TechType, TechTypeCache> customTechTypes = new Dictionary<TechType, TechTypeCache>();

        private static int startingIndex = 11010;
        private static string CallerName = null;

        private static List<TechTypeCache> cacheList = new List<TechTypeCache>();

        private static bool cacheLoaded = false;

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

        #region Caching

        public static string GetCachePath()
        {
            var saveDir = @"./QMods/Modding Helper/TechTypeCache";

            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            return Path.Combine(saveDir, "TechTypeCache.txt");
        }

        public static void LoadCache()
        {
            if (cacheLoaded) return;

            var savePathDir = GetCachePath();

            if (!File.Exists(savePathDir))
            {
                SaveCache();
                return;
            }

            var allText = File.ReadAllLines(savePathDir);

            foreach(var line in allText)
            {
                var techTypeName = line.Split(':')[0];
                var techTypeIndex = line.Split(':')[1];

                var cache = new TechTypeCache()
                { 
                    Name = techTypeName,
                    Index = int.Parse(techTypeIndex)
                };

                cacheList.Add(cache);
            }

            Logger.Log("Loaded TechTypeCache!");

            cacheLoaded = true;
        }

        public static void SaveCache()
        {
            var savePathDir = GetCachePath();
            var stringBuilder = new StringBuilder();

            foreach (var techTypeEntry in customTechTypes)
            {
                cacheList.Add(techTypeEntry.Value);

                stringBuilder.AppendLine(string.Format("{0}:{1}", techTypeEntry.Value.Name, techTypeEntry.Value.Index));
            } 

            File.WriteAllText(savePathDir, stringBuilder.ToString());
        }

        public static TechTypeCache GetCacheForTechTypeName(string name)
        {
            LoadCache();

            foreach (var cache in cacheList)
            {
                if (cache.Name == name)
                    return cache;
            }

            return null;
        }

        public static TechTypeCache GetCacheForIndex(int index)
        {
            LoadCache();

            foreach (var cache in cacheList)
            {
                if (cache.Index == index)
                    return cache;
            }

            return null;
        }

        public static int GetLargestIndexFromCache()
        {
            LoadCache();

            var index = startingIndex;
            
            foreach(var cache in cacheList)
            {
                if (cache.Index > index)
                    index = cache.Index;
            }

            return index;
        }

        public static int GetNextFreeIndex()
        {
            LoadCache();

            var largestIndex = GetLargestIndexFromCache();
            return largestIndex + 1;
        }

        public static bool MultipleCachesUsingSameIndex(int index)
        {
            LoadCache();

            var count = 0;

            foreach(var cache in cacheList)
            {
                if (cache.Index == index)
                    count++;
            }

            if (count >= 2)
                return true;

            return false;
        }

        #endregion

        #region Adding TechTypes

        public static TechType AddTechType(string name, string languageName, string languageTooltip)
        {
            CallerName = Assembly.GetCallingAssembly().GetName().Name;
            return AddTechType(name, languageName, languageTooltip, true);
        }

        public static TechType AddTechType(string name, string languageName, string languageTooltip, bool unlockOnGameStart)
        {
            var cache = GetCacheForTechTypeName(name);

            if (cache == null)
            {
                cache = new TechTypeCache()
                {
                    Name = name,
                    Index = GetNextFreeIndex()
                };
            }

            if (MultipleCachesUsingSameIndex(cache.Index))
                cache.Index = GetNextFreeIndex();

            var techType = (TechType)cache.Index;

            customTechTypes.Add(techType, cache);

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
            string key3 = ((int)techType).ToString();
            techTypeKeys[techType] = key3;
            keyTechTypes[key3] = techType;

            if (unlockOnGameStart)
                KnownTechPatcher.unlockedAtStart.Add(techType);

            CallerName = CallerName ?? Assembly.GetCallingAssembly().GetName().Name;
            Logger.Log("Successfully added Tech Type: \"{0}\" to Index: \"{1}\" for mod \"{2}\"", name, cache.Index, CallerName);
            CallerName = null;

            SaveCache();

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
                    .Concat(customTechTypes.Keys)
                    .ToArray();
            }
        }

        public static bool Prefix_IsDefined(Type enumType, object value, ref bool __result)
        {
            if (enumType.Equals(typeof(TechType)))
            {
                if (customTechTypes.Keys.Contains((TechType)value))
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
                foreach (var techType in customTechTypes)
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
                foreach (var techType in customTechTypes)
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
