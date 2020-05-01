namespace QModManager.Utility
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using QModManager.API;
    using QModManager.Checks;
    using QModManager.Patching;
    using UnityEngine;
    using UnityEngine.UI;

    internal class Dialog
    {
        private static Type SelectedTextType;
        private static PropertyInfo textProperty;
        private static PropertyInfo fontSizeProperty;

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
                SetText(confirmation.yes.gameObject, leftButton.Text);
            if (string.IsNullOrEmpty(rightButton.Text))
                confirmation.no.gameObject.SetActive(false);
            else
                SetText(confirmation.no.gameObject, rightButton.Text);

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
            object[] texts = ChangeAllFontSizes(confirmation.gameObject, -4f);

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

                SetText(confirmation.yes.gameObject, "Yes");
                SetText(confirmation.no.gameObject, "No");

                confirmation.gameObject.GetComponentInChildren<Image>().sprite = sprite;

                ChangeAllFontSizes(texts, 4f);

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

        private static void SetText(GameObject obj, string text)
        {
            if (SelectedTextType == null)
            {
                Type TxtType = typeof(Text);
                Type TxtProType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro", false, false);

                if (TxtProType != null && obj.GetComponentInChildren(TxtProType) != null)
                {
                    SelectedTextType = TxtProType;
                }
                else if (TxtType != null && obj.GetComponentInChildren(TxtType))
                {
                    SelectedTextType = TxtType;
                }
                else
                {
                    Logger.Error("Unable to find Text component in dialog box");
                    return;
                }

                textProperty = SelectedTextType.GetProperty("text", typeof(string));
                fontSizeProperty = SelectedTextType.GetProperty("fontSize", typeof(float));
            }

            object txt = obj.GetComponentInChildren(SelectedTextType);

            textProperty.SetValue(txt, text, null);
        }

        private static object[] ChangeAllFontSizes(GameObject obj, float change)
        {
            if (fontSizeProperty == null)
                return null;

            object[] textComponents = obj.GetComponentsInChildren(SelectedTextType);

            ChangeAllFontSizes(textComponents, change);

            return textComponents;
        }

        private static void ChangeAllFontSizes(object[] textComponents, float change)
        {
            if (fontSizeProperty == null)
                return;

            for (int i = 1; i < textComponents.Length; i++)
            {
                object t = textComponents[i];
                float originalSize = (float)fontSizeProperty.GetValue(t, null);
                float nextSize = originalSize + change;

                fontSizeProperty.SetValue(t, nextSize, null);
            }
        }
    }
}
