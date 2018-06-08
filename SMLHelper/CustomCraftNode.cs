using SMLHelper.Patchers;
using SMLHelper.Util;

namespace SMLHelper
{
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
    }
}
