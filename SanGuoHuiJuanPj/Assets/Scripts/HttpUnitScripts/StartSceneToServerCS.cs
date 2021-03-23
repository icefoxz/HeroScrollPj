using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartSceneToServerCS : MonoBehaviour
{
    public const string AccountId = "accountName";
    private const string Password = "Password";
    private const string PhoneNumber = "Phone";

#if UNITY_EDITOR
    public bool isSkipLogin;//是否跳过登录
    public bool isSkipInitBattle;//是否跳过初始战斗
#endif

    public static StartSceneToServerCS instance;

    //删除所有
    public void ClearAllData()
    {
        PlayerPrefs.DeleteAll();
        LoadSaveData.instance.DeleteAllSaveData();
    }

    /// <summary> 
    /// 清除帐户 
    /// </summary> 
    public void ClearAccountData()
    {
        PlayerDataForGame.instance.acData.Username = string.Empty;
        PlayerDataForGame.instance.acData.LastUpdate = default;
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(1);
        PlayerPrefs.DeleteAll();
#if UNITY_EDITOR
        throw new Exception("清除账号完成,请重启游戏！");
#endif
        LoginGameInfoFun();
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        InfoButtonOnClickFun();
    }

    private void Start()
    {
        AccountDataInfoFun();
        LoginGameInfoFun();
    }

    private void InfoButtonOnClickFun()
    {
        accountBtn.GetComponent<Button>().onClick.AddListener(DisplaySignInUi);
        createAccountBtn.onClick.AddListener(CreateAccountFun);
        hadAccountNumBtn.onClick.AddListener(HadAccountOnClickFun);
        backCreateAccountBtn.onClick.AddListener(BackCreateBtnOnClickFun);
        loginAccountBtn.onClick.AddListener(()=>ClientLogin(PlayerDataForGame.instance.acData.Username,PlayerDataForGame.instance.acData.Password));
        closeBtn.onClick.AddListener(CloseLoginInfoObjFun);
    }

    /// <summary> 
    /// 玩家账户信息初始化到游戏 
    /// </summary> 
    private void AccountDataInfoFun()
    {
        PlayerDataForGame.instance.acData.Username = PlayerPrefs.GetString(AccountId);
        PlayerDataForGame.instance.acData.Password = PlayerPrefs.GetString(Password);
        PlayerDataForGame.instance.acData.Phone = PlayerPrefs.GetString(PhoneNumber);
    }

    private void SetAccountInfo()
    {
        PlayerPrefs.SetString(AccountId, PlayerDataForGame.instance.acData.Username);
        PlayerPrefs.SetString(Password, PlayerDataForGame.instance.acData.Password);
        PlayerPrefs.SetString(PhoneNumber, PlayerDataForGame.instance.acData.Phone);
    }

    [SerializeField]
    Button closeBtn;    //关闭按钮 

    [SerializeField]
    Button loginBtn;    //登录按钮 

    public Button beginningWarBtn; //初始战斗按钮

    [SerializeField]
    GameObject accountTextObj;   //账号textObj 

    [SerializeField]
    GameObject phoneNumberObj;  //手机号显示obj 

    [SerializeField]
    Text accountText;   //账号text 

    [SerializeField]
    GameObject registerAccountObj;  //创建账号密码输入窗口 

    //[SerializeField]
    //GameObject loginInfoObj;    //账号登录信息框 

    [SerializeField]
    GameObject accountBtn;  //查看账号按钮obj 

    [SerializeField]
    GameObject bindPhoneBtnObj;  //绑定手机按钮obj 

    public SignInUi signInUi;

    public GameObject busyPanel; //等待网络的挡板

    public SignalRClient signalRClient;

    /// <summary> 
    /// 游戏登陆方法初始化 
    /// </summary> 
    public async void LoginGameInfoFun()
    {
        signInUi.Hide();
        loginBtn.onClick.RemoveAllListeners();
        loginBtn.gameObject.SetActive(false);
        beginningWarBtn.gameObject.SetActive(false);
        //判断本地是否有存档，或者播放过剧情故事 
        //如果有存档或初始剧情已播或是用户名已注册，不播剧情
        if (!string.IsNullOrWhiteSpace(PlayerDataForGame.instance.acData.Username)
            || LoadSaveData.instance.isHadSaveData
#if UNITY_EDITOR
            || isSkipInitBattle
#endif
            || StartSceneUIManager.instance.isPlayedStory)
        {
            busyPanel.SetActive(true);
            LoadSaveData.instance.LoadByJson();
            signInUi.SetValue(PlayerDataForGame.instance.acData.Username,PlayerDataForGame.instance.acData.Password);
#if UNITY_EDITOR
            if (!isSkipLogin)
#endif
            {
                //如果条件允许尝试注册新服务器
                var isSuccess = await BugHotFix.OnFixMigrateServerAccountCreationV1_94(
                    SystemInfo.deviceUniqueIdentifier,
                    PlayerDataForGame.instance.acData.Password);
                if (!isSuccess)
                    PlayerDataForGame.instance.ShowStringTips("网络异常！");
            }
            busyPanel.SetActive(false);
            loginBtn.gameObject.SetActive(true);
        }
        else
            //先播放剧情 
            beginningWarBtn.gameObject.SetActive(true);

        //如果没用户id(暂时为没存档的依据) 
        if (string.IsNullOrWhiteSpace(PlayerDataForGame.instance.acData.Username))
        {
            //登录按钮添加创建账号方法 
            loginBtn.onClick.AddListener(OnCreateAccount);
            accountBtn.SetActive(false);
            return;
        }


        //如果存档存在： 
        //登录按钮添加进入游戏方法 
        loginBtn.onClick.AddListener(()=>ClientLogin(PlayerDataForGame.instance.acData.Username,
            PlayerDataForGame.instance.acData.Password));
        accountBtn.SetActive(true);
        //loginBtn.gameObject.SetActive(true);
    }

    private void ClientLogin(string username, string password)
    {
#if UNITY_EDITOR
        if (isSkipLogin)
        {
            LoadMainScene(true, (int) HttpStatusCode.OK);
            return;
        }
#endif
        if (busyPanel) busyPanel.SetActive(true);
        BugHotFix.OnFixStaminaV2_05();
#if UNITY_EDITOR
        if (isSkipLogin)
        {
            LoadMainScene(true, (int) HttpStatusCode.OK);
            return;
        }
#endif

        //OldLoginTask(LoadMainScene);
        signalRClient.Login(LoadMainScene, username, password);
    }

    private void LoadMainScene(bool isSuccess, int code)
    {
        if (isSuccess)
        {
            PlayerDataForGame.instance.acData.Password = signInUi.PasswordField.text;
            PlayerDataForGame.instance.isNeedSaveData = true;
            LoadSaveData.instance.SaveGameData(1);
            SetAccountInfo();
            StartSceneUIManager.instance.LoadingScene(1, true);
            return;
        }

        var serverResponseError = ServerResponseError(code);
        signInUi.ShowMessage(serverResponseError);
        PlayerDataForGame.instance.ShowStringTips(serverResponseError);
        busyPanel.gameObject.SetActive(false);
    }

    private async void OldLoginTask(UnityAction<bool,int> action)
    {
        //尝试登录并获取登录信息 
        var response =
            await Http.PostAsync(Server.USER_LOGIN_API,
                Json.Serialize(PlayerDataForGame.instance.acData));
        if (!response.IsSuccess())
        {
            action?.Invoke(false, (int)response.StatusCode);
            var code = (ServerBackCode)response.StatusCode;
            if (code == ServerBackCode.ERR_PW_ERROR) //如果密码错误
            {
                PlayerDataForGame.instance.ShowStringTips("密码错误！");
                return;
            }

            PlayerDataForGame.instance.ShowStringTips("服务器响应错误！");
            return;
        }

        var ac = Json.Deserialize<UserInfo>(await response.Content.ReadAsStringAsync());
        if (ac != null)
        {
            BugHotFix.OnFixUploadSaveToServerV1_95();

            PlayerDataForGame.instance.acData.Username = ac.Username;
            PlayerDataForGame.instance.acData.LastUpdate = ac.LastUpdate;
            action?.Invoke(true, (int)response.StatusCode);
            return;
        }
        PlayerDataForGame.instance.ShowStringTips($"请求异常[{(int)response.StatusCode}]，请联系管理人！");
        loginBtn.gameObject.SetActive(true);
        action?.Invoke(true, (int)response.StatusCode);
    }

    /// <summary> 
    /// 点击查看账号按钮方法 
    /// </summary> 
    private void DisplaySignInUi()
    {
        var ac = PlayerDataForGame.instance.acData;
        signInUi.SetMode(SignInUi.Modes.Login, () => ClientLogin(signInUi.UsernameField.text, signInUi.PasswordField.text));
        signInUi.SetValue(ac.Username,ac.Password);
    }

    /// <summary> 
    /// 初次创建账号打开账号窗口方法 
    /// </summary> 
    private async void OnCreateAccount()
    {
        loginBtn.interactable = false;
        busyPanel.SetActive(true);
        var ac = await Http.PostAsync<UserInfo>(Server.INSTANCE_ID_API,
            Json.Serialize(new UserInfo { DeviceId = SystemInfo.deviceUniqueIdentifier }));
        busyPanel.SetActive(false);
        if (ac == null)
        {
            PlayerDataForGame.instance.ShowStringTips("服务器连接错误！");
            loginBtn.interactable = true;
            return;
        }

        //accountText.text = ac.Username;
        //accountTextObj.SetActive(true);
        //passwordInput.text = "";
        //passwordInput1.text = "";
        loginBtn.interactable = true;
        registerAccountObj.SetActive(true);
        signInUi.SetMode(SignInUi.Modes.SignUp, CreateAccountFun);
        signInUi.SetValue(ac.Username, string.Empty);
    }

    [SerializeField]
    Button createAccountBtn;    //创建账号按钮 

    [SerializeField]
    GameObject chackAccountObj;  //查看账户修改密码obj 

    /// <summary> 
    /// 创建账号，确认密码 
    /// </summary> 
    private async void CreateAccountFun()
    {
        if(!signInUi.IsReadyAction())return;
        //提交账号密码，申请注册账号 
        busyPanel.SetActive(true);
        var ac = await Http.PostAsync<UserInfo>(Server.PLAYER_REG_ACCOUNT_API,
            JsonConvert.SerializeObject(new UserInfo
            {
                Username = signInUi.UsernameField.text,
                Password = signInUi.PasswordField.text,
                DeviceId = SystemInfo.deviceUniqueIdentifier
            }));
        busyPanel.SetActive(false);
        if (ac == null)
        {
            Debug.Log("服务器响应错误");
            //createAccountBtn.interactable = true;
            signInUi.ShowMessage("服务器响应错误,请检查网络状态。");
            return;
        }

        //给游戏中存放账户名和密码 
        PlayerDataForGame.instance.acData.Username = ac.Username;
        PlayerDataForGame.instance.acData.Password = signInUi.PasswordField.text;
        PlayerDataForGame.instance.acData.LastUpdate = ac.LastUpdate;
        PlayerPrefs.SetString(AccountId, PlayerDataForGame.instance.acData.Username);
        PlayerPrefs.SetString(Password, PlayerDataForGame.instance.acData.Password);
        //registerAccountObj.SetActive(false);
        //passwordInput.text = string.Empty;
        //passwordInput1.text = string.Empty;
        PlayerDataForGame.instance.ShowStringTips("注册成功！");
        signInUi.ShowMessage("注册成功！");
        LoadSaveData.instance.CreatePlayerDataSave();//初始化玩家存档数据
        accountBtn.SetActive(true);
        signInUi.SetMode(SignInUi.Modes.Disable);
        //phoneNumberObj.SetActive(true);
        //chackAccountObj.SetActive(true);
        StartSceneUIManager.instance.EndStoryToChooseForce();
    }

    private string ServerResponseError(int code)
    {
        var message = string.Empty;
        switch ((ServerBackCode)code)
        {
            case ServerBackCode.SUCCESS:
                message = "SUCCESS";
                break;
            case ServerBackCode.ERR_NAME_EXIST:
                message = "ERR_NAME_EXIST";
                break;
            case ServerBackCode.ERR_NAME_SHORT:
                message = "ERR_NAME_SHORT";
                break;
            case ServerBackCode.ERR_PASS_SHORT:
                message = "密码长度过短";
                break;
            case ServerBackCode.ERR_NAME_NOT_EXIST:
                message = "账号不存在";
                break;
            case ServerBackCode.ERR_DATA_NOT_EXIST:
                message = "ERR_DATA_NOT_EXIST";
                break;
            case ServerBackCode.ERR_PHONE_SHORT:
                message = "ERR_PHONE_SHORT";
                break;
            case ServerBackCode.ERR_ACCOUNT_BIND_OTHER_PHONE:
                message = "重复绑定手机";
                break;
            case ServerBackCode.ERR_NAME_ILLEGAL:
                message = "ERR_NAME_ILLEGAL";
                break;
            case ServerBackCode.ERR_PHONE_ILLEGAL:
                message = "手机号错误";
                break;
            case ServerBackCode.ERR_PW_ERROR:
                message = "密码错误";
                break;
            case ServerBackCode.ERR_PHONE_BIND_OTHER_ACCOUNT:
                message = "该手机号绑定了其他账号";
                break;
            case ServerBackCode.ERR_PHONE_ALREADY_BINDED:
                message = "已经绑定过了";
                break;
            case ServerBackCode.ERR_SERVERSTATE_ZERO:
                message = "服务器正维护中";
                break;
            case ServerBackCode.ERR_INVALIDOPERATION:
            default:
                message = "服务器响应错误";
                break;
        }
        return message;
    }

    private void CloseLoginInfoObjFun()
    {
        registerAccountObj.SetActive(false);
        chackAccountObj.SetActive(false);
        changeAccountObj.SetActive(false);
        signInUi.Hide();
        LoginGameInfoFun();
    }

    ////////////////登陆其他账号相关////////////////////////////////// 

    [SerializeField]
    Button hadAccountNumBtn;    //已有帐号点击按钮 

    [SerializeField]
    Button backCreateAccountBtn;    //返回创建账号界面按钮 

    [SerializeField]
    GameObject changeAccountObj;    //切换账号登录界面 

    /// <summary> 
    /// 点击已有账号按钮 
    /// </summary> 
    private void HadAccountOnClickFun()
    {
        accountInput.text = "";
        pwInput.text = "";
        accountTextObj.SetActive(false);
        registerAccountObj.SetActive(false);
        changeAccountObj.SetActive(true);
    }

    /// <summary> 
    /// 返回创建账号界面 
    /// </summary> 
    private void BackCreateBtnOnClickFun()
    {
        changeAccountObj.SetActive(false);
        accountTextObj.SetActive(true);
        registerAccountObj.SetActive(true);
    }

    [SerializeField]
    InputField accountInput;    //账号输入框 

    [SerializeField]
    InputField pwInput;         //密码输入框 

    [SerializeField]
    Button loginAccountBtn; //切换账号里面的登录按钮 
}