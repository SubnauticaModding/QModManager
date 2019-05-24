namespace SMLHelper.V2.Patchers
{
    using Assets;
    using System.Collections.Generic;

    internal class SpritePatcher
    {
        internal static void Patch()
        {
            // Direct access to private fields made possible by https://github.com/CabbageCrow/AssemblyPublicizer/
            // See README.md for details.
            Dictionary<SpriteManager.Group, Dictionary<string, Atlas.Sprite>> groups = SpriteManager.groups;

            foreach (SpriteManager.Group moddedGroup in ModSprite.ModSprites.Keys)
            {
                Dictionary<string, Atlas.Sprite> spriteGroup = groups[moddedGroup];

                foreach (string spriteKey in ModSprite.ModSprites[moddedGroup].Keys)
                {
                    spriteGroup.Add(spriteKey, ModSprite.ModSprites[moddedGroup][spriteKey]);
                }
            }

            Logger.Log("SpritePatcher is done.", LogLevel.Debug);
        }
    }
}
