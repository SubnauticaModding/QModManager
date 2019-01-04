using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ModListEntry : MonoBehaviour
{
    public GameObject content;
    public GameObject expandIcon;
    public GameObject enabledBadge;
    public GameObject disabledBadge;
    public GameObject enableButton;
    public GameObject disableButton;
    public TextMeshProUGUI titleLabel;
    public TextMeshProUGUI descriptionLabel;
    public Color enabledColor;
    public Color disabledColor;

    [Header("Mod details")]

    [ReadOnly]
    public bool _enabled;
    [ReadOnly]
    public bool expanded;
    [ReadOnly]
    public string modJSON;

    public void Awake()
    {
        Refresh();
    }

    public void Initialize(QMod modInfo)
    {
        titleLabel.text = modInfo.DisplayName;
        descriptionLabel.Empty()
            .Populate("ID", modInfo.Id)
            .Populate("Author", modInfo.Author)
            .Populate("Version", modInfo.Version)
            .Populate("Loads Before", modInfo.LoadBefore)
            .Populate("Loads After", modInfo.Id == "SMLHelper" ? "All" : modInfo.LoadAfter)
            .Populate("Dependencies", modInfo.Dependencies);
        _enabled = modInfo.Enabled;
        modJSON = modInfo.ModJSON;
        Refresh();
    }

    public bool UpdateModJSON()
    {
        try
        {
            if (!File.Exists(modJSON)) return false;
            string text;
            text = File.ReadAllText(modJSON);
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
            if (dict == null) return false;
            if (!dict.ContainsKey("Enable")) return false;
            bool enable;
            enable = (bool)dict["Enable"];
            enable = !enable;
            dict["Enable"] = enable;
            text = JsonConvert.SerializeObject(dict, new JsonSerializerSettings { Formatting = Formatting.Indented });
            File.WriteAllText(modJSON, text);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }
    public void ToggleEntryExpanded()
    {
        expanded = !expanded;
        RefreshExpandedState();
    }
    public void ExpandEntry()
    {
        expanded = true;
        RefreshExpandedState();
    }
    public void CollapseEntry()
    {
        expanded = false;
        RefreshExpandedState();
    }
    public void EnableMod()
    {
        _enabled = true;
        RefreshEnabledState();
    }
    public void DisableMod()
    {
        _enabled = false;
        RefreshEnabledState();
    }
    public void Refresh()
    {
        RefreshExpandedState();
        RefreshEnabledState();
    }
    public void RefreshExpandedState()
    {
        if (gameObject.activeSelf)
        {
            Animator anim = GetComponent<Animator>();
            anim.ResetTrigger(expanded ? "collapse" : "expand");
            anim.SetTrigger(expanded ? "expand" : "collapse");
        }
    }
    public void RefreshEnabledState()
    {
        if (!UpdateModJSON())
        {
            Debug.LogError("Could not modify mod.json file for mod: " + titleLabel.text + ", path: " + modJSON);
            return;
        }
        enabledBadge.SetActive(_enabled);
        disabledBadge.SetActive(!_enabled);
        enableButton.SetActive(!_enabled);
        disableButton.SetActive(_enabled);
        titleLabel.color = _enabled ? enabledColor : disabledColor;
    }
}

public static class ModListEntryExtensions
{
    public static TextMeshProUGUI Populate(this TextMeshProUGUI tmpro, string key, string value)
    {
        if (tmpro.text != "") tmpro.text += "\n";
        tmpro.text += "<color=#CCCCCC>" + key + ":</color> " + value;
        return tmpro;
    }
    public static TextMeshProUGUI Empty(this TextMeshProUGUI tmpro)
    {
        tmpro.text = "";
        return tmpro;
    }
    public static Color ParseColor(this string s)
    {
        if (ColorUtility.TryParseHtmlString(s, out Color color)) return color;
        return Color.white;
    }
}
