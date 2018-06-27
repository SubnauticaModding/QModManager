
namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System.Collections.Generic;
    using System.Reflection;
    using Util;
    using Crafting;

    internal class CraftTreePatcher
    {
        #region Internal Fields

        internal static Dictionary<CraftTree.Type, CustomCraftTreeRoot> CustomTrees = new Dictionary<CraftTree.Type, CustomCraftTreeRoot>();
        internal static CustomCraftTreeRoot FabricatorTree = LoadTree(CraftTree.Type.Fabricator);
        internal static CustomCraftTreeRoot CyclopsFabricatorTree = LoadTree(CraftTree.Type.CyclopsFabricator);
        internal static CustomCraftTreeRoot MapRoomTree = LoadTree(CraftTree.Type.MapRoom);
        internal static CustomCraftTreeRoot ConstructorTree = LoadTree(CraftTree.Type.Constructor);
        internal static CustomCraftTreeRoot RocketTree = LoadTree(CraftTree.Type.Rocket);
        internal static CustomCraftTreeRoot SeamothUpgradesTree = LoadTree(CraftTree.Type.SeamothUpgrades);
        internal static CustomCraftTreeRoot WorkbenchTree = LoadTree(CraftTree.Type.Workbench);

        internal static CustomCraftTreeRoot LoadTree(CraftTree.Type Scheme)
        {
            var treeRoot = new CustomCraftTreeRoot(Scheme, Scheme.ToString());
            var treeLinkingNode = (CustomCraftTreeLinkingNode)treeRoot;
            var tree = CraftTree.GetTree(Scheme);

            if (tree == null || treeRoot == null || treeLinkingNode == null) return null;

            CustomCraftTreeRoot.CreateFromExistingTree(tree.nodes, ref treeLinkingNode);

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
        
            if (craftTreeInitialized && !CustomCraftTreeNode.Initialized)
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
