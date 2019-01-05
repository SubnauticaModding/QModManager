using UnityEngine;

public class EnablerDisablerController : MonoBehaviour
{
    public GameObject active;
    public GameObject inactive;

    public void SetActive(bool enable)
    {
        active.SetActive(enable);
        inactive.SetActive(!enable);
    }
    public void ChangeActive()
    {
        active.SetActive(!active.activeSelf);
        inactive.SetActive(!inactive.activeSelf);
    }

    public void Activate()
    {
        active.SetActive(true);
        inactive.SetActive(false);
    }
    public void Deactivate()
    {
        active.SetActive(false);
        inactive.SetActive(true);
    }
}