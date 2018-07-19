namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Utility;
    using Crafting;

    internal class CraftTreeTypePatcher
    {
        private const string CraftTreeTypeEnumName = "CraftTreeType";

        internal const int startingIndex = 11; // The default CraftTree.Type contains indexes 0 through 10

        internal static readonly EnumCacheManager<CraftTree.Type> cacheManager =
            new EnumCacheManager<CraftTree.Type>(
                enumTypeName: CraftTreeTypeEnumName,
                startingIndex: startingIndex,
                bannedIDs: ExtBannedIdManager.GetBannedIdsFor(CraftTreeTypeEnumName, PreRegisteredCraftTreeTypes()));

        internal static ModCraftTreeRoot CreateCustomCraftTreeAndType(string name, out CraftTree.Type craftTreeType)
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

            if (cacheManager.IsIndexValid(cache.Index))
                cache.Index = cacheManager.GetNextFreeIndex();

            craftTreeType = (CraftTree.Type)cache.Index;

            cacheManager.customEnumTypes.Add(craftTreeType, cache);

            Logger.Log("Successfully added CraftTree Type: \"{0}\" to Index: \"{1}\"", name, cache.Index);

            var customTreeRoot = new ModCraftTreeRoot(craftTreeType, name);

            CraftTreePatcher.CustomTrees[craftTreeType] = customTreeRoot;

            return customTreeRoot;
        }

        private static List<int> PreRegisteredCraftTreeTypes()
        {
            // Make sure to exclude already registered CraftTreeTypes.
            // Be aware that this approach is still subject to race conditions.
            // Any mod that patches after this one will not be picked up by this method.
            // For those cases, there are additional ways of excluding these IDs.

            var bannedIndices = new List<int>();

            var enumValues = Enum.GetValues(typeof(CraftTree.Type));

            foreach (var enumValue in enumValues)
            {
                if (enumValue == null)
                    continue; // Saftey check

                int realEnumValue = (int)enumValue;

                if (realEnumValue < startingIndex)
                    continue; // This is possibly a default tree
                // Anything below this range we won't ever assign

                if (bannedIndices.Contains(realEnumValue))
                    continue;// Already exists in list

                bannedIndices.Add(realEnumValue);
            }

            Logger.Log($"Finished known CraftTreeType exclusion. {bannedIndices.Count} IDs were added in ban list.");

            return bannedIndices;
        }

        #region Patches

        internal static void Patch(HarmonyInstance harmony)
        {
            cacheManager.SaveCache();

            var enumType = typeof(Enum);
            var thisType = typeof(CraftTreeTypePatcher);
            var techTypeType = typeof(CraftTree.Type);

            harmony.Patch(enumType.GetMethod("GetValues", BindingFlags.Public | BindingFlags.Static), null,
                new HarmonyMethod(thisType.GetMethod("Postfix_GetValues", BindingFlags.NonPublic | BindingFlags.Static)));

            harmony.Patch(enumType.GetMethod("IsDefined", BindingFlags.Public | BindingFlags.Static),
                new HarmonyMethod(thisType.GetMethod("Prefix_IsDefined", BindingFlags.NonPublic | BindingFlags.Static)), null);

            harmony.Patch(enumType.GetMethod("Parse", new Type[] { typeof(Type), typeof(string), typeof(bool) }),
                new HarmonyMethod(thisType.GetMethod("Prefix_Parse", BindingFlags.NonPublic | BindingFlags.Static)), null);

            harmony.Patch(techTypeType.GetMethod("ToString", new Type[0]),
                new HarmonyMethod(thisType.GetMethod("Prefix_ToString", BindingFlags.NonPublic | BindingFlags.Static)), null);

            Logger.Log("CraftTreeTypePatcher is done.");
        }

        internal static void Postfix_GetValues(Type enumType, ref Array __result)
        {
            if (enumType.Equals(typeof(CraftTree.Type)))
            {
                var listArray = new List<CraftTree.Type>();
                foreach (var obj in __result)
                {
                    listArray.Add((CraftTree.Type)obj);
                }

                __result = listArray
                    .Concat(cacheManager.customEnumTypes.Keys)
                    .ToArray();
            }
        }

        internal static bool Prefix_IsDefined(Type enumType, object value, ref bool __result)
        {
            if (enumType.Equals(typeof(CraftTree.Type)))
            {
                if (cacheManager.customEnumTypes.Keys.Contains((CraftTree.Type)value))
                {
                    __result = true;
                    return false;
                }
            }

            return true;
        }

        internal static bool Prefix_Parse(Type enumType, string value, bool ignoreCase, ref object __result)
        {
            if (enumType.Equals(typeof(CraftTree.Type)))
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

        internal static bool Prefix_ToString(Enum __instance, ref string __result)
        {
            if (__instance.GetType().Equals(typeof(CraftTree.Type)))
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
