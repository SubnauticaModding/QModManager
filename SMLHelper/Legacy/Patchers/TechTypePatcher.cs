using System.Reflection;
using SMLHelper.V2.Handlers;

namespace SMLHelper.Patchers
{
    [System.Obsolete("Use SMLHelper.V2 instead.")]
    public class TechTypePatcher
    {
        [System.Obsolete("Use SMLHelper.V2 instead.")]
        public static TechType AddTechType(string name, string languageName, string languageTooltip)
        {
            string modName = Assembly.GetCallingAssembly().GetName().Name;

            return TechTypeHandler.Singleton.AddTechType(modName, name, languageName, languageTooltip);
        }

        [System.Obsolete("Use SMLHelper.V2 instead.")]
        public static TechType AddTechType(string name, string languageName, string languageTooltip, bool unlockOnGameStart)
        {
            string modName = Assembly.GetCallingAssembly().GetName().Name;

            return TechTypeHandler.Singleton.AddTechType(modName, name, languageName, languageTooltip, unlockOnGameStart);
        }
    }
}
