namespace QModManager.API.SMLHelper.Patchers
{
    using Harmony;
    using QModManager.Utility;
    using System.Collections.Generic;
    using System.Linq;

    internal static class KnownTechPatcher
    {
        internal static List<TechType> UnlockedAtStart = new List<TechType>();
        internal static IDictionary<TechType, KnownTech.AnalysisTech> AnalysisTech = new SelfCheckingDictionary<TechType, KnownTech.AnalysisTech>("AnalysisTech");

        private static bool initialized = false;

        private static FMODAsset UnlockSound;

        public static void Patch(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(KnownTech), "Initialize"), 
                postfix: new HarmonyMethod(AccessTools.Method(typeof(KnownTechPatcher), "InitializePostfix")));

            Logger.Debug("KnownTechPatcher is done.");
        }

        internal static void InitializePostfix()
        {
            if (initialized)
                return;
            initialized = true;

            UnlockedAtStart.ForEach(x => KnownTech.Add(x, false));

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

                        if (customTech.unlockMessage != "")
                            tech.unlockMessage = customTech.unlockMessage;
                    }
                }
            }

            foreach (KnownTech.AnalysisTech tech in techToAdd)
            {
                if (tech == null) continue;
                if (tech.unlockSound == null) tech.unlockSound = UnlockSound;

                analysisTech.Add(tech);
            }
        }
    }
}
