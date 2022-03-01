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
    /// A Vector4 json converter that simplifies the Vector4 to only x,y,z,w serialization.
    /// </summary>
    public class Vector4Converter : JsonConverter
    {
        /// <summary>
        /// A method that determines when this converter should process.
        /// </summary>
        /// <param name="objectType">the current object type</param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector4);
        }

        /// <summary>
        /// A method that tells Newtonsoft how to Serialize the current object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector4 = (Vector4)value;
            serializer.Serialize(writer, (Vector4Json)vector4);
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
            return (Vector4)serializer.Deserialize<Vector4Json>(reader);
        }
    }

    internal record Vector4Json(float X, float Y, float Z, float W)
    {
        public static explicit operator Vector4(Vector4Json v) => new(v.X, v.Y, v.Z, v.W);
        public static explicit operator Vector4Json(Vector4 v) => new(v.x, v.y, v.z, v.w);
    }
}