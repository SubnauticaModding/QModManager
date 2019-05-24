using System.Collections.Generic;
using KnownTechPatcher2 = SMLHelper.V2.Patchers.KnownTechPatcher;

namespace SMLHelper.Patchers
{
    [System.Obsolete("Use SMLHelper.V2 instead.")]
    public class KnownTechPatcher
    {
        [System.Obsolete("Use SMLHelper.V2 instead.")]
        public static List<TechType> unlockedAtStart = new List<TechType>();

        internal static void Patch()
        {
            unlockedAtStart.ForEach(x => KnownTechPatcher2.UnlockedAtStart.Add(x));

            V2.Logger.Log("Old KnownTechPatcher is done.", V2.LogLevel.Debug);
        }
    }
}
