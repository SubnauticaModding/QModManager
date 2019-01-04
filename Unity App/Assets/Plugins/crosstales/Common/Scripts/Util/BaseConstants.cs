using UnityEngine;

namespace Crosstales.Common.Util
{
    /// <summary>Base for collected constants of very general utility for the asset.</summary>
    public abstract class BaseConstants
    {
        #region Constant variables

        /// <summary>Author of the asset.</summary>
        public const string ASSET_AUTHOR = "crosstales LLC";

        /// <summary>URL of the asset author.</summary>
        public const string ASSET_AUTHOR_URL = "https://www.crosstales.com";

        /// <summary>URL of the crosstales assets in UAS.</summary>
        public const string ASSET_CT_URL = "https://goo.gl/qwtXyb";
        //public const string ASSET_CT_URL = "https://www.assetstore.unity3d.com/#!/list/42213-crosstales?aid=1011lNGT"; // crosstales list

        /// <summary>URL of the crosstales Discord-channel.</summary>
        public const string ASSET_SOCIAL_DISCORD = "https://discord.gg/ZbZ2sh4";

        /// <summary>URL of the crosstales Facebook-profile.</summary>
        public const string ASSET_SOCIAL_FACEBOOK = "https://www.facebook.com/crosstales/";

        /// <summary>URL of the crosstales Twitter-profile.</summary>
        public const string ASSET_SOCIAL_TWITTER = "https://twitter.com/crosstales";

        /// <summary>URL of the crosstales Youtube-profile.</summary>
        public const string ASSET_SOCIAL_YOUTUBE = "https://www.youtube.com/c/Crosstales";

        /// <summary>URL of the crosstales LinkedIn-profile.</summary>
        public const string ASSET_SOCIAL_LINKEDIN = "https://www.linkedin.com/company/crosstales";

        /// <summary>URL of the crosstales XING-profile.</summary>
        public const string ASSET_SOCIAL_XING = "https://www.xing.com/companies/crosstales";

        /// <summary>URL of the 3rd party asset "PlayMaker".</summary>
        public const string ASSET_3P_PLAYMAKER = "https://www.assetstore.unity3d.com/#!/content/368?aid=1011lNGT";

        /// <summary>Factor for kilo bytes.</summary>
        public const int FACTOR_KB = 1024;

        /// <summary>Factor for mega bytes.</summary>
        public const int FACTOR_MB = FACTOR_KB * 1024;

        /// <summary>Factor for giga bytes.</summary>
        public const int FACTOR_GB = FACTOR_MB * 1024;

        /// <summary>Float value of 32768.</summary>
        public const float FLOAT_32768 = 32768f;

        /// <summary>ToString for two decimal places.</summary>
        public const string FORMAT_TWO_DECIMAL_PLACES = "0.00";

        /// <summary>ToString for no decimal places.</summary>
        public const string FORMAT_NO_DECIMAL_PLACES = "0";

        /// <summary>ToString for percent.</summary>
        public const string FORMAT_PERCENT = "0%";


        // Default values
        public const bool DEFAULT_DEBUG = false;

        /// <summary>Path delimiter for Windows.</summary>
        public const string PATH_DELIMITER_WINDOWS = @"\";

        /// <summary>Path delimiter for Unix.</summary>
        public const string PATH_DELIMITER_UNIX = "/";

        /// <summary>Application path.</summary>
        public static readonly string APPLICATION_PATH = BaseHelper.ValidatePath(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') + 1));

        #endregion


        #region Changable variables

        /// <summary>Development debug logging for the asset.</summary>
        public static bool DEV_DEBUG = false;

        // Text fragments for the asset
        public static string TEXT_TOSTRING_END = "}";
        public static string TEXT_TOSTRING_DELIMITER = "', ";
        public static string TEXT_TOSTRING_DELIMITER_END = "'";
        public static string TEXT_TOSTRING_START = " {";

        // Prefixes for URLs and paths
        public static string PREFIX_HTTP = "http://";
        public static string PREFIX_HTTPS = "https://";

        /// <summary>Kill processes after 5000 milliseconds.</summary>
        public static int PROCESS_KILL_TIME = 5000;

        #endregion


        #region Properties

        public static string PREFIX_FILE
        { //TODO verify!
            get
            {
                if (BaseHelper.isWindowsPlatform)
                {
                    return "file:///";
                }
                else
                {
                    return "file://";
                }
            }
        }

        #endregion

    }
}
// © 2015-2018 crosstales LLC (https://www.crosstales.com)