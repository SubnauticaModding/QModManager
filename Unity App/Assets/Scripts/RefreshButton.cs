using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class RefreshButton : MonoBehaviour
{
    //public SidebarMenuItem[] worksIn;

    //[ReadOnly]
    //public SidebarMenuItem currentItem;

    //public void Awake()
    //{
    //    gameObject.SetActive(false);
    //}

    //public void Update()
    //{
    //    worksIn.Do(s =>
    //    {
    //        if (s.content.gameObject.activeInHierarchy)
    //        {
    //            gameObject.SetActive(true);
    //            currentItem = s;
    //        }
    //    });
    //}

    public void Awake()
    {
        gameObject.SetActive(false);
    }

    public void OnClick()
    {
        GetComponent<Animator>().Play("Refresh");
        //currentItem.onRefresh?.Invoke();
        StartCoroutine(RefreshModList());
    }

    //public void OnModListClick()
    //{
    //    StartCoroutine(RefreshModList());
    //}

    public IEnumerator RefreshModList()
    {
        yield return new WaitForSeconds(0.5f);
        FindObjectOfType<ModList>().OnDisable();
        FindObjectOfType<ModList>().OnEnable();
        yield return StartCoroutine(FindObjectOfType<ModList>().FixHeight());
    }
}
