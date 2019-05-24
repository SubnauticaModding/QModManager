using SMLHelper.V2.Patchers;
using SMLHelper.V2.Utility;

namespace SMLHelper
{
    [System.Obsolete("Use SMLHelper.V2 instead.")]
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

        [System.Obsolete("Use CraftTree.Type instead.")]
        public CustomCraftNode(TechType techType, CraftScheme scheme, string path)
            : this(techType, Utility.CraftSchemeMap[scheme], path)
        {
        }
    }
}
