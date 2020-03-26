namespace QModManager.Utility
{
    using Harmony;
    using QModManager.API;
    using QModManager.Checks;
    using QModManager.Patching;
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    internal class Dialog
    {
        internal class Button
        {
            internal string Text = null;
            internal Action Action = null;

            internal static readonly Button Disabled = new Button();
            internal static readonly Button SeeLog = new Button("See Log", () =>
            {
                string logPath = Application.consoleLogPath;

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
                if (Patcher.CurrentlyRunningGame == QModGame.Subnautica)
                    Process.Start(VersionCheck.snNexus);
                else
                    Process.Start(VersionCheck.bzNexus);
            });

            internal Button() { }
            internal Button(string text, Action action)
            {
                Text = text;
                Action = action;
            }
        }

        internal enum DialogColor
        {
            Red,
            Blue
        }

        internal string message = "";
        internal DialogColor color = DialogColor.Blue;
        internal Button leftButton = Button.Disabled;
        internal Button rightButton = Button.Disabled;

        private GameObject coroutineHandler;

        internal void Show()
        {
            coroutineHandler = new GameObject("QModManager Dialog Coroutine");
            coroutineHandler.AddComponent<DummyBehaviour>().StartCoroutine(ShowDialogEnumerator());
        }

        private IEnumerator ShowDialogEnumerator()
        {
            while (uGUI._main == null)
                yield return null;

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

            // If one button is disabled, center the other
            float pos = 0;
            if (confirmation.yes.gameObject.activeSelf && !confirmation.no.gameObject.activeSelf)
            {
                pos = confirmation.yes.transform.localPosition.x;
                confirmation.yes.transform.localPosition = new Vector3(
                    (confirmation.yes.transform.localPosition.x + confirmation.no.transform.localPosition.x) / 2,
                    confirmation.yes.transform.localPosition.y,
                    confirmation.yes.transform.localPosition.z);
            }
            if (!confirmation.yes.gameObject.activeSelf && confirmation.no.gameObject.activeSelf)
            {
                pos = confirmation.no.transform.localPosition.x;
                confirmation.no.transform.localPosition = new Vector3(
                    (confirmation.no.transform.localPosition.x + confirmation.yes.transform.localPosition.x) / 2,
                    confirmation.no.transform.localPosition.y,
                    confirmation.no.transform.localPosition.z);
            }

            // Turn the dialog blue if the blue parameter is true
            Sprite sprite = confirmation.gameObject.GetComponentInChildren<Image>().sprite;
            if (color == DialogColor.Blue)
                confirmation.gameObject.GetComponentInChildren<Image>().sprite = confirmation.gameObject.GetComponentsInChildren<Image>()[1].sprite;

            // Reduce the text size on the buttons
            var texts = confirmation.gameObject.GetComponentsInChildren<Text>().ToList();
            texts.RemoveAt(0);
            texts.Do(t => t.fontSize = t.fontSize - 4);

            // Show dialog
            confirmation.Show(message, delegate (bool leftButtonClicked)
            {
                // Run actions based on which button was pressed
                if (leftButtonClicked)
                    leftButton.Action?.Invoke();
                else
                    rightButton.Action?.Invoke();

                // Revert everything to its original state
                if (confirmation.yes.gameObject.activeSelf && !confirmation.no.gameObject.activeSelf)
                {
                    confirmation.yes.transform.localPosition = new Vector3(
                        pos,
                        confirmation.yes.transform.localPosition.y,
                        confirmation.yes.transform.localPosition.z);
                }
                if (!confirmation.yes.gameObject.activeSelf && confirmation.no.gameObject.activeSelf)
                {
                    confirmation.no.transform.localPosition = new Vector3(
                        pos,
                        confirmation.no.transform.localPosition.y,
                        confirmation.no.transform.localPosition.z);
                }

                confirmation.yes.gameObject.SetActive(true);
                confirmation.no.gameObject.SetActive(true);

                confirmation.yes.gameObject.GetComponentInChildren<Text>().text = "Yes";
                confirmation.no.gameObject.GetComponentInChildren<Text>().text = "No";

                confirmation.gameObject.GetComponentInChildren<Image>().sprite = sprite;

                texts.Do(t => t.fontSize = t.fontSize + 4);

                UnityEngine.Object.Destroy(coroutineHandler);

                // Re-open the dialog if the button pressed was not close
                bool closeButtonClicked = (leftButtonClicked && leftButton == Button.Close) || (!leftButtonClicked && rightButton == Button.Close);
                if (!closeButtonClicked)
                    Show();
            });

            // Focus popup
            while (confirmation.selected)
                yield return new WaitForSecondsRealtime(0.25f);

            confirmation.Select();
        }
    }
}
