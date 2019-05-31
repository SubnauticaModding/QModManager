namespace QModManager.API.SMLHelper.Patchers
{
    using Harmony;
    using QModManager.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    internal static class LanguagePatcher
    {
        private static readonly string LanguageDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Language");
        private static readonly string LanguageOrigDir = Path.Combine(LanguageDir, "Originals");
        private static readonly string LanguageOverDir = Path.Combine(LanguageDir, "Overrides");
        private const char KeyValueSeparator = ':';
        private const int SplitCount = 2;

        private static readonly Dictionary<string, Dictionary<string, string>> originalCustomLines = new Dictionary<string, Dictionary<string, string>>();
        private static readonly Dictionary<string, string> customLines = new Dictionary<string, string>();

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

            harmony.Patch(AccessTools.Method(typeof(Language), "LoadLanguageFile"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(LanguagePatcher), "Postfix")));

            Logger.Debug("LanguagePatcher is done.");
        }

        private static void WriteOriginalCustomLines()
        {
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
                Logger.Debug($"Updated {filesWritten} of {originalCustomLines.Count} original language files.");
        }

        private static void WriteOriginalLinesFile(string modKey)
        {
            Dictionary<string, string> modCustomLines = originalCustomLines[modKey];
            StringBuilder text = new StringBuilder();
            foreach (string langLineKey in modCustomLines.Keys)
            {
                string value = modCustomLines[langLineKey].Replace("\n", "\\n").Replace("\r", "\\r");
                text.AppendLine($"{langLineKey}{KeyValueSeparator}{value}");
            }

            File.WriteAllText(Path.Combine(LanguageOrigDir, $"{modKey}.txt"), text.ToString(), Encoding.UTF8);
        }

        private static void ReadOverrideCustomLines()
        {
            if (!Directory.Exists(LanguageOverDir))
                Directory.CreateDirectory(LanguageOverDir);

            string[] files = Directory.GetFiles(LanguageOverDir);

            Logger.Debug($"{files.Length} language override files found.");

            if (files.Length == 0)
                return;

            foreach (string file in files)
            {
                string modName = Path.GetFileNameWithoutExtension(file);

                if (!originalCustomLines.ContainsKey(modName))
                    continue; // Not for a mod we know about

                string[] languageLines = File.ReadAllLines(file, Encoding.UTF8);

                Dictionary<string, string> originalLines = originalCustomLines[modName];

                int overridesApplied = ExtractOverrideLines(modName, languageLines, originalLines);

                Logger.Info($"Applied {overridesApplied} language overrides to mod {modName}.");
            }
        }

        internal static int ExtractOverrideLines(string modName, string[] languageLines, Dictionary<string, string> originalLines)
        {
            int overridesApplied = 0;
            for (int lineIndex = 0; lineIndex < languageLines.Length; lineIndex++)
            {
                string line = languageLines[lineIndex];
                if (string.IsNullOrEmpty(line))
                    continue; // Skip empty lines

                string[] split = line.Split(new[] { KeyValueSeparator }, SplitCount, StringSplitOptions.RemoveEmptyEntries);

                string key = split[0];

                if (split.Length != SplitCount)
                {
                    Logger.Warn($"Line '{lineIndex}' in language override file for '{modName}' was incorrectly formatted.");
                    continue; // Not correctly formatted
                }

                if (!originalLines.ContainsKey(key))
                {
                    Logger.Warn($"Key '{key}' on line '{lineIndex}' in language override file for '{modName}' did not match an original key.");
                    continue; // Skip keys we don't recognize.
                }

                string value = RemoveOptionalDelimiters(split[1]);

                customLines[key] = value.Replace("\\n", "\n").Replace("\\r", "\r");
                overridesApplied++;
            }

            return overridesApplied;
        }

        private static string RemoveOptionalDelimiters(string value)
        {
            const int firstChar = 0;
            int lastChar = value.Length - 1;

            if (value[firstChar] == '{' && value[lastChar] == '}' &&
                value[firstChar + 2] != '}' && value[lastChar - 2] != '{')
            {
                value = value.Substring(1, lastChar - 1);
            }

            return value;
        }

        private static bool FileNeedsRewrite(string modKey)
        {
            Dictionary<string, string> modCustomLines = originalCustomLines[modKey];
            string fileName = Path.Combine(LanguageOrigDir, $"{modKey}.txt");

            if (!File.Exists(fileName))
                return true; // File not found

            string[] lines = File.ReadAllLines(fileName, Encoding.UTF8);

            if (lines.Length != modCustomLines.Count)
                return true; // Difference in line count

            // Confirm if the file actually needs to be updated
            foreach (string line in lines)
            {
                string[] split = line.Split(new[] { KeyValueSeparator }, SplitCount, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length != SplitCount)
                {
                    return true; // Not correctly formatted
                }

                string lineKey = split[0];
                string lineValue = split[1].Replace("\\n", "\n").Replace("\\r", "\r");

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

        internal static string GetCustomLine(string key)
        {
            return customLines[key];
        }
    }
}
