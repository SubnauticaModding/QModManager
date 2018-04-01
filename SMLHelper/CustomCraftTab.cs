using SMLHelper.Patchers;

namespace SMLHelper
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

        public CraftScheme Scheme;
        public string Path;
        public string Name;
        public CustomSprite Sprite;

        public CustomCraftTab(string path, string name, CraftScheme scheme, Atlas.Sprite sprite)
        {
            Path = path;
            Name = name;
            Scheme = scheme;
            Sprite = new CustomSprite(SpriteManager.Group.Category, SpriteId, sprite);

            LanguagePatcher.customLines[LanguageId] = name;
            CustomSpriteHandler.customSprites.Add(Sprite);
        }
        
        public CustomCraftTab(string path, string name, CraftScheme scheme, UnityEngine.Sprite sprite) : this(path, name, scheme, new Atlas.Sprite(sprite, false))
        {
        }
    }
}