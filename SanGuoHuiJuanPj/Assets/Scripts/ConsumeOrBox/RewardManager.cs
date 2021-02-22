using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);
    }

    public int GetYvQue(int chestId)
    {
        var list = DataTable.WarChestData[chestId][4].Split(',');
        return UnityEngine.Random.Range(int.Parse(list[0]), int.Parse(list[1]) + 1);
    }
    public int GetYuanBao(int chestId)
    {
        var list = DataTable.WarChestData[chestId][5].Split(',');
        return UnityEngine.Random.Range(int.Parse(list[0]), int.Parse(list[1]) + 1);
    }

    public List<RewardsCardClass> GetCards(int chestId, bool isZyBox)
    {
        const int trapIndex = 6;
        const int towerIndex = 7;
        const int heroIndex = 8;
        var row = DataTable.WarChest[chestId];
        var trapMap = row[trapIndex].Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        var towerMap = row[towerIndex].Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        var heroMap = row[heroIndex].Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        var rewards = new List<RewardsCardClass>();
        var trapReward = RandomPickBaseOnStrategy(trapMap, GameCardType.Trap);
        var towerReward = RandomPickBaseOnStrategy(towerMap, GameCardType.Tower);
        var heroReward = RandomPickBaseOnStrategy(heroMap, GameCardType.Hero);
        if (trapReward != null && trapReward.Count > 0) rewards.AddRange(trapReward);
        if (towerReward != null && towerReward.Count > 0) rewards.AddRange(towerReward);
        if (heroReward != null && heroReward.Count > 0) rewards.AddRange(heroReward);
        return rewards;

        //private method
        List<RewardsCardClass> RandomPickBaseOnStrategy(List<string> items, GameCardType gameCardType)
        {
            var list = new List<RewardsCardClass>();
            items.ForEach(r =>
            {
                var map = r.TableStringToInts().ToList();
                var randomValue = map[1];
                if (randomValue >= UnityEngine.Random.Range(0, 101))
                {
                    var type = gameCardType;
                    var rare = map[0];
                    var rewardCards = GetRewardCards(type, rare, isZyBox);
                    if (rewardCards.Count == 0) return;
                    var pick = rewardCards.Pick();
                    var chips = UnityEngine.Random.Range(map[2], map[3] + 1);
                    RewardCard(type, pick.cardId, chips);
                    list.Add(new RewardsCardClass
                    {
                        cardId = pick.cardId,
                        cardType = (int) gameCardType,
                        cardChips = chips
                    });
                }
            });
            return list;
        }

        List<CardIdAndWeights> GetRewardCards(GameCardType cardType, int rarity, bool isZy)
        {
            switch (cardType)
            {
                case GameCardType.Hero:
                    return GetWeightList(DataTable.Hero, isZy ? 19 : 20, 21, rarity);
                case GameCardType.Tower:
                    return GetWeightList(DataTable.Tower, isZy ? 11 : 13, 14, rarity);
                case GameCardType.Trap:
                    return GetWeightList(DataTable.Trap, isZy ? 10 : 11, 13, rarity);
                default:
                    throw new ArgumentOutOfRangeException($"cardType = {cardType}");
            }
        }
    }

    /// <summary>
    /// 返回随机选中卡牌id
    /// </summary>
    /// <param name="data"></param>
    /// <param name="outputStrategyIndex"></param>
    /// <param name="outputIndex"></param>
    /// <param name="rarity"></param>
    /// <returns></returns>
    private List<CardIdAndWeights> GetWeightList(IReadOnlyDictionary<int,IReadOnlyList<string>> data, int outputStrategyIndex, int outputIndex, int rarity)
    {
        const string NoOutput = "0";
        var playerLevel = PlayerDataForGame.instance.pyData.Level;
        return data.Where(map => map.Value[outputIndex] != NoOutput && int.Parse(map.Value[3]) == rarity)
            .Select(map =>
            {
                var strategy = map.Value[outputStrategyIndex].TableStringToInts().ToList();
                return new CardIdAndWeights
                {
                    cardId = map.Key,
                    outputLevel = strategy[0],
                    weight = strategy[1]
                };
            }).Where(c => c.outputLevel != 0 && c.outputLevel <= playerLevel).ToList();
    }

    /// <summary>
    /// 获取并存储奖励碎片
    /// </summary>
    /// <param name="cardType">卡牌种类</param>
    /// <param name="cardId">具体id</param>
    /// <param name="chips">碎片数量</param>
    /// <returns></returns>
    public void RewardCard(GameCardType cardType, int cardId, int chips)
    {
        //Debug.Log("cardId:" + cardId);
        switch (cardType)
        {
            case GameCardType.Hero:
                PlayerDataForGame.instance.hstData.heroSaveData.GetOrInstance(cardId).chips += chips;
                break;
            case GameCardType.Soldier:
                PlayerDataForGame.instance.hstData.soldierSaveData.GetOrInstance(cardId).chips += chips;
                break;
            case GameCardType.Tower:
                PlayerDataForGame.instance.hstData.towerSaveData.GetOrInstance(cardId).chips+=chips;
                break;
            case GameCardType.Trap:
                PlayerDataForGame.instance.hstData.trapSaveData.GetOrInstance(cardId).chips += chips;
                break;
            case GameCardType.Spell:
                PlayerDataForGame.instance.hstData.spellSaveData.GetOrInstance(cardId).chips += chips;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    //卡牌和权重类
    private class CardIdAndWeights : IWeightElement
    {
        public int Weight => weight;
        public int cardId;
        public int weight;
        public int outputLevel;
    }
}
