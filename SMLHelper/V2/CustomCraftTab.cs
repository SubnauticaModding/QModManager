using SMLHelper.V2.Patchers;

namespace SMLHelper.V2
{
    public class CustomCraftTab
    {
        public string SpriteId
        {
            get
            {
                return Scheme.ToString() + "_" + System.IO.Path.GetFileName(Path);
            }
        }

        public string LanguageId
        {
            get
            {
                return Scheme.ToString() + "Menu_" + System.IO.Path.GetFileName(Path);
            }
        }

        public CraftTree.Type Scheme;
        public string Path;
        public string Name;
        public CustomSprite Sprite;

        public CustomCraftTab(string path, string name, CraftTree.Type scheme, Atlas.Sprite sprite)
        {
            Path = path;
            Name = name;
            Scheme = scheme;
            Sprite = new CustomSprite(SpriteManager.Group.Category, SpriteId, sprite);

            LanguagePatcher.customLines[LanguageId] = name;
            CustomSpriteHandler.customSprites.Add(Sprite);
        }
        
        public CustomCraftTab(string path, string name, CraftTree.Type scheme, UnityEngine.Sprite sprite) : this(path, name, scheme, new Atlas.Sprite(sprite, false))
        {
        }
    }
}