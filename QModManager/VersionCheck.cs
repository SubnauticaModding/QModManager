using Oculus.Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace QModManager
{
    internal static class VersionCheck
    {
        internal const string newVersionDisplayPrefix = "There is a newer version of QModManager available: ";
        internal const string newVersionDisplaySuffix = " (current version: ";
        internal const string newVersionDisplaySuffix2 = ")";

        internal const string nexusmodsURL = "https://nexusmods.com/subnautica/mods/16";

        internal static void Check()
        {
            string versionStr = Get();
            if (versionStr == null)
            {
                UnityEngine.Debug.Log("Could not get latest version! (string is null)");
                return;
            }
            VersionWrapper wrapper = JsonConvert.DeserializeObject<VersionWrapper>(versionStr);
            if (wrapper == null)
            {
                UnityEngine.Debug.Log("Could not get latest version! (VersionWrapper is null)");
                return;
            }
            Version version = new Version(wrapper.version);
            if (version == null)
            {
                UnityEngine.Debug.Log("Could not get latest version! (Version is null)");
                return;
            }
            if (!version.Equals(QMod.QModManagerVersion) && QModPatcher.erroredMods.Count <= 0) Dialog.Show(newVersionDisplayPrefix + version.ToString() + newVersionDisplaySuffix + QMod.QModManagerVersion.ToString() + newVersionDisplaySuffix2, () => Process.Start(nexusmodsURL), leftButtonText: "Download", blue: true);
            // Longest line ever - will change in the next pr (trust me)
        }

        internal const string VersionURL = "https://raw.githubusercontent.com/QModManager/QModManager/version-check/version.json";

        internal static string Get()
        {
            string result = null;
            ServicePointManager.ServerCertificateValidationCallback = CustomRemoteCertificateValidationCallback;

            using (WebClient client = new WebClient())
            {
                client.DownloadStringAsync(new Uri(VersionURL));
                client.DownloadStringCompleted += (sender, e) =>
                {
                    if (e.Cancelled)
                    {
                        UnityEngine.Debug.Log("CANCELLED");
                        return;
                    }
                    if (e.Error != null)
                    {
                        UnityEngine.Debug.LogException(e.Error);
                        return;
                    }
                    result = e.Result;
                };
            }
            return result;
        }

        internal class VersionWrapper
        {
            internal string version = null;
        }

        private static bool CustomRemoteCertificateValidationCallback(object sender,
    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
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
