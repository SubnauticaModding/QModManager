using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModListEntry : MonoBehaviour
{
    [SerializeField]
    private GameObject _content;
    [SerializeField]
    private GameObject _expandIcon;
    [SerializeField]
    private GameObject _enabledBadge;
    [SerializeField]
    private GameObject _disabledBadge;
    [SerializeField]
    private GameObject _enableButton;
    [SerializeField]
    private GameObject _disableButton;
    [SerializeField]
    private TextMeshProUGUI _titleLabel;
    [SerializeField]
    private TextMeshProUGUI _descriptionLabel;

    private bool _enabled;
    private bool _expanded;

    private void Awake()
    {
        Refresh();
    }

    public void Initialize(IModInfo modInfo)
    {
        _titleLabel.text = modInfo.DisplayName;
        _descriptionLabel.text = $"{modInfo.Description}\n<color=#CCC>Author:</color> {modInfo.AuthorName}\n<color=#CCC>Version:</color> {modInfo.Version}";
        _enabled = modInfo.Enabled;
        Refresh();
    }

    public void ToggleEntryExpanded()
    {
        _expanded = !_expanded;
        RefreshExpandedState();
    }

    public void ExpandEntry()
    {
        _expanded = true;
        RefreshExpandedState();
    }

    public void CollapseEntry()
    {
        _expanded = false;
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

    private void Refresh()
    {
        RefreshExpandedState();
        RefreshEnabledState();
    }

    private void RefreshExpandedState()
    {
        _content.SetActive(_expanded);
        _expandIcon.SetActive(!_expanded);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
    }

    private void RefreshEnabledState()
    {
        _enabledBadge.SetActive(_enabled);
        _disabledBadge.SetActive(!_enabled);
        _enableButton.SetActive(!_enabled);
        _disableButton.SetActive(_enabled);
    }
}
