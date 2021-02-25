using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// 霸业管理类
/// </summary>
public class BaYeManager : MonoBehaviour
{
    public enum EventTypes
    {
        City,
        Story
    }
    public enum StoryEventTypes
    {
        无事件,
        开箱子,
        答题,
        讨伐,
        战令
    }

    public int BaYeGoldDefault = 30; //霸业初始金币
    public int BaYeMaxGold = 75; //霸业金币上限
    private List<BaYeCityEvent> map;
    private Dictionary<int, int[]> storyEventSet;//数据表缓存
    public bool isShowTips;//是否弹出文字
    public string tipsText;//弹出文字内容

    public IReadOnlyList<BaYeCityEvent> Map => map;
    public static BaYeManager instance;
    private bool isHourlyEventRegistered;
    public EventTypes CurrentEventType { get; private set; }//当前事件类型
    public int CurrentEventPoint { get; private set; }//当前事件点
    public BaYeStoryEvent CachedStoryEvent { get; private set; }//当前缓存的故事事件

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
        var zhanLing = DataTable.PlayerLevelData[PlayerDataForGame.instance.pyData.Level - 1][10]
            .Split(',').Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(int.Parse).ToArray();
        var zhanLingMap = new Dictionary<int, int>();
        for (int i = 0; i < zhanLing.Length; i++) zhanLingMap.Add(i, zhanLing[i]);
        //如果没有霸业记录或是记录已经过期(不是今天)将初始化新的霸业记录
        if (baYe == null || !SystemTimer.IsToday(baYe.lastBaYeActivityTime))
        {
            PlayerDataForGame.instance.warsData.baYe = new BaYeDataClass
            {
                lastBaYeActivityTime = SystemTimer.instance.NowUnixTicks,
                gold = BaYeGoldDefault,
                openedChest = new bool[UIManager.instance.baYeChestButtons.Length],
                zhanLingMap = zhanLingMap
            };
            PlayerDataForGame.instance.isNeedSaveData = true;
            LoadSaveData.instance.SaveGameData(3);
        }
        BugHotFix.OnFixZhanLingV1_99(baYe, zhanLingMap);

        InitBaYeMap();
    }

    private void InitBaYeMap()
    {
        //初始化城池
        var maps = DataTable.BaYeDiTu
            .Select(m => new
            {
                Point = m.Key, Events = m.Value[2].TableStringToInts().ToList()
            }).Where(a => a.Events.Count > 0).ToList(); //获取表里的地图数据
        var events = maps.Select(city =>
            {
                var cityId = city.Point;
                var baYeEventId = city.Events.Select(id =>
                    new BaYeEventWeightElement(id, int.Parse(DataTable.BaYeShiJian[id][1]))).Pick().Id;
                 //根据地图获取对应的事件id列表，并根据权重随机获取一个事件id
                var baYeEvent = GetBaYeEvent(baYeEventId, cityId);
                return (cityId, baYeEventId, baYeEvent.ExpList, WarIds: baYeEvent.WarIds);
            })
            .OrderBy(e => e.cityId)
            .ToList(); //根据权重随机战役id
        map = events.Select(e => new BaYeCityEvent
                {CityId = e.cityId, EventId = e.baYeEventId, WarIds = e.WarIds, ExpList = e.ExpList})
            .ToList();

        var baYe = PlayerDataForGame.instance.warsData.baYe;
        foreach (var baYeEvent in baYe.data)
            map[baYeEvent.CityId] = baYeEvent;

        //初始化故事事件
        //事件点初始化
        var now = DateTime.Now;
        storyEventSet = DataTable.StoryPool
            .ToDictionary(obj => obj.Key, obj => obj.Value[1].TableStringToInts()
                .Where(id =>
                    DataTable.StoryId[id][8]
                        .IsTableTimeInRange(now)
                ).ToArray()); //读取数据表转化城key=事件点,value=事件列

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
        var eventPoints = DataTable.PlayerLevelData[PlayerDataForGame.instance.pyData.Level - 1][8]
            .TableStringToInts().ToList();
        //事件点和故事信息
        var eventPointStoryMap = eventPoints
            .Join(storyEventSet, pId => pId, se => se.Key, (point, s) => new {point, storyIds = s.Value})
            .Select(s =>
            {
                return new
                {
                    s.point, storyEvents = s.storyIds.Select(id =>
                    {
                        var weight = int.Parse(DataTable.StoryId[id][1]);
                        var type = int.Parse(DataTable.StoryId[id][2]);
                        var goldRange = DataTable.StoryId[id][3].TableStringToInts().ToList();
                        var expRange = DataTable.StoryId[id][4].TableStringToInts().ToList();
                        var yuQueRange = DataTable.StoryId[id][5].TableStringToInts().ToList();
                        var yuanBaoRange = DataTable.StoryId[id][6].TableStringToInts().ToList();
                        var zhanLing = DataTable.StoryId[id][7].TableStringToInts().ToList();
                        return new StoryEvent(id, weight, type,
                            (goldRange[0], goldRange[1]),
                            (expRange[0], expRange[1]),
                            (yuQueRange[0], yuQueRange[1]),
                            (yuanBaoRange[0], yuanBaoRange[1]),
                            (zhanLing[0], zhanLing[1]));
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
                ExpReward = Random.Range(story.ExpRange.Item1, story.ExpRange.Item2),
                YvQueReward = Random.Range(story.YuQueRange.Item1, story.YuQueRange.Item2),
                YuanBaoReward = Random.Range(story.YuanBaoRange.Item1, story.YuanBaoRange.Item2),
                WarId = int.TryParse(DataTable.StoryId[story.Id][9], out var warId) ? warId : -1,
                ZhanLing = GetZhanLing(story.ZhanLingRange.Item1, story.ZhanLingRange.Item2)
            };
        }).Where(kv => kv.Value.StoryId != 0).ToDictionary(kv => kv.Key, kv => kv.Value);
        PlayerDataForGame.instance.warsData.baYe.lastStoryEventsRefreshHour =
            SystemTimer.instance.Now.Date.AddHours(SystemTimer.instance.Now.Hour);
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
        UIManager.instance.storyEventUiController.ResetUi();
        if(PlayerDataForGame.instance.CurrentScene == PlayerDataForGame.GameScene.MainScene)
        {
            SelectorUIMove(false, null);
        }

        Dictionary<int, int> GetZhanLing(int min, int max)
        {
            var forceList = DataTable.ShiLi.Keys.ToList();
            var zhanLingSelection = new Dictionary<int, int>();
            for (int i = 0; i < 2; i++)
            {
                var index = Random.Range(0, forceList.Count);
                var pick = forceList[index];
                zhanLingSelection.Add(pick,Random.Range(min, max + 1));
                forceList.Remove(pick);
            }
            return zhanLingSelection;
        }
    }

    public void OnBaYeWarEventPointSelected(EventTypes type,int eventPoint)
    {
        var isWarEvent = false;
        switch (type)
        {
            case EventTypes.City:
            {
                isWarEvent = true;
                var cEvent = map.Single(e => e.CityId == eventPoint);
                PlayerDataForGame.instance.selectedCity = cEvent.CityId;
                PlayerDataForGame.instance.selectedBaYeEventId = cEvent.EventId;
                SelectorUIMove(cEvent.EventId == PlayerDataForGame.instance.selectedBaYeEventId,
                    UIManager.instance.baYeBattleEventController.eventList[cEvent.CityId].transform);
                print(string.Join(",", cEvent.WarIds));
            }
                break;
            case EventTypes.Story:
            {
                var storyMap = PlayerDataForGame.instance.warsData.baYe.storyMap;
                if (!storyMap.ContainsKey(eventPoint))
                    throw XDebug.Throw<BaYeManager>("霸业故事点不存在!");
                var sEvent = storyMap[eventPoint];
                OnStoryEventTrigger(eventPoint,sEvent);
                isWarEvent = (StoryEventTypes) sEvent.Type == StoryEventTypes.讨伐;
            }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        if (!isWarEvent) return;
        //讨伐事件将记录讨伐类型和讨伐点
        CurrentEventType = type;
        CurrentEventPoint = eventPoint;
    }

    //选择器UI
    private void SelectorUIMove(bool display,Transform targetTransform)
    {
        var selector = UIManager.instance.chooseBaYeEventImg;
        selector.SetActive(display);
        if (!display)
        {
            selector.transform.position = Vector3.zero;
            return;
        }
        selector.transform.position = targetTransform.position;
    }

    private BaYeCityEvent GetBaYeEvent(int eventId,int cityId)
    {
        var battleId = int.Parse(DataTable.PlayerLevel[PlayerDataForGame.instance.pyData.Level][9]);//根据等级表获取战斗id
        var baYeEvent = DataTable.BaYeShiJian[eventId];//读取事件数据
        var expList = baYeEvent[2].TableStringToInts().ToList();//从霸业事件[2]列获取对于的经验列
        var baYeBattleId = int.Parse(baYeEvent[3]);//霸业事件[3]列读取BaYeBattle表Id
        var battleStageList = DataTable.BaYeBattle[baYeBattleId];//获取对应战斗表
        var warIds = battleStageList[battleId].TableStringToInts().ToList();
        return new BaYeCityEvent {EventId = eventId, CityId = cityId, WarIds = warIds, ExpList = expList, PassedStages = new bool[expList.Count]};
    }

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

    public bool TradeZhanLing(int forceId, int amount)
    {
        if (amount < 0 && PlayerDataForGame.instance.warsData.baYe.zhanLingMap[forceId] < -amount)
            return false; //如果数目是负数，并玩家数量小于数量返回交易失败
        PlayerDataForGame.instance.warsData.baYe.zhanLingMap[forceId] += amount;
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
        return true;
    }

    public void OnBayeStoryEventReward(BaYeStoryEvent storyEvent)
    {
        var expIndex = -1; //霸业故事事件送的expId一律 -1
        var baYe = PlayerDataForGame.instance.warsData.baYe;
        if (baYe.ExpData.ContainsKey(expIndex))
            baYe.ExpData[expIndex] += storyEvent.ExpReward;
        else baYe.ExpData.Add(expIndex, storyEvent.ExpReward);
        baYe.gold += storyEvent.GoldReward;
        if (baYe.gold > BaYeMaxGold) //不超过上限
            baYe.gold = BaYeMaxGold;
        //战令
        foreach (var ling in storyEvent.ZhanLing)
        {
            baYe.zhanLingMap.Trade(ling.Key, ling.Value, true);
        }
        var py = PlayerDataForGame.instance.pyData;
        if (storyEvent.YvQueReward > 0) py.YvQue += storyEvent.YvQueReward;
        if (storyEvent.YuanBaoReward > 0) py.YuanBao += storyEvent.YuanBaoReward;
        if(PlayerDataForGame.instance.CurrentScene == PlayerDataForGame.GameScene.MainScene)
        {
            UIManager.instance.yuanBaoNumText.text = py.YvQue.ToString();
            UIManager.instance.yvQueNumText.text = py.YvQue.ToString();
        }
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(5);
    }

    public void SetExp(int expIndex,int exp)
    {
        if (PlayerDataForGame.instance.warsData.baYe.ExpData.ContainsKey(expIndex))
            PlayerDataForGame.instance.warsData.baYe.ExpData[expIndex] = exp;
        else PlayerDataForGame.instance.warsData.baYe.ExpData.Add(expIndex, exp);
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
    }

    //当霸业故事事件被触发
    private void OnStoryEventTrigger(int eventPoint,BaYeStoryEvent sEvent)
    {
        switch ((StoryEventTypes)sEvent.Type)
        {
            case StoryEventTypes.无事件: break;
            case StoryEventTypes.开箱子:
                OnReward(sEvent);
                break;
            case StoryEventTypes.答题:
            {
                var count = DataTable.TestData.Count;
                var pick = Random.Range(0, count);
                var row = DataTable.TestData[pick];
                var answerIndex = int.Parse(row[2]) - 1; //-1答案从1开始，索引从0开始。
                UIManager.instance.baYeWindowUi.ShowQuest(row[1], new[] {row[3], row[4], row[5]}, answerIndex, () =>
                    {
                        OnReward(sEvent);
                        PlayerDataForGame.instance.ShowStringTips("三日不见，当刮目相看！");
                    },
                    () => PlayerDataForGame.instance.ShowStringTips("糊涂啊~"));
                break;
            }
            case StoryEventTypes.讨伐:
            {
                //标记霸业的战斗点。等待开启战斗
                SelectorUIMove(true, UIManager.instance.storyEventUiController.storyEventPoints[eventPoint].transform);
                print($"故事type[{sEvent.Type}]-讨伐事件 storyId[{sEvent.StoryId}] warId[{sEvent.WarId}]");
                return;//征战活动不删除记录。等到触发了才删除
            }
            case StoryEventTypes.战令:
            {
                UIManager.instance.baYeWindowUi.ShowSelection(sEvent.ZhanLing, selectedId =>
                {
                    var message = TradeZhanLing(selectedId, sEvent.ZhanLing[selectedId])
                        ? $"获得[{DataTable.ShiLi[selectedId][1]}]战令"
                        : "战令获取异常！";
                    UIManager.instance.baYeForceSelectorUi.UpdateZhanLing();
                    PlayerDataForGame.instance.ShowStringTips(message);
                });
                break;
            }
            default:
                XDebug.LogError<BaYeManager>($"未知故事事件类型={sEvent.Type}!");
                throw new ArgumentOutOfRangeException();
        }
        PlayerDataForGame.instance.warsData.baYe.storyMap.Remove(eventPoint);
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);

        void OnReward(BaYeStoryEvent se)
        {
            var rewardMap = InstanceReward(se);
            UIManager.instance.baYeWindowUi.Show(rewardMap);
            OnBayeStoryEventReward(se);
            UIManager.instance.baYeWindowUi.ShowAdButton(adBtn =>
            {
                AdAgent.instance.CallAd((success, msg) =>
                {
                    if (success)
                    {
                        UIManager.instance.baYeWindowUi.Show(InstanceReward(se, 2));
                        adBtn.gameObject.SetActive(false);
                        OnBayeStoryEventReward(se);
                        UIManager.instance.ResetBaYeProgressAndGold();
                        return;
                    }
                    PlayerDataForGame.instance.ShowStringTips($"获取失败！\n{msg}");
                });
            });
            UIManager.instance.ResetBaYeProgressAndGold();

            Dictionary<int, int> InstanceReward(BaYeStoryEvent baYeStoryEvent, int rate = 1) =>
                new Dictionary<int, int>()
                {
                    {0, baYeStoryEvent.GoldReward * rate},
                    {1, baYeStoryEvent.ExpReward * rate},
                    {2, baYeStoryEvent.YuanBaoReward * rate},
                    {3, baYeStoryEvent.YvQueReward * rate}
                };
        }
    }


    private class StoryEvent : IWeightElement
    {
        public int Id { get; }
        public int Weight { get; }
        public int Type { get; }
        public (int,int) GoldRange { get; }
        public (int,int) ExpRange { get; }
        public (int,int) YuQueRange { get; }
        public (int,int) YuanBaoRange { get; }
        public (int,int) ZhanLingRange { get; }

        public StoryEvent(int id, int weight, int type, (int, int) goldRange, (int, int) expRange,(int,int) yuQueRange,(int,int) yuanBaoRange,(int,int) zhanLingRange)
        {
            Id = id;
            Weight = weight;
            Type = type;
            GoldRange = goldRange;
            ExpRange = expRange;
            YuQueRange = yuQueRange;
            YuanBaoRange = yuanBaoRange;
            ZhanLingRange = zhanLingRange;
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

    public void CacheCurrentStoryEvent() => CachedStoryEvent = PlayerDataForGame.instance.warsData.baYe.storyMap[CurrentEventPoint];
}