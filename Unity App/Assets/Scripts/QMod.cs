using System;

[Serializable]
public class QMod
{
    public QMod() { }

    public string DisplayName { get; set; }
    public string Id { get; set; }
    public string Author { get; set; }
    public string Version { get; set; }
    public string Dependencies { get; set; }
    public string LoadBefore { get; set; }
    public string LoadAfter { get; set; }
    public bool Enabled { get; set; }
    public string ModJSON { get; set; }
}