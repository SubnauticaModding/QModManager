using UnityEngine;

public class Admin : MonoBehaviour
{
    public static Admin singleton;

    public void Awake()
    {
        if (singleton != null)
        {
            Debug.LogError("Admin is supposed to be a singleton but isn't!");
            Destroy(gameObject);
        }
        singleton = this;
    }

    public void Start()
    {
        if (!Application.isEditor)
        {
            Destroy();
            return;
        }
    }

    public void Destroy()
    {
        Destroy(GetComponent<SidebarMenuItem>().content);
        Destroy(gameObject);
    }
}