// CODE FROM https://github.com/SubnauticaNitrox/Nitrox/

using System.Text;

namespace QModManager.SceneDebugger
{
    internal static class StringUtils
    {
        internal static string TruncateRight(this string value, int maxChars, string appendix = "...")
        {
            Validate.NotNull(value);
            Validate.NotNull(appendix);

            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + appendix;
        }

        internal static string TruncateLeft(this string value, int maxChars, string appendix = "...")
        {
            Validate.NotNull(value);
            Validate.NotNull(appendix);

            return value.Length <= maxChars ? value : appendix + value.Substring(value.Length - maxChars, maxChars);
        }

        internal static string ByteArrayToHexString(this byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);

            foreach (byte b in bytes)
            {
                hex.Append("0x");
                hex.Append(b.ToString("X2"));
                hex.Append(" ");
            }

            return hex.ToString();
        }
    }
}