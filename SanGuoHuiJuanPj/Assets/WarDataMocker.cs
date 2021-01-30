using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Editor;
using DG.Tweening;
using UnityEngine;

public class WarDataMocker : MonoBehaviour
{
    public int warId;
    [Tooltip("初始金币")]public int gold;//初始金币
    [Header("是否客制化卡牌，不点就用当前存档")]
    [Tooltip("是否选择客制化卡牌组合")]public bool isCustomCard;//是否客制化卡牌
    [Tooltip("只有选存档才会有效")]public UIManager.ForceFlags force;
    public PlayerDataForGame playerDataPrefab;
    public AudioController0 audioController0Prefab;
    public AudioController1 audioController1Prefab;
    public DoNewAdController adControllerPrefab;
    [Header("这里是客制化卡牌，必须点了客制化才会使用")]
    public MyCard[] heroes;
    public MyCard[] towers;
    public MyCard[] traps;

#if UNITY_EDITOR
    private void Awake()
    {
        LoadJsonFile.instance = new GameObject("loadJason").AddComponent<LoadJsonFile>();
        var player = Instantiate(playerDataPrefab);
        Instantiate(audioController0Prefab);
        Instantiate(audioController1Prefab);
        Instantiate(adControllerPrefab);
        PlayerDataForGame.instance = player;
        LoadSaveData.instance = player.gameObject.AddComponent<LoadSaveData>();
        LoadSaveData.instance.LoadByJson();
        player.WarType = PlayerDataForGame.WarTypes.Expedition;
        player.selectedWarId = warId;
        player.zhanYiColdNums = gold;
        PrepareCards();
    }
#endif

    private void PrepareCards()
    {
        var forceId = (int) force;
        var hst = PlayerDataForGame.instance.hstData;
        var hfMap = LoadJsonFile.heroTableDatas.Select(row =>
        {
            var id = int.Parse(row[0]);
            var origin = int.Parse(row[6]);
            return new {id, origin};
        }).ToDictionary(c => c.id, c => c.origin);
        Dictionary<int, List<int>> cards;
        if (!isCustomCard)
        {
            cards = hst.heroSaveData.Concat(hst.soldierSaveData.Concat(hst.towerSaveData)) //合并所有卡牌
                .Where(c => hfMap[c.id] == forceId && c.level > 0 && c.chips > 0 && c.isFight > 0) //过滤选中势力，并符合出战条件
                .GroupBy(c => c.typeIndex, c => c.id, (type, ids) => new {type, ids}) //把卡牌再根据卡牌类型分类组合
                .ToDictionary(c => c.type, c => c.ids.ToList()); //根据卡牌类型写入字典
        }
        else
        {
            cards = heroes.Select(c => new NowLevelAndHadChip {id = c.CardId, level = c.Level, typeIndex = 0})
                .Concat(towers.Select(t => new NowLevelAndHadChip {id = t.CardId, level = t.Level, typeIndex = 2}))
                .Concat(traps.Select(t => new NowLevelAndHadChip {id = t.CardId, level = t.Level, typeIndex = 3}))
                .GroupBy(c => c.typeIndex, c => c.id, (type, cIds) => new {type, cIds})
                .ToDictionary(c => c.type, c => c.cIds.ToList());
        }

        if (!cards.TryGetValue(0, out PlayerDataForGame.instance.fightHeroId))
            PlayerDataForGame.instance.fightHeroId = new List<int>();//英雄类型
        if (!cards.TryGetValue(2, out PlayerDataForGame.instance.fightTowerId)) //塔类型
            PlayerDataForGame.instance.fightTowerId = new List<int>();
        if (!cards.TryGetValue(3, out PlayerDataForGame.instance.fightTrapId)) //陷阱类型
            PlayerDataForGame.instance.fightTrapId = new List<int>();

    }
    [Serializable]
    public class MyCard
    {
        public int CardId;
        public int Level;
    }
}
