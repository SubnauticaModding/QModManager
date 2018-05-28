namespace SMLHelper
{    
    public class CraftNodeToScrub
    {
        public CraftTree.Type Scheme;
        public string Path;

        public CraftNodeToScrub(CraftTree.Type scheme, string path)
        {
            Scheme = scheme;
            Path = path;
        }

        [System.Obsolete("CraftSchemes are obsolete. Use CraftTree.Types instead.")]
        public CraftNodeToScrub(CraftScheme scheme, string path)
            : this(Utility.CraftSchemeMap[scheme], path)
        {
        }
    }
}
