using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Harmony;
using UnityEngine;

namespace SMLHelper.Patchers
{
    public class SpritePatcher
    {
        public static void Patch(HarmonyInstance harmony)
        {
            //var atlas = Atlas.GetAtlas("Items");
            //if(atlas == null)
            //{
            //    SubnauticaModLoader.Logging.Logger.Log("ll");
            //    return;
            //}

            //var propulsioncannon = atlas.GetSprite("propulsioncannon");
            //GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

            //foreach(var gameobject in allObjects)
            //{
            //    if(gameobject.name == "MainCanvas")
            //    {
            //        var newGameOject = AssetBundle.LoadFromFile("testmodel").LoadAsset("Image") as GameObject;
            //        newGameOject.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(propulsioncannon.texture, new Rect(0, 0, propulsioncannon.texture.width, propulsioncannon.texture.height),
            //            new Vector2(0.5f, 0.5f));

            //        UnityEngine.Object.Instantiate(newGameOject, gameobject.transform);
            //    }
            //}

            var spriteManager = typeof(SpriteManager);
            var getFromResources = spriteManager.GetMethod("GetFromResources", BindingFlags.Public | BindingFlags.Static);

            harmony.Patch(getFromResources, 
                new HarmonyMethod(typeof(SpritePatcher).GetMethod("Prefix")), null);
        }

        public static bool Prefix(ref Atlas.Sprite __result, string name)
        {
            foreach(var sprite in CustomSpriteHandler.customSprites)
            {
                if (sprite.TechType.AsString(true) == name.ToLowerInvariant())
                {
                    __result = new Atlas.Sprite(sprite.Sprite, false);
                    return false;
                }
            }

            return true;
        }

    }
}
