namespace SMLHelper.V2.Patchers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Assets;
    using Harmony;

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

        internal static bool Prefix(ref Atlas.Sprite __result, SpriteManager.Group group, string name)
        {
            if (!ModSprite.ModSprites.ContainsKey(group))
                return true;

            Logger.Log($"Get Sprite Call {group}:{name}.");

            Dictionary<string, Atlas.Sprite> modGroup = ModSprite.ModSprites[group];

            if (!modGroup.ContainsKey(name))
                return true;

            __result = modGroup[name];
            return false;
        }
    }
}
