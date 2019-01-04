using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            if (!Application.isPlaying && Assembly.GetCallingAssembly().GetName().Name == "QModManager" && strings == null && backupStrings == null)
            {
                if (!Environment.CurrentDirectory.Contains("Subnautica_Data") || !Environment.CurrentDirectory.Contains("Managed"))
                {
                    Console.WriteLine("There was an error initializing the dictionaries from the console app");
                    Console.WriteLine("Please make sure you have installed QModManager in the right folder");
                    return "key";
                }
                string QMMDir = Path.Combine(Environment.CurrentDirectory, "../../QModManager");
                if (!Directory.Exists(QMMDir))
                {
                    Console.WriteLine("There was an error initializing the dictionaries from the console app");
                    Console.WriteLine("The QModManager directory is missing");
                    return key;
                }
                string languageFile = Path.Combine(QMMDir, "installer_language.txt");
                string overridePath = Path.Combine(QMMDir, "../QMods");
                if (!File.Exists(languageFile))
                {
                    Console.WriteLine("There was an error initializing the dictionaries from the console app");
                    Console.WriteLine("The installer_language.txt file is missing!");
                    Console.WriteLine("Loading English...");
                    if (LoadLanguageFile("English", silent: true, path: overridePath)) return Get(key);
                    Console.WriteLine("Error loading English!");
                    return key;
                }
                try
                {
                    string text = File.ReadAllText(languageFile);
                    if (LoadLanguageFile(text, silent: true, path: overridePath)) return Get(key);
                    if (LoadLanguageFile("English", silent: true, path: overridePath)) return Get(key);
                    return key;
                }
                catch
                {
                    if (LoadLanguageFile("English", silent: true, path: overridePath)) return Get(key);
                    return key;
                }
            }
            if (strings.ContainsKey(key)) return strings[key];
            if (backupStrings.ContainsKey(key)) return backupStrings[key];
            return key;
        }
        public static string GetFormatted(string key, params object[] args)
        {
            if (string.IsNullOrEmpty(key)) return null;
            string value = Get(key);
            if (value == null) return key;
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
        internal static bool LoadLanguageFile(string language, bool backup = false, bool silent = false, string path = null)
        {
            try
            {
                if (path == null) path = Path.Combine(QModPatcher.QModBaseDir, "../QModManager/Localization/" + language + ".json");
                else path = Path.Combine(path, "../QModManager/Localization/" + language + ".json");
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
                if (!silent) Console.WriteLine($"[QModManager] {Get("LanguageHelper_Load")} {language}...");
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
