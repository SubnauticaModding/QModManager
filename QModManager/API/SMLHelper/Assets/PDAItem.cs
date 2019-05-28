namespace QModManager.API.SMLHelper.Assets
{
    using Crafting;
    using Handlers;

    /// <summary>
    /// A <see cref="Spawnable"/> item that appears in the PDA blueprints.
    /// </summary>
    /// <seealso cref="Spawnable" />
    public abstract class PDAItem : Spawnable
    {
        /// <summary>
        /// Override to set the <see cref="TechType"/> that must first be scanned or picked up to unlock the blueprint for this item.
        /// If not overriden, it this item will be unlocked from the start of the game.
        /// </summary>
        public virtual TechType RequiredForUnlock => TechType.None;

        /// <summary>
        /// Override with the main group in the PDA blueprints where this item appears.
        /// </summary>
        public abstract TechGroup GroupForPDA { get; }

        /// <summary>
        /// Override with the category within the group in the PDA blueprints where this item appears.
        /// </summary>
        public abstract TechCategory CategoryForPDA { get; }

        /// <summary>
        /// Gets a value indicating whether <see cref="RequiredForUnlock"/> has been set to lock this blueprint behind another <see cref="TechType"/>.
        /// </summary>
        /// <value>
        ///   Returns <c>true</c> if will be unlocked from the start of the game; otherwise, <c>false</c>.
        /// </value>
        /// <seealso cref="RequiredForUnlock"/>
        public bool UnlockedAtStart => this.RequiredForUnlock == TechType.None;

        /// <summary>
        /// Message which should be shown when the item is unlocked
        /// </summary>
        public virtual string DiscoverMessage => $"{this.FriendlyName} blueprint discovered!";

        internal string DiscoverMessageKey => $"{TechType.AsString()}_DiscoverMessage";

        /// <summary>
        /// Initializes a new <see cref="PDAItem"/>, the basic class for any item that appears among your PDA blueprints. <para/>
        /// DO NOT USE THIS CLASS DIRECTLY! Use <seealso cref="Craftable"/> or <see cref="Buildable"/> instead.
        /// </summary>
        /// <param name="classId">The main internal identifier for this item. Your item's <see cref="TechType" /> will be created using this name.</param>
        /// <param name="friendlyName">The name displayed in-game for this item whether in the open world or in the inventory.</param>
        /// <param name="description">The description for this item; Typically seen in the PDA, inventory, or crafting screens.</param>
        internal PDAItem(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description)
        {
            CorePatchEvents += PatchTechDataEntry;

            LanguageHandler.SetLanguageLine(DiscoverMessageKey, DiscoverMessage);
        }

        /// <summary>
        /// This provides the <see cref="TechData"/> instance used to designate how this item is crafted or constructed.
        /// </summary>
        protected abstract TechData GetBlueprintRecipe();

        private void PatchTechDataEntry()
        {
            CraftDataHandler.SetTechData(this.TechType, GetBlueprintRecipe());

            CraftDataHandler.AddToGroup(this.GroupForPDA, this.CategoryForPDA, this.TechType);

            if (this.UnlockedAtStart)
                KnownTechHandler.UnlockOnStart(this.TechType);
            else
                KnownTechHandler.SetAnalysisTechEntry(this.RequiredForUnlock, new TechType[1] { this.TechType }, Language.main.Get(this.DiscoverMessageKey));
        }
    }
}
