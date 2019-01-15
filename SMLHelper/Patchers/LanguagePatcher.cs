namespace SMLHelper.V2.Patchers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Harmony;

    internal class LanguagePatcher
    {
        private const string LanguageDir = "./QMods/Modding Helper/Language";
        private const string LanguageOrigDir = LanguageDir + "/Originals";
        private const string LanguageOverDir = LanguageDir + "/Overrides";

        private static readonly Dictionary<string, Dictionary<string, string>> OriginalCustomLines = new Dictionary<string, Dictionary<string, string>>();

        public static Dictionary<string, string> customLines = new Dictionary<string, string>();

        private static Type languageType = typeof(Language);

        internal static void Postfix(ref Language __instance)
        {
            FieldInfo stringsField = languageType.GetField("strings", BindingFlags.NonPublic | BindingFlags.Instance);
            var strings = stringsField.GetValue(__instance) as Dictionary<string, string>;
            foreach (KeyValuePair<string, string> a in customLines)
            {
                strings[a.Key] = a.Value;
            }
        }

        internal static void Patch(HarmonyInstance harmony)
        {
            if (!Directory.Exists(LanguageDir))
                Directory.CreateDirectory(LanguageDir);

            WriteOriginalCustomLines();

            ReadOverrideCustomLines();

            harmony.Patch(
                original: languageType.GetMethod("LoadLanguageFile", BindingFlags.NonPublic | BindingFlags.Instance),
                prefix: null,
                postfix: new HarmonyMethod(typeof(LanguagePatcher).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic)));

            Logger.Log("LanguagePatcher is done.");
        }

        private static void WriteOriginalCustomLines()
        {
            Logger.Log("Writing original language files.");

            if (!Directory.Exists(LanguageOrigDir))
                Directory.CreateDirectory(LanguageOrigDir);

            foreach (string modKey in OriginalCustomLines.Keys)
            {
                var text = new StringBuilder();
                foreach (string langLineKey in OriginalCustomLines[modKey].Keys)
                {
                    string value = OriginalCustomLines[modKey][langLineKey];
                    text.AppendLine($"{langLineKey}:'{value}'");
                }

                File.WriteAllText($"{LanguageOrigDir}/{modKey}.txt", text.ToString());
            }
        }

        private static void ReadOverrideCustomLines()
        {
            Logger.Log("Reading override language files.");

            if (!Directory.Exists(LanguageOverDir))
                Directory.CreateDirectory(LanguageOverDir);

            string[] files = Directory.GetFiles(LanguageOverDir);

            foreach (string file in files)
            {
                string modName = Path.GetFileName(file).Replace(".txt", string.Empty);

                if (!OriginalCustomLines.ContainsKey(modName))
                    continue; // Not for a mod we know about

                string[] languageLines = File.ReadAllLines(file);

                Dictionary<string, string> originalLines = OriginalCustomLines[modName];

                int overridesApplied = 0;

                foreach (string line in languageLines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue; // Skip empty lines

                    string[] split = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                    if (split.Length != 2)
                        continue; // Not correctly formatted

                    string key = split[0];

                    if (!originalLines.ContainsKey(key))
                        continue; // Skip keys we don't recognize.

                    string value = split[1];

                    customLines[key] = value.Trim('\'');
                    overridesApplied++;
                }

                Logger.Log($"Applied {overridesApplied} language overrides to mod {modName}.");
            }
        }

        internal static void AddCustomLanguageLine(string modAssemblyName, string lineId, string text)
        {
            if (!OriginalCustomLines.ContainsKey(modAssemblyName))
                OriginalCustomLines.Add(modAssemblyName, new Dictionary<string, string>());

            OriginalCustomLines[modAssemblyName][lineId] = text;
            customLines[lineId] = text;
        }
    }
}
