// Original mod by AlexejheroYTB
namespace QModManager.HarmonyPatches.EnableAchievements
{
    using Harmony;

    [HarmonyPatch(typeof(GameAchievements), "Unlock")]
    internal static class GameAchievements_Unlock
    {
        [HarmonyPrefix]
        internal static bool Prefix(GameAchievements.Id id)
        {
            PlatformUtils.main.GetServices().UnlockAchievement(id);
            return false;
        }
    }
}
