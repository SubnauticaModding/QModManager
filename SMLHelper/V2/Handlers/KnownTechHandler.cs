namespace SMLHelper.V2.Handlers
{
    using System.Collections.Generic;
    using Patchers;

    public class KnownTechHandler
    {
        public static void EditAnalysisTechEntry(TechType techTypeToEdit, TechType techTypeToUnlock, string UnlockMessage = "NotificationBlueprintUnlocked", FMODAsset UnlockSound = null, UnityEngine.Sprite UnlockSprite = null)
        {
            KnownTechPatcher.AnalysisTech.Add(new KnownTech.AnalysisTech()
            {
                techType = techTypeToEdit,
                unlockMessage = UnlockMessage,
                unlockSound = UnlockSound,
                unlockPopup = UnlockSprite,
                unlockTechTypes = new List<TechType>()
                {
                    techTypeToUnlock
                }
            });
        }
    }
}
