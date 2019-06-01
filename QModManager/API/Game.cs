namespace QModManager.API
{
    using System;

    /// <summary>
    /// Identifies the Subnautica games.
    /// </summary>
    [Flags]
    public enum Game
    {
        /// <summary>
        /// No game.
        /// </summary>
        None = 0b00,

        /// <summary>
        /// Subnautica.
        /// </summary>
        Subnautica = 0b01,

        /// <summary>
        /// Subnautica: Below.
        /// </summary>
        BelowZero = 0b10,

        /// <summary>
        /// Both Subnautica and Below Zero.
        /// </summary>
        Both = Subnautica | BelowZero,
    }
}
