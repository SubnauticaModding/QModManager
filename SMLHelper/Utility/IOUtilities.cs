namespace SMLHelper.V2.Utility
{
    using System.IO;

    /// <summary>
    /// Utilities for files and paths
    /// </summary>
    public static class IOUtilities
    {
        /// <summary>
        /// Works like <see cref="Path.Combine(string, string)"/>, but can have more than 2 paths
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="rest"></param>
        /// <returns></returns>
        public static string Combine(string one, string two, params string[] rest)
        {
            string path = Path.Combine(one, two);

            foreach (string str in rest)
            {
                path = Path.Combine(path, str);
            }

            return path;
        }
    }
}
