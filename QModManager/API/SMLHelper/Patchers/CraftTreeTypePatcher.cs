namespace QModManager.API.SMLHelper.Patchers
{
    using Crafting;
    using Harmony;
    using System;
    using System.Collections.Generic;
    using Utility;

    internal static class CraftTreeTypePatcher
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

            craftTreeType = (CraftTree.Type)cache.Index;

            cacheManager.Add(craftTreeType, cache);

            cacheManager.SaveCache();

            Logger.Log($"Successfully added CraftTree Type: '{name}' to Index: '{cache.Index}'", LogLevel.Debug);

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

            Array enumValues = Enum.GetValues(typeof(CraftTree.Type));

            foreach (object enumValue in enumValues)
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

            Logger.Log($"Finished known CraftTreeType exclusion. {bannedIndices.Count} IDs were added in ban list.", LogLevel.Info);

            return bannedIndices;
        }

        #region Patches

        internal static void Patch(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(Enum), "GetValues"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CraftTreeTypePatcher), "Postfix_GetValues")));

            harmony.Patch(AccessTools.Method(typeof(Enum), "IsDefined"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(CraftTreeTypePatcher), "Prefix_IsDefined")));

            harmony.Patch(AccessTools.Method(typeof(Enum), "Parse", new Type[] { typeof(Type), typeof(string), typeof(bool) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(CraftTreeTypePatcher), "Prefix_Parse")));

            harmony.Patch(AccessTools.Method(typeof(CraftTree.Type), "ToString", new Type[] { }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(CraftTreeTypePatcher), "Prefix_ToString")));

            Logger.Log("CraftTreeTypePatcher is done.", LogLevel.Debug);
        }

        internal static void Postfix_GetValues(Type enumType, ref Array __result)
        {
            if (enumType.Equals(typeof(CraftTree.Type)))
            {
                var listArray = new List<CraftTree.Type>();
                foreach (object obj in __result)
                {
                    listArray.Add((CraftTree.Type)obj);
                }

                listArray.AddRange(cacheManager.ModdedKeys);

                __result = listArray.ToArray();
            }
        }

        internal static bool Prefix_IsDefined(Type enumType, object value, ref bool __result)
        {
            if (enumType.Equals(typeof(CraftTree.Type)))
            {
                if (cacheManager.ContainsKey((CraftTree.Type)value))
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
                if (cacheManager.TryParse(value, out CraftTree.Type craftTreeType))
                {
                    __result = craftTreeType;
                    return false;
                }
            }

            return true;
        }

        internal static bool Prefix_ToString(Enum __instance, ref string __result)
        {
            if (__instance is CraftTree.Type craftTreeType)
            {
                if (cacheManager.TryGetValue(craftTreeType, out EnumTypeCache craftTreeTypeCache))
                {
                    __result = craftTreeTypeCache.Name;
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
