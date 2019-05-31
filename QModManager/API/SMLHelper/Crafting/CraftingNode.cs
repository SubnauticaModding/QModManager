namespace QModManager.API.SMLHelper.Crafting
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
