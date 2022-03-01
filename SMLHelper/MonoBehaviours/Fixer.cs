namespace SMLHelper.V2.MonoBehaviours
{
    using System;
    using UnityEngine;

    /// <summary> This component is obsolete </summary>
    [Obsolete("Use SMLHelper.V2.Assets.ModPrefabCache instead", true)]
    public class Fixer : MonoBehaviour
    {
        private void Awake() => V2.Logger.Warn("Fixer component is obsolete.");
    }
}
