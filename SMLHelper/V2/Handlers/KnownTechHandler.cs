namespace SMLHelper.V2.Handlers
{
    using System.Collections.Generic;
    using Patchers;

    /// <summary>
    /// A handler class for configuring custom unlocking conditions for item blueprints.
    /// </summary>
    public class KnownTechHandler
    {
<<<<<<< HEAD
        public static void EditAnalysisTechEntry(TechType techTypeToEdit, TechType techTypeToUnlock, string UnlockMessage = "NotificationBlueprintUnlocked", FMODAsset UnlockSound = null, UnityEngine.Sprite UnlockSprite = null)
=======
        /// <summary>
        /// Adds a custom <see cref="KnownTech.AnalysisTech"/> to add conditions for when an item blueprint unlocks.
        /// </summary>
        /// <param name="analysisTech">The analysis tech.</param>        
        public static void AddToAnalysisTech(KnownTech.AnalysisTech analysisTech)
>>>>>>> ce5afafc92f49516481d51029086814caa1bb908
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
