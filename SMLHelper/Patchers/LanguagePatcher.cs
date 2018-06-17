using System.Collections.Generic;
using LanguagePatcher2 = SMLHelper.V2.Patchers.LanguagePatcher;

namespace SMLHelper.Patchers
{
    [System.Obsolete("SMLHelper.LanguagePatcher is obsolete. Please use SMLHelper.V2 instead.")]
    public class LanguagePatcher
    {
        [System.Obsolete("SMLHelper.LanguagePatcher.unlockedAtStart is obsolete. Please use SMLHelper.V2 instead.")]
        public static Dictionary<string, string> customLines = new Dictionary<string, string>();

        internal static void Patch()
        {
            customLines.ForEach(x => LanguagePatcher2.customLines.Add(x.Key, x.Value));

            V2.Logger.Log("Old LanguagePatcher is done.");
        }
    }
}
