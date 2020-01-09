namespace QModManager.Utility
{
    using Oculus.Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal static class Config
    {
        internal static class FIELDS
        {
            internal const string CHECK_FOR_UPDATES = "Check for updates";
            internal const string ENABLE_CONSOLE = "Enable console";
            internal const string ENABLE_DEBUG_LOGS = "Enable debug logs";
            internal const string ENABLE_DEV_MODE = "Enable developer mode";
        }

        internal static readonly string ConfigPath = Path.Combine(Environment.CurrentDirectory, "qmodmanager-config.json");

        private static Dictionary<string, object> Cfg = new Dictionary<string, object>();
        private static bool Loaded = false;

        internal static void Load()
        {
            if (!File.Exists(ConfigPath)) File.WriteAllText(ConfigPath, "{}");
            string text = File.ReadAllText(ConfigPath);
            Cfg = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
        }

        internal static void Save()
        {
            string text = JsonConvert.SerializeObject(Cfg, Formatting.Indented);
            File.WriteAllText(ConfigPath, text);
        }

        internal static object Get(string field, object def = null)
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

        internal static T Get<T>(string field, T def = default)
        {
            object value = Get(field);
            if (value != null && value is T) return (T)value;
            return def;
        }

        internal static object Set(string field, object value, bool save = true)
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
