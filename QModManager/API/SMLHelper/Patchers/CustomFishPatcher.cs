namespace QModManager.API.SMLHelper.Patchers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Harmony;
    using UnityEngine;
    using Random = UnityEngine.Random;
    using QModManager.API.SMLHelper.Handlers;
    using Logger = QModManager.API.SMLHelper.Logger;

    internal static class CustomFishPatcher
    {
        static List<Creature> usedCreatures = new List<Creature>();

        private static void CreatureStart_Postfix(Creature __instance)
        {
            if(usedCreatures.Contains(__instance) || CustomFishHandler.fishTechTypes.Count == 0)
            {
                return;
            }
            TechTag tag = __instance.GetComponent<TechTag>();
            if(tag)
            {
                if(CustomFishHandler.fishTechTypes.Contains(tag.type))
                {
                    return;
                }
            }
            if(Random.value < 0.1f)
            {
                Logger.Log($"[FishFramework] Selecting fish out of {CustomFishHandler.fishTechTypes.Count} total types", LogLevel.Debug);
                int randomIndex = Random.Range(0, CustomFishHandler.fishTechTypes.Count);
                TechType randomFish = CustomFishHandler.fishTechTypes[randomIndex];

                GameObject fish = CraftData.InstantiateFromPrefab(randomFish);
                // Deletes the fish if it is a ground creature spawned in water
                if (fish.GetComponent<WalkOnGround>() && !__instance.GetComponent<WalkOnGround>())
                {
                    GameObject.Destroy(fish);
                    return;
                }
                // Deletes the fish if it is a water creature spawned on ground
                if (!fish.GetComponent<WalkOnGround>() && __instance.GetComponent<WalkOnGround>())
                {
                    GameObject.Destroy(fish);
                    return;
                }
                fish.transform.position = __instance.transform.position;

                usedCreatures.Add(__instance);
            }
        }

        public static void Patch(HarmonyInstance harmony)
        {
            Type creatureType = typeof(Creature);
            Type thisType = typeof(CustomFishPatcher);

            harmony.Patch(creatureType.GetMethod("Start", BindingFlags.Public | BindingFlags.Instance),
                null, new HarmonyMethod(thisType.GetMethod("CreatureStart_Postfix", BindingFlags.NonPublic | BindingFlags.Static)), null);

            Logger.Log("CustomFishPatcher is done.", LogLevel.Debug);
        }
    }
}
