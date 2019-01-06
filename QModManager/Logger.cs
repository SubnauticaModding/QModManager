using System.Reflection;

namespace QModManager
{
    public static class Logger
    {
        public static void ShowSubtitle(string text, float delay = 1f, float duration = 5f)
        {
            typeof(Subtitles).GetMethod("AddRawLong", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(Subtitles.main, new object[] { text, delay, duration });
        }
    }
}

