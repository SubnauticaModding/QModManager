using TechTypePatcher2 = SMLHelper.V2.Patchers.TechTypePatcher;

namespace SMLHelper.Patchers
{
    [System.Obsolete("SMLHelper.TechTypePatcher is obsolete. Please use SMLHelper.V2 instead.")]
    public class TechTypePatcher
    {
        [System.Obsolete("SMLHelper.TechTypePatcher.AddTechType is obsolete. Please use SMLHelper.V2 instead.")]
        public static TechType AddTechType(string name, string languageName, string languageTooltip)
        {
            return TechTypePatcher2.AddTechType(name, languageName, languageTooltip);
        }

        [System.Obsolete("SMLHelper.TechTypePatcher.AddTechType is obsolete. Please use SMLHelper.V2 instead.")]
        public static TechType AddTechType(string name, string languageName, string languageTooltip, bool unlockOnGameStart)
        {
            return TechTypePatcher2.AddTechType(name, languageName, languageTooltip, unlockOnGameStart);
        }
    }
}
