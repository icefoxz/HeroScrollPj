using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using wode.HTTP;

/// <summary>
/// 登录相关方法索引
/// </summary>
public enum LoginFunIndex  
{
    /// <summary>
    /// 0 申请一个账号
    /// </summary>
    CREATE_ACCOUNT_NAME,
    /// <summary>
    /// 1 创建账号
    /// </summary>
    CREATE_ACCOUNT,
    /// <summary>
    /// 2 绑定手机
    /// </summary>
    BIND_PHONE,
    /// <summary>
    /// 3 手机号登陆
    /// </summary>
    PHONE_NAME_LOGIN,
    /// <summary>
    /// 4 游戏账号登陆
    /// </summary>
    ACCOUNT_NAME_LOGIN,
    /// <summary>
    /// 5 上传存档
    /// </summary>
    UPLOAD_ARCHIVE
}

public class HttpToServerCS : MonoBehaviour
{
#if UNITY_EDITOR

    private static readonly string CreateQuickAccountName = "http://127.0.0.1:8080/login/createQuickAccountName";
    private static readonly string CreateAccount = "http://127.0.0.1:8080/login/createAccount?name={0}&pw={1}&isPhone=0";
    private static readonly string BindPhone = "http://127.0.0.1:8080/login/bindPhone?name={0}&pw={1}&phone={2}";
    private static readonly string PhoneNameLogin = "http://127.0.0.1:8080/login/nameLogin?name={0}&pw={1}&isPhone=1";
    private static readonly string AccountNameLogin = "http://127.0.0.1:8080/login/nameLogin?name={0}&pw={1}&isPhone=0";
    //private static readonly string Get_UploadArchive = "http://127.0.0.1:8080/login/upload?name={0}&pw={1}&isPhone=0&data={2}&data2={3}&data3={4}&data4={5}";
    private static readonly string Post_UploadArchive = "http://127.0.0.1:8080/login/upload";
    //private static readonly string Post_UploadArchive_Data = "{name:\\\"{0}\\\",pw:\\\"{1}\\\",isPhone:\\\"0\\\",data:\\\"{2}\\\",data2:\\\"{3}\\\",data3:\\\"{4}\\\",data4:\\\"{5}\\\"}";
    private static readonly string Post_UploadArchive_Data = "name={0}&pw={1}&isPhone=0&data={2}&data2={3}&data3={4}&data4={5}";

#elif UNITY_ANDROID && !UNITY_EDITOR

#endif

    public static HttpToServerCS instance;

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
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 登录相关方法交互服务器Get方式
    /// </summary>
    /// <param name="loginFunIndex">方法索引</param>
    /// <param name="contentStrs">上传内容</param>
    /// <returns></returns>
    public string LoginRelatedFunsForGet(LoginFunIndex loginFunIndex, string[] contentStrs)
    {
        string getUrlStr = string.Empty;

        switch (loginFunIndex)
        {
            case LoginFunIndex.CREATE_ACCOUNT_NAME:
                getUrlStr = CreateQuickAccountName;
                break;
            case LoginFunIndex.CREATE_ACCOUNT:
                getUrlStr = CreateAccount;
                break;
            case LoginFunIndex.BIND_PHONE:
                getUrlStr = BindPhone;
                break;
            case LoginFunIndex.PHONE_NAME_LOGIN:
                getUrlStr = PhoneNameLogin;
                break;
            case LoginFunIndex.ACCOUNT_NAME_LOGIN:
                getUrlStr = AccountNameLogin;
                break;
            default:
                break;
        }

        try
        {
            if (contentStrs != null)
            {
                getUrlStr = string.Format(getUrlStr, contentStrs);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
            return StringForEditor.ERROR;
        }
        Debug.Log("getUrlStr: " + getUrlStr);
        string replyStr = HttpUitls.Get(getUrlStr);
        Debug.Log("reply: " + replyStr);

        return replyStr;
    }

    /// <summary>
    /// 登录相关方法交互服务器Post方式
    /// </summary>
    /// <param name="loginFunIndex">方法索引</param>
    /// <param name="contentStrs">上传内容</param>
    /// <returns></returns>
    public string LoginRelatedFunsForPost(LoginFunIndex loginFunIndex, string[] contentStrs)
    {
        string postUrlStr = string.Empty;
        string postDataStr = string.Empty;

        switch (loginFunIndex)
        {
            case LoginFunIndex.UPLOAD_ARCHIVE:
                postUrlStr = Post_UploadArchive;
                postDataStr = Post_UploadArchive_Data;
                break;
            default:
                break;
        }

        try
        {
            if (contentStrs != null)
            {
                postDataStr = string.Format(postDataStr, contentStrs);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
            return StringForEditor.ERROR;
        }

        Debug.Log("postUrlStr: " + postUrlStr);
        Debug.Log("postDataStr: " + postDataStr);
        string replyStr = HttpUitls.Post(postUrlStr, postDataStr);
        Debug.Log("reply: " + replyStr);

        return replyStr;
    }
}