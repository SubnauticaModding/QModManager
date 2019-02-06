namespace SMLHelper.V2.Handlers
{
    using SMLHelper.V2.Patchers;

    /// <summary>
    /// A handler class for various scanner related data.
    /// </summary>
    public static class PdaHandler
    {

        /// <summary>
        /// Edits how many fragments must be scanned before unlocking the techtype's blueprint.
        /// </summary>
        /// <param name="techType">Can be either techtype of the fragment or the crafted item.</param>
        /// <param name="fragmentCount">The number of fragments to scan.</param>
        public static void EditFragmentsToScan(TechType techType, int fragmentCount)
        {
            if (fragmentCount <= 0)
            {
                fragmentCount = 1;
            }

            PdaPatcher.FragmentCount[techType] = fragmentCount;
        }

        /// <summary>
        /// Edits the time it takes to finish scanning a fragment.
        /// </summary>
        /// <param name="techType">Can be either techtype of the fragment or the crafted item.</param>
        /// <param name="scanTime">The relative time spent on scanning. Default value is 1.</param>
        public static void EditFragmentScanTime(TechType techType, float scanTime)
        {
            if (scanTime <= 0f)
            {
                scanTime = 1f;
            }

            PdaPatcher.FragmentScanTime[techType] = scanTime;
        }
    }
}
