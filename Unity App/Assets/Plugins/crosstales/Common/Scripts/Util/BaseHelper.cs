using UnityEngine;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Crosstales.Common.Util
{
    /// <summary>Base for various helper functions.</summary>
    public abstract class BaseHelper
    {
        #region Variables
        
        public static readonly System.Globalization.CultureInfo BaseCulture = new System.Globalization.CultureInfo("en-US");
        
        protected static readonly System.Text.RegularExpressions.Regex lineEndingsRegex = new System.Text.RegularExpressions.Regex(@"\r\n|\r|\n");
        //protected static readonly Regex cleanStringRegex = new Regex(@"([^a-zA-Z0-9 ]|[ ]{2,})");
        protected static readonly System.Text.RegularExpressions.Regex cleanSpacesRegex = new System.Text.RegularExpressions.Regex(@"\s+");
        protected static readonly System.Text.RegularExpressions.Regex cleanTagsRegex = new System.Text.RegularExpressions.Regex(@"<.*?>");
        //protected static readonly System.Text.RegularExpressions.Regex asciiOnlyRegex = new System.Text.RegularExpressions.Regex(@"[^\u0000-\u00FF]+");

        protected static readonly System.Random rnd = new System.Random();

        protected const string file_prefix = "file://";

        #endregion


        #region Properties

        /// <summary>Checks if an Internet connection is available.</summary>
        /// <returns>True if an Internet connection is available.</returns>
        public static bool isInternetAvailable
        {
            get
            {
#if CT_OC
                return OnlineCheck.OnlineCheck.isInternetAvailable;
#else
                return Application.internetReachability != NetworkReachability.NotReachable;
#endif
            }
        }

        /// <summary>Checks if the current platform is Windows.</summary>
        /// <returns>True if the current platform is Windows.</returns>
        public static bool isWindowsPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
            }
        }

        /// <summary>Checks if the current platform is OSX.</summary>
        /// <returns>True if the current platform is OSX.</returns>
        public static bool isMacOSPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor;
            }
        }

        /// <summary>Checks if the current platform is Linux.</summary>
        /// <returns>True if the current platform is Linux.</returns>
        public static bool isLinuxPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor;
            }
        }

        /// <summary>Checks if the current platform is standalone (Windows, macOS or Linux).</summary>
        /// <returns>True if the current platform is standalone (Windows, macOS or Linux).</returns>
        public static bool isStandalonePlatform
        {
            get
            {
                return isWindowsPlatform || isMacOSPlatform || isLinuxPlatform;
            }
        }

        /// <summary>Checks if the current platform is Android.</summary>
        /// <returns>True if the current platform is Android.</returns>
        public static bool isAndroidPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.Android;
            }
        }

        /// <summary>Checks if the current platform is iOS.</summary>
        /// <returns>True if the current platform is iOS.</returns>
        public static bool isIOSPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS;
            }
        }

        /// <summary>Checks if the current platform is WSA.</summary>
        /// <returns>True if the current platform is WSA.</returns>
        public static bool isWSAPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.WSAPlayerARM ||
                    Application.platform == RuntimePlatform.WSAPlayerX86 ||
                    Application.platform == RuntimePlatform.WSAPlayerX64 ||
                    Application.platform == RuntimePlatform.XboxOne;
            }
        }

        /// <summary>Checks if the current platform is WebGL.</summary>
        /// <returns>True if the current platform is WebGL.</returns>
        public static bool isWebGLPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.WebGLPlayer;
            }
        }

        /// <summary>Checks if the current platform is Web (WebPlayer or WebGL).</summary>
        /// <returns>True if the current platform is Web (WebPlayer or WebGL).</returns>
        public static bool isWebPlatform
        {
            get
            {
                return isWebGLPlatform;
            }
        }

        /// <summary>Checks if the current platform is Windows-based (Windows standalone or WSA).</summary>
        /// <returns>True if the current platform is Windows-based (Windows standalone or WSA).</returns>
        public static bool isWindowsBasedPlatform
        {
            get
            {
                return isWindowsPlatform || isWSAPlatform;

                //return false;
            }
        }

        /// <summary>Checks if the current platform is Apple-based (macOS standalone or iOS).</summary>
        /// <returns>True if the current platform is Apple-based (macOS standalone or iOS).</returns>
        public static bool isAppleBasedPlatform
        {
            get
            {
                return isMacOSPlatform || isIOSPlatform;
            }
        }

        /// <summary>Checks if we are inside the Editor.</summary>
        /// <returns>True if we are inside the Editor.</returns>
        public static bool isEditor
        {
            get
            {
                return Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor;
            }
        }

        /// <summary>Checks if we are in Editor mode.</summary>
        /// <returns>True if in Editor mode.</returns>
        public static bool isEditorMode
        {
            get
            {
                return isEditor && !Application.isPlaying;
            }
        }

        /// <summary>Returns the current platform.</summary>
        /// <returns>The current platform.</returns>
        public static Model.Enum.Platform CurrentPlatform
        {
            get {
                if (isWindowsPlatform)
                {
                    return Model.Enum.Platform.Windows;
                }
                else if (isMacOSPlatform)
                {
                    return Model.Enum.Platform.OSX;
                }
                else if (isLinuxPlatform)
                {
                    return Model.Enum.Platform.Linux;
                }
                else if (isAndroidPlatform)
                {
                    return Model.Enum.Platform.Android;
                }
                else if (isIOSPlatform)
                {
                    return Model.Enum.Platform.IOS;
                }
                else if (isWSAPlatform)
                {
                    return Model.Enum.Platform.WSA;
                }
                else if (isWebPlatform)
                {
                    return Model.Enum.Platform.Web;
                }

                return Model.Enum.Platform.Unsupported;
            }
        }

        #endregion


        #region Public methods

        /// <summary>Creates a string of characters with a given length.</summary>
        /// <param name="chars">Characters to generate the string (if more than one character is used, the generated string will be a randomized result of all characters)</param>
        /// <param name="stringLength">Length of the generated string</param>
        /// <returns>Generated string</returns>
        public static string CreateString(string replaceChars, int stringLength)
        {
            if (replaceChars.Length > 1)
            {
                char[] chars = new char[stringLength];

                for (int ii = 0; ii < stringLength; ii++)
                {
                    chars[ii] = replaceChars[rnd.Next(0, replaceChars.Length)];
                }

                return new string(chars);
            }
            else if (replaceChars.Length == 1)
            {
                return new string(replaceChars[0], stringLength);
            }

            return string.Empty;
        }

        /// <summary>Determines if an AudioSource has an active clip.</summary>
        /// <param name="source">AudioSource to check.</param>
        /// <returns>True if the AudioSource has an active clip.</returns>
        public static bool hasActiveClip(AudioSource source)
        {
            /*
            Debug.Log("source.clip: " + source.clip);
            Debug.Log("source.loop: " + source.loop);
            Debug.Log("source.isPlaying: " + source.isPlaying);
            Debug.Log("source.timeSamples: " + source.timeSamples);
            Debug.Log("source.clip.samples: " + source.clip.samples);
            */

            return source != null && source.clip != null &&
                ((!source.loop && source.timeSamples > 0 && source.timeSamples < source.clip.samples - 256) ||
                source.loop ||
                source.isPlaying);
        }

#if !UNITY_WSA || UNITY_EDITOR
        /// <summary>HTTPS-certification callback.</summary>
        public static bool RemoteCertificateValidationCallback(System.Object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != System.Net.Security.SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = System.Security.Cryptography.X509Certificates.X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new System.TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = System.Security.Cryptography.X509Certificates.X509VerificationFlags.AllFlags;

                        isOk = chain.Build((System.Security.Cryptography.X509Certificates.X509Certificate2)certificate);
                    }
                }
            }

            return isOk;
        }
#endif

        /// <summary>Validates a given path and add missing slash.</summary>
        /// <param name="path">Path to validate</param>
        /// <param name="addEndDelimiter">Add delimiter at the end of the path (optional, default: true)</param>
        /// <returns>Valid path</returns>
        public static string ValidatePath(string path, bool addEndDelimiter = true)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string pathTemp = path.Trim();
                string result = null;

                if (isWindowsBasedPlatform)
                {
                    result = pathTemp.Replace('/', '\\');

                    if (addEndDelimiter)
                    {
                        if (!result.EndsWith(BaseConstants.PATH_DELIMITER_WINDOWS))
                        {
                            result += BaseConstants.PATH_DELIMITER_WINDOWS;
                        }
                    }
                }
                else
                {
                    result = pathTemp.Replace('\\', '/');

                    if (addEndDelimiter)
                    {
                        if (!result.EndsWith(BaseConstants.PATH_DELIMITER_UNIX))
                        {
                            result += BaseConstants.PATH_DELIMITER_UNIX;
                        }
                    }
                }

                return string.Join("_", result.Split(System.IO.Path.GetInvalidPathChars()));
            }

            return path;
        }

        /// <summary>Validates a given file.</summary>
        /// <param name="path">File to validate</param>
        /// <returns>Valid file path</returns>
        public static string ValidateFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string result = ValidatePath(path);

                if (result.EndsWith(BaseConstants.PATH_DELIMITER_WINDOWS) || result.EndsWith(BaseConstants.PATH_DELIMITER_UNIX))
                {
                    result = result.Substring(0, result.Length - 1);
                }

                string fileName = result.Substring(isWindowsBasedPlatform ? result.LastIndexOf(BaseConstants.PATH_DELIMITER_WINDOWS) + 1 : result.LastIndexOf(BaseConstants.PATH_DELIMITER_UNIX) + 1);

                string newName = string.Join(string.Empty, fileName.Split(System.IO.Path.GetInvalidFileNameChars())); //.Replace(BaseConstants.PATH_DELIMITER_WINDOWS, string.Empty).Replace(BaseConstants.PATH_DELIMITER_UNIX, string.Empty);

                return result.Substring(0, result.Length - fileName.Length) + newName;
            }

            return path;
        }

        /*
        /// <summary>Validates a given path and add missing slash.</summary>
        /// <param name="path">Path to validate</param>
        /// <returns>Valid path</returns>
        public static string ValidPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string pathTemp = path.Trim();
                string result = null;

                if (isWindowsPlatform)
                {
                    result = pathTemp.Replace('/', '\\');

                    if (!result.EndsWith(BaseConstants.PATH_DELIMITER_WINDOWS))
                    {
                        result += BaseConstants.PATH_DELIMITER_WINDOWS;
                    }
                }
                else
                {
                    result = pathTemp.Replace('\\', '/');

                    if (!result.EndsWith(BaseConstants.PATH_DELIMITER_UNIX))
                    {
                        result += BaseConstants.PATH_DELIMITER_UNIX;
                    }
                }

                return result;
            }

            return path;
        }

        /// <summary>Validates a given file.</summary>
        /// <param name="path">File to validate</param>
        /// <returns>Valid file path</returns>
        public static string ValidFilePath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {

                string result = ValidPath(path);

                if (result.EndsWith(BaseConstants.PATH_DELIMITER_WINDOWS) || result.EndsWith(BaseConstants.PATH_DELIMITER_UNIX))
                {
                    result = result.Substring(0, result.Length - 1);
                }

                return result;
            }

            return path;
        }
        */

        /// <summary>Validates a given file.</summary>
        /// <param name="path">File to validate</param>
        /// <returns>Valid file path</returns>
        public static string ValidURLFromFilePath(string path)
        {
            if (path == null)
                return path;

            if (!string.IsNullOrEmpty(path))
            {
                if (!isValidURL(path))
                {
                    return BaseConstants.PREFIX_FILE + ValidateFile(path).Replace(" ", "%20").Replace('\\', '/');
                }

                return ValidateFile(path).Replace(" ", "%20").Replace('\\', '/');
            }

            return System.Uri.EscapeDataString(path);
        }

        /// <summary>Cleans a given URL.
        /// <param name="url">URL to clean</param>
        /// <param name="removeProtocol">Remove the protocol, e.g. http:// (default: true, optional).</param>
        /// <param name="removeWWW">Remove www (default: true, optional).</param>
        /// <param name="removeSlash">Remove slash at the end (default: true, optional)</param>
        /// <returns>Clean URL</returns>
        public static string CleanUrl(string url, bool removeProtocol = true, bool removeWWW = true, bool removeSlash = true)
        {
            string result = url.Trim();

            if (!string.IsNullOrEmpty(url))
            {
                if (removeProtocol)
                {
                    result = result.Substring(result.IndexOf("//") + 2);
                }

                if (removeWWW)
                {
                    result = result.CTReplace("www.", string.Empty);
                }

                if (removeSlash && result.EndsWith(BaseConstants.PATH_DELIMITER_UNIX))
                {
                    result = result.Substring(0, result.Length - 1);
                }

                /*
                if (urlTemp.StartsWith("http://"))
                {
                    result = urlTemp.Substring(7);
                }
                else if (urlTemp.StartsWith("https://"))
                {
                    result = urlTemp.Substring(8);
                }
                else
                {
                    result = urlTemp;
                }

                if (result.StartsWith("www."))
                {
                    result = result.Substring(4);
                }
                */
            }

            return result;
        }

        /// <summary>Cleans a given text from tags.</summary>
        /// <param name="text">Text to clean.</param>
        /// <returns>Clean text without tags.</returns>
        public static string ClearTags(string text)
        {

            return cleanTagsRegex.Replace(text, string.Empty).Trim();
        }

        /// <summary>Cleans a given text from multiple spaces.</summary>
        /// <param name="text">Text to clean.</param>
        /// <returns>Clean text without multiple spaces.</returns>
        public static string ClearSpaces(string text)
        {

            return cleanSpacesRegex.Replace(text, " ").Trim();
        }

        /// <summary>Cleans a given text from line endings.</summary>
        /// <param name="text">Text to clean.</param>
        /// <returns>Clean text without line endings.</returns>
        public static string ClearLineEndings(string text)
        {

            return lineEndingsRegex.Replace(text, string.Empty).Trim();
        }

        /// <summary>Split the given text to lines and return it as list.</summary>
        /// <param name="text">Complete text fragment</param>
        /// <param name="ignoreCommentedLines">Ignore commente lines (default: true, optional)</param>
        /// <param name="skipHeaderLines">Number of skipped header lines (default: 0, optional)</param>
        /// <param name="skipFooterLines">Number of skipped footer lines (default: 0, optional)</param>
        /// <returns>Splitted lines as array</returns>
        public static System.Collections.Generic.List<string> SplitStringToLines(string text, bool ignoreCommentedLines = true, int skipHeaderLines = 0, int skipFooterLines = 0)
        {
            System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>(100);

            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("Parameter 'text' is null or empty!" + System.Environment.NewLine + "=> 'SplitStringToLines()' will return an empty string list.");
            }
            else
            {
                string[] lines = lineEndingsRegex.Split(text);

                for (int ii = 0; ii < lines.Length; ii++)
                {
                    if (ii + 1 > skipHeaderLines && ii < lines.Length - skipFooterLines)
                    {
                        if (!string.IsNullOrEmpty(lines[ii]))
                        {
                            if (ignoreCommentedLines)
                            {
                                if (!lines[ii].StartsWith("#"))
                                { //valid and not disabled line?
                                    result.Add(lines[ii]);
                                }
                            }
                            else
                            {
                                result.Add(lines[ii]);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>Format byte-value to Human-Readable-Form.</summary>
        /// <returns>Formatted byte-value in Human-Readable-Form.</returns>
        public static string FormatBytesToHRF(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return System.String.Format("{0:0.##} {1}", len, sizes[order]);
        }

        /// <summary>Format seconds to Human-Readable-Form.</summary>
        /// <returns>Formatted seconds in Human-Readable-Form.</returns>
        public static string FormatSecondsToHourMinSec(double seconds)
        {
            int totalSeconds = (int)seconds;
            int calcSeconds = totalSeconds % 60;

            if (seconds >= 86400)
            {
                int calcDays = totalSeconds / 86400;
                int calcHours = (totalSeconds -= calcDays * 86400) / 3600;
                int calcMinutes = (totalSeconds - calcHours * 3600) / 60;

                return calcDays + "d " + calcHours + ":" + (calcMinutes < 10 ? "0" + calcMinutes.ToString() : calcMinutes.ToString()) + ":" + (calcSeconds < 10 ? "0" + calcSeconds.ToString() : calcSeconds.ToString());
            }
            else if (seconds >= 3600)
            {
                int calcHours = totalSeconds / 3600;
                int calcMinutes = (totalSeconds - calcHours * 3600) / 60;

                return calcHours + ":" + (calcMinutes < 10 ? "0" + calcMinutes.ToString() : calcMinutes.ToString()) + ":" + (calcSeconds < 10 ? "0" + calcSeconds.ToString() : calcSeconds.ToString());
            }
            else
            {
                int calcMinutes = totalSeconds / 60;

                return calcMinutes + ":" + (calcSeconds < 10 ? "0" + calcSeconds.ToString() : calcSeconds.ToString());
            }
        }

        /// <summary>
        /// Generate nice HSV colors.
        /// Based on https://gist.github.com/rje/6206099
        /// </summary>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="v">Value</param>
        /// <param name="a">Alpha (optional)</param>
        /// <returns>True if the current platform is supported.</returns>
        public static Color HSVToRGB(float h, float s, float v, float a = 1f)
        {
            if (s == 0f)
            {
                return new Color(v, v, v, a);
            }

            h /= 60f;
            int sector = Mathf.FloorToInt(h);
            float fact = h - sector;
            float p = v * (1f - s);
            float q = v * (1f - s * fact);
            float t = v * (1f - s * (1f - fact));

            switch (sector)
            {
                case 0:
                    return new Color(v, t, p, a);
                case 1:
                    return new Color(q, v, p, a);
                case 2:
                    return new Color(p, v, t, a);
                case 3:
                    return new Color(p, q, v, a);
                case 4:
                    return new Color(t, p, v, a);
                default:
                    return new Color(v, p, q, a);
            }
        }

        /// <summary>Checks if the URL is valid.</summary>
        /// <param name="url">URL to check</param>
        /// <returns>True if the URL is valid.</returns>
        public static bool isValidURL(string url)
        {
            return string.IsNullOrEmpty(url) ? false : url.StartsWith(file_prefix, System.StringComparison.OrdinalIgnoreCase) || url.StartsWith(BaseConstants.PREFIX_HTTP, System.StringComparison.OrdinalIgnoreCase) || url.StartsWith(BaseConstants.PREFIX_HTTPS, System.StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Copy or move a file.</summary>
        /// <param name="inputFile">Input file path</param>
        /// <param name="outputFile">Output file path</param>
        /// <param name="move">Move file instead of copy (default: false, optional)</param>
        public static void FileCopy(string inputFile, string outputFile, bool move = false)
        {
            if (!string.IsNullOrEmpty(outputFile))
            {
                try
                {
                    if (!System.IO.File.Exists(inputFile))
                    {
                        Debug.LogError("Input file does not exists: " + inputFile);
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(outputFile));

                        if (System.IO.File.Exists(outputFile))
                        {
                            if (BaseConstants.DEV_DEBUG)
                                Debug.LogWarning("Overwrite output file: " + outputFile);

                            System.IO.File.Delete(outputFile);
                        }

                        if (move)
                        {
#if UNITY_STANDALONE || UNITY_EDITOR
                            System.IO.File.Move(inputFile, outputFile);
#else
                            System.IO.File.Copy(inputFile, outputFile);
                            System.IO.File.Delete(inputFile);
#endif
                        }
                        else
                        {
                            System.IO.File.Copy(inputFile, outputFile);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Could not copy file!" + System.Environment.NewLine + ex);
                }
            }
        }

        // StringHelper
        /*
        public static byte[] GetBytesFromText(string text) {
			return new UnicodeEncoding().GetBytes(text);
		}

		public static string GetTextFromBytes(byte[] bytes) {
			return new UnicodeEncoding().GetString(bytes, 0, bytes.Length);
		}

		public static byte[] GetBytesFromBase64(string text) {
			return Convert.FromBase64String(text);
		}

		public static string GetBase64FromBytes(byte[] bytes) {
			return Convert.ToBase64String(bytes);
		}
        */


        // MathHelper
        /*
        public static bool IsInRange(float actValue, float refValue, float range) {
			
			return (actValue >= refValue-range) && (actValue <= refValue+range);
		}


		public static bool IsInRange(int actValue, int refValue, int range) {
			
			return (actValue >= refValue-range) && (actValue <= refValue+range);
		}
        */


        // Add Math3dHelper?


        // Color Helper
        /*
        public static string ColorToHex(Color32 color)
        {
            //			if (color == null)
            //				throw new ArgumentNullException("color");

            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        public static Color HexToColor(string hex)
        {
            if (string.IsNullOrEmpty(hex))
                throw new ArgumentNullException("hex");

            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
        */

        #endregion
    }
}
// © 2015-2018 crosstales LLC (https://www.crosstales.com)