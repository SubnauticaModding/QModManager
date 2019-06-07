using Harmony;
using QModManager.Checks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Logger = QModManager.Utility.Logger;

namespace QModManager
{
    internal static class Dialog
    {
        internal class Button
        {
            internal readonly string Text = null;
            internal readonly Action Action = null;

            internal static readonly Button Disabled = new Button();
            internal static readonly Button SeeLog = new Button("See Log", () =>
            {
                string logPath;
                if (Patcher.CurrentlyRunningGame == API.QModGame.Subnautica)
                    logPath = Path.Combine(Patcher.QModBaseDir, "../Subnautica_Data/output_log.txt");
                else
                    logPath = Path.Combine(Application.persistentDataPath, "output_log.txt");
                Logger.Debug($"Opening log file located in: \"{logPath}\"");
                if (File.Exists(logPath))
                    Process.Start(logPath);
                else
                    Logger.Error($"Expected log file at: \"{logPath}\" but none was found.");
            });
            internal static readonly Button Close = new Button("Close", () => { });
            internal static readonly Button Download = new Button("Download", () =>
            {
                if (Patcher.CurrentlyRunningGame == API.QModGame.Subnautica)
                    Process.Start(VersionCheck.snNexus);
                else
                    Process.Start(VersionCheck.bzNexus);
            });

            private Button() { }
            internal Button(string text, Action action)
            {
                this.Text = text;
                this.Action = action;
            }
        }

        internal static void Show(string error, Button leftButton, Button rightButton, bool blue)
        {
            // Create a dummy GameObject to handle the coroutine
            GameObject couroutineHandler = new GameObject("QModManager Dialog Coroutine");
            couroutineHandler.AddComponent<DummyBehaviour>().StartCoroutine(
                ShowDialogEnumerator(error, leftButton?.Action, rightButton?.Action, leftButton?.Text, rightButton?.Text, blue, couroutineHandler));
        }

        private static IEnumerator ShowDialogEnumerator(string error, Action onLeftButtonPressed, Action onRightButtonPressed, string leftButtonText, string rightButtonText, bool blue, GameObject couroutineHandler)
        {
            while (AccessTools.Field(typeof(uGUI), "_main").GetValue(null) == null)
                yield return null;

            yield return new WaitForSecondsRealtime(3);

            uGUI_SceneConfirmation confirmation = uGUI.main.confirmation;

            // Disable buttons if their action is null
            if (onLeftButtonPressed == null) confirmation.yes.gameObject.SetActive(false);
            if (onRightButtonPressed == null) confirmation.no.gameObject.SetActive(false);

            // Disable buttons if their text is null, otherwise set their button text
            if (string.IsNullOrEmpty(leftButtonText)) confirmation.yes.gameObject.SetActive(false);
            else confirmation.yes.gameObject.GetComponentInChildren<Text>().text = leftButtonText;
            if (string.IsNullOrEmpty(rightButtonText)) confirmation.no.gameObject.SetActive(false);
            else confirmation.no.gameObject.GetComponentInChildren<Text>().text = rightButtonText;

            // Turn the dialog blue if the blue parameter is true
            Sprite sprite = confirmation.gameObject.GetComponentInChildren<Image>().sprite;
            if (blue)
                confirmation.gameObject.GetComponentInChildren<Image>().sprite = confirmation.gameObject.GetComponentsInChildren<Image>()[1].sprite;

            // Reduce the text size on the buttons by two pts
            List<Text> texts = confirmation.gameObject.GetComponentsInChildren<Text>().ToList();
            texts.RemoveAt(0);
            texts.Do(t => t.fontSize = t.fontSize - 2);

            // Show the dialog
            confirmation.Show(error, delegate (bool leftButtonClicked)
            {
                // Invoke the corresponding action after a button has been pressed
                if (leftButtonClicked) onLeftButtonPressed?.Invoke();
                else onRightButtonPressed?.Invoke();

                // Enable buttons after the dialog was closed
                confirmation.yes.gameObject.SetActive(true);
                confirmation.no.gameObject.SetActive(true);

                // Reset button text after the dialog was closed
                confirmation.yes.gameObject.GetComponentInChildren<Text>().text = "Yes";
                confirmation.no.gameObject.GetComponentInChildren<Text>().text = "No";

                // Reset color after the dialog was closed
                confirmation.gameObject.GetComponentInChildren<Image>().sprite = sprite;

                // Reset button text size after the dialog was closed
                texts.Do(t => t.fontSize = t.fontSize + 2);

                // Destroy the coroutine handler after the dialog was closed
                UnityEngine.Object.Destroy(couroutineHandler);
            });

            yield return null;
        }
    }
}
