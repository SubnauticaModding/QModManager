namespace SMLHelper.V2.Patchers
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using Harmony;

    internal class KnownTechPatcher
    {
        internal static List<TechType> UnlockedAtStart = new List<TechType>();
        internal static List<KnownTech.AnalysisTech> AnalysisTech = new List<KnownTech.AnalysisTech>();

        private static bool initialized = false;

        private static FMODAsset UnlockSound;

        public static void Patch(HarmonyInstance harmony)
        {
            MethodInfo initMethod = typeof(KnownTech).GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
            MethodInfo postfix = typeof(KnownTechPatcher).GetMethod("InitializePostfix", BindingFlags.NonPublic | BindingFlags.Static);

            harmony.Patch(initMethod, null, new HarmonyMethod(postfix));
        }

        internal static void InitializePostfix()
        {
            if (initialized) return;
            initialized = true;

            UnlockedAtStart.ForEach(x => KnownTech.Add(x, false));

            List<KnownTech.AnalysisTech> analysisTech = (List<KnownTech.AnalysisTech>)typeof(KnownTech).GetField("analysisTech", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            IEnumerable<KnownTech.AnalysisTech> techToAdd = AnalysisTech.Where(a => !analysisTech.Any(a2 => a.techType == a2.techType));

            foreach (KnownTech.AnalysisTech tech in analysisTech)
            {
                if (tech.unlockSound != null && tech.techType == TechType.BloodOil)
                    UnlockSound = tech.unlockSound;

                foreach(KnownTech.AnalysisTech customTech in AnalysisTech)
                {
                    if(tech.techType == customTech.techType)
                    {
                        if(customTech.unlockTechTypes != null)
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

            foreach(KnownTech.AnalysisTech tech in techToAdd)
            {
                if (tech == null) continue;
                if (tech.unlockSound == null) tech.unlockSound = UnlockSound;

                analysisTech.Add(tech);
            }
        }
    }
}
