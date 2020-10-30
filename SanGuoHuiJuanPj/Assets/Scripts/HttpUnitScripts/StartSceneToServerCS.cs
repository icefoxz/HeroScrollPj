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

        //PlayerPrefs.SetString(accountNamePrefsStr, "");
    }

    private void Start()
    {
        AccountDataInfoFun();
        LoginGameInfoFun();
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
    Button loginBtn;    //登录按钮

    [SerializeField]
    Text accountText;   //账号text

    [SerializeField]
    GameObject loginInfoObj;    //账号登录信息框

    [SerializeField]
    GameObject registerAccountObj;  //密码输入窗口

    [SerializeField]
    GameObject accountBtn;  //账号按钮obj

    /// <summary>
    /// 游戏登陆方法初始化
    /// </summary>
    public void LoginGameInfoFun()
    {
        loginBtn.onClick.RemoveAllListeners();
        //判断本地是否有存档
        if (LoadSaveData.instance.isHadSaveData || StartSceneUIManager.instance.isPlayedStory)
        {
            if (PlayerDataForGame.instance.atData.accountName == "")
            {
                //登录按钮添加创建账号方法
                loginBtn.onClick.AddListener(LoginGameCreateAccount);
                createAccountBtn.onClick.RemoveAllListeners();
                createAccountBtn.onClick.AddListener(CreateAccountFun);
                accountBtn.SetActive(false);
            }
            else
            {
                //登录按钮添加进入游戏方法
                loginBtn.onClick.AddListener(delegate () { StartSceneUIManager.instance.LoadingScene(1, true); });
                accountBtn.GetComponent<Button>().onClick.RemoveAllListeners();
                accountBtn.GetComponent<Button>().onClick.AddListener(TakeChackAccountBtnFun);
                accountBtn.SetActive(true);
            }
            if (PlayerDataForGame.instance.atData.phoneNumber == "")
            {
                jumpBindBtn.onClick.RemoveAllListeners();
                jumpBindBtn.onClick.AddListener(JumpBindPhoneFun);
            }
        }
        else
        {
            StartSceneUIManager.instance.DontHaveSaveDataPlayStory(loginBtn);
        }
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
            }
            catch (Exception e)
            {
                Debug.Log(replyStr);
                Debug.LogError(e.ToString());
                StartSceneUIManager.instance.ShowStringTips("服务器响应错误");
                return;
            }
            accountText.text = backAccountClass.name;
            loginInfoObj.SetActive(true);
            registerAccountObj.SetActive(true);
        }
        else
        {
            Debug.Log("服务器响应错误");
            StartSceneUIManager.instance.ShowStringTips("服务器响应错误");
        }
    }

    [SerializeField]
    Button createAccountBtn;    //创建账号按钮

    [SerializeField]
    InputField passwordInput;   //密码输入框
    [SerializeField]
    InputField passwordInput1;  //密码输入框1

    [SerializeField]
    GameObject phoneNumberObj;  //手机号显示obj

    [SerializeField]
    GameObject bindPhoneObj;  //绑定手机obj

    [SerializeField]
    Button jumpBindBtn;  //跳过绑定手机Btn

    [SerializeField]
    GameObject chackAccountObj;  //查看账户修改密码obj

    /// <summary>
    /// 创建账号，确认密码
    /// </summary>
    private void CreateAccountFun()
    {
        if (passwordInput.text == "")
        {
            StartSceneUIManager.instance.ShowStringTips("请输入密码");
        }
        else
        {
            if (passwordInput1.text != passwordInput.text)
            {
                passwordInput1.text = "";
                StartSceneUIManager.instance.ShowStringTips("请确认密码");
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
                    }
                    catch (Exception e)
                    {
                        Debug.Log(replyStr);
                        Debug.LogError(e.ToString());
                        StartSceneUIManager.instance.ShowStringTips("服务器响应错误");
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
                    StartSceneUIManager.instance.ShowStringTips("注册成功");
                    phoneNumberObj.SetActive(true);
                    bindPhoneObj.SetActive(true);
                }
                else
                {
                    Debug.Log("服务器响应错误");
                    StartSceneUIManager.instance.ShowStringTips("服务器响应错误");
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
            StartSceneUIManager.instance.ShowStringTips("短信验证失败");
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
            }
            catch (Exception e)
            {
                Debug.Log(replyStr);
                Debug.LogError(e.ToString());
                StartSceneUIManager.instance.ShowStringTips("服务器响应错误");
                return;
            }
            //设置手机号存储到游戏中
            phoneNumberObj.transform.GetChild(0).GetComponent<Text>().text = backPhoneToAccountClass.phone;
            PlayerDataForGame.instance.atData.phoneNumber = backPhoneToAccountClass.phone;
            PlayerPrefs.SetString(phoneNumberPrefsStr, backPhoneToAccountClass.phone);
            bindPhoneObj.SetActive(false);
            chackAccountObj.SetActive(true);
            StartSceneUIManager.instance.ShowStringTips("绑定手机成功");
        }
        else
        {
            Debug.Log("服务器响应错误");
            StartSceneUIManager.instance.ShowStringTips("服务器响应错误");
        }

    }

    /// <summary>
    /// 跳过绑定手机界面
    /// </summary>
    private void JumpBindPhoneFun()
    {
        loginInfoObj.SetActive(false);
        LoginGameInfoFun();
    }

    /// <summary>
    /// 点击查看账号按钮方法
    /// </summary>
    private void TakeChackAccountBtnFun()
    {
        accountText.text = PlayerDataForGame.instance.atData.accountName;
        if (PlayerDataForGame.instance.atData.phoneNumber != "")
        {
            phoneNumberObj.transform.GetChild(0).GetComponent<Text>().text = PlayerDataForGame.instance.atData.phoneNumber;
            chackAccountObj.SetActive(true);
        }
        else
        {
            bindPhoneObj.SetActive(true);
        }
        phoneNumberObj.SetActive(true);
        loginInfoObj.SetActive(true);
    }
}