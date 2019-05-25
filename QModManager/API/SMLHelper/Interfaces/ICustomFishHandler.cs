namespace QModManager.API.SMLHelper.Interfaces
{
    using QModManager.API.SMLHelper.Assets;

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
