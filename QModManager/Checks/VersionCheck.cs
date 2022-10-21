namespace QModManager.Checks
{
    using System;
    using System.Net;
    using System.Reflection;
    using QModManager.Utility;
    using UnityEngine;
    using Logger = Utility.Logger;

    internal static class VersionCheck
    {
        internal const string snNexus = "https://nexusmods.com/subnautica/mods/201";
        internal const string bzNexus = "https://nexusmods.com/subnauticabelowzero/mods/1";
        internal const string VersionURL = "https://raw.githubusercontent.com/SubnauticaModding/QModManager/master/Data/latest-version.txt";

        internal static Version result = null;

        internal static void Check()
        {
            if (!Config.CheckForUpdates)
            {
                Logger.Info("QMM Internal Versionchecker: Update check disabled");
                return;
            }

            if (!NetworkUtilities.CheckConnection())
            {
                Logger.Info("QMM Internal Versionchecker: Cannot check for updates, internet disabled");
                return;
            }

            ServicePointManager.ServerCertificateValidationCallback = NetworkUtilities.CustomSCVC;

            using (var client = new WebClient())
            {
                client.DownloadStringCompleted += (sender, e) =>
                {
                    if (e.Error != null)
                    {
                        Logger.Error("QMM Internal Versionchecker: There was an error retrieving the latest version from GitHub!");
                        Logger.Exception(e.Error);
                        return;
                    }
                    Parse(e.Result);
                };

                Logger.Debug("QMM Internal Versionchecker: Getting the latest version...");
                client.DownloadStringAsync(new Uri(VersionURL));
            }
        }

        internal static void Parse(string versionStr)
        {
            try
            {
                Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                if (versionStr == null)
                {
                    Logger.Error("QMM Internal Versionchecker: There was an error retrieving the latest version from GitHub!");
                    return;
                }

                string[] versionStr_splittet = versionStr.Split('.');
                string version_builder = $"{versionStr_splittet[0]}.{(versionStr_splittet.Length >= 2 ? $"{versionStr_splittet[1]}" : "0")}.{(versionStr_splittet.Length >= 3 ? $"{versionStr_splittet[2]}" : "0")}.{(versionStr_splittet.Length >= 4 ? $"{versionStr_splittet[3]}" : "0")}";
                var latestVersion = new Version(version_builder);

                if (latestVersion == null)
                {
                    Logger.Error("QMM Internal Versionchecker: There was an error retrieving the latest version from GitHub!");
                    return;
                }
                
                //Logger.Debug($"QMM Version Checker - Parse - current Version value: {currentVersion}");
                //Logger.Debug($"QMM Version Checker - Parse - latest Version value: {latestVersion}");

                if (latestVersion > currentVersion)
                {
                    Logger.Info($"Newer version found: {latestVersion.ToStringParsed()} (current version: {currentVersion.ToStringParsed()})");
                    result = latestVersion;
                }
                else if (latestVersion < currentVersion)
                {
                    Logger.Info($"QMM Internal Versionchecker: Received latest version ({latestVersion.ToStringParsed()}) from GitHub. We're ahead. This is probably a development build (current version: {currentVersion.ToStringParsed()}).");
                }
                else
                {
                    Logger.Info($"QMM Internal Versionchecker: Received latest version from GitHub. We are up to date!");
                }
            }
            catch (Exception e)
            {
                Logger.Error("QMM Internal Versionchecker: There was an error retrieving the latest version from GitHub!");
                Logger.Exception(e);
                return;
            }
        }
    }
}
