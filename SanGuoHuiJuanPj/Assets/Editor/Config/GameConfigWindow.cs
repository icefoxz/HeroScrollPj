using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace Assets.Editor.Config
{
    public class GameConfigWindow : EditorWindow
    {
        public ConfigAsset configAsset;
        [MenuItem("GameConfig/Window")]
        static void Init()
        {
            var window = GetWindow<GameConfigWindow>();
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("请选入要替换的ConfigAsset");
            var obj = EditorGUILayout.ObjectField("ConfigAsset", configAsset, typeof(ConfigAsset), true);
            configAsset = obj as ConfigAsset;
            if (GUILayout.Button("储存")) Save();
        }

        void Save()
        {
            var serverConfig = new ServerFields
            {
                ServerUrl = configAsset.ServerUrl,
                INSTANCE_ID_API = configAsset.INSTANCE_ID_API,
                PLAYER_UPLOAD_COUNT_API = configAsset.PLAYER_UPLOAD_COUNT_API,
                PLAYER_REG_ACCOUNT_API = configAsset.PLAYER_REG_ACCOUNT_API,
                PLAYER_SAVE_DATA_UPLOAD_API = configAsset.PLAYER_SAVE_DATA_UPLOAD_API,
                USER_LOGIN_API = configAsset.USER_LOGIN_API
            };
            var encrypt = EncryptDecipherTool.DESEncrypt(Json.Serialize(serverConfig));
            System.IO.File.WriteAllText("assets/resources/game/pz.bytes", encrypt);
            XDebug.Log<GameConfigWindow>("配置文件已加密存档！");
        }
    }
}
