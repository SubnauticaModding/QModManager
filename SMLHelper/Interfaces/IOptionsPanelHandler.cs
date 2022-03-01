namespace SMLHelper.V2.Interfaces
{
    using Options;
    using Json;

    /// <summary>
    /// A handler class for registering your custom in-game mod options.
    /// </summary>
    public interface IOptionsPanelHandler
    {
        /// <summary>
        /// Registers your mod options to the in-game menu.
        /// </summary>
        /// <param name="options">The mod options. Create a new child class inheriting from this one
        /// and add your options to it.</param>
        /// <seealso cref="ModOptions"/>
        void RegisterModOptions(ModOptions options);

        /// <summary>
        /// Generates an options menu based on the attributes and members declared in the <see cref="ConfigFile"/>
        /// and registers it to the in-game menu.
        /// </summary>
        /// <typeparam name="T">A class derived from <see cref="ConfigFile"/> to generate the options menu from.</typeparam>
        /// <returns>An instance of the <typeparamref name="T"/> : <see cref="ConfigFile"/> with values loaded
        /// from the config file on disk.</returns>
        T RegisterModOptions<T>() where T : ConfigFile, new();
    }
}
