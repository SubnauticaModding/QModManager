namespace QModManager.API.SMLHelper.Patchers
{
    using Harmony;
    using QModManager.API.SMLHelper.Handlers;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using Logger = QModManager.Utility.Logger;
    using Random = UnityEngine.Random;

    internal static class FishPatcher
    {
        internal static List<Creature> usedCreatures = new List<Creature>();

        public static void Patch(HarmonyInstance harmony)
        {
            Type creatureType = typeof(Creature);
            Type thisType = typeof(FishPatcher);

            harmony.Patch(creatureType.GetMethod("Start", BindingFlags.Public | BindingFlags.Instance),
                null, new HarmonyMethod(thisType.GetMethod("CreatureStart_Postfix", BindingFlags.NonPublic | BindingFlags.Static)), null);

            Logger.Debug("CustomFishPatcher is done.");
        }

        private static void CreatureStart_Postfix(Creature __instance)
        {
            if (usedCreatures.Contains(__instance) || FishHandler.fishTechTypes.Count == 0)
                return;

            TechTag tag = __instance.GetComponent<TechTag>();
            if (tag && FishHandler.fishTechTypes.Contains(tag.type))
                return;

            if (Random.value < 0.1f)
            {
                int randomIndex = Random.Range(0, FishHandler.fishTechTypes.Count);
                TechType randomFish = FishHandler.fishTechTypes[randomIndex];

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
    }
}
