using UnityEngine;
using UnityEngine.UI;

public class ChangePwdUi : SignInBaseUi
{
    public InputField username;
    public InputField password;
    public InputField rePassword;
    public Button confirmBtn;
    public Button backBtn;

    public override void ResetUi()
    {
        username.text = GamePref.Username;
        password.text = string.Empty;
        rePassword.text = string.Empty;
        confirmBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();
        base.ResetUi();
    }
}