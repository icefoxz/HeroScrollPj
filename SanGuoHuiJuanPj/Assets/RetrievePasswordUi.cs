using UnityEngine.UI;

public class RetrievePasswordUi : SignInBaseUi
{
    public InputField username;
    public Button deviceLoginBtn;
    public Button backBtn;

    public override void ResetUi()
    {
        username.text = GamePref.Username;
        base.ResetUi();
    }
}