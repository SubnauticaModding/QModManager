using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SidebarMenuItem : MonoBehaviour
{
    public GameObject active;
    public GameObject inactive;
    public GameObject content;

	public void Awake()
	{
		var toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(OnToggleChanged);

		active?.SetActive(toggle.isOn);
		inactive?.SetActive(!toggle.isOn);
	    content?.SetActive(toggle.isOn);
	}

    public void OnDestroy()
	{
		var toggle = GetComponent<Toggle>();
		toggle.onValueChanged.RemoveAllListeners();
	}

	public void OnToggleChanged(bool toggled)
	{
		active?.SetActive(toggled);
		inactive?.SetActive(!toggled);
		content?.SetActive(toggled);
        if (content.gameObject.GetComponent<ModList>() is ModList list)
        {
            StartCoroutine(list.FixHeight());
        }
	}
}
