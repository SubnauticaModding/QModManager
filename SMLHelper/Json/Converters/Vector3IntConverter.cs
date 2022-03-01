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
    /// A Vector3Int json converter that simplifies the Vector3Int to only x,y,z serialization.
    /// </summary>
    public class Vector3IntConverter : JsonConverter
    {
        /// <summary>
        /// A method that determines when this converter should process.
        /// </summary>
        /// <param name="objectType">the current object type</param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3Int);
        }

        /// <summary>
        /// A method that tells Newtonsoft how to Serialize the current object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector3Int = (Vector3Int)value;
            serializer.Serialize(writer, (Vector3IntJson)vector3Int);
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
            return (Vector3Int)serializer.Deserialize<Vector3IntJson>(reader);
        }
    }

    internal record Vector3IntJson(int X, int Y, int Z)
    {
        public static explicit operator Vector3Int(Vector3IntJson v) => new(v.X, v.Y, v.Z);
        public static explicit operator Vector3IntJson(Vector3Int v) => new(v.x, v.y, v.z);
    }
}