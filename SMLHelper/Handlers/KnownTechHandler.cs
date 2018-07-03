namespace SMLHelper.V2.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using Patchers;

    /// <summary>
    /// A handler class for configuring custom unlocking conditions for item blueprints.
    /// </summary>
    public static class KnownTechHandler
    {
        /// <summary>
        /// Allows you to unlock a TechType on game start.
        /// </summary>
        /// <param name="techType"></param>
        public static void UnlockOnStart(TechType techType)
        {
            KnownTechPatcher.UnlockedAtStart.Add(techType);
        }

        /// <summary>
        /// Allows you to define which TechTypes are unlocked when a certain TechType is unlocked, i.e., "analysed".
        /// </summary>
        /// <param name="techTypeToEdit">This TechType is the criteria for all of the "unlock TechTypes"; when this TechType is unlocked, so are all the ones in that list</param>
        /// <param name="techTypesToUnlock">The TechTypes that will be unlocked when "techTypeToEdit" is unlocked.</param>
        /// <param name="UnlockMessage">The message that shows up on the right when the blueprint is unlocked. </param>
        /// <param name="UnlockSound">The sound that plays when you unlock the blueprint.</param>
        /// <param name="UnlockSprite">The sprite that shows up when you unlock the blueprint.</param>
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
