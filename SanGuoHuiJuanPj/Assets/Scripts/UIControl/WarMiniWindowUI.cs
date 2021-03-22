﻿using System.Collections.Generic;
using UnityEngine.UI;

public class WarMiniWindowUI : MiniWindowUI
{
    public Text expeditionText;
    public ForceFlagUI flagPrefab;//战令id为负数

    public override void Show(Dictionary<int, int> rewardMap)
    {
        flagPrefab.gameObject.SetActive(false);
        base.Show(rewardMap);
        rewardMap.TryGetValue(2, out int chestAmt);//宝箱
        expeditionText.gameObject.SetActive(PlayerDataForGame.instance.WarType ==
            PlayerDataForGame.WarTypes.Expedition && chestAmt > 0);//宝箱大于0才会显示去桃园打开宝箱的提示。并且是战役才有
    }

    public void ShowWithZhanLing(Dictionary<int, int> reward, Dictionary<int, int> zhanLing)
    {
        base.Show(reward);
        foreach (var ling in zhanLing)
        {
            var flag = Instantiate(flagPrefab, listView);
            flag.Set((ForceFlags)ling.Key);
            flag.SetLing(ling.Value);
        }
        reward.TryGetValue(2, out int chestAmt);//宝箱
        expeditionText.gameObject.SetActive(PlayerDataForGame.instance.WarType ==
            PlayerDataForGame.WarTypes.Expedition && chestAmt > 0);//宝箱大于0才会显示去桃园打开宝箱的提示。并且是战役才有
    }
}