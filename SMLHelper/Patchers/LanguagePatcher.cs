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
        private static readonly string LanguageDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Language");
        private static readonly string LanguageOrigDir = Path.Combine(LanguageDir, "Originals");
        private static readonly string LanguageOverDir = Path.Combine(LanguageDir, "Overrides");
        private const char KeyValueSeparator = ':';

        private static readonly Dictionary<string, Dictionary<string, string>> originalCustomLines = new Dictionary<string, Dictionary<string, string>>();
        private static readonly Dictionary<string, string> customLines = new Dictionary<string, string>();

        internal static void Postfix(ref Language __instance)
        {
            // Direct access to private fields made possible by https://github.com/CabbageCrow/AssemblyPublicizer/
            // See README.md for details.
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

            Logger.Log("LanguagePatcher is done.", LogLevel.Debug);
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
                Logger.Log($"Updated {filesWritten} of {originalCustomLines.Count} original language files.", LogLevel.Debug);
        }

        private static void WriteOriginalLinesFile(string modKey)
        {
            Dictionary<string, string> modCustomLines = originalCustomLines[modKey];
            var text = new StringBuilder();
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

            Logger.Log($"{files.Length} language override files found.", LogLevel.Debug);

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

                Logger.Log($"Applied {overridesApplied} language overrides to mod {modName}.", LogLevel.Info);
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

                string[] split = line.Split(new[] { KeyValueSeparator }, 2, StringSplitOptions.RemoveEmptyEntries);

                string key = split[0];

                if (split.Length < 2)
                {
                    Logger.Log($"Line '{lineIndex}' in language override file for '{modName}' was incorrectly formatted.", LogLevel.Warn);
                    continue; // Not correctly formatted
                }

                if (!originalLines.ContainsKey(key))
                {
                    Logger.Log($"Key '{key}' on line '{lineIndex}' in language override file for '{modName}' did not match an original key.", LogLevel.Warn);
                    continue; // Skip keys we don't recognize.
                }

                customLines[key] = split[1].Replace("\\n", "\n").Replace("\\r", "\r");
                overridesApplied++;
            }

            return overridesApplied;
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
                string[] split = line.Split(new[] { KeyValueSeparator }, 2, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length < 2)
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
