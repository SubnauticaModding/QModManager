using System.Collections.Generic;
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

    public IList<IModInfo> GetModList()
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

    public void CreateModEntry(IModInfo modInfo)
    {
        var newModEntry = Instantiate(modEntryTemplate, contentContainer, false);
        newModEntry.Initialize(modInfo);
        newModEntry.gameObject.SetActive(true);
        modEntries.Add(newModEntry);
    }

    public void FixHeight()
    {
        ContentSizeFitter fitter = GetComponentInChildren<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
    }
}
