using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Editor;
using UnityEngine;
using UnityEditor;

public class Configuration : MonoBehaviour
{
    public TextAsset gameConfig;

    void Awake()
    {
        var json = EncryptDecipherTool.DESDecrypt(gameConfig.text);
        var serverFields = Json.Deserialize<ServerFields>(json);
        Server.Initialize(serverFields);
    }
}

public class ServerFields
{
    public string ServerUrl { get; set; }
    public string PLAYER_SAVE_DATA_UPLOAD_API { get; set; }
    public string INSTANCE_ID_API { get; set; }
    public string PLAYER_REG_ACCOUNT_API { get; set; }
    public string PLAYER_UPLOAD_COUNT_API { get; set; }
    public string USER_LOGIN_API { get; set; }
    public string SIGNALR_LOGIN_API { get; set; }
}