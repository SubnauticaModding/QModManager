namespace SMLHelper.V2.Patchers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HarmonyLib;

    internal class KnownTechPatcher
    {
        private static readonly Func<TechType, string> AsStringFunction = (t) => t.AsString();

        internal static List<TechType> UnlockedAtStart = new List<TechType>();
        internal static IDictionary<TechType, KnownTech.AnalysisTech> AnalysisTech = new SelfCheckingDictionary<TechType, KnownTech.AnalysisTech>("AnalysisTech", AsStringFunction);

        private static bool initialized = false;

        private static FMODAsset UnlockSound;

        public static void Patch(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(KnownTech), nameof(KnownTech.Initialize)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(KnownTechPatcher), nameof(KnownTechPatcher.InitializePostfix))));

            harmony.Patch(AccessTools.Method(typeof(KnownTech), nameof(KnownTech.Deinitialize)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(KnownTechPatcher), nameof(KnownTechPatcher.DeinitializePostfix))));
        }

        internal static void DeinitializePostfix()
        {
            initialized = false;
        }

        internal static void InitializePostfix()
        {
            if (initialized)
                return;
            initialized = true;

            UnlockedAtStart.ForEach(x => KnownTech.Add(x, false));

            // Direct access to private fields made possible by https://github.com/CabbageCrow/AssemblyPublicizer/
            // See README.md for details.
            List<KnownTech.AnalysisTech> analysisTech = KnownTech.analysisTech;
            IEnumerable<KnownTech.AnalysisTech> techToAdd = AnalysisTech.Values.Where(a => !analysisTech.Any(a2 => a.techType == a2.techType));

            foreach (KnownTech.AnalysisTech tech in analysisTech)
            {
                if (tech.unlockSound != null && tech.techType == TechType.BloodOil)
                    UnlockSound = tech.unlockSound;

                foreach (KnownTech.AnalysisTech customTech in AnalysisTech.Values)
                {
                    if (tech.techType == customTech.techType)
                    {
                        if (customTech.unlockTechTypes != null)
                            tech.unlockTechTypes.AddRange(customTech.unlockTechTypes);

                        if (customTech.unlockSound != null)
                            tech.unlockSound = customTech.unlockSound;

                        if (customTech.unlockPopup != null)
                            tech.unlockPopup = customTech.unlockPopup;

                        if (customTech.unlockMessage != string.Empty)
                            tech.unlockMessage = customTech.unlockMessage;
                    }
                }
            }

            foreach (KnownTech.AnalysisTech tech in techToAdd)
            {
                if (tech == null)
                    continue;

                if (tech.unlockSound == null)
                    tech.unlockSound = UnlockSound;

                analysisTech.Add(tech);
            }
        }
    }
}
