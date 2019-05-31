namespace QModManager.API.SMLHelper.Interfaces
{
    using Assets;
    using Handlers;

    /// <summary>
    /// Interface for <see cref="CustomFishHandler"/> <para/>
    /// Can be used for dependency injection
    /// </summary>
    public interface ICustomFishHandler
    {
        /// <summary>
        /// Registers a CustomFish object into the game
        /// </summary>
        /// <param name="fish">The CustomFish that you are registering</param>
        /// <returns>The TechType created using the info from your CustomFish object</returns>
        TechType RegisterFish(CustomFish fish);
    }
}
