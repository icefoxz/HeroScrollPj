using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.UI;

public class RegUi : SignInBaseUi
{
    public InputField username;
    public InputField password;
    public InputField rePassword;
    public List<GameObject> disableList;
    public Button regBtn;
    public Button backBtn;
    

    public override void ResetUi()
    {
        username.text = GamePref.Username;
        regBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();
        password.text = string.Empty;
        rePassword.text = string.Empty;
        password.gameObject.SetActive(true);
        rePassword.gameObject.SetActive(true);
        ShowPasswordUi(true);
        base.ResetUi();
    }

    public void ShowPasswordUi(bool show) => disableList.ForEach(o =>
    {
        if (o) o.gameObject.SetActive(show);
    });
}