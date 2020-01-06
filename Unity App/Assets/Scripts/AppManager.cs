using System;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

[DisallowMultipleComponent]
public class AppManager : MonoBehaviour
{
    const string VersionURL = "http://raw.githubusercontent.com/SubnauticaModding/QModManager/master/Data/latest-version.txt";

    public static AppManager singleton;

    public GameObject dialog;

    public string Version;
    string LatestVersion;

    void Awake()
    {
        Screen.SetResolution(1024, 768, false);
        if (singleton != null)
        {
            Debug.LogError(nameof(AppManager) + " should be singleton but isn't!");
            Destroy(this);
        }
        singleton = this;
        CheckVersion();
    }

    int lastWidth = 0;
    int lastHeight = 0;

    float updateTimer = 0;

    void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer < 0.1f) return;
        updateTimer = 0;

        int width = Screen.width;
        int height = Screen.height;

        if (lastWidth != width)
        {
            var heightAccordingToWidth = width / 4f * 3f;
            Screen.SetResolution(width, Mathf.RoundToInt(heightAccordingToWidth), false, 0);
        }
        else if (lastHeight != height)
        {
            var widthAccordingToHeight = height / 3f * 4f;
            Screen.SetResolution(Mathf.RoundToInt(widthAccordingToHeight), height, false, 0);
        }

        lastWidth = width;
        lastHeight = height;
    }

    void OpenLink(string link)
    {
        Process.Start(link);
    }

    void ShowDialogIfNotLatest()
    {
        if (Version == LatestVersion) return;
        dialog.SetActive(true);
        dialog.GetComponentInChildren<TextMeshProUGUI>().text = "New version available: v" + LatestVersion;
    }
    void CheckVersion()
    {
        string result = null;
        ServicePointManager.ServerCertificateValidationCallback = CustomRemoteCertificateValidationCallback;

        using (WebClient client = new WebClient())
        {
            //client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:25.0) Gecko/20100101 Firefox/25.0");
            //client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            Debug.Log("Trying to get latest version from url: " + VersionURL);

            client.DownloadStringCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    Debug.LogError("Could not get latest version. Probably there is no connection.");
                    Debug.LogException(e.Error);
                }
                else
                {
                    result = e.Result;
                    Debug.Log(result);
                    if (!ValidateVersion(result)) LatestVersion = Version;
                    else LatestVersion = result;
                    ShowDialogIfNotLatest();
                }
            };
            client.DownloadStringAsync(new Uri(VersionURL));
        }
    }
    bool ValidateVersion(string versionStr)
    {
        if (versionStr == null)
        {
            Debug.LogError("Could not parse latest version: string null");
            return false;
        }
        return true;
    }

    static bool CustomRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown) continue;
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                if (!chain.Build((X509Certificate2)certificate)) return false;
            }
        }
        return true;
    }
}
