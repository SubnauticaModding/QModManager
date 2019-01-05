using TMPro;
using UnityEditor;
using UnityEngine;

public class SidebarFooter : MonoBehaviour
{
    public string Prefix;

    public void Awake()
    {
        GetComponent<TextMeshProUGUI>().text = Prefix + GameManager.singleton.Version;
    }
}
