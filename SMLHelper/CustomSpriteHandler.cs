using System.Collections.Generic;
using UnityEngine;

namespace SMLHelper
{
    public class CustomSpriteHandler
    {
        public static List<CustomSprite> customSprites = new List<CustomSprite>();
    }

    public class CustomSprite
    {
        public TechType TechType;
        public Atlas.Sprite Sprite;

        public CustomSprite(TechType type, Sprite sprite)
        {
            TechType = type;
            Sprite = new Atlas.Sprite(sprite, false);
        }

        public CustomSprite(TechType type, Atlas.Sprite sprite)
        {
            TechType = type;
            Sprite = sprite;
        }
    }
}
