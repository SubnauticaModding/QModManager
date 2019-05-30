namespace QModManager.API.SMLHelper.Interfaces
{
    using Handlers;

    /// <summary>
    /// Interface for <see cref="BioReactorHandler"/> <para/>
    /// Can be used for dependency injection
    /// </summary>
    public interface IBioReactorHandler
    {
        /// <summary>
        /// <para>Allows you to specify the quantity of energy that a TechType will produce with bio reactors.</para>
        /// </summary>
        /// <param name="techType">The TechType that you want to use with bioreactors.</param>
        /// <param name="charge">The quantity of energy that will be produced by this TechType.</param>
        /// <seealso cref="CraftData.BackgroundType"/>
        void SetBioReactorCharge(TechType techType, float charge);
    }
}
