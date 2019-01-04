#if UNITY_WSA && CT_ENABLED
using UnityEngine;
using UnityEditor;

namespace Crosstales.RTVoice.EditorTask
{
/// <summary>Checks if the current build target is WSA (UWP).</summary>
    [InitializeOnLoad]
    public static class WSACheck
    {

#region Constructor

        static WSACheck()
        {
            if (!Util.Constants.isPro)
            {
                if (EditorUtility.DisplayDialog(Util.Constants.ASSET_NAME + " - WSA",
                    "The standard version of this asset doesn't support WSA (UWP)." +
                    System.Environment.NewLine +
                    System.Environment.NewLine +
                    "Please consider an upgrade to the PRO version.",
                    "Go PRO", "Cancel"))
                {
                    Application.OpenURL(Util.Constants.ASSET_PRO_URL);
                }
            }
        }

#endregion

    }
}
#endif
// © 2018 crosstales LLC (https://www.crosstales.com)