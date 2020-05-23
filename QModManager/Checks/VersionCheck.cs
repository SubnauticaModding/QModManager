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
            if (PlayerPrefs.GetInt("QModManager_EnableUpdateCheck", 1) == 0)
            {
                Logger.Info("Update check disabled");
                return;
            }

            if (!NetworkUtilities.CheckConnection())
            {
                Logger.Info("Cannot check for updates, internet disabled");
                return;
            }

            ServicePointManager.ServerCertificateValidationCallback = NetworkUtilities.CustomSCVC;

            using (var client = new WebClient())
            {
                client.DownloadStringCompleted += (sender, e) =>
                {
                    if (e.Error != null)
                    {
                        Logger.Error("There was an error retrieving the latest version from GitHub!");
                        Logger.Exception(e.Error);
                        return;
                    }
                    Parse(e.Result);
                };

                Logger.Debug("Getting the latest version...");
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
                    Logger.Error("There was an error retrieving the latest version from GitHub!");
                    return;
                }
                var latestVersion = new Version(versionStr);
                if (latestVersion == null)
                {
                    Logger.Error("There was an error retrieving the latest version from GitHub!");
                    return;
                }
                if (latestVersion > currentVersion)
                {
                    Logger.Info($"Newer version found: {latestVersion.ToStringParsed()} (current version: {currentVersion.ToStringParsed()})");
                    result = latestVersion;
                }
                else if (latestVersion < currentVersion)
                {
                    Logger.Info($"Received latest version from GitHub. We're ahead. This is probably a development build.");
                }
                else
                {
                    Logger.Info($"Received latest version from GitHub. We are up to date!");
                }
            }
            catch (Exception e)
            {
                Logger.Error("There was an error retrieving the latest version from GitHub!");
                Logger.Exception(e);
                return;
            }
        }
    }
}
