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
    public Text MilitaryPower;
    public ExpSlider Exp;
    public ZhanLingUi ZhanLingUi;

    public void UpdateCharacter(ICharacter cha)
    {
        CharacterName.text = cha.Name;
        CharacterNickname.text = cha.Nickname;
        Avatar.sprite = GameResources.Instance.Avatar[cha.Gender];
    }
    public void UpdateUi()
    {
        var py = PlayerDataForGame.instance.pyData;
        var isLevelMax = py.Level >= DataTable.PlayerLevelConfig.Keys.Max();
        var maxExp = isLevelMax ? 99999 : DataTable.PlayerLevelConfig[py.Level + 1].Exp;
        var levelText = isLevelMax
            ? DataTable.GetStringText(34)
            : string.Format(DataTable.GetStringText(35), py.Level);
        Level.text = levelText;
        YuanBao.text = py.YuanBao.ToString();
        YuQue.text = py.YvQue.ToString();
        MilitaryPower.text = PlayerDataForGame.instance.MilitaryPower.ToString();
        Exp.Set(py.Exp, maxExp);
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