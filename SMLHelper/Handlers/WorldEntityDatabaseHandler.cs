namespace SMLHelper.V2.Handlers
{
    using Interfaces;
    using SMLHelper.V2.Patchers;
    using UnityEngine;
    using UWE;

    /// <summary>
    /// A handler for the WorldEntityDatabase of the game.
    /// </summary>
    public class WorldEntityDatabaseHandler : IWorldEntityDatabaseHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static IWorldEntityDatabaseHandler Main { get; } = new WorldEntityDatabaseHandler();

        private WorldEntityDatabaseHandler() { } // Hides constructor

        /// <summary>
        /// Adds in a custom <see cref="WorldEntityInfo"/> to the <see cref="WorldEntityDatabase"/> of the game.
        /// It contains information about the entity, like its <see cref="LargeWorldEntity.CellLevel"/>, its <see cref="EntitySlot.Type"/>, etc.
        /// </summary>
        /// <param name="classId">The classID of the entity whose data you are adding in.</param>
        /// <param name="data">The <see cref="WorldEntityInfo"/> data. Data is stored in the fields of the class, so they must be populated when passed in.</param>
        public static void AddCustomInfo(string classId, WorldEntityInfo data)
        {

            Main.AddCustomInfo(classId, data);
        }

        /// <summary>
        /// Adds in a custom <see cref="WorldEntityInfo"/> to the <see cref="WorldEntityDatabase"/> of the game.
        /// It contains information about the entity, like its <see cref="LargeWorldEntity.CellLevel"/>, its <see cref="EntitySlotData.EntitySlotType"/>, etc.
        /// </summary>
        /// <param name="classId">The classId of the entity.</param>
        /// <param name="techType">The <see cref="TechType"/> of the entity.</param>
        /// <param name="prefabZUp">Whether the prefab's Z-axis should be facing up, when spawned.</param>
        /// <param name="cellLevel">The <see cref="LargeWorldEntity.CellLevel"/> of the entity.</param>
        /// <param name="slotType">The <see cref="EntitySlot.Type"/> of the entity. Dictates which "slots" are suitable for this entity to spawn in. For e.g., most in-crate fragments have a <see cref="EntitySlot.Type.Small"/> slot type.</param>
        /// <param name="localScale">The scale that the entity's local scale is set to when spawned.</param>
        public static void AddCustomInfo(string classId, TechType techType, Vector3 localScale, bool prefabZUp = false, LargeWorldEntity.CellLevel cellLevel = LargeWorldEntity.CellLevel.Global, EntitySlot.Type slotType = EntitySlot.Type.Small)
        {
            Main.AddCustomInfo(classId, new WorldEntityInfo()
            {
                classId = classId,
                techType = techType,
                cellLevel = cellLevel,
                slotType = slotType,
                localScale = localScale,
                prefabZUp = prefabZUp
            });
        }

        void IWorldEntityDatabaseHandler.AddCustomInfo(string classId, WorldEntityInfo data)
        {
            if(WorldEntityDatabasePatcher.CustomWorldEntityInfos.ContainsKey(classId))
                V2.Logger.Log($"{classId}-{data.techType} already has custom WorldEntityInfo. Replacing with latest.", LogLevel.Debug);

            WorldEntityDatabasePatcher.CustomWorldEntityInfos[classId] = data;
        }
    }
}
