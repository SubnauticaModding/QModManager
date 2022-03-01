namespace SMLHelper.V2.Patchers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using HarmonyLib;
    using SMLHelper.V2.Handlers;
    using UnityEngine;
    using UWE;
    using Logger = V2.Logger;
    using Random = UnityEngine.Random;

    internal static class FishPatcher
    {
        internal static List<Creature> usedCreatures = new List<Creature>();

        public static void Patch(Harmony harmony)
        {
            Type creatureType = typeof(Creature);
            Type thisType = typeof(FishPatcher);

            harmony.Patch(AccessTools.Method(typeof(Creature), nameof(Creature.Start)),
                            postfix: new HarmonyMethod(typeof(FishPatcher), nameof(FishPatcher.CreatureStart_Postfix)));

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
                CoroutineHost.StartCoroutine(SpawnCustomFish(__instance));
            }
        }

        private static IEnumerator SpawnCustomFish(Creature originalUsedForSpawnLocation)
        {
            int randomIndex = Random.Range(0, FishHandler.fishTechTypes.Count);
            TechType randomFish = FishHandler.fishTechTypes[randomIndex];

            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(randomFish);
            yield return task;

            GameObject prefab = task.GetResult();

            if (prefab is null)
                yield break;

            GameObject fish = GameObject.Instantiate(prefab, originalUsedForSpawnLocation.transform.position, originalUsedForSpawnLocation.transform.rotation, false);
            prefab.SetActive(false);

            // Deletes the fish if it is a ground creature spawned in water
            if (fish.GetComponent<WalkOnGround>() && !originalUsedForSpawnLocation.GetComponent<WalkOnGround>())
            {
                GameObject.Destroy(fish);
                yield break;
            }

            // Deletes the fish if it is a water creature spawned on ground
            if (!fish.GetComponent<WalkOnGround>() && originalUsedForSpawnLocation.GetComponent<WalkOnGround>())
            {
                GameObject.Destroy(fish);
                yield break;
            }

            fish.SetActive(true);

            usedCreatures.Add(originalUsedForSpawnLocation);
            yield break;
        }
    }
}
