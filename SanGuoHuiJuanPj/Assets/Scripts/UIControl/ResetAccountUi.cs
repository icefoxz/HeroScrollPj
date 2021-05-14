using UnityEngine.UI;

public class ResetAccountUi : SignInBaseUi
{
    public InputField username;
    public InputField password;
    public Button resetBtn;
    public Button backBtn;

    public override void ResetUi()
    {
        username.text = GamePref.Username;
        password.text = string.Empty;
        resetBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();
        base.ResetUi();
    }
}