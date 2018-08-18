namespace SMLHelper.V2.QuickStart
{
    using Handlers;

    /// <summary>
    /// An item that can be crafted into the game world.
    /// </summary>
    /// <seealso cref="PdaItem" />
    /// <seealso cref="Spawnable" />
    public abstract class Craftable : PdaItem
    {
        /// <summary>
        /// Override with the fabricator that crafts this item.
        /// </summary>
        public abstract CraftTree.Type FabricatorType { get; }

        /// <summary>
        /// Override with the tab node steps to take to get to the tab you want the item's blueprint to appear in.
        /// If not overriden, the item will appear at the craft tree's root.
        /// </summary>
        public virtual string[] StepsToFabricatorTab => null;

        /// <summary>
        /// Initializes a new <see cref="Craftable"/>, the basic class for any item that can be crafted at a fabricator.
        /// </summary>
        /// <param name="classId">The main internal identifier for this item. Your item's <see cref="TechType" /> will be created using this name.</param>
        /// <param name="friendlyName">The name displayed in-game for this item whether in the open world or in the inventory.</param>
        /// <param name="description">The description for this item; Typically seen in the PDA, inventory, or crafting screens.</param>
        protected Craftable(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description)
        {
            CorePatchEvents += PatchCraftingTree;
        }

        private void PatchCraftingTree()
        {
            if (this.StepsToFabricatorTab is null)
                CraftTreeHandler.AddCraftingNode(this.FabricatorType, this.TechType);
            else
                CraftTreeHandler.AddCraftingNode(this.FabricatorType, this.TechType, this.StepsToFabricatorTab);
        }
    }
}
