namespace SMLHelper.V2
{
    using Interfaces;

    /// <summary>
    /// A simple location where every SMLHelper handler class can be accessed.
    /// </summary>
    public static class Handler
    {
        /// <summary>
        /// A handler with common methods for updating BioReactor values.
        /// </summary>
        public static IBioReactorHandler BioReactorHandler => Handlers.BioReactorHandler.Main;

        /// <summary>
        /// A handler class for adding and editing crafted items.
        /// </summary>
        public static ICraftDataHandler CraftDataHandler => Handlers.CraftDataHandler.Main;

        /// <summary>
        /// A handler class for creating and editing of crafting trees.
        /// </summary>
        public static ICraftTreeHandler CraftTreeHandler => Handlers.CraftTreeHandler.Main;

        /// <summary>
        /// A handler class that offers simple ways to tap into functionality of the in game menu.
        /// </summary>
        public static IIngameMenuHandler IngameMenuHandler => Handlers.IngameMenuHandler.Main;

        /// <summary>
        /// Class to manage registering of fish into the game
        /// </summary>
        public static IFishHandler FishHandler => Handlers.FishHandler.Main;

        /// <summary>
        /// A handler class for registering your custom middle click actions for items
        /// </summary>
        public static IItemActionHandler ItemActionHandler => Handlers.ItemActionHandler.Main;

        /// <summary>
        /// A handler class for configuring custom unlocking conditions for item blueprints.
        /// </summary>
        public static IKnownTechHandler KnownTechHandler => Handlers.KnownTechHandler.Main;

        /// <summary>
        /// A handler for adding language lines.
        /// </summary>
        public static ILanguageHandler LanguageHandler => Handlers.LanguageHandler.Main;

        /// <summary>
        /// A handler class for registering your custom in-game mod options.
        /// </summary>
        public static IOptionsPanelHandler OptionsPanelHandler => Handlers.OptionsPanelHandler.Main;

        /// <summary>
        /// A handler class for various scanner related data.
        /// </summary>
        public static IPDAHandler PDAHandler => Handlers.PDAHandler.Main;

        /// <summary>
        /// A handler class for adding custom sprites into the game.
        /// </summary>
        public static ISpriteHandler SpriteHandler => Handlers.SpriteHandler.Main;

        /// <summary>
        /// A handler class for everything related to creating new TechTypes.
        /// </summary>
        public static ITechTypeHandler TechTypeHandler => Handlers.TechTypeHandler.Main;

        /// <summary>
        /// A handler class for adding and editing resource spawns.
        /// </summary>
        public static ILootDistributionHandler LootDistributionHandler => Handlers.LootDistributionHandler.Main;

        /// <summary>
        /// A handler for adding custom entries to the world entity database.
        /// </summary>
        public static IWorldEntityDatabaseHandler WorldEntityDatabaseHandler => Handlers.WorldEntityDatabaseHandler.Main;

        /// <summary>
        /// A handler for adding custom entries to the PDA Encyclopedia.
        /// </summary>
        public static IPDAEncyclopediaHandler PDAEncyclopediaHandler => Handlers.PDAEncyclopediaHandler.Main;

        /// <summary>
        /// A handler for registering Unity prefabs associated to a <see cref="TechType"/>.
        /// </summary>
        public static IPrefabHandler PrefabHandler => Handlers.PrefabHandler.Main;

        /// <summary>
        /// a handler for common uses to the Survival component
        /// </summary>
        public static ISurvivalHandler SurvivalHandler => Handlers.SurvivalHandler.Main;

        /// <summary>
        /// A handler to making coordinated Vector3 spawns ingame.
        /// </summary>
        public static ICoordinatedSpawnHandler CoordinatedSpawnHandler => Handlers.CoordinatedSpawnsHandler.Main;

        /// <summary>
        /// A handler class for adding custom TechGroups into the game.
        /// </summary>
        public static ITechGroupHandler TechGroupHandler => Handlers.TechGroupHandler.Main;

        /// <summary>
        /// A handler class for adding custom TechCategories into the game.
        /// </summary>
        public static ITechCategoryHandler TechCategoryHandler => Handlers.TechCategoryHandler.Main;

        /// <summary>
        /// A handler class for registering your custom console commands.
        /// </summary>
        public static IConsoleCommandHandler ConsoleCommandsHandler => Handlers.ConsoleCommandsHandler.Main;

        /// <summary>
        /// A handler related to PingTypes
        /// </summary>
        public static IPingHandler PingHandler => Handlers.PingHandler.Main;

        /// <summary>
        /// A handler for everything related to creating new BackgroundTypes.
        /// </summary>
        public static IBackgroundTypeHandler BackgroundTypeHandler => Handlers.BackgroundTypeHandler.Main;
        
        /// <summary>
        /// A handler related to Custom Sounds
        /// </summary>
        public static ICustomSoundHandler SoundHandler => Handlers.CustomSoundHandler.Main;

        /// <summary>
        /// A handler for adding custom EquipmentTypes into the game.
        /// </summary>
        public static IEquipmentHandler EquipmentHandler => Handlers.EquipmentHandler.Main;
    }
}
