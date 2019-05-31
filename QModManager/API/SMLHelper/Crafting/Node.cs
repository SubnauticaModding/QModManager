namespace QModManager.API.SMLHelper.Crafting
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
