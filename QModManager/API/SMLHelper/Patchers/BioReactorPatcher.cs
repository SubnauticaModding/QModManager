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
            // Direct access to private fields made possible by https://github.com/CabbageCrow/AssemblyPublicizer/

            PatchUtils.PatchDictionary(BaseBioReactor.charge, CustomBioreactorCharges);

            Logger.Debug("BaseBioReactorPatcher is done.");
        }
    }
}
