using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

#region DataContract

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

public class UserSaveArchive : IUserSaveArchive
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Phone { get; set; }
    public string DeviceId { get; set; }
    public long LastUpdate { get; set; }
    public string PlayerInfo { get; set; }
    public string CardsData { get; set; }
    public string Expedition { get; set; }
    public string RewardsRecord { get; set; }

    public UserSaveArchive(IUserInfo userInfo, IPlayerData playerData, HSTDataClass h, WarsDataClass w,
        GetBoxOrCodeData b)
    {
        var hst = new HSTDataClass
        {
            heroSaveData = h.heroSaveData.Where(c => c.IsOwned).ToList(),
            soldierSaveData = h.soldierSaveData.Where(c => c.IsOwned).ToList(),
            spellSaveData = h.spellSaveData.Where(c => c.IsOwned).ToList(),
            towerSaveData = h.towerSaveData.Where(c => c.IsOwned).ToList(),
            trapSaveData = h.trapSaveData.Where(c => c.IsOwned).ToList()
        };
        var war = new WarsDataClass
        {
            baYe = w.baYe,
            warUnlockSaveData = w.warUnlockSaveData.Where(o => o.unLockCount > 0).ToList()
        };
        var reward = new GetBoxOrCodeData
        {
            fightBoxs = b.fightBoxs,
            redemptionCodeGotList = b.redemptionCodeGotList.Where(r => r.isGot).ToList()
        };
        Username = userInfo.Username;
        Password = userInfo.Password;
        Phone = userInfo.Phone;
        DeviceId = userInfo.DeviceId;
        LastUpdate = userInfo.LastUpdate;
        PlayerInfo = Json.Serialize(playerData);
        CardsData = Json.Serialize(hst);
        Expedition = Json.Serialize(war);
        RewardsRecord = Json.Serialize(reward);
    }
}

/// <summary>
/// 玩家账户信息存档
/// </summary>
public class UserInfo : IUserInfo
{
    /// <summary>
    /// 账号
    /// </summary>
    public string Username { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; }
    /// <summary>
    /// 硬件唯一标识id
    /// </summary>
    public string DeviceId { get; set; }
    /// <summary>
    /// 与服务器最后一次互交的时间
    /// </summary>
    public long LastUpdate { get; set; }
}


/// <summary>
/// 旧玩家基本信息存档数据类
/// </summary>
public class ObsoletedPlayerData
{
    //姓名
    public string name { get; set; }
    //等级
    public int level { get; set; }
    //经验
    public int exp { get; set; }
    //元宝
    public int yuanbao { get; set; }
    //玉阙
    public int yvque { get; set; }
    //体力
    public int stamina { get; set; }
    //玩家初始势力id
    public int forceId { get; set; }
    //战役宝箱
    public List<int> fightBoxs;
    //兑换码
    public List<RedemptionCodeGot> redemptionCodeGotList;
}

/// <summary>
/// 旧玩家基本信息存档数据类2
/// </summary>
public class ObsoletedPyData
{
    //等级
    public int level { get; set; }
    //经验
    public int exp { get; set; }
    //元宝
    public int yuanbao { get; set; }
    //玉阙
    public int yvque { get; set; }
    //体力
    public int stamina { get; set; }
    //玩家初始势力id
    public int forceId { get; set; }
    //战役宝箱
    public List<int> fightBoxs;
    //兑换码
    public List<RedemptionCodeGot> redemptionCodeGotList;
}

/// <summary>
/// 玩家基本信息存档数据类
/// </summary>
public class PlayerData : IPlayerData
{
    //等级
    public int Level { get; set; } = 1;
    //经验
    public int Exp { get; set; }
    //元宝
    public int YuanBao { get; set; }
    //玉阙
    public int YvQue { get; set; }
    //玩家初始势力id
    public int ForceId { get; set; }
    //上次锦囊获取时间
    public long LastJinNangRedeemTime { get; set; }
    //锦囊每天的获取次数
    public int DailyJinNangRedemptionCount { get; set; }
    //上次酒坛获取时间
    public long LastJiuTanRedeemTime { get; set; }
    //酒坛每天的获取次数
    public int DailyJiuTanRedemptionCount { get; set; }
    //游戏版本号
    public float LastGameVersion { get; set; }

}

public class GetBoxOrCodeData
{
    //战役宝箱
    public List<int> fightBoxs;
    //兑换码
    public List<RedemptionCodeGot> redemptionCodeGotList;
}

public class RedemptionCodeGot
{
    public int id;      //兑换码id
    public bool isGot;  //是否领取过
}

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

public class UnlockWarCount
{
    public int warId;           //战役id
    public int unLockCount;     //解锁关卡数
    public bool isTakeReward;   //是否领过首通宝箱
}

/// <summary>
/// 霸业管理类
/// </summary>
public class BaYeDataClass
{
    public long lastBaYeActivityTime;
    public DateTimeOffset lastStoryEventsRefreshHour;
    [JsonIgnore]public int CurrentExp => ExpData.Values.Sum();
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

public class WarsDataClass
{
    //战役解锁进度
    public List<UnlockWarCount> warUnlockSaveData;
    public BaYeDataClass baYe = new BaYeDataClass();
}

#endregion

#region 游戏内容相关类

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
/// 上传存档数据类
/// </summary>
public class UploadArchiveToServerClass
{
    public string name { get; set; }
    public string pw { get; set; }
    public string isPhone { get; set; }
    public string data { get; set; }
    public string data2 { get; set; }
    public string data3 { get; set; }
    public string data4 { get; set; }
}

/// <summary>
/// 登录返回的数据类
/// </summary>
public class BackForLoginClass
{
    public string name { get; set; }
    public string phone { get; set; }
    public string data { get; set; }
    public string data2 { get; set; }
    public string data3 { get; set; }
    public string data4 { get; set; }
    public string lastUpdate { get; set; }
    public int error { get; set; }
}

#endregion

#endregion

#region 战斗相关类

//战斗卡牌信息类
public class FightCardData
{
    //单位id,为0表示null
    public int unitId;
    //卡牌obj
    public GameObject cardObj;
    //卡牌类型
    public int cardType;
    //卡牌id
    public int cardId;
    //等级
    public int cardGrade;
    //伤害
    public int damage;
    //满血
    public int fullHp;
    //当前血量
    public int nowHp;
    //战斗状态
    public FightState fightState;
    //摆放位置记录
    public int posIndex;
    //生命值回复
    public int hpr;
    //主被动单位
    public bool activeUnit;
    //此回合是否行动
    public bool isActed;
    //是否是玩家卡牌
    public bool isPlayerCard;
    /// <summary>
    /// 单位伤害类型0物理，1法术
    /// </summary>
    public int cardDamageType;
    /// <summary>
    /// 单位行动类型0近战，1远程
    /// </summary>
    public int cardMoveType;
    /// <summary>
    /// 被攻击者的行为，0受击，1防护盾，2闪避，3护盾，4无敌
    /// </summary>
    public int attackedBehavior;
}

//战斗状态类
public class FightState
{

    /// <summary>
    /// 眩晕回合数
    /// </summary>
    public int dizzyNums { get; set; }

    /// <summary>
    /// 护盾层数
    /// </summary>
    public int withStandNums { get; set; }

    /// <summary>
    /// 无敌回合
    /// </summary>
    public int invincibleNums { get; set; }

    /// <summary>
    /// 流血层数
    /// </summary>
    public int bleedNums { get; set; }

    /// <summary>
    /// 中毒回合
    /// </summary>
    public int poisonedNums { get; set; }

    /// <summary>
    /// 灼烧回合
    /// </summary>
    public int burnedNums { get; set; }

    /// <summary>
    /// 战意层数
    /// </summary>
    public int willFightNums { get; set; }

    /// <summary>
    /// 禁锢层数
    /// </summary>
    public int imprisonedNums { get; set; }

    /// <summary>
    /// 怯战层数
    /// </summary>
    public int cowardlyNums { get; set; }

    /// <summary>
    /// 战鼓台-伤害加成
    /// </summary>
    public int zhangutaiAddtion { get; set; }

    /// <summary>
    /// 风神台-闪避加成
    /// </summary>
    public int fengShenTaiAddtion { get; set; }

    /// <summary>
    /// 霹雳台-暴击加成
    /// </summary>
    public int pilitaiAddtion { get; set; }

    /// <summary>
    /// 狼牙台-会心加成
    /// </summary>
    public int langyataiAddtion { get; set; }

    /// <summary>
    /// 烽火台-免伤加成
    /// </summary>
    public int fenghuotaiAddtion { get; set; }

    /// <summary>
    /// 死战回合
    /// </summary>
    public int deathFightNums { get; set; }

    /// <summary>
    /// 卸甲回合
    /// </summary>
    public int removeArmorNums { get; set; }

    /// <summary>
    /// 内助回合
    /// </summary>
    public int neizhuNums { get; set; }

    /// <summary>
    /// 神助回合
    /// </summary>
    public int shenzhuNums { get; set; }

    /// <summary>
    /// 防护盾数值
    /// </summary>
    public int shieldValue { get; set; }
    
    /// <summary>
    /// 迷雾阵-远程闪避加成
    /// </summary>
    public int miWuZhenAddtion { get; set; }
}

/// <summary>
/// 羁绊判断激活类
/// </summary>
public class JiBanActivedClass
{
    public int jiBanIndex { get; set; }
    public bool isActived { get; set; }
    public bool isHadBossId { get; set; }
    public List<JiBanCardTypeClass> cardTypeLists { get; set; }
}

/// <summary>
/// 单个羁绊中卡牌小类
/// </summary>
public class JiBanCardTypeClass
{
    public int cardType { get; set; }
    public int cardId { get; set; }
    public int bossId { get; set; }
    public List<FightCardData> cardLists { get; set; }
}

/// <summary>
/// 服务器返回的状态码
/// </summary>
public enum ServerBackCode
{
    SUCCESS = 200,
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