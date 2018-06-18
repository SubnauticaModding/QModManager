using System;
using Harmony;
using System.Collections.Generic;
using System.Reflection;

namespace SMLHelper.V2.Patchers
{
    public class CraftTreePatcher
    {
        internal static Dictionary<CraftTree.Type, Crafting.CustomCraftTreeRoot> CustomTrees = new Dictionary<CraftTree.Type, Crafting.CustomCraftTreeRoot>();

        internal static Crafting.CustomCraftTreeRoot FabricatorTree;
        internal static Crafting.CustomCraftTreeRoot CyclopsFabricatorTree;
        internal static Crafting.CustomCraftTreeRoot MapRoomTree;
        internal static Crafting.CustomCraftTreeRoot ConstructorTree;
        internal static Crafting.CustomCraftTreeRoot RocketTree;
        internal static Crafting.CustomCraftTreeRoot SeamothUpgradesTree;
        internal static Crafting.CustomCraftTreeRoot WorkbenchTree;
        
        static CraftTreePatcher()
        {
            LoadTrees();
        }

        internal static void LoadTrees()
        {
            FabricatorTree = LoadTree(CraftTree.Type.Fabricator);
            CyclopsFabricatorTree = LoadTree(CraftTree.Type.CyclopsFabricator);
            MapRoomTree = LoadTree(CraftTree.Type.MapRoom);
            ConstructorTree = LoadTree(CraftTree.Type.Constructor);
            RocketTree = LoadTree(CraftTree.Type.Rocket);
            SeamothUpgradesTree = LoadTree(CraftTree.Type.SeamothUpgrades);
            WorkbenchTree = LoadTree(CraftTree.Type.Workbench);
        }

        private static Crafting.CustomCraftTreeRoot LoadTree(CraftTree.Type Scheme)
        {
            var treeRoot = new Crafting.CustomCraftTreeRoot(Scheme, Scheme.ToString());
            var treeLinkingNode = (Crafting.CustomCraftTreeLinkingNode)treeRoot;
            var tree = CraftTree.GetTree(Scheme);

            if (tree == null || treeRoot == null || treeLinkingNode == null) return null;

            Crafting.CustomCraftTreeRoot.CreateFromExistingTree(tree.nodes, ref treeLinkingNode);

            return treeRoot;
        }

        #region Patches

        internal static void Patch(HarmonyInstance harmony)
        {
            var type = typeof(CraftTree);
            var patcherClass = typeof(CraftTreePatcher);

            var craftTreeGetTree = type.GetMethod("GetTree", BindingFlags.Static | BindingFlags.Public);
            var craftTreeInitialize = type.GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);

            harmony.Patch(craftTreeGetTree,
                new HarmonyMethod(patcherClass.GetMethod("GetTreePreFix", BindingFlags.Static | BindingFlags.NonPublic)), null);

            harmony.Patch(craftTreeInitialize, null,
                new HarmonyMethod(patcherClass.GetMethod("InitializePostFix", BindingFlags.Static | BindingFlags.NonPublic)));

            Logger.Log($"CraftTreePatcher is done.");
        }

        private static bool GetTreePreFix(CraftTree.Type treeType, ref CraftTree __result)
        {
            switch (treeType)
            {
                case CraftTree.Type.Fabricator:
                    __result = FabricatorTree.CraftTree;
                    return false;

                case CraftTree.Type.SeamothUpgrades:
                    __result = SeamothUpgradesTree.CraftTree;
                    return false;

                case CraftTree.Type.CyclopsFabricator:
                    __result = CyclopsFabricatorTree.CraftTree;
                    return false;

                case CraftTree.Type.MapRoom:
                    __result = MapRoomTree.CraftTree;
                    return false;

                case CraftTree.Type.Rocket:
                    __result = RocketTree.CraftTree;
                    return false;

                case CraftTree.Type.Workbench:
                    __result = WorkbenchTree.CraftTree;
                    return false;

                case CraftTree.Type.Constructor:
                    __result = ConstructorTree.CraftTree;
                    return false;
            }

            if (CustomTrees.ContainsKey(treeType))
            {
                __result = CustomTrees[treeType].CraftTree;
                return false;
            }

            return true;
        }

        private static void InitializePostFix()
        {
            var craftTreeInitialized = false;
            var craftTreeClass = typeof(CraftTree);

            craftTreeClass.GetField("initialized", BindingFlags.Static | BindingFlags.NonPublic).GetValue(craftTreeInitialized);

            if (craftTreeInitialized && !Crafting.CustomCraftTreeNode.Initialized)
            {
                foreach (CraftTree.Type cTreeKey in CustomTrees.Keys)
                {
                    CraftTree customTree = CustomTrees[cTreeKey].CraftTree;

                    MethodInfo addToCraftableTech = craftTreeClass.GetMethod("AddToCraftableTech", BindingFlags.Static | BindingFlags.NonPublic);

                    addToCraftableTech.Invoke(null, new[] { customTree });
                }
            }
        }

        #endregion
    }
}
