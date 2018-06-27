using System.Collections.Generic;
using UnityEngine;
using SMLHelper.V2.Assets;

namespace SMLHelper
{
    [System.Obsolete("Use SMLHelper.V2 instead.")]
    public class CustomSpriteHandler
    {
        [System.Obsolete("Use SMLHelper.V2 instead.")]
        public static List<CustomSprite> customSprites = new List<CustomSprite>();

        internal static void Patch()
        {
            customSprites.ForEach(x => ModSprite.Sprites.Add(x.GetModSprite()));
        }
    }

    [System.Obsolete("Use SMLHelper.V2 instead.")]
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

        internal ModSprite GetModSprite()
        {
            var customSprite = new ModSprite(TechType, Sprite);
            customSprite.Id = Id;
            customSprite.Group = Group;

            return customSprite;
        }
    }
}
