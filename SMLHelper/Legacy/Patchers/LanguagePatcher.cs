using System.Collections.Generic;
using LanguagePatcher2 = SMLHelper.V2.Patchers.LanguagePatcher;

namespace SMLHelper.Patchers
{
    [System.Obsolete("Use SMLHelper.V2 instead.")]
    public class LanguagePatcher
    {
        [System.Obsolete("Use SMLHelper.V2 instead.")]
        public static Dictionary<string, string> customLines = new Dictionary<string, string>();

        internal static void Patch()
        {
            foreach (KeyValuePair<string, string> kvPair in customLines)
                LanguagePatcher2.AddCustomLanguageLine("SMLHelperLegacy", kvPair.Key, kvPair.Value);

            V2.Logger.Log("Old LanguagePatcher is done.");
        }
    }
}
