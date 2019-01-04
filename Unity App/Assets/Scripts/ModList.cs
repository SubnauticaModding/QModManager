using Oculus.Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ModList : MonoBehaviour
{
    public ModListEntry modEntryTemplate;
    public Transform contentContainer;

    [ReadOnly]
    public List<ModListEntry> modEntries = new List<ModListEntry>();

    public void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        modEntryTemplate.gameObject.SetActive(false);

        DestroyAllEntries();

        var modList = GetModList();
        foreach (var modInfo in modList)
        {
            CreateModEntry(modInfo);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

    public void DestroyAllEntries()
    {
        foreach (var entry in modEntries)
        {
            Destroy(entry.gameObject);
        }
        modEntries.Clear();
    }

    public List<QMod> GetModList()
    {
        string QModsDir = Path.Combine(PlayerPrefs.GetString(DirectorySelector.singleton.FolderPref), "QMods");
        List<QMod> mods = new List<QMod>();
        if (!Directory.Exists(QModsDir))
        {
            Debug.LogWarning("QMods dir wasn't found in path: " + QModsDir);
            return mods;
        }
        string[] directories = Directory.GetDirectories(QModsDir);
        foreach (string dir in directories)
        {
            string modjson = Path.Combine(dir, "mod.json");
            if (!File.Exists(modjson)) continue;
            string text;
            try
            {
                text = File.ReadAllText(modjson);
            }
            catch (Exception e)
            {
                Debug.LogError("An error occured while trying to load mod.json file in directory " + dir);
                Debug.LogException(e);
                continue;
            }

            Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
            QMod mod = new QMod();

            if (dictionary.ContainsKey("DisplayName") && dictionary["DisplayName"] is string name) mod.DisplayName = name;
            else mod.DisplayName = "-";
            if (dictionary.ContainsKey("Id") && dictionary["Id"] is string id) mod.Id = id;
            else mod.Id = "-";
            if (dictionary.ContainsKey("Author") && dictionary["Author"] is string author) mod.Author = author;
            else mod.Author = "-";
            if (dictionary.ContainsKey("Version") && dictionary["Version"] is string version) mod.Version = version;
            else mod.Version = "-";
            if (dictionary.ContainsKey("LoadBefore") && dictionary["LoadBefore"] is string[] loadbefore)
                if (loadbefore.Length <= 0) mod.LoadBefore = "-";
                else mod.LoadBefore = string.Join(", ", loadbefore);
            else mod.LoadBefore = "-";
            if (dictionary.ContainsKey("LoadAfter") && dictionary["LoadAfter"] is string[] loadafter)
                if (loadafter.Length <= 0) mod.LoadAfter = "-";
                else mod.LoadAfter = string.Join(", ", loadafter);
            else mod.LoadAfter = "-";
            if (dictionary.ContainsKey("Dependencies") && dictionary["Dependencies"] is string[] dependencies)
                if (dependencies.Length <= 0) mod.Dependencies = "-";
                else mod.Dependencies = string.Join(", ", dependencies);
            else mod.Dependencies = "-";
            if (dictionary.ContainsKey("Enable") && dictionary["Enable"] is bool enabled) mod.Enabled = enabled;
            else mod.Enabled = false;

            mod.ModJSON = modjson;

            mods.Add(mod);
        }
        mods.Sort();
        return mods;
    }

    public void CreateModEntry(QMod modInfo)
    {
        var newModEntry = Instantiate(modEntryTemplate, contentContainer, false);
        newModEntry.Initialize(modInfo);
        newModEntry.gameObject.SetActive(true);
        modEntries.Add(newModEntry);
    }

    public IEnumerator FixHeight()
    {
        ScrollRect group = GetComponent<ScrollRect>();
        group.vertical = false;
        yield return new WaitForEndOfFrame();
        group.vertical = true;
        yield return null;
    }
}
