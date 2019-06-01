namespace QModManager.API.ModLoading.Internal
{
    /// <summary>
    /// FOR INTERNAL USE ONLY.
    /// </summary>
    public enum PatchingOrder
    {
        /// <summary>
        /// For pre-initialize patch methods.
        /// </summary>
        PreInitialize,

        /// <summary>
        /// For normal patch methods.
        /// </summary>
        NormalInitialize,

        /// <summary>
        /// For post-initialize patch methods.
        /// </summary>
        PostInitialize
    }
}
