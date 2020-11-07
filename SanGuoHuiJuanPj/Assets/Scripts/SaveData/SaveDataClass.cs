using System.Collections.Generic;
using UnityEngine;

#region 玩家数据相关类

/// <summary>
/// 玩家账户信息存档
/// </summary>
public class AccountDataClass
{
    /// <summary>
    /// 账号
    /// </summary>
    public string accountName { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    public string passwordStr { get; set; }
    /// <summary>
    /// 手机号
    /// </summary>
    public string phoneNumber { get; set; }
}


/// <summary>
/// 玩家基本信息存档数据类
/// </summary>
public class PlayerDataClass
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
/// 玩家基本信息存档数据类2
/// </summary>
public class PyDataClass
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
/// 玩家基本信息存档数据类3
/// </summary>
public class PlyDataClass
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
    //public int stamina { get; set; }
    //玩家初始势力id
    public int forceId { get; set; }
    //战役宝箱
    //public List<int> fightBoxs;
    //兑换码
    //public List<RedemptionCodeGot> redemptionCodeGotList;
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

public class WarsDataClass
{
    //战役解锁进度
    public List<UnlockWarCount> warUnlockSaveData;
}

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
    public List<JiBanCardTypeClass> cardTypeLists { get; set; }
}

/// <summary>
/// 单个羁绊中卡牌小类
/// </summary>
public class JiBanCardTypeClass
{
    public int cardType { get; set; }
    public int cardId { get; set; }
    public List<FightCardData> cardLists { get; set; }
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
    public string name { get; set; }
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
    public string updateTime { get; set; }
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
    public string updateTime { get; set; }
    public int error { get; set; }
}

#endregion
