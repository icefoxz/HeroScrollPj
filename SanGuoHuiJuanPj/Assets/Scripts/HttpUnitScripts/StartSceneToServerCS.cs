using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneToServerCS : MonoBehaviour
{
    private readonly string accountNamePrefsStr = "accountName";
    private readonly string passwordStrPrefsStr = "passwordStr";
    private readonly string phoneNumberPrefsStr = "phoneNumber";

    public static StartSceneToServerCS instance;

    /// <summary>
    /// 清除帐户
    /// </summary>
    public void ClearAccountData()
    {
        PlayerDataForGame.instance.atData.accountName = "";
        PlayerPrefs.SetString(accountNamePrefsStr, "");
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
        loginAccountBtn.onClick.AddListener(TakeLoginAccountBtnFun);
        closeBtn.onClick.AddListener(CloseLoginInfoObjFun);
    }

    /// <summary>
    /// 玩家账户信息初始化到游戏
    /// </summary>
    private void AccountDataInfoFun()
    {
        PlayerDataForGame.instance.atData.accountName = PlayerPrefs.GetString(accountNamePrefsStr);
        PlayerDataForGame.instance.atData.passwordStr = PlayerPrefs.GetString(passwordStrPrefsStr);
        PlayerDataForGame.instance.atData.phoneNumber = PlayerPrefs.GetString(phoneNumberPrefsStr);
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
    public void LoginGameInfoFun()
    {
        loginBtn.onClick.RemoveAllListeners();
        //判断本地是否有存档，或者播放过剧情故事
        if (LoadSaveData.instance.isHadSaveData || StartSceneUIManager.instance.isPlayedStory)
        {
            if (PlayerDataForGame.instance.atData.accountName == "")
            {
                //登录按钮添加创建账号方法
                loginBtn.onClick.AddListener(LoginGameCreateAccount);
                accountBtn.SetActive(false);
            }
            else
            {
                //登录按钮添加进入游戏方法
                loginBtn.onClick.AddListener(delegate () { StartSceneUIManager.instance.LoadingScene(1, true); });
                accountBtn.SetActive(true);
            }
        }
        else
        {
            StartSceneUIManager.instance.DontHaveSaveDataPlayStory(loginBtn);
        }
    }

    /// <summary>
    /// 点击查看账号按钮方法
    /// </summary>
    private void TakeChackAccountBtnFun()
    {
        accountText.text = PlayerDataForGame.instance.atData.accountName;
        accountTextObj.SetActive(true);
        if (PlayerDataForGame.instance.atData.phoneNumber != "")
        {
            phoneNumberObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = PlayerDataForGame.instance.atData.phoneNumber;
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
    private void LoginGameCreateAccount()
    {
        //申请一个账号
        string replyStr = HttpToServerCS.instance.LoginRelatedFunsForGet(LoginFunIndex.CREATE_ACCOUNT_NAME, null);
        if (replyStr != StringForEditor.ERROR)
        {
            BackAccountClass backAccountClass = new BackAccountClass();
            try
            {
                backAccountClass = JsonConvert.DeserializeObject<BackAccountClass>(replyStr);

                if (backAccountClass.error != (int)ServerBackCode.SUCCESS)
                {
                    string serverBackStr = HttpToServerCS.instance.ErrorAnalysisFun(null, backAccountClass.error);
                    PlayerDataForGame.instance.ShowStringTips(serverBackStr);
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                string serverBackStr = HttpToServerCS.instance.ErrorAnalysisFun(replyStr);
                PlayerDataForGame.instance.ShowStringTips(serverBackStr);
                return;
            }
            accountText.text = backAccountClass.name;
            accountTextObj.SetActive(true);
            passwordInput.text = "";
            passwordInput1.text = "";
            registerAccountObj.SetActive(true);
            loginInfoObj.SetActive(true);
        }
        else
        {
            Debug.Log("服务器响应错误");
            PlayerDataForGame.instance.ShowStringTips("服务器响应错误");
        }
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
    private void CreateAccountFun()
    {
        if (passwordInput.text == "")
        {
            PlayerDataForGame.instance.ShowStringTips("请输入密码");
        }
        else
        {
            if (passwordInput1.text != passwordInput.text)
            {
                passwordInput1.text = "";
                PlayerDataForGame.instance.ShowStringTips("请确认密码");
            }
            else
            {
                string[] arrStr = new string[2] { accountText.text, passwordInput.text };
                //提交账号密码，申请注册账号
                string replyStr = HttpToServerCS.instance.LoginRelatedFunsForGet(LoginFunIndex.CREATE_ACCOUNT, arrStr);
                if (replyStr != StringForEditor.ERROR)
                {
                    BackAccountClass backAccountClass = new BackAccountClass();
                    try
                    {
                        backAccountClass = JsonConvert.DeserializeObject<BackAccountClass>(replyStr);

                        if (backAccountClass.error != (int)ServerBackCode.SUCCESS)
                        {
                            string serverBackStr = HttpToServerCS.instance.ErrorAnalysisFun(null, backAccountClass.error);
                            PlayerDataForGame.instance.ShowStringTips(serverBackStr);
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                        string serverBackStr = HttpToServerCS.instance.ErrorAnalysisFun(replyStr);
                        PlayerDataForGame.instance.ShowStringTips(serverBackStr);
                        return;
                    }

                    //给游戏中存放账户名和密码
                    PlayerDataForGame.instance.atData.accountName = backAccountClass.name;
                    PlayerDataForGame.instance.atData.passwordStr = passwordInput.text;
                    PlayerPrefs.SetString(accountNamePrefsStr, PlayerDataForGame.instance.atData.accountName);
                    PlayerPrefs.SetString(passwordStrPrefsStr, PlayerDataForGame.instance.atData.passwordStr);
                    registerAccountObj.SetActive(false);
                    passwordInput.text = "";
                    passwordInput1.text = "";
                    PlayerDataForGame.instance.ShowStringTips("注册成功");

                    phoneNumberObj.SetActive(true);
                    chackAccountObj.SetActive(true);
                }
                else
                {
                    Debug.Log("服务器响应错误");
                    PlayerDataForGame.instance.ShowStringTips("服务器响应错误");
                }
            }
        }
    }

    /// <summary>
    /// 短信验证成功，返回数据
    /// </summary>
    /// <param name="responseString">回调信息</param>
    public void SMSVerifiedSuccessedFun(string responseString)
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

        string[] arrStr = new string[3] { PlayerDataForGame.instance.atData.accountName, PlayerDataForGame.instance.atData.passwordStr, sMSBackContentClass.phone };
        //提交账号密码手机号，申请绑定手机
        string replyStr = HttpToServerCS.instance.LoginRelatedFunsForGet(LoginFunIndex.BIND_PHONE, arrStr);
        if (replyStr != StringForEditor.ERROR)
        {
            BackPhoneToAccountClass backPhoneToAccountClass = new BackPhoneToAccountClass();
            try
            {
                backPhoneToAccountClass = JsonConvert.DeserializeObject<BackPhoneToAccountClass>(replyStr);
                if (backPhoneToAccountClass.error != (int)ServerBackCode.SUCCESS)
                {
                    string serverBackStr = HttpToServerCS.instance.ErrorAnalysisFun(null, backPhoneToAccountClass.error);
                    PlayerDataForGame.instance.ShowStringTips(serverBackStr);
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                string serverBackStr = HttpToServerCS.instance.ErrorAnalysisFun(replyStr);
                PlayerDataForGame.instance.ShowStringTips(serverBackStr);
                return;
            }
            //设置手机号存储到游戏中
            phoneNumberObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = backPhoneToAccountClass.phone;
            PlayerDataForGame.instance.atData.phoneNumber = backPhoneToAccountClass.phone;
            PlayerPrefs.SetString(phoneNumberPrefsStr, backPhoneToAccountClass.phone);

            bindPhoneBtnObj.SetActive(false);
            PlayerDataForGame.instance.ShowStringTips("绑定手机成功");
        }
        else
        {
            Debug.Log("服务器响应错误");
            PlayerDataForGame.instance.ShowStringTips("服务器响应错误");
        }
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
    private void TakeLoginAccountBtnFun()
    {
        if (accountInput.text == "")
        {
            PlayerDataForGame.instance.ShowStringTips("请输入账号");
        }
        else
        {
            if (pwInput.text == "")
            {
                PlayerDataForGame.instance.ShowStringTips("请输入密码");
            }
            else
            {
                int isPhone = 0;
                //判断是否是手机号登录
                if (accountInput.text.Length == 11)
                {
                    isPhone = 1;
                }
                string[] arrStr = new string[3] { accountInput.text, pwInput.text, isPhone.ToString() };

                string replyStr = HttpToServerCS.instance.LoginRelatedFunsForGet(LoginFunIndex.ACCOUNT_LOGIN, arrStr);
                if (replyStr != StringForEditor.ERROR)
                {
                    BackForLoginClass backForLoginClass = new BackForLoginClass();
                    try
                    {
                        backForLoginClass = JsonConvert.DeserializeObject<BackForLoginClass>(replyStr);
                        if (backForLoginClass.error != (int)ServerBackCode.SUCCESS)
                        {
                            string serverBackStr = HttpToServerCS.instance.ErrorAnalysisFun(null, backForLoginClass.error);
                            PlayerDataForGame.instance.ShowStringTips(serverBackStr);
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                        string serverBackStr = HttpToServerCS.instance.ErrorAnalysisFun(replyStr);
                        PlayerDataForGame.instance.ShowStringTips(serverBackStr);
                        return;
                    }

                    //设置账号数据存储到游戏中
                    PlyDataClass save0 = new PlyDataClass();
                    HSTDataClass save1 = new HSTDataClass();
                    WarsDataClass save2 = new WarsDataClass();
                    GetBoxOrCodeData save3 = new GetBoxOrCodeData();
                    Debug.Log("下拉数据：");
                    Debug.Log(backForLoginClass.data);
                    Debug.Log(backForLoginClass.data2);
                    Debug.Log(backForLoginClass.data3);
                    Debug.Log(backForLoginClass.data4);
                    //拉取服务器的存档文件
                    try
                    {
                        save0 = JsonConvert.DeserializeObject<PlyDataClass>(backForLoginClass.data);
                        save1 = JsonConvert.DeserializeObject<HSTDataClass>(backForLoginClass.data2);
                        save2 = JsonConvert.DeserializeObject<WarsDataClass>(backForLoginClass.data3);
                        save3 = JsonConvert.DeserializeObject<GetBoxOrCodeData>(backForLoginClass.data4);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("服务器存档解析失败：" + e.ToString());
                        PlayerDataForGame.instance.ShowStringTips("服务器存档解析失败");
                        return;
                    }
                    LoadSaveData.instance.SetGamePlayerBasicData(save0, save1, save2, save3);
                    PlayerDataForGame.instance.isNeedSaveData = true;
                    LoadSaveData.instance.SaveGameData();
                    LoadSaveData.instance.isHadSaveData = true;

                    accountText.text = backForLoginClass.name;
                    accountTextObj.SetActive(true);
                    phoneNumberObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = backForLoginClass.phone;
                    phoneNumberObj.SetActive(true);

                    PlayerDataForGame.instance.atData.accountName = backForLoginClass.name;
                    PlayerDataForGame.instance.atData.passwordStr = pwInput.text;
                    PlayerDataForGame.instance.atData.phoneNumber = backForLoginClass.phone;

                    PlayerPrefs.SetString(accountNamePrefsStr, PlayerDataForGame.instance.atData.accountName);
                    PlayerPrefs.SetString(passwordStrPrefsStr, PlayerDataForGame.instance.atData.passwordStr);
                    PlayerPrefs.SetString(phoneNumberPrefsStr, PlayerDataForGame.instance.atData.phoneNumber);

                    bindPhoneBtnObj.SetActive(false);
                    chackAccountObj.SetActive(true);

                    changeAccountObj.SetActive(false);
                    pwInput.text = "";
                    PlayerDataForGame.instance.ShowStringTips("登录账号成功");
                }
                else
                {
                    Debug.Log("服务器响应错误");
                    PlayerDataForGame.instance.ShowStringTips("服务器响应错误");
                }
            }
        }
    }
}