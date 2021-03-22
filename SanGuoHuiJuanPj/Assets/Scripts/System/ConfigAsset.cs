﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    //class ConfigEditor
    //{
    //    [MenuItem("Configure/SaveConfigs")]
    //    static void OnSaveConfig(ConfigAsset configAsset)
    //    {
    //        var serverConfig = new ServerFields
    //        {
    //            ServerUrl = configAsset.ServerUrl,
    //            INSTANCE_ID_API = configAsset.INSTANCE_ID_API,
    //            PLAYER_UPLOAD_COUNT_API = configAsset.PLAYER_UPLOAD_COUNT_API,
    //            PLAYER_REG_ACCOUNT_API = configAsset.PLAYER_REG_ACCOUNT_API,
    //            PLAYER_SAVE_DATA_UPLOAD_API = configAsset.PLAYER_SAVE_DATA_UPLOAD_API,
    //            USER_LOGIN_API = configAsset.USER_LOGIN_API
    //        };
    //        var encrypt = EncryptDecipherTool.DESEncrypt(Json.Serialize(serverConfig));
    //        gameConfig = new TextAsset(encrypt);
    //        XDebug.Log<Configuration>("配置文件已存档！");
    //    }
    //}
    [Serializable]
    [CreateAssetMenu(fileName = "config", menuName = "Configure/SpawnConfigAsset", order = 1)]
    public class ConfigAsset : ScriptableObject
    {
        public string ServerUrl;
        public string PLAYER_SAVE_DATA_UPLOAD_API;
        public string INSTANCE_ID_API;
        public string PLAYER_REG_ACCOUNT_API;
        public string PLAYER_UPLOAD_COUNT_API;
        public string USER_LOGIN_API;
        public string SIGNALR_LOGIN_API;
    }
}
