using Oculus.Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace QModManager
{
    internal static class VersionCheck
    {
        internal const string VersionURL = "https://raw.githubusercontent.com/QModManager/QModManager/version-check/version.json";

        internal static void Check()
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
                        Console.WriteLine(LanguageLines.VersionCheck.Cancelled);
                        return;
                    }
                    if (e.Error != null)
                    {
                        UnityEngine.Debug.LogException(e.Error);
                        return;
                    }
                    result = e.Result;
                    UnityEngine.Debug.Log(result);
                    Parse(result);
                };
            }
        }

        internal static void Parse(string versionStr)
        {
            if (versionStr == null)
            {
                Console.WriteLine(LanguageLines.VersionCheck.ErrorStringNull);
                return;
            }
            VersionWrapper wrapper = JsonConvert.DeserializeObject<VersionWrapper>(versionStr);
            if (wrapper == null)
            {
                Console.WriteLine(LanguageLines.VersionCheck.ErrorWrapperNull);
                return;
            }
            if (wrapper.version == null)
            {
                Console.WriteLine(LanguageLines.VersionCheck.ErrorWrapperVersionNull);
                return;
            }
            Version version = new Version(wrapper.version);
            if (version == null)
            {
                Console.WriteLine(LanguageLines.VersionCheck.ErrorVersionNull);
                return;
            }
            if (!version.Equals(QMod.QModManagerVersion) && QModPatcher.erroredMods.Count <= 0)
            {
                QModPatcher.dialogversion = version;
                QModPatcher.ShowDialog();
            }
        }

        internal class VersionWrapper
        {
            [JsonRequired]
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
