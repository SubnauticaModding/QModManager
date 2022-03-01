namespace SMLHelper.V2.Crafting
{
    internal class CraftingNode : Node
    {
        internal TechType TechType { get; set; }

        internal CraftingNode(string[] path, CraftTree.Type scheme, TechType techType) : base(path, scheme)
        {
            TechType = techType;
        }
    }
}
