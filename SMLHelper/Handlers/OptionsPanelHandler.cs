namespace SMLHelper.V2.Handlers
{
    using Options;
    using Patchers;

    public class OptionsPanelHandler
    {
        public static void RegisterModOptions(ModOptions options)
        {
            OptionsPanelPatcher.modOptions.Add(options);
        }
    }
}
