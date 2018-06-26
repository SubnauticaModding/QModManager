using SMLHelper.V2.Patchers;

namespace SMLHelper.V2.Handlers
{
    public class BaseBioReactorHandler
    {
        #region Core Methods

        /// <summary>
        /// <para>Allows you to specify the quantity of energy that a TechType will produce with bio reactors.</para>
        /// </summary>
        /// <param name="techType">The TechType that you want to use with bioreactors.</param>
        /// <param name="charge">The quantity of energy that will be produced by this TechType.</param>
        /// <seealso cref="CraftData.BackgroundType"/>
        public static void EditBioReactorCharge(TechType techType, float charge)
        {
            BaseBioReactorPatcher.CustomBioreactorCharges[techType] = charge;
        }

        #endregion

        // Typically, when adding custom items, other modders will likely be looking for "Add" methods without realising that the "Edit" methods above also add.
        // This set of methods below is here to to address the naming expectations without altering actual functionality.
        #region Redundant but friendly

        /// <summary>
        /// <para>Allows you to make a TechType compatible with bio reactors.</para>
        /// </summary>
        /// <param name="techType">The TechType that you want to use with bioreactors.</param>
        /// <param name="charge">The quantity of energy that will be produced by this TechType.</param>
        /// <seealso cref="CraftData.BackgroundType"/>
        public static void AddBioReactorCharge(TechType techType, float charge) => EditBioReactorCharge(techType, charge);

        #endregion
    }
}
