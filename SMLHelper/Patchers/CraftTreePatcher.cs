namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System.Collections.Generic;
    using System.Reflection;
    using Utility;
    using Crafting;
    using Assets;
    using System;

    internal class CraftTreePatcher
    {
        #region Internal Fields

        internal static Dictionary<CraftTree.Type, ModCraftTreeRoot> CustomTrees = new Dictionary<CraftTree.Type, ModCraftTreeRoot>();
        internal static List<Node> NodesToRemove = new List<Node>();
        internal static List<CraftingNode> CraftingNodes = new List<CraftingNode>();
        internal static List<TabNode> TabNodes = new List<TabNode>();

        #endregion

        #region Patches

        internal static void Patch(HarmonyInstance harmony)
        {
            Type type = typeof(CraftTree);
            Type patcherClass = typeof(CraftTreePatcher);

            MethodInfo craftTreeGetTree = type.GetMethod("GetTree", BindingFlags.Static | BindingFlags.Public);
            MethodInfo craftTreeInitialize = type.GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);

            MethodInfo fabricatorScheme = type.GetMethod("FabricatorScheme", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo constructorScheme = type.GetMethod("ConstructorScheme", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo workbenchScheme = type.GetMethod("WorkbenchScheme", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo seamothUpgradesScheme = type.GetMethod("SeamothUpgradesScheme", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo mapRoomSheme = type.GetMethod("MapRoomSheme", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo cyclopsFabricatorScheme = type.GetMethod("CyclopsFabricatorScheme", BindingFlags.Static | BindingFlags.NonPublic);

            harmony.Patch(craftTreeGetTree,
                new HarmonyMethod(patcherClass.GetMethod("GetTreePreFix", BindingFlags.Static | BindingFlags.NonPublic)), null);

            harmony.Patch(craftTreeInitialize, null,
                new HarmonyMethod(patcherClass.GetMethod("InitializePostFix", BindingFlags.Static | BindingFlags.NonPublic)));

            harmony.Patch(fabricatorScheme, null,
                new HarmonyMethod(patcherClass.GetMethod("FabricatorSchemePostfix", BindingFlags.Static | BindingFlags.NonPublic)));

            harmony.Patch(constructorScheme, null,
                new HarmonyMethod(patcherClass.GetMethod("ConstructorSchemePostfix", BindingFlags.Static | BindingFlags.NonPublic)));

            harmony.Patch(workbenchScheme, null,
                new HarmonyMethod(patcherClass.GetMethod("WorkbenchSchemePostfix", BindingFlags.Static | BindingFlags.NonPublic)));

            harmony.Patch(seamothUpgradesScheme, null,
                new HarmonyMethod(patcherClass.GetMethod("SeamothUpgradesSchemePostfix", BindingFlags.Static | BindingFlags.NonPublic)));

            harmony.Patch(mapRoomSheme, null,
                new HarmonyMethod(patcherClass.GetMethod("MapRoomSchemePostfix", BindingFlags.Static | BindingFlags.NonPublic)));

            harmony.Patch(cyclopsFabricatorScheme, null,
                new HarmonyMethod(patcherClass.GetMethod("CyclopsFabricatorSchemePostfix", BindingFlags.Static | BindingFlags.NonPublic)));

            Logger.Log($"CraftTreePatcher is done.", LogLevel.Debug);
        }

        private static bool GetTreePreFix(CraftTree.Type treeType, ref CraftTree __result)
        {
            if (CustomTrees.ContainsKey(treeType))
            {
                __result = CustomTrees[treeType].CraftTree;
                return false;
            }

            return true;
        }

        private static void InitializePostFix()
        {
            var craftTreeInitialized = (bool)ReflectionHelper.GetStaticField<CraftTree>("initialized");
            Type craftTreeClass = typeof(CraftTree);
        
            if (craftTreeInitialized && !ModCraftTreeNode.Initialized)
            {
                foreach (CraftTree.Type cTreeKey in CustomTrees.Keys)
                {
                    CraftTree customTree = CustomTrees[cTreeKey].CraftTree;

                    MethodInfo addToCraftableTech = craftTreeClass.GetMethod("AddToCraftableTech", BindingFlags.Static | BindingFlags.NonPublic);

                    addToCraftableTech.Invoke(null, new[] { customTree });
                }
            }
        }

        private static void FabricatorSchemePostfix(ref CraftNode __result)
        {
            RemoveNodes(ref __result, NodesToRemove, CraftTree.Type.Fabricator);
            AddCustomTabs(ref __result, TabNodes, CraftTree.Type.Fabricator);
            PatchNodes(ref __result, CraftingNodes, CraftTree.Type.Fabricator);
        }

        private static void ConstructorSchemePostfix(ref CraftNode __result)
        {
            RemoveNodes(ref __result, NodesToRemove, CraftTree.Type.Constructor);
            AddCustomTabs(ref __result, TabNodes, CraftTree.Type.Constructor);
            PatchNodes(ref __result, CraftingNodes, CraftTree.Type.Constructor);
        }

        private static void WorkbenchSchemePostfix(ref CraftNode __result)
        {
            RemoveNodes(ref __result, NodesToRemove, CraftTree.Type.Workbench);
            AddCustomTabs(ref __result, TabNodes, CraftTree.Type.Workbench);
            PatchNodes(ref __result, CraftingNodes, CraftTree.Type.Workbench);
        }

        private static void SeamothUpgradesSchemePostfix(ref CraftNode __result)
        {
            RemoveNodes(ref __result, NodesToRemove, CraftTree.Type.SeamothUpgrades);
            AddCustomTabs(ref __result, TabNodes, CraftTree.Type.SeamothUpgrades);
            PatchNodes(ref __result, CraftingNodes, CraftTree.Type.SeamothUpgrades);
        }

        private static void MapRoomSchemePostfix(ref CraftNode __result)
        {
            RemoveNodes(ref __result, NodesToRemove, CraftTree.Type.MapRoom);
            AddCustomTabs(ref __result, TabNodes, CraftTree.Type.MapRoom);
            PatchNodes(ref __result, CraftingNodes, CraftTree.Type.MapRoom);
        }

        private static void CyclopsFabricatorSchemePostfix(ref CraftNode __result)
        {
            RemoveNodes(ref __result, NodesToRemove, CraftTree.Type.CyclopsFabricator);
            AddCustomTabs(ref __result, TabNodes, CraftTree.Type.CyclopsFabricator);
            PatchNodes(ref __result, CraftingNodes, CraftTree.Type.CyclopsFabricator);
        }

        #endregion

        #region Handling Nodes

        private static void AddCustomTabs(ref CraftNode nodes, List<TabNode> customTabs, CraftTree.Type scheme)
        {
            foreach (TabNode tab in customTabs)
            {
                // Wrong crafter, skip.
                if (tab.Scheme != scheme) continue;

                TreeNode currentNode = default;
                currentNode = nodes;

                // Patch into game's CraftTree.
                for (int i = 0; i < tab.Path.Length; i++)
                {
                    string currentPath = tab.Path[i];
                    Logger.Log("Tab Current Path: " + currentPath + " Tab: " + tab.Name + " Crafter: " + tab.Scheme.ToString(), LogLevel.Debug);

                    TreeNode node = currentNode[currentPath];

                    // Reached the end of the line.
                    if (node != null)
                        currentNode = node;
                    else
                        break;
                }

                // Add the new tab node.
                var newNode = new CraftNode(tab.Name, TreeAction.Expand, TechType.None);
                currentNode.AddNode(new TreeNode[]
                {
                    newNode
                });
            }
        }

        private static void PatchNodes(ref CraftNode nodes, List<CraftingNode> customNodes, CraftTree.Type scheme)
        {
            foreach (CraftingNode customNode in customNodes)
            {
                // Wrong crafter, just skip the node.
                if (customNode.Scheme != scheme) continue;

                // Have to do this to make sure C# shuts up.
                TreeNode node = default;
                node = nodes;

                // Loop through the path provided by the node.
                // Get the node for the last path.
                for (int i = 0; i < customNode.Path.Length; i++)
                {
                    string currentPath = customNode.Path[i];
                    TreeNode currentNode = node[currentPath];

                    if (currentNode != null) node = currentNode;
                    else break;
                }

                // Add the node.
                node.AddNode(new TreeNode[]
                {
                    new CraftNode(customNode.TechType.AsString(false), TreeAction.Craft, customNode.TechType)
                });
            }
        }

        private static void RemoveNodes(ref CraftNode nodes, List<Node> nodesToRemove, CraftTree.Type scheme)
        {
            // This method can be used to both remove single child nodes, thus removing one recipe from the tree.
            // Or it can remove entire tabs at once, removing the tab and all the recipes it contained in one go.

            foreach (Node nodeToRemove in nodesToRemove)
            {
                // Not for this fabricator. Skip.
                if (nodeToRemove.Scheme != scheme) continue;

                // Get the names of each node in the path to traverse tree until we reach the node we want.
                TreeNode currentNode = default;
                currentNode = nodes;

                // Travel the path down the tree.
                string currentPath = null;
                for (int step = 0; step < nodeToRemove.Path.Length; step++)
                {
                    currentPath = nodeToRemove.Path[step];
                    if (step > nodeToRemove.Path.Length)
                    {
                        break;
                    }

                    currentNode = currentNode[currentPath];
                }

                // Hold a reference to the parent node
                TreeNode parentNode = currentNode.parent;

                // Safty checks.
                if (currentNode != null && currentNode.id == currentPath)
                {
                    currentNode.Clear(); // Remove all child nodes (if any)
                    parentNode.RemoveNode(currentNode); // Remove the node
                }
            }
        }

        #endregion
    }
}
