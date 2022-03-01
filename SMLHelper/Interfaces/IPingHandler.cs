namespace SMLHelper.V2.Interfaces
{
#if SUBNAUTICA
    using Sprite = Atlas.Sprite;
#elif BELOWZERO
    using Sprite = UnityEngine.Sprite;
#endif
    
    /// <summary>
    /// A handler related to PingTypes
    /// </summary>
    public interface IPingHandler
    {
        /// <summary>
        /// Registers a ping type for use when creating a beacon
        /// </summary>
        /// <param name="pingName">The name of the new ping type</param>
        /// <param name="sprite">The sprite that is associated with the ping</param>
        /// <returns>The newly registered PingType</returns>
        PingType RegisterNewPingType(string pingName, Sprite sprite);

        /// <summary>
        /// Safely looks for a modded ping type in the SMLHelper PingTypeCache and outputs its <see cref="PingType"/> value when found.
        /// </summary>
        /// <param name="pingTypeString">The string used to define the modded PingType</param>
        /// <param name="moddedPingType">The PingType enum value. Defaults to <see cref="PingType.None"/> when the PingType was not found.</param>
        /// <returns><c>True</c> if the PingType was found; Otherwise <c>false</c></returns>
        bool TryGetModdedPingType(string pingTypeString, out PingType moddedPingType);
    }
}
