namespace SMLHelper.V2.Crafting
{
    using System;
    using Patchers;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Patchers.EnumPatching;
    using UnityEngine.Assertions;

    /// <summary>
    /// The root node of a CraftTree. The whole tree starts here.<para/>
    /// Build up your custom crafting tree from this root node using the AddCraftingNode and AddTabNode methods.<br/>
    /// This tree will be automatically patched into the game. No further calls into <see cref="CraftTreeHandler"/> required.<para/>
    /// For more advanced usage, you can replace the default value of <see cref="CraftTreeCreation"/> with your own custom function.        
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

            CraftTreeCreation = () => new CraftTree(_schemeAsString, CraftNode);
        }

        /// <summary>
        /// Dynamically creates the CraftTree object for this crafting tree.
        /// The CraftNode objects were created and linked as the classes of the ModCraftTreeFamily were created and linked.
        /// </summary>
        internal CraftTree CustomCraftingTree => CraftTreeCreation.Invoke();

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
                    var thing = (ModCraftTreeLinkingNode)tab;
                    CreateFromExistingTree(node, ref thing);
                }

                if (node.action == TreeAction.Craft)
                {
                    TechTypeExtensions.FromString(node.id, out TechType techType, false);

                    root.AddCraftingNode(techType);
                }
            }
        }

        /// <summary>
        /// The craft tree creation function.<br/>
        /// Default implementaion returns a new <see cref="CraftTree"/> instantiated with <see cref="SchemeAsString"/> and the root <see cref="CraftNode"/>.<para/>
        /// You can replace this function with your own to have more control of the crafting tree when it is being created.
        /// </summary>
        public Func<CraftTree> CraftTreeCreation;

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

            int stepCountToTab = stepsToNode.Length - 1;

            string nodeID = stepsToNode[stepCountToTab];
            string[] stepsToTab = new string[stepCountToTab];
            Array.Copy(stepsToNode, stepsToTab, stepCountToTab);

            ModCraftTreeTab tab = GetTabNode(stepsToTab);

            return tab?.GetNode(nodeID);
        }
    }
}
