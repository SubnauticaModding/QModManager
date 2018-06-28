using SMLHelper.V2.Handlers;

namespace SMLHelper.Patchers
{
    [System.Obsolete("Use SMLHelper.V2 instead.")]
    public class TechTypePatcher
    {
        [System.Obsolete("Use SMLHelper.V2 instead.")]
        public static TechType AddTechType(string name, string languageName, string languageTooltip)
        {
            return TechTypeHandler.AddTechType(name, languageName, languageTooltip);
        }

        [System.Obsolete("Use SMLHelper.V2 instead.")]
        public static TechType AddTechType(string name, string languageName, string languageTooltip, bool unlockOnGameStart)
        {
            return TechTypeHandler.AddTechType(name, languageName, languageTooltip, unlockOnGameStart);
        }
    }
}
