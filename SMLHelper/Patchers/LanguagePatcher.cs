using System.Collections.Generic;
using LanguagePatcher2 = SMLHelper.V2.Patchers.LanguagePatcher;

namespace SMLHelper.Patchers
{
    public class LanguagePatcher
    {
        public static Dictionary<string, string> customLines = new Dictionary<string, string>();

        public static void Patch()
        {
            customLines.ForEach(x => LanguagePatcher2.customLines.Add(x.Key, x.Value));
        }
    }
}
