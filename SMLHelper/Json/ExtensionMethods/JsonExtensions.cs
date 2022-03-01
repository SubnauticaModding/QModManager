namespace SMLHelper.V2.Json.ExtensionMethods
{
    using SMLHelper.V2.Utility;
#if SUBNAUTICA_STABLE
    using Oculus.Newtonsoft.Json;
#else
    using Newtonsoft.Json;
#endif

    /// <summary>
    /// Extension methods for parsing objects as JSON data.
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// Loads the JSON properties from a file on disk into the <paramref name="jsonObject"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="jsonObject"/>.</typeparam>
        /// <param name="jsonObject">The object instance to load the properties into.</param>
        /// <param name="path">The file path to the JSON file to parse.</param>
        /// <param name="createIfNotExist">Whether a new JSON file should be created with default values if it does not
        /// already exist.</param>
        /// <param name="jsonConverters">The <see cref="JsonConverter"/>s to be used for deserialization.</param>
        /// <seealso cref="SaveJson{T}(T, string, JsonConverter[])"/>
        public static void LoadJson<T>(this T jsonObject, string path = null, 
            bool createIfNotExist = true, params JsonConverter[] jsonConverters) where T : class
            => JsonUtils.Load(jsonObject, path, createIfNotExist, jsonConverters);

        /// <summary>
        /// Saves the fields and properties of the <paramref name="jsonObject"/> as JSON properties to the file on disk.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="jsonObject"/>.</typeparam>
        /// <param name="jsonObject">The object instance to save the fields and properties from.</param>
        /// <param name="path">The file path at which to save the JSON file.</param>
        /// <param name="jsonConverters">The <see cref="JsonConverter"/>s to be used for serialization.</param>
        public static void SaveJson<T>(this T jsonObject, string path = null,
            params JsonConverter[] jsonConverters) where T : class
            => JsonUtils.Save(jsonObject, path, jsonConverters);
    }
}
