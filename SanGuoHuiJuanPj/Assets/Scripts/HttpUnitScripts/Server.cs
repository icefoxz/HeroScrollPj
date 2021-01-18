using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Server
{
    private const string ServerUrl = "https://heroscrollpjtestserver0.azurewebsites.net/api/";
    public const string PLAYER_SAVE_DATA_UPLOAD_API = "UploadSaveData";
    public const string INSTANCE_ID_API = "GenerateUserId";
    public const string PLAYER_REG_ACCOUNT_API = "RegUser";
    public const string PLAYER_UPLOAD_COUNT_API = "UploadCount";
    public const string PHONE_BINDING_API = "";// => Get();
    public const string USER_LOGIN_API = "Login";
    public static HttpClient InstanceClient() => new HttpClient() {BaseAddress = new Uri(ServerUrl)};
}