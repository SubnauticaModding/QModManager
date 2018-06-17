using System.Collections.Generic;
using UnityEngine;
using CustomSpriteHandler2 = SMLHelper.V2.CustomSpriteHandler;
using CustomSprite2 = SMLHelper.V2.CustomSprite;

namespace SMLHelper
{
    public class CustomSpriteHandler
    {
        public static List<CustomSprite> customSprites = new List<CustomSprite>();

        internal static void Patch()
        {
            customSprites.ForEach(x => CustomSpriteHandler2.customSprites.Add(x.GetV2Sprite()));
        }
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

        public CustomSprite2 GetV2Sprite()
        {
            var customSprite = new CustomSprite2(TechType, Sprite);
            customSprite.Id = Id;
            customSprite.Group = Group;

            return customSprite;
        }
    }
}
