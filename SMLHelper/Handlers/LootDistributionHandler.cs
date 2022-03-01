namespace SMLHelper.V2.Handlers
{
    using System.Collections.Generic;
    using Interfaces;
    using Patchers;
    using SMLHelper.V2.Assets;
    using UWE;

    /// <summary>
    /// A handler that manages Loot Distribution.
    /// </summary>
    public class LootDistributionHandler : ILootDistributionHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static ILootDistributionHandler Main { get; } = new LootDistributionHandler();

        private LootDistributionHandler() { } // Hides constructor

        #region Static Methods

        /// <summary>
        /// Adds in a custom entry into the Loot Distribution of the game.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="classId"></param>
        public static void AddLootDistributionData(string classId, LootDistributionData.SrcData data)
        {
            Main.AddLootDistributionData(classId, data);
        }

        /// <summary>
        /// Adds in a custom entry into the Loot Distribution of the game.
        /// You must also add the <see cref="WorldEntityInfo"/> into the <see cref="WorldEntityDatabase"/> using <see cref="WorldEntityDatabaseHandler"/>.
        /// </summary>
        /// <param name="data">The <see cref="LootDistributionData.SrcData"/> that contains data related to the spawning of a prefab, also contains the path to the prefab.</param>
        /// <param name="classId">The classId of the prefab.</param>
        /// <param name="info">The WorldEntityInfo of the prefab. For more information on how to set this up, see <see cref="WorldEntityDatabaseHandler"/>.</param>
        public static void AddLootDistributionData(string classId, LootDistributionData.SrcData data, WorldEntityInfo info)
        {
            Main.AddLootDistributionData(classId, data, info);
        }

        /// <summary>
        /// Adds in a custom entry into the Loot Distribution of the game.
        /// You must also add the <see cref="WorldEntityInfo"/> into the <see cref="WorldEntityDatabase"/> using <see cref="WorldEntityDatabaseHandler"/>.
        /// </summary>
        /// <param name="classId">The classId of the prefab.</param>
        /// <param name="prefabPath">The prefab path of the prefab.</param>
        /// <param name="biomeDistribution">The <see cref="LootDistributionData.BiomeData"/> dictating how the prefab should spawn in the world.</param>
        public static void AddLootDistributionData(string classId, string prefabPath, IEnumerable<LootDistributionData.BiomeData> biomeDistribution)
        {
            Main.AddLootDistributionData(classId, prefabPath, biomeDistribution);
        }

        /// <summary>
        /// Adds in a custom entry into the Loot Distribution of the game.
        /// You must also add the <see cref="WorldEntityInfo"/> into the <see cref="WorldEntityDatabase"/> using <see cref="WorldEntityDatabaseHandler"/>.
        /// </summary>
        /// <param name="classId">The classId of the prefab.</param>
        /// <param name="prefabPath">The prefab path of the prefab.</param>
        /// <param name="biomeDistribution">The <see cref="LootDistributionData.BiomeData"/> dictating how the prefab should spawn in the world.</param>
        /// <param name="info">The WorldEntityInfo of the prefab. For more information on how to set this up, see <see cref="WorldEntityDatabaseHandler"/>.</param>
        public static void AddLootDistributionData(string classId, string prefabPath, IEnumerable<LootDistributionData.BiomeData> biomeDistribution, WorldEntityInfo info)
        {
            Main.AddLootDistributionData(classId, prefabPath, biomeDistribution, info);
        }

        /// <summary>
        /// Adds in a custom entry into the Loot Distribution of the game.
        /// You must also add the <see cref="WorldEntityInfo"/> into the <see cref="WorldEntityDatabase"/> using <see cref="WorldEntityDatabaseHandler"/>.
        /// </summary>
        /// <param name="prefab">The custom prefab which you want to spawn naturally in the game.</param>
        /// <param name="biomeDistribution">The <see cref="LootDistributionData.BiomeData"/> dictating how the prefab should spawn in the world.</param>
        /// <param name="info">The WorldEntityInfo of the prefab. For more information on how to set this up, see <see cref="WorldEntityDatabaseHandler"/>.</param>
        public static void AddLootDistributionData(ModPrefab prefab, IEnumerable<LootDistributionData.BiomeData> biomeDistribution, WorldEntityInfo info)
        {
            Main.AddLootDistributionData(prefab, biomeDistribution, info);
        }

        /// <summary>
        /// Edits Loot Distribution Data for existing/original class IDs. 
        /// </summary>
        public static void EditLootDistributionData(string classId, BiomeType biome, float probability, int count)
        {
            Main.EditLootDistributionData(classId, biome, probability, count);
        }

        /// <summary>
        /// Edits Loot Distribution data for existing prefabs, for e.g. original game prefabs.
        /// </summary>
        /// <param name="classId">The ClassID of the prefab. If unsure, use CraftData.GetClassIdForTechType.</param>
        /// <param name="biomeDistribution">The list of <see cref="LootDistributionData.BiomeData"/> that contains information about how/when it should spawn in biomes.</param>
        public static void EditLootDistributionData(string classId, IEnumerable<LootDistributionData.BiomeData> biomeDistribution)
        {
            Main.EditLootDistributionData(classId, biomeDistribution);
        }

        #endregion

        #region Interface methods

        /// <summary>
        /// Adds in a custom entry into the Loot Distribution of the game.
        /// You must also add the <see cref="WorldEntityInfo"/> into the <see cref="WorldEntityDatabase"/> using <see cref="WorldEntityDatabaseHandler"/>.
        /// </summary>
        /// <param name="data">The <see cref="LootDistributionData.SrcData"/> that contains data related to the spawning of a prefab, also contains the path to the prefab.</param>
        /// <param name="classId">The classId of the prefab.</param>
        /// <param name="info">The WorldEntityInfo of the prefab. For more information on how to set this up, see <see cref="WorldEntityDatabaseHandler"/>.</param>
        void ILootDistributionHandler.AddLootDistributionData(string classId, LootDistributionData.SrcData data, WorldEntityInfo info)
        {
            Main.AddLootDistributionData(classId, data);

            WorldEntityDatabaseHandler.AddCustomInfo(classId, info);
        }

        /// <summary>
        /// Adds in a custom entry into the Loot Distribution of the game.
        /// You must also add the <see cref="WorldEntityInfo"/> into the <see cref="WorldEntityDatabase"/> using <see cref="WorldEntityDatabaseHandler"/>.
        /// </summary>
        /// <param name="classId">The classId of the prefab.</param>
        /// <param name="prefabPath">The prefab path of the prefab.</param>
        /// <param name="biomeDistribution">The <see cref="LootDistributionData.BiomeData"/> dictating how the prefab should spawn in the world.</param>
        void ILootDistributionHandler.AddLootDistributionData(string classId, string prefabPath, IEnumerable<LootDistributionData.BiomeData> biomeDistribution)
        {
            Main.AddLootDistributionData(classId, new LootDistributionData.SrcData()
            {
                distribution = new List<LootDistributionData.BiomeData>(biomeDistribution),
                prefabPath = prefabPath
            });
        }

        /// <summary>
        /// Adds in a custom entry into the Loot Distribution of the game.
        /// </summary>
        /// <param name="classId">The classId of the prefab.</param>
        /// <param name="prefabPath">The prefab path of the prefab.</param>
        /// <param name="biomeDistribution">The <see cref="LootDistributionData.BiomeData"/> dictating how the prefab should spawn in the world.</param>
        /// <param name="info">The WorldEntityInfo of the prefab. For more information on how to set this up, see <see cref="WorldEntityDatabaseHandler"/>.</param>
        void ILootDistributionHandler.AddLootDistributionData(string classId, string prefabPath, IEnumerable<LootDistributionData.BiomeData> biomeDistribution, WorldEntityInfo info)
        {
            Main.AddLootDistributionData(classId, new LootDistributionData.SrcData()
            {
                distribution = new List<LootDistributionData.BiomeData>(biomeDistribution),
                prefabPath = prefabPath
            });

            WorldEntityDatabaseHandler.AddCustomInfo(classId, info);
        }

        /// <summary>
        /// Adds in a custom entry into the Loot Distribution of the game.
        /// </summary>
        /// <param name="prefab">The custom prefab which you want to spawn naturally in the game.</param>
        /// <param name="biomeDistribution">The <see cref="LootDistributionData.BiomeData"/> dictating how the prefab should spawn in the world.</param>
        /// <param name="info">The WorldEntityInfo of the prefab. For more information on how to set this up, see <see cref="WorldEntityDatabaseHandler"/>.</param>
        void ILootDistributionHandler.AddLootDistributionData(ModPrefab prefab, IEnumerable<LootDistributionData.BiomeData> biomeDistribution, WorldEntityInfo info)
        {
            Main.AddLootDistributionData(prefab.ClassID, prefab.PrefabFileName, biomeDistribution);

            WorldEntityDatabaseHandler.AddCustomInfo(prefab.ClassID, info);
        }

        void ILootDistributionHandler.AddLootDistributionData(string classId, LootDistributionData.SrcData data)
        {
            if (LootDistributionPatcher.CustomSrcData.ContainsKey(classId))
                Logger.Log($"{classId}-{data.prefabPath} already has custom distribution data. Replacing with latest.", LogLevel.Debug);

            LootDistributionPatcher.CustomSrcData[classId] = data;
        }

        void ILootDistributionHandler.EditLootDistributionData(string classId, BiomeType biome, float probability, int count)
        {
            if (!LootDistributionPatcher.CustomSrcData.TryGetValue(classId, out LootDistributionData.SrcData srcData))
            {
                LootDistributionPatcher.CustomSrcData[classId] = (srcData = new LootDistributionData.SrcData());

                var biomeDistribution = new List<LootDistributionData.BiomeData>
                {
                    new LootDistributionData.BiomeData()
                    {
                        biome = biome,
                        probability = probability,
                        count = count
                    }
                };

                srcData.distribution = biomeDistribution;

                return;
            }

            for (int i = 0; i < srcData.distribution.Count; i++)
            {
                LootDistributionData.BiomeData distribution = srcData.distribution[i];

                if (distribution.biome == biome)
                {
                    distribution.count = count;
                    distribution.probability = probability;

                    return;
                }
            }

            // If we reached this point, that means the srcData is present, but the biome in the distribution is not.
            // Lets add it manually.
            srcData.distribution.Add(new LootDistributionData.BiomeData()
            {
                biome = biome,
                probability = probability,
                count = count
            });
        }

        /// <summary>
        /// Edits Loot Distribution data for existing prefabs, for e.g. original game prefabs.
        /// </summary>
        /// <param name="classId">The ClassID of the prefab. If unsure, use CraftData.GetClassIdForTechType.</param>
        /// <param name="biomeDistribution">The list of <see cref="LootDistributionData.BiomeData"/> that contains information about how/when it should spawn in biomes.</param>
        void ILootDistributionHandler.EditLootDistributionData(string classId, IEnumerable<LootDistributionData.BiomeData> biomeDistribution)
        {
            foreach (LootDistributionData.BiomeData distribution in biomeDistribution)
            {
                Main.EditLootDistributionData(classId, distribution.biome, distribution.probability, distribution.count);
            }
        }

        #endregion
    }
}
