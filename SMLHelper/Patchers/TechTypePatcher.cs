using TechTypePatcher2 = SMLHelper.V2.Patchers.TechTypePatcher;

namespace SMLHelper.Patchers
{
    public class TechTypePatcher
    {
        public static TechType AddTechType(string name, string languageName, string languageTooltip)
        {
            return TechTypePatcher2.AddTechType(name, languageName, languageTooltip);
        }

        public static TechType AddTechType(string name, string languageName, string languageTooltip, bool unlockOnGameStart)
        {
            return TechTypePatcher2.AddTechType(name, languageName, languageTooltip, unlockOnGameStart);
        }
    }
}
