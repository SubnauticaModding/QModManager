namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System.Reflection;
    using Assets;

    internal class SpritePatcher
    {
        internal static void Patch(HarmonyInstance harmony)
        {
            var spriteManager = typeof(SpriteManager);
            var getFromResources = spriteManager.GetMethod("GetFromResources", BindingFlags.Public | BindingFlags.Static);

            harmony.Patch(getFromResources,
                new HarmonyMethod(typeof(SpritePatcher).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic)), null);
            Logger.Log("SpritePatcher is done.");
        }

        internal static bool Prefix(ref Atlas.Sprite __result, string name)
        {
            foreach (var sprite in ModSprite.Sprites)
            {
                if (sprite.TechType.AsString(true) == name.ToLowerInvariant())
                {
                    __result = sprite.Sprite;
                    return false;
                }
                else if(sprite.TechType == TechType.None && sprite.Id.ToLowerInvariant() == name.ToLowerInvariant())
                {
                    __result = sprite.Sprite;
                    return false;
                }
            }

            return true;
        }

    }
}
