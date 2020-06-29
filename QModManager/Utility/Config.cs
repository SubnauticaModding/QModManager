namespace QModManager.Utility
{
    using Oculus.Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal static class Config
    {
        internal static bool CheckForUpdates
        {
            get => Get("Check for updates", true);
            set => Set("Check for updates", value);
        }

        internal static bool EnableConsole
        {
            get => Get("Enable console", false);
            set => Set("Enable console", value);
        }

        internal static bool EnableDebugLogs
        {
            get => Get("Enable debug logs", false);
            set => Set("Enable debug logs", value);
        }

        internal static bool EnableDevMode
        {
            get => Get("Enable developer mode", false);
            set => Set("Enable developer mode", value);
        }

        private static readonly string ConfigPath = Path.Combine(Environment.CurrentDirectory, "qmodmanager-config.json");

        private static Dictionary<string, object> Cfg = new Dictionary<string, object>();
        private static bool Loaded = false;
        private static readonly JsonSerializer serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

    private static void Load()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    Save();
                }

                using StreamReader sr = new StreamReader(ConfigPath);
                using JsonReader reader = new JsonTextReader(sr);
                Cfg = serializer.Deserialize<Dictionary<string, object>>(reader);

                if (Cfg == null) 
                {
                    Cfg = new Dictionary<string, object>();
                    Save();
                }

                Loaded = true;
            }
            catch (Exception e)
            {
                Logger.Error($"There was an error loading the config file.");
                Logger.Exception(e);
            }
        }

        private static void Save()
        {
            try
            {
                using StreamWriter sw = new StreamWriter(ConfigPath);
                using JsonWriter writer = new JsonTextWriter(sw);
                serializer.Serialize(writer, Cfg);
            }
            catch (Exception e)
            {
                Logger.Error($"There was an error saving the config file.");
                Logger.Exception(e);
            }
        }

        private static T Get<T>(string field, T def = default)
        {
            if (!Loaded)
            {
                Load();
            }

            if (!Cfg.TryGetValue(field, out object value))
                return def;

            if (value is T typedValue) return typedValue;

            return def;
        }

        private static void Set(string field, object value)
        {
            if (!Loaded) Load();

            Cfg[field] = value;

            Save();
        }
    }
}
