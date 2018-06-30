
namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System.Collections.Generic;
    using System.Reflection;
    using Utility;
    using Crafting;

    internal class CraftTreePatcher
    {
        #region Internal Fields

        internal static Dictionary<CraftTree.Type, ModCraftTreeRoot> CustomTrees = new Dictionary<CraftTree.Type, ModCraftTreeRoot>();
        internal static ModCraftTreeRoot FabricatorTree = LoadTree(CraftTree.Type.Fabricator);
        internal static ModCraftTreeRoot CyclopsFabricatorTree = LoadTree(CraftTree.Type.CyclopsFabricator);
        internal static ModCraftTreeRoot MapRoomTree = LoadTree(CraftTree.Type.MapRoom);
        internal static ModCraftTreeRoot ConstructorTree = LoadTree(CraftTree.Type.Constructor);
        internal static ModCraftTreeRoot RocketTree = LoadTree(CraftTree.Type.Rocket);
        internal static ModCraftTreeRoot SeamothUpgradesTree = LoadTree(CraftTree.Type.SeamothUpgrades);
        internal static ModCraftTreeRoot WorkbenchTree = LoadTree(CraftTree.Type.Workbench);

        internal static ModCraftTreeRoot LoadTree(CraftTree.Type Scheme)
        {
            var treeRoot = new ModCraftTreeRoot(Scheme, Scheme.ToString());
            var treeLinkingNode = (ModCraftTreeLinkingNode)treeRoot;
            var tree = CraftTree.GetTree(Scheme);

            if (tree == null || treeRoot == null || treeLinkingNode == null) return null;

            ModCraftTreeRoot.CreateFromExistingTree(tree.nodes, ref treeLinkingNode);

            return treeRoot;
        }

        #endregion

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
            var craftTreeInitialized = (bool)ReflectionHelper.GetStaticField<CraftTree>("initialized");
            var craftTreeClass = typeof(CraftTree);
        
            if (craftTreeInitialized && !ModCraftTreeNode.Initialized)
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
