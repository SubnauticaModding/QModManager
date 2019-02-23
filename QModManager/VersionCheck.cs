using System;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace QModManager
{
    internal static class VersionCheck
    {
        internal const string snNexus = "https://nexusmods.com/subnautica/mods/201";
        internal const string bzNexus = "https://nexusmods.com/subnauticabelowzero/mods/1";
        internal const string VersionURL = "https://raw.githubusercontent.com/QModManager/QModManager/dev/error-fixing/Data/latest-version.txt";

        internal static void Check()
        {
            if (PlayerPrefs.GetInt("QModManager_EnableUpdateCheck", 1) == 0)
            {
                Logger.Info("Update check disabled");
                return;
            }

            ServicePointManager.ServerCertificateValidationCallback = CustomRemoteCertificateValidationCallback;

            using (WebClient client = new WebClient())
            {
                client.DownloadStringCompleted += (sender, e) =>
                {
                    if (e.Error != null)
                    {
                        Logger.Error("There was an error retrieving the latest version from GitHub!");
                        Debug.LogException(e.Error);
                        return;
                    }
                    Parse(e.Result);
                };

                Logger.Debug("Getting the latest version...");
                client.DownloadStringAsync(new Uri(VersionURL));
            }
        }
        private static void Parse(string versionStr)
        {
            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (versionStr == null)
            {
                Logger.Error("There was an error retrieving the latest version from GitHub!");
                return;
            }
            Version latestVersion = new Version(versionStr);
            if (latestVersion == null)
            {
                Logger.Error("There was an error retrieving the latest version from GitHub!");
                return;
            }
            if (latestVersion > currentVersion)
            {
                Logger.Info($"Newer version found: {latestVersion.ToString()} (current version: {currentVersion.ToString()}");
                if (Patcher.erroredMods.Count <= 0)
                {
                    Dialog.Show(
                        $"There is a newer version of QModManager available: {latestVersion.ToString()} (current version: {currentVersion.ToString()})",
                        Dialog.Button.download, Dialog.Button.close, true);
                }
            }
            else 
            {
                Logger.Info($"Recieved latest version from GitHub. We are up to date!");
            }
        }

        internal static bool CustomRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain,
            // look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        continue;
                    }
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                        break;
                    }
                }
            }
            return isOk;
        }
    }
}
