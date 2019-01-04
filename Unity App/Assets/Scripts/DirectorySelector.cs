using System;
using System.IO;
using Crosstales.FB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DirectorySelector : MonoBehaviour
{
    public static DirectorySelector singleton;

    [ReadOnly]
    public string FolderPref = "SubnauticaInstallDirectory";

    public TextMeshProUGUI label;
    public TextMeshProUGUI statusLabel;
    public Image icon;
    public Sprite noFolderIcon;
    public Sprite hasFolderIcon;
    public Color green;
    public Color red;

    public void Awake()
    {
        if (singleton != null)
        {
            Debug.LogError("DirectorySelector should be singleton but it isn't!");
            Destroy(this);
        }
        singleton = this;
        Refresh();
    }

    public string GetInstallFolderPref()
    {
        return PlayerPrefs.GetString(FolderPref, string.Empty);
    }

    public void Refresh()
    {
        string currentSelectedInstallFolder = GetInstallFolderPref();
        if (InstallFolderIsValid(currentSelectedInstallFolder))
        {
            label.text = currentSelectedInstallFolder;
            icon.overrideSprite = hasFolderIcon;
        }
        else
        {
            label.text = "Select Subnautica Install Directory...";
            icon.overrideSprite = noFolderIcon;
        }

        RefreshStatusLabel(currentSelectedInstallFolder);
    }

    public void RefreshStatusLabel(string installFolder)
    {
        if (string.IsNullOrEmpty(installFolder))
        {
            statusLabel.text = "";
            return;
        }

        bool isSubnauticaInstalled = CheckSubnauticaInstalled(installFolder);
        bool isQModsPatched = false;
        bool failed = false;
        try
        {
            isQModsPatched = CheckQModsPatched(installFolder);
        }
        catch
        {
            failed = true;
        }

        string snInstalledColor = ColorUtility.ToHtmlStringRGBA(isSubnauticaInstalled ? green : red);
        string snInstalledText = isSubnauticaInstalled ? "INSTALLED" : "NOT INSTALLED";

        statusLabel.text = $"Subnautica <color=#{snInstalledColor}>{snInstalledText}</color> ";

        string qmodsPatchedColor = ColorUtility.ToHtmlStringRGBA(isQModsPatched && !failed ? green : red);
        string qmodsPatchedText = failed ? "ERROR" : isQModsPatched ? "PATCHED" : "NOT PATCHED";

        statusLabel.text += $" - QModManager <color=#{qmodsPatchedColor}>{qmodsPatchedText}</color> ";
    }

    public static bool CheckSubnauticaInstalled(string installFolder)
    {
        var managedFolder = Path.Combine(installFolder, "Subnautica_Data", "Managed");
        var assemblyFile = Path.Combine(managedFolder, "Assembly-CSharp.dll");
        return Directory.Exists(managedFolder) && File.Exists(assemblyFile);
    }

    public static bool CheckQModsPatched(string installFolder)
    {
        if (!CheckSubnauticaInstalled(installFolder)) return false;
        try
        {
            return Injector.IsInjected(Path.Combine(installFolder, "Subnautica_Data/Managed/Assembly-CSharp.dll"));
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public static bool InstallFolderIsValid(string currentSelectedInstallFolder)
    {
        if (string.IsNullOrEmpty(currentSelectedInstallFolder)) return false;
        if (!CheckSubnauticaInstalled(currentSelectedInstallFolder)) return false;
        return true;
    }

    public void SelectInstallFolder()
    {
        var currentSelectedInstallFolder = GetInstallFolderPref();
        var newInstallFolder = FileBrowser.OpenSingleFolder("Subnautica Install Directory", currentSelectedInstallFolder);
        PlayerPrefs.SetString(FolderPref, newInstallFolder);
        PlayerPrefs.Save();

        Refresh();
    }
}
