using System;
using System.Collections.Generic;
using System.Linq;
using Beebyte.Obfuscator;
using CorrelateLib;
using Newtonsoft.Json;

#region 玩家数据相关类

/// <summary>
/// 玩家账户信息存档
/// </summary>
public interface IUserInfo
{
    /// <summary>
    /// 账号
    /// </summary>
    string Username { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    string Password { get; set; }
    /// <summary>
    /// 手机号
    /// </summary>
    string Phone { get; set; }
    /// <summary>
    /// 硬件唯一标识id
    /// </summary>
    string DeviceId { get; set; }
    /// <summary>
    /// 与服务器最后一次互交的时间
    /// </summary>
    long LastUpdate { get; set; }
    /// <summary>
    /// 游戏版本号
    /// </summary>
    float GameVersion { get; set; }
}

/// <summary>
/// 玩家基本信息存档数据类
/// </summary>
public interface IPlayerData
{
    //等级
    int Level { get; set; }
    //经验
    int Exp { get; set; }
    //元宝
    int YuanBao { get; set; }
    //玉阙
    int YvQue { get; set; }
    //体力
    int Stamina { get; set; }
    //玩家初始势力id
    int ForceId { get; set; }
    //上次锦囊获取时间
    long LastJinNangRedeemTime { get; set; }
    //锦囊每天的获取次数
    int DailyJinNangRedemptionCount { get; set; }
    //上次酒坛获取时间
    long LastJiuTanRedeemTime { get; set; }
    //酒坛每天的获取次数
    int DailyJiuTanRedemptionCount { get; set; }
    //上次无双宝箱获取时间
    long LastFourDaysChestRedeemTime { get; set; }
    //上次史诗宝箱获取时间
    long LastWeekChestRedeemTime { get; set; }
    //上一个游戏版本号
    float LastGameVersion { get; set; }
}

/// <summary>
/// 玩家上传下载的数据规范
/// </summary>
public interface IUserSaveArchive
{
    // 账号
    string Username { get; set; }
    // 密码
    string Password { get; set; }
    // 手机号
    string Phone { get; set; }
    // 硬件唯一标识id
    string DeviceId { get; set; }
    // 与服务器最后一次互交的时间
    long LastUpdate { get; set; }
    //玩家信息
    string PlayerInfo { get; set; }
    /// <summary>
    ///卡牌数据 HSTDataClass
    /// </summary>
    string CardsData { get; set; }
    /// <summary>
    /// 征战记录 WarsDataClass
    /// </summary>
    string Expedition { get; set; }
    /// <summary>
    ///奖励或兑换码记录 GetBoxOrCodeData
    /// </summary>
    string RewardsRecord { get; set; }
}
[Skip]
public class GetBoxOrCodeData
{
    //战役宝箱
    public List<int> fightBoxs;
    //兑换码
    public List<RedemptionCodeGot> redemptionCodeGotList;
}
[Skip]
public class RedemptionCodeGot
{
    public int id;      //兑换码id
    public bool isGot;  //是否领取过
}
[Skip]
public class NowLevelAndHadChip : IGameCard//card
{
    public static NowLevelAndHadChip Instance(GameCardDto dto)
    {
        return new NowLevelAndHadChip
        {
            id = dto.CardId, 
            chips = dto.Chips, 
            level = dto.Level, 
            typeIndex = (int) dto.Type
        };
    }
    public int id;          //id
    public int level = 1;   //当前等级
    public int chips;       //拥有碎片
    public int isFight;     //是否出战
    public int typeIndex;   //单位类型0武将1士兵2塔3陷阱4技能
    public bool isHad;      //是否拥有过
    public int maxLevel = 1;    //历史最高星级

    public bool IsOwned => chips > 0 || level > 0;//是否拥有
    public int CardId => id;
    public int Level => level;
    public int Chips => chips;
    public int Type => typeIndex;
}
/// <summary>
/// 武将，士兵，塔等 信息存档数据类
/// </summary>
[Skip]
public class HSTDataClass
{
    //武将
    public List<NowLevelAndHadChip> heroSaveData;
    //士兵
    public List<NowLevelAndHadChip> soldierSaveData;
    //塔
    public List<NowLevelAndHadChip> towerSaveData;
    //陷阱
    public List<NowLevelAndHadChip> trapSaveData;
    //技能
    public List<NowLevelAndHadChip> spellSaveData;
}
[Skip]
public class UnlockWarCount
{
    public int warId;           //战役id
    public int unLockCount;     //解锁关卡数
    public bool isTakeReward;   //是否领过首通宝箱
}
/// <summary>
/// 霸业管理类
/// </summary>
[Skip]
public class BaYeDataClass
{
    public long lastBaYeActivityTime;
    public DateTimeOffset lastStoryEventsRefreshHour;
    [JsonIgnore] public int CurrentExp => ExpData.Values.Sum();
    public int gold;
    /// <summary>
    /// 经验映像记录，key = 城池/事件id 注意：-1为额外添加的奖励事件, value = 奖励的经验值
    /// </summary>
    public Dictionary<int, int> ExpData = new Dictionary<int, int>();
    public List<BaYeCityEvent> data = new List<BaYeCityEvent>();
    /// <summary>
    /// 故事剧情映像表，key = eventPoint地点, value = storyEvent故事事件
    /// </summary>
    public Dictionary<int, BaYeStoryEvent> storyMap = new Dictionary<int, BaYeStoryEvent>();
    /// <summary>
    /// 战令，key = 势力Id, value = 战令数量
    /// </summary>
    public Dictionary<int, int> zhanLingMap = new Dictionary<int, int>();

    private bool[] openedChest1 = new bool[5];

    public bool[] openedChest
    {
        get
        {
            if (openedChest1 == null)
            {
                openedChest1 = new bool[5];
            }

            return openedChest1;
        }
        set => openedChest1 = value;
    }
}
[Skip]
public class BaYeCityEvent
{
    public List<int> ExpList { get; set; } = new List<int>();
    public List<int> WarIds { get; set; } = new List<int>();
    public bool[] PassedStages { get; set; } = new bool[0];
    public int CityId { get; set; } = -1;
    public int EventId { get; set; } = -1;
}
[Skip]
public class BaYeStoryEvent
{
    public int StoryId { get; set; }
    public int Type { get; set; }
    public int WarId { get; set; }
    public int GoldReward { get; set; }
    public int ExpReward { get; set; }
    public int YvQueReward { get; set; }
    public int YuanBaoReward { get; set; }
    public Dictionary<int,int> ZhanLing { get; set; }
}
[Skip]
public class WarsDataClass
{
    //战役解锁进度
    public List<UnlockWarCount> warUnlockSaveData;
    public BaYeDataClass baYe = new BaYeDataClass();

    public UnlockWarCount GetCampaign(int warId)
    {
        var war = warUnlockSaveData.FirstOrDefault(w => w.warId == warId);
        if (war == null)
        {
            war = new UnlockWarCount {warId = warId};
            warUnlockSaveData.Add(war);
        }
        return war;
    }
}

#endregion

#region 游戏内容相关类
[Skip]
//宝箱卡片类
public class RewardsCardClass
{
    public int cardType;
    public int cardId;
    public int cardChips;
}

#endregion
