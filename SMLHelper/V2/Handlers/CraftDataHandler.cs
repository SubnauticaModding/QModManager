namespace SMLHelper.V2.Handlers
{
    using Crafting;
    using Patchers;

    /// <summary>
    /// A handler class for adding and editing crafted items.
    /// </summary>
    public class CraftDataHandler
    {
        #region Core Methods

        /// <summary>
        /// <para>Allows you to edit inventory background type for TechTypes.</para>
        /// </summary>
        /// <param name="techType">The TechType whose BackgroundType you want to edit.</param>
        /// <param name="backgroundType">The BackgroundType for that TechType.</param>
        /// <seealso cref="CraftData.BackgroundType"/>
        public static void EditBackgroundType(TechType techType, CraftData.BackgroundType backgroundType)
        {
            CraftDataPatcher.CustomBackgroundTypes[techType] = backgroundType;
        }

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
        /// <para>Allows you to edit how TechTypes are harvested.</para>
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
        /// <param name="time">The crafting time, in seconds, for that TechType.</param>
        public static void EditCraftingTime(TechType techType, float time)
        {
            CraftDataPatcher.CustomCraftingTimes[techType] = time;
        }

        /// <summary>
        /// <para>Allows you to edit the cooked creature list, i.e. associate the unedible TechType to the cooked TechType.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="uncooked">The TechType whose cooked creature counterpart to edit.</param>
        /// <param name="cooked">The cooked creature counterpart for that TechType.</param>
        public static void EditCookedCreatureList(TechType uncooked, TechType cooked)
        {
            CraftDataPatcher.CustomCookedCreatureList[uncooked] = cooked;
        }

        /// <summary>
        /// Allows you to add items to the buildable list.
        /// </summary>
        /// <param name="techType">The TechType which you want to add to the buildable list.</param>
        public static void AddToBuildableList(TechType techType)
        {
            CraftDataPatcher.CustomBuildables.Add(techType);
        }

        #endregion

        // Typically, when adding custom items, other modders will likely be looking for "Add" methods without realising that the "Edit" methods above also add.
        // This set of methods below is here to to address the naming expectations without altering actual functionality.
        #region Redundant but friendly

        /// <summary>
        /// <para>Allows you to associate an inventory background type to your TechType.</para>
        /// </summary>
        /// <param name="techType">The TechType whose BackgroundType you want to set.</param>
        /// <param name="backgroundType">The BackgroundType for that TechType.</param>
        /// <seealso cref="CraftData.BackgroundType"/>
        public static void AddBackgroundType(TechType techType, CraftData.BackgroundType backgroundType) => EditBackgroundType(techType, backgroundType);

        /// <summary>
        /// <para>Allows you to add a recipe, i.e. TechData for your TechType.</para>
        /// </summary>
        /// <param name="techType">The TechType that is receiving a new TechData recipe.</param>
        /// <param name="techData">The TechData recipe for that TechType.</param>
        /// <seealso cref="TechData"/>
        public static void AddTechData(TechType techType, TechData techData) => EditTechData(techType, techData);

        /// <summary>
        /// <para>Allows you to add an EquipmentType attribute to your TechType.</para>
        /// </summary>
        /// <param name="techType">The TechType whose EqiupmentType you want to set.</param>
        /// <param name="equipmentType">The EquipmentType for that TechType.</param>
        public static void AddEquipmentType(TechType techType, EquipmentType equipmentType) => EditEquipmentType(techType, equipmentType);

        /// <summary>
        /// <para>Allows you to add a QuickSlotType attribute to your TechType.</para>
        /// </summary>
        /// <param name="techType">The TechType whose QuickSlotType you want to set.</param>
        /// <param name="slotType">The QuickSlotType for that TechType.</param>
        public static void AddQuickSlotType(TechType techType, QuickSlotType slotType) => EditQuickSlotType(techType, slotType);

        /// <summary>
        /// <para>Allows you to add harvest output, i.e. what TechType you get when you "harvest" your TechType.</para>        
        /// </summary>
        /// <param name="techType">The TechType whose harvest output you want to set.</param>
        /// <param name="harvestOutput">The harvest output for that TechType.</param>
        public static void AddHarvestOutputList(TechType techType, TechType harvestOutput) => EditHarvestOutputList(techType, harvestOutput);

        /// <summary>
        /// <para>Allows you to set how your TechType is harvested.</para>
        /// </summary>
        /// <param name="techType">The TechType whose HarvestType you want to set.</param>
        /// <param name="harvestType">The HarvestType for that TechType.</param>
        public static void AddHarvestTypeList(TechType techType, HarvestType harvestType) => EditHarvestTypeList(techType, harvestType);

        /// <summary>
        /// <para>Allows you to set a non-default item size for your TechType.</para>
        /// <para>By default item sizes are 1x1 in the inventory.</para>
        /// </summary>
        /// <param name="techType">The TechType whose item size you want to set.</param>
        /// <param name="size">The item size for that TechType.</param>
        public static void AddItemSize(TechType techType, Vector2int size) => EditItemSize(techType, size);

        /// <summary>
        /// <para>Allows you to add a non-default crafting time for your TechType.</para>
        /// </summary>
        /// <param name="techType">The TechType whose crafting time you want to set.</param>
        /// <param name="time">The crafting time, in seconds, for that TechType.</param>
        public static void AddCraftingTime(TechType techType, float time) => EditCraftingTime(techType, time);

        /// <summary>
        /// <para>Allows you to pair your cooked creature TechType with your unedible/uncooked creature TechType.</para>
        /// </summary>
        /// <param name="uncooked">The TechType whose cooked creature counterpart to edit.</param>
        /// <param name="cooked">The cooked creature counterpart for that TechType.</param>
        public static void AddCookedCreatureList(TechType uncooked, TechType cooked) => EditCookedCreatureList(uncooked, cooked);

        #endregion
    }
}
