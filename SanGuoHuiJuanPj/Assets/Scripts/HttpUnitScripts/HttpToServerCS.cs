using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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