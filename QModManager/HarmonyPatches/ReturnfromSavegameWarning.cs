namespace QModManager.Patching
{
    using HarmonyLib;
    using QModManager.API;
    using MyLogger = Utility;
    using UWE;
    using System.Collections;
    using UnityEngine;

    internal static class ReturnfromSavegameWarning
    {
        public static bool AnySavegamewasloaded = false;
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    internal static class ReturnfromSavegameWarningPlayerAwake
    {
        [HarmonyPostfix]
        internal static void Postfix(Player __instance)
        {
            if (ReturnfromSavegameWarning.AnySavegamewasloaded)
            {
                MyLogger.Logger.Error("Entering a Savegame after playing a other one without restarting the Game. Modders do not recommend that. Restart the Game to prevent errors or unexpected behaviour");
                if (Utility.Config.ShowWarnOnLoadSecondSave)
                {
                    CoroutineHost.StartCoroutine(ShowIngameMessage_async("Modders do not recommend loading multiple savegames without restarting the game."));
                }
            }
            else
            {
                ReturnfromSavegameWarning.AnySavegamewasloaded = true;
            }
        }
        public static IEnumerator ShowIngameMessage_async(string Message)
        {
            yield return new WaitForSecondsRealtime(2);
            yield return new WaitForSeconds(3);
            ErrorMessage.AddMessage(Message);
        }
    }

    [HarmonyPatch(typeof(uGUI_MainMenu), nameof(uGUI_MainMenu.Awake))]
    internal static class ReturnfromSavegameWarninguGUI_MainMenuAwake
    {
        [HarmonyPostfix]
        internal static void Postfix(uGUI_OptionsPanel __instance)
        {
            if (ReturnfromSavegameWarning.AnySavegamewasloaded)
            {
                MyLogger.Logger.Error("Entering Main Menu after playing a Savegame. Modders do not recommend to start or load a Savegame now. Restart the Game to prevent errors or unexpected behaviour");
                if (Utility.Config.ShowWarnOnLoadSecondSave)
                {
                    //QModServices.Main.AddCriticalMessage("Note that Modders recommend to restart the Game before loading the next Savegame", 15, "orange");
                }
            }
        }
    }
}
