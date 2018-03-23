using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Harmony;

namespace SMLHelper.Patchers
{
    public class KnownTechPatcher
    {
        public static List<TechType> unlockedAtStart = new List<TechType>();

        public static void Patch(HarmonyInstance harmony)
        {
            var knownTech = typeof(KnownTech);
            var initMethod = knownTech.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
            var postfix = typeof(KnownTechPatcher).GetMethod("Postfix", BindingFlags.Public | BindingFlags.Static);

            harmony.Patch(initMethod, null, new HarmonyMethod(postfix));
        }

        private static bool initialized = false;
        public static void Postfix()
        {
            if (initialized) return;

            foreach(var techType in unlockedAtStart)
            {
                KnownTech.Add(techType, false);
            }
        }
    }
}
