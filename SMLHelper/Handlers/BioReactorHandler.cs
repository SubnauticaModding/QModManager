namespace SMLHelper.V2.Handlers
{
    using Patchers;

    public static class BioReactorHandler
    {
        /// <summary>
        /// <para>Allows you to specify the quantity of energy that a TechType will produce with bio reactors.</para>
        /// </summary>
        /// <param name="techType">The TechType that you want to use with bioreactors.</param>
        /// <param name="charge">The quantity of energy that will be produced by this TechType.</param>
        /// <seealso cref="CraftData.BackgroundType"/>
        public static void SetBioReactorCharge(TechType techType, float charge)
        {
            BioReactorPatcher.CustomBioreactorCharges[techType] = charge;
        }
    }
}
