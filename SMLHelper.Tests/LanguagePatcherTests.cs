namespace SMLHelper.Tests
{
    using NUnit.Framework;
    using SMLHelper.V2.Patchers;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class LanguagePatcherTests
    {
        private static readonly IEnumerable<string> Keys = new string[]
        {
            "Key",
            "Tooltip_Key",
        };

        private static readonly IEnumerable<string> CustomValues = new string[]
        {
            // No special tokens
            "CustomValue",
            "CustomValue1",
            "2Custom:Value1",
            "CustomValue%",
            // string.format tokens
            "CustomValue{0}",
            "{0}CustomValue",
            "{0}CustomValue{1}",
            "{0}Custom{1}Value{2}",
            "Custom{0}Value",
            // With Unity line breaks
            "Custom\\nValue",
            "\\nCustomValue",
            "CustomValue\\n",
            "\\nCustom\\nValue\\n",
            // With mix
            "Custom:{0}\\n{1}:Value;",
            "Custom-Value\\n{0}",
            "#1\\nCustom_Value\\n",
            "Custom{0}:{1}%Value%",
        };

        [Test, Combinatorial]
        public void ExtractOverrideLines_WhenTextIsValid_SingleEntry_KeyIsKnown_Overrides(
            [ValueSource(nameof(CustomValues))] string customValue)
        {
            var originalLines = new Dictionary<string, string>
            {
                { "Key", "OriginalValue" }
            };

            string text = "Key:" + customValue;

            Console.WriteLine("TestText");
            Console.WriteLine(text);
            int overridesApplied = LanguagePatcher.ExtractOverrideLines("Test1", new[] { text }, originalLines);

            Assert.AreEqual(1, overridesApplied);
            Assert.AreEqual(customValue.Replace("\\n", "\n"), LanguagePatcher.GetCustomLine("Key"));
        }


        [Test, Combinatorial]
        public void ExtractOverrideLines_WhenTextIsValid_MultipleEntries_KeyIsKnown_Overrides(
            [ValueSource(nameof(CustomValues))] string otherCustomValue,
            [ValueSource(nameof(Keys))] string secondKey)
        {
            var originalLines = new Dictionary<string, string>
            {
                { "Key1", "OriginalValue1" },
                { secondKey, "OriginalValue2" },
            };

            string line1 = "Key1:CustomValue1";
            string line2 = secondKey + ":" + otherCustomValue;
            Console.WriteLine("TestText");
            Console.WriteLine(line1);
            Console.WriteLine(line2);
            int overridesApplied = LanguagePatcher.ExtractOverrideLines("Test1", new[] { line1, line2 }, originalLines);

            Assert.AreEqual(2, overridesApplied);
            Assert.AreEqual("CustomValue1", LanguagePatcher.GetCustomLine("Key1"));
            Assert.AreEqual(otherCustomValue.Replace("\\n", "\n"), LanguagePatcher.GetCustomLine(secondKey));
        }

        [Test, Combinatorial]
        public void ExtractOverrideLines_WhenLegacyDelimitersPresent_Overrides(
            [ValueSource(nameof(CustomValues))] string customValue)
        {
            var originalLines = new Dictionary<string, string>
            {
                { "Key", "OriginalValue" }
            };

            string text = "Key:{" + customValue + "}";

            Console.WriteLine("TestText");
            Console.WriteLine(text);
            int overridesApplied = LanguagePatcher.ExtractOverrideLines("Test1", new[] { text }, originalLines);

            Assert.AreEqual(1, overridesApplied);
            Assert.AreEqual(customValue.Replace("\\n", "\n"), LanguagePatcher.GetCustomLine("Key"));
        }
    }
}
