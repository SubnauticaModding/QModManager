namespace QModManager.API.SMLHelper.Crafting
{
    using Assets;
    using Patchers;

    internal class TabNode : Node
    {
        internal Atlas.Sprite Sprite { get; set; }
        internal string DisplayName { get; set; }
        internal string Name { get; set; }

        internal TabNode(string[] path, CraftTree.Type scheme, Atlas.Sprite sprite, string modName, string name, string displayName) : base(path, scheme)
        {
            Sprite = sprite;
            DisplayName = displayName;
            Name = name;

            ModSprite.Add(new ModSprite(SpriteManager.Group.Category, $"{Scheme.ToString()}_{Name}", Sprite));
            LanguagePatcher.AddCustomLanguageLine(modName, $"{Scheme.ToString()}Menu_{Name}", DisplayName);
        }
    }
}
