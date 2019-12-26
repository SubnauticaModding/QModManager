namespace QModManager.Patching
{
    internal enum PatchingOrder
    {
        MetaPreInitialize = -2,

        /// <summary>
        /// For pre-initialize patch methods
        /// </summary>
        PreInitialize = -1,

        /// <summary>
        /// For normal patch methods
        /// </summary>
        NormalInitialize = 0,

        /// <summary>
        /// For post-initialize patch methods
        /// </summary>
        PostInitialize = 1,

        MetaPostInitialize = 2
    }
}
