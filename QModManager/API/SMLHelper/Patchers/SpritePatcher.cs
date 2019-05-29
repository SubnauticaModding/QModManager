namespace QModManager.API.SMLHelper.Patchers
{
    using Assets;
    using QModManager.Utility;
    using System.Collections.Generic;

    internal static class SpritePatcher
    {
        internal static void Patch()
        {
            Dictionary<SpriteManager.Group, Dictionary<string, Atlas.Sprite>> groups = SpriteManager.groups;

            foreach (SpriteManager.Group moddedGroup in ModSprite.ModSprites.Keys)
            {
                Dictionary<string, Atlas.Sprite> spriteGroup = groups[moddedGroup];

                foreach (string spriteKey in ModSprite.ModSprites[moddedGroup].Keys)
                {
                    spriteGroup.Add(spriteKey, ModSprite.ModSprites[moddedGroup][spriteKey]);
                }
            }

            Logger.Log("SpritePatcher is done.");
        }
    }
}
