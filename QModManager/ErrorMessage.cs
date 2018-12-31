using System;

namespace QModManager
{
    public static class ErrorMessage
    {
        public static void ShowError(string error, Action<bool> function, string leftButtonText = "Yes", bool disableRightButton = false, string RightButtonText = "No")
        {
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
