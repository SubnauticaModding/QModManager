namespace QModManager.API.SMLHelper.Interfaces
{
    using Options;

    public interface IOptionsPanelHandler
    {
        /// <summary>
        /// Registers your mod options to the in-game menu.
        /// </summary>
        /// <param name="options">The mod options. Create a new child class inheriting from this one and add your options to it.</param>
        /// <seealso cref="ModOptions"/>
        void RegisterModOptions(ModOptions options);
    }
}
