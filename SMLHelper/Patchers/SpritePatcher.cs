namespace SMLHelper.V2.Patchers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Assets;

    internal class SpritePatcher
    {
        internal static void Patch()
        {
            FieldInfo groupsField = typeof(SpriteManager).GetField("groups", BindingFlags.Static | BindingFlags.NonPublic);

            var groups = (Dictionary<SpriteManager.Group, Dictionary<string, Atlas.Sprite>>)groupsField.GetValue(null);

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
