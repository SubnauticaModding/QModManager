namespace SMLHelper.V2.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using Patchers;

    /// <summary>
    /// A handler class for configuring custom unlocking conditions for item blueprints.
    /// </summary>
    public class KnownTechHandler
    {
        public static void EditAnalysisTechEntry(TechType techTypeToEdit, TechType techTypeToUnlock, string UnlockMessage = "NotificationBlueprintUnlocked", FMODAsset UnlockSound = null, UnityEngine.Sprite UnlockSprite = null)
        {
            EditAnalysisTechEntry(techTypeToEdit, new List<TechType>() { techTypeToUnlock },
                UnlockMessage, UnlockSound, UnlockSprite);
        }

        public static void EditAnalysisTechEntry(TechType techTypeToEdit, IEnumerable<TechType> techTypesToUnlock, string UnlockMessage = "NotificationBlueprintUnlocked", FMODAsset UnlockSound = null, UnityEngine.Sprite UnlockSprite = null)
        {
            KnownTechPatcher.AnalysisTech.Add(new KnownTech.AnalysisTech()
            {
                techType = techTypeToEdit,
                unlockMessage = UnlockMessage,
                unlockSound = UnlockSound,
                unlockPopup = UnlockSprite,
                unlockTechTypes = techTypesToUnlock.ToList()
            });
        }
    }
}
