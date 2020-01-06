using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ContentHolder))]
public class ContentHolderInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var contentHolder = target as ContentHolder;

        base.OnInspectorGUI();

        if (contentHolder.gameObject.activeSelf) EditorGUI.BeginDisabledGroup(true);

        if (GUILayout.Button("Enable this", new GUIStyle(GUI.skin.button) { fontSize = 30 }))
        {
            foreach (ContentHolder ch in GameObject.FindObjectsOfType<ContentHolder>())
            {
                ch.gameObject.SetActive(false);
            }
            contentHolder.gameObject.SetActive(true);
        }

        if (contentHolder.gameObject.activeSelf) EditorGUI.EndDisabledGroup();
    }
}
