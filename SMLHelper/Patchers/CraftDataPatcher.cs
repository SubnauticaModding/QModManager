namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using Assets;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Utility;

    internal class CraftDataPatcher
    {
        #region Internal Fields

        internal static Dictionary<TechType, ITechData> CustomTechData = new Dictionary<TechType, ITechData>();
        internal static Dictionary<TechType, TechType> CustomHarvestOutputList = new Dictionary<TechType, TechType>();
        internal static Dictionary<TechType, HarvestType> CustomHarvestTypeList = new Dictionary<TechType, HarvestType>();
        internal static Dictionary<TechType, int> CustomFinalCutBonusList = new Dictionary<TechType, int>(TechTypeExtensions.sTechTypeComparer);
        internal static Dictionary<TechType, Vector2int> CustomItemSizes = new Dictionary<TechType, Vector2int>();
        internal static Dictionary<TechType, EquipmentType> CustomEquipmentTypes = new Dictionary<TechType, EquipmentType>();
        internal static Dictionary<TechType, QuickSlotType> CustomSlotTypes = new Dictionary<TechType, QuickSlotType>();
        internal static Dictionary<TechType, float> CustomCraftingTimes = new Dictionary<TechType, float>();
        internal static Dictionary<TechType, TechType> CustomCookedCreatureList = new Dictionary<TechType, TechType>();
        internal static Dictionary<TechType, CraftData.BackgroundType> CustomBackgroundTypes = new Dictionary<TechType, CraftData.BackgroundType>(TechTypeExtensions.sTechTypeComparer);
        internal static List<TechType> CustomBuildables = new List<TechType>();

        #endregion

        #region Reflection
        private static readonly Type CraftDataType = typeof(CraftData);

        private static readonly FieldInfo GroupsField =
            CraftDataType.GetField("groups", BindingFlags.NonPublic | BindingFlags.Static);

        #endregion

        #region Group Handling

        internal static void AddToCustomGroup(TechGroup group, TechCategory category, TechType techType, TechType after)
        {
            var groups = GroupsField.GetValue(null) as Dictionary<TechGroup, Dictionary<TechCategory, List<TechType>>>;
            var techGroup = groups[group];
            if (techGroup == null)
            {
                // Should never happen, but doesn't hurt to add it.
                Logger.Log("Invalid TechGroup!");
                return;
            }

            var techCategory = techGroup[category];
            if (techCategory == null)
            {
                Logger.Log($"Invalid TechCategory Combination! TechCategory: {category} TechGroup: {group}");
                return;
            }

            if(after == TechType.None)
            {
                techCategory.Add(techType);
                Logger.Log($"Added \"{techType.AsString():G}\" to groups under \"{group:G}->{category:G}\"");
            }
            else
            { 
                int index = techCategory.IndexOf(after);
                techCategory.Insert(index + 1, techType);

                Logger.Log($"Added \"{techType.AsString():G}\" to groups under \"{group:G}->{category:G}\" after \"{after.AsString():G}\"");
            }
        }

        internal static void RemoveFromCustomGroup(TechGroup group, TechCategory category, TechType techType)
        {
            var groups = GroupsField.GetValue(null) as Dictionary<TechGroup, Dictionary<TechCategory, List<TechType>>>;
            var techGroup = groups[group];
            if (techGroup == null)
            {
                // Should never happen, but doesn't hurt to add it.
                Logger.Log("Invalid TechGroup!");
                return;
            }

            var techCategory = techGroup[category];
            if (techCategory == null)
            {
                Logger.Log($"Invalid TechCategory Combination! TechCategory: {category} TechGroup: {group}");
                return;
            }

            techCategory.Remove(techType);

            Logger.Log($"Removed \"{techType.AsString():G}\" from groups under \"{group:G}->{category:G}\"");
        }

        #endregion

        #region Patching

        internal static void Patch(HarmonyInstance harmony)
        {
            PatchUtils.PatchDictionary(CraftDataType, "harvestOutputList", CustomHarvestOutputList, BindingFlags.Static | BindingFlags.Public);
            PatchUtils.PatchDictionary(CraftDataType, "harvestTypeList", CustomHarvestTypeList);
            PatchUtils.PatchDictionary(CraftDataType, "harvestFinalCutBonusList", CustomFinalCutBonusList);
            PatchUtils.PatchDictionary(CraftDataType, "itemSizes", CustomItemSizes);
            PatchUtils.PatchDictionary(CraftDataType, "equipmentTypes", CustomEquipmentTypes);
            PatchUtils.PatchDictionary(CraftDataType, "slotTypes", CustomSlotTypes);
            PatchUtils.PatchDictionary(CraftDataType, "craftingTimes", CustomCraftingTimes);
            PatchUtils.PatchDictionary(CraftDataType, "cookedCreatureList", CustomCookedCreatureList);
            PatchUtils.PatchDictionary(CraftDataType, "backgroundTypes", CustomBackgroundTypes);
            PatchUtils.PatchList(CraftDataType, "buildables", CustomBuildables);

            var preparePrefabIDCache = CraftDataType.GetMethod("PreparePrefabIDCache", BindingFlags.Public | BindingFlags.Static);
            var getMethod = CraftDataType.GetMethod("Get", BindingFlags.Public | BindingFlags.Static);

            harmony.Patch(preparePrefabIDCache, null,
                new HarmonyMethod(typeof(CraftDataPatcher).GetMethod("PreparePrefabIDCachePostfix", BindingFlags.NonPublic | BindingFlags.Static)));

            harmony.Patch(getMethod, 
                new HarmonyMethod(typeof(CraftDataPatcher).GetMethod("GetTechDataPrefix", BindingFlags.NonPublic | BindingFlags.Static)), null);

            Logger.Log("CraftDataPatcher is done.");
        }

        private static void PreparePrefabIDCachePostfix()
        {
            var techMapping = CraftDataType.GetField("techMapping", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as Dictionary<TechType, string>;
            var entClassTechTable = CraftDataType.GetField("entClassTechTable", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as Dictionary<string, TechType>;

            foreach (var prefab in ModPrefab.Prefabs)
            {
                techMapping[prefab.TechType] = prefab.ClassID;
                entClassTechTable[prefab.ClassID] = prefab.TechType;
            }
        }

        private static bool GetTechDataPrefix(ref ITechData __result, TechType techType)
        {
            if(CustomTechData.ContainsKey(techType))
            {
                __result = CustomTechData[techType];
                return false;
            }

            return true;
        }

        #endregion
    }
}
