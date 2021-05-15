using System.IO;
using UnityEngine;

public class PlayerDataMock : PlayerDataForGame
{
    public override void Init()
    {
        base.Init();
        var isPlayerDataExist = File.Exists(AppDebugClass.plyDataString);
        var isCardDataExists = File.Exists(AppDebugClass.hstDataString);
        var isWarProgressDataExists = File.Exists(AppDebugClass.warUnlockDataString);
        var isRewardDataExists = File.Exists(AppDebugClass.gbocDataString);

        if (!isPlayerDataExist || !isCardDataExists || !isWarProgressDataExists || !isRewardDataExists)
            Debug.LogError($"找不到玩家存档，请下载玩家存档后再用重启游戏！");
        LoadSaveData.instance.LoadByJson();
    }
}