namespace QModManager.Checks
{
    using System;
    using System.Net;
    using System.Reflection;
    using QModManager.Patching;
    using QModManager.Utility;
    using UnityEngine;
    using Logger = Utility.Logger;

    internal static class VersionCheck
    {
        internal const string snNexus = "https://nexusmods.com/subnautica/mods/201";
        internal const string bzNexus = "https://nexusmods.com/subnauticabelowzero/mods/1";
        internal const string VersionURL = "https://raw.githubusercontent.com/SubnauticaModding/QModManager/master/Data/latest-version.txt";

        internal static void Check()
        {
            if (PlayerPrefs.GetInt("QModManager_EnableUpdateCheck", 1) == 0)
            {
                Logger.Info("Update check disabled");
                return;
            }

            if (!NetworkUtilities.CheckConnection())
            {
                Logger.Warn("Cannot check for updates, internet disabled");
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
                    Logger.Info($"Newer version found: {latestVersion.ToStringParsed()} (current version: {currentVersion.ToStringParsed()}");
                    if (Patcher.ErrorModCount <= 0)
                    {
                        Dialog.Show(
                            $"There is a newer version of QModManager available: {latestVersion.ToStringParsed()} (current version: {currentVersion.ToStringParsed()})",
                            Dialog.Button.download, Dialog.Button.close, true);
                    }
                }
                else
                {
                    Logger.Info($"Recieved latest version from GitHub. We are up to date!");
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
