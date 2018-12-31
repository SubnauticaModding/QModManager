using System;

namespace QModManager
{
    public static class ErrorMessage
    {
        public static void ShowError(string error, Action<bool> function, string leftButtonText = "Yes", bool disableRightButton = false, string rightButtonText = "No")
        {
            // Still need to implement leftButtonText and rightButtonText
            uGUI_SceneConfirmation confirmation = uGUI.main.confirmation;
            confirmation.no.gameObject.SetActive(true);
            confirmation.Show(error, delegate (bool confirmed) 
            {
                function.Invoke(confirmed);
                if (disableRightButton) confirmation.no.gameObject.SetActive(true);
            });
        }
    }
}
