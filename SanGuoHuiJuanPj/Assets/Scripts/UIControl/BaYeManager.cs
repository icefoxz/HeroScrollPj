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
    public int BaYeGoldDefault = 30; //霸业初始金币
    public int BaYeMaxGold = 50; //霸业金币上限
    private List<BaYeCityEvent> map;
    private Dictionary<int, int[]> storyEventSet;//数据表缓存
    public bool isShowTips;//是否弹出文字
    public string tipsText;//弹出文字内容

    public IReadOnlyList<BaYeCityEvent> Map => map;
    public static BaYeManager instance;
    private bool isHourlyEventRegistered;
    void Awake()
    {
        if (instance == null)
            instance = this;
        if (instance != this)
            Destroy(this);
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
        //初始化城池
        var maps = LoadJsonFile.baYeDiTuTableDatas
            .Select(s => new
            {
                Point = int.Parse(s[0]), Events = s[2].Split(',')
                    .Where(text => !string.IsNullOrWhiteSpace(text)).Select(int.Parse).ToList()
            }).Where(a => a.Events.Count > 0).ToList(); //获取表里的地图数据
        var events = maps.Select(city =>
            {
                var cityId = city.Point;
                var baYeEventId = city.Events.Select(id =>
                    new BaYeEventWeightElement(id, int.Parse(LoadJsonFile.baYeShiJianTableDatas[id][1]))).Pick().Id;
                 //根据地图获取对应的事件id列表，并根据权重随机获取一个事件id
                var baYeEvent = GetBaYeEvent(baYeEventId, cityId);
                return (cityId, baYeEventId, baYeEvent.ExpList, baYeEvent.WarId);
            })
            .OrderBy(e => e.cityId)
            .ToList(); //根据权重随机战役id
        map = events.Select(e => new BaYeCityEvent
                {CityId = e.cityId, EventId = e.baYeEventId, WarId = e.WarId, ExpList = e.ExpList})
            .ToList();

        var baYe = PlayerDataForGame.instance.warsData.baYe;
        foreach (var baYeEvent in baYe.data)
            map[baYeEvent.CityId] = baYeEvent;

        //初始化故事事件
        //事件点初始化
        storyEventSet = LoadJsonFile.storyPoolTableDatas.Select(column => new
                {column, point = int.Parse(column[0])})
            .Select(table =>
            {
                var storyIds = table.column[1].Split(',')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(int.Parse).ToArray();
                return new {table.point, storyIds};
            }).ToDictionary(obj => obj.point, obj => obj.storyIds); //读取数据表转化城key=事件点,value=事件列

        if (isHourlyEventRegistered) return;
        isHourlyEventRegistered = true;
        TimeSystemControl.instance.OnHourly += GenerateBaYeStoryEvents;
        if (SystemTimer.instance.Now - PlayerDataForGame.instance.warsData.baYe.lastStoryEventsRefreshHour >=
#if UNITY_EDITOR
            TimeSpan.FromHours(0) //调试期间每次开启游戏都会刷新霸业
#else
            TimeSpan.FromHours(1)
#endif
        ) //如果现在时间与上一次刷新时间(大于)相差1小时以上
            GenerateBaYeStoryEvents(); //刷新霸业事件
    }

    /// <summary>
    /// 刷新霸业故事事件
    /// </summary>
    public void GenerateBaYeStoryEvents()
    {
        //获取玩家等级可开启的事件点id列表
        //获取每个故事id的权重
        //根据权重随机故事事件(story)id
        //再每个事件点上的故事id随机行上的奖励范围
        //储存在霸业存档里
        var eventPoints = LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.Level - 1][8].Split(',')
            .Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToList();
        //事件点和故事信息
        var eventPointStoryMap = eventPoints
            .Join(storyEventSet, pId => pId, se => se.Key, (point, s) => new {point, storyIds = s.Value})
            .Select(s =>
            {
                return new
                {
                    s.point, storyEvents = s.storyIds.Select(id =>
                    {
                        var weight = int.Parse(LoadJsonFile.storyIdTableDatas[id][1]);
                        var type = int.Parse(LoadJsonFile.storyIdTableDatas[id][2]);
                        var goldRange = LoadJsonFile.storyIdTableDatas[id][3].Split(',')
                            .Where(t => !string.IsNullOrWhiteSpace(t)).Select(int.Parse).ToList();
                        var expRange = LoadJsonFile.storyIdTableDatas[id][4].Split(',')
                            .Where(t => !string.IsNullOrWhiteSpace(t)).Select(int.Parse).ToList();
                        return new StoryEvent(id, weight, type, (goldRange[0], goldRange[1]),
                            (expRange[0], expRange[1]));
                    })
                };
            })
            .ToDictionary(e => e.point, e => e.storyEvents.Pick());

        PlayerDataForGame.instance.warsData.baYe.storyMap = eventPointStoryMap.ToDictionary(m => m.Key, m =>
        {
            var story = m.Value;
            return new BaYeStoryEvent
            {
                StoryId = story.Id,
                Type = story.Type,
                GoldReward = Random.Range(story.GoldRange.Item1, story.GoldRange.Item2),
                ExpReward = Random.Range(story.ExpRange.Item1, story.ExpRange.Item2)
            };
        }).Where(kv=>kv.Value.StoryId!=0).ToDictionary(kv=>kv.Key,kv=>kv.Value);
        PlayerDataForGame.instance.warsData.baYe.lastStoryEventsRefreshHour =
            SystemTimer.instance.Now.Date.AddHours(SystemTimer.instance.Now.Hour);
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
        UIManager.instance.storyEventUiController.ResetUi();
    }

    public List<int> GetBaYeEventExp(int eventId)
    {
        var baYeEvent = LoadJsonFile.baYeShiJianTableDatas[eventId];//读取事件数据
        return baYeEvent[2].Split(',')
            .Where(s => !string.IsNullOrWhiteSpace(s)).ToList().Select(int.Parse).ToList();//从霸业事件[2]列获取对于的经验列
    }

    private BaYeCityEvent GetBaYeEvent(int eventId,int cityId)
    {
        var baYeEvent = LoadJsonFile.baYeShiJianTableDatas[eventId];//读取事件数据
        var baYeExp = baYeEvent[2].Split(',')
            .Where(s => !string.IsNullOrWhiteSpace(s)).ToList().Select(int.Parse).ToList();//从霸业事件[2]列获取对于的经验列
        var baYeBattleId = int.Parse(baYeEvent[3]);//霸业事件[3]列读取BaYeBattle表Id
        var battleTable = LoadJsonFile.baYeBattleTableDatas[baYeBattleId];
        var playerLevelAlign = PlayerDataForGame.instance.pyData.Level - 1;
        var levelTableId = LoadJsonFile.playerLevelTableDatas[playerLevelAlign][9];
        var warId = int.Parse(battleTable[int.Parse(levelTableId) + 1]);

        return new BaYeCityEvent {EventId = eventId, CityId = cityId, WarId = warId, ExpList = baYeExp, PassedStages = new bool[baYeExp.Count]};
    }
    /// <summary>
    /// 获取宝箱数据 item1 = id，item2 = 经验，item3 = 奖励id
    /// </summary>
    /// <returns></returns>
    public List<(int, int, int)> GetRewardChests() =>
        LoadJsonFile.baYeRenWuTableDatas.Select(item => (int.Parse(item[0]), int.Parse(item[1]), int.Parse(item[2])))
            .ToList();

    public void AddExp(int expIndex,int exp)
    {
        if (PlayerDataForGame.instance.warsData.baYe.ExpData.ContainsKey(expIndex))
            PlayerDataForGame.instance.warsData.baYe.ExpData[expIndex] += exp;
        else PlayerDataForGame.instance.warsData.baYe.ExpData.Add(expIndex, exp);
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
    }

    public void SetGold(int value)
    {
        PlayerDataForGame.instance.warsData.baYe.gold = value > BaYeMaxGold ? BaYeMaxGold : value;
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
    }

    public void AddGold(int gold)
    {
        PlayerDataForGame.instance.warsData.baYe.gold += gold;
        if (PlayerDataForGame.instance.warsData.baYe.gold > BaYeMaxGold)
            PlayerDataForGame.instance.warsData.baYe.gold = BaYeMaxGold;
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
    }

    public void AddGoldAndExp(int expIndex, int exp, int gold)
    {
        if (PlayerDataForGame.instance.warsData.baYe.ExpData.ContainsKey(expIndex))
            PlayerDataForGame.instance.warsData.baYe.ExpData[expIndex] += exp;
        else PlayerDataForGame.instance.warsData.baYe.ExpData.Add(expIndex, exp);
        PlayerDataForGame.instance.warsData.baYe.gold += gold;
        if (PlayerDataForGame.instance.warsData.baYe.gold > BaYeMaxGold)
            PlayerDataForGame.instance.warsData.baYe.gold = BaYeMaxGold;
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
    }

    public void SetExp(int expIndex,int exp)
    {
        if (PlayerDataForGame.instance.warsData.baYe.ExpData.ContainsKey(expIndex))
            PlayerDataForGame.instance.warsData.baYe.ExpData[expIndex] = exp;
        else PlayerDataForGame.instance.warsData.baYe.ExpData.Add(expIndex, exp);
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
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

    //当霸业故事事件被触发
    public bool OnStoryEventTrigger(int eventPoint)
    {
        var storyMap = PlayerDataForGame.instance.warsData.baYe.storyMap;
        if (!storyMap.ContainsKey(eventPoint)) return false;
        var sEvent = storyMap[eventPoint];
        switch (sEvent.Type)
        {
            case 0: break;
            case 1:
                OnReward(sEvent);
                break;
            case 2:
                var count = LoadJsonFile.testTableDatas.Count;
                var pick = Random.Range(0, count);
                var row = LoadJsonFile.testTableDatas[pick];
                var answerIndex = int.Parse(row[2]) - 1;//-1答案从1开始，索引从0开始。
                UIManager.instance.baYeMiniWindowUi.Show(row[1], new[] {row[3], row[4], row[5]}, answerIndex, () =>
                    {
                        OnReward(sEvent);
                        PlayerDataForGame.instance.ShowStringTips("三日不见，当刮目相看！");
                    },
                    () => PlayerDataForGame.instance.ShowStringTips("糊涂啊~"));
                break;
            default:
                XDebug.LogError<BaYeManager>($"未知故事事件类型={sEvent.Type}!");
                throw new ArgumentOutOfRangeException();
        }

        PlayerDataForGame.instance.warsData.baYe.storyMap.Remove(eventPoint);
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
        return true;

        void OnReward(BaYeStoryEvent storyEvent)
        {
            var rewardMap = new Dictionary<int, int>();
            if (storyEvent.GoldReward > 0)
                rewardMap.Add(0, storyEvent.GoldReward);
            if (sEvent.ExpReward > 0)
                rewardMap.Add(1, storyEvent.ExpReward);
            UIManager.instance.baYeMiniWindowUi.Show(rewardMap);
            AddGoldAndExp(-1, storyEvent.ExpReward, storyEvent.GoldReward);
            UIManager.instance.ResetBaYeProgressAndGold(PlayerDataForGame.instance.warsData.baYe);
        }
    }

    private class StoryEvent : IWeightElement
    {
        public int Id { get; }
        public int Weight { get; }
        public int Type { get; }
        public (int,int) GoldRange { get; }
        public (int,int) ExpRange { get; }

        public StoryEvent(int id, int weight, int type, (int, int) goldRange, (int, int) expRange)
        {
            Id = id;
            Weight = weight;
            Type = type;
            GoldRange = goldRange;
            ExpRange = expRange;
        }
    }

    private class BaYeEventWeightElement : IWeightElement
    {
        public int Id { get; set; }
        public int Weight { get; set; }

        public BaYeEventWeightElement(int id, int weight)
        {
            Id = id;
            Weight = weight;
        }
    }
}