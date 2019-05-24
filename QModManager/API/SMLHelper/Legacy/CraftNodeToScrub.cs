namespace SMLHelper
{    
    [System.Obsolete("Use SMLHelper.V2 instead.")]
    public class CraftNodeToScrub
    {
        public CraftTree.Type Scheme;
        public string Path;

        public CraftNodeToScrub(CraftTree.Type scheme, string path)
        {
            Scheme = scheme;
            Path = path;
        }

        [System.Obsolete("Use CraftTree.Type instead.")]
        public CraftNodeToScrub(CraftScheme scheme, string path)
            : this(Utility.CraftSchemeMap[scheme], path)
        {
        }
    }
}
