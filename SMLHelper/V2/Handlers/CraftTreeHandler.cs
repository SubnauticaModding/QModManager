using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLHelper.V2.Patchers;
using SMLHelper.V2.Crafting;

namespace SMLHelper.V2.Handlers
{
    public class CraftTreeHandler
    {
        /// <summary>
        /// Your first method call to start a new custom crafting tree.
        /// Creating a new CraftTree only makes sense if you're going to use it in a new type of GhostCrafter/Fabricator.
        /// </summary>
        /// <param name="name">The name for the new <see cref="CraftTree.Type"/> enum.</param>
        /// <param name="craftTreeType">The new enum instance for your custom craft tree.</param>
        /// <returns>A new root node for your custom craft tree.</returns>
        /// <remarks>This node is automatically assigned to <see cref="CraftTreePatcher.CustomTrees"/>.</remarks>
        public static Crafting.CustomCraftTreeRoot CreateCustomCraftTreeAndType(string name, out CraftTree.Type craftTreeType)
        {
            return CraftTreeTypePatcher.CreateCustomCraftTreeAndType(name, out craftTreeType);
        }

        public static Crafting.CustomCraftTreeRoot GetExistingTree(CraftTree.Type Scheme)
        {
            switch(Scheme)
            {
                case CraftTree.Type.Fabricator:
                    return CraftTreePatcher.FabricatorTree;

                case CraftTree.Type.CyclopsFabricator:
                    return CraftTreePatcher.CyclopsFabricatorTree;

                case CraftTree.Type.MapRoom:
                    return CraftTreePatcher.MapRoomTree;

                case CraftTree.Type.Rocket:
                    return CraftTreePatcher.RocketTree;

                case CraftTree.Type.Workbench:
                    return CraftTreePatcher.WorkbenchTree;

                case CraftTree.Type.Constructor:
                    return CraftTreePatcher.ConstructorTree;
            }

            return null;
        }

        /// <summary>
        /// Adds a new tab in the specified crafter.
        /// </summary>
        /// <param name="path">The path to the new tab.</param>
        /// <param name="name">The name of the new tab.</param>
        /// <param name="scheme">The crafter to add a new tab to.</param>
        /// <param name="sprite">The sprite for the tab.</param>
        public static void AddCustomTab(string path, string name, CraftTree.Type scheme, Atlas.Sprite sprite)
        {
            CraftTreePatcher.CustomTabs.Add(new Crafting.CustomCraftTab(path, name, scheme, sprite));
        }

        /// <summary>
        /// Adds a new tab in the specified crafter.
        /// </summary>
        /// <param name="path">The path to the new tab.</param>
        /// <param name="name">The name of the new tab.</param>
        /// <param name="scheme">The crafter to add a new tab to.</param>
        /// <param name="sprite">The sprite for the tab.</param>
        public static void AddCustomTab(string path, string name, CraftTree.Type scheme, UnityEngine.Sprite sprite)
        {
            CraftTreePatcher.CustomTabs.Add(new Crafting.CustomCraftTab(path, name, scheme, sprite));
        }

        /// <summary>
        /// Adds a new tab in the specified crafter.
        /// </summary>
        /// <param name="tab">The tab to add.</param>
        public static void AddCustomTab(Crafting.CustomCraftTab tab)
        {
            CraftTreePatcher.CustomTabs.Add(tab);
        }

        /// <summary>
        /// Adds a new node in the specified crafter.
        /// </summary>
        /// <param name="techType">The TechType to craft in the node.</param>
        /// <param name="scheme">The crafter to add the node to.</param>
        /// <param name="path">The path to the node. Example: Resources/BasicMaterials. You can get the paths from dnSpy -> CraftTree.cs</param>
        public static void AddCustomNode(TechType techType, CraftTree.Type scheme, string path) 
        {
            CraftTreePatcher.CustomNodes.Add(new Crafting.CustomCraftNode(techType, scheme, path));
        }

        /// <summary>
        /// Adds a new node in the specified crafter.
        /// </summary>
        /// <param name="moddedTechType">The modded TechType to craft in the node.</param>
        /// <param name="scheme">The crafter to add the node to.</param>
        /// <param name="path">The path to the node. Example: Resources/BasicMaterials. You can get the paths from dnSpy -> CraftTree.cs</param>
        public static void AddCustomNode(string moddedTechType, CraftTree.Type scheme, string path)
        {
            CraftTreePatcher.CustomNodes.Add(new Crafting.CustomCraftNode(moddedTechType, scheme, path));
        }

        /// <summary>
        /// Adds a new node in the specified crafter.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public static void AddCustomNode(Crafting.CustomCraftNode node)
        {
            CraftTreePatcher.CustomNodes.Add(node);
        }
    }
}
