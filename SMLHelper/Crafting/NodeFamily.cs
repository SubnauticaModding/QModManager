namespace SMLHelper.V2.Crafting
{
    using Assets;
    using Patchers;

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

    internal class CraftingNode : Node
    {
        internal TechType TechType { get; set; }

        internal CraftingNode(string[] path, CraftTree.Type scheme, TechType techType) : base(path, scheme)
        {
            TechType = techType;
        }
    }

    internal class TabNode : Node
    {
        internal Atlas.Sprite Sprite { get; set; }
        internal string DisplayName { get; set; }
        internal string Name { get; set; }

        internal TabNode(string[] path, CraftTree.Type scheme, Atlas.Sprite sprite, string name, string displayName) : base(path, scheme)
        {
            Sprite = sprite;
            DisplayName = displayName;
            Name = name;

            ModSprite.Sprites.Add(new ModSprite(SpriteManager.Group.Category, $"{Scheme.ToString()}_{Name}", Sprite));
            LanguagePatcher.customLines[$"{Scheme.ToString()}Menu_{Name}"] = DisplayName;
        }
    }
}
