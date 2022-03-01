namespace SMLHelper.V2.Utility
{
    using System;
    using System.IO;
    using System.Globalization;
    using System.Text;
    using System.Threading;
    using System.Reflection;
#if SUBNAUTICA_STABLE
    using Oculus.Newtonsoft.Json;
#else
    using Newtonsoft.Json;
#endif

    /// <summary>
    /// A collection of utilities for interacting with JSON files.
    /// </summary>
    public static class JsonUtils
    {
        private static string GetDefaultPath<T>(Assembly assembly) where T : class
        {
            return Path.Combine(
                Path.GetDirectoryName(assembly.Location),
                $"{GetName<T>()}.json"
            );
        }

        private static string GetName<T>(bool camelCase = true) where T : class
        {
            string name = typeof(T).Name;
            if (camelCase)
            {
                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                name = currentCulture.TextInfo.ToTitleCase(name)
                    .Replace("_", string.Empty).Replace(" ", string.Empty);
                name = char.ToLowerInvariant(name[0]) + name.Substring(1);
            }
            return name;
        }

        /// <summary>
        /// Create an instance of <typeparamref name="T"/>, populated with data from the JSON file at the given 
        /// <paramref name="path"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to initialise and populate with JSON data.</typeparam>
        /// <param name="path">The path on disk at which the JSON file can be found.</param>
        /// <param name="createFileIfNotExist">Whether a new JSON file should be created with default values if it does not 
        /// already exist.</param>
        /// <param name="jsonConverters">An array of <see cref="JsonConverter"/>s to be used for deserialization.</param>
        /// <returns>The <typeparamref name="T"/> instance populated with data from the JSON file at
        /// <paramref name="path"/>, or default values if it cannot be found or an error is encountered while parsing the
        /// file.</returns>
        /// <seealso cref="Load{T}(T, string, bool, JsonConverter[])"/>
        /// <seealso cref="Save{T}(T, string, JsonConverter[])"/>
        public static T Load<T>(string path = null, bool createFileIfNotExist = true,
            params JsonConverter[] jsonConverters) where T : class, new()
        {
            if (string.IsNullOrEmpty(path))
            {
                path = GetDefaultPath<T>(Assembly.GetCallingAssembly());
            }

            if (Directory.Exists(Path.GetDirectoryName(path)) && File.Exists(path))
            {
                try
                {
                    string serializedJson = File.ReadAllText(path);
                    return JsonConvert.DeserializeObject<T>(
                        serializedJson, jsonConverters
                    );
                }
                catch (Exception ex)
                {
                    Logger.Announce($"Could not parse JSON file, loading default values: {path}", LogLevel.Warn, true);
                    Logger.Error(ex.Message);
                    Logger.Error(ex.StackTrace);
                    return new T();
                }
            }
            else if (createFileIfNotExist)
            {
                T jsonObject = new T();
                Save(jsonObject, path, jsonConverters);
                return jsonObject;
            }
            else
            {
                return new T();
            }
        }

        /// <summary>
        /// Loads data from the JSON file at <paramref name="path"/> into the <paramref name="jsonObject"/>.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="jsonObject"/> to populate with JSON data.</typeparam>
        /// <param name="jsonObject">The <typeparamref name="T"/> instance to popular with JSON data.</param>
        /// <param name="path">The path on disk at which the JSON file can be found.</param>
        /// <param name="createFileIfNotExist">Whether a new JSON file should be created with default values if it does not
        /// already exist.</param>
        /// <param name="jsonConverters">An array of <see cref="JsonConverter"/>s to be used for deserialization.</param>
        /// <seealso cref="Load{T}(string, bool, JsonConverter[])"/>
        /// <seealso cref="Save{T}(T, string, JsonConverter[])"/>
        public static void Load<T>(T jsonObject, string path = null, bool createFileIfNotExist = true,
            params JsonConverter[] jsonConverters) where T : class
        {
            if (string.IsNullOrEmpty(path))
            {
                path = GetDefaultPath<T>(Assembly.GetCallingAssembly());
            }

            if (Directory.Exists(Path.GetDirectoryName(path)) && File.Exists(path))
            {
                try
                {
                    var jsonSerializerSettings = new JsonSerializerSettings()
                    {
                        Converters = jsonConverters,
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    };

                    string serializedJson = File.ReadAllText(path);
                    JsonConvert.PopulateObject(
                        serializedJson, jsonObject, jsonSerializerSettings
                    );
                }
                catch (Exception ex)
                {
                    Logger.Announce($"Could not parse JSON file, instance values unchanged: {path}", LogLevel.Warn, true);
                    Logger.Error(ex.Message);
                    Logger.Error(ex.StackTrace);
                }
            }
            else if (createFileIfNotExist)
            {
                Save(jsonObject, path, jsonConverters);
            }
        }

        /// <summary>
        /// Saves the <paramref name="jsonObject"/> parsed as JSON data to the JSON file at <paramref name="path"/>,
        /// creating it if it does not exist.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="jsonObject"/> to parse into JSON data.</typeparam>
        /// <param name="jsonObject">The <typeparamref name="T"/> instance to parse into JSON data.</param>
        /// <param name="path">The path on disk at which to store the JSON file.</param>
        /// <param name="jsonConverters">An array of <see cref="JsonConverter"/>s to be used for serialization.</param>
        /// <seealso cref="Load{T}(T, string, bool, JsonConverter[])"/>
        /// <seealso cref="Load{T}(string, bool, JsonConverter[])"/>
        public static void Save<T>(T jsonObject, string path = null,
            params JsonConverter[] jsonConverters) where T : class
        {
            if (string.IsNullOrEmpty(path))
            {
                path = GetDefaultPath<T>(Assembly.GetCallingAssembly());
            }

            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            using (var jsonTextWriter = new JsonTextWriter(stringWriter)
            {
                Indentation = 4,
                Formatting = Formatting.Indented
            })
            {
                var jsonSerializer = new JsonSerializer();
                foreach (var jsonConverter in jsonConverters)
                {
                    jsonSerializer.Converters.Add(jsonConverter);
                }
                jsonSerializer.Serialize(jsonTextWriter, jsonObject);
            }

            var fileInfo = new FileInfo(path);
            fileInfo.Directory.Create(); // Only creates the directory if it doesn't already exist
            File.WriteAllText(path, stringBuilder.ToString());
        }
    }
}
