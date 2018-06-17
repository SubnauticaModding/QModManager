using CraftTreeTypePatcher2 = SMLHelper.V2.Patchers.CraftTreeTypePatcher;
using CraftTreePatcher2 = SMLHelper.V2.Patchers.CraftTreePatcher;
using CustomCraftTreeRoot2 = SMLHelper.V2.CustomCraftTreeRoot;

namespace SMLHelper.Patchers
{
    [System.Obsolete("SMLHelper.CraftTreeTypePatcher is obsolete. Please use SMLHelper.V2 instead.")]
    public class CraftTreeTypePatcher
    {
        [System.Obsolete("SMLHelper.CraftTreeTypePatcher.CreateCustomCraftTreeAndType is obsolete. Please use SMLHelper.V2 instead.")]
        public static CustomCraftTreeRoot CreateCustomCraftTreeAndType(string name, out CraftTree.Type craftTreeType)
        {
            var customCraftTreeRoot2 = CraftTreeTypePatcher2.CreateCustomCraftTreeAndType(name, out craftTreeType);
            CraftTreePatcher2.CustomTrees.Remove(craftTreeType);

            var customCraftTreeRoot = new CustomCraftTreeRoot(craftTreeType, name);

            CraftTreePatcher.CustomTrees.Add(craftTreeType, customCraftTreeRoot);

            return customCraftTreeRoot;
        }

    }
}
