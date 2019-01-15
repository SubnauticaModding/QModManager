namespace SMLHelper.Patchers
{
    using System;
    using System.Collections.Generic;
    using CraftTreePatcher2 = SMLHelper.V2.Patchers.CraftTreePatcher;
    using V2.Handlers;
    using V2.Crafting;
    using System.Linq;

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
            customTabs.ForEach(x => CraftTreePatcher2.TabNodes.Add(new TabNode(x.Path.Split('/'), x.Scheme, x.Sprite.Sprite, "SMLHelper", System.IO.Path.GetFileName(x.Path), x.Name)));
            customNodes.ForEach(x => CraftTreePatcher2.CraftingNodes.Add(new CraftingNode(x.Path.Split('/').Take(x.Path.Split('/').Length - 1).ToArray(), x.Scheme, x.TechType)));
            customCraftNodes.ForEach(x => CraftTreePatcher2.CraftingNodes.Add(new CraftingNode(x.Key.Split('/').Take(x.Key.Split('/').Length - 1).ToArray(), CraftTree.Type.Fabricator, x.Value)));
            nodesToRemove.ForEach(x => CraftTreePatcher2.NodesToRemove.Add(new Node(x.Path.Split('/'), x.Scheme)));

            CustomTrees.ForEach(x => CraftTreePatcher2.CustomTrees.Add(x.Key, x.Value.GetV2RootNode()));

            V2.Logger.Log("Old CraftTreePatcher is done.");
        }
    }
}
