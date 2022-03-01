namespace SMLHelper.V2.Crafting
{
    /// <summary>
    /// A crafting node of a CraftTree. This is the last node on a tree; The one that actually crafts something.
    /// </summary>
    /// <seealso cref="ModCraftTreeNode" />
    public class ModCraftTreeCraft : ModCraftTreeNode
    {
        internal ModCraftTreeCraft(TechType techType)
            : base(techType.AsString(), TreeAction.Craft, techType)
        {
        }
    }
}
