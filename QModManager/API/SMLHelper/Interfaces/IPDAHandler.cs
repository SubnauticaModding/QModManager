namespace QModManager.API.SMLHelper.Interfaces
{
    using Handlers;

    /// <summary>
    /// Interface for <see cref="PDAHandler"/> <para/>
    /// Can be used for dependency injection
    /// </summary>
    public interface IPDAHandler
    {
        /// <summary>
        /// Edits how many fragments must be scanned before unlocking the techtype's blueprint.
        /// </summary>
        /// <param name="techType">Can be either techtype of the fragment or the crafted item.</param>
        /// <param name="fragmentCount">The number of fragments to scan.</param>
        void EditFragmentsToScan(TechType techType, int fragmentCount);

        /// <summary>
        /// Edits the time it takes to finish scanning a fragment.
        /// </summary>
        /// <param name="techType">Can be either techtype of the fragment or the crafted item.</param>
        /// <param name="scanTime">The relative time spent on scanning. Default value is 1.</param>
        void EditFragmentScanTime(TechType techType, float scanTime);
    }
}
