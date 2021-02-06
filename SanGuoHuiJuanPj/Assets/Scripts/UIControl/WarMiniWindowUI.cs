using System.Collections.Generic;
using UnityEngine.UI;

public class WarMiniWindowUI : MiniWindowUI
{
    public Text expeditionText;

    public override void Show(Dictionary<int, int> rewardMap)
    {
        base.Show(rewardMap);
        rewardMap.TryGetValue(2, out int chestAmt);//宝箱
        expeditionText.gameObject.SetActive(PlayerDataForGame.instance.WarType ==
            PlayerDataForGame.WarTypes.Expedition && chestAmt > 0);//宝箱大于0才会显示去桃园打开宝箱的提示。并且是战役才有
    }
}