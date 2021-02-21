using System;
using System.Collections.Generic;
using System.Linq;
using Beebyte.Obfuscator;
using Newtonsoft.Json;

#region 玩家数据相关类

public enum SignalRDataTypes
{
    Message,
    Generic
}

public interface ISignalRField
{
    SignalRDataTypes DataTypes { get; }
    string Method { get; }
    string JData { get; }
}

/// <summary>
/// 玩家账户信息存档
/// </summary>
public interface IUserInfo
{
    /// <summary>
    /// 账号
    /// </summary>
    [SkipRename]string Username { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    [SkipRename]string Password { get; set; }
    /// <summary>
    /// 手机号
    /// </summary>
    [SkipRename]string Phone { get; set; }
    /// <summary>
    /// 硬件唯一标识id
    /// </summary>
    [SkipRename]string DeviceId { get; set; }
    /// <summary>
    /// 与服务器最后一次互交的时间
    /// </summary>
    [SkipRename]long LastUpdate { get; set; }
}


/// <summary>
/// 玩家基本信息存档数据类
/// </summary>
public interface IPlayerData
{
    //等级
    [SkipRename]int Level { get; set; }
    //经验
    [SkipRename]int Exp { get; set; }
    //元宝
    [SkipRename]int YuanBao { get; set; }
    //玉阙
    [SkipRename]int YvQue { get; set; }
    //玩家初始势力id
    [SkipRename]int ForceId { get; set; }
    //上次锦囊获取时间
    [SkipRename]long LastJinNangRedeemTime { get; set; }
    //锦囊每天的获取次数
    [SkipRename]int DailyJinNangRedemptionCount { get; set; }
    //上次酒坛获取时间
    [SkipRename]long LastJiuTanRedeemTime { get; set; }
    //酒坛每天的获取次数
    [SkipRename]int DailyJiuTanRedemptionCount { get; set; }
    //上一个游戏版本号
    [SkipRename]float LastGameVersion { get; set; }

}

/// <summary>
/// 玩家上传下载的数据规范
/// </summary>
public interface IUserSaveArchive
{
    // 账号
    [SkipRename]string Username { get; set; }
    // 密码
    [SkipRename]string Password { get; set; }
    // 手机号
    [SkipRename]string Phone { get; set; }
    // 硬件唯一标识id
    [SkipRename]string DeviceId { get; set; }
    // 与服务器最后一次互交的时间
    [SkipRename]long LastUpdate { get; set; }
    //玩家信息
    [SkipRename]string PlayerInfo { get; set; }
    /// <summary>
    ///卡牌数据 HSTDataClass
    /// </summary>
    [SkipRename]string CardsData { get; set; }
    /// <summary>
    /// 征战记录 WarsDataClass
    /// </summary>
    [SkipRename]string Expedition { get; set; }
    /// <summary>
    ///奖励或兑换码记录 GetBoxOrCodeData
    /// </summary>
    [SkipRename]string RewardsRecord { get; set; }
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
public class NowLevelAndHadChip
{
    public int id;          //id
    public int level;       //当前等级
    public int chips;       //拥有碎片
    public int isFight;     //是否出战
    public int typeIndex;   //单位类型0武将1士兵2塔3陷阱4技能
    public bool isHad;      //是否拥有过
    public int maxLevel;    //历史最高星级

    public bool IsOwned => chips > 0 || level > 0;//是否拥有
}
[Skip]
/// <summary>
/// 武将，士兵，塔等 信息存档数据类
/// </summary>
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
[Skip]
/// <summary>
/// 霸业管理类
/// </summary>
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

#region 服务器返回数据类

public class ErrorBackClass
{
    /// <summary>
    /// 查错码
    /// </summary>
    public int error { get; set; }
}


/// <summary>
/// 短信验证码类
/// </summary>
public class SMSBackContentClass
{
    /// <summary>
    /// 前置号
    /// </summary>
    public string country { get; set; }
    /// <summary>
    /// 手机号
    /// </summary>
    public string phone { get; set; }
}


/// <summary>
/// 返回账号类
/// </summary>
public class BackAccountClass
{
    /// <summary>
    /// 账户名
    /// </summary>
    public string username { get; set; }
    public string phone { get; set; }
    public long lastUpdate { get; set; }
    /// <summary>
    /// 查错码
    /// </summary>
    public int error { get; set; }
}

/// <summary>
/// 返回手机绑定账号类
/// </summary>
public class BackPhoneToAccountClass
{
    /// <summary>
    /// 手机号
    /// </summary>
    public string phone { get; set; }
    /// <summary>
    /// 账户名
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// 查错码
    /// </summary>
    public int error { get; set; }
}

/// <summary>
/// 上传存档返回数据类
/// </summary>
public class BackForUploadArchiveClass
{
    /// <summary>
    /// 时间戳
    /// </summary>
    public string lastUpdate { get; set; }
    /// <summary>
    /// 查错码
    /// </summary>
    public int error { get; set; }
}

/// <summary>
/// 服务器返回的状态码
/// </summary>
public enum ServerBackCode
{
    SUCCESS = 200,
    ERR_INVALIDOPERATION = 555,
    ERR_NAME_EXIST = 1001,
    ERR_NAME_SHORT = 1002,
    /// <summary>
    /// 密码过短
    /// </summary>
    ERR_PASS_SHORT = 1003,
    /// <summary>
    /// 账号不存在
    /// </summary>
    ERR_NAME_NOT_EXIST = 1004,
    ERR_DATA_NOT_EXIST = 1005,
    ERR_PHONE_SHORT = 1006,
    /// <summary>
    /// 重复绑定手机
    /// </summary>
    ERR_ACCOUNT_BIND_OTHER_PHONE = 1007,
    ERR_NAME_ILLEGAL = 1008,
    /// <summary>
    /// 手机号错误 
    /// </summary>
    ERR_PHONE_ILLEGAL = 1009,
    /// <summary>
    /// 密码错误
    /// </summary>
    ERR_PW_ERROR = 1010,
    /// <summary>
    /// 该手机号绑定了其他账号
    /// </summary>
    ERR_PHONE_BIND_OTHER_ACCOUNT = 1011,
    /// <summary>
    /// 已经绑定过
    /// </summary>
    ERR_PHONE_ALREADY_BINDED = 1012
}

#endregion