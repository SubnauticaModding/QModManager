namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System.Collections.Generic;
    using Utility;

    internal class BioReactorPatcher
    {
        #region Internal Fields

        internal static Dictionary<TechType, float> CustomBioreactorCharges = new Dictionary<TechType, float>(TechTypeExtensions.sTechTypeComparer);

        #endregion

        #region Patching

        internal static void Patch(HarmonyInstance harmony)
        {
            MiscUtils.PatchDictionary(typeof(BaseBioReactor), "charge", CustomBioreactorCharges);

            Logger.Log("BaseBioReactorPatcher is done.");
        }

        #endregion
    }
}
