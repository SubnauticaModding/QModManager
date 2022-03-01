namespace SMLHelper.V2.Handlers
{
    using System;
    using System.Collections.Generic;
    using Patchers;
    using Interfaces;

    /// <summary>
    /// a common handler for uses specified to the Survival component
    /// </summary>
    public class SurvivalHandler : ISurvivalHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static ISurvivalHandler Main { get; } = new SurvivalHandler();

        private SurvivalHandler()
        {
            // Hides constructor
        }

        #region Implementation Methods
        /// <summary>
        /// <para>makes the item gives oxygen on use.</para>
        /// </summary>
        /// <param name="techType">the TechType that you want to make it give oxygen on use</param>
        /// <param name="oxygenGiven">the oxygen amount the item gives</param>
        /// <param name="isEdible">set it to <see langword="true" /> if the item is edible and has the <see cref="Eatable"/> component attached to it. 
        /// <para>defaults to <see langword="false" /></para>
        /// </param>
        void ISurvivalHandler.GiveOxygenOnConsume(TechType techType, float oxygenGiven, bool isEdible)
        {
            if (SurvivalPatcher.CustomSurvivalInventoryAction.TryGetValue(techType, out List<Action> action))
            {
                action.Add(() => { Player.main.GetComponent<OxygenManager>().AddOxygen(oxygenGiven); }); // add an action to the list
                return;
            }

            // if we reach to this point then the techtype doesn't exist in the dictionary so we add it
            SurvivalPatcher.CustomSurvivalInventoryAction[techType] = new List<Action>()
            {
                () =>
                {
                    Player.main.GetComponent<OxygenManager>().AddOxygen(oxygenGiven);
                }
            };
            if (!isEdible)
                SurvivalPatcher.InventoryUseables.Add(techType);
        }
        /// <summary>
        /// <para>makes the item Heal the player on consume.</para>
        /// </summary>
        /// <param name="techType">the TechType that you want it to heal back</param>
        /// <param name="healthBack">amount to heal the player</param>
        /// <param name="isEdible">set it to <see langword="true" /> if the item is edible and has the <see cref="Eatable"/> component attached to it. 
        /// <para>defaults to <see langword="false" /></para>
        /// </param>
        void ISurvivalHandler.GiveHealthOnConsume(TechType techType, float healthBack, bool isEdible)
        {
            if (SurvivalPatcher.CustomSurvivalInventoryAction.TryGetValue(techType, out List<Action> action))
            {
                action.Add(() => { Player.main.GetComponent<LiveMixin>().AddHealth(healthBack); }); // add an action to the list
                return;
            }

            // if we reach to this point then the techtype doesn't exist in the dictionary so we add it
            SurvivalPatcher.CustomSurvivalInventoryAction[techType] = new List<Action>()
            {
                () =>
                {
                    Player.main.GetComponent<LiveMixin>().AddHealth(healthBack);
                }
            };
            if (!isEdible)
                SurvivalPatcher.InventoryUseables.Add(techType);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// <para>makes the item gives oxygen on use.</para>
        /// </summary>
        /// <param name="techType">the TechType that you want to make it give oxygen on use</param>
        /// <param name="oxygenGiven">the oxygen amount the item gives</param>
        /// <param name="isEdible">set it to <see langword="true" /> if the item is edible and has the <see cref="Eatable"/> component attached to it. 
        /// <para>defaults to <see langword="false" /></para>
        /// </param>
        public static void GiveOxygenOnConsume(TechType techType, float oxygenGiven, bool isEdible = false)
        {
            Main.GiveOxygenOnConsume(techType, oxygenGiven, isEdible);
        }
        /// <summary>
        /// <para>makes the item Heal the player on consume.</para>
        /// </summary>
        /// <param name="techType">the TechType that you want it to heal back</param>
        /// <param name="healthBack">amount to heal the player</param>
        /// <param name="isEdible">set it to <see langword="true" /> if the item is edible and has the <see cref="Eatable"/> component attached to it. 
        /// <para>defaults to <see langword="false" /></para>
        /// </param>
        public static void GiveHealthOnConsume(TechType techType, float healthBack, bool isEdible = false)
        {
            Main.GiveHealthOnConsume(techType, healthBack, isEdible);
        }
        #endregion
    }
}
