using System.IO;
using Crosstales.FB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DirectorySelector : MonoBehaviour
{
    private const string FolderPref = "SubnauticaInstallDirectory";

    [SerializeField]
    private TextMeshProUGUI _label;
    [SerializeField]
    private TextMeshProUGUI _statusLabel;
    [SerializeField]
    private Image _icon;
    [SerializeField]
    private Sprite _noFolderIcon;
    [SerializeField]
    private Sprite _hasFolderIcon;

    private void Awake()
    {
        Refresh();
    }

    private string GetInstallFolderPref()
    {
        return PlayerPrefs.GetString(FolderPref, string.Empty);
    }

    private void Refresh()
    {
        var currentSelectedInstallFolder = GetInstallFolderPref();
        if (InstallFolderIsValid(currentSelectedInstallFolder))
        {
            _label.text = currentSelectedInstallFolder;
            _icon.overrideSprite = _hasFolderIcon;
        }
        else
        {
            _label.text = "Select Subnautica Install Directory...";
            _icon.overrideSprite = _noFolderIcon;
        }

        RefreshStatusLabel(currentSelectedInstallFolder);
    }

    private void RefreshStatusLabel(string installFolder)
    {
        if (string.IsNullOrEmpty(installFolder))
        {
            _statusLabel.text = "";
            return;
        }

        var isSubnauticaInstalled = CheckSubnauticaInstalled(installFolder);
        var isQModsPatched = CheckQModsPatched(installFolder);

        const string green = "#54C069";
        const string red = "#EA4F46";

        var snInstalledColor = isSubnauticaInstalled ? green : red;
        var snInstalledText = isSubnauticaInstalled ? "INSTALLED" : "NOT INSTALLED";

        _statusLabel.text = $"Subnautica <color={snInstalledColor}>{snInstalledText}</color> ";

        var qmodsPatchedColor = isQModsPatched ? green : red;
        var qmodsPatchedText = isQModsPatched ? "PATCHED" : "NOT PATCHED";
        _statusLabel.text += $" - QMods <color={qmodsPatchedColor}>{qmodsPatchedText} (TODO)</color> ";
    }

    private static bool CheckSubnauticaInstalled(string installFolder)
    {
        var managedFolder = Path.Combine(installFolder, "Subnautica_Data", "Managed");
        var assemblyFile = Path.Combine(managedFolder, "Assembly-CSharp.dll");
        return Directory.Exists(managedFolder) && File.Exists(assemblyFile);
    }

    private static bool CheckQModsPatched(string installFolder)
    {
        // TODO
        return false;
    }

    private static bool InstallFolderIsValid(string currentSelectedInstallFolder)
    {
        return !string.IsNullOrEmpty(currentSelectedInstallFolder);
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
