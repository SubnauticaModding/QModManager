namespace SMLHelper.V2.Patchers.EnumPatching
{
    using System.Collections.Generic;
    using SMLHelper.V2.Handlers;
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
            EnumTypeCache cache = cacheManager.RequestCacheForTypeName(name) ?? new EnumTypeCache()
            {
                Name = name,
                Index = cacheManager.GetNextAvailableIndex()
            };

            var techType = (TechType)cache.Index;
            cacheManager.Add(techType, cache.Index, cache.Name);

            // Direct access to private fields made possible by https://github.com/CabbageCrow/AssemblyPublicizer/
            // See README.md for details.
            TechTypeExtensions.stringsNormal[techType] = name;
            TechTypeExtensions.stringsLowercase[techType] = name.ToLowerInvariant();
            TechTypeExtensions.techTypesNormal[name] = techType;
            TechTypeExtensions.techTypesIgnoreCase[name] = techType;

            string intKey = cache.Index.ToString();
            TechTypeExtensions.techTypeKeys[techType] = intKey;
            TechTypeExtensions.keyTechTypes[intKey] = techType;

            Logger.Log($"Successfully added Tech Type: '{name}' to Index: '{cache.Index}'", LogLevel.Debug);
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
                Logger.Log($"Finished known TechTypes exclusion. {bannedIndices.Count} IDs were added in ban list.", LogLevel.Debug);

            return bannedIndices;
        }

        internal static void Patch()
        {
            IngameMenuHandler.Main.RegisterOneTimeUseOnSaveEvent(() => cacheManager.SaveCache());

            Logger.Log($"Added {cacheManager.ModdedKeysCount} TechTypes succesfully into the game.", LogLevel.Info);

            Logger.Log("TechTypePatcher is done.", LogLevel.Debug);
        }
    }
}
