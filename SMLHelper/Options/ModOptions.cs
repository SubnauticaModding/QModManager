namespace SMLHelper.V2.Options
{
    using Interfaces;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using QModManager.API;

    /// <summary>
    /// Abstract class that provides the framework for your mod's in-game configuration options.
    /// </summary>
    public abstract partial class ModOptions
    {
        /// <summary>
        /// The name of this set of configuration options.
        /// </summary>
        public string Name;

        /// <summary>
        /// Obtains the <see cref="ModOption"/>s that belong to this instance. Can be null.
        /// </summary>
        public List<ModOption> Options => _options == null ? null : new List<ModOption>(_options.Values);

        // This is a dictionary now in case we want to get the ModOption quickly
        // based on the provided ID.
        private Dictionary<string, ModOption> _options;

        private void AddOption(ModOption option)
        {
            _options.Add(option.Id, option);
            option.SetParent(this);
        }

        internal void AddOptionsToPanel(uGUI_TabbedControlsPanel panel, int tabIndex)
        {
            panel.AddHeading(tabIndex, Name);

            _options = new Dictionary<string, ModOption>(); // we need to do this every time we adding options
            BuildModOptions();

            _options.Values.ForEach(option => option.AddToPanel(panel, tabIndex));
        }

        /// <summary>
        /// Creates a new instance of <see cref="ModOptions"/>.
        /// </summary>
        /// <param name="name">The name that will display above this section of options in the in-game menu.</param>
        public ModOptions(string name)
        {
            Name = name;
        }

        /// <summary>
        /// <para>Builds up the configuration the options.</para>
        /// <para>This method should be composed of calls into the following methods: 
        /// <seealso cref="AddSliderOption(string, string, float, float, float)"/> | <seealso cref="AddToggleOption"/> | <seealso cref="AddChoiceOption(string, string, string[], int)"/> | <seealso cref="AddKeybindOption(string, string, GameInput.Device, KeyCode)"/>.</para>
        /// <para>Make sure you have subscribed to the events in the constructor to handle what happens when the value is changed:
        /// <seealso cref="SliderChanged"/> | <seealso cref="ToggleChanged"/> | <seealso cref="ChoiceChanged"/> | <seealso cref="KeybindChanged"/>.</para>
        /// </summary>
        public abstract void BuildModOptions();

        /// <summary> The event that is called whenever a game object created for the option </summary>
        protected event EventHandler<GameObjectCreatedEventArgs> GameObjectCreated;

        internal void OnGameObjectCreated(string id, GameObject gameObject)
        {
            GameObjectCreated?.Invoke(this, new GameObjectCreatedEventArgs(id, gameObject));
        }
    }

    /// <summary> Contains all the information about a created game object event </summary>
    public class GameObjectCreatedEventArgs : EventArgs, IModOptionEventArgs
    {
        /// <summary> The ID of the <see cref="ModOption"/> for which game object was created </summary>
        public string Id { get; }

        /// <summary> New game object for the <see cref="ModOption"/> </summary>
        public GameObject GameObject { get; }

        /// <summary> Constructs a new <see cref="GameObjectCreatedEventArgs"/> </summary>
        /// <param name="id"> The ID of the <see cref="ModOption"/> for which game object was created </param>
        /// <param name="gameObject"> New game object for the <see cref="ModOption"/> </param>
        public GameObjectCreatedEventArgs(string id, GameObject gameObject)
        {
            Id = id;
            GameObject = gameObject;
        }
    }

    /// <summary>
    /// The common abstract class to all mod options.
    /// </summary>
    public abstract class ModOption
    {
        /// <summary>
        /// The internal ID that identifies this option.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The display text to be shown for this option in the in-game menus.
        /// </summary>
        public string Label { get; }

        /// <summary> UI GameObject for this option </summary>
        public GameObject OptionGameObject { get; protected set; }

        /// <summary> Parent <see cref="ModOptions"/> for this option </summary>
        protected ModOptions parentOptions;

        internal void SetParent(ModOptions parent)
        {
            if (parentOptions == null)
                parentOptions = parent;
            else
                V2.Logger.Log($"ModOption.SetParent: parent already setted for {Id}", LogLevel.Warn);
        }

        // adds UI GameObject to panel and updates OptionGameObject
        internal virtual void AddToPanel(uGUI_TabbedControlsPanel panel, int tabIndex)
        {
            if (isNeedAdjusting && AdjusterComponent != null)
                OptionGameObject.AddComponent(AdjusterComponent);

            parentOptions.OnGameObjectCreated(Id, OptionGameObject);
        }

        /// <summary>
        /// Base constructor for all mod options.
        /// </summary>
        /// <param name="label">The display text to show on the in-game menus.</param>
        /// <param name="id">The internal ID if this option.</param>
        internal ModOption(string label, string id)
        {
            Label = label;
            Id = id;
        }

        // if ModsOptionsAdjusted mod is active, we don't add adjuster components
        internal static readonly bool isNeedAdjusting = (QModServices.Main.FindModById("ModsOptionsAdjusted")?.Enable != true);

        // type of component derived from ModOptionAdjust (for using in base.AddToPanel)
        internal abstract Type AdjusterComponent { get; }

        // base class for 'adjuster' components (so ui elements don't overlap with their text labels)
        // reason for using components is to skip one frame before manually adjust ui elements to make sure that Unity UI Layout components is updated
        internal abstract class ModOptionAdjust : MonoBehaviour
        {
            private const float minCaptionWidth_MainMenu = 480f;
            private const float minCaptionWidth_GameMenu = 360f;
            private GameObject caption = null;

            protected float CaptionWidth { get => caption?.GetComponent<RectTransform>().rect.width ?? 0f; }

            protected bool isMainMenu { get; private set; } = true; // is it main menu or game menu

            protected static Vector2 SetVec2x(Vector2 vec, float val) { vec.x = val; return vec; }

            public void Awake() => isMainMenu = gameObject.GetComponentInParent<MainMenuOptions>() != null;

            // we add ContentSizeFitter component to text label so it will change width in its Update() based on text
            protected void SetCaptionGameObject(string gameObjectPath, float minWidth = 0f)
            {
                caption = gameObject.transform.Find(gameObjectPath)?.gameObject;

                if (!caption)
                {
                    V2.Logger.Log($"ModOptionAdjust: caption gameobject '{gameObjectPath}' not found", LogLevel.Warn);
                    return;
                }

                caption.AddComponent<LayoutElement>().minWidth = minWidth != 0f ? minWidth : (isMainMenu ? minCaptionWidth_MainMenu : minCaptionWidth_GameMenu);
                caption.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize; // for autosizing captions

                RectTransform transform = caption.GetComponent<RectTransform>();
                transform.pivot = SetVec2x(transform.pivot, 0f);
                transform.anchoredPosition = SetVec2x(transform.anchoredPosition, 0f);
            }
        }
    }
}
