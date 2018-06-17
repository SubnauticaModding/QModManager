using SMLHelper.V2.Patchers;
using SMLHelper.V2.Util;
using CustomCraftNode2 = SMLHelper.V2.Crafting.CustomCraftNode;

namespace SMLHelper
{
    [System.Obsolete("SMLHelper.CustomCraftNode is obsolete. Please use SMLHelper.V2 instead.")]
    public class CustomCraftNode
    {
        public TechType TechType;
        public CraftTree.Type Scheme;
        public string Path;
        internal readonly bool ItemExists;

        public CustomCraftNode(TechType techType, CraftTree.Type scheme, string path)
        {
            TechType = techType;
            Scheme = scheme;
            Path = path;
            ItemExists = true;
        }

        public CustomCraftNode(string moddedTechTypeName, CraftTree.Type scheme, string path)
        {            
            Scheme = scheme;
            Path = path;

            EnumTypeCache cache = TechTypePatcher.cacheManager.GetCacheForTypeName(moddedTechTypeName);

            ItemExists = cache != null;

            if (ItemExists)
            {
                TechType = (TechType)cache.Index;
            }
        }

        [System.Obsolete("CraftSchemes are obsolete. Use CraftTree.Types instead.")]
        public CustomCraftNode(TechType techType, CraftScheme scheme, string path)
            : this(techType, Utility.CraftSchemeMap[scheme], path)
        {
        }

        public CustomCraftNode2 GetV2CraftNode()
        {
            var customNode = new CustomCraftNode2(TechType, Scheme, Path);
            return customNode;
        }
    }
}
