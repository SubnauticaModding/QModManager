using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QModManager
{
    public static class LanguageHelper
    {
        internal static Dictionary<string, string> strings;
        internal static Dictionary<string, string> backupStrings;
        internal static string language;

        public static Location Contains(string key)
        {
            if (strings.ContainsKey(key)) return Location.Main;
            if (backupStrings.ContainsKey(key)) return Location.Backup;
            return Location.Missing;
        }
        public static string Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            if (strings == null && backupStrings == null)
            {

            }
            if (strings.ContainsKey(key)) return strings[key];
            if (backupStrings.ContainsKey(key)) return backupStrings[key];
            return null;
        }
        public static string GetFormatted(string key, params object[] args)
        {
            if (string.IsNullOrEmpty(key)) return null;
            string value = Get(key);
            if (value == null) return null;
            if (args == null) return value;
            if (args.Length <= 0) return value;
            try
            {
                return string.Format(value, args);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.ToString());
                return value;
            }
        }

        internal static void Init()
        {
            Hooks.Start += () => Language.main.OnLanguageChanged += () => LanguageChanged();
            LoadLanguageFile("English", true);
            language = PlayerPrefs.GetString("Language");
            if (!LoadLanguageFile(language)) LoadLanguageFile("English");
        }
        internal static void LanguageChanged()
        {
            if (!LoadLanguageFile(PlayerPrefs.GetString("language"))) LoadLanguageFile("English");
        }
        internal static bool LoadLanguageFile(string language, bool backup = false)
        {
            try
            {
                string path = Path.Combine(QModPatcher.QModBaseDir, "../QModManager/Localization/" + language + ".json");
                if (!File.Exists(path)) return false;
                string text = File.ReadAllText(path);
                Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                if (dictionary == null) return false;
                if (backup) backupStrings = dictionary;
                else
                {
                    strings = dictionary;
                    LanguageHelper.language = language;
                }
                Console.WriteLine($"[QModManager] {Get("LanguageHelper_Load")} {language}...");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public enum Location
        {
            Missing,
            Backup,
            Main,
        }
    }
}
