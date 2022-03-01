namespace SMLHelper.V2.Patchers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Crafting;
    using HarmonyLib;

    internal class CraftTreePatcher
    {
        #region Internal Fields

        internal static Dictionary<CraftTree.Type, ModCraftTreeRoot> CustomTrees = new Dictionary<CraftTree.Type, ModCraftTreeRoot>();
        internal static List<Node> NodesToRemove = new List<Node>();
        internal static List<CraftingNode> CraftingNodes = new List<CraftingNode>();
        internal static List<TabNode> TabNodes = new List<TabNode>();

        #endregion

        #region Patches

        internal static void Patch(Harmony harmony)
        {
            PatchUtils.PatchClass(harmony);

            Logger.Log($"CraftTreePatcher is done.", LogLevel.Debug);
        }

        [PatchUtils.Prefix]
        [HarmonyPatch(typeof(CraftTree), nameof(CraftTree.GetTree))]
        private static bool GetTreePreFix(CraftTree.Type treeType, ref CraftTree __result)
        {
            if (CustomTrees.ContainsKey(treeType))
            {
                __result = CustomTrees[treeType].CustomCraftingTree;
                return false;
            }

            return true;
        }

        [PatchUtils.Postfix]
        [HarmonyPatch(typeof(CraftTree), nameof(CraftTree.Initialize))]
        private static void InitializePostFix()
        {
            if (CraftTree.initialized && !ModCraftTreeNode.Initialized)
            {
                foreach (CraftTree.Type cTreeKey in CustomTrees.Keys)
                {
                    CraftTree customTree = CustomTrees[cTreeKey].CustomCraftingTree;

                    MethodInfo addToCraftableTech = AccessTools.Method(typeof(CraftTree), nameof(CraftTree.AddToCraftableTech));

                    addToCraftableTech.Invoke(null, new[] { customTree });
                }
            }
        }

        [PatchUtils.Postfix]
        [HarmonyPatch(typeof(CraftTree), nameof(CraftTree.FabricatorScheme))]
        private static void FabricatorSchemePostfix(ref CraftNode __result)
        {
            PatchCraftTree(ref __result, CraftTree.Type.Fabricator);
        }

        [PatchUtils.Postfix]
        [HarmonyPatch(typeof(CraftTree), nameof(CraftTree.ConstructorScheme))]
        private static void ConstructorSchemePostfix(ref CraftNode __result)
        {
            PatchCraftTree(ref __result, CraftTree.Type.Constructor);
        }

        [PatchUtils.Postfix]
        [HarmonyPatch(typeof(CraftTree), nameof(CraftTree.WorkbenchScheme))]
        private static void WorkbenchSchemePostfix(ref CraftNode __result)
        {
            PatchCraftTree(ref __result, CraftTree.Type.Workbench);
        }

        [PatchUtils.Postfix]
        [HarmonyPatch(typeof(CraftTree), nameof(CraftTree.SeamothUpgradesScheme))]
        private static void SeamothUpgradesSchemePostfix(ref CraftNode __result)
        {
            PatchCraftTree(ref __result, CraftTree.Type.SeamothUpgrades);
        }

        [PatchUtils.Postfix]
        [HarmonyPatch(typeof(CraftTree), nameof(CraftTree.MapRoomSheme))]
        private static void MapRoomSchemePostfix(ref CraftNode __result)
        {
            PatchCraftTree(ref __result, CraftTree.Type.MapRoom);
        }

        [PatchUtils.Postfix]
        [HarmonyPatch(typeof(CraftTree), nameof(CraftTree.CyclopsFabricatorScheme))]
        private static void CyclopsFabricatorSchemePostfix(ref CraftNode __result)
        {
            PatchCraftTree(ref __result, CraftTree.Type.CyclopsFabricator);
        }

#if BELOWZERO
        [PatchUtils.Postfix]
        [HarmonyPatch(typeof(CraftTree), nameof(CraftTree.SeaTruckFabricatorScheme))]
        private static void SeaTruckFabricatorSchemePostfix(ref CraftNode __result)
        {
            PatchCraftTree(ref __result, CraftTree.Type.SeaTruckFabricator);
        }
#endif

        #endregion

        #region Handling Nodes

        private static void PatchCraftTree(ref CraftNode __result, CraftTree.Type type)
        {
            RemoveNodes(ref __result, NodesToRemove, type);
            AddCustomTabs(ref __result, TabNodes, type);
            PatchNodes(ref __result, CraftingNodes, type);
        }

        private static void AddCustomTabs(ref CraftNode nodes, List<TabNode> customTabs, CraftTree.Type scheme)
        {
            foreach (TabNode tab in customTabs)
            {
                // Wrong crafter, skip.
                if (tab.Scheme != scheme)
                    continue;

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
                if (customNode.Scheme != scheme)
                    continue;

                // Have to do this to make sure C# shuts up.
                TreeNode node = default;
                node = nodes;

                // Loop through the path provided by the node.
                // Get the node for the last path.
                for (int i = 0; i < customNode.Path.Length; i++)
                {
                    string currentPath = customNode.Path[i];
                    TreeNode currentNode = node[currentPath];

                    if (currentNode != null)
                        node = currentNode;
                    else
                        break;
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
                if (nodeToRemove.Scheme != scheme)
                    continue;

                if (nodeToRemove.Path == null || nodeToRemove.Path.Length == 0)
                {
                    Logger.Warn($"An empty path in {nameof(RemoveNodes)} for '{scheme}' was skipped");
                    continue;
                }

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

                // Safty checks.
                if (currentNode != null && currentNode.id == currentPath)
                {
                    if (currentNode.parent == null)
                    {
                        Logger.Warn($"Skipped removing craft tree node in {nameof(RemoveNodes)} for '{scheme}'. Could not identify the parent node.");
                    }
                    else
                    {
                        currentNode.Clear(); // Remove all child nodes (if any)
                        currentNode.parent.RemoveNode(currentNode); // Remove the node from its parent
                    }
                }
            }
        }

        #endregion
    }
}
