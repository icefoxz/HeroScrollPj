using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SignInUi : MonoBehaviour
{
    public enum Modes
    {
        Disable,
        Login,
        SignUp
    }
    public int MessageHideInSecs = 5;
    public InputField UsernameField;
    public InputField PasswordField;
    public InputField ConfirmPwdField;
    public Text ConfirmPwdLabel;
    public Button SighUpBtn;
    public Button LoginBtn;
    public Text Message;
    private Modes currentMode;
    private bool IsUsernameEmpty => string.IsNullOrWhiteSpace(UsernameField.text);
    private bool IsPasswordEmpty => string.IsNullOrWhiteSpace(PasswordField.text);
    private bool IsPasswordNotMatch => ConfirmPwdField.text != PasswordField.text;

    public void SetValue(string username, string password, string confirmPwd = null)
    {
        UsernameField.text = username;
        PasswordField.text = password;
        ConfirmPwdField.text = confirmPwd;
    }

    public void Hide() => gameObject.SetActive(false);

    public void SetMode(Modes mode, UnityAction action = null)
    {
        currentMode = mode;
        Message.text = string.Empty;
        SighUpBtn.interactable = mode != Modes.Disable;
        SighUpBtn.gameObject.SetActive(mode == Modes.SignUp);
        LoginBtn.interactable = mode != Modes.Disable;
        LoginBtn.gameObject.SetActive(mode == Modes.Login);
        UsernameField.interactable = false;
        PasswordField.interactable = mode != Modes.Disable;
        ConfirmPwdField.gameObject.SetActive(mode == Modes.SignUp);
        ConfirmPwdLabel.gameObject.SetActive(mode == Modes.SignUp);
        gameObject.SetActive(true);
        SighUpBtn.onClick.RemoveAllListeners();
        LoginBtn.onClick.RemoveAllListeners();
        if (mode == Modes.Login) LoginBtn.onClick.AddListener(action);
        if (mode == Modes.SignUp) SighUpBtn.onClick.AddListener(action);
    }

    public bool IsReadyAction()
    {
        if (currentMode == Modes.Disable) return false;
        if (IsUsernameEmpty)
        {
            ShowMessage("无效用户Id!");
            return false;
        }

        if (IsPasswordEmpty)
        {
            ShowMessage("请输入密码!");
            return false;
        }

        if (currentMode == Modes.SignUp && IsPasswordNotMatch)
        {
            ShowMessage("密码不同!");
            return false;
        }
        return true;
    }

    public void ShowMessage(string message)
    {
        if(!gameObject.activeSelf)return;
        StopAllCoroutines();
        StartCoroutine(MessagingInSecs(message));
    }

    IEnumerator MessagingInSecs(string msg)
    {
        Message.text = msg;
        yield return new WaitForSeconds(MessageHideInSecs);
        Message.text = string.Empty;
    }
}
