using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Reflection;

namespace SMLHelper.V2.FishFramework
{
    [HarmonyPatch(typeof(Creature), "Start")]
    public class FishSpawner
    {
        public static List<TechType> fishTechTypes = new List<TechType>();

        static List<Creature> usedCreatures = new List<Creature>();

        public static void Postfix(Creature __instance)
        {
            if(usedCreatures.Contains(__instance) || fishTechTypes.Count == 0)
            {
                return;
            }
            TechTag tag = __instance.GetComponent<TechTag>();
            if(tag)
            {
                if(fishTechTypes.Contains(tag.type))
                {
                    return;
                }
            }
            if(Random.value < 0.1f)
            {
                Console.WriteLine($"[FishFramework] Selecting fish out of {fishTechTypes.Count} total types");
                int randomIndex = Random.Range(0, fishTechTypes.Count);
                TechType randomFish = fishTechTypes[randomIndex];

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
