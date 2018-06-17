using System.Collections.Generic;
using KnownTechPatcher2 = SMLHelper.V2.Patchers.KnownTechPatcher;

namespace SMLHelper.Patchers
{
    public class KnownTechPatcher
    {
        public static List<TechType> unlockedAtStart = new List<TechType>();

        public static void Patch()
        {
            unlockedAtStart.ForEach(x => KnownTechPatcher2.unlockedAtStart.Add(x));

            V2.Logger.Log("Old KnownTechPatcher is done.");
        }
    }
}
