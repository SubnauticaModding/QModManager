namespace SMLHelper.V2.Handlers
{
    using System;
    using System.Collections.Generic;
    using SMLHelper.V2.FishFramework;
    using SMLHelper.V2.Assets;
    using System.IO;

    /// <summary>
    /// Class to manage registering of fish into the game
    /// </summary>
    public static class CustomFishHandler
    {
        /// <summary>
        /// A list of all the custom fish that have so far been registered into the game. This includes ones from mods that may have been loaded earlier.
        /// It is mainly used by CustomFishPatcher to spawn fish in
        /// </summary>
        public static List<TechType> fishTechTypes = new List<TechType>();

        /// <summary>
        /// Registers a CustomFish object into the game
        /// </summary>
        /// <param name="fish">The CustomFish that you are registering</param>
        /// <returns>The TechType created using the info from your CustomFish object</returns>
        public static TechType RegisterFish(CustomFish fish)
        {
            Logger.Log($"[FishFramework] Creating fish: {fish.displayName}");
            TechType type = TechTypeHandler.AddTechType(fish.id, fish.displayName, fish.tooltip);

            fishTechTypes.Add(type);

            CustomFishPrefab fishPrefab = new CustomFishPrefab(fish.id, $"WorldEntities/Tools/{fish.id}", type)
            {
                modelPrefab = fish.modelPrefab,
                swimSpeed = fish.swimSpeed,
                swimRadius = fish.swimRadius,
                swimInterval = fish.swimInterval,
                pickupable = fish.isPickupable,
                isWaterCreature = fish.isWaterCreature
            };

            if(!string.IsNullOrEmpty(fish.spriteFileName))
            {
                SpriteHandler.RegisterSprite(type, Path.Combine(Environment.CurrentDirectory + "/QMods/",fish.spriteFileName));
            }

            PrefabHandler.RegisterPrefab(fishPrefab);

            return type;
        }
    }
}
