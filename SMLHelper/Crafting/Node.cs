namespace SMLHelper.V2.Crafting
{
    internal class Node
    {
        internal string[] Path { get; set; }
        internal CraftTree.Type Scheme { get; set; }

        internal Node(string[] path, CraftTree.Type scheme)
        {
            Path = path;
            Scheme = scheme;
        }
    }
}
