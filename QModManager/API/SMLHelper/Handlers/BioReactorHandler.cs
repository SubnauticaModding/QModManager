namespace QModManager.API.SMLHelper.Handlers
{
    using Patchers;
    using Interfaces;

    public class BioReactorHandler : IBioReactorHandler
    {
        // Singleton
        public static IBioReactorHandler Main { get; } = new BioReactorHandler();

        private BioReactorHandler()
        {
            // Hides constructor
        }

        /// <summary>
        /// <para>Allows you to specify the quantity of energy that a TechType will produce with bio reactors.</para>
        /// </summary>
        /// <param name="techType">The TechType that you want to use with bioreactors.</param>
        /// <param name="charge">The quantity of energy that will be produced by this TechType.</param>
        /// <seealso cref="CraftData.BackgroundType"/>
        void IBioReactorHandler.SetBioReactorCharge(TechType techType, float charge)
        {
            BioReactorPatcher.CustomBioreactorCharges.Add(techType, charge);
        }

        /// <summary>
        /// <para>Allows you to specify the quantity of energy that a TechType will produce with bio reactors.</para>
        /// </summary>
        /// <param name="techType">The TechType that you want to use with bioreactors.</param>
        /// <param name="charge">The quantity of energy that will be produced by this TechType.</param>
        /// <seealso cref="CraftData.BackgroundType"/>
        public static void SetBioReactorCharge(TechType techType, float charge)
        {
            Main.SetBioReactorCharge(techType, charge);
        }
    }
}
