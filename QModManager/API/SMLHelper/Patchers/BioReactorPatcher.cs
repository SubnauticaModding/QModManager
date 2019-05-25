namespace QModManager.API.SMLHelper.Patchers
{
    using Harmony;
    using System.Collections.Generic;

    internal static class BioReactorPatcher
    {
        internal static IDictionary<TechType, float> CustomBioreactorCharges = new SelfCheckingDictionary<TechType, float>("CustomBioreactorCharges", TechTypeExtensions.sTechTypeComparer);

        internal static void Patch(HarmonyInstance harmony)
        {
            // Direct access to private fields made possible by https://github.com/CabbageCrow/AssemblyPublicizer/
            // See README.md for details.
            PatchUtils.PatchDictionary(BaseBioReactor.charge, CustomBioreactorCharges);

            Logger.Log("BaseBioReactorPatcher is done.", LogLevel.Debug);
        }
    }
}
