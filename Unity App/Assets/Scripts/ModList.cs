using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModList : MonoBehaviour
{
    [SerializeField]
    private ModListEntry _modEntryTemplate;
    [SerializeField]
    private Transform _contentContainer;

    private readonly List<ModListEntry> _modEntries = new List<ModListEntry>();

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        _modEntryTemplate.gameObject.SetActive(false);

        DestroyAllEntries();

        var modList = GetModList();
        foreach (var modInfo in modList)
        {
            CreateModEntry(modInfo);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

    private void DestroyAllEntries()
    {
        foreach (var entry in _modEntries)
        {
            DestroyImmediate(entry.gameObject);
        }
        _modEntries.Clear();
    }

    private IList<IModInfo> GetModList()
    {
        return new List<IModInfo>
        {
            new TestModInfo(),
            new TestModInfo(),
            new TestModInfo(),
            new TestModInfo(),
            new TestModInfo(),
            new TestModInfo(),
            new TestModInfo(),
        };
    }

    private void CreateModEntry(IModInfo modInfo)
    {
        var newModEntry = Instantiate(_modEntryTemplate, _contentContainer, false);
        newModEntry.Initialize(modInfo);
        newModEntry.gameObject.SetActive(true);
        _modEntries.Add(newModEntry);
    }
}
