using System;
using System.Diagnostics;
using System.IO;
using UnityEngine.UI;

namespace QModManager
{
    internal static class ErrorDialog
    {
        internal static void Show(string error, Action onLeftButtonPressed = null, Action onRightButtonPressed = null, string leftButtonText = "See Log", string rightButtonText = "Close")
        {
            uGUI_SceneConfirmation confirmation = uGUI.main.confirmation;

            if (onLeftButtonPressed == null)
            {
                onLeftButtonPressed = () => 
                {
                    Process.Start(Path.Combine(QModPatcher.QModBaseDir, "../Subnautica_Data/output_log.txt"));
                };
            }

            if (onRightButtonPressed == null)
            {
                onRightButtonPressed = () => 
                {
                    Console.Write("");
                };
            }

            if (string.IsNullOrEmpty(leftButtonText))
            {
                confirmation.yes.gameObject.SetActive(false);
            }
            else
            {
                confirmation.yes.gameObject.GetComponentInChildren<Text>().text = leftButtonText;
            }
            if (string.IsNullOrEmpty(rightButtonText))
            {
                confirmation.no.gameObject.SetActive(false);
            }
            else
            {
                confirmation.no.gameObject.GetComponentInChildren<Text>().text = rightButtonText;
            }

            confirmation.Show(error, delegate (bool leftButtonClicked)
            {
                if (leftButtonClicked) onLeftButtonPressed.Invoke();
                else onRightButtonPressed.Invoke();

                Console.Write("");

                confirmation.yes.gameObject.SetActive(true);
                confirmation.no.gameObject.SetActive(true);

                confirmation.yes.gameObject.GetComponentInChildren<Text>().text = "Yes";
                confirmation.no.gameObject.GetComponentInChildren<Text>().text = "No";
            });
        }
    }
}
