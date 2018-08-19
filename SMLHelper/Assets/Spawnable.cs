namespace SMLHelper.V2.Assets
{
    using System.Reflection;
    using Assets;
    using Handlers;

    /// <summary>
    /// An item that can be spawned into the game.
    /// </summary>
    /// <seealso cref="ModPrefab"/>
    public abstract class Spawnable : ModPrefab
    {
        /// <summary>
        /// A simple delegate type that takes no parameters and returns void.
        /// </summary>
        public delegate void PatchEvent();

        /// <summary>
        /// Override with the folder where your mod's icons and other assets are stored.
        /// If not overriden, this defaults to "[your mod assembly name]/Assets".
        /// </summary>
        /// <example>"MyModAssembly/Assets"</example>
        public virtual string AssetsFolder => $"{Assembly.GetCallingAssembly().GetName().Name}/Assets";

        /// <summary>
        /// Override with the file name for this item's icon.
        /// If not overriden, this defaults to "[this item's ClassID].png".
        /// </summary>
        /// <example>"MyClassID.png"</example>
        public virtual string IconFileName => $"{this.ClassID}.png";

        /// <summary>
        /// The in-game name of this spawnable item.
        /// </summary>
        public string FriendlyName { get; protected set; }

        /// <summary>
        /// The description text when viewing this spawnable item from the inventory or crafting UI.
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Returns <c>true</c> if this spawnable item has already been patched; Otherwise <c>false</c>.
        /// This will become <c>true</c> after the <seealso cref="Patch"/> method has finished running.
        /// </summary>
        public bool IsPatched { get; private set; } = false;

        /// <summary>
        /// Initializes a new <see cref="Spawnable"/>, the basic class needed for any item that can be spawned into the Subnautica game world.
        /// </summary>
        /// <param name="classId">The main internal identifier for this item. Your item's <see cref="TechType"/> will be created using this name.</param>
        /// <param name="friendlyName">The name displayed in-game for this item whether in the open world or in the inventory.</param>
        /// <param name="description">The description for this item; Typically seen in the PDA, inventory, or crafting screens.</param>
        protected Spawnable(string classId, string friendlyName, string description)
            : base(classId, $"{classId}Prefab")
        {
            this.FriendlyName = friendlyName;
            this.Description = description;

            CorePatchEvents += RegisterBasics;
        }

        /// <summary>
        /// This event triggers <c>before</c> the core patching methods begins.
        /// You can attach simple <seealso cref="PatchEvent"/> methods to this event if you want to run code <c>before</c> the any of the core patching methods begin.
        /// </summary>
        protected PatchEvent OnStartedPatching;

        /// <summary>
        /// The main patching methods are executed here.
        /// This event should only be used by the SMLHelper QuickStart classes.
        /// </summary>
        internal PatchEvent CorePatchEvents;

        /// <summary>
        /// This event triggers <c>after</c> the core patching methods begins.
        /// You can attach simple <seealso cref="PatchEvent"/> methods to this event if you want to run code <c>after</c> the core patching methods have finished.
        /// </summary>
        protected PatchEvent OnFinishedPatching;

        /// <summary>
        /// Starts all patching code in SMLHelper.
        /// If <seealso cref="IsPatched"/> is <c>true</c> then this method is skipped to avoid duplicate patching.
        /// </summary>
        /// <seealso cref="OnStartedPatching"/>
        /// <seealso cref="OnFinishedPatching"/>
        public void Patch()
        {
            if (this.IsPatched)
                return; // Already patched. Skip.

            OnStartedPatching?.Invoke();

            // Because invocation order isn't guaranteed by event handlers,
            // we make sure the TechType is patched first before anything else that might require it.
            this.TechType = TechTypeHandler.AddTechType(this.ClassID, this.FriendlyName, this.Description, false);

            CorePatchEvents.Invoke();

            this.IsPatched = true;

            OnFinishedPatching?.Invoke();
        }

        private void RegisterBasics()
        {
            PrefabHandler.RegisterPrefab(this);

            SpriteHandler.RegisterSprite(this.TechType, $"./QMods/{this.AssetsFolder}/{this.IconFileName}");
        }
    }
}
