namespace SMLHelper.V2.Patchers
{
    using System.Reflection;
    using Harmony;

    internal class OptionsPanelPatcher
    {
        internal static void Patch(HarmonyInstance harmony)
        {
            var uGUI_OptionsPanelType = typeof(uGUI_OptionsPanel);
            var thisType = typeof(OptionsPanelPatcher);
            var startMethod = uGUI_OptionsPanelType.GetMethod("AddTabs", BindingFlags.NonPublic | BindingFlags.Instance);

            harmony.Patch(startMethod, null, new HarmonyMethod(thisType.GetMethod("AddTabs_Postfix", BindingFlags.NonPublic | BindingFlags.Static)));
        }

        internal static void AddTabs_Postfix(uGUI_OptionsPanel __instance)
        {
            var optionsPanel = __instance;

            // Make the Mods tab
            int modsTab = optionsPanel.AddTab("Mods");

            optionsPanel.AddHeading(modsTab, "Test Heading");
            optionsPanel.AddSliderOption(modsTab, "Test Slider", 1.0f, 1.0f);
            optionsPanel.AddToggleOption(modsTab, "Test Toggle", false);
        }
    }
}
