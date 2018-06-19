namespace SMLHelper.V2
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CustomSpriteHandler
    {
        public static List<CustomSprite> customSprites = new List<CustomSprite>();
    }

    public class CustomSprite
    {
        public TechType TechType;
        public Atlas.Sprite Sprite;

        public SpriteManager.Group Group;
        public string Id;

        public CustomSprite(TechType type, Atlas.Sprite sprite)
        {
            TechType = type;
            Sprite = sprite;
        }

        public CustomSprite(SpriteManager.Group group, string id, Atlas.Sprite sprite)
        {
            Group = group;
            Id = id;
            Sprite = sprite;

            TechType = TechType.None;
        }

        public CustomSprite(SpriteManager.Group group, string id, Sprite sprite) : this(group, id, new Atlas.Sprite(sprite, false))
        {
        }

        public CustomSprite(TechType type, Sprite sprite) : this(type, new Atlas.Sprite(sprite, false))
        {
        }
    }
}
