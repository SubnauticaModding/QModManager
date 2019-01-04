using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Crosstales.FB;
using Harmony;
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
            UnlockPages();
        }
        else
        {
            label.text = string.IsNullOrEmpty(currentSelectedInstallFolder) ? "currentSelectedInstallFolder" : currentSelectedInstallFolder;
            icon.overrideSprite = noFolderIcon;
            LockPages();
        }

        if (!CheckQModsPatched(currentSelectedInstallFolder)) LockPages();

        RefreshStatusLabel(currentSelectedInstallFolder);
    }

    public void RefreshStatusLabel(string installFolder)
    {
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

    public static void UnlockPages()
    {
        List<Toggle> toggles = FindObjectsOfType<SidebarMenuItem>().Select(m => m.GetComponent<Toggle>()).ToList();
        toggles.Do(t =>
        {
            t.interactable = true;
            t.GetComponentsInChildren<Image>(true)[3].color = new Color(1, 1, 1, 0.4980392f);
            t.GetComponentsInChildren<TextMeshProUGUI>(true)[1].color = new Color(1, 1, 1, 0.4980392f);
        });
    }
    public static void LockPages()
    {
        Toggle[] toggles = FindObjectsOfType<Toggle>();
        toggles.Do(t =>
        {
            t.interactable = false;
            t.GetComponentsInChildren<Image>(true)[3].color = new Color(1, 1, 1, 0.1176471f);
            t.GetComponentsInChildren<TextMeshProUGUI>(true)[1].color = new Color(1, 1, 1, 0.1176471f);
        });
        toggles[0].interactable = true;
        toggles[0].isOn = true;
    }
}
