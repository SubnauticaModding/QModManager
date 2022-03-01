namespace SMLHelper.V2.Interfaces
{
    /// <summary>
    /// a handler for common uses to the Survival component
    /// </summary>
    public interface ISurvivalHandler
    {
        /// <summary>
        /// <para>makes the item gives oxygen on use.</para>
        /// </summary>
        /// <param name="techType">the TechType that you want to make it give oxygen on use</param>
        /// <param name="oxygenGiven">the oxygen amount the item gives</param>
        /// <param name="isEdible">set it to <see langword="true" /> if the item is edible and has the <see cref="Eatable"/> component attached to it. 
        /// <para>defaults to <see langword="false" /></para>
        /// </param>
        void GiveOxygenOnConsume(TechType techType, float oxygenGiven, bool isEdible = false);
        /// <summary>
        /// <para>makes the item Heal the player on consume.</para>
        /// </summary>
        /// <param name="techType">the TechType that you want it to heal back</param>
        /// <param name="healthBack">amount to heal the player</param>
        /// <param name="isEdible">set it to <see langword="true" /> if the item is edible and has the <see cref="Eatable"/> component attached to it. 
        /// <para>defaults to <see langword="false" /></para>
        /// </param>
        void GiveHealthOnConsume(TechType techType, float healthBack, bool isEdible = false);
    }
}
