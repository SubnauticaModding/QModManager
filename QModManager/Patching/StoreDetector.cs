using System;
using QModManager.Checks;

namespace QModManager.Patching
{
    internal class StoreDetector
    {
        internal static string GetUsedGameStore()
        {
            if (NonValidStore())
            {
                return "Invalid store";
            }

            if (IsSteam())
            {
                return "Steam";
            }

            if (IsEpic())
            {
                return "Epic Games";
            }

            if (IsMSStore())
            {
                return "MSStore";
            }

            return "Unable to identify game store.";
        }

        private static bool IsSteam()
        {
            return PlatformServicesUtils.IsRuntimePluginDllPresent("CSteamworks");
        }

        private static bool IsEpic()
        {
            return Array.IndexOf(Environment.GetCommandLineArgs(), "-EpicPortal") != -1;
        }

        private static bool IsMSStore()
        {
            return PlatformServicesUtils.IsRuntimePluginDllPresent("XGamingRuntimeThunks");
        }

        private static bool NonValidStore()
        {
            return PirateCheck.PirateDetected;
        }
    }
}
