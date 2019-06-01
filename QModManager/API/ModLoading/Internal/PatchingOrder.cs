namespace QModManager.API.ModLoading.Internal
{
    internal enum PatchingOrder
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
