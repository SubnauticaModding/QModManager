using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace QModManager
{
    public static class LanguageHelper
    {
        internal static Dictionary<string, string> strings;
        internal static Dictionary<string, string> english;

        public static string Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            if (strings.ContainsKey(key)) return strings[key];
            if (english.ContainsKey(key)) return english[key];
            return null;
        }

        internal static void Init()
        {
            Language.main.OnLanguageChanged += () => LanguageChanged();
            LoadLanguageFile("English", ref english);
            if (!LoadLanguageFile(PlayerPrefs.GetString("language"))) LoadLanguageFile("English");
        }
        internal static void LanguageChanged()
        {
            if (!LoadLanguageFile(PlayerPrefs.GetString("language"))) LoadLanguageFile("English");
        }
        internal static bool LoadLanguageFile(string language) => LoadLanguageFile(language, ref strings);
        internal static bool LoadLanguageFile(string language, ref Dictionary<string, string> result)
        {
            try
            {
                string path = Path.Combine(QModPatcher.QModBaseDir, "../QModManager/Localization/" + language + ".json");
                if (!File.Exists(path)) return false;
                string text = File.ReadAllText(path);
                Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                if (dictionary == null) return false;
                result = dictionary;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }
}
