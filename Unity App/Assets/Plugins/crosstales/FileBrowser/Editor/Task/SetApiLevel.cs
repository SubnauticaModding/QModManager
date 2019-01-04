using UnityEditor;
using UnityEngine;

namespace Crosstales.FB.EditorExt
{
    /// <summary>Sets the required .NET API level.</summary>
    [InitializeOnLoad]
    public static class SetApiLevel
    {

        #region Constructor

        static SetApiLevel()
        {
            //TODO add new entries from Unity 2019

            ApiCompatibilityLevel level = ApiCompatibilityLevel.NET_2_0;
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

            //Debug.Log("API level: " + PlayerSettings.GetApiCompatibilityLevel(group));

            if (!PlayerSettings.GetApiCompatibilityLevel(group).ToString().Equals("NET_4_6")
                && PlayerSettings.GetApiCompatibilityLevel(group) != level
#if UNITY_2018 || UNITY_2019
                && PlayerSettings.GetApiCompatibilityLevel(group) != ApiCompatibilityLevel.NET_Standard_2_0
#endif
                )
            {
                PlayerSettings.SetApiCompatibilityLevel(group, level);

                Debug.Log("API level changed to '" + level + "'");
            }
        }

        #endregion
    }
}
// © 2017-2018 crosstales LLC (https://www.crosstales.com)