namespace SMLHelper.V2.Handlers
{
    using Options;
    using Patchers;

    /// <summary>
    /// A handler class for registering your custom in-game mod options.
    /// </summary>
    public class OptionsPanelHandler
    {
        /// <summary>
        /// Registers your mod options to the in-game menu.
        /// </summary>
        /// <param name="options">The mod options. Create a new child class inheriting from this one and add your options to it.</param>
        /// <seealso cref="ModOptions"/>
        public static void RegisterModOptions(ModOptions options)
        {
            OptionsPanelPatcher.modOptions.Add(options.Name, options);
        }
    }
}
