namespace SMLHelper.V2.Interfaces
{
    /// <summary>
    /// A handler class for everything related to creating new TechCategorys.
    /// </summary>
    public interface ITechCategoryHandler
    {
        /// <summary>
        /// Adds a new <see cref="TechCategory"/> into the game. 
        /// </summary>
        /// <param name="techCatagoryName">The name of the TechCategory. Should not contain special characters.</param>
        /// <param name="displayName">The display name of the TechCategory. Can be anything.</param>
        /// <returns>The new <see cref="TechCategory"/> that is created.</returns>
        TechCategory AddTechCategory(string techCatagoryName, string displayName);

        /// <summary>
        /// Safely looks for a modded item from another mod in the SMLHelper TechCategoryCache and outputs its <see cref="TechCategory" /> value when found.
        /// </summary>
        /// <param name="techCategoryString">The string used to define the techcategory.</param>
        /// <param name="modTechCategory">The TechCategory enum value of the modded. Defaults to <see cref="TechCategory.Misc" /> when the item was not found.</param>
        /// <returns>
        ///   <c>True</c> if the item was found; Otherwise <c>false</c>.
        /// </returns>
        bool TryGetModdedTechCategory(string techCategoryString, out TechCategory modTechCategory);

        /// <summary>
        /// Safely looks for a modded item from another mod in the SMLHelper TechCategoryCache.
        /// </summary>
        /// <param name="techCategoryString">The string used to define the techcategory.</param>
        /// <returns>
        ///   <c>True</c> if the item was found; Otherwise <c>false</c>.
        /// </returns>
        bool ModdedTechCategoryExists(string techCategoryString);

        /// <summary>
        /// Registers the TechCategory to a TechGroup.
        /// </summary>
        /// <param name="techGroup">The tech group.</param>
        /// <param name="techCategory">The tech category.</param>
        /// <returns></returns>
        bool TryRegisterTechCategoryToTechGroup(TechGroup techGroup, TechCategory techCategory);
    }
}