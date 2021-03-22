﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    /// <summary>
    /// 游戏版本号
    /// </summary>
    public float GameVersion { get; set; }
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
    //体力
    public int Stamina { get; set; }
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
    //上次领取198宝箱时间
    public long LastFourDaysChestRedeemTime { get; set; }
    //上传领取298宝箱时间
    public long LastWeekChestRedeemTime { get; set; }
    //游戏版本号
    public float LastGameVersion { get; set; }
    //战斗的时间倍率
    public float WarTimeScale { get; set; } = 1;
}

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
    public int jiBanId { get; set; }
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
