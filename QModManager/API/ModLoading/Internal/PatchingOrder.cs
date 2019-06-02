namespace QModManager.API.ModLoading.Internal
{
    /// <summary>
    /// FOR INTERNAL USE ONLY.
    /// </summary>
    public enum PatchingOrder
    {
        /// <summary>
        /// For pre-initialize patch methods. FOR INTERNAL USE ONLY.
        /// </summary>
        PreInitialize,

        /// <summary>
        /// For normal patch methods. FOR INTERNAL USE ONLY.
        /// </summary>
        NormalInitialize,

        /// <summary>
        /// For post-initialize patch methods. FOR INTERNAL USE ONLY.
        /// </summary>
        PostInitialize
    }
}
