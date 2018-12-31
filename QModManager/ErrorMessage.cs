using System;

namespace QModManager
{
    public static class ErrorMessage
    {
        public static void ShowError(string error, Action<bool> function, string leftButtonText = "Yes", string rightButtonText = "No")
        {
            // Still need to implement leftButtonText and rightButtonText
            uGUI_SceneConfirmation confirmation = uGUI.main.confirmation;

            if (leftButtonText == "" || leftButtonText == null) confirmation.no.gameObject.SetActive(false);
            if (rightButtonText == "" || rightButtonText == null) confirmation.no.gameObject.SetActive(false);
            confirmation.Show(error, delegate (bool confirmed) 
            {
                function.Invoke(confirmed);
                confirmation.no.gameObject.SetActive(true);
                confirmation.yes.gameObject.SetActive(true);
            });
        }
    }
}
