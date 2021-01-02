using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public  static RewardManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);
    }

    public int GetYvQue(int chestId)
    {
        var list = LoadJsonFile.warChestTableDatas[chestId][4].Split(',');
        return UnityEngine.Random.Range(int.Parse(list[0]), int.Parse(list[1]) + 1);
    }
    public int GetYuanBao(int chestId)
    {
        var list = LoadJsonFile.warChestTableDatas[chestId][5].Split(',');
        return UnityEngine.Random.Range(int.Parse(list[0]), int.Parse(list[1]) + 1);
    }

    public List<RewardsCardClass> GetCards(int chestId,bool isZyBox)
    {
        var cardId = 0;
        var chips = 0;
        var rewards = new List<RewardsCardClass>();
        for (int i = 0; i < 3; i++)
        {
            string rewardCardString = LoadJsonFile.warChestTableDatas[chestId][i + 6];
            if (string.IsNullOrWhiteSpace(rewardCardString))
            {
                continue;
            }
            string typeId = "";
            switch (i)
            {
                case 0:
                    typeId = "3";   //陷阱类型
                    break;
                case 1:
                    typeId = "2";   //塔类型
                    break;
                case 2:
                    typeId = "0";   //武将类型
                    break;
                default:
                    break;
            }
            string[] rewardArrs = rewardCardString.Split(';');
            for (int j = 0; j < rewardArrs.Length; j++)
            {
                if (rewardArrs[j] != "")
                {
                    //0:稀有度 1：获得概率 2-3：少-多
                    string[] cardArrs = rewardArrs[j].Split(',');
                    //出现概率
                    if (int.Parse(cardArrs[1]) >= UnityEngine.Random.Range(1, 101))
                    {
                        cardId = GetRewardCardId(typeId, cardArrs[0], isZyBox);
                        chips = UnityEngine.Random.Range(int.Parse(cardArrs[2]), int.Parse(cardArrs[3]) + 1);

                        GetAndSaveCardChips(typeId, cardId, chips);

                        RewardsCardClass rewardCard = new RewardsCardClass
                        {
                            cardType = int.Parse(typeId), 
                            cardId = cardId, 
                            cardChips = chips
                        };
                        rewards.Add(rewardCard);
                    }
                }
            }
        }
        return rewards;
    }

    /// <summary>
    /// 获取到随机的卡牌id
    /// </summary>
    /// <param name="cardType">单位类型</param>
    /// <param name="rarity">稀有度</param>
    /// <param name="isZyBox">是否为战役宝箱</param>
    /// <returns>获取单位id</returns>
    public int GetRewardCardId(string cardType, string rarity, bool isZyBox)
    {
        int cardId = -1;

        switch (cardType)
        {
            case "0":
                cardId = GetBackRandomCardId(LoadJsonFile.heroTableDatas, isZyBox ? 19 : 20, 21, rarity);
                break;
            //case "1":
            //    cardIds = GetMustRarityCardSToList(LoadJsonFile.soldierTableDatas, rarity, 16);
            //    cardId = cardIds[UnityEngine.Random.Range(0, cardIds.Count)];
            //    break;
            case "2":
                cardId = GetBackRandomCardId(LoadJsonFile.towerTableDatas, isZyBox ? 11 : 13, 14, rarity);
                break;
            case "3":
                cardId = GetBackRandomCardId(LoadJsonFile.trapTableDatas, isZyBox ? 9 : 12, 13, rarity);
                break;
            //case "4":
            //    cardIds = GetMustRarityCardSToList(LoadJsonFile.spellTableDatas, rarity, 7);
            //    cardId = cardIds[UnityEngine.Random.Range(0, cardIds.Count)];
            //    break;
            default:
                break;
        }
        return cardId;
    }

    /// <summary>
    /// 返回随机选中卡牌id
    /// </summary>
    /// <param name="dataLists"></param>
    /// <param name="indexColumn"></param>
    /// <returns></returns>
    private int GetBackRandomCardId(List<List<string>> dataLists, int indexColumn, int indexChanChu, string rarity)
    {
        List<CardIdAndWeights> cwList = new List<CardIdAndWeights>();
        for (int i = 0; i < dataLists.Count; i++)
        {
            if (dataLists[i][indexChanChu] != "0" && dataLists[i][3] == rarity)
            {
                string[] arrs = dataLists[i][indexColumn].Split(',');
                if (int.Parse(arrs[0]) <= PlayerDataForGame.instance.pyData.level)
                {
                    CardIdAndWeights cw = new CardIdAndWeights();
                    cw.cardId = i;
                    cw.weights = int.Parse(arrs[1]);
                    cwList.Add(cw);
                }
            }
        }
        return BackCardIdByWeightValue(cwList);
    }

    //根据权重得到随机id
    private int BackCardIdByWeightValue(List<CardIdAndWeights> datas)
    {
        int weightValueSum = 0;
        for (int i = 0; i < datas.Count; i++)
        {
            weightValueSum += datas[i].weights;
        }
        int randNum = UnityEngine.Random.Range(0, weightValueSum);
        int indexTest = 0;
        while (randNum >= 0)
        {
            randNum -= datas[indexTest].weights;
            indexTest++;
        }
        indexTest -= 1;
        return datas[indexTest].cardId;
    }

    /// <summary>
    /// 获取并存储奖励碎片
    /// </summary>
    /// <param name="cardType">卡牌种类</param>
    /// <param name="cardId">具体id</param>
    /// <param name="chips">碎片数量</param>
    /// <returns></returns>
    public void GetAndSaveCardChips(string cardType, int cardId, int chips)
    {
        //Debug.Log("cardId:" + cardId);
        switch (cardType)
        {
            case "0":
                PlayerDataForGame.instance.hstData.heroSaveData[UIManager.instance.FindIndexFromData(PlayerDataForGame.instance.hstData.heroSaveData, cardId)].chips += chips;
                break;
            case "1":
                PlayerDataForGame.instance.hstData.soldierSaveData[UIManager.instance.FindIndexFromData(PlayerDataForGame.instance.hstData.soldierSaveData, cardId)].chips += chips;
                break;
            case "2":
                PlayerDataForGame.instance.hstData.towerSaveData[UIManager.instance.FindIndexFromData(PlayerDataForGame.instance.hstData.towerSaveData, cardId)].chips += chips;
                break;
            case "3":
                PlayerDataForGame.instance.hstData.trapSaveData[UIManager.instance.FindIndexFromData(PlayerDataForGame.instance.hstData.trapSaveData, cardId)].chips += chips;
                break;
            case "4":
                PlayerDataForGame.instance.hstData.spellSaveData[UIManager.instance.FindIndexFromData(PlayerDataForGame.instance.hstData.spellSaveData, cardId)].chips += chips;
                break;
            default:
                break;
        }
    }


    //卡牌和权重类
    private class CardIdAndWeights
    {
        public int cardId;
        public int weights;
    }

}
