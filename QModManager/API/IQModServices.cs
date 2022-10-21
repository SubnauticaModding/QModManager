namespace QModManager.API
{
    using System;
    using System.Reflection;
    using QModManager.Utility;

    /// <summary>
    /// An set of services provided by QModManager for mods to use.
    /// </summary>
    public interface IQModServices : IQModAPI
    {
        /// <summary>
        /// Finds a specific mod by its unique <see cref="IQMod.Id"/> value.
        /// </summary>
        /// <param name="modId">The mod ID.</param>
        /// <returns>The <see cref="IQMod"/> instance of the mod if found; otherwise returns <c>null</c>.</returns>
        IQMod FindModById(string modId);

        /// <summary>
        /// Finds a specific mod with a <see cref="IQMod.LoadedAssembly"/> that matches the provided one.
        /// </summary>
        /// <param name="modAssembly">The mod assembly.</param>
        /// <returns>The <see cref="IQMod"/> instance of the mod if found; otherwise returns <c>null</c>.</returns>
        IQMod FindModByAssembly(Assembly modAssembly);

        /// <summary>
        /// Adds a critical message to the main menu.
        /// Message will stay in the main menu and on the loading screen.
        /// </summary>
        /// <param name="msg">The message to add.</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="autoformat">Whether or not to apply formatting tags to the message, or show it as it is.</param>
        void AddCriticalMessage(string msg, int size = MainMenuMessages.defaultSize, string color = MainMenuMessages.defaultColor, bool autoformat = true);

        /// <summary>
        /// Gets the currently running game.
        /// </summary>
        /// <value>
        /// The currently running game.
        /// </value>
        QModGame CurrentlyRunningGame { get; }

        /// <summary>
        /// Gets a value indicating whether Nitrox is being used.
        /// </summary>
        /// <value>
        ///   <c>true</c> if Nitrox is being used; otherwise, <c>false</c>.
        /// </value>
        bool NitroxRunning { get; }

        /// <summary>
        /// Gets a value indicating whether Piracy was detected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if Piracy was detected; otherwise, <c>false</c>.
        /// </value>
        bool PirateDetected { get; }
        
        /// <summary>
        /// Gets the current Q Mod Manager Version.
        /// </summary>
        /// <value>
        ///   Return Running QMM Version.
        /// </value>
        Version QMMrunningVersion { get; }
        
        /// <summary>
        /// Gets a value indicating when a Savegame was already loaded. (Turns <c>true</c> when entering the Mainmenu again.)
        /// </summary>
        /// <value>
        ///   <c>true</c> Mainmenu is entered AFTER a Savegame was loaded already; otherwise, <c>false</c>.
        /// </value>
        bool AnySavegamewasalreadyloaded { get; }
    }
}