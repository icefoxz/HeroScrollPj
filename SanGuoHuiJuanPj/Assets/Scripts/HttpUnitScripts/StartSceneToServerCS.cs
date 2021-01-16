using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneToServerCS : MonoBehaviour
{
    private const string AccountId = "accountName";
    private const string Password = "Password";
    private const string PhoneNumber = "Phone";

    public static StartSceneToServerCS instance;

    /// <summary> 
    /// 清除帐户 
    /// </summary> 
    public void ClearAccountData()
    {
        PlayerDataForGame.instance.acData.Username = "";
        PlayerDataForGame.instance.acData.LastUpdate = default;
        PlayerPrefs.SetString(AccountId, "");
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
        accountBtn.GetComponent<Button>().onClick.AddListener(TakeChackAccountBtnFun);
        createAccountBtn.onClick.AddListener(CreateAccountFun);
        hadAccountNumBtn.onClick.AddListener(HadAccountOnClickFun);
        backCreateAccountBtn.onClick.AddListener(BackCreateBtnOnClickFun);
        loginAccountBtn.onClick.AddListener(UserLogin);
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

    [SerializeField]
    Button closeBtn;    //关闭按钮 

    [SerializeField]
    Button loginBtn;    //登录按钮 

    [SerializeField]
    GameObject accountTextObj;   //账号textObj 

    [SerializeField]
    GameObject phoneNumberObj;  //手机号显示obj 

    [SerializeField]
    Text accountText;   //账号text 

    [SerializeField]
    GameObject registerAccountObj;  //创建账号密码输入窗口 

    [SerializeField]
    GameObject loginInfoObj;    //账号登录信息框 

    [SerializeField]
    GameObject accountBtn;  //查看账号按钮obj 

    [SerializeField]
    GameObject bindPhoneBtnObj;  //绑定手机按钮obj 

    /// <summary> 
    /// 游戏登陆方法初始化 
    /// </summary> 
    public async void LoginGameInfoFun()
    {
        loginBtn.onClick.RemoveAllListeners();
        loginBtn.gameObject.SetActive(false);
        //判断本地是否有存档，或者播放过剧情故事 
        //如果没存档并还未播放剧情 
        if (!LoadSaveData.instance.isHadSaveData && !StartSceneUIManager.instance.isPlayedStory)
        {
            //先播放剧情 
            StartSceneUIManager.instance.DontHaveSaveDataPlayStory(loginBtn);
            return;
        }
        //如果没用户id(暂时为没存档的依据) 
        if (string.IsNullOrWhiteSpace(PlayerDataForGame.instance.acData.Username))
        {
            //登录按钮添加创建账号方法 
            loginBtn.onClick.AddListener(OnCreateAccount);
            accountBtn.SetActive(false);
            loginBtn.gameObject.SetActive(true);
            return;
        }

        //如果存档存在： 
        //尝试登录并获取登录信息 
#if !DEBUG
         
        var response = 
            await Http.PostAsync<BackAccountClass>(Server.UserLoginApi, 
                Json.Serialize(PlayerDataForGame.instance.acData)); 
        //如果服务器获取信息 
        if (response != null) 
        { 
            var code = (ServerBackCode) response.error; 
            if (code == ServerBackCode.ERR_NAME_NOT_EXIST) //如果id并不存在服务器 
            { 
                //从服务器获取id并存档。 
                var ac = await Http.PostAsync<BackAccountClass>(Server.InstanceIdApi, 
                    JsonConvert.SerializeObject(new 
                    { 
                        password = PlayerDataForGame.instance.acData.Password 
                    })); 
                //如果服务器成功便存档，获取失败直接略过等下次重启客户端再尝试。 
                if (ac != null) 
                { 
                    PlayerDataForGame.instance.acData.Username = ac.Username; 
                    PlayerDataForGame.instance.acData.LastUpdate = ac.LastUpdate; 
                    PlayerDataForGame.instance.isNeedSaveData = true; 
                    LoadSaveData.instance.SaveGameData(1); 
                    loginBtn.gameObject.SetActive(true); 
                } 
            } 
 
            //登录按钮添加进入游戏方法 
            loginBtn.onClick.AddListener(() => StartSceneUIManager.instance.LoadingScene(1, true)); 
            accountBtn.SetActive(true); 
            loginBtn.gameObject.SetActive(true); 
            return; 
        } 
 
        PlayerDataForGame.instance.ShowStringTips(" 服务器响应错误！"); 
        loginBtn.gameObject.SetActive(true); 
#else
        //登录按钮添加进入游戏方法 
        loginBtn.onClick.AddListener(() => StartSceneUIManager.instance.LoadingScene(1, true));
        accountBtn.SetActive(true);
        loginBtn.gameObject.SetActive(true);
        return;
#endif
    }

    /// <summary> 
    /// 点击查看账号按钮方法 
    /// </summary> 
    private void TakeChackAccountBtnFun()
    {
        accountText.text = PlayerDataForGame.instance.acData.Username;
        accountTextObj.SetActive(true);
        if (PlayerDataForGame.instance.acData.Phone != "")
        {
            phoneNumberObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = PlayerDataForGame.instance.acData.Phone;
            bindPhoneBtnObj.SetActive(false);
        }
        else
        {
            bindPhoneBtnObj.SetActive(true);
        }
        phoneNumberObj.SetActive(true);
        chackAccountObj.SetActive(true);
        loginInfoObj.SetActive(true);
    }

    /// <summary> 
    /// 初次创建账号打开账号窗口方法 
    /// </summary> 
    private async void OnCreateAccount()
    {
        loginBtn.interactable = false;
        var ac = await Http.GetAsync<UserInfo>(Server.InstanceIdApi);
        if (ac == null)
        {
            PlayerDataForGame.instance.ShowStringTips("服务器连接错误！");
            loginBtn.interactable = true;
            return;
        }

        loginBtn.interactable = true;
        accountText.text = ac.Username;
        accountTextObj.SetActive(true);
        passwordInput.text = "";
        passwordInput1.text = "";
        registerAccountObj.SetActive(true);
        loginInfoObj.SetActive(true);
    }

    [SerializeField]
    Button createAccountBtn;    //创建账号按钮 

    [SerializeField]
    InputField passwordInput;   //密码输入框 
    [SerializeField]
    InputField passwordInput1;  //密码输入框1 

    [SerializeField]
    GameObject chackAccountObj;  //查看账户修改密码obj 

    /// <summary> 
    /// 创建账号，确认密码 
    /// </summary> 
    private async void CreateAccountFun()
    {
        if (string.IsNullOrWhiteSpace(passwordInput.text))
        {
            PlayerDataForGame.instance.ShowStringTips("请输入密码");
            return;
        }

        if (passwordInput1.text != passwordInput.text)
        {
            passwordInput1.text = string.Empty;
            PlayerDataForGame.instance.ShowStringTips("请确认密码");
            return;
        }

        createAccountBtn.interactable = false;
        var arrStr = new[] { accountText.text, passwordInput.text };
        //提交账号密码，申请注册账号 
        var ac = await Http.PostAsync<UserInfo>(Server.RegAccountApi,
            JsonConvert.SerializeObject(new { username = accountText.text, passwordInput.text }));
        if (ac == null)
        {
            Debug.Log("服务器响应错误");
            PlayerDataForGame.instance.ShowStringTips("服务器响应错误");
            createAccountBtn.interactable = true;
            return;
        }

        //给游戏中存放账户名和密码 
        PlayerDataForGame.instance.acData.Username = ac.Username;
        PlayerDataForGame.instance.acData.Password = passwordInput.text;
        PlayerDataForGame.instance.acData.LastUpdate = ac.LastUpdate;
        PlayerPrefs.SetString(AccountId, PlayerDataForGame.instance.acData.Username);
        PlayerPrefs.SetString(Password, PlayerDataForGame.instance.acData.Password);
        registerAccountObj.SetActive(false);
        passwordInput.text = string.Empty;
        passwordInput1.text = string.Empty;
        PlayerDataForGame.instance.ShowStringTips("注册成功");

        phoneNumberObj.SetActive(true);
        chackAccountObj.SetActive(true);
    }

    /// <summary> 
    /// 短信验证成功，返回数据 
    /// </summary> 
    /// <param name="responseString">回调信息</param> 
    public async void SMSVerifiedSuccessedFun(string responseString)
    {
        SMSBackContentClass sMSBackContentClass = new SMSBackContentClass();
        try
        {
            //提取回调参数中的手机号信息 
            sMSBackContentClass = JsonConvert.DeserializeObject<SMSBackContentClass>(responseString);
        }
        catch (Exception e)
        {
            Debug.Log("responseString: " + responseString);
            Debug.LogError(e.ToString());
            PlayerDataForGame.instance.ShowStringTips("短信验证失败");
            return;
        }

        //提交账号密码手机号，申请绑定手机 
        var replyStr = await Http.PostAsync(Server.PhoneBindingApi, JsonConvert.SerializeObject(new
        {
            username = PlayerDataForGame.instance.acData.Username,
            password = PlayerDataForGame.instance.acData.Password,
            sMSBackContentClass.phone
        }));

        if (replyStr == HttpResponse.ERROR)
        {
            Debug.Log("服务器响应错误");
            PlayerDataForGame.instance.ShowStringTips("服务器响应错误");
            return;
        }

        BackPhoneToAccountClass backPhoneToAccountClass = new BackPhoneToAccountClass();
        try
        {
            backPhoneToAccountClass = Json.Deserialize<BackPhoneToAccountClass>(replyStr);
            if (backPhoneToAccountClass.error != (int)ServerBackCode.SUCCESS)
            {
                string serverBackStr = ServerResponseError(backPhoneToAccountClass.error);
                PlayerDataForGame.instance.ShowStringTips(serverBackStr);
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            var errResponse = Json.Deserialize<ErrorBackClass>(replyStr);
            string serverBackStr = errResponse == null ? "服务器响应错误!" : ServerResponseError(errResponse.error);
            PlayerDataForGame.instance.ShowStringTips(serverBackStr);
            return;
        }

        //设置手机号存储到游戏中 
        phoneNumberObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = backPhoneToAccountClass.phone;
        PlayerDataForGame.instance.acData.Phone = backPhoneToAccountClass.phone;
        PlayerPrefs.SetString(PhoneNumber, backPhoneToAccountClass.phone);

        bindPhoneBtnObj.SetActive(false);
        PlayerDataForGame.instance.ShowStringTips("绑定手机成功");
    }

    private string ServerResponseError(int code)
    {
        string errorText;
        switch ((ServerBackCode)code)
        {
            case ServerBackCode.SUCCESS:
                errorText = "SUCCESS";
                break;
            case ServerBackCode.ERR_NAME_EXIST:
                errorText = "ERR_NAME_EXIST";
                break;
            case ServerBackCode.ERR_NAME_SHORT:
                errorText = "ERR_NAME_SHORT";
                break;
            case ServerBackCode.ERR_PASS_SHORT:
                errorText = "密码长度过短";
                break;
            case ServerBackCode.ERR_NAME_NOT_EXIST:
                errorText = "账号不存在";
                break;
            case ServerBackCode.ERR_DATA_NOT_EXIST:
                errorText = "ERR_DATA_NOT_EXIST";
                break;
            case ServerBackCode.ERR_PHONE_SHORT:
                errorText = "ERR_PHONE_SHORT";
                break;
            case ServerBackCode.ERR_ACCOUNT_BIND_OTHER_PHONE:
                errorText = "重复绑定手机";
                break;
            case ServerBackCode.ERR_NAME_ILLEGAL:
                errorText = "ERR_NAME_ILLEGAL";
                break;
            case ServerBackCode.ERR_PHONE_ILLEGAL:
                errorText = "手机号错误";
                break;
            case ServerBackCode.ERR_PW_ERROR:
                errorText = "密码错误";
                break;
            case ServerBackCode.ERR_PHONE_BIND_OTHER_ACCOUNT:
                errorText = "该手机号绑定了其他账号";
                break;
            case ServerBackCode.ERR_PHONE_ALREADY_BINDED:
                errorText = "已经绑定过了";
                break;
            default:
                errorText = "服务器响应错误";
                break;
        }
        return errorText;
    }

    private void CloseLoginInfoObjFun()
    {
        registerAccountObj.SetActive(false);
        chackAccountObj.SetActive(false);
        changeAccountObj.SetActive(false);
        loginInfoObj.SetActive(false);
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

    /// <summary> 
    /// 账号登陆 
    /// </summary> 
    private async void UserLogin()
    {
        loginAccountBtn.gameObject.SetActive(false);
        if (string.IsNullOrWhiteSpace(accountInput.text))
        {
            PlayerDataForGame.instance.ShowStringTips("请输入账号");
            loginAccountBtn.gameObject.SetActive(true);
            return;
        }

        if (string.IsNullOrWhiteSpace(pwInput.text))
        {
            PlayerDataForGame.instance.ShowStringTips("请输入密码");
            loginAccountBtn.gameObject.SetActive(true);
            return;
        }

        int isPhone = 0;
        //判断是否是手机号登录 
        if (accountInput.text.Length == 11)
        {
            isPhone = 1;
        }

        var ac = await Http.PostAsync<BackAccountClass>(Server.UserLoginApi, Json.Serialize(new
        {
            username = accountInput.text,
            password = pwInput.text
        }));

        if (ac == null)
        {
            Debug.Log("服务器响应错误");
            PlayerDataForGame.instance.ShowStringTips("服务器响应错误");
            return;
        }

        if (ac.error != (int)ServerBackCode.SUCCESS)
        {
            PlayerDataForGame.instance.ShowStringTips(ServerResponseError(ac.error));
            return;
        }

        //拉取服务器的存档文件 
        var save0 = await Http.PostAsync<PlayerData>(Server.GetSave0Api, Json.Serialize(PlayerDataForGame.instance.acData));
        var save1 = await Http.PostAsync<HSTDataClass>(Server.GetSave1Api, Json.Serialize(PlayerDataForGame.instance.acData));
        var save2 = await Http.PostAsync<WarsDataClass>(Server.GetSave2Api, Json.Serialize(PlayerDataForGame.instance.acData));
        var save3 = await Http.PostAsync<GetBoxOrCodeData>(Server.GetSave3Api, Json.Serialize(PlayerDataForGame.instance.acData));

        if (save0 == null || save1 == null || save2 == null || save3 == null)
        {
            PlayerDataForGame.instance.ShowStringTips("服务器存档解析失败");
            return;
        }

        LoadSaveData.instance.SetGamePlayerBasicData(save0, save1, save2, save3);
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData();
        LoadSaveData.instance.isHadSaveData = true;

        accountText.text = ac.username;
        accountTextObj.SetActive(true);
        phoneNumberObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = ac.phone;
        phoneNumberObj.SetActive(true);

        PlayerDataForGame.instance.acData.Username = ac.username;
        PlayerDataForGame.instance.acData.Password = pwInput.text;
        PlayerDataForGame.instance.acData.Phone = ac.phone;

        PlayerPrefs.SetString(AccountId, PlayerDataForGame.instance.acData.Username);
        PlayerPrefs.SetString(Password, PlayerDataForGame.instance.acData.Password);
        PlayerPrefs.SetString(PhoneNumber, PlayerDataForGame.instance.acData.Phone);

        bindPhoneBtnObj.SetActive(false);
        chackAccountObj.SetActive(true);

        changeAccountObj.SetActive(false);
        pwInput.text = "";
        PlayerDataForGame.instance.ShowStringTips("登录账号成功");
    }
}