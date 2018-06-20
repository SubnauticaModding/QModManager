namespace SMLHelper.V2.Handlers
{
    using Crafting;
    using Patchers;

    /// <summary>
    /// A handler class for editing crafted items.
    /// </summary>
    public class CraftDataHandler
    {
        /// <summary>
        /// <para>Allows you to edit recipes, i.e. TechData for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to edit.</param>
        /// <param name="techData">The TechData for that TechType.</param>
        /// <seealso cref="TechData"/>
        public static void EditTechData(TechType techType, TechData techData)
        {
            CraftDataPatcher.CustomTechData[techType] = techData;
        }

        /// <summary>
        /// <para>Allows you to edit EquipmentTypes for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose EqiupmentType you want to edit.</param>
        /// <param name="equipmentType">The EquipmentType for that TechType.</param>
        public static void EditEquipmentType(TechType techType, EquipmentType equipmentType)
        {
            CraftDataPatcher.CustomEquipmentTypes[techType] = equipmentType;
        }

        /// <summary>
        /// <para>Allows you to edit QuickSlotType for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose QuickSlotType you want to edit.</param>
        /// <param name="slotType">The QuickSlotType for that TechType.</param>
        public static void EditQuickSlotType(TechType techType, QuickSlotType slotType)
        {
            CraftDataPatcher.CustomSlotTypes[techType] = slotType;
        }

        /// <summary>
        /// <para>Allows you to edit harvest output, i.e. what TechType you get when you "harvest" a TechType.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose harvest output you want to edit.</param>
        /// <param name="harvestOutput">The harvest output for that TechType.</param>
        public static void EditHarvestOutputList(TechType techType, TechType harvestOutput)
        {
            CraftDataPatcher.CustomHarvestOutputList[techType] = harvestOutput;
        }

        /// <summary>
        /// <para>Allows you to edit HarvestType for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose HarvestType you want to edit.</param>
        /// <param name="harvestType">The HarvestType for that TechType.</param>
        public static void EditHarvestTypeList(TechType techType, HarvestType harvestType)
        {
            CraftDataPatcher.CustomHarvestTypeList[techType] = harvestType;
        }

        /// <summary>
        /// <para>Allows you to edit item sizes for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose item size you want to edit.</param>
        /// <param name="size">The item size for that TechType.</param>
        public static void EditItemSize(TechType techType, Vector2int size)
        {
            CraftDataPatcher.CustomItemSizes[techType] = size;
        }

        /// <summary>
        /// <para>Allows you to edit crafting times for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose crafting time you want to edit.</param>
        /// <param name="time">The crafting time for that TechType.</param>
        public static void EditCraftingTime(TechType techType, float time)
        {
            CraftDataPatcher.CustomCraftingTimes[techType] = time;
        }

        /// <summary>
        /// <para>Allows you to edit the cooked creature list, i.e. associate the unedible TechType to the cooked TechType.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose cooked creature counterpart to edit.</param>
        /// <param name="cooked">The cooked creature counterpart for that TechType.</param>
        public static void EditCookedCreatureList(TechType techType, TechType cooked)
        {
            CraftDataPatcher.CustomCookedCreatureList[techType] = cooked;
        }

        /// <summary>
        /// Allows you to add items to the buildable list.
        /// </summary>
        /// <param name="techType">The TechType which you want to add to the buildable list.</param>
        public static void AddToBuildableList(TechType techType)
        {
            CraftDataPatcher.CustomBuildables.Add(techType);
        }
    }
}
