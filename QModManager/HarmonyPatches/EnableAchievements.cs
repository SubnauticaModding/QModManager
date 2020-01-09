// Original mod by AlexejheroYTB
namespace QModManager.HarmonyPatches.EnableAchievements
{
    using Harmony;

    [HarmonyPatch(typeof(GameAchievements), nameof(GameAchievements.Unlock))]
    internal static class GameAchievements_Unlock_Patch
    {
        // This patch allows achievements to be earned even if console commands were used

        [HarmonyPrefix]
        internal static bool Prefix(GameAchievements.Id id)
        {
            PlatformUtils.main.GetServices().UnlockAchievement(id);
            return false;
        }
    }
}
