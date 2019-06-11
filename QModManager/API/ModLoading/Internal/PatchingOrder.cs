namespace QModManager.API.ModLoading.Internal
{
    internal enum PatchingOrder
    {
        /// <summary>
        /// For pre-initialize patch methods
        /// </summary>
        PreInitialize,

        /// <summary>
        /// For normal patch methods
        /// </summary>
        NormalInitialize,

        /// <summary>
        /// For post-initialize patch methods
        /// </summary>
        PostInitialize
    }
}
