namespace QModManager.API.SMLHelper.Interfaces
{
    using QModManager.API.SMLHelper.Assets;

    public interface ICustomFishHandler
    {
        TechType RegisterFish(CustomFish fish);
    }
}
