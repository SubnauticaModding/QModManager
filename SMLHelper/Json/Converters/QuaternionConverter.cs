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
    /// A Quaternion json converter that simplifies the Vector3 to only x,y,z serialization.
    /// </summary>
    public class QuaternionConverter : JsonConverter
    {
        /// <summary>
        /// A method that determines when this converter should process.
        /// </summary>
        /// <param name="objectType">the current object type</param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Quaternion);
        }

        /// <summary>
        /// A method that tells Newtonsoft how to Serialize the current object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var quaternion = (Quaternion)value;
            serializer.Serialize(writer, (QuaternionJson)quaternion);
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
            return (Quaternion)serializer.Deserialize<QuaternionJson>(reader);
        }
    }

    internal record QuaternionJson(float X, float Y, float Z, float W)
    {
        public static explicit operator Quaternion(QuaternionJson q) => new(q.X, q.Y, q.Z, q.W);
        public static explicit operator QuaternionJson(Quaternion q) => new(q.x, q.y, q.z, q.w);
    }
}
