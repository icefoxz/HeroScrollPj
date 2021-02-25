using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class BugHotFix
{
    private const string V1_90YvQueSet = "hotfix1.90";
    private const string V1_95UpLoadDataServer = "hotfix1.95Upload";
    private const string V1_95MigrateServer = "hotfix1.95Migration";
    /// <summary>
    /// 标记上一个修复的版本
    /// </summary>
    /// <param name="version"></param>
    private static void SavePlayerDataWithFixVersion(float version,int saveIndex)
    {
        if (PlayerDataForGame.instance.pyData.LastGameVersion >= version) return;
        PlayerDataForGame.instance.pyData.LastGameVersion = version;
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(saveIndex);
    }

    public static void OnFixYvQueV1_90()
    {
        const float fixVersion = 1.9f;
        if (PlayerDataForGame.instance.pyData.LastGameVersion >= fixVersion) return;
        if (PlayerPrefs.GetInt(V1_90YvQueSet, 0) > 0) return;
        //修正玩家可以无限刷霸业宝箱3的bug，如果玩家刷玉阙到一定的值，将修改玩家玉阙数据
        if (PlayerDataForGame.instance.pyData.YvQue >= PlayerDataForGame.instance.Bug1_9YvQueCheck)
            PlayerDataForGame.instance.pyData.YvQue = PlayerDataForGame.instance.Bug1_9YvQueSet;
        PlayerPrefs.SetInt(V1_90YvQueSet, 1);
        SavePlayerDataWithFixVersion(fixVersion,1);
    }

    //todo 当服务器完成重要数据交互的时候，这里才会把自动上传改成第一次创建账号时。
    public static async void OnFixUploadSaveToServerV1_95()
    {
        //const float fixVersion = 1.95f;
        //if (PlayerDataForGame.instance.pyData.LastGameVersion > fixVersion) return;
        // 给玩家上传存档，如果玩家存档次数=0，将尝试上传。如果已经存档了便不会尝试上传存档。
        //var response = await Http.PostAsync(Server.PLAYER_UPLOAD_COUNT_API,
        //    Json.Serialize(PlayerDataForGame.instance.acData));
        //if (!response.IsSuccess()) return;
        //var countString = await response.Content.ReadAsStringAsync();
#if UNITY_EDITOR
        ////if (!int.TryParse(countString, out int count)) return;
#else
        //if (count > 1)
        //{
        //    PlayerPrefs.SetInt(V1_95UpLoadDataServer, 1);
        //    SavePlayerDataWithFixVersion(fixVersion);
        //    return;
        //}
#endif
        var pData = PlayerDataForGame.instance;
        var saveArchive = Json.Serialize(new UserSaveArchive(pData.acData, pData.pyData, pData.hstData, pData.warsData,
            pData.gbocData));
        var result = await Http.PostAsync(Server.PLAYER_SAVE_DATA_UPLOAD_API,saveArchive);
        if (!result.IsSuccess()) return;
        //PlayerPrefs.SetInt(V1_95UpLoadDataServer, 0);
        //SavePlayerDataWithFixVersion(fixVersion);
    }

    public static async Task<bool> OnFixMigrateServerAccountCreationV1_94(string deviceId, string password)
    {
        const float fixVersion = 1.94f;
        if (PlayerDataForGame.instance.pyData.LastGameVersion > fixVersion) return true;

        var loginResponse = await Http.PostAsync(Server.USER_LOGIN_API,
            Json.Serialize(new UserInfo
                {DeviceId = deviceId, Username = PlayerDataForGame.instance.acData.Username, Password = password}));
        if (loginResponse.IsSuccess()) return true;
        //申请一个账号id
        var userInfo = await Http.PostAsync<UserInfo>(Server.INSTANCE_ID_API, Json.Serialize(new UserInfo
        {
            DeviceId = deviceId
        }));
        if (userInfo == null) return false;
        var ac = PlayerDataForGame.instance.acData;
        ac.Username = userInfo.Username;
        ac.DeviceId = userInfo.DeviceId;
        PlayerPrefs.SetString(StartSceneToServerCS.AccountId, ac.Username);
        //注册玩家账号
        userInfo = await Http.PostAsync<UserInfo>(Server.PLAYER_REG_ACCOUNT_API, Json.Serialize(new UserInfo
        {
            DeviceId = deviceId,
            Username = ac.Username,
            Password = password
        }));
        if (userInfo == null) return false;
        ac.Username = userInfo.Username;
        ac.LastUpdate = userInfo.LastUpdate;
        PlayerPrefs.SetInt(V1_95MigrateServer, 1);
        SavePlayerDataWithFixVersion(float.Parse(Application.version),1);
        return true;
    }

    public static void OnFixZhanLingV1_99(BaYeDataClass baYe, Dictionary<int, int> zhanLingMap)
    {
        const float fixVersion = 1.99f;
        if (baYe.zhanLingMap.Count == 0 && PlayerDataForGame.instance.pyData.LastGameVersion < fixVersion)
        {
            baYe.zhanLingMap = zhanLingMap;
            PlayerDataForGame.instance.isNeedSaveData = true;
            SavePlayerDataWithFixVersion(fixVersion, 5);
        }
    }

    public static void OnFixLianYuV2_02()
    {
        const float fixVersion = 2.02f;
        if (PlayerDataForGame.instance.pyData.LastGameVersion >= fixVersion) return;
        ResetWar(80);
        ResetWar(81);
        ResetWar(82);
        void ResetWar(int warId)
        {
            var war = PlayerDataForGame.instance.warsData.warUnlockSaveData.Single(w => w.warId == warId);
            var index = PlayerDataForGame.instance.warsData.warUnlockSaveData.IndexOf(war);
            PlayerDataForGame.instance.warsData.warUnlockSaveData[index] = new UnlockWarCount {warId = warId};
            SavePlayerDataWithFixVersion(fixVersion,1);
        }
    }
}