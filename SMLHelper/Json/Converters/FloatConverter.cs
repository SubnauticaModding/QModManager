namespace SMLHelper.V2.Json.Converters
{
    using System;
    using System.Globalization;
#if SUBNAUTICA_STABLE
    using Oculus.Newtonsoft.Json;
#else
    using Newtonsoft.Json;
#endif

    /// <summary>
    /// A <see cref="JsonConverter"/> for rounding floats or doubles to a given number of decimal places,
    /// trimming trailing 0s.
    /// </summary>
    public class FloatConverter : JsonConverter
    {
        private readonly int DecimalPlaces = 4;
        private readonly MidpointRounding Mode = MidpointRounding.AwayFromZero;
        /// <summary>
        /// Creates a new <see cref="FloatConverter"/>.
        /// </summary>
        /// <param name="decimalPlaces">The number of decimal places to round to, where -1 disables rounding.</param>
        /// <param name="mode">The rounding mode to use when rounding.</param>
        /// <seealso cref="MidpointRounding"/>
        public FloatConverter(int decimalPlaces, MidpointRounding mode)
        {
            DecimalPlaces = decimalPlaces;
            Mode = mode;
        }
        /// <summary>
        /// Creates a new <see cref="FloatConverter"/>.
        /// </summary>
        /// <param name="decimalPlaces">The number of decimal places to round to, where -1 disables rounding.</param>
        public FloatConverter(int decimalPlaces) => DecimalPlaces = decimalPlaces;
        /// <summary>
        /// Creates a new <see cref="FloatConverter"/>.
        /// </summary>
        /// <param name="mode">The rounding mode to use when rounding.</param>
        public FloatConverter(MidpointRounding mode) => Mode = mode;
        /// <summary>
        /// Creates a new <see cref="FloatConverter"/>.
        /// </summary>
        public FloatConverter() { }

        /// <summary>
        /// The method for writing the <paramref name="value"/> data to the <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            double d = Convert.ToDouble(value);
            if (DecimalPlaces > -1)
            {
                writer.WriteValue(Math.Round(d, DecimalPlaces, Mode).ToString(CultureInfo.InvariantCulture.NumberFormat));
            }
            else
            {
                writer.WriteValue(d.ToString(CultureInfo.InvariantCulture.NumberFormat));
            }
        }

        /// <summary>
        /// The method for reading the <see cref="float"/> or <see cref="double"/> data from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var s = (string)reader.Value;
            if (objectType == typeof(float))
            {
                return float.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
            }
            else
            {
                return double.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
            }
        }

        /// <summary>
        /// The method for determining whether the current <paramref name="objectType"/> can be processed byt this
        /// <see cref="JsonConverter"/>
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType) => objectType == typeof(float) || objectType == typeof(double);
    }
}
