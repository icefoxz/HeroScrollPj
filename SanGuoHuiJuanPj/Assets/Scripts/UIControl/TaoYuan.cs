using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Utl;
using Beebyte.Obfuscator;
using CorrelateLib;
using UnityEngine;
using UnityEngine.UI;

[Skip]public class TaoYuan : MonoBehaviour
{
    public int openJiuTanYBNums;   //开酒坛所需元宝
    public Button freeJiuTanAdButton;//开酒坛的免费按键
    private bool isConsumeAd;//是否消费了广告
    public JiuTanUI jiuTan;//酒坛
    public Button jinNangBtn;//锦囊
    public TaoYuanChestUI zhanYiChest;//战役宝箱
    public YvQueChestUI copperChest;//铜宝箱
    public YvQueChestUI goldChest;//金宝箱
    public JinNangUI jinNangUi;//锦囊
    private Dictionary<TaoYuanChestUI, int> chestCostMap;//宝箱和价钱的映射表

    private void Start() => InitChests();

    //初始化宝箱的展示
    private void InitChests()
    {
        chestCostMap = new Dictionary<TaoYuanChestUI, int>
        {
            {jiuTan, DataTable.GetGameValue(3)},
            {zhanYiChest, 0},//战役宝箱不花费
            {copperChest, DataTable.GetGameValue(4)},
            {goldChest, DataTable.GetGameValue(5)}
        };
        freeJiuTanAdButton.onClick.AddListener(OnWatchAdGetJiuTan);
        foreach (var map in chestCostMap)
        {
            var chest = map.Key;
            var cost = map.Value;
            chest.chestButton.onClick.AddListener(() => OpenChest(chest));
            if (chest != zhanYiChest)
            {
                chest.value.text = cost.ToString();
                continue;
            }
            //如果是战役宝箱
            var zyChestCount = PlayerDataForGame.instance.gbocData.fightBoxs.Count; //战役宝箱数量
            chest.value.text = chest.value.text = zyChestCount.ToString();
            chest.chestButton.enabled = zyChestCount > 0;
            continue;
        }

        jinNangBtn.onClick.AddListener(RequestJinNang);
    }

    private void OnJinNangFailed(string arg)
    {
        UIManager.instance.PlayOnClickMusic();
        PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(44));
    }

    private void RequestJinNang()
    {
        if (TimeSystemControl.instance.OnClickToGetJinNang())
        {
            ApiPanel.instance.Invoke(OnOpenJinNang, OnJinNangFailed, EventStrings.Req_JinNang);
            return;
        }
        OnJinNangFailed(string.Empty);
    }

    public void OnOpenJinNang(ViewBag viewBag)
    {
        var jinNang = viewBag.GetJinNang();
        var doubleToken = viewBag.Values[0].ToString();
        var playerDto = viewBag.GetPlayerDataDto();
        //var list = DataTable.Tips.Values.ToList();
        //var randId = UnityEngine.Random.Range(0, list.Count);
        //var tips = list[randId];
        //var yuanBao = UnityEngine.Random.Range(tips.YuanBaoReward.Min, tips.YuanBaoReward.ExcMax);
        var textColor = jinNang.Color == 1 ? ColorDataStatic.name_deepRed : ColorDataStatic.name_brown;
        jinNangUi.OnReward(jinNang.Text, textColor, jinNang.Sign, jinNang.Stamina, jinNang.YuanBao, doubleToken,
            playerDto);
    }

    public void UpdateJiuTan(bool isReady,string jiuTanCount, string countDown)
    {
        jiuTan.chestButton.gameObject.SetActive(isReady);
        jiuTan.chestButton.enabled = isReady;
        freeJiuTanAdButton.gameObject.SetActive(isReady);
        freeJiuTanAdButton.enabled = isReady;
        jiuTan.displayText.gameObject.SetActive(!isReady);//倒数文本
        jiuTan.lasting.gameObject.SetActive(!isReady);//倒数文本
        jiuTan.UpdateUi(jiuTanCount, countDown);
    }

    private bool isWatchingJiuTanAd;

    /// <summary>
    /// 观看视频免费开启酒坛
    /// </summary>
    public void OnWatchAdGetJiuTan()
    {
        if (isWatchingJiuTanAd) return;
        isWatchingJiuTanAd = true;
        //获得酒坛消耗元宝按钮
        freeJiuTanAdButton.enabled = false;
        jiuTan.chestButton.enabled = false;

        AdAgent.instance.BusyRetry(() =>
        {
            int index = openJiuTanYBNums;
            openJiuTanYBNums = 0;
            isConsumeAd = true; //已成功消费广告
            OpenJiuTan();
            openJiuTanYBNums = index;
            PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(5));
            freeJiuTanAdButton.enabled = true;
            jiuTan.chestButton.enabled = true;
            isWatchingJiuTanAd = false;
        }, () =>
        {
            PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(6));
            freeJiuTanAdButton.enabled = true;
            jiuTan.chestButton.enabled = true;
            isConsumeAd = false;
            isWatchingJiuTanAd = false;
        });
    }

    /// <summary>
    /// 打开桃园宝箱
    /// </summary>
    /// <param name="chestUi"></param>
    public void OpenChest(TaoYuanChestUI chestUi)
    {
        AudioController0.instance.ChangeAudioClip(13);

        var chestId = -1; //宝箱在表里的Id
        var isConsume = false;//是否消费
        if (chestUi == jiuTan) //酒坛
        {
            chestId = 0;
            //如果玩家元宝小于预设的酒坛花费或开酒坛失败(时间还未到)
            if (PlayerDataForGame.instance.pyData.YuanBao < openJiuTanYBNums ||
                !TimeSystemControl.instance.OnClickToGetJiuTan())
            {
                PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(0));
                AudioController0.instance.PlayAudioSource(0);
                return;
            }

            PlayerDataForGame.instance.Redemption(PlayerDataForGame.RedeemTypes.JiuTan); //标记已消费酒坛

            if (!isConsumeAd)
            {
                ConsumeManager.instance.DeductYuanBao(openJiuTanYBNums); //扣除酒坛元宝
                isConsume = true;//消费元宝
            }
            else
            {
                //如果已消费广告就不扣除元宝
                isConsumeAd = false;
            }
        }

        if (chestUi == copperChest)
        {
            chestId = 1;
            if (!TimeSystemControl.instance.OnClickToGetFreeBox1())
                if (ConsumeManager.instance.DeductYuQue(chestCostMap[chestUi])) //如果时间开启不了，尝试扣除玉阙开启
                    isConsume = true; //消费玉阙
                else return;//如果无法消费，取消请求
        }

        if (chestUi == goldChest)
        {
            chestId = 2;
            if (!TimeSystemControl.instance.OnClickToGetFreeBox2())
                if (ConsumeManager.instance.DeductYuQue(chestCostMap[chestUi])) isConsume = true; //消费玉阙
                else return; //如果无法消费，取消请求
            UIManager.instance.ShowOrHideGuideObj(0, false);
        }

        if (chestUi == zhanYiChest) //战役宝箱
        {
            if (PlayerDataForGame.instance.gbocData.fightBoxs.Count <= 0) //如果没有宝箱记录
            {
                PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(7));
                AudioController0.instance.PlayAudioSource(0);
                chestUi.chestButton.enabled = false;
                return;
            }

            chestId = PlayerDataForGame.instance.gbocData.fightBoxs[0]; //获取叠在最上面的奖励id
            PlayerDataForGame.instance.gbocData.fightBoxs.Remove(chestId); //存档移除奖励
            var chestCount = PlayerDataForGame.instance.gbocData.fightBoxs.Count; //剩余的宝箱数量
            chestUi.value.text = chestCount.ToString(); //改变宝箱数量
            UIManager.instance.ShowOrHideGuideObj(1, false);
            chestUi.chestButton.enabled = chestCount > 0;
        }

        ApiPanel.instance.Invoke(viewBag =>
                OnChestRecallAction(UIManager.instance.WarChestRecallAction(viewBag), chestUi),
            PlayerDataForGame.instance.ShowStringTips, EventStrings.Req_WarChest,
            ViewBag.Instance().SetValues(chestId, isConsume));
    }

    public void OnChestRecallAction(PlayerDataDto player, TaoYuanChestUI chestUi)
    {
        if (chestUi == jiuTan)
        {
            chestUi.value.text = (10 - player.DailyJiuTanRedemptionCount).ToString();
        }

        chestUi.SetChest(true); //UI，打开箱子
        AudioController0.instance.ChangeAudioClip(0);
        AudioController0.instance.PlayAudioSource(0);
    }

    public void OpenJiuTan() => OpenChest(jiuTan);

    public void CloseAllChests()
    {
        if (chestCostMap == null) return;
        foreach (var map in chestCostMap)
        {
            var chest = map.Key;
            chest.SetChest(false);
        }
    }
}
