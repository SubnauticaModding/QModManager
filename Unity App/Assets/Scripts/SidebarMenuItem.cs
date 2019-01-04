using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SidebarMenuItem : MonoBehaviour
{
	[SerializeField]
	private GameObject _active;
	[SerializeField]
	private GameObject _inactive;
	[SerializeField]
	private GameObject _content;

	private void Awake()
	{
		var toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(OnToggleChanged);

		if (_active != null)
		{
			_active.SetActive(toggle.isOn);
		}

		if (_inactive != null)
		{
			_inactive.SetActive(!toggle.isOn);
		}

		if (_content != null)
		{
			_content.SetActive(toggle.isOn);
		}
	}

	private void OnDestroy()
	{
		var toggle = GetComponent<Toggle>();
		toggle.onValueChanged.RemoveAllListeners();
	}

	public void OnToggleChanged(bool toggled)
	{
		if (_active != null)
		{
			_active.SetActive(toggled);
		}

		if (_inactive != null)
		{
			_inactive.SetActive(!toggled);
		}

		if (_content != null)
		{
			_content.SetActive(toggled);
		}
	}
}
