using GitHub.Unity;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

public class GitHubController : EditorWindow
{
    public static GitClient client;
    public static GitHubController singleton;

    [MenuItem("Tools/GitHub")]
    public static void ShowWindow()
    {
        GetWindow<GitHubController>();
    }

    public void OnEnable()
    {
        Init();
    }

    public void OnGUI()
    {
        GUILayout.Label("GitHub Utility Window", EditorStyles.boldLabel);
        if (GUILayout.Button("Update version"))
        {
            client.AddAndCommit(new List<string>() { "./Unity App/Assets/Data/version.txt", "./Unity App/Assets/Data/version.txt.meta" }, "UPDATE VERSION - " + DateTime.Now.ToString(CultureInfo.InvariantCulture), null).Then(() => Debug.Log("Commited version.txt and version.txt.meta files!")).Catch(e => Debug.LogException(e)).Start();
        }
    }

    public void Init()
    {
        if (client != null)
        {
            Debug.LogWarning("Git client was already initialized!");
            return;
        }
        DefaultEnvironment defaultEnvironment = new DefaultEnvironment();
        defaultEnvironment.Initialize(null, NPath.Default, NPath.Default, NPath.Default, Application.dataPath.ToNPath());
        ProcessEnvironment processEnvironment = new ProcessEnvironment(defaultEnvironment);
        ProcessManager processManager = new ProcessManager(defaultEnvironment, processEnvironment, TaskManager.Instance.Token);
        GitClient gitClient = new GitClient(defaultEnvironment, processManager, TaskManager.Instance.Token);
        client = gitClient;
        singleton = this;
    }
}
