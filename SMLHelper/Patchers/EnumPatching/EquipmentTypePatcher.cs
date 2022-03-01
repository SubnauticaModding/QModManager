namespace SMLHelper.V2.Patchers.EnumPatching
{
    using System;
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using Handlers;
    using Utility;

    internal class EquipmentTypePatcher
    {
        private const string EnumName = "EquipmentType";
        internal static readonly int startingIndex = 1000;

        internal static readonly EnumCacheManager<EquipmentType> cacheManager =
            new EnumCacheManager<EquipmentType>(
                enumTypeName: EnumName,
                startingIndex: startingIndex,
                bannedIDs: ExtBannedIdManager.GetBannedIdsFor(EnumName, PreRegisteredEquipmentTypes()));

        private static List<int> PreRegisteredEquipmentTypes()
        {
            var preRegistered = new List<int>();
            foreach (EquipmentType type in Enum.GetValues(typeof(EquipmentType)))
            {
                var typeCode = (int) type;
                if (typeCode >= startingIndex && !preRegistered.Contains(typeCode))
                {
                    preRegistered.Add(typeCode);
                }
            }
            Logger.Log($"Finished known EquipmentType exclusion. {preRegistered.Count} IDs were added in ban list.");
            return preRegistered;
        }

        internal static EquipmentType AddEquipmentType(string name)
        {
            var cache = cacheManager.RequestCacheForTypeName(name) ?? new EnumTypeCache()
            {
                Name = name,
                Index = cacheManager.GetNextAvailableIndex()
            };
            var equipmentType = (EquipmentType) cache.Index;
            cacheManager.Add(equipmentType, cache.Index, cache.Name);
            Logger.Log($"Successfully added EquipmentType: '{name}' to Index: '{cache.Index}'", LogLevel.Debug);
            return equipmentType;
        }

        internal static void Patch()
        {
            IngameMenuHandler.Main.RegisterOneTimeUseOnSaveEvent(() => cacheManager.SaveCache());
            Logger.Log($"Added {cacheManager.ModdedKeysCount} EquipmentTypes succesfully into the game.");
            Logger.Log("EquipmentTypePatcher is done.", LogLevel.Debug);
        }
    }
}
