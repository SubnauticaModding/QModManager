namespace SMLHelper.V2.Handlers
{
    using Interfaces;
    using Patchers;

    /// <summary>
    /// A handler class for adding and editing crafted items.
    /// </summary>
    public partial class CraftDataHandler: ICraftDataHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static ICraftDataHandler Main { get; } = new CraftDataHandler();

        private CraftDataHandler()
        {
            // Hides constructor
        }

        #region Common Static Methods

        /// <summary>
        /// <para>Allows you to edit EquipmentTypes for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose EqiupmentType you want to edit.</param>
        /// <param name="equipmentType">The EquipmentType for that TechType.</param>
        public static void SetEquipmentType(TechType techType, EquipmentType equipmentType)
        {
            Main.SetEquipmentType(techType, equipmentType);
        }

#if SUBNAUTICA
        /// <summary>
        /// <para>Allows you to edit QuickSlotType for TechTypes. Can be used for existing TechTypes too.</para>
        /// <para>Careful: This has to be called after <see cref="SetTechData(TechType, Crafting.TechData)"/> and <see cref="SetTechData(TechType, Crafting.TechData)"/>.</para>
        /// </summary>
        /// <param name="techType">The TechType whose QuickSlotType you want to edit.</param>
        /// <param name="slotType">The QuickSlotType for that TechType.</param>
#elif BELOWZERO
        /// <summary>
        /// <para>Allows you to edit QuickSlotType for TechTypes. Can be used for existing TechTypes too.</para>
        /// <para>Careful: This has to be called after <see cref="SetTechData(TechType, Crafting.RecipeData)"/>.</para>
        /// </summary>
        /// <param name="techType">The TechType whose QuickSlotType you want to edit.</param>
        /// <param name="slotType">The QuickSlotType for that TechType.</param>
#endif
        public static void SetQuickSlotType(TechType techType, QuickSlotType slotType)
        {
            Main.SetQuickSlotType(techType, slotType);
        }

        /// <summary>
        /// <para>Allows you to edit harvest output, i.e. what TechType you get when you "harvest" a TechType.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose harvest output you want to edit.</param>
        /// <param name="harvestOutput">The harvest output for that TechType.</param>
        public static void SetHarvestOutput(TechType techType, TechType harvestOutput)
        {
            Main.SetHarvestOutput(techType, harvestOutput);
        }

        /// <summary>
        /// <para>Allows you to edit how TechTypes are harvested.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose HarvestType you want to edit.</param>
        /// <param name="harvestType">The HarvestType for that TechType.</param>
        public static void SetHarvestType(TechType techType, HarvestType harvestType)
        {
            Main.SetHarvestType(techType, harvestType);
        }

        /// <summary>
        /// <para>Allows you to edit how much additional slices/seeds are given upon last knife hit.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose final cut bonus you want to edit.</param>
        /// <param name="bonus">The number of additional slices/seeds you'll receive on last cut.</param>
        public static void SetHarvestFinalCutBonus(TechType techType, int bonus)
        {
            Main.SetHarvestFinalCutBonus(techType, bonus);
        }

        /// <summary>
        /// <para>Allows you to edit item sizes for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose item size you want to edit.</param>
        /// <param name="size">The item size for that TechType.</param>
        public static void SetItemSize(TechType techType, Vector2int size)
        {
            Main.SetItemSize(techType, size);
        }

        /// <summary>
        /// <para>Allows you to edit item sizes for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose item size you want to edit.</param>
        /// <param name="x">The width of the item</param>
        /// <param name="y">The height of the item</param>
        public static void SetItemSize(TechType techType, int x, int y)
        {
            Main.SetItemSize(techType, x, y);
        }

        /// <summary>
        /// <para>Allows you to edit crafting times for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose crafting time you want to edit.</param>
        /// <param name="time">The crafting time, in seconds, for that TechType.</param>
        public static void SetCraftingTime(TechType techType, float time)
        {
            Main.SetCraftingTime(techType, time);
        }

        /// <summary>
        /// <para>Allows you to edit the cooked creature list, i.e. associate the unedible TechType to the cooked TechType.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="uncooked">The TechType whose cooked creature counterpart to edit.</param>
        /// <param name="cooked">The cooked creature counterpart for that TechType.</param>
        public static void SetCookedVariant(TechType uncooked, TechType cooked)
        {
            Main.SetCookedVariant(uncooked, cooked);
        }

        /// <summary>
        /// <para>Allows you to edit inventory background colors for TechTypes.</para>
        /// </summary>
        /// <param name="techType">The TechType whose BackgroundType you want to edit.</param>
        /// <param name="backgroundColor">The background color for that TechType.</param>
        /// <seealso cref="CraftData.BackgroundType"/>
        public static void SetBackgroundType(TechType techType, CraftData.BackgroundType backgroundColor)
        {
            Main.SetBackgroundType(techType, backgroundColor);
        }

        /// <summary>
        /// Allows you to add items to the buildable list.
        /// </summary>
        /// <param name="techType">The TechType which you want to add to the buildable list.</param>
        public static void AddBuildable(TechType techType)
        {
            Main.AddBuildable(techType);
        }

        /// <summary>
        /// Allows you to add items to the game's internal grouping system.
        /// Required if you want to make buildable items show up in the Habitat Builder.
        /// </summary>
        /// <param name="group">The TechGroup you want to add your TechType to.</param>
        /// <param name="category">The TechCategory (in the TechGroup) you want to add your TechType to.</param>
        /// <param name="techType">The TechType you want to add.</param>
        public static void AddToGroup(TechGroup group, TechCategory category, TechType techType)
        {
            Main.AddToGroup(group, category, techType, TechType.None);
        }

        /// <summary>
        /// Allows you to add items to the game's internal grouping system.
        /// Required if you want to make buildable items show up in the Habitat Builder.
        /// </summary>
        /// <param name="group">The TechGroup you want to add your TechType to.</param>
        /// <param name="category">The TechCategory (in the TechGroup) you want to add your TechType to.</param>
        /// <param name="techType">The TechType you want to add.</param>
        /// <param name="after">Added TechType will be added after this TechType, for sorting purposes.</param>
        public static void AddToGroup(TechGroup group, TechCategory category, TechType techType, TechType after)
        {
            Main.AddToGroup(group, category, techType, after);
        }

        /// <summary>
        /// Allows you to remove an existing TechType from the game's internal group system.
        /// </summary>
        /// <param name="group">The TechGroup in which the TechType is located.</param>
        /// <param name="category">The TechCategory in which the TechType is located.</param>
        /// <param name="techType">The TechType which you want to remove.</param>
        public static void RemoveFromGroup(TechGroup group, TechCategory category, TechType techType)
        {
            Main.RemoveFromGroup(group, category, techType);
        }

        #endregion

        #region Common Interface Methods

        /// <summary>
        /// Allows you to add items to the game's internal grouping system.
        /// Required if you want to make buildable items show up in the Habitat Builder or show in the Blueprints Tab of the PDA.
        /// </summary>
        /// <param name="group">The TechGroup you want to add your TechType to.</param>
        /// <param name="category">The TechCategory (in the TechGroup) you want to add your TechType to.</param>
        /// <param name="techType">The TechType you want to add.</param>
        void ICraftDataHandler.AddToGroup(TechGroup group, TechCategory category, TechType techType)
        {
            CraftDataPatcher.AddToCustomGroup(group, category, techType, TechType.None);
        }

        /// <summary>
        /// Allows you to add items to the game's internal grouping system.
        /// Required if you want to make buildable items show up in the Habitat Builder or show in the Blueprints Tab of the PDA.
        /// </summary>
        /// <param name="group">The TechGroup you want to add your TechType to.</param>
        /// <param name="category">The TechCategory (in the TechGroup) you want to add your TechType to.</param>
        /// <param name="techType">The TechType you want to add.</param>
        /// <param name="after">Added TechType will be added after this TechType, for sorting purposes.</param>
        void ICraftDataHandler.AddToGroup(TechGroup group, TechCategory category, TechType techType, TechType after)
        {
            CraftDataPatcher.AddToCustomGroup(group, category, techType, after);
        }

        /// <summary>
        /// Allows you to remove an existing TechType from the game's internal group system.
        /// </summary>
        /// <param name="group">The TechGroup in which the TechType is located.</param>
        /// <param name="category">The TechCategory in which the TechType is located.</param>
        /// <param name="techType">The TechType which you want to remove.</param>
        void ICraftDataHandler.RemoveFromGroup(TechGroup group, TechCategory category, TechType techType)
        {
            CraftDataPatcher.RemoveFromCustomGroup(group, category, techType);
        }

        #endregion
    }
}
