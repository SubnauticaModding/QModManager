public interface IModInfo
{
    string DisplayName { get; }
    string Description { get; }
    string AuthorName { get; }
    string Version { get; }
    bool Enabled { get; }
}

public class TestModInfo : IModInfo
{
    public string DisplayName => "TestDisplayName";
    public string Description => "TestDescription Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed sit amet fringilla magna.";
    public string AuthorName => "TestAuthorName";
    public string Version => "Test 1.0.0";
    public bool Enabled => true;
}