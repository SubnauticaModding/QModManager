using Crosstales.FB;
using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class DirectorySelector : MonoBehaviour
{
    public static DirectorySelector singleton;

    string FolderPref = "SubnauticaInstallDirectory";

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
            label.text = string.IsNullOrEmpty(currentSelectedInstallFolder) ? "Select Subnautica Install Directory..." : currentSelectedInstallFolder;
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
        string managedFolder = Path.Combine(installFolder, "Subnautica_Data", "Managed");
        string assemblyFile = Path.Combine(managedFolder, "Assembly-CSharp.dll");
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
        string currentSelectedInstallFolder = GetInstallFolderPref();
        string newInstallFolder = FileBrowser.OpenSingleFolder("Subnautica Install Directory", currentSelectedInstallFolder);

        if (!string.IsNullOrEmpty(newInstallFolder.Trim()))
        {
            PlayerPrefs.SetString(FolderPref, newInstallFolder);
            PlayerPrefs.Save();
        }

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

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Set Default Path")]
    public static void SetDefaultPath()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Cannot do this while not in playmode!");
            return;
        }

        PlayerPrefs.SetString(singleton.FolderPref, "D:/Program Files (x86)/Steam/steamapps/common/Subnautica");
        PlayerPrefs.Save();

        singleton.Refresh();
    }
#endif
}
