namespace SMLHelper.V2.Handlers
{
    using Interfaces;
    using Patchers;

    /// <summary>
    /// A handler class for various scanner related data.
    /// </summary>
    public class PDAHandler : IPDAHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static IPDAHandler Main { get; } = new PDAHandler();

        private PDAHandler()
        {
            // Hide constructor
        }

        /// <summary>
        /// Edits how many fragments must be scanned before unlocking the techtype's blueprint.
        /// </summary>
        /// <param name="techType">Can be either techtype of the fragment or the crafted item.</param>
        /// <param name="fragmentCount">The number of fragments to scan.</param>
        void IPDAHandler.EditFragmentsToScan(TechType techType, int fragmentCount)
        {
            if (fragmentCount <= 0)
            {
                fragmentCount = 1;
            }

            PDAPatcher.FragmentCount[techType] = fragmentCount;
        }

        /// <summary>
        /// Edits the time it takes to finish scanning a fragment.
        /// </summary>
        /// <param name="techType">Can be either techtype of the fragment or the crafted item.</param>
        /// <param name="scanTime">The relative time spent on scanning. Default value is 1.</param>
        void IPDAHandler.EditFragmentScanTime(TechType techType, float scanTime)
        {
            if (scanTime <= 0f)
            {
                scanTime = 1f;
            }

            PDAPatcher.FragmentScanTime[techType] = scanTime;
        }

        /// <summary>
        /// Adds in a custom <see cref="PDAScanner.EntryData"/>. ***Cannot be used to Change the values of a techtype that has data already!***
        /// </summary>
        /// <param name="entryData">The <see cref="PDAScanner.EntryData"/> of the entry. Must be populated when passed in.</param>
        void IPDAHandler.AddCustomScannerEntry(PDAScanner.EntryData entryData)
        {
            if (PDAPatcher.CustomEntryData.ContainsKey(entryData.key))
                Logger.Log($"{entryData.key} already has custom PDAScanner.EntryData. Replacing with latest.", LogLevel.Debug);

            PDAPatcher.CustomEntryData[entryData.key] = entryData;
        }

        /// <summary>
        /// Edits how many fragments must be scanned before unlocking the techtype's blueprint.
        /// </summary>
        /// <param name="techType">Can be either techtype of the fragment or the crafted item.</param>
        /// <param name="fragmentCount">The number of fragments to scan.</param>
        public static void EditFragmentsToScan(TechType techType, int fragmentCount)
        {
            Main.EditFragmentsToScan(techType, fragmentCount);
        }

        /// <summary>
        /// Edits the time it takes to finish scanning a fragment.
        /// </summary>
        /// <param name="techType">Can be either techtype of the fragment or the crafted item.</param>
        /// <param name="scanTime">The relative time spent on scanning. Default value is 1.</param>
        public static void EditFragmentScanTime(TechType techType, float scanTime)
        {
            Main.EditFragmentScanTime(techType, scanTime);
        }

        /// <summary>
        /// Adds in a custom <see cref="PDAScanner.EntryData"/>.
        /// </summary>
        /// <param name="entryData">The <see cref="PDAScanner.EntryData"/> of the entry. Must be populated when passed in.</param>
        public static void AddCustomScannerEntry(PDAScanner.EntryData entryData)
        {
            Main.AddCustomScannerEntry(entryData);
        }

        /// <summary>
        /// Adds in a custom <see cref="PDAScanner.EntryData"/>.
        /// </summary>
        /// <param name="key">The scanned object's <see cref="TechType"/>. In case of fragments, the fragment <see cref="TechType"/> is the key.</param>
        /// <param name="blueprint">The <paramref name="blueprint"/> when unlocked when scanned. In case of fragments, this is the actual <see cref="TechType"/> that unlocks when all fragments are scanned.</param>
        /// <param name="isFragment">Whether the <paramref name="key"/> is a fragment or not.</param>
        /// <param name="totalFragmentsRequired">The total amount of objects of <paramref name="key"/> that need to be scanned to unlock the <paramref name="blueprint"/> and <paramref name="encyclopediaKey"/>.</param>
        /// <param name="scanTime">The amount of time it takes to finish one scan. In seconds.</param>
        /// <param name="destroyAfterScan">Whether the object should be destroyed after the scan is finished.</param>
        /// <param name="encyclopediaKey">The key to the encyclopedia entry.</param>
        public static void AddCustomScannerEntry(TechType key, TechType blueprint, bool isFragment, string encyclopediaKey, int totalFragmentsRequired = 2, float scanTime = 2f, bool destroyAfterScan = true)
        {
            Main.AddCustomScannerEntry(new PDAScanner.EntryData()
            {
                key = key,
                blueprint = blueprint,
                isFragment = isFragment,
                totalFragments = totalFragmentsRequired,
                scanTime = scanTime,
                destroyAfterScan = destroyAfterScan
            });
        }
    }
}
