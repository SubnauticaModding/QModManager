namespace SMLHelper.V2.Patchers
{
    using System.Collections.Generic;
    using HarmonyLib;
    using Logger = V2.Logger;

    internal class LootDistributionPatcher
    {
        internal static readonly SelfCheckingDictionary<string, LootDistributionData.SrcData> CustomSrcData = new SelfCheckingDictionary<string, LootDistributionData.SrcData>("CustomSrcData");

        internal static void Patch(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(LootDistributionData), nameof(LootDistributionData.Initialize)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(LootDistributionPatcher), nameof(LootDistributionPatcher.InitializePostfix))));

            Logger.Log("LootDistributionPatcher is done.", LogLevel.Debug);
        }

        private static void InitializePostfix(LootDistributionData __instance)
        {
            foreach (KeyValuePair<string, LootDistributionData.SrcData> entry in CustomSrcData)
            {
                LootDistributionData.SrcData customSrcData = entry.Value;
                string classId = entry.Key;
                if(customSrcData != null)
                {
                    if (__instance.srcDistribution.TryGetValue(entry.Key, out LootDistributionData.SrcData srcData))
                    {
                        EditExistingData(classId, srcData, customSrcData, __instance.dstDistribution);
                    }
                    else
                    {
                        AddCustomData(classId, customSrcData, __instance.srcDistribution, __instance.dstDistribution);
                    }
                }
            }
        }

        private static void EditExistingData(string classId, LootDistributionData.SrcData existingData, LootDistributionData.SrcData changes, Dictionary<BiomeType, LootDistributionData.DstData> dstData)
        {
            foreach (LootDistributionData.BiomeData customBiomeDist in changes.distribution)
            {
                bool foundBiome = false;

                for (int i = 0; i < existingData.distribution.Count; i++)
                {
                    LootDistributionData.BiomeData biomeDist = existingData.distribution[i];

                    if (customBiomeDist.biome == biomeDist.biome)
                    {
                        biomeDist.count = customBiomeDist.count;
                        biomeDist.probability = customBiomeDist.probability;

                        foundBiome = true;
                    }
                }

                if (!foundBiome)
                {
                    existingData.distribution.Add(customBiomeDist);
                }

                if (!dstData.TryGetValue(customBiomeDist.biome, out LootDistributionData.DstData biomeDistData))
                {
                    biomeDistData = new LootDistributionData.DstData
                    {
                        prefabs = new List<LootDistributionData.PrefabData>()
                    };
                    dstData.Add(customBiomeDist.biome, biomeDistData);
                }

                bool foundPrefab = false;

                for (int j = 0; j < biomeDistData.prefabs.Count; j++)
                {
                    LootDistributionData.PrefabData prefabData = biomeDistData.prefabs[j];

                    if (prefabData.classId == classId)
                    {
                        prefabData.count = customBiomeDist.count;
                        prefabData.probability = customBiomeDist.probability;

                        foundPrefab = true;
                    }
                }

                if (!foundPrefab)
                {
                    biomeDistData.prefabs.Add(new LootDistributionData.PrefabData()
                    {
                        classId = classId,
                        count = customBiomeDist.count,
                        probability = customBiomeDist.probability
                    });
                }
            }
        }

        private static void AddCustomData(string classId, LootDistributionData.SrcData customSrcData, Dictionary<string, LootDistributionData.SrcData> srcDistribution, Dictionary<BiomeType, LootDistributionData.DstData> dstDistribution)
        {
            srcDistribution.Add(classId, customSrcData);

            List<LootDistributionData.BiomeData> distribution = customSrcData.distribution;

            if (distribution != null)
            {
                for (int i = 0; i < distribution.Count; i++)
                {
                    LootDistributionData.BiomeData biomeData = distribution[i];
                    BiomeType biome = biomeData.biome;
                    int count = biomeData.count;
                    float probability = biomeData.probability;

                    if (!dstDistribution.TryGetValue(biome, out LootDistributionData.DstData dstData))
                    {
                        dstData = new LootDistributionData.DstData
                        {
                            prefabs = new List<LootDistributionData.PrefabData>()
                        };
                        dstDistribution.Add(biome, dstData);
                    }

                    var prefabData = new LootDistributionData.PrefabData
                    {
                        classId = classId,
                        count = count,
                        probability = probability
                    };
                    dstData.prefabs.Add(prefabData);
                }
            }
        }
    }
}
