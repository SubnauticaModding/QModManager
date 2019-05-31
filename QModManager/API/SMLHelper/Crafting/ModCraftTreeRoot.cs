namespace QModManager.API.SMLHelper.Crafting
{
    using System.Linq;
    using Patchers;
    using UnityEngine.Assertions;

    /// <summary>
    /// The root node of a CraftTree. The whole tree starts here.
    /// </summary>    
    /// <seealso cref="ModCraftTreeLinkingNode" />
    public class ModCraftTreeRoot : ModCraftTreeLinkingNode
    {
        private readonly string _schemeAsString;
        private readonly CraftTree.Type _scheme;

        internal override string SchemeAsString => _schemeAsString;
        internal override CraftTree.Type Scheme => _scheme;

        internal ModCraftTreeRoot(CraftTree.Type scheme, string schemeAsString)
            : base("Root", TreeAction.None, TechType.None)
        {
            Assert.IsTrue((int)scheme > CraftTreeTypePatcher.startingIndex, "Custom CraftTree types must have an index higher than the in-game types.");

            _schemeAsString = schemeAsString;
            _scheme = scheme;
            HasCustomTrees = true;
        }

        /// <summary>
        /// Dynamically creates the CraftTree object for this crafting tree.
        /// The CraftNode objects were created and linked as the classes of the ModCraftTreeFamily were created and linked.
        /// </summary>
        internal CraftTree CraftTree => new CraftTree(_schemeAsString, CraftNode);

        /// <summary>
        /// Populates a new ModCraftTreeRoot from a CraftNode tree.
        /// </summary>
        /// <param name="tree">The tree to create the ModCraftTreeRoot from.</param>
        /// <param name="root"></param>
        internal static void CreateFromExistingTree(CraftNode tree, ref ModCraftTreeLinkingNode root)
        {
            foreach (CraftNode node in tree)
            {
                if (node.action == TreeAction.Expand)
                {
                    ModCraftTreeTab tab = root.AddTabNode(node.id);
                    ModCraftTreeLinkingNode thing = (ModCraftTreeLinkingNode)tab;
                    CreateFromExistingTree(node, ref thing);
                }

                if (node.action == TreeAction.Craft)
                {
                    TechType techType = TechType.None;
                    TechTypeExtensions.FromString(node.id, out techType, false);

                    if (node.id == "SeamothHullModule2") techType = TechType.VehicleHullModule2;
                    else if (node.id == "SeamothHullModule3") techType = TechType.VehicleHullModule3;

                    root.AddCraftingNode(techType);
                }
            }
        }

        /// <summary>
        /// Gets the tab node at the specified path from the root.
        /// </summary>
        /// <param name="stepsToTab">
        /// <para>The steps to the target tab.</para>
        /// <para>These must match the id value of the CraftNode in the crafting tree you're targeting.</para>
        /// <para>Do not include "root" in this path.</para>
        /// </param>
        /// <returns>If the specified tab node is found, returns that <see cref="ModCraftTreeTab"/>; Otherwise, returns null.</returns>
        public ModCraftTreeTab GetTabNode(params string[] stepsToTab)
        {
            ModCraftTreeTab tab = base.GetTabNode(stepsToTab[0]);

            for (int i = 1; i < stepsToTab.Length && tab != null; i++)
            {
                tab = tab.GetTabNode(stepsToTab[i]);
            }

            return tab;
        }

        /// <summary>
        /// Gets the node at the specified path from the root.
        /// </summary>
        /// <param name="stepsToNode">
        /// <para>The steps to the target tab.</para>
        /// <para>These must match the id value of the CraftNode in the crafting tree you're targeting.</para>
        /// <para>Do not include "root" in this path.</para>
        /// </param>
        /// <returns>If the specified tab node is found, returns that <see cref="ModCraftTreeNode"/>; Otherwise, returns null.</returns>
        public ModCraftTreeNode GetNode(params string[] stepsToNode)
        {
            if (stepsToNode.Length == 1)
            {
                return base.GetNode(stepsToNode[0]);
            }

            string nodeID = stepsToNode[stepsToNode.Length - 1];
            string[] stepsToTab = stepsToNode.Take(stepsToNode.Length - 1).ToArray();
            ModCraftTreeTab tab = GetTabNode(stepsToTab);

            if (tab == null) return null;

            return tab.GetNode(nodeID);
        }
    }
}
