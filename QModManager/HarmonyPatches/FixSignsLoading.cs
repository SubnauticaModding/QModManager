// Original mod by OSubMarin
namespace QModManager.HarmonyPatches.FixSignsLoading
{
    using Harmony;
    using UnityEngine;

    /// <summary>
    /// Will fix the signs loading bug (see https://youtu.be/8eGj40Xzkag).
    /// This bug happens in vanilla version of the game (no mods installed).
    /// Note that the initial exceptions will still be raised upon loading (and logged in Player.log file).
    /// That's because this fix is applied as a postfix method (in other words, signs are fixed after their initial loading failure).
    /// A benefit from this is that it does not prevent modders from applying further modifications to the <see cref="uGUI_SignInput.UpdateScale"/> method.
    /// Also note that if something crash in there that probably means Unknown World fixed the issue (because it has been reported to them). If that's the case this entire file can be safely removed from QModManager project.
    /// </summary>
    [HarmonyPatch(typeof(uGUI_SignInput), nameof(uGUI_SignInput.UpdateScale))]
    internal static class uGUI_SignInputFixer_UpdateScale_Patch
    {
        /// <summary>
        /// This function is patched into the game using Harmony.
        /// </summary>
        /// <param name="__instance">The <see cref="uGUI_SignInput"/> instance that owns the method.</param>
        [HarmonyPostfix]
        internal static void Postfix(uGUI_SignInput __instance)
        {
            // If this uGUI_SignInput is enabled and attached to a game object.
            if (__instance.enabled && __instance.gameObject != null && __instance.gameObject.GetComponent<SignFixerComponent>() == null)
            {
                // If this uGUI_SignInput has a valid parent.
                if (__instance.transform != null && __instance.transform.parent != null)
                {
                    // Get Sign component from this uGUI_SignInput parent.
                    Sign sign = __instance.transform.parent.GetComponent<Sign>();
                    // If we were able to get the Sign component for this uGUI_SignInput.
                    if (sign != null)
                    {
                        // Add our fixer component to the game object.
                        __instance.gameObject.AddComponent<SignFixerComponent>();
                    }
                }
            }
        }
    }

    /// <summary>
    /// MonoBehaviour component attached to sign objects to fix their loading problem (see https://youtu.be/8eGj40Xzkag).
    /// </summary>
    public class SignFixerComponent : MonoBehaviour
    {
        /// <summary>
        /// This function gets called by <see cref="Awake"/> method.
        /// </summary>
        private void RestoreSignState()
        {
            // If current MonoBehaviour is enabled and if this component has a valid parent.
            if (enabled && transform != null && transform.parent != null)
            {
                // Get the Sign component from parent.
                Sign sign = transform.parent.GetComponent<Sign>();
                // If we were able to get the Sign component.
                if (sign != null)
                    sign.OnProtoDeserialize(null); // Restore state again now that RectTransform (base->uGUI_SignInput.rt) is set.
            }
        }

        /// <summary>
        /// This function gets called once the MonoBehaviour wakes up.
        /// </summary>
        public void Awake()
        {
            // If current MonoBehavour is enabled, call RestoreSignState next frame (we can't restore the Sign state this frame because the RectTransform property of Sign's base class uGUI_SignInput has not been set yet). 
            if (enabled)
                Invoke("RestoreSignState", 1f);
        }
    }
}
