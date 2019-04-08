namespace SMLHelper.V2.Handlers
{
    using System;
    using System.Collections.Generic;
    using SMLHelper.V2.FishFramework;
    using SMLHelper.V2.Assets;
    using System.IO;

    public static class CustomFishHandler
    {
        public static List<TechType> fishTechTypes = new List<TechType>();

        public static void RegisterFish(CustomFish fish)
        {
            Logger.Log($"[FishFramework] Creating fish: {fish.displayName}");
            TechType type = TechTypeHandler.AddTechType(fish.id, fish.displayName, fish.tooltip);

            fishTechTypes.Add(type);

            CustomFishPrefab fishPrefab = new CustomFishPrefab(fish.id, $"WorldEntities/Tools/{fish.id}", type)
            {
                modelPrefab = fish.modelPrefab,
                scale = fish.scale,
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
        }
    }
}
