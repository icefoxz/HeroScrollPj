using System.Collections.Generic;
using UnityEngine.UI;

public class WarMiniWindowUI : MiniWindowUI
{
    public Text expeditionText;

    public override void Show(Dictionary<int, int> rewardMap)
    {
        base.Show(rewardMap);
        expeditionText.gameObject.SetActive(PlayerDataForGame.instance.WarType ==
                                            PlayerDataForGame.WarTypes.Expedition);
    }
}