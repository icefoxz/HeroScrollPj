using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Beebyte.Obfuscator;
using UnityEngine;

public static class Server
{
#if !UNITY_EDITOR
    private static  string ServerUrl { get; set; } 
    public  static string PLAYER_SAVE_DATA_UPLOAD_API { get; private set; }
    public  static string INSTANCE_ID_API { get; private set; }
    public  static string PLAYER_REG_ACCOUNT_API { get; private set; }
    public  static string PLAYER_UPLOAD_COUNT_API { get; private set; }
    public  static string USER_LOGIN_API { get; private set; }
    public static string SIGNALR_LOGIN_API { get; private set; }
#else
    //private static string ServerUrl { get; set; } = "https://heroscrollpjtestserver0.azurewebsites.net/api/";
    private static string ServerUrl { get; set; } = "http://localhost:7071/api/";
    public static string PLAYER_SAVE_DATA_UPLOAD_API { get; private set; } = "UploadSaveData";
    public static string INSTANCE_ID_API { get; private set; } = "GenerateUserId";
    public static string PLAYER_REG_ACCOUNT_API { get; private set; } = "RegUser";
    public static string PLAYER_UPLOAD_COUNT_API { get; private set; } = "UploadCount";
    public static string USER_LOGIN_API { get; private set; } = "Login";
    public static string SIGNALR_LOGIN_API { get; private set; } = "SignalRLogin";
#endif

    private static bool isInitialized;
    public static string PHONE_BINDING_API;
    public static HttpClient InstanceClient() => new HttpClient() {BaseAddress = new Uri(ServerUrl)};
    public static void Initialize(ServerFields fields)
    {
        if (isInitialized) return;
        isInitialized = true;
#if !UNITY_EDITOR
        ServerUrl = fields.ServerUrl;
        PLAYER_SAVE_DATA_UPLOAD_API = fields.PLAYER_SAVE_DATA_UPLOAD_API;
        INSTANCE_ID_API = fields.INSTANCE_ID_API;
        PLAYER_REG_ACCOUNT_API = fields.PLAYER_REG_ACCOUNT_API;
        PLAYER_UPLOAD_COUNT_API = fields.PLAYER_UPLOAD_COUNT_API;
        USER_LOGIN_API = fields.USER_LOGIN_API;
        SIGNALR_LOGIN_API = fields.SIGNALR_LOGIN_API;
#endif
    }
}