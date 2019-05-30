using System;

namespace QModInstaller
{
    /// <summary>
    /// Container class for the entry point <para/>
    /// NOT FOR MANUAL USAGE!
    /// </summary>
    [Obsolete("NOT FOR MANUAL USAGE!", true)]
    public class QModPatcher
    {
        /// <summary>
        /// QModManager entry point <para/>
        /// NOT FOR MANUAL USAGE!
        /// </summary>
        [Obsolete("NOT FOR MANUAL USAGE!", true)]
        public static void Patch() => QModManager.Patcher.Main(new string[0]);
    }
}