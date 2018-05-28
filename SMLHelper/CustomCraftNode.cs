namespace SMLHelper
{
    public class CustomCraftNode
    {
        public TechType TechType;
        public CraftTree.Type Scheme;
        public string Path;

        public CustomCraftNode(TechType techType, CraftTree.Type scheme, string path)
        {
            TechType = techType;
            Scheme = scheme;
            Path = path;
        }

        [System.Obsolete("CraftSchemes are obsolete. Use CraftTree.Types instead.")]
        public CustomCraftNode(TechType techType, CraftScheme scheme, string path)
            : this(techType, Utility.CraftSchemeMap[scheme], path)
        {
        }
    }
}
