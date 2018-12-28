namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class LanguagePatcher
    {
        public static Dictionary<string, string> customLines = new Dictionary<string, string>();
        private static Type languageType = typeof(Language);

        internal static void Postfix(ref Language __instance)
        {
            FieldInfo stringsField = languageType.GetField("strings", BindingFlags.NonPublic | BindingFlags.Instance);
            Dictionary<string, string> strings = stringsField.GetValue(__instance) as Dictionary<string, string>;
            foreach (KeyValuePair<string, string> a in customLines)
            {
                strings[a.Key] = a.Value;
            }
        }

        internal static void Patch(HarmonyInstance harmony)
        {
            MethodInfo method = languageType.GetMethod("LoadLanguageFile", BindingFlags.NonPublic | BindingFlags.Instance);

            harmony.Patch(method, null,
                new HarmonyMethod(typeof(LanguagePatcher).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic)));
            Logger.Log("LanguagePatcher is done.");
        }
    }
}
