using System;

namespace QModInstaller
{
    /// <summary>
    /// Container class for the entry point
    /// </summary>
    [Obsolete("Should not be used!", true)]
    public class QModPatcher
    {
        /// <summary>
        /// QModManager entry point
        /// </summary>
        [Obsolete("Should not be used!", true)]
        public static void Patch() => QModManager.Patcher.Patch();
    }
}