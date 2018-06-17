using SMLHelper.V2.Handlers;

namespace SMLHelper.Patchers
{
    [System.Obsolete("SMLHelper.TechTypePatcher is obsolete. Please use SMLHelper.V2 instead.")]
    public class TechTypePatcher
    {
        [System.Obsolete("SMLHelper.TechTypePatcher.AddTechType is obsolete. Please use SMLHelper.V2 instead.")]
        public static TechType AddTechType(string name, string languageName, string languageTooltip)
        {
            return TechTypeHandler.AddTechType(name, languageName, languageTooltip);
        }

        [System.Obsolete("SMLHelper.TechTypePatcher.AddTechType is obsolete. Please use SMLHelper.V2 instead.")]
        public static TechType AddTechType(string name, string languageName, string languageTooltip, bool unlockOnGameStart)
        {
            return TechTypeHandler.AddTechType(name, languageName, languageTooltip, unlockOnGameStart);
        }
    }
}
