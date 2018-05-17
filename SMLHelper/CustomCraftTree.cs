using System.Collections.Generic;
using SMLHelper.Patchers;
using UnityEngine.Assertions;

namespace SMLHelper
{
    /// <summary>
    /// Creating a new CraftTree only makes sense if you're going to use it in a new type of GhostCrafter
    /// </summary>
    public class CustomCraftTree
    {
        public readonly CraftTree.Type Scheme;
        public readonly CraftNode Nodes;
        public readonly string Name;

        public readonly CraftTree CustomTree;

        public CustomCraftTree(CraftTree.Type scheme, CraftNode nodes, string name = null)
        {
            Assert.IsTrue((int)scheme > CraftTreeTypePatcher.StartingIndex, "Custom CraftTree types must have an index higher than the in-game types.");

            Scheme = scheme;
            Nodes = nodes;
            Name = name ?? scheme.ToString();
            CustomTree = new CraftTree(Name, Nodes);

            CustomTrees.Add(scheme, CustomTree);
            HasCustomTrees = true;
        }

        internal static Dictionary<CraftTree.Type, CraftTree> CustomTrees = new Dictionary<CraftTree.Type, CraftTree>();

        internal static bool Initialized = false;

        internal static bool HasCustomTrees { get; private set; } = false;
    }
}
