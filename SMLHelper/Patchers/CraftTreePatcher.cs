using Harmony;
using System.Collections.Generic;
using System.Reflection;

namespace SMLHelper.Patchers
{
    public class CraftTreePatcher
    {
        public static List<CustomCraftTab> customTabs = new List<CustomCraftTab>();
        public static List<CustomCraftNode> customNodes = new List<CustomCraftNode>();

        public static void FabricatorSchemePostfix(ref CraftNode __result)
        {
            AddCustomTabs(ref __result, customTabs, CraftScheme.Fabricator);
            PatchNodes(ref __result, customNodes, CraftScheme.Fabricator);
        }

        public static void ConstructorSchemePostfix(ref CraftNode __result)
        {
            AddCustomTabs(ref __result, customTabs, CraftScheme.Constructor);
            PatchNodes(ref __result, customNodes, CraftScheme.Constructor);
        }

        public static void WorkbenchSchemePostfix(ref CraftNode __result)
        {
            AddCustomTabs(ref __result, customTabs, CraftScheme.Workbench);
            PatchNodes(ref __result, customNodes, CraftScheme.Workbench);
        }

        public static void SeamothUpgradesSchemePostfix(ref CraftNode __result)
        {
            AddCustomTabs(ref __result, customTabs, CraftScheme.SeamothUpgrades);
            PatchNodes(ref __result, customNodes, CraftScheme.SeamothUpgrades);
        }

        public static void MapRoomShemePostfix(ref CraftNode __result)
        {
            AddCustomTabs(ref __result, customTabs, CraftScheme.MapRoom);
            PatchNodes(ref __result, customNodes, CraftScheme.MapRoom);
        }

        public static void CyclopsFabricatorSchemePostfix(ref CraftNode __result)
        {
            AddCustomTabs(ref __result, customTabs, CraftScheme.CyclopsFabricator);
            PatchNodes(ref __result, customNodes, CraftScheme.CyclopsFabricator);
        }

        private static void AddCustomTabs(ref CraftNode nodes, List<CustomCraftTab> customTabs, CraftScheme scheme)
        {
            foreach(var tab in customTabs)
            {
                if (tab.Scheme != scheme) continue;

                var path = tab.Path.SplitByChar('/');
                var currentNode = default(TreeNode);
                currentNode = nodes;

                for(int i = 0; i < path.Length; i++)
                {
                    var currentPath = path[i];

                    var node = currentNode[currentPath];
                    if(node == null)
                    {
                        var newNode = new CraftNode(currentPath, TreeAction.Expand, TechType.None);
                        currentNode.AddNode(new TreeNode[]
                        {
                            newNode
                        });

                        node = newNode;
                    }

                    currentNode = node;
                }
            }
        }

        private static void PatchNodes(ref CraftNode nodes, List<CustomCraftNode> customNodes, CraftScheme scheme)
        {
            foreach(var customNode in customNodes)
            {
                if (customNode.Scheme != scheme) continue;

                var path = customNode.Path.SplitByChar('/');
                var currentNode = default(TreeNode);
                currentNode = nodes;

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
                    new CraftNode(path[path.Length - 1], TreeAction.Craft, customNode.TechType)
                });
            }
        }

        public static void Patch(HarmonyInstance harmony)
        {
            var type = typeof(CraftTree);
            var fabricatorScheme = type.GetMethod("FabricatorScheme", BindingFlags.Static | BindingFlags.NonPublic);
            var constructorScheme = type.GetMethod("ConstructorScheme", BindingFlags.Static | BindingFlags.NonPublic);
            var workbenchScheme = type.GetMethod("WorkbenchScheme", BindingFlags.Static | BindingFlags.NonPublic);
            var seamothUpgradesScheme = type.GetMethod("SeamothUpgradesScheme", BindingFlags.NonPublic | BindingFlags.Static);
            var mapRoomSheme = type.GetMethod("MapRoomSheme", BindingFlags.Static | BindingFlags.NonPublic);
            var cyclopsFabricatorScheme = type.GetMethod("CyclopsFabricatorScheme", BindingFlags.Static | BindingFlags.NonPublic);

            harmony.Patch(fabricatorScheme, null,
                new HarmonyMethod(typeof(CraftTreePatcher).GetMethod("FabricatorSchemePostfix")));

            harmony.Patch(constructorScheme, null,
                new HarmonyMethod(typeof(CraftTreePatcher).GetMethod("ConstructorSchemePostfix")));

            harmony.Patch(workbenchScheme, null,
                new HarmonyMethod(typeof(CraftTreePatcher).GetMethod("WorkbenchSchemePostfix")));

            harmony.Patch(seamothUpgradesScheme, null,
                new HarmonyMethod(typeof(CraftTreePatcher).GetMethod("SeamothUpgradesSchemePostfix")));

            harmony.Patch(mapRoomSheme, null,
                new HarmonyMethod(typeof(CraftTreePatcher).GetMethod("MapRoomShemePostfix")));

            harmony.Patch(cyclopsFabricatorScheme, null,
                new HarmonyMethod(typeof(CraftTreePatcher).GetMethod("CyclopsFabricatorSchemePostfix")));

            Logger.Log($"CraftTreePatcher is done.");
        }
    }
}
