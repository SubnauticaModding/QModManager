using System;

namespace SMLHelper.V2.Json.Attributes
{
    /// <summary>
    /// Attribute used to specify a file name for use with a <see cref="JsonFile"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FileNameAttribute : Attribute
    {
        /// <summary>
        /// The filename.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Used to specify the file name for a <see cref="JsonFile"/>.
        /// </summary>
        /// <param name="fileName"></param>
        public FileNameAttribute(string fileName)
        {
            FileName = fileName;
        }
    }
}
