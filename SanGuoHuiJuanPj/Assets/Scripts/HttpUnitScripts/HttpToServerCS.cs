using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using wode.HTTP;

public class HttpToServerCS : MonoBehaviour
{
#if UNITY_EDITOR
    private static readonly string CreateQuickAccountName = "http://127.0.0.1:8080/login/createQuickAccountName";
    private static readonly string CreateAccount = "http://127.0.0.1:8080/login/createAccount?name={0}&pw={1}&isPhone=0";
    private static readonly string BindPhone = "http://127.0.0.1:8080/login/bindPhone?name={0}&pw={1}&phone={2}";
    private static readonly string AccountNameOrPhoneLogin = "http://127.0.0.1:8080/login/nameLogin?name={0}&pw={1}&isPhone={2}";
    private static readonly string Post_UploadArchive = "http://127.0.0.1:8080/login/upload";
#elif UNITY_ANDROID && !UNITY_EDITOR
    private static readonly string CreateQuickAccountName = "http://39.105.62.202:8080/login/createQuickAccountName";
    private static readonly string CreateAccount = "http://39.105.62.202:8080/login/createAccount?name={0}&pw={1}&isPhone=0";
    private static readonly string BindPhone = "http://39.105.62.202:8080/login/bindPhone?name={0}&pw={1}&phone={2}";
    private static readonly string AccountNameOrPhoneLogin = "http://39.105.62.202:8080/login/nameLogin?name={0}&pw={1}&isPhone={2}";
    private static readonly string Post_UploadArchive = "http://39.105.62.202:8080/login/upload";
#endif

    //private static readonly string Post_UploadArchive_Data = "{name:\\\"{0}\\\",pw:\\\"{1}\\\",isPhone:\\\"0\\\",data:\\\"{2}\\\",data2:\\\"{3}\\\",data3:\\\"{4}\\\",data4:\\\"{5}\\\"}";
    private static readonly string[] Post_UploadArchive_KeyName = new string[7] {
        "name",
        "pw",
        "isPhone",
        "data",
        "data2",
        "data3",
        "data4"
    };

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
            case LoginFunIndex.ACCOUNT_LOGIN:
                getUrlStr = AccountNameOrPhoneLogin;
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
        //Debug.Log("getUrlStr: " + getUrlStr);
        string replyStr = HttpUitls.Get(getUrlStr);
        //Debug.Log("reply: " + replyStr);

        return replyStr;
    }

    /// <summary>
    /// 登录相关方法交互服务器Post方式
    /// </summary>
    /// <param name="loginFunIndex">方法索引</param>
    /// <param name="contentStrs">上传内容</param>
    /// <returns></returns>
    public string LoginRelatedFunsForPost(LoginFunIndex loginFunIndex, object[] contentStrs)
    {
        string postUrlStr = string.Empty;

        switch (loginFunIndex)
        {
            case LoginFunIndex.UPLOAD_ARCHIVE:
                postUrlStr = Post_UploadArchive;
                break;
            default:
                break;
        }

        Dictionary<string, object> dic = new Dictionary<string, object>();
        for (int i = 0; i < contentStrs.Length; i++)
        {
            dic.Add(Post_UploadArchive_KeyName[i], contentStrs[i]);
        }

        //Debug.Log("postUrlStr: " + postUrlStr);
        string replyStr = HttpUitls.PostForValue(postUrlStr, dic);
        //Debug.Log("reply: " + replyStr);

        return replyStr;
    }

    /// <summary>
    /// 解析服务器错误码
    /// </summary>
    /// <param name="serverBackStr"></param>
    /// <param name="errorCode"></param>
    /// <returns></returns>
    public string ErrorAnalysisFun(string serverBackStr, int errorCode = -1)
    {
        ErrorBackClass errorBackClass = new ErrorBackClass();
        if (serverBackStr != null)
        {
            try
            {
                errorBackClass = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorBackClass>(serverBackStr);
            }
            catch (System.Exception)
            {
                Debug.Log(serverBackStr);
                return "服务器响应错误";
            }
        }
        
        string backRemindStr = string.Empty;
        ServerBackCode serverBackCode;

        if (errorCode != -1)
        {
            serverBackCode = (ServerBackCode)errorCode;
        }
        else
        {
            serverBackCode = (ServerBackCode)errorBackClass.error;
        }

        switch (serverBackCode)
        {
            case ServerBackCode.SUCCESS:
                backRemindStr = "SUCCESS";
                break;
            case ServerBackCode.ERR_NAME_EXIST:
                backRemindStr = "ERR_NAME_EXIST";
                break;
            case ServerBackCode.ERR_NAME_SHORT:
                backRemindStr = "ERR_NAME_SHORT";
                break;
            case ServerBackCode.ERR_PASS_SHORT:
                backRemindStr = "密码长度过短";
                break;
            case ServerBackCode.ERR_NAME_NOT_EXIST:
                backRemindStr = "账号不存在";
                break;
            case ServerBackCode.ERR_DATA_NOT_EXIST:
                backRemindStr = "ERR_DATA_NOT_EXIST";
                break;
            case ServerBackCode.ERR_PHONE_SHORT:
                backRemindStr = "ERR_PHONE_SHORT";
                break;
            case ServerBackCode.ERR_ACCOUNT_BIND_OTHER_PHONE:
                backRemindStr = "重复绑定手机";
                break;
            case ServerBackCode.ERR_NAME_ILLEGAL:
                backRemindStr = "ERR_NAME_ILLEGAL";
                break;
            case ServerBackCode.ERR_PHONE_ILLEGAL:
                backRemindStr = "手机号错误";
                break;
            case ServerBackCode.ERR_PW_ERROR:
                backRemindStr = "密码错误";
                break;
            case ServerBackCode.ERR_PHONE_BIND_OTHER_ACCOUNT:
                backRemindStr = "该手机号绑定了其他账号";
                break;
            case ServerBackCode.ERR_PHONE_ALREADY_BINDED:
                backRemindStr = "已经绑定过了";
                break;
            default:
                backRemindStr = "服务器响应错误";
                break;
        }
        return backRemindStr;
    }
}