using System;
using Harmony;
using System.Collections.Generic;
using CraftTreePatcher2 = SMLHelper.V2.Patchers.CraftTreePatcher;
using CraftTreeHandler = SMLHelper.V2.Handlers.CraftTreeHandler;
using Crafting = SMLHelper.V2.Crafting;

namespace SMLHelper.Patchers
{

    [Obsolete("SMLHelper.Patchers.CraftTreePatcher is obsolete. Please use SMLHelper.V2 instead.")]
    public class CraftTreePatcher
    {
        [Obsolete("SMLHelper.Patchers.CraftTreePatcher.customTabs is obsolete. Please use SMLHelper.V2 instead.")]
        public static List<CustomCraftTab> customTabs = new List<CustomCraftTab>();

        [Obsolete("SMLHelper.Patchers.CraftTreePatcher.customNodes is obsolete. Please use SMLHelper.V2 instead.")]
        public static List<CustomCraftNode> customNodes = new List<CustomCraftNode>();

        [Obsolete("SMLHelper.Patchers.CraftTreePatcher.nodesToRemove is obsolete. Please use SMLHelper.V2 instead.")]
        public static List<CraftNodeToScrub> nodesToRemove = new List<CraftNodeToScrub>();

        internal static Dictionary<CraftTree.Type, CustomCraftTreeRoot> CustomTrees = new Dictionary<CraftTree.Type, CustomCraftTreeRoot>();

        [Obsolete("CraftTreePatcher.customCraftNodes is obsolete. Use CraftTreePatcher.customNodes", false)]
        public static Dictionary<string, TechType> customCraftNodes = new Dictionary<string, TechType>();

        internal static void Patch()
        {
            var nodes = new List<CustomCraftNode>();

            foreach (var customNode in customNodes)
                nodes.Add(customNode);

            foreach (var customNode in customCraftNodes)
                nodes.Add(new CustomCraftNode(customNode.Value, CraftTree.Type.Fabricator, customNode.Key));

            foreach(var tab in customTabs)
            {
                if (tab == null) continue;

                var path = tab.Path.SplitByChar('/');
                var tree = CraftTreeHandler.GetExistingTree(tab.Scheme);

                if (tree == null) continue;

                var tabNode = default(Crafting.CustomCraftTreeTab);

                foreach (var pathNode in path)
                {
                    SMLHelper.V2.Logger.Log("PathNod: " + pathNode);
                    V2.Logger.Log("Tab Node: " + ((tabNode != null) ? tabNode.Name : ""));

                    var newTabNode = tree.GetTabNode(pathNode);
                    if (newTabNode == null)
                    {
                        V2.Logger.Log("Adding TabNode");
                        tabNode.AddTabNode(pathNode, tab.Name, tab.Sprite.Sprite);
                        V2.Logger.Log("Added TabNode");
                    }
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

                var tabNode = default(Crafting.CustomCraftTreeTab);

                foreach(var pathNode in path)
                {
                    SMLHelper.V2.Logger.Log("PathNod: " + pathNode);
                    V2.Logger.Log("Tab Node: " + ((tabNode != null) ? tabNode.Name : ""));

                    var newTabNode = tree.GetTabNode(pathNode);

                    if (newTabNode == null)
                    {
                        V2.Logger.Log("Adding CraftNode");
                        tabNode.AddCraftingNode(node.TechType);
                        V2.Logger.Log("Added CraftNode");
                    }
                    else
                        tabNode = newTabNode;
                }
            }

            CustomTrees.ForEach(x => CraftTreePatcher2.CustomTrees.Add(x.Key, x.Value.GetV2RootNode()));

            V2.Logger.Log("Old CraftTreePatcher is done.");
        }
    }
}
