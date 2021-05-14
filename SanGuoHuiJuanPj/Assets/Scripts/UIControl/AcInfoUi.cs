using UnityEngine;
using UnityEngine.UI;

public class AcInfoUi : SignInBaseUi
{
    public InputField username;
    public InputField password;
    public GameObject warningMessage;
    public Button changePasswordBtn;
    public Button backBtn;

    public override void ResetUi()
    {
        username.text = GamePref.Username;
        password.text = string.Empty;
        changePasswordBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();
        base.ResetUi();
    }
}