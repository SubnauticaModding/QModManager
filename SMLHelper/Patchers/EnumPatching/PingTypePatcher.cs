namespace SMLHelper.V2.Patchers.EnumPatching
{
    using System;
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using Handlers;
    using Utility;
#if SUBNAUTICA
    using Sprite = Atlas.Sprite;
#elif BELOWZERO
    using Sprite = UnityEngine.Sprite;
#endif

    internal class PingTypePatcher
    {
        private const string EnumName = "PingType";
        internal static readonly int startingIndex = 1000;

        internal static readonly EnumCacheManager<PingType> cacheManager =
            new EnumCacheManager<PingType>(
                enumTypeName: EnumName,
                startingIndex: startingIndex,
                bannedIDs: ExtBannedIdManager.GetBannedIdsFor(EnumName, PreRegisteredPingTypes()));

        private static List<int> PreRegisteredPingTypes()
        {
            var preRegistered = new List<int>();
            foreach (PingType type in Enum.GetValues(typeof(PingType)))
            {
                var typeCode = (int) type;
                if (typeCode >= startingIndex && !preRegistered.Contains(typeCode))
                {
                    preRegistered.Add(typeCode);
                }
            }
            
            Logger.Log($"Finished known PingType exclusion. {preRegistered.Count} IDs were added in ban list.");
            return preRegistered;
        }

        internal static PingType AddPingType(string name, Sprite sprite)
        {
            var cache = cacheManager.RequestCacheForTypeName(name) ?? new EnumTypeCache()
            {
                Name = name,
                Index = cacheManager.GetNextAvailableIndex()
            };

            var pingType = (PingType) cache.Index;
            cacheManager.Add(pingType, cache.Index, cache.Name);
            ModSprite.Add(SpriteManager.Group.Pings, pingType.ToString(), sprite);
            
            if (PingManager.sCachedPingTypeStrings.valueToString.ContainsKey(pingType) == false)
                PingManager.sCachedPingTypeStrings.valueToString.Add(pingType, name);

            if (PingManager.sCachedPingTypeTranslationStrings.valueToString.ContainsKey(pingType) == false)
                PingManager.sCachedPingTypeTranslationStrings.valueToString.Add(pingType, name);
            
            Logger.Log($"Successfully added PingType: '{name}' to Index: '{cache.Index}'", LogLevel.Debug);
            return pingType;
        }

        internal static void Patch()
        {
            IngameMenuHandler.Main.RegisterOneTimeUseOnSaveEvent(() => cacheManager.SaveCache());

            Logger.Log($"Added {cacheManager.ModdedKeysCount} PingTypes succesfully into the game.");

            Logger.Log("PingTypePatcher is done.", LogLevel.Debug);
        }
    }
}
