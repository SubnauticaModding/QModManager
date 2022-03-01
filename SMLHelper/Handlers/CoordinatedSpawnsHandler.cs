using System;
using System.Collections.Generic;
using UnityEngine;
#if SUBNAUTICA_STABLE
using Oculus.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif

namespace SMLHelper.V2.Handlers
{
    using Interfaces;
    using Patchers;

    /// <summary>
    /// a Handler that handles and registers Coordinated (<see cref="Vector3"/> spawns).
    /// </summary>
    public class CoordinatedSpawnsHandler : ICoordinatedSpawnHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static ICoordinatedSpawnHandler Main { get; } = new CoordinatedSpawnsHandler();

        private CoordinatedSpawnsHandler()
        {
            // Hide Constructor
        }

        #region Interface Implementations
        /// <summary>
        /// Registers Multiple Coordinated spawns for one single passed TechType
        /// </summary>
        /// <param name="techTypeToSpawn">The TechType to spawn</param>
        /// <param name="coordinatesToSpawnTo">the coordinates the <see cref="TechType"/> should spawn to</param>
        void ICoordinatedSpawnHandler.RegisterCoordinatedSpawnsForOneTechType(TechType techTypeToSpawn, List<Vector3> coordinatesToSpawnTo)
        {
            var spawnInfos = new List<SpawnInfo>();
            foreach (var coordinate in coordinatesToSpawnTo)
            {
                spawnInfos.Add(new SpawnInfo(techTypeToSpawn, coordinate));
            }
            LargeWorldStreamerPatcher.spawnInfos.AddRange(spawnInfos);
        }

        /// <summary>
        /// Registers a Coordinated Spawn
        /// </summary>
        /// <param name="spawnInfo">the SpawnInfo to spawn</param>
        void ICoordinatedSpawnHandler.RegisterCoordinatedSpawn(SpawnInfo spawnInfo)
        {
            LargeWorldStreamerPatcher.spawnInfos.Add(spawnInfo);
        }

        /// <summary>
        /// registers Many Coordinated Spawns.
        /// </summary>
        /// <param name="spawnInfos">The SpawnInfos to spawn.</param>
        void ICoordinatedSpawnHandler.RegisterCoordinatedSpawns(List<SpawnInfo> spawnInfos)
        {
            LargeWorldStreamerPatcher.spawnInfos.AddRange(spawnInfos);
        }

        /// <summary>
        /// Registers Multiple Coordinated spawns with rotations for one single passed TechType
        /// </summary>
        /// <param name="techTypeToSpawn">The TechType to spawn</param>
        /// <param name="coordinatesAndRotationsToSpawnTo">the coordinates(Key) and the rotations(Value) the <see cref="TechType"/> should spawn to</param>
        void ICoordinatedSpawnHandler.RegisterCoordinatedSpawnsForOneTechType(TechType techTypeToSpawn, Dictionary<Vector3, Vector3> coordinatesAndRotationsToSpawnTo)
        {
            var spawnInfos = new List<SpawnInfo>();
            foreach (var kvp in coordinatesAndRotationsToSpawnTo)
            {
                spawnInfos.Add(new SpawnInfo(techTypeToSpawn, kvp.Key, kvp.Value));
            }
            LargeWorldStreamerPatcher.spawnInfos.AddRange(spawnInfos);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Registers a Coordinated Spawn
        /// </summary>
        /// <param name="spawnInfo">the SpawnInfo to spawn</param>
        public static void RegisterCoordinatedSpawn(SpawnInfo spawnInfo)
        {
            Main.RegisterCoordinatedSpawn(spawnInfo);
        }

        /// <summary>
        /// registers Many Coordinated Spawns.
        /// </summary>
        /// <param name="spawnInfos">The SpawnInfo to spawn.</param>
        public static void RegisterCoordinatedSpawns(List<SpawnInfo> spawnInfos)
        {
            Main.RegisterCoordinatedSpawns(spawnInfos);
        }

        /// <summary>
        /// Registers Multiple Coordinated spawns for one single passed TechType
        /// </summary>
        /// <param name="techTypeToSpawn">The TechType to spawn</param>
        /// <param name="coordinatesToSpawnTo">the coordinates the <see cref="TechType"/> should spawn to</param>
        public static void RegisterCoordinatedSpawnsForOneTechType(TechType techTypeToSpawn, List<Vector3> coordinatesToSpawnTo)
        {
            Main.RegisterCoordinatedSpawnsForOneTechType(techTypeToSpawn, coordinatesToSpawnTo);
        }

        /// <summary>
        /// Registers Multiple Coordinated spawns with rotations for one single passed TechType
        /// </summary>
        /// <param name="techTypeToSpawn">The TechType to spawn</param>
        /// <param name="coordinatesAndRotationsToSpawnTo">the coordinates(Key) and the rotations(Value) the <see cref="TechType"/> should spawn to</param>
        public static void RegisterCoordinatedSpawnsForOneTechType(TechType techTypeToSpawn, Dictionary<Vector3, Vector3> coordinatesAndRotationsToSpawnTo)
        {
            Main.RegisterCoordinatedSpawnsForOneTechType(techTypeToSpawn, coordinatesAndRotationsToSpawnTo);
        }

        #endregion
    }

    #region SpawnInfo
    /// <summary>
    /// A basic struct that provides enough info for the <see cref="CoordinatedSpawnsHandler"/> System to function.
    /// </summary>
    public struct SpawnInfo : IEquatable<SpawnInfo>
    {
        [JsonProperty]
        internal TechType TechType { get; }
        [JsonProperty]
        internal string ClassId { get; }
        [JsonProperty]
        internal Vector3 SpawnPosition { get; }
        [JsonProperty]
        internal Quaternion Rotation { get; }
        [JsonProperty]
        internal SpawnType Type { get; }

        /// <summary>
        /// Initializes a new <see cref="SpawnInfo"/>.
        /// </summary>
        /// <param name="techType">TechType to spawn.</param>
        /// <param name="spawnPosition">Position to spawn into.</param>
        public SpawnInfo(TechType techType, Vector3 spawnPosition)
            : this(default, techType, spawnPosition, Quaternion.identity) { }

        /// <summary>
        /// Initializes a new <see cref="SpawnInfo"/>.
        /// </summary>
        /// <param name="classId">ClassID to spawn.</param>
        /// <param name="spawnPosition">Position to spawn into.</param>
        public SpawnInfo(string classId, Vector3 spawnPosition)
            : this(classId, default, spawnPosition, Quaternion.identity) { }

        /// <summary>
        /// Initializes a new <see cref="SpawnInfo"/>.
        /// </summary>
        /// <param name="techType">TechType to spawn.</param>
        /// <param name="spawnPosition">Position to spawn into.</param>
        /// <param name="rotation">Rotation to spawn at.</param>
        public SpawnInfo(TechType techType, Vector3 spawnPosition, Quaternion rotation)
            : this(default, techType, spawnPosition, rotation) { }

        /// <summary>
        /// Initializes a new <see cref="SpawnInfo"/>.
        /// </summary>
        /// <param name="classId">ClassID to spawn.</param>
        /// <param name="spawnPosition">Position to spawn into.</param>
        /// <param name="rotation">Rotation to spawn at.</param>
        public SpawnInfo(string classId, Vector3 spawnPosition, Quaternion rotation)
            : this(classId, default, spawnPosition, rotation) { }

        /// <summary>
        /// Initializes a new <see cref="SpawnInfo"/>.
        /// </summary>
        /// <param name="techType">TechType to spawn.</param>
        /// <param name="spawnPosition">Position to spawn into.</param>
        /// <param name="rotation">Rotation to spawn at.</param>
        public SpawnInfo(TechType techType, Vector3 spawnPosition, Vector3 rotation)
            : this(default, techType, spawnPosition, Quaternion.Euler(rotation)) { }

        /// <summary>
        /// Initializes a new <see cref="SpawnInfo"/>.
        /// </summary>
        /// <param name="classId">ClassID to spawn.</param>
        /// <param name="spawnPosition">Position to spawn into.</param>
        /// <param name="rotation">Rotation to spawn at.</param>
        public SpawnInfo(string classId, Vector3 spawnPosition, Vector3 rotation)
            : this(classId, default, spawnPosition, Quaternion.Euler(rotation)) { }

        [JsonConstructor]
        internal SpawnInfo(string classId, TechType techType, Vector3 spawnPosition, Quaternion rotation)
        {
            ClassId = classId;
            TechType = techType;
            SpawnPosition = spawnPosition;
            Rotation = rotation;
            Type = TechType switch
            {
                default(TechType) => SpawnType.ClassId,
                _ => SpawnType.TechType
            };
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <remarks>
        /// It is worth noting that we use Unity's <see cref="Vector3.operator=="/> and <see cref="Quaternion.operator=="/>
        /// operator comparisons for comparing the <see cref="SpawnPosition"/> and <see cref="Rotation"/> properties of each instance, 
        /// to allow for an approximate comparison of these values.
        /// </remarks>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="SpawnInfo"/> and represents the same
        /// value as this instance; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="Equals(SpawnInfo)"/>
        public override bool Equals(object obj) => obj is SpawnInfo spawnInfo && Equals(spawnInfo);

        /// <summary>
        /// A custom hash code algorithm that takes into account the values of each property of the <see cref="SpawnInfo"/> instance,
        /// and attempts to reduce diagonal collisions.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            unchecked // overflow is fine, just wrap around
            {
                int hash = 13;
                hash = (hash * 7) + TechType.GetHashCode();
                hash = (hash * 7) + ClassId.GetHashCode();
                hash = (hash * 7) + SpawnPosition.GetHashCode();
                hash = (hash * 7) + Rotation.GetHashCode();
                hash = (hash * 7) + Type.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Indicates whether the current <see cref="SpawnInfo"/> is equal to another.
        /// </summary>
        /// <remarks>
        /// It is worth noting that we use Unity's <see cref="Vector3.operator=="/> and <see cref="Quaternion.operator=="/>
        /// operator comparisons for comparing the <see cref="SpawnPosition"/> and <see cref="Rotation"/> properties of each instance, 
        /// to allow for an approximate comparison of these values.
        /// </remarks>
        /// <param name="other">The other <see cref="SpawnInfo"/>.</param>
        /// <returns><see langword="true"/> if the current <see cref="SpawnInfo"/> is equal to the <paramref name="other"/> parameter;
        /// otherwise <see langword="false"/>.</returns>
        public bool Equals(SpawnInfo other) => other.TechType == TechType
                                            && other.ClassId == ClassId
                                            && other.SpawnPosition == SpawnPosition
                                            && other.Rotation == Rotation
                                            && other.Type == Type;

        internal enum SpawnType
        {
            ClassId,
            TechType
        }

        /// <summary>
        /// Indicates whether two <see cref="SpawnInfo"/> instances are equal.
        /// </summary>
        /// <param name="a">The first instance to compare.</param>
        /// <param name="b">The second instance to compare.</param>
        /// <returns><see langword="true"/> if the <see cref="SpawnInfo"/> instances are equal; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="operator!="/>
        /// <seealso cref="Equals(SpawnInfo)"/>
        public static bool operator ==(SpawnInfo a, SpawnInfo b) => a.Equals(b);

        /// <summary>
        /// Indicates whether two <see cref="SpawnInfo"/> instances are not equal.
        /// </summary>
        /// <param name="a">The first instance to compare.</param>
        /// <param name="b">The second instance to compare.</param>
        /// <returns><see langword="true"/> if the <see cref="SpawnInfo"/> instances are not equal; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="operator=="/>
        /// <seealso cref="Equals(SpawnInfo)"/>
        public static bool operator !=(SpawnInfo a, SpawnInfo b) => !(a == b);
    }
    #endregion
}
