namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System.Collections.Generic;

    internal class BioReactorPatcher
    {
        #region Internal Fields

        internal static IDictionary<TechType, float> CustomBioreactorCharges = new SelfCheckingDictionary<TechType, float>("CustomBioreactorCharges", TechTypeExtensions.sTechTypeComparer);

        #endregion

        #region Patching

        internal static void Patch(HarmonyInstance harmony)
        {
            PatchUtils.PatchDictionary(typeof(BaseBioReactor), "charge", CustomBioreactorCharges);

            Logger.Log("BaseBioReactorPatcher is done.");
        }

        #endregion
    }
}
