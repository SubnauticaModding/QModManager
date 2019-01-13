using System;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    public GameObject dialog;

    public string Version;
    [ReadOnly] public string VersionURL = "https://raw.githubusercontent.com/QModManager/QModManager/unity-app/Unity%20App/Assets/Data/Editor/latestversion.txt";
    [ReadOnly] public string LatestVersion;

    public void Awake()
    {
        Screen.SetResolution(1024, 768, false);
        if (singleton != null)
        {
            Debug.LogError("GameManager should be singleton but isn't!");
            Destroy(this);
        }
        singleton = this;
        CheckVersion();
    }

    [ReadOnly] public int lastWidth = 0;
    [ReadOnly] public int lastHeight = 0;

    [ReadOnly] public float updateTimer = 0;

    public void Update()
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

    public void OpenLink(string link)
    {
        Process.Start(link);
    }

    public void ShowDialogIfNotLatest()
    {
        if (Version == LatestVersion) return;
        dialog.SetActive(true);
        dialog.GetComponentInChildren<TextMeshProUGUI>().text = "New version available: v" + LatestVersion;
    }
    public void CheckVersion()
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
                    Debug.LogError("Version check cancelled...?");
                    return;
                }
                if (e.Error != null)
                {
                    Debug.LogError("Could not get latest version. Probably the internet is turned off.");
                    Debug.LogException(e.Error);
                    return;
                }
                result = e.Result;
                Debug.Log(result);
                if (!ValidateVersion(result)) LatestVersion = Version;
                else LatestVersion = result;
                ShowDialogIfNotLatest();
            };
        }
    }
    public bool ValidateVersion(string versionStr)
    {
        if (versionStr == null)
        {
            Debug.LogError("Could not parse latest version: string null");
            return false;
        }
        return true;
    }
    private static bool CustomRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
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
