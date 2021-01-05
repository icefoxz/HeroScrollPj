using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 霸业管理类
/// </summary>
public class BaYeManager : MonoBehaviour
{
    public int SelectedForceId { get; set; }
    public int BaYeGoldDefault = 50; //霸业初始金币
    private List<BaYeEvent> map;
    public bool isShowTips;//是否弹出文字
    public string tipsText;//弹出文字内容
    public IReadOnlyList<BaYeEvent> Map => map;
    public static BaYeManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Init()
    {
        var baYe = PlayerDataForGame.instance.warsData.baYe;
        //如果没有霸业记录或是记录已经过期(不是今天)将初始化新的霸业记录
        if (baYe == null || !SystemTimer.IsToday(baYe.lastBaYeActivityTime))
        {
            PlayerDataForGame.instance.warsData.baYe = baYe = new BaYeDataClass
            {
                lastBaYeActivityTime = SystemTimer.instance.NowUnixTicks,
                gold = BaYeGoldDefault,
                openedChest = new bool[UIManager.instance.baYeChestButtons.Length]
            };
            PlayerDataForGame.instance.isNeedSaveData = true;
            LoadSaveData.instance.SaveGameData(3);
        }

        InitBaYeMap();
    }

    private void InitBaYeMap()
    {
        var maps = LoadJsonFile.baYeDiTuTableDatas
            .Select(s => (int.Parse(s[0]), s[2].Split(',')
                .Where(text => !string.IsNullOrWhiteSpace(text)).ToList()))
            .Where(a => a.Item2.Any()).ToList(); //获取表里的地图数据
        var events = maps.Select(city =>
            {
                var cityId = city.Item1;
                var baYeEventId = int.Parse(LoadJsonFile.baYeShiJianTableDatas[
                    BackShiJianIdByWeightValue(city.Item2.Select(int.Parse)
                        .ToList())][3]); //根据地图获取对应的事件id列表，并根据权重随机获取一个事件id
                var baYeEvent = GetBaYeEvent(baYeEventId, cityId);
                return (cityId, baYeEventId, baYeEvent.ExpList, baYeEvent.WarId);
            })
            .OrderBy(e => e.cityId)
            .ToList(); //根据权重随机战役id
        map = events.Select(e => new BaYeEvent
                {CityId = e.cityId, EventId = e.baYeEventId, WarId = e.WarId, ExpList = e.ExpList})
            .ToList();

        var baYe = PlayerDataForGame.instance.warsData.baYe;
        foreach (var baYeEvent in baYe.data)
            map[baYeEvent.CityId] = baYeEvent;
        //{
        //    map[baYeEvent.CityId].CityId = baYeEvent.CityId;
        //    map[baYeEvent.CityId].EventId = baYeEvent.EventId;
        //    map[baYeEvent.CityId].ForceId = baYeEvent.ForceId;
        //    map[baYeEvent.CityId].WarId = baYeEvent.WarId;
        //}
    }

    public List<int> GetBaYeEventExp(int eventId)
    {
        var baYeEvent = LoadJsonFile.baYeShiJianTableDatas[eventId];//读取事件数据
        return baYeEvent[2].Split(',')
            .Where(s => !string.IsNullOrWhiteSpace(s)).ToList().Select(int.Parse).ToList();//从霸业事件[2]列获取对于的经验列
    }

    private BaYeEvent GetBaYeEvent(int eventId,int cityId)
    {
        var baYeEvent = LoadJsonFile.baYeShiJianTableDatas[eventId];//读取事件数据
        var baYeExp = baYeEvent[2].Split(',')
            .Where(s => !string.IsNullOrWhiteSpace(s)).ToList().Select(int.Parse).ToList();//从霸业事件[2]列获取对于的经验列
        var baYeBattleId = int.Parse(baYeEvent[3]);//霸业事件[3]列读取BaYeBattle表Id
        var battleTable = LoadJsonFile.baYeBattleTableDatas[baYeBattleId];
        var playerLevelAlign = PlayerDataForGame.instance.pyData.level - 1;
        var levelTableId = LoadJsonFile.playerLevelTableDatas[playerLevelAlign][9];
        var warId = int.Parse(battleTable[int.Parse(levelTableId) + 1]);

        return new BaYeEvent {EventId = eventId, CityId = cityId, WarId = warId, ExpList = baYeExp, PassedStages = new bool[baYeExp.Count]};
    }
    /// <summary>
    /// 获取宝箱数据 item1 = id，item2 = 经验，item3 = 奖励id
    /// </summary>
    /// <returns></returns>
    public List<(int, int, int)> GetRewardChests() =>
        LoadJsonFile.baYeRenWuTableDatas.Select(item => (int.Parse(item[0]), int.Parse(item[1]), int.Parse(item[2])))
            .ToList();

    public void AddExp(int code,int exp)
    {
        if (PlayerDataForGame.instance.warsData.baYe.ExpData.ContainsKey(code))
            PlayerDataForGame.instance.warsData.baYe.ExpData[code] += exp;
        else PlayerDataForGame.instance.warsData.baYe.ExpData.Add(code, exp);
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
    }

    public void SetExp(int code,int exp)
    {
        if (PlayerDataForGame.instance.warsData.baYe.ExpData.ContainsKey(code))
            PlayerDataForGame.instance.warsData.baYe.ExpData[code] = exp;
        else PlayerDataForGame.instance.warsData.baYe.ExpData.Add(code, exp);
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
    }


    private int BackShiJianIdByWeightValue(List<int> datas)
    {
        int weightValueSum = 0;
        for (int i = 0; i < datas.Count; i++)
        {
            weightValueSum += int.Parse(LoadJsonFile.baYeShiJianTableDatas[datas[i]][1]);
        }

        int randNum = Random.Range(0, weightValueSum);
        int indexTest = 0;
        while (randNum >= 0)
        {
            randNum -= int.Parse(LoadJsonFile.baYeShiJianTableDatas[datas[indexTest]][1]);
            indexTest++;
        }

        indexTest -= 1;
        return datas[indexTest];
    }

    //刷新霸业的势力武将选择（派去战斗）
    public void SelectBaYeForceCards(int forceId)
    {
        SelectedForceId = forceId;
        PlayerDataForGame.instance.fightHeroId.Clear();
        PlayerDataForGame.instance.fightTowerId.Clear();
        PlayerDataForGame.instance.fightTrapId.Clear();

        NowLevelAndHadChip heroDataIndex = new NowLevelAndHadChip(); //临时记录武将存档信息
        for (int i = 0; i < PlayerDataForGame.instance.hstData.heroSaveData.Count; i++)
        {
            heroDataIndex = PlayerDataForGame.instance.hstData.heroSaveData[i];
            if (forceId == int.Parse(LoadJsonFile.heroTableDatas[heroDataIndex.id][6]))
            {
                if (heroDataIndex.level > 0 || heroDataIndex.chips > 0)
                {
                    if (heroDataIndex.isFight > 0)
                    {
                        PlayerDataForGame.instance.fightHeroId.Add(heroDataIndex.id);
                    }
                }
            }
        }

        NowLevelAndHadChip fuzhuDataIndex = new NowLevelAndHadChip();
        for (int i = 0; i < PlayerDataForGame.instance.hstData.towerSaveData.Count; i++)
        {
            fuzhuDataIndex = PlayerDataForGame.instance.hstData.towerSaveData[i];
            if (forceId == int.Parse(LoadJsonFile.towerTableDatas[fuzhuDataIndex.id][15]))
            {
                if (fuzhuDataIndex.level > 0 || fuzhuDataIndex.chips > 0)
                {
                    if (fuzhuDataIndex.isFight > 0)
                    {
                        PlayerDataForGame.instance.fightTowerId.Add(fuzhuDataIndex.id);
                    }
                }
            }
        }

        for (int i = 0; i < PlayerDataForGame.instance.hstData.trapSaveData.Count; i++)
        {
            fuzhuDataIndex = PlayerDataForGame.instance.hstData.trapSaveData[i];
            if (forceId == int.Parse(LoadJsonFile.trapTableDatas[fuzhuDataIndex.id][14]))
            {
                if (fuzhuDataIndex.level > 0 || fuzhuDataIndex.chips > 0)
                {
                    if (fuzhuDataIndex.isFight > 0)
                    {
                        PlayerDataForGame.instance.fightTrapId.Add(fuzhuDataIndex.id);
                    }
                }
            }
        }
    }

}