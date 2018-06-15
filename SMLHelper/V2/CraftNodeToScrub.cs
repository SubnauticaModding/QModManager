namespace SMLHelper.V2
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
    }
}
