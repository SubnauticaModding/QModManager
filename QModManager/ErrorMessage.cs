using System;

namespace QModManager
{
    public static class ErrorMessage
    {
        public static void ShowError(string error, Action<bool> function, string leftButtonText = "Yes", string rightButtonText = "No")
        {
            // Still need to implement leftButtonText and rightButtonText
            uGUI_SceneConfirmation confirmation = uGUI.main.confirmation;

            if (string.IsNullOrEmpty(leftButtonText)) confirmation.yes.gameObject.SetActive(false);
            if (string.IsNullOrEmpty(rightButtonText)) confirmation.no.gameObject.SetActive(false);

            confirmation.Show(error, delegate (bool leftButtonClicked)
            {
                function.Invoke(leftButtonClicked);
                confirmation.no.gameObject.SetActive(true);
                confirmation.yes.gameObject.SetActive(true);
            });
        }

        public static void ShowError(string error, Action onLeftButton, Action onRightButton, string leftButtonText = "Yes", string rightButtonText = "No")
            => ShowError(error, leftButtonClicked =>
            {
                if (leftButtonClicked) onLeftButton.Invoke();
                else onRightButton.Invoke();
            }, leftButtonText, rightButtonText);
    }
}
