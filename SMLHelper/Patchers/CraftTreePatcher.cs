using System;
using Harmony;
using System.Collections.Generic;
using CraftTreePatcher2 = SMLHelper.V2.Patchers.CraftTreePatcher;

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
            customTabs.ForEach(x => CraftTreePatcher2.customTabs.Add(x.GetV2CraftTab()));
            customNodes.ForEach(x => CraftTreePatcher2.customNodes.Add(x.GetV2CraftNode()));
            nodesToRemove.ForEach(x => CraftTreePatcher2.nodesToRemove.Add(x.GetV2CraftNode()));
            customCraftNodes.ForEach(x => CraftTreePatcher2.customCraftNodes.Add(x));
            CustomTrees.ForEach(x => CraftTreePatcher2.CustomTrees.Add(x.Key, x.Value.GetV2RootNode()));

            V2.Logger.Log("Old CraftTreePatcher is done.");
        }
    }
}
