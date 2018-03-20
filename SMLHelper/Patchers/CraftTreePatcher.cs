using Harmony;
using System.Collections.Generic;
using System.Reflection;

namespace SMLHelper.Patchers
{
    public class CraftTreePatcher
    {
        public static Dictionary<string, TechType> customCraftNodes = new Dictionary<string, TechType>();

        public static void Postfix(ref CraftNode __result)
        {
            foreach (var craftNode2 in customCraftNodes)
            {
                var path = craftNode2.Key.SplitByChar('/');

                var currentNode = default(TreeNode);
                currentNode = __result;

                for (int i = 0; i < path.Length; i++)
                {
                    var currentPath = path[i];
                    if (i == (path.Length - 1))
                    {
                        break;
                    }

                    currentNode = currentNode[currentPath];
                }

                currentNode.AddNode(new TreeNode[]
                {
                    new CraftNode(path[path.Length - 1], TreeAction.Craft, craftNode2.Value)
                });
            }
        }

        public static void Patch(HarmonyInstance harmony)
        {
            var type = typeof(CraftTree);
            var fabricatorScheme = type.GetMethod("FabricatorScheme", BindingFlags.Static | BindingFlags.NonPublic);

            harmony.Patch(fabricatorScheme, null,
                new HarmonyMethod(typeof(CraftTreePatcher).GetMethod("Postfix")));
        }
    }
}
