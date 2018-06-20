namespace SMLHelper.V2.Handlers
{
    using Patchers;

    public class KnownTechHandler
    {
        public static void AddToAnalysisTech(KnownTech.AnalysisTech analysisTech)
        {
            KnownTechPatcher.AnalysisTech.Add(analysisTech);
        }
    }
}
