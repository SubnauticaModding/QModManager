namespace SMLHelper.V2.Assets
{
    using Crafting;
    using SMLHelper.V2.Interfaces;
    using System.Collections.Generic;

    /// <summary>
    /// A <see cref="Spawnable"/> item that appears in the PDA blueprints.
    /// </summary>
    /// <seealso cref="Spawnable" />
    public abstract class PdaItem: Spawnable
    {
        internal IKnownTechHandler KnownTechHandler { get; set; } = Handlers.KnownTechHandler.Main;
        internal IPDAHandler PDAHandler { get; set; } = Handlers.PDAHandler.Main;
        internal IPDAEncyclopediaHandler PDAEncyclopediaHandler { get; set; } = Handlers.PDAEncyclopediaHandler.Main;

        /// <summary>
        /// Override to set the <see cref="TechType"/> that must first be scanned or picked up to unlock the blueprint for this item.
        /// If not overriden, it this item will be unlocked from the start of the game.
        /// </summary>
        public virtual TechType RequiredForUnlock => TechType.None;

        /// <summary>
        /// Override to add a scanner entry to the <see cref="RequiredForUnlock"/> TechType if it does not have one.
        /// WARNING. You can overwrite an existing entry with this. Use with Caution as this can break recipe unlocks of the original! 
        /// Default is <see langword="false"/>.
        /// </summary>
        public virtual bool AddScannerEntry => false;

        /// <summary>
        /// Override to set the number of <see cref="RequiredForUnlock"/> that must be scanned to unlock;
        /// If not overriden, Default value is <see langword="1 fragment"/>.
        /// </summary>
        public virtual int FragmentsToScan => 1;

        /// <summary>
        /// Override to set the speed that the <see cref="RequiredForUnlock"/> fragments are scanned;
        /// If not overriden, Default value is <see langword="2 seconds"/>.
        /// </summary>
        public virtual float TimeToScanFragment => 2f;

        /// <summary>
        /// Override to allow fragments to be scanned for materials after the relavent TechType is already Unlocked.
        /// Default is <see langword="false"/>.
        /// </summary>
        public virtual bool DestroyFragmentOnScan => false;

        /// <summary>
        /// Override to add a <see cref="PDAEncyclopedia.EntryData"/> into the PDA's Encyclopedia for this object.
        /// WARNING. You can overwrite an existing entry with this. Use with Caution! 
        /// Default is <see langword="Null"/>.
        /// </summary>
        public virtual PDAEncyclopedia.EntryData EncyclopediaEntryData => null;

        /// <summary>
        /// Override with the main group in the PDA blueprints where this item appears.
        /// </summary>
        public virtual TechGroup GroupForPDA => TechGroup.Uncategorized;

        /// <summary>
        /// Override with the category within the group in the PDA blueprints where this item appears.
        /// </summary>
        public virtual TechCategory CategoryForPDA => TechCategory.Misc;

        /// <summary>
        /// Override this property to assign whether or not the <see cref="TechType"/> should be unlocked at the start, defaulted to <c><see cref="RequiredForUnlock"/> == <see cref="TechType.None"/></c>
        /// </summary>
        public virtual bool UnlockedAtStart => RequiredForUnlock == TechType.None;

        /// <summary>
        /// Message which should be shown when the item is unlocked. <para/>
        /// If not overridden, the message will default to Subnautica's (language key "<see langword="NotificationBlueprintUnlocked"/>").
        /// </summary>
        public virtual string DiscoverMessage => null;

        internal string DiscoverMessageResolved => DiscoverMessage == null ? "NotificationBlueprintUnlocked" : $"{TechType.AsString()}_DiscoverMessage";

        /// <summary>
        /// Initializes a new <see cref="PdaItem"/>, the basic class for any item that appears among your PDA blueprints.
        /// </summary>
        /// <param name="classId">The main internal identifier for this item. Your item's <see cref="TechType" /> will be created using this name.</param>
        /// <param name="friendlyName">The name displayed in-game for this item whether in the open world or in the inventory.</param>
        /// <param name="description">The description for this item; Typically seen in the PDA, inventory, or crafting screens.</param>
        protected PdaItem(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description)
        {
            CorePatchEvents += PatchTechDataEntry;
        }
#if SUBNAUTICA
        /// <summary>
        /// This provides the <see cref="TechData"/> instance used to designate how this item is crafted or constructed.
        /// </summary>
        protected abstract TechData GetBlueprintRecipe();
#elif BELOWZERO
        /// <summary>
        /// This provides the <see cref="RecipeData"/> instance used to designate how this item is crafted or constructed.
        /// </summary>
        protected abstract RecipeData GetBlueprintRecipe();
#endif
        private void PatchTechDataEntry()
        {
            CraftDataHandler.SetTechData(TechType, GetBlueprintRecipe());


            if(GroupForPDA != TechGroup.Uncategorized)
            {
                List<TechCategory> categories = new List<TechCategory>();
                CraftData.GetBuilderCategories(GroupForPDA, categories);
                if(categories.Contains(CategoryForPDA))
                {
                    CraftDataHandler.AddToGroup(GroupForPDA, CategoryForPDA, TechType);
                }
                else
                {
                    Logger.Error($"Failed to add {TechType} to {GroupForPDA}/{CategoryForPDA} as it is not a registered combination.");
                }
            }

            if(EncyclopediaEntryData != null)
            {
                PDAEncyclopediaHandler.AddCustomEntry(EncyclopediaEntryData);
            }

            if(!UnlockedAtStart)
            {
                KnownTechHandler.SetAnalysisTechEntry(RequiredForUnlock, new TechType[1] { TechType }, DiscoverMessageResolved);

                if(AddScannerEntry)
                {
                    PDAScanner.EntryData entryData = new PDAScanner.EntryData()
                    {
                        key = RequiredForUnlock,
                        blueprint = TechType,
                        destroyAfterScan = DestroyFragmentOnScan,
                        isFragment = true,
                        locked = true,
                        scanTime = TimeToScanFragment,
                        totalFragments = FragmentsToScan
                    };

                    if(EncyclopediaEntryData != null)
                    {
                        entryData.encyclopedia = EncyclopediaEntryData.key;
                    }
                    PDAHandler.AddCustomScannerEntry(entryData);
                }
            }
        }

        internal sealed override void PatchTechType()
        {
            TechType = TechTypeHandler.AddTechType(Mod, ClassID, FriendlyName, Description, UnlockedAtStart);
        }
    }
}
