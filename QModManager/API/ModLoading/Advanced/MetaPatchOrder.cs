namespace QModManager.API.ModLoading.Advanced
{
    // This enum has been placed into this separate namespace to highlight the fact that it is for advanced used only.

    /// <summary>
    /// <c>WARNING:</c> For advanced users only!<para/>
    /// Use this only if you absolutely know what you are doing.
    /// </summary>
    public enum MetaPatchOrder
    {
        /// <summary>
        /// Informs that the patch method should be executed before <see cref="Normal"/> patch methods.<para/>
        /// <c>WARNING:</c> Do not use unless you absolutely have to.        
        /// </summary>
        Early,

        /// <summary>
        /// Informs that the patch method should be executed at the normal patching time.<para/>
        /// For 99.9% of mods out there, this is what you want to use.
        /// </summary>
        Normal,

        /// <summary>
        /// Informs that the patch method should be executed after <see cref="Normal"/> patch methods.<para/>
        /// <c>WARNING:</c> Do not use unless you absolutely have to.
        /// </summary>
        Late
    }
}
