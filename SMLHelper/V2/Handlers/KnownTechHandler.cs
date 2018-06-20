namespace SMLHelper.V2.Handlers
{
    using Patchers;

    /// <summary>
    /// A handler class for configuring custom unlocking conditions for item blueprints.
    /// </summary>
    public class KnownTechHandler
    {
        /// <summary>
        /// Adds a custom <see cref="KnownTech.AnalysisTech"/> to add conditions for when an item blueprint unlocks.
        /// </summary>
        /// <param name="analysisTech">The analysis tech.</param>        
        public static void AddToAnalysisTech(KnownTech.AnalysisTech analysisTech)
        {
            KnownTechPatcher.AnalysisTech.Add(analysisTech);
        }
    }
}
