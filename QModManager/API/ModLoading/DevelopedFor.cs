namespace QModManager.API.ModLoading
{
    using System;

    /// <summary>
    /// Defines what game a QMod can run on.
    /// </summary>
    [Flags]
    public enum DevelopedFor
    {
        /// <summary>
        /// Mod developed exclusively for Subnautica
        /// </summary>
        Subnautica = Patcher.Game.Subnautica,

        /// <summary>
        /// Mod developed exclusively for Below Zero
        /// </summary>
        BelowZero = Patcher.Game.BelowZero,

        /// <summary>
        /// Mod that can be loaded on both Subnautica and Below Zero
        /// </summary>
        BothGames = Patcher.Game.Both,
    }
}
