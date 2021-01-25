using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Beebyte.Obfuscator;
using UnityEngine;
using UnityEngine.UI;

public class TaoYuan : MonoBehaviour
{
    public int openJiuTanYBNums;   //开酒坛所需元宝
    public Button freeJiuTanAdButton;//开酒坛的免费按键
    private bool isConsumeAd;//是否消费了广告
    public JiuTanUI jiuTan;//酒坛
    public TaoYuanChestUI zhanYiChest;//战役宝箱
    public YvQueChestUI copperChest;//铜宝箱
    public YvQueChestUI goldChest;//金宝箱
    private Dictionary<TaoYuanChestUI, int> chestCostMap;//宝箱和价钱的映射表

    private void Start()
    {
        InitChests();
    }

    //初始化宝箱的展示
    private void InitChests()
    {
        isBusy = false;
        chestCostMap = new Dictionary<TaoYuanChestUI, int>
        {
            {jiuTan, LoadJsonFile.GetGameValue(3)},
            {zhanYiChest, 0},//战役宝箱不花费
            {copperChest, LoadJsonFile.GetGameValue(4)},
            {goldChest, LoadJsonFile.GetGameValue(5)}
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
            chest.chestButton.interactable = zyChestCount > 0;
            continue;
        }
    }

    public void UpdateJiuTan(bool isReady,string jiuTanCount, string countDown)
    {
        jiuTan.chestButton.gameObject.SetActive(isReady);
        jiuTan.chestButton.interactable = isReady;
        freeJiuTanAdButton.gameObject.SetActive(isReady);
        freeJiuTanAdButton.interactable = isReady;
        jiuTan.displayText.gameObject.SetActive(!isReady);//倒数文本
        jiuTan.lasting.gameObject.SetActive(!isReady);//倒数文本
        if (!isReady) jiuTan.UpdateUi(jiuTanCount, countDown);
    }

    /// <summary>
    /// 观看视频免费开启酒坛
    /// </summary>
    [Skip] public void OnWatchAdGetJiuTan()
    {
        //获得酒坛消耗元宝按钮
        freeJiuTanAdButton.interactable = false;
        jiuTan.chestButton.interactable = false;
        DoNewAdController.instance.GetReWardVideo(OnSuccess, OnFailed);
        
        void OnSuccess()
        {
            int index = openJiuTanYBNums;
            openJiuTanYBNums = 0;
            isConsumeAd = true;//已成功消费广告
            OpenChest(jiuTan);
            openJiuTanYBNums = index;
            PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(5));
            freeJiuTanAdButton.interactable = true;
            jiuTan.chestButton.interactable = true;

        }

        void OnFailed()
        {
            PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(6));
            freeJiuTanAdButton.interactable = true;
            jiuTan.chestButton.interactable = true;
            isConsumeAd = false;
        }
    }

    /// <summary>
    /// 打开桃园宝箱
    /// </summary>
    /// <param name="chest"></param>
    public void OpenChest(TaoYuanChestUI chest)
    {
        if (isBusy) return;
        isBusy = true;
        AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[13], AudioController0.instance.audioVolumes[13]);

        var isSuccessSpend = false;
        var chestId = -1;//宝箱在表里的Id
        if (chest == jiuTan) //酒坛
        {
            chestId = 0;
            //如果玩家元宝小于预设的酒坛花费或开酒坛失败(时间还未到)
            if (PlayerDataForGame.instance.pyData.YuanBao < openJiuTanYBNums ||
                !TimeSystemControl.instance.OnClickToGetJiuTan())
            {
                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(0));
                AudioController0.instance.PlayAudioSource(0);
                isBusy = false;
                return;
            }

            PlayerDataForGame.instance.Redemption(PlayerDataForGame.RedeemTypes.JiuTan);//标记已消费酒坛
            if (!isConsumeAd)
                isSuccessSpend = ConsumeManager.instance.DeductYuanBao(openJiuTanYBNums); //扣除酒坛元宝
            else
            {
                //如果已消费广告就不扣除元宝
                isSuccessSpend = true;
                isConsumeAd = false;
            }
        }

        if (chest == copperChest)
        {
            chestId = 1;
            isSuccessSpend = TimeSystemControl.instance.OnClickToGetFreeBox1() ||
                             ConsumeManager.instance.DeductYuQue(chestCostMap[chest]); //如果时间开启不了，尝试扣除玉阙开启
        }

        if (chest == goldChest)
        {
            chestId = 2;
            isSuccessSpend = TimeSystemControl.instance.OnClickToGetFreeBox2() ||
                             ConsumeManager.instance.DeductYuQue(chestCostMap[chest]);
            UIManager.instance.ShowOrHideGuideObj(0, false);
        }

        if (chest == zhanYiChest) //战役宝箱
        {
            if (PlayerDataForGame.instance.gbocData.fightBoxs.Count <= 0)//如果没有宝箱记录
            {
                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(7));
                AudioController0.instance.PlayAudioSource(0);
                chest.chestButton.interactable = false;
                isBusy = false;
                return;
            }

            chestId = PlayerDataForGame.instance.gbocData.fightBoxs[0];//获取叠在最上面的奖励id
            PlayerDataForGame.instance.gbocData.fightBoxs.Remove(chestId);//存档移除奖励
            var chestCount = PlayerDataForGame.instance.gbocData.fightBoxs.Count;//剩余的宝箱数量
            chest.value.text = chestCount.ToString();//改变宝箱数量
            UIManager.instance.ShowOrHideGuideObj(1, false);
            chest.chestButton.interactable = chestCount > 0;
            isSuccessSpend = true;
        }

        if (isSuccessSpend)
        {
            int exp = int.Parse(LoadJsonFile.warChestTableDatas[chestId][3]);//获取经验
            UIManager.instance.GetPlayerExp(exp);//增加经验
            var yuanBao = RewardManager.instance.GetYuanBao(chestId);//获取元宝
            var yvQue = RewardManager.instance.GetYvQue(chestId);//获取玉阙
            ConsumeManager.instance.AddYuQue(yvQue);//增加玉阙
            ConsumeManager.instance.AddYuanBao(yuanBao);//增加元宝

            var rewards = RewardManager.instance.GetCards(chestId, chest == zhanYiChest);//获取卡牌
            PlayerDataForGame.instance.isNeedSaveData = true;
            LoadSaveData.instance.SaveGameData();

            UIManager.instance.ShowRewardsThings(yuanBao, yvQue, exp, 0, rewards, 1.5f);//显示奖励窗口
            isBusy = true;
            chest.SetChest(true);//UI，打开箱子
            AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[0], AudioController0.instance.audioVolumes[0]);
        }
        AudioController0.instance.PlayAudioSource(0);
        isBusy = false;
    }

    bool isBusy;  //是否有宝箱正在开启
    public void CloseBoxChange()
    {
        isBusy = false;
    }

    public void CloseAllChests()
    {
        foreach (var map in chestCostMap)
        {
            var chest = map.Key;
            chest.SetChest(false);
        }
    }
}
