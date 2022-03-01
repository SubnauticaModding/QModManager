namespace SMLHelper.V2.Patchers.EnumPatching
{
    using System;
    using System.Collections.Generic;
    using SMLHelper.V2.Handlers;
    using Utility;

    internal class TechCategoryPatcher
    {
        private const string TechCategoryEnumName = "TechCategory";

        internal const int startingIndex = 25; 

        internal static readonly EnumCacheManager<TechCategory> cacheManager =
            new EnumCacheManager<TechCategory>(
                enumTypeName: TechCategoryEnumName,
                startingIndex: startingIndex,
                bannedIDs: ExtBannedIdManager.GetBannedIdsFor(TechCategoryEnumName, PreRegisteredTechCategoryTypes()));

        internal static TechCategory AddTechCategory(string name)
        {
            EnumTypeCache cache = cacheManager.RequestCacheForTypeName(name) ?? new EnumTypeCache()
            {
                Name = name,
                Index = cacheManager.GetNextAvailableIndex()
            };

            TechCategory TechCategory = (TechCategory)cache.Index;

            cacheManager.Add(TechCategory, cache.Index, cache.Name);

            Logger.Log($"Successfully added TechCategory: '{name}' to Index: '{cache.Index}'", LogLevel.Debug);


            return TechCategory;
        }

        private static List<int> PreRegisteredTechCategoryTypes()
        {
            // Make sure to exclude already registered CraftTreeTypes.
            // Be aware that this approach is still subject to race conditions.
            // Any mod that patches after this one will not be picked up by this method.
            // For those cases, there are additional ways of excluding these IDs.

            var bannedIndices = new List<int>();

            Array enumValues = Enum.GetValues(typeof(TechCategory));

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

            Logger.Log($"Finished known TechCategory exclusion. {bannedIndices.Count} IDs were added in ban list.", LogLevel.Info);

            return bannedIndices;
        }

        internal static void Patch()
        {
            IngameMenuHandler.Main.RegisterOneTimeUseOnSaveEvent(() => cacheManager.SaveCache());

            Logger.Log($"Added {cacheManager.ModdedKeysCount} TechCategorys succesfully into the game.");
            Logger.Log("TechCategoryPatcher is done.", LogLevel.Debug);
        }
    }
}
