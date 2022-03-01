using System.Collections.Generic;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Handlers;
using UWE;

namespace SMLHelper.V2.Interfaces
{
    /// <summary>
    /// A handler that manages Loot Distribution (spawning of fragments, fish, etc).
    /// </summary>
    public interface ILootDistributionHandler
    {
        /// <summary>
        /// Adds in a custom entry into the Loot Distribution of the game.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="classId"></param>
        void AddLootDistributionData(string classId, LootDistributionData.SrcData data);

        /// <summary>
        /// Adds in a custom entry into the Loot Distribution of the game.
        /// You must also add the <see cref="WorldEntityInfo"/> into the <see cref="WorldEntityDatabase"/> using <see cref="WorldEntityDatabaseHandler"/>.
        /// </summary>
        /// <param name="data">The <see cref="LootDistributionData.SrcData"/> that contains data related to the spawning of a prefab, also contains the path to the prefab.</param>
        /// <param name="classId">The classId of the prefab.</param>
        /// <param name="info">The WorldEntityInfo of the prefab. For more information on how to set this up, see <see cref="WorldEntityDatabaseHandler"/>.</param>
        void AddLootDistributionData(string classId, LootDistributionData.SrcData data, WorldEntityInfo info);

        /// <summary>
        /// Adds in a custom entry into the Loot Distribution of the game.
        /// You must also add the <see cref="WorldEntityInfo"/> into the <see cref="WorldEntityDatabase"/> using <see cref="WorldEntityDatabaseHandler"/>.
        /// </summary>
        /// <param name="classId">The classId of the prefab.</param>
        /// <param name="prefabPath">The prefab path of the prefab.</param>
        /// <param name="biomeDistribution">The <see cref="LootDistributionData.BiomeData"/> dictating how the prefab should spawn in the world.</param>
        void AddLootDistributionData(string classId, string prefabPath, IEnumerable<LootDistributionData.BiomeData> biomeDistribution);

        /// <summary>
        /// Adds in a custom entry into the Loot Distribution of the game.
        /// You must also add the <see cref="WorldEntityInfo"/> into the <see cref="WorldEntityDatabase"/> using <see cref="WorldEntityDatabaseHandler"/>.
        /// </summary>
        /// <param name="classId">The classId of the prefab.</param>
        /// <param name="prefabPath">The prefab path of the prefab.</param>
        /// <param name="biomeDistribution">The <see cref="LootDistributionData.BiomeData"/> dictating how the prefab should spawn in the world.</param>
        /// <param name="info">The WorldEntityInfo of the prefab. For more information on how to set this up, see <see cref="WorldEntityDatabaseHandler"/>.</param>
        void AddLootDistributionData(string classId, string prefabPath, IEnumerable<LootDistributionData.BiomeData> biomeDistribution, WorldEntityInfo info);

        /// <summary>
        /// Adds in a custom entry into the Loot Distribution of the game.
        /// You must also add the <see cref="WorldEntityInfo"/> into the <see cref="WorldEntityDatabase"/> using <see cref="WorldEntityDatabaseHandler"/>.
        /// </summary>
        /// <param name="prefab">The custom prefab which you want to spawn naturally in the game.</param>
        /// <param name="biomeDistribution">The <see cref="LootDistributionData.BiomeData"/> dictating how the prefab should spawn in the world.</param>
        /// <param name="info">The WorldEntityInfo of the prefab. For more information on how to set this up, see <see cref="WorldEntityDatabaseHandler"/>.</param>
        void AddLootDistributionData(ModPrefab prefab, IEnumerable<LootDistributionData.BiomeData> biomeDistribution, WorldEntityInfo info);

        /// <summary>
        /// Edits Loot Distribution Data for existing/original class IDs. 
        /// </summary>
        void EditLootDistributionData(string classId, BiomeType biome, float probability, int count);

        /// <summary>
        /// Edits Loot Distribution data for existing prefabs, for e.g. original game prefabs.
        /// </summary>
        /// <param name="classId">The ClassID of the prefab. If unsure, use CraftData.GetClassIdForTechType.</param>
        /// <param name="biomeDistribution">The list of <see cref="LootDistributionData.BiomeData"/> that contains information about how/when it should spawn in biomes.</param>
        void EditLootDistributionData(string classId, IEnumerable<LootDistributionData.BiomeData> biomeDistribution);
    }
}
