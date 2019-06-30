namespace QModManager.API.SMLHelper.Handlers
{
    using System;
    using System.Collections.Generic;
    using QModManager.API.SMLHelper.Assets;
    using System.IO;
    using QModManager.API.SMLHelper.Interfaces;
    using QModManager.Utility;

    /// <summary>
    /// Class to manage registering of fish into the game
    /// </summary>
    public class FishHandler : IFishHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static IFishHandler Main { get; } = new FishHandler();

        private FishHandler() { }

        /// <summary>
        /// A list of all the custom fish that have so far been registered into the game. This includes ones from mods that may have been loaded earlier.
        /// It is mainly used by CustomFishPatcher to spawn fish in
        /// </summary>
        internal static List<TechType> fishTechTypes = new List<TechType>();

        /// <summary>
        /// Registers a CustomFish object into the game
        /// </summary>
        /// <param name="fish">The CustomFish that you are registering</param>
        /// <returns>The TechType created using the info from your CustomFish object</returns>
        TechType IFishHandler.RegisterFish(Fish fish)
        {
            TechType type = TechTypeHandler.AddTechType(fish.id, fish.displayName, fish.tooltip);

            fishTechTypes.Add(type);

            FishPrefab fishPrefab = new FishPrefab(fish.id, $"WorldEntities/Tools/{fish.id}", type)
            {
                modelPrefab = fish.modelPrefab,
                swimSpeed = fish.swimSpeed,
                swimRadius = fish.swimRadius,
                swimInterval = fish.swimInterval,
                pickupable = fish.isPickupable,
                isWaterCreature = fish.isWaterCreature
            };

            if (!string.IsNullOrEmpty(fish.spriteFileName))
            {
                SpriteHandler.RegisterSprite(type, Path.Combine(Path.Combine(Environment.CurrentDirectory, "QMods"), fish.spriteFileName));
            }

            PrefabHandler.RegisterPrefab(fishPrefab);

            Logger.Debug($"Successfully registered fish: '{fish.displayName}' with Tech Type: '{fish.id}'");

            return type;
        }

        /// <summary>
        /// Registers a CustomFish object into the game
        /// </summary>
        /// <param name="fish">The CustomFish that you are registering</param>
        /// <returns>The TechType created using the info from your CustomFish object</returns>
        public static TechType RegisterFish(Fish fish)
        {
            return Main.RegisterFish(fish);
        }
    }
}
