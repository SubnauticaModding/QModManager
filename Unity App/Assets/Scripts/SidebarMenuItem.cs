using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
[RequireComponent(typeof(StateController))]
[DisallowMultipleComponent]
public class SidebarMenuItem : MonoBehaviour
{
    public GameObject content;
    public bool isDefault;
    StateController controller;
    Toggle toggle;
    public UnityEvent onRefresh;

    public void Start()
    {
        toggle = GetComponent<Toggle>();
        controller = GetComponent<StateController>();

        toggle.onValueChanged.AddListener(OnToggleChanged);
        toggle.isOn = isDefault;

        controller.SetActive(toggle.isOn);
        content?.SetActive(toggle.isOn);
    }

    public void OnDestroy()
	{
		Toggle toggle = GetComponent<Toggle>();
		toggle.onValueChanged.RemoveAllListeners();
	}

	public void OnToggleChanged(bool toggled)
	{
        controller?.SetActive(toggled);
		content?.SetActive(toggled);
        if (content.gameObject.GetComponent<ModList>() is ModList list)
        {
            StartCoroutine(list.FixHeight());
        }
	}
}
