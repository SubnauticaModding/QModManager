namespace SMLHelper.V2.Interfaces
{
    /// <summary>
    /// A handler related to EquipmentTypes
    /// </summary>
    public interface IEquipmentHandler
    {
        /// <summary>
        /// Registers an equipment type for use when creating a equipment
        /// </summary>
        /// <param name="equipmentName">The name of the new equipment type</param>
        /// <returns>The newly registered EquipmentType</returns>
        EquipmentType AddEquipmentType(string equipmentName);

        /// <summary>
        /// Safely looks for a modded equipment type in the SMLHelper EquipmentTypeCache and outputs its <see cref="EquipmentType"/> value when found.
        /// </summary>
        /// <param name="equipmentTypeString">The string used to define the modded EquipmentType</param>
        /// <param name="moddedEquipmentType">The EquipmentType enum value. Defaults to <see cref="EquipmentType.None"/> when the EquipmentType was not found.</param>
        /// <returns><c>True</c> if the EquipmentType was found; Otherwise <c>false</c></returns>
        bool TryGetModdedEquipmentType(string equipmentTypeString, out EquipmentType moddedEquipmentType);

        /// <summary>
        ///  Checks for the existence of an added EquipmentType in the SMLHelper EquipmentTypeCache and outputs the result of the check
        /// </summary>
        /// <param name="equipmentString">The string used to define the modded EquipmentType</param>
        /// <returns><c>True</c> if the EquipmentType was found; Otherwise <c>false</c></returns>
        public bool ModdedEquipmentTypeExists(string equipmentString);
    }
}
