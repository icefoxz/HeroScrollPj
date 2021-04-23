using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.Utl;
using CorrelateLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginUiController : MonoBehaviour
{
    [Serializable]
    public enum ActionWindows
    {
        Login,
        Register,
        Info,
        ChangePassword,
        ForgetPassword,
        ResetAccount
    }
    public LoginUi login;
    public RegUi register;
    public AcInfoUi accountInfo;
    public ChangePwdUi changePassword;
    public RetrievePasswordUi forgetPassword;
    public ResetAccountUi resetAccount;
    public UnityAction<int,int> OnLoggedInAction;
    public UnityAction OnResetAccountAction;
    public Image busyPanel;

#if UNITY_EDITOR
    void Start()
    {
        OnLoggedInAction += (arrangement, newReg) =>
            XDebug.Log<LoginUiController>($"{nameof(OnLoggedInAction)} Invoke({arrangement},{newReg})!");
        OnResetAccountAction += () => XDebug.Log<LoginUiController>($"{nameof(OnResetAccountAction)} Invoke()!");
    }
#endif

    private Dictionary<ActionWindows, SignInBaseUi> _windowsObjs;

    private Dictionary<ActionWindows, SignInBaseUi> windowObjs
    {
        get
        {
            if (_windowsObjs == null)
                _windowsObjs = new Dictionary<ActionWindows, SignInBaseUi>
                {
                    {ActionWindows.Login, login},
                    {ActionWindows.Register, register},
                    {ActionWindows.Info, accountInfo},
                    {ActionWindows.ChangePassword, changePassword},
                    {ActionWindows.ForgetPassword, forgetPassword},
                    {ActionWindows.ResetAccount, resetAccount}
                };
            return _windowsObjs;
        }
    }

    public void OnAction(ActionWindows action)
    {
        if(!gameObject.activeSelf)
            gameObject.SetActive(true);
        ResetWindows();
        switch (action)
        {
            case ActionWindows.Login:
                InitLogin();
                break;
            case ActionWindows.Register:
                InitRegister();
                break;
            case ActionWindows.Info:
                InitAccountInfo();
                break;
            case ActionWindows.ChangePassword:
                InitChangePassword();
                break;
            case ActionWindows.ForgetPassword:
                InitForgetPassword();
                break;
            case ActionWindows.ResetAccount:
                InitResetAccount();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
        windowObjs[action].Open();
    }

    public void Close()
    {
        ResetWindows();
        gameObject.SetActive(false);
    }

    private void InitResetAccount()
    {
        resetAccount.backBtn.onClick.AddListener(()=>OnAction(ActionWindows.Login));
        resetAccount.resetBtn.onClick.AddListener(()=>AsyncInvoke(ResetAccountApi));
    }

    private async Task ResetAccountApi()
    {
        var response = await Http.PostAsync(Server.RESET_GAMEPLAY_API,
            Json.Serialize(Server.GetUserInfo(resetAccount.username.text, resetAccount.password.text)));
        var message = "账号重置成功！";
        if (!response.IsSuccessStatusCode) message = $"请求失败[{(int)response.StatusCode}]!";
        UnityMainThread.thread.RunNextFrame(() =>
        {
            OnResetAccountAction?.Invoke();
            OnAction(ActionWindows.Login);
            login.SetMessage(message);
        });
    }

    private void InitForgetPassword()
    {
        forgetPassword.backBtn.onClick.AddListener(()=>OnAction(ActionWindows.Login));
        forgetPassword.deviceLoginBtn.onClick.AddListener(DeviceLoginApi);
    }

    private void InitChangePassword()
    {
        changePassword.backBtn.onClick.AddListener(ResetWindows);
        changePassword.confirmBtn.onClick.AddListener(ChangePasswordApi);
    }

    private void ChangePasswordApi()
    {
        ApiPanel.instance.Invoke(arg =>
            {
                var vb = Json.Deserialize<ViewBag>(arg);
                if (vb != null) ResetWindows();
                PlayerDataForGame.instance.ShowStringTips(arg);
            }, EventStrings.Req_ChangePassword,
            ViewBag.Instance()
                .SetValues(GamePref.Username, SystemInfo.deviceUniqueIdentifier, changePassword.password));
    }

    private void InitAccountInfo()
    {
        accountInfo.backBtn.onClick.AddListener(ResetWindows);
        accountInfo.changePasswordBtn.onClick.AddListener(()=>OnAction(ActionWindows.ChangePassword));
    }

    private void InitRegister()
    {
        register.backBtn.onClick.AddListener(()=>OnAction(ActionWindows.Login));
        register.regBtn.onClick.AddListener(()=>AsyncInvoke(RegisterAccountApi));
    }

    private async Task RegisterAccountApi()
    {
        var userInfo = await Http.PostAsync<UserInfo>(Server.PLAYER_REG_ACCOUNT_API,
            Json.Serialize(Server.GetUserInfo(register.username.text, register.password.text)));
        if(userInfo==null)
        {
            register.message.text = "注册失败!";
            return;
        }
        UnityMainThread.thread.RunNextFrame(() =>
        {
            PlayerDataForGame.instance.ShowStringTips("注册成功!");
            OnAction(ActionWindows.Login);
        });
    }

    private void InitLogin()
    {
        login.directLoginBtn.onClick.AddListener(DeviceLoginApi);
        login.loginBtn.onClick.AddListener(AccountLoginApi);
        login.forgetPasswordBtn.onClick.AddListener(()=>OnAction(ActionWindows.ForgetPassword));
        login.regBtn.onClick.AddListener(()=>AsyncInvoke(RequestUsernameToRegister));
        login.resetAccountBtn.onClick.AddListener(()=>OnAction(ActionWindows.ResetAccount));
    }

    private async Task RequestUsernameToRegister()
    {
        var user = await Http.PostAsync<UserInfo>(Server.REQUEST_USERNAME_API,
            Json.Serialize(Server.GetUserInfo(null, null)));
        UnityMainThread.thread.RunNextFrame(() =>
        {
            OnAction(ActionWindows.Register);
            if (user == null)
            {
                register.message.text = "设备已绑定了账号，请用设备登录修改账号信息!";
                register.ShowPasswordUi(false);
                return;
            }
            register.username.text = user.Username;
        });
    }

    private async void AsyncInvoke(Func<Task> task)
    {
        busyPanel.gameObject.SetActive(true);
        await task.Invoke();
        UnityMainThread.thread.RunNextFrame(()=> busyPanel.gameObject.SetActive(false));
    }

    private void AccountLoginApi()
    {
        busyPanel.gameObject.SetActive(true);
        login.message.text = string.Empty;
        SignalRClient.instance.UserLogin(LoginAction, login.username.text, login.password.text);
    }

    private void DeviceLoginApi()
    {
        busyPanel.gameObject.SetActive(true);
        login.message.text = string.Empty;
        SignalRClient.instance.DirectLogin(LoginAction);
    }

    private void LoginAction(bool success, int code, int arrangement,int isNewReg)
    {
        busyPanel.gameObject.SetActive(false);
        if (success)
        {
            UnityMainThread.thread.RunNextFrame(() => OnLoggedInAction.Invoke(arrangement, isNewReg));
            return;
        }
        login.message.text = Server.ResponseMessage(code);
    }

    public void ResetWindows()
    {
        foreach (var obj in windowObjs) obj.Value.ResetUi();
    }

}

public abstract class SignInBaseUi : MonoBehaviour
{
    public Text message;
    public virtual void ResetUi()
    {
        ResetMessage();
        gameObject.SetActive(false);
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    public void SetMessage(string msg)
    {
        if(message) message.text = msg;
    }

    public void ResetMessage()
    {
        if(message) message.text = string.Empty;
    }
}