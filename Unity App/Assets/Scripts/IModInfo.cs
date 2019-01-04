public interface IModInfo
{
    string DisplayName { get; }
    string Id { get; }
    string Author { get; }
    string Version { get; }
    string[] Dependencies { get; }
    string[] LoadBefore { get; }
    string[] LoadAfter { get; }
    bool Enabled { get; }
}

public class TestModInfo : IModInfo
{
    public string DisplayName => "Test Mod";
    public string Id => "TestMod";
    public string Author => "Author";
    public string Version => "1.0.0";
    public string[] Dependencies => new string[] { "SMLHelper" };
    public string[] LoadBefore => new string[] { "OneMod" };
    public string[] LoadAfter => new string[] { "AnotherMod" };
    public bool Enabled => true;
}