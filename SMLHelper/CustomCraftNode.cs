namespace SMLHelper
{
    public class CustomCraftNode
    {
        public TechType TechType;
        public CraftScheme Scheme;
        public string Path;

        public CustomCraftNode(TechType techType, CraftScheme scheme, string path)
        {
            TechType = techType;
            Scheme = scheme;
            Path = path;
        }
    }
}
