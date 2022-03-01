namespace SMLHelper.V2.Handlers
{
    using Options;
    using Options.Attributes;
    using Patchers;
    using Interfaces;
    using Json;
    using System.Reflection;

    /// <summary>
    /// A handler class for registering your custom in-game mod options.
    /// </summary>
    public class OptionsPanelHandler : IOptionsPanelHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static IOptionsPanelHandler Main { get; } = new OptionsPanelHandler();

        private OptionsPanelHandler()
        {
            // Hide constructor
        }

        /// <summary>
        /// Registers your mod options to the in-game menu.
        /// </summary>
        /// <param name="options">The mod options. Create a new child class inheriting from this one
        /// and add your options to it.</param>
        /// <seealso cref="ModOptions"/>
        public static void RegisterModOptions(ModOptions options)
        {
            Main.RegisterModOptions(options);
        }

        /// <summary>
        /// Registers your mod options to the in-game menu.
        /// </summary>
        /// <param name="options">The mod options. Create a new child class inheriting from this one
        /// and add your options to it.</param>
        /// <seealso cref="ModOptions"/>
        void IOptionsPanelHandler.RegisterModOptions(ModOptions options)
        {
            OptionsPanelPatcher.modOptions.Add(options.Name, options);
        }

        /// <summary>
        /// Generates an options menu based on the attributes and members declared in the <see cref="ConfigFile"/>
        /// and registers it to the in-game menu.
        /// </summary>
        /// <typeparam name="T">A class derived from <see cref="ConfigFile"/> to generate the options menu from.</typeparam>
        /// <returns>An instance of the <typeparamref name="T"/> : <see cref="ConfigFile"/> with values loaded
        /// from the config file on disk.</returns>
        public static T RegisterModOptions<T>() where T : ConfigFile, new()
            => Main.RegisterModOptions<T>();

        /// <summary>
        /// Generates an options menu based on the attributes and members declared in the <see cref="ConfigFile"/>
        /// and registers it to the in-game menu.
        /// </summary>
        /// <typeparam name="T">A class derived from <see cref="ConfigFile"/> to generate the options menu from.</typeparam>
        /// <returns>An instance of the <typeparamref name="T"/> : <see cref="ConfigFile"/> with values loaded
        /// from the config file on disk.</returns>
        T IOptionsPanelHandler.RegisterModOptions<T>()
        {
            var optionsMenuBuilder = new OptionsMenuBuilder<T>();
            RegisterModOptions(optionsMenuBuilder);
            optionsMenuBuilder.ConfigFileMetadata.Registered = true;

            var menuAttribute = typeof(T).GetCustomAttribute<MenuAttribute>(true)
                ?? new MenuAttribute(optionsMenuBuilder.Name);

            if (menuAttribute.SaveOn.HasFlag(MenuAttribute.SaveEvents.SaveGame))
                IngameMenuHandler.RegisterOnSaveEvent(() => optionsMenuBuilder.ConfigFileMetadata.Config.Save());

            if (menuAttribute.SaveOn.HasFlag(MenuAttribute.SaveEvents.QuitGame))
                IngameMenuHandler.RegisterOnQuitEvent(() => optionsMenuBuilder.ConfigFileMetadata.Config.Save());

            return optionsMenuBuilder.ConfigFileMetadata.Config;
        }
    }
}
