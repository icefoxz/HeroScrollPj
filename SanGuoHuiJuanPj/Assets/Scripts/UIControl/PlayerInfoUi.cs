using System.Linq;
using CorrelateLib;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUi : MonoBehaviour
{
    public Text CharacterName;
    public Text CharacterNickname;
    public Image Avatar;
    public Text Level;
    public Text YuanBao;
    public Text YuQue;
    public ExpSlider Exp;
    public ZhanLingUi ZhanLingUi;

    public void UpdateCharacter(ICharacter cha)
    {
        CharacterName.text = cha.Name;
        CharacterNickname.text = cha.Nickname;
        Avatar.sprite = GameResources.Instance.Avatar[cha.Avatar];
    }
    public void UpdateUi()
    {
        var isLevelMax = PlayerDataForGame.instance.pyData.Level >= DataTable.PlayerLevelConfig.Keys.Max();
        var maxExp = isLevelMax ? 99999 : DataTable.PlayerLevelConfig[PlayerDataForGame.instance.pyData.Level + 1].Exp;
        var levelText = isLevelMax
            ? DataTable.GetStringText(34)
            : string.Format(DataTable.GetStringText(35), PlayerDataForGame.instance.pyData.Level);
        Level.text = levelText;
        YuanBao.text = PlayerDataForGame.instance.pyData.YuanBao.ToString();
        YuQue.text = PlayerDataForGame.instance.pyData.YvQue.ToString();
        Exp.Set(PlayerDataForGame.instance.pyData.Exp, maxExp);
        var cha = PlayerDataForGame.instance.Character;
        if (cha != null) UpdateCharacter(cha);
        //货币 
        UpdateZhanLing();
    }
    public void UpdateZhanLing(int effectValue = default)
    {
        if (effectValue != default) ZhanLingUi.ShowEffect(effectValue);
        ZhanLingUi.UpdateUi(PlayerDataForGame.instance.Stamina.Value);
    }

    public void UpdateZhanLingCountdown(string countText)
    {
        ZhanLingUi.UpdateCountdown(countText);
        UpdateZhanLing();
    }
}