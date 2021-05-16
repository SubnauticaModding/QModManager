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
                string gameSuffix = "Subnautica";
                if (Patcher.CurrentlyRunningGame == QModGame.BelowZero)
                    gameSuffix += "Zero";

                string logPath = Path.Combine(Environment.CurrentDirectory, $"qmodmanager_log-{gameSuffix}.txt");

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
        private float pos = 0;
        private Sprite Sprite;

        internal void Show()
        {
            if(coroutineHandler is null)
            {
                coroutineHandler = new GameObject("QModManager Dialog Coroutine");
                coroutineHandler.AddComponent<DummyBehaviour>().StartCoroutine(ShowDialogEnumerator());
            }
            else if(coroutineHandler.TryGetComponent(out DummyBehaviour dummyBehaviour))
            {
                dummyBehaviour.StartCoroutine(ShowDialogEnumerator());
            }
            else
            {
                coroutineHandler.AddComponent<DummyBehaviour>().StartCoroutine(ShowDialogEnumerator());
            }
        }

        private IEnumerator ShowDialogEnumerator()
        {
            //Must have at least one button with an action
            if(leftButton.Action is null && rightButton.Action is null)
            {
                UnityEngine.Object.Destroy(coroutineHandler);
                yield break;
            }

            //Ensures a new dialog is not created until the main menu has fully loaded or it will break;
            while(uGUI_MainMenu.main is null || FPSInputModule.current is null || FPSInputModule.current.lastGroup != uGUI_MainMenu.main)
                yield return null;

            uGUI_SceneConfirmation confirmation = uGUI.main.confirmation;

            // Show dialog 
            //Had to move this before the code to change its values as the values are reset during the showing in BelowZero.
            confirmation.Show(message, OnCallBack);

            // Disable left button if its text or action is null, otherwise set their button text
            if(leftButton.Action == null || string.IsNullOrEmpty(leftButton.Text))
                confirmation.yes.gameObject.SetActive(false);
            else
                SetText(confirmation.yes.gameObject, leftButton.Text);

            // Disable right button if its text or action is null, otherwise set their button text
            if(rightButton.Action == null || string.IsNullOrEmpty(rightButton.Text))
                confirmation.no.gameObject.SetActive(false);
            else
                SetText(confirmation.no.gameObject, rightButton.Text);

            // Reduce the text size on the buttons
            ChangeAllFontSizes(confirmation.gameObject, -4f);

            // If one button is disabled, center the other
            if(confirmation.yes.gameObject.activeSelf && !confirmation.no.gameObject.activeSelf)
            {
                pos = confirmation.yes.transform.localPosition.x;
                confirmation.yes.transform.localPosition = new Vector3(
                    (confirmation.yes.transform.localPosition.x + confirmation.no.transform.localPosition.x) / 2,
                    confirmation.yes.transform.localPosition.y,
                    confirmation.yes.transform.localPosition.z);
            }
            else if(!confirmation.yes.gameObject.activeSelf && confirmation.no.gameObject.activeSelf)
            {
                pos = confirmation.no.transform.localPosition.x;
                confirmation.no.transform.localPosition = new Vector3(
                    (confirmation.no.transform.localPosition.x + confirmation.yes.transform.localPosition.x) / 2,
                    confirmation.no.transform.localPosition.y,
                    confirmation.no.transform.localPosition.z);
            }

            // Turn the dialog blue if the blue parameter is true
            Sprite = confirmation.gameObject.GetComponentInChildren<Image>().sprite;
            if(color == DialogColor.Blue)
                confirmation.gameObject.GetComponentInChildren<Image>().sprite = confirmation.gameObject.GetComponentsInChildren<Image>()[1].sprite;

        }

        internal void OnCallBack(bool leftButtonClicked)
        {
            uGUI_SceneConfirmation confirmation = uGUI.main.confirmation;

            // Run actions based on which button was pressed
            if(leftButtonClicked)
                leftButton.Action?.Invoke();
            else
                rightButton.Action?.Invoke();

            // Revert everything to its original state
            if(confirmation.yes.gameObject.activeSelf && !confirmation.no.gameObject.activeSelf)
            {
                confirmation.yes.transform.localPosition = new Vector3(
                    pos,
                    confirmation.yes.transform.localPosition.y,
                    confirmation.yes.transform.localPosition.z);
            }
            if(!confirmation.yes.gameObject.activeSelf && confirmation.no.gameObject.activeSelf)
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

            confirmation.gameObject.GetComponentInChildren<Image>().sprite = Sprite;

            ChangeAllFontSizes(confirmation.gameObject, 4f);

            // Re-open the dialog if the button pressed was not close
            bool closeButtonClicked = (leftButtonClicked && leftButton == Button.Close) || (!leftButtonClicked && rightButton == Button.Close);
            if(!closeButtonClicked)
                Show();
            else
                UnityEngine.Object.Destroy(coroutineHandler);
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

        private static void ChangeAllFontSizes(GameObject obj, float change)
        {
            if (fontSizeProperty == null)
                return;

            object[] textComponents = obj.GetComponentsInChildren(SelectedTextType);

            // Loop starts at 1 because text 0 is the main dialog text, which shouldn't be changed
            for(int i = 1; i < textComponents.Length; i++)
            {
                object t = textComponents[i];
                float originalSize = (float)fontSizeProperty.GetValue(t, null);
                float nextSize = originalSize + change;

                fontSizeProperty.SetValue(t, nextSize, null);
            }
        }
    }
}
