namespace SMLHelper.V2.Interfaces
{
#if SUBNAUTICA
    using Sprite = Atlas.Sprite;
#else
    using UnityEngine;
#endif

    /// <summary>
    /// A handler class for everything related to creating new TechGroups.
    /// </summary>
    public interface ITechGroupHandler
    {
        /// <summary>
        /// Adds a new <see cref="TechGroup"/> into the game. 
        /// </summary>
        /// <param name="techGroupName">The name of the TechGroup. Should not contain special characters.</param>
        /// <param name="displayName">The display name of the TechGroup. Can be anything.</param>
        /// <returns>The new <see cref="TechGroup"/> that is created.</returns>
        TechGroup AddTechGroup(string techGroupName, string displayName);

        /// <summary>
        /// Safely looks for a modded item from another mod in the SMLHelper TechGroupCache and outputs its <see cref="TechGroup" /> value when found.
        /// </summary>
        /// <param name="techGroupString">The string used to define the techgroup.</param>
        /// <param name="modTechGroup">The TechGroup enum value of the modded. Defaults to <see cref="TechGroup.Uncategorized" /> when the item was not found.</param>
        /// <returns>
        ///   <c>True</c> if the item was found; Otherwise <c>false</c>.
        /// </returns>
        bool TryGetModdedTechGroup(string techGroupString, out TechGroup modTechGroup);

        /// <summary>
        /// Safely looks for a modded item from another mod in the SMLHelper TechGroupCache.
        /// </summary>
        /// <param name="techGroupString">The string used to define the techgroup.</param>
        /// <returns>
        ///   <c>True</c> if the item was found; Otherwise <c>false</c>.
        /// </returns>
        bool ModdedTechGroupExists(string techGroupString);

    }
}