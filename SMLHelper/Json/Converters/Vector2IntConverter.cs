using System;
using UnityEngine;
#if SUBNAUTICA_STABLE
using Oculus.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
namespace SMLHelper.V2.Json.Converters
{
    /// <summary>
    /// A Vector2Int json converter that simplifies the Vector2Int to only x,y serialization.
    /// </summary>
    public class Vector2IntConverter : JsonConverter
    {
        /// <summary>
        /// A method that determines when this converter should process.
        /// </summary>
        /// <param name="objectType">the current object type</param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2Int);
        }

        /// <summary>
        /// A method that tells Newtonsoft how to Serialize the current object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector2Int = (Vector2Int)value;
            serializer.Serialize(writer, (Vector2IntJson)vector2Int);
        }

        /// <summary>
        /// A method that tells Newtonsoft how to Deserialize and read the current object.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (Vector2Int)serializer.Deserialize<Vector2IntJson>(reader);
        }
    }

    internal record Vector2IntJson(int X, int Y)
    {
        public static explicit operator Vector2Int(Vector2IntJson v) => new(v.X, v.Y);
        public static explicit operator Vector2IntJson(Vector2Int v) => new(v.x, v.y);
    }
}