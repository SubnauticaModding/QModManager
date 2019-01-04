using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public Color fieldColor;

    [ReadOnly]
    public bool _enabled;
    [ReadOnly]
    public bool expanded;

    public void Awake()
    {
        Refresh();
    }

    public void Initialize(IModInfo modInfo)
    {
        string color = ColorUtility.ToHtmlStringRGBA(fieldColor);
        titleLabel.text = modInfo.DisplayName;
        descriptionLabel.text = $"<color=#{color}>Author:</color> {modInfo.Author}\n<color=#{color}>Version:</color> {modInfo.Version}";
        enabled = modInfo.Enabled;
        Refresh();
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
        enabled = true;
        RefreshEnabledState();
    }

    public void DisableMod()
    {
        enabled = false;
        RefreshEnabledState();
    }

    public void Refresh()
    {
        RefreshExpandedState();
        RefreshEnabledState();
    }

    public void RefreshExpandedState()
    {
        content.SetActive(expanded);
        expandIcon.SetActive(!expanded);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
    }

    public void RefreshEnabledState()
    {
        enabledBadge.SetActive(enabled);
        disabledBadge.SetActive(!enabled);
        enableButton.SetActive(!enabled);
        disableButton.SetActive(enabled);
    }
}
