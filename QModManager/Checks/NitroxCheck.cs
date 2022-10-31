using System;

namespace QModManager.Checks
{
    internal static class NitroxCheck
    {
        internal static bool IsRunning => Environment.GetEnvironmentVariable("NITROX_LAUNCHER_PATH") is not null;
    }
}
