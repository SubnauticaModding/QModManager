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
            // Old custom tabs added through new CustomCraftTreeNode classes
            foreach (var tab in customTabs)
            {
                if (tab == null) continue;

                var root = CraftTreeHandler.GetExistingTree(tab.Scheme);

                if (root == null) continue;

                if (!tab.Path.Contains("/")) // Added to the root
                {
                    root.AddTabNode(tab.Path, tab.Name, tab.Sprite.Sprite);
                }
                else // Added under an existing tab
                {
                    var path = tab.Path.SplitByChar('/');
                    CustomCraftTreeTab node = root.GetTabNode(path[0]);

                    for (int i = 1; i < path.Length - 1; i++)
                    {
                        node = node.GetTabNode(path[i]);
                    }

                    var tabName = path[path.Length - 1]; // Last
                    node.AddTabNode(tabName, tab.Name, tab.Sprite.Sprite);
                }
            }

            // Old custom craft nodes added through new CustomCraftTreeNode classes

            var craftNodes = new List<CustomCraftNode>();

            foreach (var customNode in customNodes)
                craftNodes.Add(new CustomCraftNode(customNode.TechType, customNode.Scheme, customNode.Path));

            foreach (var customNode in customCraftNodes)
                craftNodes.Add(new CustomCraftNode(customNode.Value, CraftTree.Type.Fabricator, customNode.Key));

            foreach (var craftNode in craftNodes)
            {
                if (craftNode == null) continue;

                var root = CraftTreeHandler.GetExistingTree(craftNode.Scheme);

                if (root == null) continue;

                if (!craftNode.Path.Contains("/")) // Added to the root
                {
                    root.AddCraftingNode(craftNode.TechType);
                }
                else // Added under an existing tab
                {
                    var path = craftNode.Path.SplitByChar('/');
                    CustomCraftTreeTab node = root.GetTabNode(path[0]);

                    for (int i = 1; i < path.Length - 1; i++)
                    {
                        node = node.GetTabNode(path[i]);
                    }

                    var tabName = path[path.Length - 1]; // Last
                    var techType = TechType.None;
                    if(TechTypeExtensions.FromString(tabName, out techType, false))
                    {
                        node.AddCraftingNode(techType);
                    }
                    else
                    {
                        node.AddModdedCraftingNode(tabName);
                    }
                }
            }

            // Old node scrubbing handled through new CustomCraftTreeNode classes
            foreach (var scrubNode in nodesToRemove)
            {
                if (scrubNode == null) continue;

                var root = CraftTreeHandler.GetExistingTree(scrubNode.Scheme);

                if (root == null) continue;

                if (!scrubNode.Path.Contains("/")) // Removed from the root
                {
                    root.GetNode(scrubNode.Path).RemoveNode();
                }
                else // Removed from an existing tab
                {
                    var path = scrubNode.Path.SplitByChar('/');
                    CustomCraftTreeTab node = root.GetTabNode(path[0]);

                    for (int i = 1; i < path.Length - 1; i++)
                    {
                        node = node.GetTabNode(path[i]);
                    }

                    var tabName = path[path.Length - 1]; // Last
                    node.RemoveNode();
                }
            }

            CustomTrees.ForEach(x => CraftTreePatcher2.CustomTrees.Add(x.Key, x.Value.GetV2RootNode()));

            V2.Logger.Log("Old CraftTreePatcher is done.");
        }
    }
}
