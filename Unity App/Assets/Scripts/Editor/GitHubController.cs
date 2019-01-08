using GitHub.Unity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GitHubController : EditorWindow
{
    public static GitClient client;
    public static GitHubController singleton;

    public string Version;

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
        try
        {
            Version = EditorGUILayout.TextField("Version", Version) ?? File.ReadAllText(Path.Combine(Application.dataPath, "Data/version.txt"));
        }
        catch (Exception e)
        {
            Debug.LogError("An error occured while trying to load the current version for the github utilities window");
            Debug.LogException(e);
        }
        if (GUILayout.Button("Save version"))
        {
            try
            {
                File.WriteAllText(Path.Combine(Application.dataPath, "Data/version.txt"), Version);
                client.AddAndCommit(new List<string>() { "./Assets/Data/version.txt", "./Assets/Data/version.txt.meta" }, "UPDATE VERSION - " + DateTime.Now.ToString(CultureInfo.InvariantCulture), null)
                    .Then(() => client.Push("git@github.com:QModManager/QModManager.git", "unity-app")
                    .Then(() => Debug.Log("Commited version.txt and files!"))
                    .Catch(e => Debug.LogException(e))
                    .Start())
                    .Catch(e => Debug.LogException(e))
                    .Start();
            }
            catch (Exception e)
            {
                Debug.LogError("An error occured while trying to save the latest version for the github utilities window");
                Debug.LogException(e);
            }
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
