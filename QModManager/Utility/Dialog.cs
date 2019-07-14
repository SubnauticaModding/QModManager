namespace QModManager.Utility
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Harmony;
    using QModManager.Checks;
    using QModManager.Patching;
    using QModManager.API;
    using UnityEngine;
    using UnityEngine.UI;

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

                if (Patcher.CurrentlyRunningGame == QModGame.Subnautica)
                {
                    logPath = Path.Combine(Patcher.QModBaseDir, "../Subnautica_Data/output_log.txt");
                }
                else
                {
                    logPath = Path.Combine(Application.persistentDataPath, "output_log.txt");
                }

                Logger.Debug($"Opening log file located in: \"{logPath}\"");

                if (File.Exists(logPath))
                {
                    Process.Start(logPath);
                }
                else
                {
                    Logger.Error($"Expected log file at: \"{logPath}\" but none was found.");
                }
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
                Text = text;
                Action = action;
            }
        }

        internal static void Show(string error, Button leftButton, Button rightButton, bool blue, bool cannotClose = false, float waitTime = 3f)
        {
            // Create a dummy GameObject to handle the coroutine
            var couroutineHandler = new GameObject("QModManager Dialog Coroutine");
            couroutineHandler.AddComponent<DummyBehaviour>().StartCoroutine(
                ShowDialogEnumerator(error, leftButton, rightButton, blue, cannotClose, waitTime, couroutineHandler));
        }

        private static IEnumerator ShowDialogEnumerator(string error, Button leftButton, Button rightButton, bool blue, bool cannotClose, float waitTime, GameObject couroutineHandler)
        {
            while (AccessTools.Field(typeof(uGUI), "_main").GetValue(null) == null)
                yield return null;

            yield return new WaitForSecondsRealtime(waitTime);

            uGUI_SceneConfirmation confirmation = uGUI.main.confirmation;

            // Disable buttons if their action is null
            if (leftButton.Action == null)
                confirmation.yes.gameObject.SetActive(false);
            if (rightButton.Action == null)
                confirmation.no.gameObject.SetActive(false);

            // Disable buttons if their text is null, otherwise set their button text
            if (string.IsNullOrEmpty(leftButton.Text))
                confirmation.yes.gameObject.SetActive(false);
            else
                confirmation.yes.gameObject.GetComponentInChildren<Text>().text = leftButton.Text;
            if (string.IsNullOrEmpty(rightButton.Text))
                confirmation.no.gameObject.SetActive(false);
            else
                confirmation.no.gameObject.GetComponentInChildren<Text>().text = rightButton.Text;

            // Turn the dialog blue if the blue parameter is true
            Sprite sprite = confirmation.gameObject.GetComponentInChildren<Image>().sprite;
            if (blue)
                confirmation.gameObject.GetComponentInChildren<Image>().sprite = confirmation.gameObject.GetComponentsInChildren<Image>()[1].sprite;

            // Reduce the text size on the buttons by two pts
            var texts = confirmation.gameObject.GetComponentsInChildren<Text>().ToList();
            texts.RemoveAt(0);
            texts.Do(t => t.fontSize = t.fontSize - 2);

            // Show the dialog
            confirmation.Show(error, delegate (bool leftButtonClicked)
            {
                // Invoke the corresponding action after a button has been pressed
                if (leftButtonClicked)
                    leftButton.Action?.Invoke();
                else
                    rightButton.Action?.Invoke();

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

                // Re-open the dialog if it is not closeable
                if (cannotClose)
                    Show(error, leftButton, rightButton, blue, cannotClose, .25f);
            });

            yield return null;
        }
    }
}
