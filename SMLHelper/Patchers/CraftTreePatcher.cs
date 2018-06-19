

namespace SMLHelper.Patchers
{
    using System;
    using Harmony;
    using System.Collections.Generic;
    using CraftTreePatcher2 = SMLHelper.V2.Patchers.CraftTreePatcher;
    using V2.Handlers;
    using V2.Crafting;

    [Obsolete("SMLHelper.Patchers.CraftTreePatcher is obsolete. Please use SMLHelper.V2 instead.")]
    public class CraftTreePatcher
    {
        [Obsolete("SMLHelper.Patchers.CraftTreePatcher.customTabs is obsolete. Please use SMLHelper.V2 instead.")]
        public static List<SMLHelper.CustomCraftTab> customTabs = new List<SMLHelper.CustomCraftTab>();

        [Obsolete("SMLHelper.Patchers.CraftTreePatcher.customNodes is obsolete. Please use SMLHelper.V2 instead.")]
        public static List<SMLHelper.CustomCraftNode> customNodes = new List<SMLHelper.CustomCraftNode>();

        [Obsolete("SMLHelper.Patchers.CraftTreePatcher.nodesToRemove is obsolete. Please use SMLHelper.V2 instead.")]
        public static List<SMLHelper.CraftNodeToScrub> nodesToRemove = new List<SMLHelper.CraftNodeToScrub>();

        internal static Dictionary<CraftTree.Type, SMLHelper.CustomCraftTreeRoot> CustomTrees = new Dictionary<CraftTree.Type, SMLHelper.CustomCraftTreeRoot>();

        [Obsolete("CraftTreePatcher.customCraftNodes is obsolete. Use CraftTreePatcher.customNodes", false)]
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
                    var newTabNode = tree.GetTabNode(pathNode);
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
                    var newTabNode = tree.GetTabNode(pathNode);
                    if (newTabNode == null)
                    {
                        var techType = TechType.None;

                        if(TechTypeExtensions.FromString(pathNode, out techType, false))
                        {
                            tabNode.AddCraftingNode(techType);
                        }
                        else
                        {
                            tabNode.AddModdedCraftingNode(pathNode);
                        }
                    }
                    else
                    {
                        tabNode = newTabNode;
                    }
                }
            }

            foreach(var node in nodesToRemove)
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
