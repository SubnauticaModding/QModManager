namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    internal class LanguagePatcher
    {
        private const string LanguageDir = "./QMods/Modding Helper/Language";
        private const string LanguageOrigDir = LanguageDir + "/Originals";
        private const string LanguageOverDir = LanguageDir + "/Overrides";
        private const char TextDelimiterOpen = '{';
        private const char TextDelimiterClose = '}';
        private const char KeyValueSeparator = ':';

        private static readonly Dictionary<string, Dictionary<string, string>> originalCustomLines = new Dictionary<string, Dictionary<string, string>>();
        private static readonly Dictionary<string, string> customLines = new Dictionary<string, string>();

        private static Type languageType = typeof(Language);

        internal static void Postfix(ref Language __instance)
        {
            Dictionary<string, string> strings = __instance.strings;
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

            Logger.Log("LanguagePatcher is done.", LogLevel.Debug);
        }

        private static void WriteOriginalCustomLines()
        {
            Logger.Log("Writing original language files.", LogLevel.Debug);

            if (!Directory.Exists(LanguageOrigDir))
                Directory.CreateDirectory(LanguageOrigDir);

            if (originalCustomLines.Count == 0)
                return;

            int filesWritten = 0;
            foreach (string modKey in originalCustomLines.Keys)
            {
                if (!FileNeedsRewrite(modKey))
                    continue; // File is identical to captured lines. No need to rewrite it.

                WriteOriginalLinesFile(modKey);
                filesWritten++;
            }

            if (filesWritten > 0)
                Logger.Log($"Updated {filesWritten} of {originalCustomLines.Count} original language files.", LogLevel.Info);
        }

        private static void WriteOriginalLinesFile(string modKey)
        {
            Dictionary<string, string> modCustomLines = originalCustomLines[modKey];
            var text = new StringBuilder();
            foreach (string langLineKey in modCustomLines.Keys)
            {
                string value = modCustomLines[langLineKey];
                text.AppendLine($"{langLineKey}{KeyValueSeparator}{TextDelimiterOpen}{value}{TextDelimiterClose}");
            }

            File.WriteAllText($"{LanguageOrigDir}/{modKey}.txt", text.ToString(), Encoding.UTF8);
        }

        private static void ReadOverrideCustomLines()
        {
            if (!Directory.Exists(LanguageOverDir))
                Directory.CreateDirectory(LanguageOverDir);

            string[] files = Directory.GetFiles(LanguageOverDir);

            if (files.Length == 0)
                return;

            Logger.Log($"{files.Length} language override files found.", LogLevel.Debug);

            foreach (string file in files)
            {
                string modName = Path.GetFileName(file).Replace(".txt", string.Empty);

                if (!originalCustomLines.ContainsKey(modName))
                    continue; // Not for a mod we know about

                string[] languageLines = File.ReadAllLines(file, Encoding.UTF8);

                Dictionary<string, string> originalLines = originalCustomLines[modName];

                int overridesApplied = 0;
                for (int lineIndex = 0; lineIndex < languageLines.Length; lineIndex++)
                {
                    string line = languageLines[lineIndex];
                    if (string.IsNullOrEmpty(line))
                        continue; // Skip empty lines

                    string[] split = line.Split(new[] { KeyValueSeparator }, StringSplitOptions.RemoveEmptyEntries);

                    if (split.Length != 2)
                    {
                        Logger.Log($"Line '{lineIndex}' in language override file for '{modName}' was not correctly formatted.", LogLevel.Warn);
                        continue; // Not correctly formatter
                    }

                    string key = split[0];

                    if (!originalLines.ContainsKey(key))
                    {
                        Logger.Log($"Key '{key}' on line '{lineIndex}' in language override file for '{modName}' did not match an original key.", LogLevel.Warn);
                        continue; // Skip keys we don't recognize.
                    }

                    customLines[key] = TrimTextDelimiters(split[1]);
                    overridesApplied++;
                }

                Logger.Log($"Applied {overridesApplied} language overrides to mod {modName}.", LogLevel.Info);
            }
        }

        private static bool FileNeedsRewrite(string modKey)
        {
            Dictionary<string, string> modCustomLines = originalCustomLines[modKey];
            string fileName = $"{LanguageOrigDir}/{modKey}.txt";

            if (!File.Exists(fileName))
                return true; // File not found

            string[] lines = File.ReadAllLines(fileName, Encoding.UTF8);

            if (lines.Length != modCustomLines.Count)
                return true; // Difference in line count

            // Confirm if the file actually needs to be updated
            foreach (string line in lines)
            {
                string[] split = line.Split(new[] { KeyValueSeparator }, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length != 2)
                    return true; // Malformatted, likely externally edited

                string lineKey = split[0];
                string lineValue = TrimTextDelimiters(split[1]);

                if (modCustomLines.TryGetValue(lineKey, out string origValue))
                {
                    if (origValue != lineValue)
                    {
                        return true; // Difference in line content
                    }
                }
                else
                {
                    return true; // Key not found
                }
            }

            return false; // All lines matched and valid
        }

        internal static void AddCustomLanguageLine(string modAssemblyName, string lineId, string text)
        {
            if (!originalCustomLines.ContainsKey(modAssemblyName))
                originalCustomLines.Add(modAssemblyName, new Dictionary<string, string>());

            originalCustomLines[modAssemblyName][lineId] = text;
            customLines[lineId] = text;
        }

        internal static string TrimTextDelimiters(string value)
        {
            return value.Trim(TextDelimiterOpen, TextDelimiterClose);
        }
    }
}
