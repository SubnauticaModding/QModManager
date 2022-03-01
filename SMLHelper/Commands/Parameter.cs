
namespace SMLHelper.V2.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    internal struct Parameter
    {
        private static Dictionary<Type, Func<string, object>> TypeConverters = new Dictionary<Type, Func<string, object>>()
        {
            [typeof(string)] = (s) => s,
            [typeof(bool)] = (s) => bool.Parse(s),
            [typeof(int)] = (s) => int.Parse(s, CultureInfo.InvariantCulture.NumberFormat),
            [typeof(float)] = (s) => float.Parse(s, CultureInfo.InvariantCulture.NumberFormat),
            [typeof(double)] = (s) => double.Parse(s, CultureInfo.InvariantCulture.NumberFormat)
        };

        public static IEnumerable<Type> SupportedTypes => TypeConverters.Keys;

        public Type ParameterType { get; }
        public bool IsOptional { get; }
        public string Name { get; }
        public bool IsValidParameterType { get; }

        public Parameter(ParameterInfo parameter)
        {
            ParameterType = parameter.ParameterType;
            IsOptional = parameter.IsOptional;
            Name = parameter.Name;
            IsValidParameterType = SupportedTypes.Contains(ParameterType);
        }

        public object Parse(string input)
            => TypeConverters[ParameterType](input);
    }
}
