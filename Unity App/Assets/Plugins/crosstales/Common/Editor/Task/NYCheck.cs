using UnityEngine;
using UnityEditor;

namespace Crosstales.Common.EditorTask
{
    /// <summary>Checks if a 'Happy new year'-message must be displayed.</summary>
    [InitializeOnLoad]
    public static class NYCheck
    {
        private const string KEY_NYCHECK_DATE = "CT_CFG_NYCHECK_DATE";

        #region Constructor

        static NYCheck()
        {
            string lastYear = EditorPrefs.GetString(KEY_NYCHECK_DATE);

            string year = System.DateTime.Now.ToString("yyyy");
            //string year = "9999"; //only for test

            string month = System.DateTime.Now.ToString("MM");
            //string month = "01"; //only for test

            if (!year.Equals(lastYear) && month.Equals("01"))
            {
                Debug.LogWarning(Util.BaseHelper.CreateString("-", 1000));
                Debug.LogWarning("¸.•°*”˜˜”*°•.¸ ★  crosstales LLC wishes you a happy and successful " + year + "!  ★ ¸.•*¨`*•.♫❤♫❤♫❤");
                Debug.LogWarning(Util.BaseHelper.CreateString("-", 1000));

                if (!year.Equals("9999"))
                    EditorPrefs.SetString(KEY_NYCHECK_DATE, year);
            }
        }

        #endregion

    }
}
// © 2017-2018 crosstales LLC (https://www.crosstales.com)