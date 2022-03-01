#if SUBNAUTICA
namespace SMLHelper.V2.Interfaces
{
    using Crafting;

    public partial interface ICraftDataHandler
    {
        /// <summary>
        /// <para>Allows you to edit recipes, i.e. TechData for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to edit.</param>
        /// <param name="techData">The TechData for that TechType.</param>
        /// <seealso cref="TechData"/>
        void SetTechData(TechType techType, ITechData techData);

        /// <summary>
        /// <para>Allows you to edit recipes, i.e. TechData for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to edit.</param>
        /// <param name="techData">The TechData for that TechType.</param>
        /// <seealso cref="TechData"/>
        void SetTechData(TechType techType, TechData techData);

        /// <summary>
        /// Safely accesses the crafting data from a modded item.<para/>
        /// WARNING: This method is highly dependent on mod load order. 
        /// Make sure your mod is loading after the mod whose TechData you are trying to access.
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to access.</param>
        /// <returns>The ITechData from the modded item if it exists; Otherwise, returns <c>null</c>.</returns>
        ITechData GetModdedTechData(TechType techType);

        /// <summary>
        /// Safely accesses the crafting data from any item.<para/>
        /// WARNING: This method is highly dependent on mod load order. 
        /// Make sure your mod is loading after the mod whose TechData you are trying to access.
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to access.</param>
        /// <returns>Returns TechData if it exists; Otherwise, returns <c>null</c>.</returns>
        TechData GetTechData(TechType techType);

        /// <summary>
        /// Sets the eating sound for the provided TechType.
        /// </summary>
        /// <param name="consumable">The item being consumed during <see cref="Survival.Eat(UnityEngine.GameObject)"/>.</param>
        /// <param name="soundPath">
        /// The sound path.
        /// <para>
        /// Value values are
        /// - "event:/player/drink"
        /// - "event:/player/drink_stillsuit"
        /// - "event:/player/use_first_aid"
        /// - "event:/player/eat" (default)
        /// </para>
        /// </param>
        void SetEatingSound(TechType consumable, string soundPath);
    }
}
#endif
