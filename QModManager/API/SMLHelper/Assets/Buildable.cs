namespace SMLHelper.V2.Assets
{
    using Handlers;

    /// <summary>
    /// An item that can be built into the game world.
    /// </summary>
    /// <seealso cref="PdaItem" />
    /// <seealso cref="Spawnable"/>
    public abstract class Buildable : PdaItem
    {
        /// <summary>
        /// Override with the text to be displayed when your hover the cursor over the built item.
        /// </summary>
        public virtual string HandOverText => null;

        /// <summary>
        /// Initializes a new <see cref="Buildable"/>, the basic class for any item that can built using the Habitat Builder Tool.
        /// </summary>
        /// <param name="classId">The main internal identifier for this item. Your item's <see cref="TechType" /> will be created using this name.</param>
        /// <param name="friendlyName">The name displayed in-game for this item whether in the open world or in the inventory.</param>
        /// <param name="description">The description for this item; Typically seen in the PDA, inventory, or crafting screens.</param>
        protected Buildable(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description)
        {
            CorePatchEvents += PatchBuildable;
        }

        private void PatchBuildable()
        {
            CraftDataHandler.AddBuildable(this.TechType);

            if (!string.IsNullOrEmpty(HandOverText))
            {
                LanguageHandler.SetLanguageLine($"{this.ClassID}HandOver", this.HandOverText);
            }
        }
    }
}
