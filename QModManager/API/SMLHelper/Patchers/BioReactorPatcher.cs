namespace QModManager.API.SMLHelper.Patchers
{
    using Harmony;
    using QModManager.Utility;
    using System.Collections.Generic;

    internal static class BioReactorPatcher
    {
        internal static IDictionary<TechType, float> CustomBioreactorCharges = new SelfCheckingDictionary<TechType, float>("CustomBioreactorCharges", TechTypeExtensions.sTechTypeComparer);

        internal static void Patch(HarmonyInstance harmony)
        {
            PatchUtils.PatchDictionary(BaseBioReactor.charge, CustomBioreactorCharges);

            Logger.Debug("BaseBioReactorPatcher is done.");
        }
    }
}
