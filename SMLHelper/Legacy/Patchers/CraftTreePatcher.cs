namespace SMLHelper.Patchers
{
    using System;
    using System.Collections.Generic;
    using CraftTreePatcher2 = SMLHelper.V2.Patchers.CraftTreePatcher;
    using V2.Handlers;
    using V2.Crafting;

    [Obsolete("Use SMLHelper.V2 instead.")]
    public class CraftTreePatcher
    {
        [Obsolete("Use SMLHelper.V2 instead.")]
        public static List<CustomCraftTab> customTabs = new List<CustomCraftTab>();

        [Obsolete("Use SMLHelper.V2 instead.")]
        public static List<CustomCraftNode> customNodes = new List<CustomCraftNode>();

        [Obsolete("Use SMLHelper.V2 instead.")]
        public static List<CraftNodeToScrub> nodesToRemove = new List<CraftNodeToScrub>();

        internal static Dictionary<CraftTree.Type, SMLHelper.CustomCraftTreeRoot> CustomTrees = new Dictionary<CraftTree.Type, SMLHelper.CustomCraftTreeRoot>();

        [Obsolete("Use SMLHelper.V2 instead.")]
        public static Dictionary<string, TechType> customCraftNodes = new Dictionary<string, TechType>();

        internal static void Patch()
        {
            var nodes = new List<CustomCraftNode>();

            foreach (var customNode in customNodes)
                nodes.Add(new CustomCraftNode(customNode.TechType, customNode.Scheme, customNode.Path));

            foreach (var customNode in customCraftNodes)
                nodes.Add(new CustomCraftNode(customNode.Value, CraftTree.Type.Fabricator, customNode.Key));

            foreach(var tab in customTabs)
            {
                if (tab == null) continue;

                var path = tab.Path.SplitByChar('/');
                var tree = CraftTreeHandler.GetExistingTree(tab.Scheme);

                if (tree == null) continue;

                var tabNode = default(CustomCraftTreeTab);

                foreach (var pathNode in path)
                {
                    var newTabNode = default(CustomCraftTreeTab);

                    if (tabNode == null)
                        newTabNode = tree.GetTabNode(pathNode);
                    else
                        newTabNode = tabNode.GetTabNode(pathNode);

                    if (newTabNode == null)
                        tabNode.AddTabNode(pathNode, tab.Name, tab.Sprite.Sprite);
                    else
                        tabNode = newTabNode;
                }
            }

            foreach(var node in nodes)
            {
                if (node == null) continue;

                var path = node.Path.SplitByChar('/');
                var tree = CraftTreeHandler.GetExistingTree(node.Scheme);

                if (tree == null) continue;

                var tabNode = default(CustomCraftTreeTab);

                foreach(var pathNode in path)
                {
                    var newTabNode = default(CustomCraftTreeTab);

                    if (tabNode == null)
                        newTabNode = tree.GetTabNode(pathNode);
                    else
                        newTabNode = tabNode.GetTabNode(pathNode);

                    if (newTabNode == null && tabNode != null)
                    {
                        var techType = TechType.None;
                        var craftNode = default(CustomCraftTreeCraft);

                        if(TechTypeExtensions.FromString(pathNode, out techType, false))
                        {
                            craftNode = tabNode.AddCraftingNode(techType);
                        }
                        else
                        {
                            craftNode = tabNode.AddModdedCraftingNode(pathNode);
                        }
                    }
                    else
                    {
                        tabNode = newTabNode;
                    }
                }
            }

            foreach (var node in nodesToRemove)
            {
                if (node == null) continue;

                var path = node.Path.SplitByChar('/');
                var tree = CraftTreeHandler.GetExistingTree(node.Scheme);

                if (tree == null) continue;

                var treeNode = default(CustomCraftTreeNode);

                foreach (var pathNode in path)
                {
                    var newTreeNode = tree.GetNode(pathNode);

                    if(newTreeNode == null)
                    {
                        treeNode.RemoveNode();
                    }
                    else
                    {
                        treeNode = newTreeNode;
                    }
                }
            }

            CustomTrees.ForEach(x => CraftTreePatcher2.CustomTrees.Add(x.Key, x.Value.GetV2RootNode()));

            V2.Logger.Log("Old CraftTreePatcher is done.");
        }
    }
}
