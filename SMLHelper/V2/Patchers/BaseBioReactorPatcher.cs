using Harmony;
using System.Collections.Generic;

namespace SMLHelper.V2.Patchers
{
    internal class BaseBioReactorPatcher
    {
        #region Internal Fields

        internal static Dictionary<TechType, float> CustomBioreactorCharges = new Dictionary<TechType, float>(TechTypeExtensions.sTechTypeComparer);

        #endregion

        #region Patching

        internal static void Patch(HarmonyInstance harmony)
        {
            Utility.PatchDictionary(typeof(BaseBioReactor), "charge", CustomBioreactorCharges);

            Logger.Log("BaseBioReactorPatcher is done.");
        }

        #endregion
    }
}
