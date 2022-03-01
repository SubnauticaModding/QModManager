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
    /// A Vector2 json converter that simplifies the Vector2 to only x,y serialization.
    /// </summary>
    public class Vector2Converter : JsonConverter
    {
        /// <summary>
        /// A method that determines when this converter should process.
        /// </summary>
        /// <param name="objectType">the current object type</param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2);
        }

        /// <summary>
        /// A method that tells Newtonsoft how to Serialize the current object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector2 = (Vector2)value;
            serializer.Serialize(writer, (Vector2Json)vector2);
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
            return (Vector2)serializer.Deserialize<Vector2Json>(reader);
        }
    }

    internal record Vector2Json(float X, float Y)
    {
        public static explicit operator Vector2(Vector2Json v) => new(v.X, v.Y);
        public static explicit operator Vector2Json(Vector2 v) => new(v.x, v.y);
    }
}