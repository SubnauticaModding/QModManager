using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class SidebarFooter : MonoBehaviour
{
    public string Prefix;

    public void Awake()
    {
        GetComponent<TextMeshProUGUI>().text = Prefix + GameManager.singleton.Version;
    }
}
