using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    public string Version;

    public void Awake()
    {
        if (singleton != null)
        {
            Debug.LogError("GameManager should be singleton but it isn't!");
            Destroy(this);
        }
        singleton = this;
    }
}
