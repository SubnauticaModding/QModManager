namespace QModManager.API.SMLHelper.Patchers
{
    using Crafting;
    using Harmony;
    using QModManager.Utility;
    using System.Collections.Generic;
    using System.Reflection;
    using Utility;

    internal static class CraftTreePatcher
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
            harmony.Patch(AccessTools.Method(typeof(CraftTree), "GetTree"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(CraftTreePatcher), "GetTreePreFix")));

            harmony.Patch(AccessTools.Method(typeof(CraftTree), "Initialize"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CraftTreePatcher), "InitializePostFix")));

            harmony.Patch(AccessTools.Method(typeof(CraftTree), "FabricatorScheme"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CraftTreePatcher), "FabricatorSchemePostfix")));

            harmony.Patch(AccessTools.Method(typeof(CraftTree), "ConstructorScheme"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CraftTreePatcher), "ConstructorSchemePostfix")));

            harmony.Patch(AccessTools.Method(typeof(CraftTree), "WorkbenchScheme"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CraftTreePatcher), "WorkbenchSchemePostfix")));

            harmony.Patch(AccessTools.Method(typeof(CraftTree), "SeamothUpgradesScheme"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CraftTreePatcher), "SeamothUpgradesSchemePostfix")));

            harmony.Patch(AccessTools.Method(typeof(CraftTree), "MapRoomSheme"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CraftTreePatcher), "MapRoomSchemePostfix")));

            harmony.Patch(AccessTools.Method(typeof(CraftTree), "CyclopsFabricatorScheme"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CraftTreePatcher), "CyclopsFabricatorSchemePostfix")));

            Logger.Debug($"CraftTreePatcher is done.");
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
            if (CraftTree.initialized && !ModCraftTreeNode.Initialized)
            {
                foreach (CraftTree.Type cTreeKey in CustomTrees.Keys)
                {
                    CraftTree customTree = CustomTrees[cTreeKey].CraftTree;

                    MethodInfo addToCraftableTech = AccessTools.Method(typeof(CraftTree), "AddToCraftableTech");

                    addToCraftableTech.Invoke(null, new[] { customTree });
                }
            }
        }

        private static void FabricatorSchemePostfix(ref CraftNode __result)
        {
            SchemePostfix(ref __result, CraftTree.Type.Fabricator);
        }

        private static void ConstructorSchemePostfix(ref CraftNode __result)
        {
            SchemePostfix(ref __result, CraftTree.Type.Constructor);
        }

        private static void WorkbenchSchemePostfix(ref CraftNode __result)
        {
            SchemePostfix(ref __result, CraftTree.Type.Workbench);
        }

        private static void SeamothUpgradesSchemePostfix(ref CraftNode __result)
        {
            SchemePostfix(ref __result, CraftTree.Type.SeamothUpgrades);
        }

        private static void MapRoomSchemePostfix(ref CraftNode __result)
        {
            SchemePostfix(ref __result, CraftTree.Type.MapRoom);
        }

        private static void CyclopsFabricatorSchemePostfix(ref CraftNode __result)
        {
            SchemePostfix(ref __result, CraftTree.Type.CyclopsFabricator);
        }

        private static void SchemePostfix(ref CraftNode __result, CraftTree.Type type)
        {
            RemoveNodes(ref __result, NodesToRemove, type);
            AddCustomTabs(ref __result, TabNodes, type);
            PatchNodes(ref __result, CraftingNodes, type);
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
                    Logger.Debug("Tab Current Path: " + currentPath + " Tab: " + tab.Name + " Crafter: " + tab.Scheme.ToString());

                    TreeNode node = currentNode[currentPath];

                    // Reached the end of the line.
                    if (node != null)
                        currentNode = node;
                    else
                        break;
                }

                // Add the new tab node.
                CraftNode newNode = new CraftNode(tab.Name, TreeAction.Expand, TechType.None);
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
