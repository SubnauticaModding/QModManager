namespace SMLHelper.V2.Options.Attributes
{
    using Json;
    using System;

    /// <summary>
    /// Attribute used to signify the specified <see cref="float"/>, <see cref="double"/> or <see cref="int"/> should be represented
    /// in the mod's option menu as a <see cref="ModSliderOption"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// using SMLHelper.V2.Json;
    /// using SMLHelper.V2.Options;
    /// 
    /// [Menu("My Options Menu")]
    /// public class Config : ConfigFile
    /// {
    ///     [Slider("My Slider", 0, 50, DefaultValue = 25, Format = "{0:F2}")]
    ///     public float MySlider;
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="MenuAttribute"/>
    /// <seealso cref="ConfigFile"/>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SliderAttribute : ModOptionAttribute
    {
        /// <summary>
        /// The minimum value of the slider.
        /// </summary>
        public float Min { get; set; } = 0;

        /// <summary>
        /// The maximum value of the slider.
        /// </summary>
        public float Max { get; set; } = 100;

        /// <summary>
        /// The default value of the slider.
        /// </summary>
        public float DefaultValue { get; set; }

        /// <summary>
        /// The format to use when displaying the value, e.g. "{0:F2}" or "{0:F0} %"
        /// </summary>
        public string Format { get; set; } = "{0:F0}";

        /// <summary>
        /// The step to apply to the slider (ie. round to nearest)
        /// </summary>
        public float Step { get; set; } = 0.05f;

        /// <summary>
        /// Signifies the specified <see cref="float"/>, <see cref="double"/> or <see cref="int"/> should be represented in the mod's
        /// options menu as a <see cref="ModSliderOption"/>.
        /// </summary>
        /// <param name="label">The label for the slider. If none is set, the name of the method will be used.</param>
        /// <param name="min">The minimum value of the slider.</param>
        /// <param name="max">The maximum value of the slider.</param>
        public SliderAttribute(string label, float min, float max) : base(label)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Signifies the specified <see cref="float"/>, <see cref="double"/> or <see cref="int"/> should be represented in the mod's
        /// options menu as a <see cref="ModSliderOption"/>.
        /// </summary>
        /// <param name="min">The minimum value of the slider.</param>
        /// <param name="max">The maximum value of the slider.</param>
        public SliderAttribute(float min, float max) : this(null, min, max) { }

        /// <summary>
        /// Signifies the specified <see cref="float"/>, <see cref="double"/> or <see cref="int"/> should be represented in the mod's
        /// options menu as a <see cref="ModSliderOption"/>.
        /// </summary>
        /// <param name="label">The label for the slider. If none is set, the name of the method will be used.</param>
        public SliderAttribute(string label = null) : base(label) { }

        /// <summary>
        /// Signifies the specified <see cref="float"/>, <see cref="double"/> or <see cref="int"/> should be represented in the mod's
        /// options menu as a <see cref="ModSliderOption"/>.
        /// </summary>
        public SliderAttribute() { }
    }
}
