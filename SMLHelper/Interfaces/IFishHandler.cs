namespace SMLHelper.V2.Interfaces
{
    using Assets;
    using Handlers;

    /// <summary>
    /// Interface for <see cref="FishHandler"/> <para/>
    /// Can be used for dependency injection
    /// </summary>
    public interface IFishHandler
    {
        /// <summary>
        /// Registers a CustomFish object into the game
        /// </summary>
        /// <param name="fish">The CustomFish that you are registering</param>
        /// <returns>The TechType created using the info from your CustomFish object</returns>
        TechType RegisterFish(Fish fish);
    }
}
