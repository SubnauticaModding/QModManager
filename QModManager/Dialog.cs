using Harmony;
using QModManager.Checks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Logger = QModManager.Utility.Logger;

namespace QModManager
{
    internal static class Dialog
    {
        internal class Button
        {
            internal string text = null;
            internal Action action = null;

            internal static readonly Button disabled = new Button();
            internal static readonly Button seeLog = new Button("See Log", () =>
            {
                string logPath;
                if (Patcher.game == Patcher.Game.Subnautica)
                    logPath = Path.Combine(Patcher.QModBaseDir, "../Subnautica_Data/output_log.txt");
                else
                    logPath = Path.Combine(Application.persistentDataPath, "output_log.txt");
                Logger.Debug($"Opening log file located in: \"{logPath}\"");
                if (File.Exists(logPath))
                    Process.Start(logPath);
                else
                    Logger.Error($"Expected log file at: \"{logPath}\" but none was found.");
            });
            internal static readonly Button close = new Button("Close", () => { });
            internal static readonly Button download = new Button("Download", () =>
            {
                if (Patcher.game == Patcher.Game.Subnautica)
                    Process.Start(VersionCheck.snNexus);
                else
                    Process.Start(VersionCheck.bzNexus);
            });

            internal Button() { }
            internal Button(string text, Action action)
            {
                this.text = text;
                this.action = action;
            }
        }

        internal static void Show(string error, Button leftButton, Button rightButton, bool blue)
        {
            GameObject couroutineHandler = new GameObject("QModManager Dialog Coroutine");
            couroutineHandler.AddComponent<DummyBehaviour>().StartCoroutine(ShowDialogEnumerator(error, leftButton?.action, rightButton?.action, leftButton?.text, rightButton?.text, blue, couroutineHandler));
        }

        internal static IEnumerator ShowDialogEnumerator(string error, Action onLeftButtonPressed, Action onRightButtonPressed, string leftButtonText, string rightButtonText, bool blue, GameObject couroutineHandler)
        {
            while (typeof(uGUI).GetField("_main", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) == null)
                yield return null;

            yield return new WaitForSecondsRealtime(2);

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
            {
                confirmation.gameObject.GetComponentInChildren<Image>().sprite = confirmation.gameObject.GetComponentsInChildren<Image>()[1].sprite;
            }

            // Reduce the text size on the buttons by two pts
            List<Text> texts = confirmation.gameObject.GetComponentsInChildren<Text>().ToList();
            texts.RemoveAt(0);
            texts.Do(t => t.fontSize = t.fontSize - 2);

            // Revert everything after the popup was closed
            confirmation.Show(error, delegate (bool leftButtonClicked)
            {
                if (leftButtonClicked) onLeftButtonPressed?.Invoke();
                else onRightButtonPressed?.Invoke();

                confirmation.yes.gameObject.SetActive(true);
                confirmation.no.gameObject.SetActive(true);

                confirmation.yes.gameObject.GetComponentInChildren<Text>().text = "Yes";
                confirmation.no.gameObject.GetComponentInChildren<Text>().text = "No";

                confirmation.gameObject.GetComponentInChildren<Image>().sprite = sprite;

                texts.Do(t => t.fontSize = t.fontSize + 2);

                UnityEngine.Object.Destroy(couroutineHandler);
            });

            yield return null;
        }
    }
}
