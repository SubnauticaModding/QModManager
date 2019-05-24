using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;

namespace SMLHelper.Patchers
{
    [System.Obsolete("Use SMLHelper.V2 instead.")]
    public class TechTypePatcher
    {
        [System.Obsolete("Use SMLHelper.V2 instead.")]
        public static TechType AddTechType(string name, string languageName, string languageTooltip)
        {
            string modName = ReflectionHelper.CallingAssemblyNameByStackTrace();

            return TechTypeHandler.Singleton.AddTechType(modName, name, languageName, languageTooltip);
        }

        [System.Obsolete("Use SMLHelper.V2 instead.")]
        public static TechType AddTechType(string name, string languageName, string languageTooltip, bool unlockOnGameStart)
        {
            string modName = ReflectionHelper.CallingAssemblyNameByStackTrace();

            return TechTypeHandler.Singleton.AddTechType(modName, name, languageName, languageTooltip, unlockOnGameStart);
        }
    }
}
