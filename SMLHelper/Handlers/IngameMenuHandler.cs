namespace SMLHelper.V2.Handlers
{
    using System;
    using SMLHelper.V2.Interfaces;
    using SMLHelper.V2.Patchers;

    /// <summary>
    /// A handler class that offers simple ways to tap into functionality of the in game menu.
    /// </summary>
    public class IngameMenuHandler : IIngameMenuHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static IIngameMenuHandler Main { get; } = new IngameMenuHandler();

        private IngameMenuHandler() { } // Hide constructor

        /// <summary>
        /// Registers a simple <see cref="Action"/> method to invoke whenever the player saves the game via the in game menu.
        /// </summary>
        /// <param name="onSaveAction">The method to invoke.</param>
        public static void RegisterOnSaveEvent(Action onSaveAction)
        {
            Main.RegisterOnSaveEvent(onSaveAction);
        }

        /// <summary>
        /// Registers a simple <see cref="Action"/> method to invoke whenever the player loads a saved game via the in game menu.
        /// </summary>
        /// <param name="onLoadAction">The method to invoke.</param>
        public static void RegisterOnLoadEvent(Action onLoadAction)
        {
            Main.RegisterOnLoadEvent(onLoadAction);
        }

        /// <summary>
        /// Registers a simple <see cref="Action"/> method to invoke whenever the player quits the game via the in game menu.
        /// </summary>
        /// <param name="onQuitAction">The method to invoke.</param>
        public static void RegisterOnQuitEvent(Action onQuitAction)
        {
            Main.RegisterOnQuitEvent(onQuitAction);
        }

        /// <summary>
        /// Removes a method previously added through <see cref="RegisterOnSaveEvent(Action)"/> so it is no longer invoked when saving the game.<para/>
        /// If you plan on using this, do not register an anonymous method.
        /// </summary>
        /// <param name="onSaveAction">The method invoked.</param>
        public static void UnregisterOnSaveEvent(Action onSaveAction)
        {
            Main.UnregisterOnSaveEvent(onSaveAction);
        }

        /// <summary>
        /// Removes a method previously added through <see cref="RegisterOnLoadEvent(Action)"/> so it is no longer invoked when loading the game.<para/>
        /// If you plan on using this, do not register an anonymous method.
        /// </summary>
        /// <param name="onLoadAction">The method invoked.</param>
        public static void UnregisterOnLoadEvent(Action onLoadAction)
        {
            Main.UnregisterOnLoadEvent(onLoadAction);
        }

        /// <summary>
        /// Removes a method previously added through <see cref="RegisterOnSaveEvent(Action)"/> so it is no longer invoked when quitting the game.<para/>
        /// If you plan on using this, do not register an anonymous method.
        /// </summary>
        /// <param name="onQuitAction">The method invoked.</param>
        public static void UnregisterOnQuitEvent(Action onQuitAction)
        {
            Main.UnregisterOnQuitEvent(onQuitAction);
        }

        /// <summary>
        /// Registers a simple <see cref="Action"/> method to invoke the <c>first time</c> the player saves the game via the in game menu.
        /// </summary>
        /// <param name="onSaveAction">The method to invoke. This action will not be invoked a second time.</param>
        public static void RegisterOneTimeUseOnSaveEvent(Action onSaveAction)
        {
            Main.RegisterOneTimeUseOnSaveEvent(onSaveAction);
        }

        /// <summary>
        /// Registers a simple <see cref="Action"/> method to invoke the <c>first time</c> the player loads a saved game via the in game menu.
        /// </summary>
        /// <param name="onLoadAction">The method to invoke. This action will not be invoked a second time.</param>
        public static void RegisterOneTimeUseOnLoadEvent(Action onLoadAction)
        {
            Main.RegisterOneTimeUseOnLoadEvent(onLoadAction);
        }

        /// <summary>
        /// Registers a simple <see cref="Action"/> method to invoke the <c>first time</c> the player quits the game via the in game menu.
        /// </summary>
        /// <param name="onQuitAction">The method to invoke. This action will not be invoked a second time.</param>
        public static void RegisterOneTimeUseOnQuitEvent(Action onQuitAction)
        {
            Main.RegisterOneTimeUseOnQuitEvent(onQuitAction);
        }

        /// <summary>
        /// Registers a simple <see cref="Action"/> method to invoke whenever the player saves the game via the in game menu.
        /// </summary>
        /// <param name="onSaveAction">The method to invoke.</param>
        void IIngameMenuHandler.RegisterOnSaveEvent(Action onSaveAction)
        {
            IngameMenuPatcher.OnSaveEvents += onSaveAction;
        }

        void IIngameMenuHandler.RegisterOnLoadEvent(Action onLoadAction)
        {
            IngameMenuPatcher.OnLoadEvents += onLoadAction;
        }

        /// <summary>
        /// Registers a simple <see cref="Action"/> method to invoke whenever the player quits the game via the in game menu.
        /// </summary>
        /// <param name="onQuitAction">The method to invoke.</param>
        void IIngameMenuHandler.RegisterOnQuitEvent(Action onQuitAction)
        {
            IngameMenuPatcher.OnQuitEvents += onQuitAction;
        }

        /// <summary>
        /// Removes a method previously added through <see cref="RegisterOnSaveEvent(Action)"/> so it is no longer invoked when saving the game.<para/>
        /// If you plan on using this, do not register an anonymous method.
        /// </summary>
        /// <param name="onSaveAction">The method invoked.</param>
        void IIngameMenuHandler.UnregisterOnSaveEvent(Action onSaveAction)
        {
            IngameMenuPatcher.OnSaveEvents -= onSaveAction;
        }

        void IIngameMenuHandler.UnregisterOnLoadEvent(Action onLoadAction)
        {
            IngameMenuPatcher.OnLoadEvents -= onLoadAction;
        }

        /// <summary>
        /// Removes a method previously added through <see cref="RegisterOnSaveEvent(Action)"/> so it is no longer invoked when quiting the game.<para/>
        /// If you plan on using this, do not register an anonymous method.
        /// </summary>
        /// <param name="onQuitAction">The method invoked.</param>
        void IIngameMenuHandler.UnregisterOnQuitEvent(Action onQuitAction)
        {
            IngameMenuPatcher.OnQuitEvents -= onQuitAction;
        }

        /// <summary>
        /// Registers a simple <see cref="Action"/> method to invoke the <c>first time</c> the player saves the game via the in game menu.
        /// </summary>
        /// <param name="onSaveAction">The method to invoke. This action will not be invoked a second time.</param>
        void IIngameMenuHandler.RegisterOneTimeUseOnSaveEvent(Action onSaveAction)
        {
            IngameMenuPatcher.AddOneTimeUseSaveEvent(onSaveAction);
        }

        void IIngameMenuHandler.RegisterOneTimeUseOnLoadEvent(Action onLoadAction)
        {
            IngameMenuPatcher.AddOneTimeUseLoadEvent(onLoadAction);
        }

        /// <summary>
        /// Registers a simple <see cref="Action"/> method to invoke the <c>first time</c> the player quits the game via the in game menu.
        /// </summary>
        /// <param name="onQuitAction">The method to invoke. This action will not be invoked a second time.</param>
        void IIngameMenuHandler.RegisterOneTimeUseOnQuitEvent(Action onQuitAction)
        {
            IngameMenuPatcher.AddOneTimeUseQuitEvent(onQuitAction);
        }
    }
}
