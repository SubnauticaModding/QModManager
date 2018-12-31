using System;

namespace QModManager
{
    internal static class Error
    {
        internal static void ShowError(string error, Action<bool> function, string leftButtonText = "Yes", string rightButtonText = "No")
        {
            // Still need to implement leftButtonText and rightButtonText
            uGUI_SceneConfirmation confirmation = uGUI.main.confirmation;

            if (function == null) function = (_) => { };

            if (string.IsNullOrEmpty(leftButtonText)) confirmation.yes.gameObject.SetActive(false);
            if (string.IsNullOrEmpty(rightButtonText)) confirmation.no.gameObject.SetActive(false);

            confirmation.Show(error, delegate (bool leftButtonClicked)
            {
                function.Invoke(leftButtonClicked);
                confirmation.no.gameObject.SetActive(true);
                confirmation.yes.gameObject.SetActive(true);
            });
        }

        internal static void ShowError(string error, Action onLeftButton, Action onRightButton, string leftButtonText = "Yes", string rightButtonText = "No")
        {
            if (onLeftButton == null) onLeftButton = () => { };
            if (onRightButton == null) onRightButton = () => { };
            ShowError(error, leftButtonClicked =>
                       {
                           if (leftButtonClicked) onLeftButton.Invoke();
                               else onRightButton.Invoke();
                       }, leftButtonText, rightButtonText);
        }
    }
}
