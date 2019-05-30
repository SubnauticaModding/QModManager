namespace QModManager.API.SMLHelper.Handlers
{
    using Options;
    using Patchers;
    using Interfaces;

    /// <summary>
    /// A handler class for registering your custom in-game mod options.
    /// </summary>
    public class OptionsPanelHandler : IOptionsPanelHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static IOptionsPanelHandler Main { get; } = new OptionsPanelHandler();

        private OptionsPanelHandler() { }

        /// <summary>
        /// Registers your mod options to the in-game menu.
        /// </summary>
        /// <param name="options">The mod options. Create a new child class inheriting from this one and add your options to it.</param>
        /// <seealso cref="ModOptions"/>
        public static void RegisterModOptions(ModOptions options)
        {
            Main.RegisterModOptions(options);
        }

        /// <summary>
        /// Registers your mod options to the in-game menu.
        /// </summary>
        /// <param name="options">The mod options. Create a new child class inheriting from this one and add your options to it.</param>
        /// <seealso cref="ModOptions"/>
        void IOptionsPanelHandler.RegisterModOptions(ModOptions options)
        {
            OptionsPanelPatcher.modOptions.Add(options.Name, options);
        }
    }
}
