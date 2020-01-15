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
            set => Get("Enable debug logs", value);
        }

        internal static bool EnableDevMode
        {
            get => Get("Enable developer mode", false);
            set => Set("Enable developer mode", value);
        }

        private static readonly string ConfigPath = Path.Combine(Environment.CurrentDirectory, "qmodmanager-config.json");

        private static Dictionary<string, object> Cfg = new Dictionary<string, object>();
        private static bool Loaded = false;

        private static void Load()
        {
            if (!File.Exists(ConfigPath)) File.WriteAllText(ConfigPath, "{}");
            string text = File.ReadAllText(ConfigPath);
            Cfg = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
        }

        private static void Save()
        {
            string text = JsonConvert.SerializeObject(Cfg, Formatting.Indented);
            File.WriteAllText(ConfigPath, text);
        }

        private static object Get(string field, object def = null)
        {
            try
            {
                if (!Loaded) Load();

                if (Cfg.TryGetValue(field, out object value))
                    return value;

                return def;
            }
            catch (Exception e)
            {
                Logger.Error($"There was an error retrieving the value \"{field}\" from the config file.");
                Logger.Exception(e);

                return null;
            }
        }

        private static T Get<T>(string field, T def = default)
        {
            object value = Get(field);
            if (value != null && value is T) return (T)value;
            return def;
        }

        private static object Set(string field, object value, bool save = true)
        {
            if (!Loaded) Load();

            Cfg.TryGetValue(field, out object oldValue);
            Cfg.Remove(field);
            Cfg.Add(field, value);
            if (save) Save();
            return oldValue;
        }
    }
}
