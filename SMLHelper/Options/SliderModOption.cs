namespace SMLHelper.V2.Options
{
    using Interfaces;
    using System;
    using System.Reflection;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;
#if SUBNAUTICA
    using Text = UnityEngine.UI.Text;
#elif BELOWZERO
    using Text = TMPro.TextMeshProUGUI;
#endif

    /// <summary>
    /// Contains all the information about a slider changed event.
    /// </summary>
    public class SliderChangedEventArgs : EventArgs, IModOptionEventArgs
    {
        /// <summary>
        /// The ID of the <see cref="ModSliderOption"/> that was changed.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The new value for the <see cref="ModSliderOption"/>.
        /// </summary>
        public float Value { get; }

        /// <summary>
        /// The new value for the <see cref="ModSliderOption"/> parsed as an <see cref="int"/>
        /// </summary>
        public int IntegerValue { get; }

        /// <summary>
        /// Constructs a new <see cref="SliderChangedEventArgs"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="ModSliderOption"/> that was changed.</param>
        /// <param name="value">The new value for the <see cref="ModSliderOption"/>.</param>
        public SliderChangedEventArgs(string id, float value)
        {
            this.Id = id;
            this.Value = value;
            this.IntegerValue = Mathf.RoundToInt(value);
        }
    }

    public abstract partial class ModOptions
    {
        /// <summary>
        /// The event that is called whenever a slider is changed. Subscribe to this in the constructor.
        /// </summary>
        protected event EventHandler<SliderChangedEventArgs> SliderChanged;

        /// <summary>
        /// Notifies a slider change to all subsribed event handlers.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        internal void OnSliderChange(string id, float value)
        {
            SliderChanged(this, new SliderChangedEventArgs(id, value));
        }

        /// <summary>
        /// Adds a new <see cref="ModSliderOption"/> to this instance.
        /// </summary>
        /// <param name="id">The internal ID for the slider option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="minValue">The minimum value for the range.</param>
        /// <param name="maxValue">The maximum value for the range.</param>
        /// <param name="value">The starting value.</param>
        protected void AddSliderOption(string id, string label, float minValue, float maxValue, float value)
        {
            AddSliderOption(id, label, minValue, maxValue, value, null, null, 0);
        }

        /// <summary>
        /// Adds a new <see cref="ModSliderOption"/> to this instance.
        /// </summary>
        /// <param name="id">The internal ID for the slider option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="minValue">The minimum value for the range.</param>
        /// <param name="maxValue">The maximum value for the range.</param>
        /// <param name="value">The starting value.</param>
        /// <param name="defaultValue">The default value for the slider. If this is null then 'value' used as default.</param>
        /// <param name="valueFormat"> format for value, e.g. "{0:F2}" or "{0:F0} %"
        /// (more on this <see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">here</see>)</param>
        protected void AddSliderOption(string id, string label, float minValue, float maxValue, float value, float? defaultValue, string valueFormat = null)
        {
            AddSliderOption(id, label, minValue, maxValue, value, defaultValue, valueFormat, 0);
        }

        /// <summary>
        /// Adds a new <see cref="ModSliderOption"/> to this instance.
        /// </summary>
        /// <param name="id">The internal ID for the slider option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="minValue">The minimum value for the range.</param>
        /// <param name="maxValue">The maximum value for the range.</param>
        /// <param name="value">The starting value.</param>
        /// <param name="defaultValue">The default value for the slider. If this is null then 'value' used as default.</param>
        /// <param name="valueFormat"> format for value, e.g. "{0:F2}" or "{0:F0} %"
        /// (more on this <see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">here</see>)</param>
        /// <param name="step">Step for the slider, ie. round to nearest X</param>
        protected void AddSliderOption(string id, string label, float minValue, float maxValue, float value, float? defaultValue, string valueFormat = null, float step = 0)
        {
            AddOption(new ModSliderOption(id, label, minValue, maxValue, value, defaultValue, valueFormat, step));
        }
    }

    /// <summary>
    /// A mod option class for handling an option that can have any floating point value between a minimum and maximum.
    /// </summary>
    public class ModSliderOption : ModOption
    {
        /// <summary>
        /// The minimum value of the <see cref="ModSliderOption"/>.
        /// </summary>
        public float MinValue { get; }

        /// <summary>
        /// The maximum value of the <see cref="ModSliderOption"/>.
        /// </summary>
        public float MaxValue { get; }

        /// <summary>
        /// The current value of the <see cref="ModSliderOption"/>.
        /// </summary>
        public float Value { get; }

        /// <summary>
        /// The default value of the <see cref="ModSliderOption"/>.
        /// Showed on the slider by small gray circle. Slider's handle will snap to the default value near it.
        /// </summary>
        public float DefaultValue { get; }

        /// <summary>
        /// The step value of the <see cref="ModSliderOption"/>.
        /// Defaults to 0f for compatibility with older mods.
        /// </summary>
        public float Step { get; } = 0;

        /// <summary> Format for value field (<see cref="ModOptions.AddSliderOption(string, string, float, float, float, float?, string)"/>) </summary>
        public string ValueFormat { get; }

        private SliderValue sliderValue = null;

#if !BELOWZERO
        private float previousValue;
#endif
        internal override void AddToPanel(uGUI_TabbedControlsPanel panel, int tabIndex)
        {
#if BELOWZERO
            UnityAction<float> callback = new UnityAction<float>((value) => parentOptions.OnSliderChange(Id, sliderValue?.ConvertToDisplayValue(value) ?? value));
#else
            UnityAction<float> callback = new UnityAction<float>((value) =>
            {
                value = sliderValue?.ConvertToDisplayValue(value) ?? value;
                if (value != previousValue)
                {
                    previousValue = value;
                    parentOptions.OnSliderChange(Id, value);
                }
            });
#endif

#if SUBNAUTICA
            panel.AddSliderOption(tabIndex, Label, Value, MinValue, MaxValue, DefaultValue, callback);
#elif BELOWZERO
            panel.AddSliderOption(tabIndex, Label, Value, MinValue, MaxValue, DefaultValue, Step, callback, SliderLabelMode.Default, "0.0");
#endif

            // AddSliderOption for some reason doesn't return created GameObject, so we need this little hack
            Transform options = panel.tabs[tabIndex].container.transform;
            OptionGameObject = options.GetChild(options.childCount - 1).gameObject; // last added game object

#if BELOWZERO
            // if we using custom value format, we need to replace vanilla uGUI_SliderWithLabel with our component
            if (ValueFormat != null)
                OptionGameObject.transform.Find("Slider").gameObject.AddComponent<SliderValue>().ValueFormat = ValueFormat;

            // fixing tooltip for slider
            OptionGameObject.transform.Find("Slider/Caption").GetComponent<Text>().raycastTarget = true;
#else
            // if we using custom value format or step, we need to replace vanilla uGUI_SliderWithLabel with our component
            if (ValueFormat != null || Step >= Mathf.Epsilon)
            {
                var sliderValue = OptionGameObject.transform.Find("Slider").gameObject.AddComponent<SliderValue>();
                sliderValue.Step = Step;
                if (ValueFormat != null)
                    sliderValue.ValueFormat = ValueFormat;
            }
#endif

            base.AddToPanel(panel, tabIndex);

            sliderValue = OptionGameObject.GetComponentInChildren<SliderValue>(); // we can also add custom SliderValue in OnGameObjectCreated event
        }

        /// <summary>
        /// Instantiates a new <see cref="ModSliderOption"/> for handling an option that can have any floating point value between a minimum and maximum.
        /// </summary>
        /// <param name="id">The internal ID of this option.</param>
        /// <param name="label">The display text to show on the in-game menus.</param>
        /// <param name="minValue">The minimum value for the range.</param>
        /// <param name="maxValue">The maximum value for the range.</param>
        /// <param name="value">The starting value.</param>
        /// <param name="defaultValue">The default value for the slider. If this is null then 'value' used as default.</param>
        /// <param name="valueFormat">Format for value field (<see cref="ModOptions.AddSliderOption(string, string, float, float, float, float?, string, float)"/>) </param>
        /// <param name="step">Step for the slider ie. round to nearest X</param>
        internal ModSliderOption(string id, string label, float minValue, float maxValue, float value, float? defaultValue = null, string valueFormat = null, float step = 0) : base(label, id)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Value = value;
            DefaultValue = defaultValue ?? value;
            ValueFormat = valueFormat;
            Step = step;
        }

        /// <summary>
        /// Component for customizing slider's value behaviour.
        /// If you need more complex behaviour than just custom value format then you can inherit this component 
        /// and add it to "Slider" game object in OnGameObjectCreated event (see <see cref="AddToPanel"/> for details on adding component)
        /// You can override value converters <see cref="ConvertToDisplayValue"/> and <see cref="ConvertToSliderValue"/>,
        /// in that case internal range for slider will be changed to [0.0f : 1.0f] and you can control displayed value with these converters
        /// (also this value will be passed to <see cref="ModOptions.OnSliderChange"/> event)
        /// </summary>
        public class SliderValue : MonoBehaviour
        {
            /// <summary> The value label of the <see cref="SliderValue"/> </summary>
            protected Text label;

            /// <summary> The slider controlling this <see cref="SliderValue"/> </summary>
            protected Slider slider;

            /// <summary>
            /// The minimum value of the <see cref="SliderValue"/>.
            /// In case of custom value converters it can be not equal to internal minimum value for slider
            /// </summary>
            protected float minValue;

            /// <summary>
            /// The maximum value of the <see cref="SliderValue"/>.
            /// In case of custom value converters it can be not equal to internal maximum value for slider
            /// </summary>
            protected float maxValue;

            /// <summary> Custom value format property. Set it right after adding component to game object for proper behaviour </summary>
            public string ValueFormat
            {
                get => valueFormat;
                set => valueFormat = value ?? "{0}";
            }

            /// <summary> Custom value format </summary>
            protected string valueFormat = "{0}";

            /// <summary> Step for the slider </summary>
            internal float Step;

            /// <summary>
            /// Width for value text field. Used by <see cref="SliderOptionAdjust"/> to adjust label width.
            /// It is calculated in <see cref="UpdateValueWidth"/>, but you can override this property.
            /// </summary>
            public virtual float ValueWidth { get; protected set; } = -1f;

            /// <summary> Override this if you need to initialize custom value converters </summary>
            protected virtual void InitConverters() { }

            /// <summary> Converts internal slider value [0.0f : 1.0f] to displayed value </summary>
            public virtual float ConvertToDisplayValue(float sliderValue)
            {
                if (Step >= Mathf.Epsilon)
                {
                    var value = Mathf.Round(slider.value / Step) * Step;

                    if (value != sliderValue)
                        slider.value = value;

                    return value;
                }
                else
                    return sliderValue;
            }

            /// <summary> Converts displayed value to internal slider value [0.0f : 1.0f] </summary>
            public virtual float ConvertToSliderValue(float displayValue) => displayValue;

            /// <summary> Component initialization. If you overriding this, make sure that you calling base.Awake() </summary>
            protected virtual void Awake()
            {
                bool _isOverrided(string methodName)
                {
                    MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                    return method.DeclaringType != method.GetBaseDefinition().DeclaringType;
                }

                bool useConverters = _isOverrided(nameof(SliderValue.ConvertToDisplayValue)) &&
                                     _isOverrided(nameof(SliderValue.ConvertToSliderValue));

                if (GetComponent<uGUI_SliderWithLabel>() is uGUI_SliderWithLabel sliderLabel)
                {
                    label = sliderLabel.label;
                    slider = sliderLabel.slider;
                    Destroy(sliderLabel);
                }
                else
                    V2.Logger.Log("uGUI_SliderWithLabel not found", LogLevel.Error);

                if (GetComponent<uGUI_SnappingSlider>() is uGUI_SnappingSlider snappingSlider)
                {
                    minValue = snappingSlider.minValue;
                    maxValue = snappingSlider.maxValue;

                    // if we use overrided converters, we change range of the slider to [0.0f : 1.0f]
                    if (useConverters)
                    {
                        InitConverters();

                        snappingSlider.minValue = 0f;
                        snappingSlider.maxValue = 1f;
                        snappingSlider.defaultValue = ConvertToSliderValue(snappingSlider.defaultValue);
                        snappingSlider.value = ConvertToSliderValue(snappingSlider.value);
                    }
                }
                else
                    V2.Logger.Log("uGUI_SnappingSlider not found", LogLevel.Error);

                slider.onValueChanged.AddListener(new UnityAction<float>(OnValueChanged));
                UpdateLabel();
            }

            /// <summary> <see cref="MonoBehaviour"/>.Start() </summary>
            protected virtual IEnumerator Start() => UpdateValueWidth();

            /// <summary>
            /// Method for calculating necessary label's width. Creates temporary label and compares widths of min and max values,
            /// then sets <see cref="ValueWidth"/> to the wider. Be aware that in case of using custom converters some intermediate value may be wider than min/max values.
            /// </summary>
            protected virtual IEnumerator UpdateValueWidth()
            {
                // we need to know necessary width for value text field based on min/max values and value format
                GameObject tempLabel = Instantiate(label.gameObject);
                tempLabel.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

                // we'll add formatted min value to the label and skip one frame for updating ContentSizeFitter
                tempLabel.GetComponent<Text>().text = string.Format(valueFormat, minValue);
                yield return null;
                float widthForMin = tempLabel.GetComponent<RectTransform>().rect.width;

                // same for max value
                tempLabel.GetComponent<Text>().text = string.Format(valueFormat, maxValue);
                yield return null;
                float widthForMax = tempLabel.GetComponent<RectTransform>().rect.width;

                Destroy(tempLabel);
                ValueWidth = Math.Max(widthForMin, widthForMax);
            }

            /// <summary> Called when user changes slider value </summary>
            protected virtual void OnValueChanged(float value) => UpdateLabel();

            /// <summary>
            /// Updates label's text with formatted and converted slider's value.
            /// Override this if you need even more control on slider's value behaviour.
            /// </summary>
            protected virtual void UpdateLabel()
            {
                float val = ConvertToDisplayValue(slider.value); // doing it separately in case that valueFormat is changing in ConvertToDisplayValue
                label.text = string.Format(valueFormat, val);
            }
        }

        private class SliderOptionAdjust : ModOptionAdjust
        {
#if SUBNAUTICA
            private const string sliderBackground = "Slider/Background";
#elif BELOWZERO
            private const string sliderBackground = "Slider/Slider/Background";
#endif
            private const float spacing_MainMenu = 30f;
            private const float spacing_GameMenu = 10f;
            private const float valueSpacing = 15f; // used in game menu

            public IEnumerator Start()
            {
                SetCaptionGameObject("Slider/Caption", isMainMenu ? 488f : 364.7f); // need to use custom width for slider's captions
                yield return null; // skip one frame

                float sliderValueWidth = 0f;

                if (gameObject.GetComponentInChildren<SliderValue>() is SliderValue sliderValue)
                {
                    // wait while SliderValue calculating ValueWidth (one or two frames)
                    while (sliderValue.ValueWidth < 0)
                        yield return null;

                    sliderValueWidth = sliderValue.ValueWidth + (isMainMenu ? 0f : valueSpacing);
                }

                // changing width for slider value label (we don't change width for default format!)
                float widthDelta = 0f;
                RectTransform sliderValueRect = gameObject.transform.Find("Slider/Value") as RectTransform;

                if (sliderValueWidth > sliderValueRect.rect.width)
                {
                    widthDelta = sliderValueWidth - sliderValueRect.rect.width;
                    sliderValueRect.sizeDelta = SetVec2x(sliderValueRect.sizeDelta, sliderValueWidth);
                }
                else
                    sliderValueWidth = sliderValueRect.rect.width;

                RectTransform rect = gameObject.transform.Find(sliderBackground) as RectTransform;

                if (widthDelta != 0f)
                    rect.localPosition = SetVec2x(rect.localPosition, rect.localPosition.x - widthDelta);

                // changing width for slider
                float widthAll = gameObject.GetComponent<RectTransform>().rect.width;
                float widthSlider = rect.rect.width;
                float widthText = CaptionWidth + (isMainMenu ? spacing_MainMenu : spacing_GameMenu);

                // it's not pixel-perfect, but it's good enough
                if (widthText + widthSlider + sliderValueWidth > widthAll)
                    rect.sizeDelta = SetVec2x(rect.sizeDelta, widthAll - widthText - sliderValueWidth - widthSlider);
                else if (widthDelta > 0f)
                    rect.sizeDelta = SetVec2x(rect.sizeDelta, -widthDelta);

                Destroy(this);
            }
        }
        internal override Type AdjusterComponent => typeof(SliderOptionAdjust);
    }
}
