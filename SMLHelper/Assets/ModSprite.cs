namespace SMLHelper.V2.Assets
{
    using System.Collections.Generic;

    /// <summary>
    /// A class that handles a custom sprite and what item it is associated to.
    /// </summary>
    public class ModSprite
    {
        internal static List<ModSprite> Sprites = new List<ModSprite>();

        public TechType TechType;
        public Atlas.Sprite Sprite;
        public SpriteManager.Group Group;
        public string Id;

        public ModSprite(TechType type, Atlas.Sprite sprite)
        {
            TechType = type;
            Sprite = sprite;
        }

        public ModSprite(SpriteManager.Group group, string id, Atlas.Sprite sprite)
        {
            Group = group;
            Id = id;
            Sprite = sprite;
            TechType = TechType.None;
        }

        public ModSprite(SpriteManager.Group group, string id, UnityEngine.Sprite sprite) : this(group, id, new Atlas.Sprite(sprite, false))
        {
        }

        public ModSprite(TechType type, UnityEngine.Sprite sprite) : this(type, new Atlas.Sprite(sprite, false))
        {
        }
    }
}
