using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightController : MonoBehaviour
{
    public static FightController instance;

    StateOfFight stateOfFight;  //战斗状态

    [HideInInspector]
    public int recordWinner;    //标记胜负-1输.0.1胜

    private int roundNums;

    [HideInInspector]
    public bool isRoundBegin;

    [HideInInspector]
    public bool isPlayerRound;

    private int fightUnitIndex; //行动单位索引

    [SerializeField]
    private float attackShakeTimeToGo;  //移动时间
    [SerializeField]
    private float attackShakeTimeToBack;  //移动时间
    [SerializeField]
    private float attackIntervalTime;   //间隔时间

    private float timerForFight;

    /// <summary>
    /// 记录攻击种类，0普通，1会心，2暴击
    /// </summary>
    public int indexAttackType;

    int targetIndex;    //目标卡牌id

    [SerializeField]
    GameObject startFightBtn;   //开战按钮

    [SerializeField]
    Transform transferStation;  //卡牌中转站

    [SerializeField]
    Toggle autoFightTog;    //自动战斗勾选控件

    bool isNeedToAttack;    //记录特殊远程兵种这次攻击是否是普攻

    [SerializeField]
    GameObject fightBackForShake;
    [SerializeField]
    float doShakeIntensity;

    public AudioSource audioSource;

    private List<FightCardData> gunMuCards; //滚木列表
    private List<FightCardData> gunShiCards;//滚石列表

    int getGold = 0;    //本场战斗获得金币数
    List<int> getBoxsList = new List<int>();    //本场战斗获得宝箱

    /// <summary>
    /// 玩家所有羁绊激活情况
    /// </summary>
    public Dictionary<int, JiBanActivedClass> playerJiBanAllTypes;
    /// <summary>
    /// 敌方所有羁绊激活情况
    /// </summary>
    public Dictionary<int, JiBanActivedClass> enemyJiBanAllTypes;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        gunMuCards = new List<FightCardData>();
        gunShiCards = new List<FightCardData>();

        playerJiBanAllTypes = new Dictionary<int, JiBanActivedClass>();
        enemyJiBanAllTypes = new Dictionary<int, JiBanActivedClass>();

        stateOfFight = StateOfFight.ReadyForFight;

        roundNums = 0;
        fightUnitIndex = 0;
        isRoundBegin = false;
        isPlayerRound = true;
        timerForFight = 0;
        targetIndex = -1;
    }

    //普攻
    IEnumerator PuTongGongji(float damageBonus, FightCardData attackUnit, FightCardData attackedUnit, bool isCanFightBack)
    {
        isNeedToAttack = true;
        //攻击的是老家
        if (attackedUnit.cardType == 522)
        {
            if (LoadJsonFile.heroTableDatas[attackUnit.cardId][5] == "28" ||
                LoadJsonFile.heroTableDatas[attackUnit.cardId][5] == "29" ||
                LoadJsonFile.heroTableDatas[attackUnit.cardId][5] == "32" ||
                LoadJsonFile.heroTableDatas[attackUnit.cardId][5] == "33") { }
            else
            {
                int cutHpNum = (int)(HeroCardMakeSomeDamages(isCanFightBack, attackUnit) * damageBonus);
                cutHpNum = FightDamageForSpecialSkill(cutHpNum, attackUnit, attackedUnit, isCanFightBack);
                if (isNeedToAttack)
                {
                    attackedUnit.nowHp -= cutHpNum;
                    AttackedAnimShow(attackedUnit, cutHpNum, false);
                    if (attackedUnit.nowHp <= 0)
                    {
                        recordWinner = attackedUnit.isPlayerCard ? -1 : 1;
                    }
                }
            }
        }
        else
        {
            //攻击的是陷阱单位
            if (attackedUnit.cardType == 3)
            {
                if (LoadJsonFile.heroTableDatas[attackUnit.cardId][5] == "28" ||
                    LoadJsonFile.heroTableDatas[attackUnit.cardId][5] == "29" ||
                    LoadJsonFile.heroTableDatas[attackUnit.cardId][5] == "32" ||
                    LoadJsonFile.heroTableDatas[attackUnit.cardId][5] == "33") { }
                else
                {
                    int finalDamage = (int)(HeroCardMakeSomeDamages(isCanFightBack, attackUnit) * damageBonus);
                    finalDamage = FightDamageForSpecialSkill(finalDamage, attackUnit, attackedUnit, isCanFightBack);
                    if (isNeedToAttack)
                    {
                        AttackTrapUnit(finalDamage, attackUnit, attackedUnit, isCanFightBack);
                    }
                }
            }
            else
            {
                //攻击的是塔单位
                if (attackedUnit.cardType == 2)
                {
                    if (LoadJsonFile.heroTableDatas[attackUnit.cardId][5] == "28" ||
                        LoadJsonFile.heroTableDatas[attackUnit.cardId][5] == "29" ||
                        LoadJsonFile.heroTableDatas[attackUnit.cardId][5] == "32" ||
                        LoadJsonFile.heroTableDatas[attackUnit.cardId][5] == "33") { }
                    else
                    {
                        int finalDamage = (int)(HeroCardMakeSomeDamages(isCanFightBack, attackUnit) * damageBonus);
                        finalDamage = FightDamageForSpecialSkill(finalDamage, attackUnit, attackedUnit, isCanFightBack);
                        if (isNeedToAttack)
                        {
                            attackedUnit.nowHp -= finalDamage;
                            AttackedAnimShow(attackedUnit, finalDamage, false);
                        }
                    }
                }
                else
                {
                    //攻击的是武将单位
                    int finalDamage = (int)(HeroCardMakeSomeDamages(isCanFightBack, attackUnit) * damageBonus);
                    finalDamage = FightDamageForSpecialSkill(finalDamage, attackUnit, attackedUnit, isCanFightBack);
                    finalDamage = DefDamageProcessFun(attackUnit, attackedUnit, finalDamage);
                    finalDamage = TieQiFenTan(finalDamage, attackedUnit);
                    if (isNeedToAttack)
                    {
                        finalDamage = AddOrCutShieldValue(finalDamage, attackedUnit, false);
                        attackedUnit.nowHp -= finalDamage;
                        AttackedAnimShow(attackedUnit, finalDamage, false);
                    }
                    yield return StartCoroutine(SpecialHeroSkill0(finalDamage, attackUnit, attackedUnit, isCanFightBack));
                }
            }
        }
        yield return new WaitForSeconds(attackIntervalTime);

        if (isCanFightBack)
        {
            yield return StartCoroutine(SpecialHeroSkill1(damageBonus, attackUnit, attackedUnit));
        }
    }

    /// <summary>
    /// 计算暴击会心等伤害加成
    /// </summary>
    /// <param name="isCanAdd"></param>
    /// <param name="fightCardData"></param>
    /// <returns></returns>
    private int HeroCardMakeSomeDamages(bool isCanAdd, FightCardData fightCardData)
    {
        List<string> heroData = LoadJsonFile.heroTableDatas[fightCardData.cardId];
        int damage = (int)(fightCardData.damage * (fightCardData.fightState.zhangutaiAddtion + 100) / 100f);
        if (isCanAdd)
        {
            switch (indexAttackType)
            {
                case 1:
                    damage = (int)(damage * float.Parse(heroData[15]) / 100f);
                    break;
                case 2:
                    damage = (int)(damage * float.Parse(heroData[13]) / 100f);
                    break;
                default:
                    break;
            }
        }
        //羁绊相关伤害
        Dictionary<int, JiBanActivedClass> jiBanAllTypes = fightCardData.isPlayerCard ? playerJiBanAllTypes : enemyJiBanAllTypes;
        //判断势力
        switch (LoadJsonFile.heroTableDatas[fightCardData.cardId][6])
        {
            //蜀国
            case "2":
                if (jiBanAllTypes[(int)JiBanSkillName.TaoYuanJieYi].isActived)
                {
                    //桃园结义激活，蜀国武将伤害提升
                    damage = (int)(damage * (LoadJsonFile.GetGameValue(148) + 100) / 100f);
                }
                break;

            default:
                break;
        }
        //判断兵系
        switch (LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[fightCardData.cardId][5])][5])
        {
            //战船系
            case "8":
                if (jiBanAllTypes[(int)JiBanSkillName.ShuiShiDouDu].isActived)
                {
                    //水师都督激活时战船系武将伤害加成50%
                    damage = (int)(damage * (LoadJsonFile.GetGameValue(160) + 100) / 100f);
                }
                break;
            //统御系
            case "11":
                if (jiBanAllTypes[(int)JiBanSkillName.WoLongFengChu].isActived)
                {
                    //卧龙凤雏激活时统御系武将伤害加成30%
                    damage = (int)(damage * (LoadJsonFile.GetGameValue(151) + 100) / 100f);
                }
                break;
            default:
                break;
        }
        //判断近战远程
        switch (fightCardData.cardMoveType)
        {
            //近战
            case 0:
                if (jiBanAllTypes[(int)JiBanSkillName.HuChiELai].isActived)
                {
                    //虎痴恶来激活时近战武将伤害加成30%
                    damage = (int)(damage * (LoadJsonFile.GetGameValue(152) + 100) / 100f);
                }
                break;
            default:
                break;
        }
        //判断物理法术
        switch (fightCardData.cardDamageType)
        {
            //物理
            case 0:
                
                break;
            //法术
            case 1:
                if (jiBanAllTypes[(int)JiBanSkillName.HanMoSanXian].isActived)
                {
                    //汉末三仙激活时法术武将伤害加成30%
                    damage = (int)(damage * (LoadJsonFile.GetGameValue(161) + 100) / 100f);
                }
                break;
            default:
                break;
        }


        if (fightCardData.cardType == 0 && fightCardData.fightState.removeArmorNums > 0)
        {
            //若攻击者有卸甲状态伤害降低30%
            damage = (int)(damage * (100 - LoadJsonFile.GetGameValue(7)) / 100f);
        }
        return damage;
    }

    ///////攻击trap单位////////////
    private void AttackTrapUnit(int damage, FightCardData attackUnit, FightCardData attackedUnit, bool isCanFightBack)
    {
        switch (attackedUnit.cardId)
        {
            case 0://拒马
                attackedUnit.nowHp -= damage;
                AttackedAnimShow(attackedUnit, damage, false);
                if (isCanFightBack && attackUnit.cardMoveType == 0 && LoadJsonFile.heroTableDatas[attackUnit.cardId][5] != "23")
                {
                    damage = DefDamageProcessFun(attackedUnit, attackUnit, damage);
                    attackUnit.nowHp -= (int)(damage * (LoadJsonFile.GetGameValue(8) / 100f));
                    GameObject effectObj = AttackToEffectShow(attackUnit, false, "7A");
                    effectObj.transform.localScale = new Vector3(1, attackedUnit.isPlayerCard ? 1 : -1, 1);
                    AttackedAnimShow(attackUnit, damage, false);
                    PlayAudioForSecondClip(89, 0.2f);
                }
                break;
            case 1://地雷
                attackedUnit.nowHp -= damage;
                AttackedAnimShow(attackedUnit, damage, false);
                if (isCanFightBack && attackUnit.cardMoveType == 0)  //踩地雷的是近战
                {
                    int dileiDamage = (int)(int.Parse(LoadJsonFile.trapTableDatas[attackedUnit.cardId][6].Split(',')[attackedUnit.cardGrade - 1]) * LoadJsonFile.GetGameValue(9) / 100f);
                    dileiDamage = DefDamageProcessFun(attackedUnit, attackUnit, dileiDamage);
                    attackUnit.nowHp -= dileiDamage;
                    AttackToEffectShow(attackUnit, false, "201A");
                    AttackedAnimShow(attackUnit, dileiDamage, false);
                    PlayAudioForSecondClip(88, 0.2f);
                }
                break;
            case 2://石墙
                attackedUnit.nowHp -= damage;
                AttackedAnimShow(attackedUnit, damage, false);
                break;
            case 3://八阵图
                attackedUnit.nowHp -= damage;
                AttackedAnimShow(attackedUnit, damage, false);
                if (attackUnit.cardMoveType == 0 && LoadJsonFile.heroTableDatas[attackUnit.cardId][5] != "23")
                {
                    TakeOneUnitDizzed(attackUnit, LoadJsonFile.GetGameValue(133));
                }
                break;
            case 4://金锁阵
                attackedUnit.nowHp -= damage;
                AttackedAnimShow(attackedUnit, damage, false);
                if (attackUnit.cardMoveType == 0 && LoadJsonFile.heroTableDatas[attackUnit.cardId][5] != "23")
                {
                    TakeToImprisoned(attackUnit, LoadJsonFile.GetGameValue(10));
                }
                break;
            case 5://鬼兵阵
                attackedUnit.nowHp -= damage;
                AttackedAnimShow(attackedUnit, damage, false);
                if (attackUnit.cardMoveType == 0 && LoadJsonFile.heroTableDatas[attackUnit.cardId][5] != "23")
                {
                    TakeToCowardly(attackUnit, LoadJsonFile.GetGameValue(11));
                }
                break;
            case 6://火墙
                attackedUnit.nowHp -= damage;
                AttackedAnimShow(attackedUnit, damage, false);
                if (isCanFightBack && attackUnit.cardMoveType == 0 && LoadJsonFile.heroTableDatas[attackUnit.cardId][5] != "23")
                {
                    TakeToBurn(attackUnit, LoadJsonFile.GetGameValue(12));
                }
                break;
            case 7://毒泉
                attackedUnit.nowHp -= damage;
                AttackedAnimShow(attackedUnit, damage, false);
                if (isCanFightBack && attackUnit.cardMoveType == 0 && LoadJsonFile.heroTableDatas[attackUnit.cardId][5] != "23")
                {
                    TakeToPoisoned(attackUnit, LoadJsonFile.GetGameValue(13));
                }
                break;
            case 8://刀墙
                attackedUnit.nowHp -= damage;
                AttackedAnimShow(attackedUnit, damage, false);
                if (isCanFightBack && attackUnit.cardMoveType == 0 && LoadJsonFile.heroTableDatas[attackUnit.cardId][5] != "23")
                {
                    TakeToBleed(attackUnit, LoadJsonFile.GetGameValue(14));
                }
                break;
            case 9://滚石
                attackedUnit.nowHp -= damage;
                AttackedAnimShow(attackedUnit, damage, false);
                break;
            case 10://滚木
                attackedUnit.nowHp -= damage;
                AttackedAnimShow(attackedUnit, damage, false);
                break;
            case 11://金币宝箱
                if (attackedUnit.nowHp > 0)
                {
                    attackedUnit.nowHp -= damage;
                    GetGoldBoxFun(attackedUnit);
                }
                AttackedAnimShow(attackedUnit, damage, false);
                break;
            case 12://宝箱
                if (attackedUnit.nowHp > 0)
                {
                    attackedUnit.nowHp -= damage;
                    if (attackedUnit.nowHp <= 0)
                    {
                        //GameObject obj = EffectsPoolingControl.instance.GetEffectToFight("GetGold", 1.5f, attackedUnit.cardObj.transform);
                        //obj.GetComponentInChildren<Text>().text = string.Format(LoadJsonFile.GetStringText(8), LoadJsonFile.enemyUnitTableDatas[attackedUnit.unitId][4]);
                        PlayAudioForSecondClip(98, 0);
                    }
                }
                AttackedAnimShow(attackedUnit, damage, false);
                break;
            default:
                break;
        }
    }

    #region 陷阱单位特殊方法
    //获得金币宝箱
    private void GetGoldBoxFun(FightCardData attackedUnit)
    {
        if (attackedUnit.nowHp <= 0)
        {
            GameObject obj = EffectsPoolingControl.instance.GetEffectToFight("GetGold", 1.5f, attackedUnit.cardObj.transform);
            obj.GetComponentInChildren<Text>().text = string.Format(LoadJsonFile.GetStringText(8), LoadJsonFile.enemyUnitTableDatas[attackedUnit.unitId][4]);
            PlayAudioForSecondClip(98, 0);
        }
    }

    IEnumerator GunMuGunShiSkill(List<FightCardData> gunMuList, List<FightCardData> gunShiList)
    {
        for (int i = 0; i < gunMuList.Count; i++)
        {
            if (gunMuList[i].nowHp <= 0 && !gunMuList[i].isActed)
            {
                yield return StartCoroutine(GunMuTrapAttack(gunMuList[i], 1f, LoadJsonFile.GetGameValue(15)));
            }
        }

        for (int i = 0; i < gunShiList.Count; i++)
        {
            if (gunShiList[i].nowHp <= 0 && !gunShiList[i].isActed)
            {
                yield return StartCoroutine(GunShiTrapAttack(gunShiList[i], 1f, LoadJsonFile.GetGameValue(16)));
            }
        }
    }

    //滚木反击
    IEnumerator GunMuTrapAttack(FightCardData attackUnit, float damageRate, int dizzedRate)
    {
        attackUnit.isActed = true;

        yield return new WaitForSeconds(attackShakeTimeToGo / 2);

        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        PlayAudioForSecondClip(95, 0);
        MoveToFightWay0(attackUnit, attackShakeTimeToGo / 2);
        yield return new WaitForSeconds(attackShakeTimeToGo);

        List<FightCardData> newGunMuList = new List<FightCardData>();
        List<FightCardData> newGunShiList = new List<FightCardData>();

        List<int> fightColumns = new List<int>() { 0, 1, 2, 3, 4 };  //记录会攻击到的列
        int cutHpNum = (int)(attackUnit.damage * damageRate);
        for (int i = 0; i < fightColumns.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int cardIndex = fightColumns[i] + j * 5;
                if (fightCardDatas[cardIndex] != null && fightCardDatas[cardIndex].nowHp > 0)
                {
                    int nowDamage = DefDamageProcessFun(attackUnit, fightCardDatas[cardIndex], cutHpNum);
                    if (fightCardDatas[cardIndex].cardType == 3)    //滚石和滚木，对陷阱造成2倍伤害
                    {
                        nowDamage = nowDamage * 2;
                    }
                    if (cardIndex != 17)
                    {
                        fightCardDatas[cardIndex].nowHp -= nowDamage;
                        TakeOneUnitDizzed(fightCardDatas[cardIndex], dizzedRate);
                    }
                    else
                    {
                        fightCardDatas[cardIndex].nowHp -= nowDamage;
                        if (fightCardDatas[cardIndex].nowHp <= 0)
                        {
                            recordWinner = attackUnit.isPlayerCard ? 1 : -1;
                        }
                    }
                    AttackToEffectShow(fightCardDatas[cardIndex], false, "209A");
                    ShowSpellTextObj(fightCardDatas[cardIndex].cardObj, LoadJsonFile.GetStringText(9), true, true);
                    AttackedAnimShow(fightCardDatas[cardIndex], nowDamage, false);

                    if (fightCardDatas[cardIndex].nowHp <= 0 &&
                        fightCardDatas[cardIndex].cardType == 3 &&
                        (fightCardDatas[cardIndex].cardId == 9 || fightCardDatas[cardIndex].cardId == 10))
                    {
                        if (fightCardDatas[cardIndex].cardId == 9)
                        {
                            newGunShiList.Add(fightCardDatas[cardIndex]);
                        }
                        else
                        {
                            newGunMuList.Add(fightCardDatas[cardIndex]);
                        }
                    }
                    break;
                }
            }
        }

        if (newGunMuList.Count > 0 || newGunShiList.Count > 0)
        {
            yield return StartCoroutine(GunMuGunShiSkill(newGunMuList, newGunShiList));
        }

        yield return new WaitForSeconds(attackShakeTimeToGo);
    }

    //滚石反击
    IEnumerator GunShiTrapAttack(FightCardData attackUnit, float damageRate, int dizzedRate)
    {
        attackUnit.isActed = true;

        yield return new WaitForSeconds(attackShakeTimeToGo / 2);

        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        PlayAudioForSecondClip(94, 0);
        MoveToFightWay0(attackUnit, attackShakeTimeToGo / 2);
        yield return new WaitForSeconds(attackShakeTimeToGo);

        List<FightCardData> newGunMuList = new List<FightCardData>();
        List<FightCardData> newGunShiList = new List<FightCardData>();

        int startIndex = attackUnit.posIndex % 5;
        int cutHpNum = (int)(attackUnit.damage * damageRate);
        for (int i = 0; i < 4; i++)
        {
            int cardIndex = startIndex + i * 5;
            if (fightCardDatas[cardIndex] != null && fightCardDatas[cardIndex].nowHp > 0)
            {
                int nowDamage = DefDamageProcessFun(attackUnit, fightCardDatas[cardIndex], cutHpNum);
                if (fightCardDatas[cardIndex].cardType == 3)    //滚石和滚木，对陷阱造成2倍伤害
                {
                    nowDamage = nowDamage * 2;
                }
                if (cardIndex != 17)
                {
                    fightCardDatas[cardIndex].nowHp -= nowDamage;
                    TakeOneUnitDizzed(fightCardDatas[cardIndex], dizzedRate);
                }
                else
                {
                    fightCardDatas[cardIndex].nowHp -= nowDamage;
                    if (fightCardDatas[cardIndex].nowHp <= 0)
                    {
                        recordWinner = attackUnit.isPlayerCard ? 1 : -1;
                    }
                }
                AttackToEffectShow(fightCardDatas[cardIndex], false, "209A");
                ShowSpellTextObj(fightCardDatas[cardIndex].cardObj, LoadJsonFile.GetStringText(10), true, true);
                AttackedAnimShow(fightCardDatas[cardIndex], nowDamage, false);

                if (fightCardDatas[cardIndex].nowHp <= 0 &&
                        fightCardDatas[cardIndex].cardType == 3 &&
                        (fightCardDatas[cardIndex].cardId == 9 || fightCardDatas[cardIndex].cardId == 10))
                {
                    if (fightCardDatas[cardIndex].cardId == 9)
                    {
                        newGunShiList.Add(fightCardDatas[cardIndex]);
                    }
                    else
                    {
                        newGunMuList.Add(fightCardDatas[cardIndex]);
                    }
                }
            }
        }

        if (newGunMuList.Count > 0 || newGunShiList.Count > 0)
        {
            yield return StartCoroutine(GunMuGunShiSkill(newGunMuList, newGunShiList));
        }

        yield return new WaitForSeconds(attackShakeTimeToGo);
    }

    //箭楼远射技能(塔)
    public void JianLouYuanSheSkill(FightCardData attackUnit, int finalDamage)
    {
        int damage = (int)(LoadJsonFile.GetGameValue(17) / 100f * finalDamage);
        PlayAudioForSecondClip(20, 0);

        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        List<int> canFightUnits = new List<int>();
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            if (fightCardDatas[i] != null && fightCardDatas[i].cardType == 0 && fightCardDatas[i].nowHp > 0)
            {
                canFightUnits.Add(i);
            }
        }
        List<int> attackedIndexList = BackRandsList(canFightUnits, LoadJsonFile.GetGameValue(18));
        for (int i = 0; i < attackedIndexList.Count; i++)
        {
            FightCardData attackedUnit = fightCardDatas[attackedIndexList[i]];
            AttackToEffectShow(attackedUnit, false, "20A");

            int nowDamage = DefDamageProcessFun(attackUnit, attackedUnit, damage);
            attackedUnit.nowHp -= nowDamage;
            AttackedAnimShow(attackedUnit, nowDamage, false);
        }
    }

    #endregion

    ///////造成伤害时触发的特殊技能///////////

    private int FightDamageForSpecialSkill(int finalDamage, FightCardData attackUnit, FightCardData attackedUnit, bool isCanFightBack)
    {
        if (isCanFightBack)
        {
            if (attackUnit.fightState.imprisonedNums <= 0) //攻击者没有禁锢状态
            {
                switch (LoadJsonFile.heroTableDatas[attackUnit.cardId][5])
                {
                    case "3":
                        PlayAudioForSecondClip(3, 0);
                        AttackToEffectShow(attackedUnit, false, "3A");
                        break;
                    case "4":
                        ShowSpellTextObj(attackUnit.cardObj, "4", false);
                        AttackToEffectShow(attackUnit, false, "4A");
                        if (attackUnit.fightState.withStandNums <= 0)
                        {
                            FightForManager.instance.CreateSateIcon(attackUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_withStand, true);
                        }
                        attackUnit.fightState.withStandNums++;
                        PlayAudioForSecondClip(4, 0);
                        break;
                    case "6":
                        AttackToEffectShow(attackedUnit, false, "6A");
                        PlayAudioForSecondClip(6, 0);
                        break;
                    case "8":
                        XiangBingTrampleAttAck(attackedUnit, attackUnit);
                        break;
                    case "9":
                        AttackToEffectShow(attackedUnit, false, "9A");
                        PlayAudioForSecondClip(9, 0);
                        break;
                    case "60":
                        AttackToEffectShow(attackedUnit, false, "60A");
                        PlayAudioForSecondClip(9, 0);
                        break;
                    case "10":
                        finalDamage = SiShiSheMingAttack(finalDamage, attackUnit, attackedUnit);
                        break;
                    case "11":
                        finalDamage = TieQiWuWeiAttack(finalDamage, attackUnit, attackedUnit);
                        break;
                    case "12":
                        finalDamage = ShenWuZhanYiAttack(finalDamage, attackUnit, attackedUnit);
                        break;
                    case "59":
                        QiangBingChuanCiAttack(finalDamage, attackUnit, attackedUnit, 59);
                        break;
                    case "14":
                        QiangBingChuanCiAttack(finalDamage, attackUnit, attackedUnit, 14);
                        break;
                    case "15":
                        JianBingHengSaoAttack(finalDamage, attackUnit, attackedUnit);
                        break;
                    case "16":
                        AttackToEffectShow(attackedUnit, false, "16A");
                        PlayAudioForSecondClip(16, 0);
                        break;
                    case "17":
                        AttackToEffectShow(attackedUnit, false, "17A");
                        PlayAudioForSecondClip(17, 0);
                        break;
                    case "18":
                        finalDamage = FuBingTuLuAttack(finalDamage, attackedUnit, attackUnit);
                        break;
                    case "19":
                        AttackToEffectShow(attackedUnit, false, "19A");
                        PlayAudioForSecondClip(19, 0);
                        break;
                    case "51":
                        AttackToEffectShow(attackedUnit, false, "19A");
                        PlayAudioForSecondClip(19, 0);
                        break;
                    case "20":
                        GongBingYuanSheSkill(finalDamage, attackUnit, attackedUnit, 20);
                        break;
                    case "52":
                        GongBingYuanSheSkill(finalDamage, attackUnit, attackedUnit, 52);
                        break;
                    case "21":
                        finalDamage = ZhanChuanChongJiAttack(finalDamage, attackedUnit, attackUnit);
                        break;
                    case "22":
                        finalDamage = ZhanCheZhuangYaAttack(finalDamage, attackedUnit, attackUnit);
                        break;
                    case "23":
                        finalDamage = GongChengChePoCheng(finalDamage, attackUnit, attackedUnit);
                        break;
                    case "24":
                        TouShiCheSkill(finalDamage, attackUnit);
                        break;
                    case "25":
                        CiKePoJiaAttack(attackedUnit, attackUnit);
                        break;
                    case "26":
                        JunShiSkill(attackUnit, 26, finalDamage);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "27":
                        JunShiSkill(attackUnit, 27, finalDamage);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "28":
                        isNeedToAttack = false;
                        break;
                    case "29":
                        isNeedToAttack = false;
                        break;
                    case "30":
                        DuShiSkill(LoadJsonFile.GetGameValue(19), attackUnit, 30, finalDamage);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "31":
                        DuShiSkill(LoadJsonFile.GetGameValue(20), attackUnit, 31, finalDamage);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "32":
                        isNeedToAttack = false;
                        break;
                    case "33":
                        isNeedToAttack = false;
                        break;
                    case "34":
                        BianShiSkill(LoadJsonFile.GetGameValue(21), attackUnit, 34);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "35":
                        BianShiSkill(LoadJsonFile.GetGameValue(22), attackUnit, 35);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "36":
                        MouShiSkill(LoadJsonFile.GetGameValue(23), attackUnit, 36);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "37":
                        MouShiSkill(LoadJsonFile.GetGameValue(24), attackUnit, 37);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "38":
                        NeiZhengSkill(attackUnit);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "39":
                        FuZuoBiHuSkill(finalDamage, attackUnit);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "40":
                        QiXieXiuFu(attackUnit);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "42":
                        YiShengSkill(LoadJsonFile.GetGameValue(25), attackUnit, 42);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "43":
                        YiShengSkill(LoadJsonFile.GetGameValue(26), attackUnit, 43);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "44":
                        ShuiBingXieJia(attackUnit, attackedUnit);
                        break;
                    case "45":
                        MeiRenJiNeng(attackUnit, 45);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "46":
                        MeiRenJiNeng(attackUnit, 46);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "47":
                        ShuiKeSkill(LoadJsonFile.GetGameValue(27), attackUnit, 47);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "48":
                        ShuiKeSkill(LoadJsonFile.GetGameValue(28), attackUnit, 48);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "49":
                        PlayAudioForSecondClip(49, 0);
                        AttackToEffectShow(attackedUnit, false, "49A");
                        break;
                    case "50":
                        PlayAudioForSecondClip(50, 0);
                        AttackToEffectShow(attackedUnit, false, "50A");
                        break;
                    case "53":
                        YinShiSkill(LoadJsonFile.GetGameValue(29), attackUnit, 53, finalDamage);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "54":
                        YinShiSkill(LoadJsonFile.GetGameValue(30), attackUnit, 54, finalDamage);
                        SpecilSkillNeedPuGongFun(attackedUnit);
                        break;
                    case "55":
                        HuoChuanSkill(finalDamage, attackUnit, attackedUnit);
                        break;
                    case "56":
                        ManZuSkill(attackUnit, attackedUnit);
                        break;
                    case "57":
                        PlayAudioForSecondClip(57, 0);
                        AttackToEffectShow(attackedUnit, true);
                        break;
                    case "58":
                        finalDamage = TieQiSkill(finalDamage, attackUnit, attackedUnit);
                        break;
                    case "65":
                        finalDamage = HuangJinSkill(finalDamage, attackUnit, attackedUnit);
                        break;
                    default:
                        //isNeedToAttack = false;
                        PlayAudioForSecondClip(0, 0);
                        AttackToEffectShow(attackedUnit, true);
                        break;
                }
            }
            else
            {
                PlayAudioForSecondClip(0, 0);
                ShowSpellTextObj(attackUnit.cardObj, LoadJsonFile.GetStringText(11), true, true);
            }
        }

        if (attackedUnit.cardType == 0)
        {
            switch (LoadJsonFile.heroTableDatas[attackedUnit.cardId][5])
            {
                case "7":
                    if (isCanFightBack)
                    {
                        CiJiaFanShangAttack(finalDamage, attackUnit, attackedUnit);
                    }
                    break;
                default:
                    break;
            }
        }
        return finalDamage;
    }

    //特殊单位的技能行动0
    IEnumerator SpecialHeroSkill0(int finalDamage, FightCardData attackUnit, FightCardData attackedUnit, bool isCanFightBack)
    {
        if (isCanFightBack)
        {
            if (attackedUnit.fightState.dizzyNums <= 0 && attackedUnit.fightState.imprisonedNums <= 0)
            {
                //禁卫
                if (LoadJsonFile.heroTableDatas[attackedUnit.cardId][5] == "13" && attackUnit.cardMoveType == 0)
                {
                    yield return StartCoroutine(JinWeiFanJiAttack(attackedUnit, attackUnit));
                }
            }
        }
    }

    //特殊单位的技能行动1
    IEnumerator SpecialHeroSkill1(float damageBonus, FightCardData attackUnit, FightCardData attackedUnit)
    {
        yield return StartCoroutine(GunMuGunShiSkill(gunMuCards, gunShiCards));

        if (attackUnit.fightState.imprisonedNums > 0) //禁锢，不进行技能攻击
        {
            attackUnit.fightState.imprisonedNums--;
            ShowSpellTextObj(attackUnit.cardObj, LoadJsonFile.GetStringText(11), true, true);
            if (attackUnit.fightState.imprisonedNums <= 0)
            {
                attackUnit.fightState.imprisonedNums = 0;
                FightForManager.instance.DestroySateIcon(attackUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_imprisoned, true);
            }
        }
        else
        {
            switch (LoadJsonFile.heroTableDatas[attackUnit.cardId][5])
            {
                case "9":
                    yield return StartCoroutine(XianFengYongWu(attackUnit, attackedUnit, 9));
                    break;
                case "60":
                    yield return StartCoroutine(XianFengYongWu(attackUnit, attackedUnit, 60));
                    break;
                case "16":
                    yield return StartCoroutine(QiBingChiCheng(attackUnit, attackedUnit));
                    break;
                case "17":
                    yield return StartCoroutine(DaoBingLianZhan(damageBonus, attackUnit, attackedUnit));
                    break;
                case "19":
                    yield return StartCoroutine(NuBingLianShe(attackUnit, attackedUnit, 19));
                    break;
                case "51":
                    yield return StartCoroutine(NuBingLianShe(attackUnit, attackedUnit, 51));
                    break;
                case "28":
                    yield return StartCoroutine(ShuShiLuoLei(LoadJsonFile.GetGameValue(31), attackUnit, 28));
                    break;
                case "29":
                    yield return StartCoroutine(ShuShiLuoLei(LoadJsonFile.GetGameValue(32), attackUnit, 29));
                    break;
                case "32":
                    yield return StartCoroutine(TongShuaiSkill(attackUnit, 32));
                    break;
                case "33":
                    yield return StartCoroutine(TongShuaiSkill(attackUnit, 33));
                    break;
                case "12":
                    yield return StartCoroutine(ShenWuZhanYi(damageBonus, attackUnit, attackedUnit));
                    break;
                default:
                    break;
            }
        }

        yield return StartCoroutine(GunMuGunShiSkill(gunMuCards, gunShiCards));
        //消除滚石滚木
        for (int i = 0; i < gunMuCards.Count; i++)
        {
            if (gunMuCards[i].nowHp <= 0)
            {
                gunMuCards.Remove(gunMuCards[i]);
            }
        }
        for (int i = 0; i < gunShiCards.Count; i++)
        {
            if (gunShiCards[i].nowHp <= 0)
            {
                gunShiCards.Remove(gunShiCards[i]);
            }
        }
    }

    #region 英雄特殊技能方法

    private int tongShuaiBurnRoundPy = -1;
    private int tongShuaiBurnRoundEm = -1;

    //统帅回合攻击目标
    int[][] GoalGfSetFireRound = new int[3][] {
        new int[1] { 7},
        new int[6] { 2, 5, 6,10,11,12},
        new int[11]{ 0, 1, 3, 4, 8, 9,13,14,15,16,17},
    };
    //统帅野火技能
    IEnumerator TongShuaiSkill(FightCardData attackUnit, int classType)
    {
        FightCardData[] fightCardDatas;
        List<GameObject> posListToSetBurn;

        int burnRoundIndex = 0; //记录烧到第几圈了
        if (attackUnit.isPlayerCard)
        {
            burnRoundIndex = tongShuaiBurnRoundPy;
            fightCardDatas = FightForManager.instance.enemyFightCardsDatas;
            posListToSetBurn = FightForManager.instance.enemyCardsPos;
        }
        else
        {
            burnRoundIndex = tongShuaiBurnRoundEm;
            fightCardDatas = FightForManager.instance.playerFightCardsDatas;
            posListToSetBurn = FightForManager.instance.playerCardsPos;
        }

        //尝试消除上一圈火焰,灼烧之前回合烧过的地方
        if (burnRoundIndex != -1)
        {
            for (int i = 0; i < GoalGfSetFireRound[burnRoundIndex].Length; i++)
            {
                if (fightCardDatas[GoalGfSetFireRound[burnRoundIndex][i]] != null && fightCardDatas[GoalGfSetFireRound[burnRoundIndex][i]].cardType == 0 && fightCardDatas[GoalGfSetFireRound[burnRoundIndex][i]].nowHp > 0)
                {
                    TakeToBurn(fightCardDatas[GoalGfSetFireRound[burnRoundIndex][i]], LoadJsonFile.GetGameValue(33), attackUnit);
                }
                Transform obj = posListToSetBurn[GoalGfSetFireRound[burnRoundIndex][i]].transform.Find(StringNameStatic.StateIconPath_burned);
                if (obj != null)
                    Destroy(obj.gameObject);
            }
        }

        burnRoundIndex++;
        if (burnRoundIndex == 3)
            burnRoundIndex = 0;
        if (attackUnit.isPlayerCard)
        {
            tongShuaiBurnRoundPy = burnRoundIndex;
        }
        else
        {
            tongShuaiBurnRoundEm = burnRoundIndex;
        }

        ShowSpellTextObj(attackUnit.cardObj, classType.ToString(), false);
        PlayAudioForSecondClip(32, 0);

        int[] targets = GoalGfSetFireRound[burnRoundIndex];

        string effectStr = classType + "A";

        int damage = HeroCardMakeSomeDamages(true, attackUnit);
        damage = (int)(damage * (1 - burnRoundIndex * LoadJsonFile.GetGameValue(34) / 100f));  //伤害递减

        for (int i = 0; i < targets.Length; i++)
        {
            EffectsPoolingControl.instance.GetEffectToFight1(effectStr, 1f, posListToSetBurn[targets[i]].transform);

            GameObject stateDinObj = Instantiate(Resources.Load("Prefabs/stateDin/" + StringNameStatic.StateIconPath_burned, typeof(GameObject)) as GameObject, posListToSetBurn[targets[i]].transform);
            stateDinObj.name = StringNameStatic.StateIconPath_burned;

            if (fightCardDatas[targets[i]] != null && fightCardDatas[targets[i]].nowHp > 0)
            {
                int nowDamage = DefDamageProcessFun(attackUnit, fightCardDatas[targets[i]], damage);
                fightCardDatas[targets[i]].nowHp -= nowDamage;
                AttackedAnimShow(fightCardDatas[targets[i]], nowDamage, false);
                if (fightCardDatas[targets[i]].cardType == 522)
                {
                    if (fightCardDatas[targets[i]].nowHp <= 0)
                    {
                        recordWinner = fightCardDatas[targets[i]].isPlayerCard ? -1 : 1;
                    }
                }
                else
                {
                    if (fightCardDatas[targets[i]].cardType == 0)
                    {
                        TakeToBurn(fightCardDatas[targets[i]], classType == 32 ? LoadJsonFile.GetGameValue(35) : LoadJsonFile.GetGameValue(36), attackUnit);
                    }
                }
            }
        }
        yield return new WaitForSeconds(attackShakeTimeToGo);
    }

    [SerializeField]
    GameObject thunderCloudObj;

    //术士落雷技能
    IEnumerator ShuShiLuoLei(int fightNums, FightCardData attackUnit, int classType)
    {
        ShowSpellTextObj(attackUnit.cardObj, classType.ToString(), false);
        PlayAudioForSecondClip(29, 0);

        thunderCloudObj.SetActive(true);
        thunderCloudObj.GetComponent<Image>().DOColor(new Color(0, 0, 0, 175f / 255f), 1f);
        yield return new WaitForSeconds(yuanChengShakeTimeToGo);

        for (int i = 0; i < fightNums; i++)
        {
            PlayAudioForSecondClip(28, 0);

            yield return new WaitForSeconds(attackShakeTimeToBack);
            bool isGameOver = ShuShiTakeThunder(attackUnit, classType);
            yield return new WaitForSeconds(attackShakeTimeToGo);
            thunderCloudObj.transform.GetChild(0).gameObject.SetActive(false);

            if (isGameOver)
            {
                break;
            }
            if (i < fightNums - 1)
            {
                float waitTime = BeforeFightDoThingFun(attackUnit);
                yield return new WaitForSeconds(waitTime);
            }

            yield return StartCoroutine(GunMuGunShiSkill(gunMuCards, gunShiCards));
        }
        thunderCloudObj.GetComponent<Image>().DOColor(new Color(0, 0, 0, 0), 0.5f);
        thunderCloudObj.SetActive(false);
    }
    //召唤落雷
    private bool ShuShiTakeThunder(FightCardData attackUnit, int classType)
    {
        string effectStr = classType + "A";
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        List<GameObject> posListToThunder = attackUnit.isPlayerCard ? FightForManager.instance.enemyCardsPos : FightForManager.instance.playerCardsPos;
        List<int> canFightUnits = new List<int>();
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            canFightUnits.Add(i);
        }
        int damage = (int)(HeroCardMakeSomeDamages(true, attackUnit) * LoadJsonFile.GetGameValue(37) / 100f);
        List<int> attackedIndexList = BackRandsList(canFightUnits, classType == 28 ? Random.Range(1, LoadJsonFile.GetGameValue(38)) : Random.Range(1, LoadJsonFile.GetGameValue(39)));

        thunderCloudObj.transform.GetChild(0).gameObject.SetActive(true);

        for (int i = 0; i < attackedIndexList.Count; i++)
        {
            EffectsPoolingControl.instance.GetEffectToFight1(effectStr, 1f, posListToThunder[attackedIndexList[i]].transform);
            if (fightCardDatas[attackedIndexList[i]] != null && fightCardDatas[attackedIndexList[i]].nowHp > 0)
            {
                int nowDamage = DefDamageProcessFun(attackUnit, fightCardDatas[attackedIndexList[i]], damage);
                fightCardDatas[attackedIndexList[i]].nowHp -= nowDamage;
                AttackedAnimShow(fightCardDatas[attackedIndexList[i]], nowDamage, false);
                if (fightCardDatas[attackedIndexList[i]].cardType == 522)
                {
                    if (fightCardDatas[attackedIndexList[i]].nowHp <= 0)
                    {
                        recordWinner = fightCardDatas[attackedIndexList[i]].isPlayerCard ? -1 : 1;
                        return true;
                    }
                }
                else
                {
                    TakeOneUnitDizzed(fightCardDatas[attackedIndexList[i]], LoadJsonFile.GetGameValue(40), attackUnit);
                }
            }
        }
        return false;
    }

    //刀兵连斩技能
    IEnumerator DaoBingLianZhan(float damageBonus, FightCardData attackUnit, FightCardData attackedUnit)
    {
        if (attackUnit.nowHp <= 0 || attackUnit.fightState.dizzyNums > 0 || attackUnit.fightState.imprisonedNums > 0)
        {

        }
        else
        {
            if (attackedUnit.nowHp <= 0 && attackedUnit.cardType != 522)
            {
                //Debug.Log("-----刀兵连斩");
                float waitTime = BeforeFightDoThingFun(attackUnit);
                yield return new WaitForSeconds(waitTime);
                ShowSpellTextObj(attackUnit.cardObj, "17", false);

                targetIndex = FindOpponentIndex(attackUnit);  //锁定目标卡牌
                MoveToFightWay1(attackUnit);
                yield return new WaitForSeconds(attackShakeTimeToGo);

                FightCardData nextAttackedUnit = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas[targetIndex] : FightForManager.instance.playerFightCardsDatas[targetIndex];
                AttackToEffectShow(nextAttackedUnit, false, "17A");
                PlayAudioForSecondClip(17, 0);

                yield return StartCoroutine(PuTongGongji(damageBonus + LoadJsonFile.GetGameValue(41) / 100f, attackUnit, nextAttackedUnit, true));
            }
        }
    }

    //神武战意技能
    IEnumerator ShenWuZhanYi(float damageBonus, FightCardData attackUnit, FightCardData attackedUnit)
    {
        if (attackUnit.nowHp <= 0 || attackUnit.fightState.dizzyNums > 0 || attackUnit.fightState.imprisonedNums > 0)
        {

        }
        else
        {
            if (attackedUnit.nowHp > 0 && attackedUnit.cardType == 0)
            {
                if (attackedUnit.attackedBehavior == 2 || attackedUnit.attackedBehavior == 3)
                {
                    float waitTime = BeforeFightDoThingFun(attackUnit);
                    yield return new WaitForSeconds(waitTime);
                    ShowSpellTextObj(attackUnit.cardObj, "12", false);
                    MoveToFightWay1(attackUnit);
                    yield return new WaitForSeconds(attackShakeTimeToGo);
                    AttackToEffectShow(attackedUnit, false, "12A");
                    PlayAudioForSecondClip(12, 0);

                    float propAttack = 1 + LoadJsonFile.GetGameValue(97) / 100f * attackUnit.fightState.willFightNums;

                    yield return StartCoroutine(PuTongGongji(propAttack, attackUnit, attackedUnit, true));
                }
            }
        }
    }

    //先锋勇武技能
    IEnumerator XianFengYongWu(FightCardData attackUnit, FightCardData attackedUnit, int classType)
    {
        if (attackUnit.nowHp <= 0 || attackUnit.fightState.dizzyNums > 0 || attackUnit.fightState.imprisonedNums > 0)
        {

        }
        else
        {
            int propNums = 0;   //特殊技能触发概率
            int attackNums = 0; //攻击次数
            if (classType == 9)
            {
                propNums = LoadJsonFile.GetGameValue(42);
                attackNums = LoadJsonFile.GetGameValue(43);
            }
            else
            {
                propNums = LoadJsonFile.GetGameValue(44);
                attackNums = LoadJsonFile.GetGameValue(45);
            }

            if (TakeSpecialAttack(propNums))
            {
                for (int i = 0; i < attackNums; i++)
                {
                    float waitTime = BeforeFightDoThingFun(attackUnit);
                    yield return new WaitForSeconds(waitTime);
                    ShowSpellTextObj(attackUnit.cardObj, "9", false);
                    MoveToFightWay1(attackUnit);
                    yield return new WaitForSeconds(attackShakeTimeToGo);
                    AttackToEffectShow(attackedUnit, false, classType + "A");
                    PlayAudioForSecondClip(9, 0);
                    //TakeToCowardly(attackedUnit, LoadJsonFile.GetGameValue(46));
                    yield return StartCoroutine(PuTongGongji(1f, attackUnit, attackedUnit, false));
                }
            }
        }
    }

    //骑兵驰骋技能
    IEnumerator QiBingChiCheng(FightCardData attackUnit, FightCardData attackedUnit)
    {
        if (attackUnit.nowHp <= 0 || attackUnit.fightState.dizzyNums > 0 || attackUnit.fightState.imprisonedNums > 0)
        {

        }
        else
        {
            if (attackedUnit.nowHp > 0 && (indexAttackType != 0 || TakeSpecialAttack(LoadJsonFile.GetGameValue(47))))
            {
                //Debug.Log("-----骑兵驰骋");
                float waitTime = BeforeFightDoThingFun(attackUnit);
                yield return new WaitForSeconds(waitTime);
                ShowSpellTextObj(attackUnit.cardObj, "16", false);
                MoveToFightWay1(attackUnit);
                yield return new WaitForSeconds(attackShakeTimeToGo);
                AttackToEffectShow(attackedUnit, false, "16A");
                PlayAudioForSecondClip(16, 0);

                yield return StartCoroutine(PuTongGongji(1f, attackUnit, attackedUnit, true));
            }
        }
    }

    //弩兵连射技能
    IEnumerator NuBingLianShe(FightCardData attackUnit, FightCardData attackedUnit, int classIndex)
    {
        if (attackedUnit.nowHp > 0 && TakeSpecialAttack(classIndex == 51 ? LoadJsonFile.GetGameValue(48) : LoadJsonFile.GetGameValue(49)))
        {
            //Debug.Log("-----弩兵连射");
            float waitTime = BeforeFightDoThingFun(attackUnit);
            yield return new WaitForSeconds(waitTime);
            MoveToFightWay0(attackUnit, yuanChengShakeTimeToGo);
            yield return new WaitForSeconds(yuanChengShakeTimeToGo / 2);

            ShowSpellTextObj(attackUnit.cardObj, "19", false);
            AttackToEffectShow(attackedUnit, false, "19A");
            PlayAudioForSecondClip(19, 0);
            //连射
            yield return StartCoroutine(PuTongGongji(1, attackUnit, attackedUnit, false));

            if (classIndex == 51)//强弩攻击第三次
            {
                yield return new WaitForSeconds(waitTime);
                MoveToFightWay0(attackUnit, yuanChengShakeTimeToGo);
                yield return new WaitForSeconds(yuanChengShakeTimeToGo / 2);
                AttackToEffectShow(attackedUnit, false, "19A");
                PlayAudioForSecondClip(19, 0);
                //连射
                yield return StartCoroutine(PuTongGongji(1, attackUnit, attackedUnit, false));
            }
        }
    }

    //禁卫反击技能
    IEnumerator JinWeiFanJiAttack(FightCardData attackUnit, FightCardData attackedUnit)
    {
        if (attackUnit.nowHp > 0)
        {
            yield return new WaitForSeconds(0.3f);
            ShowSpellTextObj(attackUnit.cardObj, "13", false);
            yield return new WaitForSeconds(0.2f);
            PlayAudioForSecondClip(13, 0);
            AttackToEffectShow(attackedUnit, false, "13A");

            int damage = DefDamageProcessFun(attackUnit, attackedUnit, attackUnit.damage);
            attackedUnit.nowHp -= damage;
            AttackedAnimShow(attackedUnit, damage, false);
        }
    }

    /// <summary>
    /// 黄巾群起技能
    /// </summary>
    /// <param name="finalDamage"></param>
    /// <param name="attackUnit"></param>
    /// <param name="attackedUnit"></param>
    /// <returns></returns>
    private int HuangJinSkill(int finalDamage, FightCardData attackUnit, FightCardData attackedUnit)
    {
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.playerFightCardsDatas : FightForManager.instance.enemyFightCardsDatas;

        PlayAudioForSecondClip(65, 0);
        ShowSpellTextObj(attackUnit.cardObj, "65", false);
        AttackToEffectShow(attackedUnit, false, "65A");

        int sameTypeHeroNums = 0;
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            if (fightCardDatas[i] != null && fightCardDatas[i].nowHp > 0 && fightCardDatas[i].cardType == 0 && LoadJsonFile.heroTableDatas[fightCardDatas[i].cardId][5] == "65")
            {
                AttackToEffectShow(fightCardDatas[i], false, "65B");

                sameTypeHeroNums++;
            }
        }
        finalDamage = (int)(sameTypeHeroNums * finalDamage * LoadJsonFile.GetGameValue(146) / 100f);
        return finalDamage;
    }

    /// <summary>
    /// 铁骑分摊伤害
    /// </summary>
    /// <param name="finalDamage"></param>
    /// <param name="attackedUnit"></param>
    /// <returns></returns>
    private int TieQiFenTan(int finalDamage, FightCardData attackedUnit)
    {
        if (finalDamage <= 0)
            return finalDamage;

        int damage = finalDamage;
        if (attackedUnit.cardType == 0)
        {
            switch (LoadJsonFile.heroTableDatas[attackedUnit.cardId][5])
            {
                case "58":
                    List<FightCardData> tieQiCardsList = attackedUnit.isPlayerCard ? tieQiCardsPy : tieQiCardsEm;
                    int survivalUnit = 0;
                    for (int i = 0; i < tieQiCardsList.Count; i++)
                    {
                        if (tieQiCardsList[i].nowHp > 0)
                        {
                            survivalUnit++;
                        }
                    }

                    if (tieQiCardsList.Count > 1)
                    {
                        damage = (int)((float)damage / (survivalUnit > 0 ? survivalUnit : 1));
                        for (int i = 0; i < tieQiCardsList.Count; i++)
                        {
                            if (tieQiCardsList[i].nowHp > 0)
                            {
                                ShowSpellTextObj(tieQiCardsList[i].cardObj, "58_0", false);
                                if (tieQiCardsList[i] != attackedUnit)
                                {
                                    if (!(tieQiCardsList[i].fightState.invincibleNums > 0))
                                    {
                                        int backDamage = AddOrCutShieldValue(damage, tieQiCardsList[i], false);
                                        tieQiCardsList[i].nowHp -= backDamage;
                                        AttackedAnimShow(tieQiCardsList[i], backDamage, false);
                                    }
                                }
                            }
                        }
                    }

                    break;
                default:
                    break;
            }
        }
        return damage;
    }

    private List<FightCardData> tieQiCardsPy = new List<FightCardData>();   //玩家铁骑索引
    private List<FightCardData> tieQiCardsEm = new List<FightCardData>();   //敌人铁骑索引
    //铁骑连环技能
    private int TieQiSkill(int finalDamage, FightCardData attackUnit, FightCardData attackedUnit)
    {
        List<FightCardData> tieQiCardsList = attackUnit.isPlayerCard ? tieQiCardsPy : tieQiCardsEm;

        PlayAudioForSecondClip(58, 0);
        ShowSpellTextObj(attackUnit.cardObj, "58", false);
        AttackToEffectShow(attackedUnit, false, "58A");

        float damageBonus = 0;
        if (tieQiCardsList.Count > 1)
        {
            damageBonus = LoadJsonFile.GetGameValue(50) / 100f * tieQiCardsList.Count;
        }
        finalDamage = (int)(finalDamage * (1 + damageBonus));
        return finalDamage;
    }
    //刷新铁骑连环状态图标
    public void UpdateTieQiStateIconShow(FightCardData tieQiCard, bool isAdd)
    {
        List<FightCardData> tieQiCards = tieQiCard.isPlayerCard ? tieQiCardsPy : tieQiCardsEm;

        if (isAdd)
        {
            tieQiCards.Add(tieQiCard);
        }
        else
        {
            FightForManager.instance.DestroySateIcon(tieQiCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_lianHuan, true);
            tieQiCards.Remove(tieQiCard);
        }

        if (tieQiCards.Count > 1)
        {
            //Debug.Log(tieQiCards.Count - 1);
            if (tieQiCards.Count == 2)
            {
                FightForManager.instance.CreateSateIcon(tieQiCards[0].cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_lianHuan, true);
            }
            FightForManager.instance.CreateSateIcon(tieQiCards[tieQiCards.Count - 1].cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_lianHuan, true);
            //for (int i = 0; i < tieQiCards.Count; i++)
            //{
            //    Debug.Log(i);
            //    FightForManager.instance.CreateSateIcon(tieQiCards[i].cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_lianHuan, true);
            //}
        }
        else
        {
            for (int i = 0; i < tieQiCards.Count; i++)
            {
                FightForManager.instance.DestroySateIcon(tieQiCards[i].cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_lianHuan, true);
            }
        }
    }

    /// <summary>
    /// 清空敌方铁骑列表
    /// </summary>
    public void ClearEmTieQiCardList()
    {
        tieQiCardsEm.Clear();   //敌方铁骑列表清空
    }

    //蛮族剧毒技能
    private void ManZuSkill(FightCardData attackUnit, FightCardData attackedUnit)
    {
        PlayAudioForSecondClip(56, 0);
        ShowSpellTextObj(attackUnit.cardObj, "56", false);
        AttackToEffectShow(attackedUnit, false, "56A");
        if (attackedUnit.cardType == 0 && attackedUnit.nowHp > 0)
        {
            if (TakeSpecialAttack(LoadJsonFile.GetGameValue(51)))
            {
                if (attackedUnit.fightState.poisonedNums <= 0)
                {
                    FightForManager.instance.CreateSateIcon(attackedUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_poisoned, true);
                }
                ShowSpellTextObj(attackedUnit.cardObj, LoadJsonFile.GetStringText(12), true, true);
                attackedUnit.fightState.poisonedNums++;
            }
        }
    }

    //火船引燃技能
    private void HuoChuanSkill(int finalDamage, FightCardData attackUnit, FightCardData attackedUnit)
    {
        int takeBurnPro = LoadJsonFile.GetGameValue(52);   //附加灼烧概率

        //血量小于50%，发起自杀攻击
        if (attackUnit.nowHp / (float)attackUnit.fullHp <= LoadJsonFile.GetGameValue(54) / 100f)
        {
            PlayAudioForSecondClip(84, 0);
            ShowSpellTextObj(attackUnit.cardObj, "55_0", false);
            AttackToEffectShow(attackedUnit, false, "55A0");

            takeBurnPro = LoadJsonFile.GetGameValue(53);

            attackUnit.nowHp = 0;
            UpdateUnitHpShow(attackUnit);

            finalDamage = (int)(finalDamage * LoadJsonFile.GetGameValue(55) / 100f);

            FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
            for (int i = 0; i < FightForManager.instance.CardNearbyAdditionForeach[targetIndex].Length; i++)
            {
                FightCardData attackedUnits = fightCardDatas[FightForManager.instance.CardNearbyAdditionForeach[targetIndex][i]];
                if (attackedUnits != null && attackedUnits.nowHp > 0)
                {
                    AttackToEffectShow(attackedUnits, false, "55A");
                    int backDamage = DefDamageProcessFun(attackUnit, attackedUnits, finalDamage);
                    attackedUnits.nowHp -= backDamage;
                    AttackedAnimShow(attackedUnits, backDamage, false);
                    if (attackedUnits.cardType == 522)
                    {
                        if (attackedUnits.nowHp <= 0)
                        {
                            recordWinner = attackedUnits.isPlayerCard ? -1 : 1;
                        }
                    }
                    if (attackedUnits.cardType == 0 && attackedUnits.nowHp > 0)
                    {
                        TakeToBurn(attackedUnits, takeBurnPro, attackUnit);
                    }
                }
            }

        }
        else
        {
            PlayAudioForSecondClip(55, 0);
            ShowSpellTextObj(attackUnit.cardObj, "55", false);
            AttackToEffectShow(attackedUnit, false, "55A");
        }
        TakeToBurn(attackedUnit, takeBurnPro, attackUnit);
    }

    //军师技能
    private void JunShiSkill(FightCardData attackUnit, int classType, int finalDamage)
    {
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        List<int> canFightUnits = new List<int>();
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            if (fightCardDatas[i] != null && fightCardDatas[i].cardType == 0 && fightCardDatas[i].nowHp > 0)
            {
                canFightUnits.Add(i);
            }
        }
        //按血量从少到多排序
        canFightUnits.Sort((int a, int b) =>
        {
            return (fightCardDatas[a].nowHp / (float)fightCardDatas[a].fullHp).CompareTo(fightCardDatas[a].nowHp / (float)fightCardDatas[b].fullHp);
        });
        int fightNums = 0;
        int killPos = 0;
        int zhanShaXian = attackUnit.damage;    //斩杀线
        switch (indexAttackType)
        {
            case 0:
                zhanShaXian = (int)(zhanShaXian * LoadJsonFile.GetGameValue(56) / 100f);
                break;
            case 1: //会心
                zhanShaXian = (int)(zhanShaXian * LoadJsonFile.GetGameValue(58) / 100f);
                break;
            case 2: //暴击
                zhanShaXian = (int)(zhanShaXian * LoadJsonFile.GetGameValue(57) / 100f);
                break;
            default:
                break;
        }

        if (classType == 26)
        {
            fightNums = LoadJsonFile.GetGameValue(59);
            killPos = LoadJsonFile.GetGameValue(60);
        }
        else
        {
            fightNums = LoadJsonFile.GetGameValue(61);
            killPos = LoadJsonFile.GetGameValue(62);
        }
        fightNums = canFightUnits.Count > fightNums ? fightNums : canFightUnits.Count;

        if (fightNums > 0)
        {
            isNeedToAttack = false;
            ShowSpellTextObj(attackUnit.cardObj, classType.ToString(), false);
            string effectStr = classType + "A";
            PlayAudioForSecondClip(classType, 0);

            finalDamage = (int)(LoadJsonFile.GetGameValue(63) / 100f * finalDamage);

            for (int i = 0; i < fightNums; i++)
            {
                int nowDamage = 0;
                //造成伤害
                FightCardData attackedUnit = fightCardDatas[canFightUnits[i]];
                if (attackedUnit.nowHp < zhanShaXian && TakeSpecialAttack(killPos))
                {
                    ShowSpellTextObj(attackedUnit.cardObj, LoadJsonFile.GetStringText(13), true, true);

                    nowDamage = attackedUnit.fightState.shieldValue + attackedUnit.nowHp;
                    attackedUnit.nowHp = 0;
                }
                else
                {
                    nowDamage = DefDamageProcessFun(attackUnit, attackedUnit, finalDamage);
                    attackedUnit.nowHp -= nowDamage;
                }
                AttackedAnimShow(attackedUnit, nowDamage, false);
                AttackToEffectShow(attackedUnit, false, effectStr);
            }
        }
    }

    //水兵卸甲技能
    private void ShuiBingXieJia(FightCardData attackUnit, FightCardData attackedUnit)
    {
        AttackToEffectShow(attackedUnit, false, "44A");
        PlayAudioForSecondClip(44, 0);
        if (attackedUnit.cardType == 0)
        {
            //Debug.Log("-----水兵卸甲技能");
            ShowSpellTextObj(attackUnit.cardObj, "44", false);
            TakeToRemoveArmor(attackedUnit, LoadJsonFile.GetGameValue(64), attackUnit);
        }
    }

    //器械修复技能
    private void QiXieXiuFu(FightCardData attackUnit)
    {
        int fightNums = LoadJsonFile.GetGameValue(132);
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.playerFightCardsDatas : FightForManager.instance.enemyFightCardsDatas;
        List<int> canHuiFuUnits = new List<int>();
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            if (fightCardDatas[i] != null && fightCardDatas[i].cardType != 0 && fightCardDatas[i].nowHp > 0 && fightCardDatas[i].nowHp != fightCardDatas[i].fullHp)
            {
                canHuiFuUnits.Add(i);
            }
        }
        if (canHuiFuUnits.Count > 0)
        {
            isNeedToAttack = false;
            if (fightNums > canHuiFuUnits.Count)
            {
                fightNums = canHuiFuUnits.Count;
            }
            canHuiFuUnits.Sort((int a, int b) =>
            {
                return fightCardDatas[a].nowHp.CompareTo(fightCardDatas[b].nowHp);
            });

            ShowSpellTextObj(attackUnit.cardObj, "40", false);

            int addtionNums = (int)(attackUnit.damage * (LoadJsonFile.GetGameValue(65) / 100f) / fightNums);
            for (int i = 0; i < fightNums; i++)
            {
                AttackToEffectShow(fightCardDatas[canHuiFuUnits[i]], false, "40A");
                ShowSpellTextObj(fightCardDatas[canHuiFuUnits[i]].cardObj, LoadJsonFile.GetStringText(15), true, false);
                fightCardDatas[canHuiFuUnits[i]].nowHp += addtionNums;
                AttackedAnimShow(fightCardDatas[canHuiFuUnits[i]], addtionNums, true);
            }
        }
    }

    //辩士技能
    private void BianShiSkill(int fightNums, FightCardData attackUnit, int classType)
    {
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        List<int> canFightUnits = new List<int>();
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            if (fightCardDatas[i] != null && fightCardDatas[i].nowHp > 0 && fightCardDatas[i].cardType == 0)
            {
                canFightUnits.Add(i);
            }
        }
        if (canFightUnits.Count > 0)
        {
            isNeedToAttack = false;
            List<int> attackedIndexList = BackRandsList(canFightUnits, fightNums);
            ShowSpellTextObj(attackUnit.cardObj, classType.ToString(), false);
            string effectStr = "";
            if (classType == 34)  //辩士
            {
                effectStr = "34A";
                PlayAudioForSecondClip(34, 0);
            }
            else
            {//大辩士
                effectStr = "35A";
                PlayAudioForSecondClip(35, 0);
            }
            for (int i = 0; i < attackedIndexList.Count; i++)
            {
                AttackToEffectShow(fightCardDatas[attackedIndexList[i]], false, effectStr);
                TakeToImprisoned(fightCardDatas[attackedIndexList[i]],
                    indexAttackType == 1 ?
                    LoadJsonFile.GetGameValue(66) :
                    (LoadJsonFile.GetGameValue(68) * attackUnit.cardGrade + LoadJsonFile.GetGameValue(67)),
                    attackUnit);
            }
        }
    }

    //说客技能
    private void ShuiKeSkill(int fightNums, FightCardData attackUnit, int classType)
    {
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        List<int> canFightUnits = new List<int>();
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            if (fightCardDatas[i] != null && fightCardDatas[i].nowHp > 0 && fightCardDatas[i].cardType == 0)
            {
                canFightUnits.Add(i);
            }
        }
        if (canFightUnits.Count > 0)
        {
            isNeedToAttack = false;
            List<int> attackedIndexList = BackRandsList(canFightUnits, fightNums);
            ShowSpellTextObj(attackUnit.cardObj, classType.ToString(), false);
            string effectStr = "";
            if (classType == 47)  //说客
            {
                effectStr = "47A";
                PlayAudioForSecondClip(47, 0);
            }
            else
            {//大说客
                effectStr = "48A";
                PlayAudioForSecondClip(48, 0);
            }
            for (int i = 0; i < attackedIndexList.Count; i++)
            {
                AttackToEffectShow(fightCardDatas[attackedIndexList[i]], false, effectStr);
                TakeToCowardly(fightCardDatas[attackedIndexList[i]],
                    indexAttackType == 1 ?
                    LoadJsonFile.GetGameValue(69) :
                    (LoadJsonFile.GetGameValue(71) * attackUnit.cardGrade + LoadJsonFile.GetGameValue(70)),
                    attackUnit);
            }
        }
    }

    //投石车精准技能
    private void TouShiCheSkill(int finalDamage, FightCardData attackUnit)
    {
        isNeedToAttack = false;
        ShowSpellTextObj(attackUnit.cardObj, "24", false);
        PlayAudioForSecondClip(24, 0);
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        List<int> canFightUnits = new List<int>();
        if (fightCardDatas[12] != null && fightCardDatas[12].nowHp > 0)
        {
            canFightUnits.Add(12);
        }
        if (fightCardDatas[15] != null && fightCardDatas[15].nowHp > 0)
        {
            canFightUnits.Add(15);
        }
        if (fightCardDatas[16] != null && fightCardDatas[16].nowHp > 0)
        {
            canFightUnits.Add(16);
        }
        if (fightCardDatas[17] != null && fightCardDatas[17].nowHp > 0)
        {
            canFightUnits.Add(17);
        }
        int randTarget = canFightUnits[Random.Range(0, canFightUnits.Count)];
        FightCardData attackedUnit = fightCardDatas[randTarget];
        if (attackedUnit != null && (!(attackedUnit.fightState.invincibleNums > 0 || OffsetWithStand(attackedUnit))))
        {
            AttackToEffectShow(attackedUnit, false, "24A");
            if (attackedUnit.cardType == 522)   //如果目标是老巢，造成1.5倍伤害
            {
                finalDamage = (int)(finalDamage * LoadJsonFile.GetGameValue(72) / 100f);
                finalDamage = DefDamageProcessFun(attackUnit, attackedUnit, finalDamage);
                attackedUnit.nowHp -= finalDamage;
                if (attackedUnit.nowHp <= 0)
                {
                    recordWinner = attackedUnit.isPlayerCard ? -1 : 1;
                }
            }
            else
            {
                finalDamage = DefDamageProcessFun(attackUnit, attackedUnit, finalDamage);
                attackedUnit.nowHp -= finalDamage;
            }
            AttackedAnimShow(attackedUnit, finalDamage, false);
        }
    }

    //攻城车破城技能
    private int GongChengChePoCheng(int damage, FightCardData attackUnit, FightCardData attackedUnit)
    {
        if (attackedUnit.cardType != 0)
        {
            //Debug.Log("----攻城车破城");
            ShowSpellTextObj(attackUnit.cardObj, "23", false);
            AttackToEffectShow(attackedUnit, false, "23A");
            PlayAudioForSecondClip(23, 0);
            return (int)(damage * LoadJsonFile.GetGameValue(73) / 100f);
        }
        else
        {
            AttackToEffectShow(attackedUnit, true);
            return (int)(damage * LoadJsonFile.GetGameValue(74) / 100f);
        }
    }

    //隐士技能
    private void YinShiSkill(int fightNums, FightCardData attackUnit, int classType, int finalDamage)
    {
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        List<int> canFightUnits = new List<int>();
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            if (fightCardDatas[i] != null && fightCardDatas[i].nowHp > 0 && fightCardDatas[i].cardType == 0)
            {
                canFightUnits.Add(i);
            }
        }
        if (canFightUnits.Count > 0)
        {
            isNeedToAttack = false;
            ShowSpellTextObj(attackUnit.cardObj, classType.ToString(), false);
            string effectStr = classType + "A";
            PlayAudioForSecondClip(classType, 0);

            finalDamage = (int)(LoadJsonFile.GetGameValue(75) / 100f * finalDamage);
            int attackedNums = 0;
            for (int i = 0; i < canFightUnits.Count; i++)
            {
                FightCardData attackedUnit = fightCardDatas[canFightUnits[i]];
                AttackToEffectShow(attackedUnit, false, effectStr);
                //造成伤害
                int nowDamage = DefDamageProcessFun(attackUnit, attackedUnit, finalDamage);
                attackedUnit.nowHp -= nowDamage;
                AttackedAnimShow(attackedUnit, nowDamage, false);
                //击退
                int nextPos = attackedUnit.posIndex + 5;
                if (nextPos <= 19 && fightCardDatas[nextPos] == null)
                {
                    StartCoroutine(TakeCardPosBack(fightCardDatas, attackedUnit, nextPos, 0.2f, isPlayerRound));
                }

                attackedNums++;
                if (attackedNums >= fightNums)
                    break;
            }
        }
    }

    //弓兵远射技能
    private void GongBingYuanSheSkill(int finalDamage, FightCardData attackUnit, FightCardData attackedUnitForIndex, int classIndex)
    {
        //Debug.Log("---弓兵远射");
        ShowSpellTextObj(attackUnit.cardObj, "20", false);
        int damage = finalDamage;
        PlayAudioForSecondClip(20, 0);

        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        List<int> canFightUnits = new List<int>();
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            if (i != attackedUnitForIndex.posIndex && fightCardDatas[i] != null && fightCardDatas[i].nowHp > 0)
            {
                canFightUnits.Add(i);
            }
        }
        isNeedToAttack = false;

        int fightNums = 0;
        if (classIndex == 20)   //弓兵和大弓
        {
            fightNums = Mathf.Min(LoadJsonFile.GetGameValue(76), canFightUnits.Count);
            damage = (int)(damage * (LoadJsonFile.GetGameValue(77) / 100f) / (fightNums + 1));
        }
        else
        {
            fightNums = Mathf.Min(LoadJsonFile.GetGameValue(78), canFightUnits.Count);
            damage = (int)(damage * (LoadJsonFile.GetGameValue(79) / 100f) / (fightNums + 1));
        }

        List<int> attackedIndexList = BackRandsList(canFightUnits, fightNums);
        attackedIndexList.Add(attackedUnitForIndex.posIndex);

        for (int i = 0; i < attackedIndexList.Count; i++)
        {
            FightCardData attackedUnit = fightCardDatas[attackedIndexList[i]];
            AttackToEffectShow(attackedUnit, false, "20A");
            int nowDamage = DefDamageProcessFun(attackUnit, attackedUnit, damage);
            attackedUnit.nowHp -= nowDamage;
            AttackedAnimShow(attackedUnit, nowDamage, false);
            if (attackedUnit.cardType == 522)
            {
                if (attackedUnit.nowHp <= 0)
                {
                    recordWinner = attackedUnit.isPlayerCard ? -1 : 1;
                }
            }
        }
    }

    //辅佐庇护技能
    private void FuZuoBiHuSkill(int finalDamage, FightCardData attackUnit)
    {
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.playerFightCardsDatas : FightForManager.instance.enemyFightCardsDatas;
        int addIndex = -1;
        float minFlo = 2;
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            if (fightCardDatas[i] != null && fightCardDatas[i].cardType == 0 && fightCardDatas[i].nowHp > 0 && fightCardDatas[i].fightState.shieldValue < 1000)
            {
                float nowFlo = (fightCardDatas[i].nowHp + fightCardDatas[i].fightState.shieldValue) / (float)fightCardDatas[i].fullHp;
                if (nowFlo < minFlo)
                {
                    minFlo = nowFlo;
                    addIndex = i;
                }
            }
        }
        if (addIndex != -1)
        {
            isNeedToAttack = false;
            AddOrCutShieldValue(finalDamage, fightCardDatas[addIndex], true);
            //Debug.Log("---辅佐技能");
            ShowSpellTextObj(attackUnit.cardObj, "39", false);
            AttackToEffectShow(fightCardDatas[addIndex], false, "39A");
            PlayAudioForSecondClip(39, 0);
        }
    }

    //内政技能
    private void NeiZhengSkill(FightCardData attackUnit)
    {
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.playerFightCardsDatas : FightForManager.instance.enemyFightCardsDatas;

        int prop = LoadJsonFile.GetGameValue(127) * attackUnit.cardGrade + LoadJsonFile.GetGameValue(126);

        int cardIndex = -1;
        int cardIndex2 = -1;    //非普通攻击
        int cardIndex3 = -1;    //会心一击
        int maxBadNums = 0;
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            int nowBadNums = 0;
            if (fightCardDatas[i] != null && fightCardDatas[i].nowHp > 0 && fightCardDatas[i].cardType == 0)
            {
                if (fightCardDatas[i].fightState.dizzyNums > 0)
                {
                    nowBadNums += fightCardDatas[i].fightState.dizzyNums;
                }
                if (fightCardDatas[i].fightState.imprisonedNums > 0)
                {
                    nowBadNums += fightCardDatas[i].fightState.imprisonedNums;
                }
                if (fightCardDatas[i].fightState.bleedNums > 0)
                {
                    nowBadNums += fightCardDatas[i].fightState.bleedNums;
                }
                if (fightCardDatas[i].fightState.poisonedNums > 0)
                {
                    nowBadNums += fightCardDatas[i].fightState.poisonedNums;
                }
                if (fightCardDatas[i].fightState.burnedNums > 0)
                {
                    nowBadNums += fightCardDatas[i].fightState.burnedNums;
                }
                if (fightCardDatas[i].fightState.removeArmorNums > 0)
                {
                    nowBadNums += fightCardDatas[i].fightState.removeArmorNums;
                }
                if (fightCardDatas[i].fightState.cowardlyNums > 0)
                {
                    nowBadNums += fightCardDatas[i].fightState.cowardlyNums;
                }
                if (nowBadNums > maxBadNums)  //记录负面效果最多的单位
                {
                    maxBadNums = nowBadNums;
                    cardIndex3 = cardIndex2;
                    cardIndex2 = cardIndex;
                    cardIndex = i;
                }
            }
        }
        if (cardIndex != -1)
        {
            isNeedToAttack = false;
            //Debug.Log("---内政技能");
            ShowSpellTextObj(attackUnit.cardObj, "38", false);
            PlayAudioForSecondClip(38, 0);
            AttackToEffectShow(fightCardDatas[cardIndex], false, "38A");
            ShowSpellTextObj(fightCardDatas[cardIndex].cardObj, LoadJsonFile.GetStringText(14), true, false);

            if (TakeSpecialAttack(prop))
            {
                ClearOneUnitBadState(fightCardDatas[cardIndex]);
            }
        }
        if (indexAttackType != 0 && cardIndex2 != -1)
        {
            cardIndex = cardIndex2;
            isNeedToAttack = false;
            ShowSpellTextObj(attackUnit.cardObj, "38", false);
            PlayAudioForSecondClip(38, 0);
            AttackToEffectShow(fightCardDatas[cardIndex], false, "38A");
            ShowSpellTextObj(fightCardDatas[cardIndex].cardObj, LoadJsonFile.GetStringText(14), true, false);
            if (TakeSpecialAttack(prop))
            {
                ClearOneUnitBadState(fightCardDatas[cardIndex]);
            }

            if (indexAttackType == 1 && cardIndex3 != -1)
            {
                cardIndex = cardIndex3;
                isNeedToAttack = false;
                ShowSpellTextObj(attackUnit.cardObj, "38", false);
                PlayAudioForSecondClip(38, 0);
                AttackToEffectShow(fightCardDatas[cardIndex], false, "38A");
                ShowSpellTextObj(fightCardDatas[cardIndex].cardObj, LoadJsonFile.GetStringText(14), true, false);
                if (TakeSpecialAttack(prop))
                {
                    ClearOneUnitBadState(fightCardDatas[cardIndex]);
                }
            }
        }
    }

    //清除单个单位的负面状态
    private void ClearOneUnitBadState(FightCardData cardData)
    {
        if (cardData.fightState.dizzyNums > 0)
        {
            cardData.fightState.dizzyNums = 0;
            FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_dizzy, true);
        }
        if (cardData.fightState.imprisonedNums > 0)
        {
            cardData.fightState.imprisonedNums = 0;
            FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_imprisoned, true);
        }
        if (cardData.fightState.bleedNums > 0)
        {
            cardData.fightState.bleedNums = 0;
            FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_bleed, true);
        }
        if (cardData.fightState.poisonedNums > 0)
        {
            cardData.fightState.poisonedNums = 0;
            FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_poisoned, true);
        }
        if (cardData.fightState.burnedNums > 0)
        {
            cardData.fightState.burnedNums = 0;
            FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_burned, true);
        }
        if (cardData.fightState.removeArmorNums > 0)
        {
            cardData.fightState.removeArmorNums = 0;
            FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_removeArmor, true);
        }
        if (cardData.fightState.cowardlyNums > 0)
        {
            cardData.fightState.cowardlyNums = 0;
            FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_cowardly, true);
        }
    }

    //医生技能
    private void YiShengSkill(int fightNums, FightCardData attackUnit, int classType)
    {
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.playerFightCardsDatas : FightForManager.instance.enemyFightCardsDatas;
        List<int> canHuiFuUnits = new List<int>();
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            if (fightCardDatas[i] != null && fightCardDatas[i].cardType == 0 && fightCardDatas[i].nowHp > 0 && fightCardDatas[i].nowHp != fightCardDatas[i].fullHp)
            {
                canHuiFuUnits.Add(i);
            }
        }
        if (canHuiFuUnits.Count > 0)
        {
            isNeedToAttack = false;

            if (fightNums > canHuiFuUnits.Count)
            {
                fightNums = canHuiFuUnits.Count;
            }
            canHuiFuUnits.Sort((int a, int b) =>
            {
                return fightCardDatas[a].nowHp.CompareTo(fightCardDatas[b].nowHp);
            });

            ShowSpellTextObj(attackUnit.cardObj, classType.ToString(), false);

            int addtionNums = 0;
            string effectStr = "";
            if (classType == 42)  //医士
            {
                effectStr = "42A";
                PlayAudioForSecondClip(42, 0);
                addtionNums = (int)(attackUnit.damage * (LoadJsonFile.GetGameValue(80) / 100f) / fightNums);
            }
            else
            {//大医士
                effectStr = "43A";
                PlayAudioForSecondClip(43, 0);
                addtionNums = (int)(attackUnit.damage * (LoadJsonFile.GetGameValue(81) / 100f) / fightNums);
            }
            for (int i = 0; i < fightNums; i++)
            {
                AttackToEffectShow(fightCardDatas[canHuiFuUnits[i]], false, effectStr);
                fightCardDatas[canHuiFuUnits[i]].nowHp += addtionNums;
                ShowSpellTextObj(fightCardDatas[canHuiFuUnits[i]].cardObj, LoadJsonFile.GetStringText(15), true, false);
                AttackedAnimShow(fightCardDatas[canHuiFuUnits[i]], addtionNums, true);
            }
        }
    }

    //美人技能
    private void MeiRenJiNeng(FightCardData attackUnit, int classType)
    {
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.playerFightCardsDatas : FightForManager.instance.enemyFightCardsDatas;
        int index = -1;
        bool isHadFirst = false;

        PlayAudioForSecondClip(classType, 0);

        int prop = LoadJsonFile.GetGameValue(129) * attackUnit.cardGrade + LoadJsonFile.GetGameValue(128);
        int fightNums = 1;  //添加单位数量
        if (indexAttackType != 0)
        {
            if (indexAttackType == 1)
            {
                fightNums = LoadJsonFile.GetGameValue(131);
            }
            else
            {
                fightNums = LoadJsonFile.GetGameValue(130);
            }
        }
        for (int m = 0; m < fightNums; m++)
        {
            for (int i = 0; i < fightCardDatas.Length; i++)
            {
                if (fightCardDatas[i] != null && fightCardDatas[i].cardType == 0 && fightCardDatas[i].nowHp > 0)
                {
                    if (!isHadFirst)
                    {
                        index = i;
                        isHadFirst = true;
                    }
                    else
                    {
                        if (classType == 45)
                        {
                            if (fightCardDatas[i].damage > fightCardDatas[index].damage)
                            {
                                if (fightCardDatas[i].fightState.neizhuNums <= fightCardDatas[index].fightState.neizhuNums)
                                {
                                    index = i;
                                }
                                else
                                {
                                    //新单位伤害高，但内助层数也高
                                }
                            }
                            else
                            {
                                if (fightCardDatas[i].fightState.neizhuNums < fightCardDatas[index].fightState.neizhuNums)
                                {
                                    index = i;
                                }
                                else
                                {
                                    //新单位伤害低，而且内助层数也高
                                }
                            }
                        }
                        else
                        {
                            if (fightCardDatas[i].damage > fightCardDatas[index].damage)
                            {
                                if (fightCardDatas[i].fightState.shenzhuNums <= fightCardDatas[index].fightState.shenzhuNums)
                                {
                                    index = i;
                                }
                                else
                                {
                                    //新单位伤害高，但内助层数也高
                                }
                            }
                            else
                            {
                                if (fightCardDatas[i].fightState.shenzhuNums < fightCardDatas[index].fightState.shenzhuNums)
                                {
                                    index = i;
                                }
                                else
                                {
                                    //新单位伤害低，而且内助层数也高
                                }
                            }
                        }
                    }
                }
            }
            if (index != -1)
            {
                isNeedToAttack = false;
                ShowSpellTextObj(attackUnit.cardObj, classType.ToString(), false);
                if (TakeSpecialAttack(prop))
                {
                    AttackToEffectShow(fightCardDatas[index], false, classType + "A");
                    if (classType == 45)  //美人
                    {
                        if (fightCardDatas[index].fightState.neizhuNums <= 0)
                        {
                            FightForManager.instance.CreateSateIcon(fightCardDatas[index].cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_neizhu, false);
                        }
                        fightCardDatas[index].fightState.neizhuNums++;
                    }
                    else
                    {//大美人
                        if (fightCardDatas[index].fightState.shenzhuNums <= 0)
                        {
                            FightForManager.instance.CreateSateIcon(fightCardDatas[index].cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_shenzhu, false);
                        }
                        fightCardDatas[index].fightState.shenzhuNums++;
                    }
                }
            }
        }
    }

    //谋士技能
    private void MouShiSkill(int fightNums, FightCardData attackUnit, int classType)
    {
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        List<int> canFightUnits = new List<int>();
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            if (fightCardDatas[i] != null && fightCardDatas[i].nowHp > 0 && fightCardDatas[i].cardType == 0)
            {
                canFightUnits.Add(i);
            }
        }
        if (canFightUnits.Count > 0)
        {
            isNeedToAttack = false;
            List<int> attackedIndexList = BackRandsList(canFightUnits, fightNums);
            //Debug.Log("---谋士技能");
            ShowSpellTextObj(attackUnit.cardObj, classType.ToString(), false);
            string effectStr = "";
            if (classType == 36)  //谋士
            {
                effectStr = "36A";
                PlayAudioForSecondClip(36, 0);
            }
            else
            {//大谋士
                effectStr = "37A";
                PlayAudioForSecondClip(37, 0);
            }

            //眩晕概率
            int propNums = LoadJsonFile.GetGameValue(83) * attackUnit.cardGrade + LoadJsonFile.GetGameValue(82);
            if (indexAttackType == 2)
            {
                propNums += LoadJsonFile.GetGameValue(84);
            }
            else
            {
                if (indexAttackType == 1)
                {
                    propNums += LoadJsonFile.GetGameValue(85);
                }
            }

            for (int i = 0; i < attackedIndexList.Count; i++)
            {
                AttackToEffectShow(fightCardDatas[attackedIndexList[i]], false, effectStr);
                TakeOneUnitDizzed(fightCardDatas[attackedIndexList[i]], propNums, attackUnit);
            }
        }
    }

    //毒士技能
    private void DuShiSkill(int fightNums, FightCardData attackUnit, int classType, int finalDamage)
    {
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        List<int> canFightUnits = new List<int>();
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            if (fightCardDatas[i] != null && fightCardDatas[i].nowHp > 0 && fightCardDatas[i].cardType == 0)
            {
                canFightUnits.Add(i);
            }
        }
        if (canFightUnits.Count > 0)
        {
            isNeedToAttack = false;
            List<int> attackedIndexList = BackRandsList(canFightUnits, fightNums);
            //Debug.Log("---毒士技能");
            ShowSpellTextObj(attackUnit.cardObj, classType.ToString(), false);

            finalDamage = (int)(finalDamage * LoadJsonFile.GetGameValue(86) / 100f);

            string effectStr = "";
            if (classType == 30)  //毒士
            {
                effectStr = "30A";
                PlayAudioForSecondClip(30, 0);
            }
            else
            {//大毒士
                effectStr = "31A";
                PlayAudioForSecondClip(31, 0);
            }

            int prop = LoadJsonFile.GetGameValue(89) * attackUnit.cardGrade + LoadJsonFile.GetGameValue(88);
            if (indexAttackType != 0)
            {
                if (indexAttackType == 1)
                {
                    prop += LoadJsonFile.GetGameValue(124);
                }
                else
                {
                    prop += LoadJsonFile.GetGameValue(125);
                }
            }
            prop = Mathf.Min(LoadJsonFile.GetGameValue(87), prop);

            for (int i = 0; i < attackedIndexList.Count; i++)
            {
                AttackToEffectShow(fightCardDatas[attackedIndexList[i]], false, effectStr);
                TakeToPoisoned(fightCardDatas[attackedIndexList[i]], prop, attackUnit);

                int backDamage = DefDamageProcessFun(attackUnit, fightCardDatas[attackedIndexList[i]], finalDamage);
                fightCardDatas[attackedIndexList[i]].nowHp -= backDamage;
                AttackedAnimShow(fightCardDatas[attackedIndexList[i]], backDamage, false);
            }
        }
    }

    //敢死死战技能
    private int GanSiSiZhanAttack(int finalDamage, FightCardData attackedUnit)
    {
        if (attackedUnit.fightState.deathFightNums >= 1)
        {
            //Debug.Log("---敢死死战");
            ShowSpellTextObj(attackedUnit.cardObj, "41", false);
            finalDamage = -1 * finalDamage;
        }
        return finalDamage;
    }

    //刺客破甲技能
    private void CiKePoJiaAttack(FightCardData attackedUnit, FightCardData attackUnit)
    {
        //Debug.Log("---刺客破甲");
        PlayAudioForSecondClip(25, 0);
        AttackToEffectShow(attackedUnit, false, "25A");
        if (attackedUnit.cardType == 0)
        {
            ShowSpellTextObj(attackUnit.cardObj, "25", false);
            TakeToBleed(attackedUnit, LoadJsonFile.GetGameValue(147), attackUnit);
            ShowSpellTextObj(attackUnit.cardObj, LoadJsonFile.GetStringText(16), true, true);
        }
    }

    IEnumerator TakeCardPosBack(FightCardData[] fightCardDatas, FightCardData attackedUnit, int nextPos, float waitTime, bool isPlayer)
    {
        yield return new WaitForSeconds(waitTime);

        FightForManager.instance.CardGoIntoBattleProcess(attackedUnit, attackedUnit.posIndex, fightCardDatas, false);

        ShowSpellTextObj(attackedUnit.cardObj, LoadJsonFile.GetStringText(17), true, true);
        attackedUnit.cardObj.transform.DOMove(
            isPlayer ? FightForManager.instance.enemyCardsPos[nextPos].transform.position : FightForManager.instance.playerCardsPos[nextPos].transform.position,
            waitTime
            ).SetEase(Ease.Unset).OnComplete(delegate ()
            {
                FightForManager.instance.CardGoIntoBattleProcess(attackedUnit, nextPos, fightCardDatas, true);
            });

        fightCardDatas[nextPos] = attackedUnit;
        fightCardDatas[attackedUnit.posIndex] = null;
        attackedUnit.posIndex = nextPos;
    }

    //战船冲击技能
    private int ZhanChuanChongJiAttack(int finalDamage, FightCardData attackedUnit, FightCardData attackUnit)
    {
        //使敌方武将和士兵单位往后退一格。当敌方单位无法再后退时，造成2.5倍伤害
        PlayAudioForSecondClip(21, 0);
        AttackToEffectShow(attackedUnit, false, "21A");
        ShowSpellTextObj(attackUnit.cardObj, "21", false);
        if (attackedUnit.cardType == 0)
        {
            FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
            int nextPos = attackedUnit.posIndex + 5;
            if (nextPos <= 19 && fightCardDatas[nextPos] == null)
            {
                StartCoroutine(TakeCardPosBack(fightCardDatas, attackedUnit, nextPos, 0.2f, isPlayerRound));
            }
            else
            {
                finalDamage = (int)(finalDamage * LoadJsonFile.GetGameValue(90) / 100f);
            }
        }
        return finalDamage;
    }

    //战车撞压技能
    private int ZhanCheZhuangYaAttack(int finalDamage, FightCardData attackedUnit, FightCardData attackUnit)
    {
        //50%概率使敌方武将和士兵单位【眩晕】。对已经眩晕单位，造成2.5倍伤害
        PlayAudioForSecondClip(22, 0);
        ShowSpellTextObj(attackUnit.cardObj, "22", false);
        AttackToEffectShow(attackedUnit, false, "22A");
        if (attackedUnit.fightState.dizzyNums > 0)
        {
            finalDamage = (int)(finalDamage * LoadJsonFile.GetGameValue(92) / 100f);
        }
        TakeOneUnitDizzed(attackedUnit, LoadJsonFile.GetGameValue(91), attackUnit);
        return finalDamage;
    }

    //斧兵屠戮技能
    private int FuBingTuLuAttack(int finalDamage, FightCardData attackedUnit, FightCardData attackUnit)
    {
        PlayAudioForSecondClip(18, 0);
        AttackToEffectShow(attackedUnit, false, "18A");

        //破护盾
        if (attackedUnit.fightState.withStandNums > 0)
        {
            attackedUnit.fightState.withStandNums = 0;
            FightForManager.instance.DestroySateIcon(attackedUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_withStand, true);
        }

        float damageProp = (1f - (float)attackedUnit.nowHp / attackedUnit.fullHp) / (LoadJsonFile.GetGameValue(93) / 100f) * (LoadJsonFile.GetGameValue(94) / 100f);
        if (damageProp > 0)
        {
            ShowSpellTextObj(attackUnit.cardObj, "18", false);
            finalDamage = (int)(finalDamage * (1f + damageProp));
        }
        return finalDamage;
    }

    //戟兵横扫技能
    private void JianBingHengSaoAttack(int finalDamage, FightCardData attackUnit, FightCardData attackedUnit)
    {
        PlayAudioForSecondClip(15, 0);
        AttackToEffectShow(attackedUnit, false, "15A");
        ShowSpellTextObj(attackUnit.cardObj, "15", false);

        //对目标周围的其他单位造成50%伤害
        finalDamage = (int)(finalDamage * LoadJsonFile.GetGameValue(95) / 100f);
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        for (int i = 0; i < FightForManager.instance.CardNearbyAdditionForeach[targetIndex].Length; i++)
        {
            FightCardData attackedUnits = fightCardDatas[FightForManager.instance.CardNearbyAdditionForeach[targetIndex][i]];
            if (attackedUnits != null && attackedUnits.nowHp > 0)
            {
                int backDamage = DefDamageProcessFun(attackUnit, attackedUnits, finalDamage);
                attackedUnits.nowHp -= backDamage;
                AttackedAnimShow(attackedUnits, backDamage, false);
                if (attackedUnits.cardType == 522)
                {
                    if (attackedUnits.nowHp <= 0)
                    {
                        recordWinner = attackedUnits.isPlayerCard ? -1 : 1;
                    }
                }
            }
        }
    }

    //枪兵穿刺技能
    private void QiangBingChuanCiAttack(int finalDamage, FightCardData attackUnit, FightCardData attackedUnit, int classType)
    {
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        PlayAudioForSecondClip(14, 0);
        ShowSpellTextObj(attackUnit.cardObj, "14", false);
        GameObject effectObj = AttackToEffectShow(attackedUnit, false, classType + "A");
        effectObj.transform.localScale = new Vector3(1, attackUnit.isPlayerCard ? 1 : -1, 1);

        //对目标身后单位造成100 % 伤害
        finalDamage = (int)(finalDamage * LoadJsonFile.GetGameValue(96) / 100f);
        int fightNums = classType == 14 ? 2 : 1;    //穿刺目标个数

        int chuanCiUnitId = targetIndex;
        for (int n = 0; n < fightNums; n++)
        {
            chuanCiUnitId = chuanCiUnitId + 5;
            if (chuanCiUnitId < 20 && fightCardDatas[chuanCiUnitId] != null && fightCardDatas[chuanCiUnitId].nowHp > 0)
            {
                int backDamage = DefDamageProcessFun(attackUnit, fightCardDatas[chuanCiUnitId], finalDamage);
                fightCardDatas[chuanCiUnitId].nowHp -= backDamage;
                AttackedAnimShow(fightCardDatas[chuanCiUnitId], backDamage, false);
                if (fightCardDatas[chuanCiUnitId].cardType == 522)
                {
                    if (fightCardDatas[chuanCiUnitId].nowHp <= 0)
                    {
                        recordWinner = fightCardDatas[chuanCiUnitId].isPlayerCard ? -1 : 1;
                    }
                }
            }
        }
    }

    //神武战意技能
    private int ShenWuZhanYiAttack(int finalDamage, FightCardData attackUnit, FightCardData attackedUnit)
    {
        //每次攻击时，获得【战意】状态，提升伤害，可叠加10次
        if (attackUnit.fightState.willFightNums <= 0)
        {
            FightForManager.instance.CreateSateIcon(attackUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_willFight, true);
        }
        if (attackUnit.fightState.willFightNums < 10)
        {
            attackUnit.fightState.willFightNums++;
            attackUnit.cardObj.transform.Find(StringNameStatic.StateIconPath_willFight + "Din").GetComponent<Image>().color = new Color(1, 1, 1, 0.4f + 0.6f * (attackUnit.fightState.willFightNums / 10f));
        }
        PlayAudioForSecondClip(12, 0);
        AttackToEffectShow(attackedUnit, false, "12A");
        ShowSpellTextObj(attackUnit.cardObj, "12", false);
        finalDamage = (int)(finalDamage * (1 + LoadJsonFile.GetGameValue(97) / 100f * attackUnit.fightState.willFightNums));
        return finalDamage;
    }

    //白马无畏技能
    private int TieQiWuWeiAttack(int finalDamage, FightCardData attackUnit, FightCardData attackedUnit)
    {
        PlayAudioForSecondClip(11, 0);
        AttackToEffectShow(attackedUnit, false, "11A");
        //自身血量每降低10%，提高15%伤害
        float damageProp = (1f - (float)attackUnit.nowHp / attackUnit.fullHp) / (LoadJsonFile.GetGameValue(98) / 100f) * (LoadJsonFile.GetGameValue(99) / 100f);
        if (damageProp > 0)
        {
            ShowSpellTextObj(attackUnit.cardObj, "11", false);
            finalDamage = (int)(finalDamage * (1f + damageProp));
        }
        return finalDamage;
    }

    //死士舍命技能
    private int SiShiSheMingAttack(int finalDamage, FightCardData attackUnit, FightCardData attackedUnit)
    {
        //血量低于25%时，获得【舍命】。下次攻击时，发起自杀式攻击，对敌方全体武将造成一次100%伤害
        if (attackUnit.nowHp / (float)attackUnit.fullHp <= (LoadJsonFile.GetGameValue(100) / 100f))
        {
            attackUnit.nowHp = 0;
            UpdateUnitHpShow(attackUnit);
            ShowSpellTextObj(attackUnit.cardObj, "10", false);
            PlayAudioForSecondClip(10, 0);
            finalDamage = (int)(finalDamage * LoadJsonFile.GetGameValue(101) / 100f);

            FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
            for (int i = 0; i < fightCardDatas.Length; i++)
            {
                FightCardData attackedUnits = fightCardDatas[i];
                if (attackedUnits != null && attackedUnits.cardType == 0 && attackedUnits.nowHp > 0)
                {
                    int backDamage = DefDamageProcessFun(attackUnit, attackedUnits, finalDamage);
                    attackedUnits.nowHp -= backDamage;
                    AttackedAnimShow(attackedUnits, backDamage, false);
                    AttackToEffectShow(attackedUnits, false, "10A");
                }
            }
            AttackToEffectShow(attackedUnit, false, "10A");

            return finalDamage;
        }
        else
        {
            PlayAudioForSecondClip(0, 0);
            AttackToEffectShow(attackedUnit, true);
            return finalDamage;
        }
    }

    //象兵践踏技能
    private void XiangBingTrampleAttAck(FightCardData attackedUnit, FightCardData attackUnit)
    {
        //Debug.Log("---象兵践踏");
        ShowSpellTextObj(attackUnit.cardObj, "8", false);

        PlayAudioForSecondClip(8, 0);
        AttackToEffectShow(attackedUnit, false, "8A");
        TakeOneUnitDizzed(attackedUnit, LoadJsonFile.GetGameValue(102), attackUnit);
    }

    //刺甲兵种反伤-护盾闪避无效
    private void CiJiaFanShangAttack(int finalDamage, FightCardData attackedUnit, FightCardData attackUnit)
    {
        if (attackedUnit.cardMoveType == 0)    //近战
        {
            PlayAudioForSecondClip(7, 0.2f);
            ShowSpellTextObj(attackUnit.cardObj, "7", false);
            finalDamage = DefDamageProcessFun(attackUnit, attackedUnit, finalDamage);
            attackedUnit.nowHp -= finalDamage;
            AttackedAnimShow(attackedUnit, finalDamage, false);
            GameObject effectObj = AttackToEffectShow(attackedUnit, false, "7A");
            effectObj.transform.localScale = new Vector3(1, attackUnit.isPlayerCard ? 1 : -1, 1);
        }
    }
    #endregion

    //敢死兵种添加死战状态
    private void SiZhanStateCreate(FightCardData attackedUnit)
    {
        if (attackedUnit.fightState.deathFightNums == 0 && attackedUnit.nowHp > 0 && attackedUnit.nowHp / (float)attackedUnit.fullHp < (LoadJsonFile.GetGameValue(103) / 100f))
        {
            //Debug.Log("---附加死战状态");
            ShowSpellTextObj(attackedUnit.cardObj, "41", false);
            if (attackedUnit.nowHp <= 0)
            {
                attackedUnit.nowHp = 1;
            }
            if (attackedUnit.fightState.deathFightNums <= 0)
            {
                FightForManager.instance.CreateSateIcon(attackedUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_deathFight, true);
            }
            attackedUnit.fightState.deathFightNums = 1;
        }
    }

    /// <summary>
    /// 初始化无敌状态或死战状态
    /// </summary>
    /// <param name="fightCardData"></param>
    private void InitCardAfterFightedState(FightCardData fightCardData)
    {
        if (fightCardData.fightState.dizzyNums > 0 || fightCardData.fightState.imprisonedNums > 0)
            return;
        if (fightCardData.cardType == 0)
        {
            switch (LoadJsonFile.heroTableDatas[fightCardData.cardId][5])
            {
                //武将的陷阵兵种
                case "5":
                    if (fightCardData.nowHp / (float)fightCardData.fullHp <= (LoadJsonFile.GetGameValue(104) / 100f))
                    {
                        if (fightCardData.fightState.invincibleNums <= 0)
                        {
                            fightCardData.fightState.invincibleNums = LoadJsonFile.GetGameValue(105);
                            FightForManager.instance.CreateSateIcon(fightCardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_invincible, true);
                        }
                    }
                    break;
                //敢死兵种添加死战
                case "41":
                    SiZhanStateCreate(fightCardData);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 防御伤害计算流程
    /// </summary>
    /// <param name="attackUnit">攻击者</param>
    /// <param name="attackedUnit">被攻击者</param>
    /// <param name="damage">伤害值</param>
    public int DefDamageProcessFun(FightCardData attackUnit, FightCardData attackedUnit, int damage)
    {
        attackedUnit.attackedBehavior = 0;
        int finalDamage = damage;
        if (attackedUnit.cardType == 0)
        {
            //判断闪避
            int dodgeRateNums = int.Parse(LoadJsonFile.heroTableDatas[attackedUnit.cardId][10]) + attackedUnit.fightState.fengShenTaiAddtion;
            if (LoadJsonFile.heroTableDatas[attackedUnit.cardId][5] == "3" || LoadJsonFile.heroTableDatas[attackedUnit.cardId][5] == "10")   //飞甲，死士 自身血量每降低10 %，提高5%闪避
            {
                if (LoadJsonFile.heroTableDatas[attackedUnit.cardId][5] == "3")
                {
                    dodgeRateNums = dodgeRateNums + (int)((1f - (float)attackedUnit.nowHp / attackedUnit.fullHp) / (LoadJsonFile.GetGameValue(106) / 100f) * LoadJsonFile.GetGameValue(107));
                }
                else
                {
                    dodgeRateNums = dodgeRateNums + (int)((1f - (float)attackedUnit.nowHp / attackedUnit.fullHp) / (LoadJsonFile.GetGameValue(108) / 100f) * LoadJsonFile.GetGameValue(109));
                }
            }
            if (TakeSpecialAttack(dodgeRateNums))
            {
                finalDamage = 0;
                ShowSpellTextObj(attackedUnit.cardObj, LoadJsonFile.GetStringText(19), false);
                PlayAudioForSecondClip(97, 0);
                attackedUnit.attackedBehavior = 2;
            }
            else
            {
                //远程攻击者，判断远程闪避
                if (attackUnit != null && attackUnit.cardType == 0 && attackUnit.cardMoveType == 1 && TakeSpecialAttack(attackedUnit.fightState.miWuZhenAddtion))
                {
                    finalDamage = 0;
                    ShowSpellTextObj(attackedUnit.cardObj, LoadJsonFile.GetStringText(19), false);
                    PlayAudioForSecondClip(97, 0);
                    attackedUnit.attackedBehavior = 2;
                }
                else
                {
                    //判断无敌
                    if (attackedUnit.fightState.invincibleNums > 0)
                    {
                        finalDamage = 0;
                        ShowSpellTextObj(attackedUnit.cardObj, LoadJsonFile.GetStringText(18), false);
                        PlayAudioForSecondClip(96, 0);
                        attackedUnit.attackedBehavior = 4;
                    }
                    else
                    {
                        //判断护盾//不可抵挡法术
                        if (attackUnit != null && (attackUnit.cardType != 0 || attackUnit.cardDamageType == 0) && OffsetWithStand(attackedUnit))
                        {
                            finalDamage = 0;
                            ShowSpellTextObj(attackedUnit.cardObj, LoadJsonFile.GetStringText(18), false);
                            PlayAudioForSecondClip(96, 0);
                            attackedUnit.attackedBehavior = 3;
                        }
                        else
                        {
                            int defPropNums = 0;
                            //免伤计算
                            if (attackedUnit.fightState.removeArmorNums <= 0)   //是否有卸甲
                            {
                                defPropNums = int.Parse(LoadJsonFile.heroTableDatas[attackedUnit.cardId][11]) + attackedUnit.fightState.fenghuotaiAddtion;
                                //白马/重甲，自身血量每降低10%，提高5%免伤
                                switch (LoadJsonFile.heroTableDatas[attackedUnit.cardId][5])
                                {
                                    case "2":
                                        //重甲，自身血量每降低10%，提高5%免伤
                                        defPropNums = defPropNums + (int)((1f - (float)attackedUnit.nowHp / attackedUnit.fullHp) / (LoadJsonFile.GetGameValue(110) / 100f) * LoadJsonFile.GetGameValue(111));
                                        break;
                                    case "11":
                                        //白马，自身血量每降低10%，提高5%免伤
                                        defPropNums = defPropNums + (int)((1f - (float)attackedUnit.nowHp / attackedUnit.fullHp) / (LoadJsonFile.GetGameValue(112) / 100f) * LoadJsonFile.GetGameValue(113));
                                        break;
                                    case "58":
                                        //铁骑，单位越多免伤越高
                                        List<FightCardData> tieQiList = attackedUnit.isPlayerCard ? tieQiCardsPy : tieQiCardsEm;
                                        int nowTieQiNums = 0;
                                        for (int i = 0; i < tieQiList.Count; i++)
                                        {
                                            if (tieQiList[i].nowHp > 0)
                                            {
                                                nowTieQiNums++;
                                            }
                                        }
                                        if (nowTieQiNums > 1)
                                        {
                                            defPropNums = defPropNums + Mathf.Min(LoadJsonFile.GetGameValue(114), nowTieQiNums * LoadJsonFile.GetGameValue(115));
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                //羁绊相关免伤
                                Dictionary<int, JiBanActivedClass> jiBanAllTypes = attackedUnit.isPlayerCard ? playerJiBanAllTypes : enemyJiBanAllTypes;
                                //五子良将激活时所受伤害减免30%
                                if (jiBanAllTypes[(int)JiBanSkillName.WuZiLiangJiang].isActived)
                                {
                                    defPropNums = defPropNums + LoadJsonFile.GetGameValue(153);
                                }
                                if (LoadJsonFile.heroTableDatas[attackedUnit.cardId][6] == "3") //吴势力
                                {
                                    //虎踞江东激活时吴国所受伤害减免30%
                                    if (jiBanAllTypes[(int)JiBanSkillName.HuJuJiangDong].isActived)
                                    {
                                        defPropNums = defPropNums + LoadJsonFile.GetGameValue(157);
                                    }
                                    //天作之合激活时吴国所受伤害减免30%
                                    if (jiBanAllTypes[(int)JiBanSkillName.TianZuoZhiHe].isActived)
                                    {
                                        defPropNums = defPropNums + LoadJsonFile.GetGameValue(158);
                                    }
                                }

                                defPropNums = defPropNums > LoadJsonFile.GetGameValue(116) ? LoadJsonFile.GetGameValue(116) : defPropNums;

                                //判断攻击者的伤害类型，获得被攻击者的物理或法术免伤百分比
                                if (attackUnit != null && attackUnit.cardDamageType == 0)
                                {
                                    defPropNums = defPropNums + int.Parse(LoadJsonFile.heroTableDatas[attackedUnit.cardId][23]);
                                }
                                else
                                {
                                    defPropNums = defPropNums + int.Parse(LoadJsonFile.heroTableDatas[attackedUnit.cardId][24]);
                                }
                                defPropNums = Mathf.Min(defPropNums, 100);
                                finalDamage = (int)((100f - defPropNums) / 100f * finalDamage);
                            }

                            //流血状态加成
                            if (attackedUnit.fightState.bleedNums > 0)
                            {
                                finalDamage = (int)(finalDamage * LoadJsonFile.GetGameValue(117) / 100f);
                            }
                            //抵扣防护盾
                            finalDamage = AddOrCutShieldValue(finalDamage, attackedUnit, false);
                        }
                    }
                }
            }
            if (attackedUnit.cardType == 0)
            {
                switch (LoadJsonFile.heroTableDatas[attackedUnit.cardId][5])
                {
                    case "1":
                        //近战兵种受到暴击和会心加盾
                        if (attackedUnit.fightState.dizzyNums <= 0 && attackedUnit.fightState.imprisonedNums <= 0)
                        {
                            if (indexAttackType != 0)
                            {
                                if (attackedUnit.fightState.withStandNums <= 0)
                                {
                                    FightForManager.instance.CreateSateIcon(attackedUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_withStand, true);
                                }
                                attackedUnit.fightState.withStandNums++;
                            }
                        }
                        break;
                    case "12":
                        //神武战意技能
                        if (attackedUnit.fightState.willFightNums <= 0)
                        {
                            FightForManager.instance.CreateSateIcon(attackedUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_willFight, true);
                        }
                        if (attackedUnit.fightState.willFightNums < 10)
                        {
                            attackedUnit.fightState.willFightNums++;
                            attackedUnit.cardObj.transform.Find(StringNameStatic.StateIconPath_willFight + "Din").GetComponent<Image>().color = new Color(1, 1, 1, 0.4f + 0.6f * (attackedUnit.fightState.willFightNums / 10f));
                        }
                        ShowSpellTextObj(attackedUnit.cardObj, "12", false);
                        break;
                    case "41":
                        //敢死死战技能
                        finalDamage = GanSiSiZhanAttack(finalDamage, attackedUnit);
                        break;
                    default:
                        break;
                }
                if (attackUnit != null && attackUnit.fightState.dizzyNums <= 0 && attackUnit.fightState.imprisonedNums <= 0)
                {
                    switch (LoadJsonFile.heroTableDatas[attackUnit.cardId][5])
                    {
                        case "6":
                            //虎卫吸血
                            if (attackUnit.fightState.dizzyNums <= 0 && attackUnit.fightState.imprisonedNums <= 0)
                            {
                                ShowSpellTextObj(attackUnit.cardObj, "6", false);
                                int addHp = finalDamage;
                                attackUnit.nowHp += addHp;
                                AttackedAnimShow(attackUnit, addHp, true);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        return finalDamage;
    }

    //抵消护盾
    private bool OffsetWithStand(FightCardData fightCardData)
    {
        if (fightCardData.fightState.withStandNums > 0)
        {
            fightCardData.fightState.withStandNums--;
            if (fightCardData.fightState.withStandNums <= 0)
            {
                fightCardData.fightState.withStandNums = 0;
                FightForManager.instance.DestroySateIcon(fightCardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_withStand, true);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    //判断行动单位是否有眩晕
    private bool OffsetDizzyState(FightCardData dizzyUnit)
    {
        if (dizzyUnit.fightState.dizzyNums > 0)
        {
            dizzyUnit.isActed = true;
            //Debug.Log("---处于眩晕");
            dizzyUnit.fightState.dizzyNums--;
            if (dizzyUnit.fightState.dizzyNums <= 0)
            {
                dizzyUnit.fightState.dizzyNums = 0;
                FightForManager.instance.DestroySateIcon(dizzyUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_dizzy, true);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    //判断行动单位是否有内助
    private bool OffsetNeiZhuState(FightCardData attackUnit)
    {
        if (attackUnit.fightState.neizhuNums > 0)
        {
            attackUnit.fightState.neizhuNums--;
            if (attackUnit.fightState.neizhuNums <= 0)
            {
                attackUnit.fightState.neizhuNums = 0;
                FightForManager.instance.DestroySateIcon(attackUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_neizhu, false);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    //判断行动单位是否有神助
    private bool OffsetShenZhuState(FightCardData attackUnit)
    {
        if (attackUnit.fightState.shenzhuNums > 0)
        {
            attackUnit.fightState.shenzhuNums--;
            if (attackUnit.fightState.shenzhuNums <= 0)
            {
                attackUnit.fightState.shenzhuNums = 0;
                FightForManager.instance.DestroySateIcon(attackUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_shenzhu, false);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 获得武将免疫减益效果成功几率
    /// </summary>
    /// <param name="cardData"></param>
    /// <returns></returns>
    private int GetCardDebuffSuccessRate(FightCardData cardData)
    {
        int prop = int.Parse(LoadJsonFile.heroTableDatas[cardData.cardId][22]);
        ////羁绊相关减益
        //Dictionary<int, JiBanActivedClass> jiBanAllTypes = cardData.isPlayerCard ? playerJiBanAllTypes : enemyJiBanAllTypes;
        ////魏五奇谋激活时武将减益成功率提升10%
        //if (jiBanAllTypes[(int)JiBanSkillName.WuZiLiangJiang].isActived)
        //{
        //    defPropNums = defPropNums + LoadJsonFile.GetGameValue(153);
        //}
        return prop;
    }

    /// <summary>
    /// 获得武将减益效果成功几率
    /// </summary>
    /// <param name="cardData"></param>
    /// <returns></returns>
    private int GetCardTakeDebuffSuccessRate(FightCardData cardData, int prop)
    {
        int props = prop;
        //羁绊相关减益
        Dictionary<int, JiBanActivedClass> jiBanAllTypes = cardData.isPlayerCard ? playerJiBanAllTypes : enemyJiBanAllTypes;
        //魏五奇谋激活时武将减益成功率提升10%
        if (jiBanAllTypes[(int)JiBanSkillName.WeiWuMouShi].isActived)
        {
            props += LoadJsonFile.GetGameValue(156);
        }
        return props;
    }

    //触发眩晕
    private void TakeOneUnitDizzed(FightCardData attackedUnit, int prob, FightCardData attackUnit = null)
    {
        if (attackUnit != null)
            prob = GetCardTakeDebuffSuccessRate(attackUnit, prob);
        if (attackedUnit.cardType == 0 && TakeSpecialAttack(prob))
        {
            //判断免疫负面状效果触发
            if (!TakeSpecialAttack(GetCardDebuffSuccessRate(attackedUnit)))
            {
                if (attackedUnit.fightState.dizzyNums <= 0)
                {
                    FightForManager.instance.CreateSateIcon(attackedUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_dizzy, true);
                }
                attackedUnit.fightState.dizzyNums++;
            }
        }
    }

    //触发禁锢
    private void TakeToImprisoned(FightCardData attackedUnit, int prob, FightCardData attackUnit = null)
    {
        if (attackUnit != null)
            prob = GetCardTakeDebuffSuccessRate(attackUnit, prob);
        if (attackedUnit.cardType == 0 && TakeSpecialAttack(prob))
        {
            if (!TakeSpecialAttack(GetCardDebuffSuccessRate(attackedUnit)))
            {
                if (attackedUnit.fightState.imprisonedNums <= 0)
                {
                    FightForManager.instance.CreateSateIcon(attackedUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_imprisoned, true);
                }
                attackedUnit.fightState.imprisonedNums += 1;
                ShowSpellTextObj(attackedUnit.cardObj, LoadJsonFile.GetStringText(11), true, true);
            }
        }
    }

    //触发卸甲
    private void TakeToRemoveArmor(FightCardData attackedUnit, int prob, FightCardData attackUnit = null)
    {
        if (attackUnit != null)
            prob = GetCardTakeDebuffSuccessRate(attackUnit, prob);
        if (attackedUnit.cardType == 0 && TakeSpecialAttack(prob))
        {
            if (!TakeSpecialAttack(GetCardDebuffSuccessRate(attackedUnit)))
            {
                if (attackedUnit.fightState.removeArmorNums <= 0)
                {
                    FightForManager.instance.CreateSateIcon(attackedUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_removeArmor, true);
                }
                attackedUnit.fightState.removeArmorNums += 1;
            }
        }
    }

    //触发流血
    private void TakeToBleed(FightCardData attackedUnit, int prob, FightCardData attackUnit = null)
    {
        if (attackUnit != null)
            prob = GetCardTakeDebuffSuccessRate(attackUnit, prob);
        if (attackedUnit.cardType == 0 && TakeSpecialAttack(prob))
        {
            if (!TakeSpecialAttack(GetCardDebuffSuccessRate(attackedUnit)))
            {
                if (attackedUnit.fightState.bleedNums <= 0)
                {
                    FightForManager.instance.CreateSateIcon(attackedUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_bleed, true);
                }
                attackedUnit.fightState.bleedNums++;
                ShowSpellTextObj(attackedUnit.cardObj, LoadJsonFile.GetStringText(16), true, true);
            }
        }
    }

    //触发中毒
    private void TakeToPoisoned(FightCardData attackedUnit, int prob, FightCardData attackUnit = null)
    {
        if (attackUnit != null)
            prob = GetCardTakeDebuffSuccessRate(attackUnit, prob);
        if (attackedUnit.cardType == 0 && TakeSpecialAttack(prob))
        {
            if (!TakeSpecialAttack(GetCardDebuffSuccessRate(attackedUnit)))
            {
                if (attackedUnit.fightState.poisonedNums <= 0)
                {
                    FightForManager.instance.CreateSateIcon(attackedUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_poisoned, true);
                }
                attackedUnit.fightState.poisonedNums++;
                ShowSpellTextObj(attackedUnit.cardObj, LoadJsonFile.GetStringText(12), true, true);
            }
        }
    }

    //触发灼烧
    private void TakeToBurn(FightCardData attackedUnit, int prob, FightCardData attackUnit = null)
    {
        if (attackUnit != null)
            prob = GetCardTakeDebuffSuccessRate(attackUnit, prob);
        if (attackedUnit.cardType == 0 && TakeSpecialAttack(prob))
        {
            if (!TakeSpecialAttack(GetCardDebuffSuccessRate(attackedUnit)))
            {
                if (attackedUnit.fightState.burnedNums <= 0)
                {
                    FightForManager.instance.CreateSateIcon(attackedUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_burned, true);
                }
                attackedUnit.fightState.burnedNums++;
                ShowSpellTextObj(attackedUnit.cardObj, LoadJsonFile.GetStringText(20), true, true);
            }
        }
    }

    //触发怯战
    private void TakeToCowardly(FightCardData attackedUnit, int prob, FightCardData attackUnit = null)
    {
        if (attackUnit != null)
            prob = GetCardTakeDebuffSuccessRate(attackUnit, prob);
        if (attackedUnit.cardType == 0 && TakeSpecialAttack(prob))
        {
            if (!TakeSpecialAttack(GetCardDebuffSuccessRate(attackedUnit)))
            {
                if (attackedUnit.fightState.cowardlyNums <= 0)
                {
                    FightForManager.instance.CreateSateIcon(attackedUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_cowardly, true);
                }
                attackedUnit.fightState.cowardlyNums += 1;
                ShowSpellTextObj(attackedUnit.cardObj, LoadJsonFile.GetStringText(21), true, true);
            }
        }
    }

    //灼烧触发(暂无上限数值)
    private void BurningFightUnit(FightCardData cardData)
    {
        ShowSpellTextObj(cardData.cardObj, LoadJsonFile.GetStringText(20), true, true);

        if (cardData.fightState.invincibleNums <= 0)
        {
            int cutHpNum = (int)(LoadJsonFile.GetGameValue(118) / 100f * cardData.fullHp);
            cutHpNum = AddOrCutShieldValue(cutHpNum, cardData, false);
            cardData.nowHp -= cutHpNum;
            AttackedAnimShow(cardData, cutHpNum, false);
        }

        cardData.fightState.burnedNums--;
        PlayAudioForSecondClip(87, 0);
        if (cardData.fightState.burnedNums <= 0)
        {
            cardData.fightState.burnedNums = 0;
            FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_burned, true);
        }
    }

    /// <summary>
    /// 辅佐添加的防护盾增减值
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="attackUnit"></param>
    /// <param name="isAdd"></param>
    /// <returns></returns>
    private int AddOrCutShieldValue(int damage, FightCardData attackUnit, bool isAdd)
    {
        int finalDamage = damage;
        if (attackUnit.cardType == 0)
        {
            if (isAdd)
            {
                if (attackUnit.fightState.shieldValue <= 0)
                {
                    FightForManager.instance.CreateSateIcon(attackUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_shield, true);
                }
                attackUnit.fightState.shieldValue += damage;
                attackUnit.fightState.shieldValue = Mathf.Min(attackUnit.fightState.shieldValue, LoadJsonFile.GetGameValue(119));
                float fadeFlo = Mathf.Max(0.3f, attackUnit.fightState.shieldValue / (float)LoadJsonFile.GetGameValue(119));
                attackUnit.cardObj.transform.Find(StringNameStatic.StateIconPath_shield + "Din").GetComponent<Image>().color = new Color(1, 1, 1, fadeFlo);
            }
            else
            {
                if (damage > 0)
                {
                    if (damage < attackUnit.fightState.shieldValue)
                    {
                        attackUnit.fightState.shieldValue -= damage;
                        finalDamage = 0;
                        float fadeFlo = Mathf.Max(0.3f, attackUnit.fightState.shieldValue / (float)LoadJsonFile.GetGameValue(119));
                        attackUnit.cardObj.transform.Find(StringNameStatic.StateIconPath_shield + "Din").GetComponent<Image>().color = new Color(1, 1, 1, fadeFlo);
                    }
                    else
                    {
                        if (attackUnit.fightState.shieldValue > 0)
                        {
                            finalDamage = damage - attackUnit.fightState.shieldValue;
                            attackUnit.fightState.shieldValue = 0;
                            FightForManager.instance.DestroySateIcon(attackUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_shield, true);
                        }
                    }
                }
            }
        }
        return finalDamage;
    }

    /// <summary>
    /// 技能特效表现
    /// </summary>
    /// <param name="attackedUnit"></param>
    /// <param name="isPuGong"></param>
    /// <param name="effectName"></param>
    /// <returns></returns>
    public GameObject AttackToEffectShow(FightCardData attackedUnit, bool isPuGong, string effectName = "")
    {
        GameObject effectObj = new GameObject();
        PlayerDataForGame.garbageStationObjs.Add(effectObj);

        if (isPuGong)
        {
            effectObj = EffectsPoolingControl.instance.GetEffectToFight("0A", 0.5f, attackedUnit.cardObj.transform);
            effectObj.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        }
        else
        {
            effectObj = EffectsPoolingControl.instance.GetEffectToFight(effectName, 1f, attackedUnit.cardObj.transform);
        }

        if (indexAttackType != 0) //非普通攻击
        {
            effectObj.transform.localScale = new Vector3(effectObj.transform.localScale.x * 1.5f, effectObj.transform.localScale.y * 1.5f, effectObj.transform.localScale.z * 1.5f);
        }
        else
        {
            if (effectObj.transform.localScale.x != 1f)
            {
                effectObj.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        return effectObj;
    }

    /// <summary>
    /// 被击表现
    /// </summary>
    /// <param name="attackedUnit"></param>
    /// <param name="cutHpNum"></param>
    /// <param name="isAdd"></param>
    public void AttackedAnimShow(FightCardData attackedUnit, int cutHpNum, bool isAdd)
    {
        if (cutHpNum < 0)
        {
            isAdd = true;
        }
        GameObject effectObj = new GameObject();
        PlayerDataForGame.garbageStationObjs.Add(effectObj);

        effectObj = EffectsPoolingControl.instance.GetEffectToFight("dropBlood", 1.5f, attackedUnit.cardObj.transform);
        if (isAdd)
        {
            effectObj.GetComponentInChildren<Text>().text = "+" + Mathf.Abs(cutHpNum);
            effectObj.GetComponentInChildren<Text>().color = ColorDataStatic.huiFu_green;
        }
        else
        {
            effectObj.GetComponentInChildren<Text>().text = "-" + Mathf.Abs(cutHpNum);
            effectObj.GetComponentInChildren<Text>().color = Color.red;
            if (cutHpNum > 0)
            {
                attackedUnit.cardObj.transform.DOShakePosition(0.3f, new Vector3(10, 20, 10));
            }
            if (indexAttackType != 0)
            {
                effectObj.transform.localScale = new Vector3(effectObj.transform.localScale.x * 1.3f, effectObj.transform.localScale.y * 1.3f, effectObj.transform.localScale.z * 1.3f);
                Vector3 vec3 = fightBackForShake.transform.position;
                if (indexAttackType == 1)//会心
                {
                    ShowSpellTextObj(attackedUnit.cardObj, LoadJsonFile.GetStringText(22), true, true);
                    fightBackForShake.transform.DOShakePosition(0.25f, doShakeIntensity).OnComplete(delegate ()
                    {
                        fightBackForShake.transform.position = vec3;
                    });
                }
                else //暴击
                {
                    ShowSpellTextObj(attackedUnit.cardObj, LoadJsonFile.GetStringText(23), true, true);
                    fightBackForShake.transform.DOShakePosition(0.25f, doShakeIntensity).OnComplete(delegate ()
                    {
                        fightBackForShake.transform.position = vec3;
                    });
                }
            }
            else
            {
                if (effectObj.transform.localScale.x != 1f)
                {
                    effectObj.transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }

        //更新血条
        UpdateUnitHpShow(attackedUnit);

        InitCardAfterFightedState(attackedUnit);
    }

    //特殊单位普攻特效
    private void SpecilSkillNeedPuGongFun(FightCardData attackedUnit)
    {
        if (isNeedToAttack)
        {
            PlayAudioForSecondClip(0, 0);
            AttackToEffectShow(attackedUnit, true);
        }
    }

    //更新血条显示
    public void UpdateUnitHpShow(FightCardData updateUnit)
    {
        if (updateUnit.nowHp <= 0)
        {
            updateUnit.nowHp = 0;
            if (updateUnit.cardType != 522)
            {
                updateUnit.cardObj.transform.GetChild(9).gameObject.SetActive(true);
            }
            else
            {
                updateUnit.cardObj.transform.GetChild(4).gameObject.SetActive(true);
            }
        }
        if (updateUnit.nowHp > updateUnit.fullHp)
        {
            updateUnit.nowHp = updateUnit.fullHp;
        }
        updateUnit.cardObj.transform.GetChild(2).GetComponent<Image>().fillAmount = 1f - updateUnit.nowHp / (float)updateUnit.fullHp;
    }

    /// <summary>
    /// 技能文字表现
    /// </summary>
    /// <param name="fightCardObj"></param>
    /// <param name="showTextName"></param>
    /// <param name="isHorizontal"></param>
    /// <param name="isRed"></param>
    public void ShowSpellTextObj(GameObject fightCardObj, string showTextName, bool isHorizontal, bool isRed = true)
    {
        GameObject effectObj = new GameObject();
        PlayerDataForGame.garbageStationObjs.Add(effectObj);

        if (isHorizontal)
        {
            Transform go = fightCardObj.transform.Find("spellTextH(Clone)");
            if (go != null)
            {
                go.gameObject.SetActive(false);
            }
            effectObj = EffectsPoolingControl.instance.GetEffectToFight("spellTextH", 1.5f, fightCardObj.transform);
            effectObj.GetComponentInChildren<Text>().text = showTextName;
            effectObj.GetComponentInChildren<Text>().color = isRed ? Color.red : ColorDataStatic.huiFu_green;
        }
        else
        {
            Transform go = fightCardObj.transform.Find("spellTextV(Clone)");
            if (go != null)
            {
                go.gameObject.SetActive(false);
            }
            effectObj = EffectsPoolingControl.instance.GetEffectToFight("spellTextV", 1.5f, fightCardObj.transform);
            effectObj.GetComponentsInChildren<Image>()[1].sprite = Resources.Load("Image/battle/" + showTextName, typeof(Sprite)) as Sprite;
        }
    }

    /// <summary>
    /// 战斗结束上阵卡牌回复血量消除相关状态
    /// </summary>
    private void CollectiveRecoveryHp()
    {
        //尝试消除统帅火焰
        if (tongShuaiBurnRoundPy != -1)
        {
            for (int i = 0; i < GoalGfSetFireRound[tongShuaiBurnRoundPy].Length; i++)
            {
                Transform obj = FightForManager.instance.enemyCardsPos[GoalGfSetFireRound[tongShuaiBurnRoundPy][i]].transform.Find(StringNameStatic.StateIconPath_burned);
                if (obj != null)
                    Destroy(obj.gameObject);
            }
            tongShuaiBurnRoundPy = -1;
        }
        if (tongShuaiBurnRoundEm != -1)
        {
            for (int i = 0; i < GoalGfSetFireRound[tongShuaiBurnRoundEm].Length; i++)
            {
                Transform obj = FightForManager.instance.playerCardsPos[GoalGfSetFireRound[tongShuaiBurnRoundEm][i]].transform.Find(StringNameStatic.StateIconPath_burned);
                if (obj != null)
                    Destroy(obj.gameObject);
            }
            tongShuaiBurnRoundEm = -1;
        }

        FightCardData fightCardData;
        //我方
        for (int i = 0; i < FightForManager.instance.playerFightCardsDatas.Length; i++)
        {
            fightCardData = FightForManager.instance.playerFightCardsDatas[i];
            if (i != 17 && fightCardData != null && fightCardData.nowHp > 0)
            {
                int addtionNums = (int)(fightCardData.fullHp * fightCardData.hpr / 100f);
                fightCardData.nowHp += addtionNums;
                ShowSpellTextObj(fightCardData.cardObj, LoadJsonFile.GetStringText(15), true, false);
                AttackedAnimShow(fightCardData, addtionNums, true);
                if (fightCardData.cardType == 0)
                {
                    OnClearCardStateUpdate(fightCardData);  //消除所有状态
                }
            }
        }
    }

    //播放特殊音效
    public void PlayAudioForSecondClip(int clipIndex, float delayedTime)
    {
        if (AudioController0.instance.ChangeAudioClip(WarsUIManager.instance.audioClipsFightEffect[clipIndex], WarsUIManager.instance.audioVolumeFightEffect[clipIndex]))
        {
            AudioController0.instance.PlayAudioSource(0);
        }
        else
        {
            AudioController0.instance.audioSource.volume = AudioController0.instance.audioSource.volume * 0.75f;
            audioSource.clip = WarsUIManager.instance.audioClipsFightEffect[clipIndex];
            audioSource.volume = WarsUIManager.instance.audioVolumeFightEffect[clipIndex];
            if (AudioController0.instance.isPlayMusic != 1)
                return;
            audioSource.PlayDelayed(delayedTime);
        }
    }


    //计算是否触发特殊攻击状态
    private bool TakeSpecialAttack(int odds)
    {
        int num = Random.Range(1, 101);
        if (num <= odds)
            return true;
        else
            return false;
    }

    //从list中随机抽取对应数量的元素
    private List<int> BackRandsList(List<int> parentList, int childsCount)
    {
        List<int> randsList = new List<int>();

        if (parentList.Count <= childsCount)
        {
            randsList = parentList;//LoadJsonFile.DeepClone<int>(parentList);
        }
        else
        {
            for (int i = 0; i < childsCount; i++)
            {
                int rand = parentList[Random.Range(0, parentList.Count)];
                while (randsList.IndexOf(rand) != -1)
                {
                    rand = parentList[Random.Range(0, parentList.Count)];
                }
                randsList.Add(rand);
            }
        }
        return randsList;
    }

    //设置卡牌的parent
    private void ChangeGameObjParent(GameObject cardObj, Transform parent)
    {
        cardObj.transform.SetParent(parent);
    }

    /// <summary>
    /// 全屏技能特效展示，0会心一击
    /// </summary>
    [SerializeField]
    GameObject[] fullScreenEffectObjs;
    [SerializeField]
    Transform[] jBEffectShowPos;    //0敌方1我方位置

    //关闭所有开启的全屏特技
    private void CloseAllFullScreenEffect()
    {
        for (int i = 0; i < fullScreenEffectObjs.Length; i++)
        {
            if (fullScreenEffectObjs[i].activeSelf)
            {
                fullScreenEffectObjs[i].SetActive(false);
            }
        }
    }

    //全屏技能特效展示,会心一击,羁绊
    private void ShowAllScreenFightEffect(FullScreenEffectName fullScreenEffectName, int indexResPic = 0)
    {
        GameObject effectObj = fullScreenEffectObjs[(int)fullScreenEffectName];
        if (effectObj.activeSelf)
        {
            effectObj.SetActive(false);
        }
        switch (fullScreenEffectName)
        {
            case FullScreenEffectName.HuiXinEffect:
                PlayAudioForSecondClip(92, 0);
                break;
            case FullScreenEffectName.JiBanEffect:
                effectObj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load("Image/JiBan/art/" + indexResPic, typeof(Sprite)) as Sprite;
                effectObj.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/JiBan/name_h/" + indexResPic, typeof(Sprite)) as Sprite;
                PlayAudioForSecondClip(100, 0);
                break;
            default:
                break;
        }
        effectObj.SetActive(true);
    }

    /// <summary>
    /// ////////////////战斗主线逻辑//////////////////////////////////////////////////
    /// </summary>

    //位置列攻击目标选择次序
    private int[][] AttackSelectionOrder = new int[5][]
    {
        new int[11]{ 0, 2, 3, 5, 7, 8, 10,12,13,15,17},     //0列
        new int[11]{ 1, 2, 4, 6, 7, 9, 11,12,14,16,17},     //1列
        new int[12]{ 0, 1, 2, 5, 6, 7, 10,11,12,15,16,17},  //2列
        new int[8] { 0, 3, 5, 8, 10,13,15,17},              //3列
        new int[8] { 1, 4, 6, 9, 11,14,16,17},              //4列
    };

    float roundTime = 1.5f;   //回合开始倒计时
    float timer = 0;

    [SerializeField]
    Slider roundTimeSlider; //回合时间条
    bool isFirstRound;

    //一场战斗开始前数据初始化
    public void InitStartFight()
    {
        isRoundBegin = false;
        recordWinner = 0;
        roundNums = 0;
        CloseAllFullScreenEffect();
        timer = 0;
        startFightBtn.GetComponent<Button>().interactable = true;
        startFightBtn.GetComponent<Animator>().SetBool("isShow", true);
        isFirstRound = true;
        roundTimeSlider.gameObject.SetActive(false);
        StartAddSomeState();
    }

    //战斗开始前的属性附加
    private void StartAddSomeState()
    {
        FightCardData fightCardData;
        for (int i = 0; i < FightForManager.instance.playerFightCardsDatas.Length; i++)
        {
            fightCardData = FightForManager.instance.playerFightCardsDatas[i];
            if (i != 17 && fightCardData != null && fightCardData.cardType == 0 && fightCardData.nowHp > 0)
            {
                switch (LoadJsonFile.heroTableDatas[fightCardData.cardId][5])
                {
                    case "4"://盾兵
                        if (fightCardData.fightState.withStandNums <= 0)
                        {
                            FightForManager.instance.CreateSateIcon(fightCardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_withStand, true);
                        }
                        fightCardData.fightState.withStandNums = 1;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //按钮方法-开始战斗
    public void OnClickForRoundBegin()
    {
        if (!isRoundBegin)
        {
            PlayAudioForSecondClip(93, 0);

            isFirstRound = false;
            RoundBeginFun();
        }
    }

    //实时更新等待条
    private void LateUpdate()
    {
        if (!isRoundBegin && !isFirstRound && autoFightTog.isOn)
        {
            timer += Time.deltaTime;
            if (timer >= roundTime)
            {
                RoundBeginFun();
                timer = 0;
            }
            roundTimeSlider.value = 1 - timer / roundTime;
        }
    }

    //锁定玩家上场卡牌
    private void LockFightCardState()
    {
        for (int i = 0; i < FightForManager.instance.playerFightCardsDatas.Length; i++)
        {
            if (i != 17 && FightForManager.instance.playerFightCardsDatas[i] != null)
            {
                if (!FightForManager.instance.playerFightCardsDatas[i].cardObj.GetComponent<CardForDrag>().isFightCard)
                {
                    FightForManager.instance.playerFightCardsDatas[i].cardObj.GetComponent<CardForDrag>().isFightCard = true;
                    FightForManager.instance.playerFightCardsDatas[i].cardObj.transform.GetChild(8).gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// 回合开始方法
    /// </summary>
    private void RoundBeginFun()
    {
        timer = 0;
        isRoundBegin = true;
        roundTimeSlider.fillRect.GetComponent<Image>().color = Color.white;
        roundTimeSlider.gameObject.SetActive(false);
        startFightBtn.GetComponent<Button>().interactable = false;
        startFightBtn.GetComponent<Animator>().SetBool("isShow", false);
        LockFightCardState();
        StartCoroutine(LiteForStartRound());
    }

    //等待战鼓消失动画结束后,回合开始数据重置
    IEnumerator LiteForStartRound()
    {
        yield return new WaitForSeconds(1f);
        roundNums++;
        targetIndex = -1;
        fightUnitIndex = 0;
        isPlayerRound = true;
        timerForFight = 0;
        stateOfFight = StateOfFight.ReadyForFight;

        float updateStateTime = 0;  //刷新状态需等待的时间
        updateStateTime = UpdateCardDataBeforeRound();
        yield return new WaitForSeconds(updateStateTime);

        //羁绊部分的战前附加
        yield return StartCoroutine(InitJiBanForStartFight());

        CardMoveToFight();
    }

    /// <summary>
    /// 战斗开始羁绊数据附加到卡牌上
    /// </summary>
    IEnumerator InitJiBanForStartFight()
    {
        float waitTime = 0;
        //玩家部分
        foreach (var item in playerJiBanAllTypes)
        {
            if (item.Value.isActived)
            {
                //Debug.Log("玩家触发羁绊： " + item.Value.jiBanIndex);
                ShowAllScreenFightEffect(FullScreenEffectName.JiBanEffect, item.Value.jiBanIndex);
                yield return new WaitForSeconds(1f);
                waitTime = JiBanAddStateForCard(item.Value, true);
                yield return new WaitForSeconds(waitTime);
            }
        }
        //敌人部分
        foreach (var item in enemyJiBanAllTypes)
        {
            if (item.Value.isActived)
            {
                //Debug.Log("敌方触发羁绊： " + item.Value.jiBanIndex);
                ShowAllScreenFightEffect(FullScreenEffectName.JiBanEffect, item.Value.jiBanIndex);
                yield return new WaitForSeconds(1f);
                waitTime = JiBanAddStateForCard(item.Value, false);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    //给卡牌上附加羁绊属性
    private float JiBanAddStateForCard(JiBanActivedClass jiBanActivedClass, bool isPlayer)
    {
        FightCardData fightCardData;
        FightCardData[] cardDatas = isPlayer ? FightForManager.instance.playerFightCardsDatas : FightForManager.instance.enemyFightCardsDatas;
        FightCardData[] otherCardDatas = isPlayer ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        float waitTime = 0f;
        switch ((JiBanSkillName)jiBanActivedClass.jiBanIndex)
        {
            case JiBanSkillName.TaoYuanJieYi:
                //30%概率分别为羁绊武将增加1层【神助】
                for (int i = 0; i < jiBanActivedClass.cardTypeLists.Count; i++)
                {
                    for (int j = 0; j < jiBanActivedClass.cardTypeLists[i].cardLists.Count; j++)
                    {
                        fightCardData = jiBanActivedClass.cardTypeLists[i].cardLists[j];
                        if (fightCardData != null && fightCardData.nowHp > 0)
                        {
                            AttackToEffectShow(fightCardData, false, "JB" + jiBanActivedClass.jiBanIndex);
                            if (TakeSpecialAttack(LoadJsonFile.GetGameValue(134)))
                            {
                                if (fightCardData.fightState.shenzhuNums <= 0)
                                {
                                    FightForManager.instance.CreateSateIcon(fightCardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_shenzhu, false);
                                }
                                fightCardData.fightState.shenzhuNums++;
                            }
                        }
                    }
                }
                waitTime = 1f;
                break;
            case JiBanSkillName.WuHuShangJiang:
                //对敌方全体武将造成一次物理攻击（平均*0.5），并有75%概率造成【怯战】
                PlayAudioForSecondClip(101, 0);
                fullScreenEffectObjs[2].SetActive(false);
                fullScreenEffectObjs[2].transform.position = jBEffectShowPos[isPlayer ? 0 : 1].position;
                fullScreenEffectObjs[2].SetActive(true);

                int damage = 0; //记录总伤害
                int heroNums = 0;
                for (int i = 0; i < jiBanActivedClass.cardTypeLists.Count; i++)
                {
                    for (int j = 0; j < jiBanActivedClass.cardTypeLists[i].cardLists.Count; j++)
                    {
                        fightCardData = jiBanActivedClass.cardTypeLists[i].cardLists[j];
                        if (fightCardData != null && fightCardData.nowHp > 0)
                        {
                            damage += fightCardData.damage;
                            heroNums++;
                            AttackToEffectShow(fightCardData, false, "JB" + jiBanActivedClass.jiBanIndex);
                        }
                    }
                }
                damage = (int)(damage * LoadJsonFile.GetGameValue(149) / heroNums / 100f);
                for (int i = 0; i < otherCardDatas.Length; i++)
                {
                    if (otherCardDatas[i] != null && otherCardDatas[i].cardType == 0 && otherCardDatas[i].nowHp > 0)
                    {
                        int nowDamage = DefDamageProcessFun(null, otherCardDatas[i], damage);
                        otherCardDatas[i].nowHp -= nowDamage;
                        AttackToEffectShow(otherCardDatas[i], true);
                        AttackedAnimShow(otherCardDatas[i], nowDamage, false);
                        TakeToCowardly(otherCardDatas[i], LoadJsonFile.GetGameValue(150));
                    }
                }
                waitTime = 2f;
                break;
            case JiBanSkillName.WoLongFengChu:
                //概率分别为羁绊武将增加1层【神助】
                for (int i = 0; i < jiBanActivedClass.cardTypeLists.Count; i++)
                {
                    for (int j = 0; j < jiBanActivedClass.cardTypeLists[i].cardLists.Count; j++)
                    {
                        fightCardData = jiBanActivedClass.cardTypeLists[i].cardLists[j];
                        if (fightCardData != null && fightCardData.nowHp > 0)
                        {
                            AttackToEffectShow(fightCardData, false, "JB" + jiBanActivedClass.jiBanIndex);
                            if (TakeSpecialAttack(LoadJsonFile.GetGameValue(136)))
                            {
                                if (fightCardData.fightState.shenzhuNums <= 0)
                                {
                                    FightForManager.instance.CreateSateIcon(fightCardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_shenzhu, false);
                                }
                                fightCardData.fightState.shenzhuNums++;
                            }
                        }
                    }
                }
                waitTime = 1f;
                break;
            case JiBanSkillName.HuChiELai:
                //30 % 概率分别为羁绊武将增加1层【护盾】
                for (int i = 0; i < jiBanActivedClass.cardTypeLists.Count; i++)
                {
                    for (int j = 0; j < jiBanActivedClass.cardTypeLists[i].cardLists.Count; j++)
                    {
                        fightCardData = jiBanActivedClass.cardTypeLists[i].cardLists[j];
                        if (fightCardData != null && fightCardData.nowHp > 0)
                        {
                            AttackToEffectShow(fightCardData, false, "JB" + jiBanActivedClass.jiBanIndex);
                            if (TakeSpecialAttack(LoadJsonFile.GetGameValue(137)))
                            {
                                if (fightCardData.fightState.withStandNums <= 0)
                                {
                                    FightForManager.instance.CreateSateIcon(fightCardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_withStand, true);
                                }
                                fightCardData.fightState.withStandNums++;
                            }
                        }
                    }
                }
                waitTime = 1f;
                break;
            case JiBanSkillName.WuZiLiangJiang:
                //30 % 概率分别为羁绊武将增加1层【护盾】
                for (int i = 0; i < jiBanActivedClass.cardTypeLists.Count; i++)
                {
                    for (int j = 0; j < jiBanActivedClass.cardTypeLists[i].cardLists.Count; j++)
                    {
                        fightCardData = jiBanActivedClass.cardTypeLists[i].cardLists[j];
                        if (fightCardData != null && fightCardData.nowHp > 0)
                        {
                            AttackToEffectShow(fightCardData, false, "JB" + jiBanActivedClass.jiBanIndex);
                            if (TakeSpecialAttack(LoadJsonFile.GetGameValue(138)))
                            {
                                if (fightCardData.fightState.withStandNums <= 0)
                                {
                                    FightForManager.instance.CreateSateIcon(fightCardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_withStand, true);
                                }
                                fightCardData.fightState.withStandNums++;
                            }
                        }
                    }
                }
                waitTime = 1f;
                break;
            case JiBanSkillName.WeiWuMouShi:
                //对敌方全体武将造成一次物理攻击（平均*0.5），并有20%概率造成【眩晕】
                PlayAudioForSecondClip(103, 0);
                fullScreenEffectObjs[4].SetActive(false);
                fullScreenEffectObjs[4].transform.position = jBEffectShowPos[isPlayer ? 0 : 1].position;
                fullScreenEffectObjs[4].SetActive(true);

                int damage1 = 0; //记录总伤害
                int heroNums1 = 0;
                for (int i = 0; i < jiBanActivedClass.cardTypeLists.Count; i++)
                {
                    for (int j = 0; j < jiBanActivedClass.cardTypeLists[i].cardLists.Count; j++)
                    {
                        fightCardData = jiBanActivedClass.cardTypeLists[i].cardLists[j];
                        if (fightCardData != null && fightCardData.nowHp > 0)
                        {
                            damage1 += fightCardData.damage;
                            heroNums1++;
                            AttackToEffectShow(fightCardData, false, "JB" + jiBanActivedClass.jiBanIndex);
                        }
                    }
                }
                damage1 = (int)(damage1 * LoadJsonFile.GetGameValue(154) / heroNums1 / 100f);
                for (int i = 0; i < otherCardDatas.Length; i++)
                {
                    if (otherCardDatas[i] != null && otherCardDatas[i].cardType == 0 && otherCardDatas[i].nowHp > 0)
                    {
                        int nowDamage = DefDamageProcessFun(null, otherCardDatas[i], damage1);
                        otherCardDatas[i].nowHp -= nowDamage;
                        AttackToEffectShow(otherCardDatas[i], true);
                        AttackedAnimShow(otherCardDatas[i], nowDamage, false);
                        TakeOneUnitDizzed(otherCardDatas[i], LoadJsonFile.GetGameValue(155));
                    }
                }
                waitTime = 2f;
                break;
            case JiBanSkillName.HuJuJiangDong:
                //30 % 概率分别为羁绊武将增加1层【护盾】
                for (int i = 0; i < jiBanActivedClass.cardTypeLists.Count; i++)
                {
                    for (int j = 0; j < jiBanActivedClass.cardTypeLists[i].cardLists.Count; j++)
                    {
                        fightCardData = jiBanActivedClass.cardTypeLists[i].cardLists[j];
                        if (fightCardData != null && fightCardData.nowHp > 0)
                        {
                            AttackToEffectShow(fightCardData, false, "JB" + jiBanActivedClass.jiBanIndex);
                            if (TakeSpecialAttack(LoadJsonFile.GetGameValue(140)))
                            {
                                if (fightCardData.fightState.withStandNums <= 0)
                                {
                                    FightForManager.instance.CreateSateIcon(fightCardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_withStand, true);
                                }
                                fightCardData.fightState.withStandNums++;
                            }
                        }
                    }
                }
                waitTime = 1f;
                break;
            case JiBanSkillName.ShuiShiDouDu:
                //对敌方全体武将造成一次隐士攻击（平均*0.5）
                PlayAudioForSecondClip(102, 0);
                fullScreenEffectObjs[3].SetActive(false);
                fullScreenEffectObjs[3].transform.position = jBEffectShowPos[isPlayer ? 0 : 1].position;
                fullScreenEffectObjs[3].SetActive(true);

                int damage2 = 0; //记录总伤害
                int heroNums2 = 0;
                for (int i = 0; i < jiBanActivedClass.cardTypeLists.Count; i++)
                {
                    for (int j = 0; j < jiBanActivedClass.cardTypeLists[i].cardLists.Count; j++)
                    {
                        fightCardData = jiBanActivedClass.cardTypeLists[i].cardLists[j];
                        if (fightCardData != null && fightCardData.nowHp > 0)
                        {
                            damage2 += fightCardData.damage;
                            heroNums2++;
                            AttackToEffectShow(fightCardData, false, "JB" + jiBanActivedClass.jiBanIndex);
                        }
                    }
                }
                damage2 = (int)(damage2 * LoadJsonFile.GetGameValue(159) / heroNums2 / 100f);
                for (int i = 0; i < otherCardDatas.Length; i++)
                {
                    if (otherCardDatas[i] != null && otherCardDatas[i].cardType == 0 && otherCardDatas[i].nowHp > 0)
                    {
                        int nowDamage = DefDamageProcessFun(null, otherCardDatas[i], damage2);
                        otherCardDatas[i].nowHp -= nowDamage;
                        AttackToEffectShow(otherCardDatas[i], true);
                        AttackedAnimShow(otherCardDatas[i], nowDamage, false);
                        //击退
                        int nextPos = otherCardDatas[i].posIndex + 5;
                        if (nextPos <= 19 && otherCardDatas[nextPos] == null)
                        {
                            StartCoroutine(TakeCardPosBack(otherCardDatas, otherCardDatas[i], nextPos, 0.2f, isPlayerRound));
                        }
                    }
                }
                waitTime = 2f;
                break;
            case JiBanSkillName.TianZuoZhiHe:
                //40%概率分别为羁绊武将增加1层【神助】
                for (int i = 0; i < jiBanActivedClass.cardTypeLists.Count; i++)
                {
                    for (int j = 0; j < jiBanActivedClass.cardTypeLists[i].cardLists.Count; j++)
                    {
                        fightCardData = jiBanActivedClass.cardTypeLists[i].cardLists[j];
                        if (fightCardData != null && fightCardData.nowHp > 0)
                        {
                            AttackToEffectShow(fightCardData, false, "JB" + jiBanActivedClass.jiBanIndex);
                            if (TakeSpecialAttack(LoadJsonFile.GetGameValue(142)))
                            {
                                if (fightCardData.fightState.shenzhuNums <= 0)
                                {
                                    FightForManager.instance.CreateSateIcon(fightCardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_shenzhu, false);
                                }
                                fightCardData.fightState.shenzhuNums++;
                            }
                        }
                    }
                }
                waitTime = 1f;
                break;
            case JiBanSkillName.HeBeiSiTingZhu:
                //30%概率分别为羁绊武将增加1层【护盾】
                for (int i = 0; i < jiBanActivedClass.cardTypeLists.Count; i++)
                {
                    for (int j = 0; j < jiBanActivedClass.cardTypeLists[i].cardLists.Count; j++)
                    {
                        fightCardData = jiBanActivedClass.cardTypeLists[i].cardLists[j];
                        if (fightCardData != null && fightCardData.nowHp > 0)
                        {
                            AttackToEffectShow(fightCardData, false, "JB" + jiBanActivedClass.jiBanIndex);
                            if (TakeSpecialAttack(LoadJsonFile.GetGameValue(143)))
                            {
                                if (fightCardData.fightState.withStandNums <= 0)
                                {
                                    FightForManager.instance.CreateSateIcon(fightCardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_withStand, true);
                                }
                                fightCardData.fightState.withStandNums++;
                            }
                        }
                    }
                }
                waitTime = 1f;
                break;
            case JiBanSkillName.JueShiWuShuang:
                //50%概率分别为羁绊武将增加1层【神助】
                for (int i = 0; i < jiBanActivedClass.cardTypeLists.Count; i++)
                {
                    for (int j = 0; j < jiBanActivedClass.cardTypeLists[i].cardLists.Count; j++)
                    {
                        fightCardData = jiBanActivedClass.cardTypeLists[i].cardLists[j];
                        if (fightCardData != null && fightCardData.nowHp > 0)
                        {
                            AttackToEffectShow(fightCardData, false, "JB" + jiBanActivedClass.jiBanIndex);
                            if (TakeSpecialAttack(LoadJsonFile.GetGameValue(144)))
                            {
                                if (fightCardData.fightState.shenzhuNums <= 0)
                                {
                                    FightForManager.instance.CreateSateIcon(fightCardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_shenzhu, false);
                                }
                                fightCardData.fightState.shenzhuNums++;
                            }
                        }
                    }
                }
                waitTime = 1f;
                break;
            case JiBanSkillName.HanMoSanXian:
                ////30%概率分别为羁绊武将增加1层【神助】
                //for (int i = 0; i < jiBanActivedClass.cardTypeLists.Count; i++)
                //{
                //    for (int j = 0; j < jiBanActivedClass.cardTypeLists[i].cardLists.Count; j++)
                //    {
                //        fightCardData = jiBanActivedClass.cardTypeLists[i].cardLists[j];
                //        if (fightCardData != null && fightCardData.nowHp > 0)
                //        {
                //            AttackToEffectShow(fightCardData, false, "JB" + jiBanActivedClass.jiBanIndex);
                //            if (TakeSpecialAttack(LoadJsonFile.GetGameValue(145)))
                //            {
                //                if (fightCardData.fightState.shenzhuNums <= 0)
                //                {
                //                    FightForManager.instance.CreateSateIcon(fightCardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_shenzhu, false);
                //                }
                //                fightCardData.fightState.shenzhuNums++;
                //            }
                //        }
                //    }
                //}
                waitTime = 0f;
                break;
            default:
                break;
        }
        return waitTime;
    }

    //回合开始状态等统一处理
    private float UpdateCardDataBeforeRound()
    {
        float waitTime = 1f;
        int maxRounds = 0;
        int nowRounds = 0;
        indexAttackType = 0;

        gunMuCards.Clear();
        gunShiCards.Clear();

        bool isHadTongShuai = false;    //是否有统帅

        for (int i = 0; i < FightForManager.instance.playerFightCardsDatas.Length; i++)
        {
            if (FightForManager.instance.playerFightCardsDatas[i] != null && FightForManager.instance.playerFightCardsDatas[i].nowHp > 0)
            {
                FightForManager.instance.playerFightCardsDatas[i].isActed = false;
                if (FightForManager.instance.playerFightCardsDatas[i].cardType == 0)
                {
                    nowRounds = UpdateOneCardBeforeRound(FightForManager.instance.playerFightCardsDatas[i]);
                    //是否有统帅
                    if (!isHadTongShuai && FightForManager.instance.playerFightCardsDatas[i].nowHp > 0 &&
                        (LoadJsonFile.heroTableDatas[FightForManager.instance.playerFightCardsDatas[i].cardId][5] == "32" ||
                        LoadJsonFile.heroTableDatas[FightForManager.instance.playerFightCardsDatas[i].cardId][5] == "33"))
                    {
                        isHadTongShuai = true;
                    }
                }
                else
                {
                    if (FightForManager.instance.playerFightCardsDatas[i].cardType == 2)
                    {
                        switch (FightForManager.instance.playerFightCardsDatas[i].cardId)
                        {
                            case 0:
                                //营寨回合开始回血
                                int addHp = (int)(LoadJsonFile.GetGameValue(120) / 100f * FightForManager.instance.playerFightCardsDatas[i].fullHp);
                                FightForManager.instance.playerFightCardsDatas[i].nowHp += addHp;
                                AttackedAnimShow(FightForManager.instance.playerFightCardsDatas[i], addHp, true);
                                break;
                            //case 17:
                            //    //迷雾阵迷雾动画开启
                            //    for (int j = 0; j < FightForManager.instance.playerFightCardsDatas[i].cardObj.transform.childCount; j++)
                            //    {
                            //        if (FightForManager.instance.playerFightCardsDatas[i].cardObj.transform.GetChild(j).name == StringNameStatic.StateIconPath_miWuZhenAddtion + "Din")
                            //        {
                            //            if (!FightForManager.instance.playerFightCardsDatas[i].cardObj.transform.GetChild(j).GetComponent<Animator>().enabled)
                            //            {
                            //                FightForManager.instance.playerFightCardsDatas[i].cardObj.transform.GetChild(j).GetComponent<Animator>().enabled = true;
                            //            }
                            //            else
                            //            {
                            //                break;
                            //            }
                            //        }
                            //    }
                            //    break;
                            default:
                                break;
                        }
                    }
                }
                //滚石滚木列表添加
                if (FightForManager.instance.playerFightCardsDatas[i].cardType == 3)
                {
                    if (FightForManager.instance.playerFightCardsDatas[i].cardId == 9)
                    {
                        gunShiCards.Add(FightForManager.instance.playerFightCardsDatas[i]);
                    }
                    if (FightForManager.instance.playerFightCardsDatas[i].cardId == 10)
                    {
                        gunMuCards.Add(FightForManager.instance.playerFightCardsDatas[i]);
                    }
                }
            }
            if (maxRounds < nowRounds)
                maxRounds = nowRounds;
        }

        if (!isHadTongShuai)
        {
            //尝试消除统帅火焰
            if (tongShuaiBurnRoundPy != -1)
            {
                for (int i = 0; i < GoalGfSetFireRound[tongShuaiBurnRoundPy].Length; i++)
                {
                    Transform obj = FightForManager.instance.enemyCardsPos[GoalGfSetFireRound[tongShuaiBurnRoundPy][i]].transform.Find(StringNameStatic.StateIconPath_burned);
                    if (obj != null)
                        Destroy(obj.gameObject);
                }
                tongShuaiBurnRoundPy = -1;
            }
        }
        else
        {
            isHadTongShuai = false;
        }

        for (int i = 0; i < FightForManager.instance.enemyFightCardsDatas.Length; i++)
        {
            if (FightForManager.instance.enemyFightCardsDatas[i] != null && FightForManager.instance.enemyFightCardsDatas[i].nowHp > 0)
            {
                FightForManager.instance.enemyFightCardsDatas[i].isActed = false;
                if (FightForManager.instance.enemyFightCardsDatas[i].cardType == 0)
                {
                    nowRounds = UpdateOneCardBeforeRound(FightForManager.instance.enemyFightCardsDatas[i]);
                    //是否有统帅
                    if (!isHadTongShuai && FightForManager.instance.enemyFightCardsDatas[i].nowHp > 0 &&
                        (LoadJsonFile.heroTableDatas[FightForManager.instance.enemyFightCardsDatas[i].cardId][5] == "32" ||
                        LoadJsonFile.heroTableDatas[FightForManager.instance.enemyFightCardsDatas[i].cardId][5] == "33"))
                    {
                        isHadTongShuai = true;
                    }
                }
                else
                {
                    if (FightForManager.instance.enemyFightCardsDatas[i].cardType == 2)
                    {
                        switch (FightForManager.instance.enemyFightCardsDatas[i].cardId)
                        {
                            case 0:
                                //营寨回合开始回血
                                int addHp = (int)(LoadJsonFile.GetGameValue(120) / 100f * FightForManager.instance.enemyFightCardsDatas[i].fullHp);
                                FightForManager.instance.enemyFightCardsDatas[i].nowHp += addHp;
                                AttackedAnimShow(FightForManager.instance.enemyFightCardsDatas[i], addHp, true);
                                break;
                            case 17:
                                //迷雾阵迷雾动画开启
                                Transform tran = FightForManager.instance.enemyFightCardsDatas[i].cardObj.transform;
                                for (int j = 0; j < tran.childCount; j++)
                                {
                                    Transform tranChild = tran.GetChild(j);
                                    if (tranChild.name == StringNameStatic.StateIconPath_miWuZhenAddtion + "Din")
                                    {
                                        if (!tranChild.GetComponent<Animator>().enabled)
                                        {
                                            tranChild.GetComponent<Animator>().enabled = true;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                break;
                            case 18:
                                //迷雾阵迷雾动画开启
                                Transform tran0 = FightForManager.instance.enemyFightCardsDatas[i].cardObj.transform;
                                for (int j = 0; j < tran0.childCount; j++)
                                {
                                    Transform tranChild = tran0.GetChild(j);
                                    if (tranChild.name == StringNameStatic.StateIconPath_miWuZhenAddtion + "Din")
                                    {
                                        if (!tranChild.GetComponent<Animator>().enabled)
                                        {
                                            tranChild.GetComponent<Animator>().enabled = true;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                //滚石滚木列表添加
                if (FightForManager.instance.enemyFightCardsDatas[i].cardType == 3)
                {
                    if (FightForManager.instance.enemyFightCardsDatas[i].cardId == 9)
                    {
                        gunShiCards.Add(FightForManager.instance.enemyFightCardsDatas[i]);
                    }
                    if (FightForManager.instance.enemyFightCardsDatas[i].cardId == 10)
                    {
                        gunMuCards.Add(FightForManager.instance.enemyFightCardsDatas[i]);
                    }
                }
            }
            if (maxRounds < nowRounds)
                maxRounds = nowRounds;
        }

        if (!isHadTongShuai)
        {
            //尝试消除统帅火焰
            if (tongShuaiBurnRoundEm != -1)
            {
                for (int i = 0; i < GoalGfSetFireRound[tongShuaiBurnRoundEm].Length; i++)
                {
                    Transform obj = FightForManager.instance.playerCardsPos[GoalGfSetFireRound[tongShuaiBurnRoundEm][i]].transform.Find(StringNameStatic.StateIconPath_burned);
                    if (obj != null)
                        Destroy(obj.gameObject);
                }
                tongShuaiBurnRoundEm = -1;
            }
        }

        return maxRounds * waitTime;
    }
    private int UpdateOneCardBeforeRound(FightCardData cardData)
    {
        int showRounds = 0;
        if (cardData.fightState.poisonedNums > 0) //中毒触发
        {
            showRounds++;
            ShowSpellTextObj(cardData.cardObj, LoadJsonFile.GetStringText(12), true, true);
            PlayAudioForSecondClip(86, 0);

            if (cardData.fightState.invincibleNums <= 0)
            {
                int cutHpNum = (int)(LoadJsonFile.GetGameValue(121) / 100f * cardData.fullHp);
                cutHpNum = AddOrCutShieldValue(cutHpNum, cardData, false);
                cardData.nowHp -= cutHpNum;
                AttackedAnimShow(cardData, cutHpNum, false);
            }

            cardData.fightState.poisonedNums--;
            if (cardData.fightState.poisonedNums <= 0)
            {
                cardData.fightState.poisonedNums = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_poisoned, true);
            }
        }
        return showRounds;
    }


    //回合结束更新卡牌特殊状态
    private void UpdateCardStateAfterRound()
    {
        //我方
        for (int i = 0; i < FightForManager.instance.playerFightCardsDatas.Length; i++)
        {
            if (i != 17 && FightForManager.instance.playerFightCardsDatas[i] != null)
            {
                if (FightForManager.instance.playerFightCardsDatas[i].nowHp > 0)
                {
                    OnCardStateUpdate(FightForManager.instance.playerFightCardsDatas[i]);
                }
                if (FightForManager.instance.playerFightCardsDatas[i].nowHp <= 0)
                {
                    if (FightForManager.instance.playerFightCardsDatas[i].cardType == 0)
                    {
                        //羁绊消除
                        FightForManager.instance.TryToActivatedBond(FightForManager.instance.playerFightCardsDatas[i], false);

                        switch (LoadJsonFile.heroTableDatas[FightForManager.instance.playerFightCardsDatas[i].cardId][5])
                        {
                            case "58": //铁骑阵亡
                                UpdateTieQiStateIconShow(FightForManager.instance.playerFightCardsDatas[i], false);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        if (FightForManager.instance.playerFightCardsDatas[i].cardType == 2) //塔倒了
                        {
                            FightForManager.instance.CardGoIntoBattleProcess(FightForManager.instance.playerFightCardsDatas[i], i, FightForManager.instance.playerFightCardsDatas, false);
                        }
                    }
                    Destroy(FightForManager.instance.playerFightCardsDatas[i].cardObj);
                    FightForManager.instance.playerFightCardsDatas[i] = null;
                    FightForManager.instance.nowHeroNums--;
                    FightForManager.instance.UpdateFightNumTextShow();
                }
            }
        }
        //敌方
        for (int i = 0; i < FightForManager.instance.enemyFightCardsDatas.Length; i++)
        {
            if (i != 17 && FightForManager.instance.enemyFightCardsDatas[i] != null)
            {
                if (FightForManager.instance.enemyFightCardsDatas[i].nowHp > 0)
                {
                    OnCardStateUpdate(FightForManager.instance.enemyFightCardsDatas[i]);
                }
                if (FightForManager.instance.enemyFightCardsDatas[i].nowHp <= 0)
                {
                    if (FightForManager.instance.enemyFightCardsDatas[i].cardType == 0)
                    {
                        //羁绊消除
                        FightForManager.instance.TryToActivatedBond(FightForManager.instance.enemyFightCardsDatas[i], false);

                        switch (LoadJsonFile.heroTableDatas[FightForManager.instance.enemyFightCardsDatas[i].cardId][5])
                        {
                            case "58": //铁骑阵亡
                                UpdateTieQiStateIconShow(FightForManager.instance.enemyFightCardsDatas[i], false);
                                break;

                            default:
                                break;
                        }
                        OnClearCardStateUpdate(FightForManager.instance.enemyFightCardsDatas[i]);
                    }
                    else
                    {
                        if (FightForManager.instance.enemyFightCardsDatas[i].cardType == 2) //塔倒了
                        {
                            FightForManager.instance.CardGoIntoBattleProcess(FightForManager.instance.enemyFightCardsDatas[i], i, FightForManager.instance.enemyFightCardsDatas, false);
                        }
                    }
                    //杀死敌将获得金币
                    getGold += int.Parse(LoadJsonFile.enemyUnitTableDatas[FightForManager.instance.enemyFightCardsDatas[i].unitId][4]);
                    if (LoadJsonFile.enemyUnitTableDatas[FightForManager.instance.enemyFightCardsDatas[i].unitId][5] != "")
                    {
                        string[] arr = LoadJsonFile.enemyUnitTableDatas[FightForManager.instance.enemyFightCardsDatas[i].unitId][5].Split(',');
                        for (int j = 0; j < arr.Length; j++)
                        {
                            if (arr[j] != "")
                            {
                                getBoxsList.Add(int.Parse(arr[j]));
                            }
                        }
                    }
                    Destroy(FightForManager.instance.enemyFightCardsDatas[i].cardObj);
                    FightForManager.instance.enemyFightCardsDatas[i] = null;
                }
            }
        }
    }

    //消除卡牌所有状态图标特效等
    private void OnClearCardStateUpdate(FightCardData cardData)
    {
        try
        {
            if (cardData.fightState.dizzyNums > 0)          //眩晕状态
            {
                cardData.fightState.dizzyNums = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_dizzy, true);
            }
            if (cardData.fightState.imprisonedNums > 0)     //禁锢状态
            {
                cardData.fightState.imprisonedNums = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_imprisoned, true);
            }
            if (cardData.fightState.bleedNums > 0)          //流血状态
            {
                cardData.fightState.bleedNums = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_bleed, true);
            }
            if (cardData.fightState.poisonedNums > 0)       //中毒状态
            {
                cardData.fightState.poisonedNums = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_poisoned, true);
            }
            if (cardData.fightState.burnedNums > 0)         //灼烧触发
            {
                cardData.fightState.burnedNums = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_burned, true);
            }
            if (cardData.fightState.removeArmorNums > 0)    //卸甲状态
            {
                cardData.fightState.removeArmorNums = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_removeArmor, true);
            }
            if (cardData.fightState.withStandNums > 0)      //护盾状态
            {
                cardData.fightState.withStandNums = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_withStand, true);
            }
            if (cardData.fightState.invincibleNums > 0)     //无敌消减
            {
                cardData.fightState.invincibleNums = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_invincible, true);
            }
            if (cardData.fightState.deathFightNums > 0)     //死战状态
            {
                cardData.fightState.deathFightNums = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_deathFight, true);
            }
            if (cardData.fightState.willFightNums > 0)      //战意状态
            {
                cardData.fightState.willFightNums = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_willFight, true);
            }
            if (cardData.fightState.neizhuNums > 0)         //内助状态
            {
                cardData.fightState.neizhuNums = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_neizhu, false);
            }
            if (cardData.fightState.shenzhuNums > 0)        //神助状态
            {
                cardData.fightState.shenzhuNums = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_shenzhu, false);
            }
            if (cardData.fightState.cowardlyNums > 0)       //怯战状态
            {
                cardData.fightState.cowardlyNums = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_cowardly, true);
            }
            if (cardData.fightState.miWuZhenAddtion > 0)    //隐蔽状态
            {
                cardData.fightState.miWuZhenAddtion = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_miWuZhenAddtion, false);
            }
            if (cardData.fightState.shieldValue > 0)        //防护盾状态
            {
                cardData.fightState.shieldValue = 0;
                FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_shield, true);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    //回合结束单个卡牌状态刷新
    private void OnCardStateUpdate(FightCardData cardData)
    {
        try
        {
            if (cardData.fightState.invincibleNums > 0) //无敌消减
            {
                cardData.fightState.invincibleNums--;
                if (cardData.fightState.invincibleNums <= 0)
                {
                    cardData.fightState.invincibleNums = 0;
                    FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_invincible, true);
                }
            }
            if (cardData.fightState.burnedNums > 0) //灼烧触发
            {
                BurningFightUnit(cardData);
            }
            if (cardData.fightState.bleedNums > 0) //流血状态
            {
                cardData.fightState.bleedNums--;
                if (cardData.fightState.bleedNums <= 0)
                {
                    cardData.fightState.bleedNums = 0;
                    FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_bleed, true);
                }
            }
            if (cardData.fightState.removeArmorNums > 0) //卸甲状态
            {
                cardData.fightState.removeArmorNums--;
                if (cardData.fightState.removeArmorNums <= 0)
                {
                    cardData.fightState.removeArmorNums = 0;
                    FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_removeArmor, true);
                }
            }
            if (cardData.fightState.deathFightNums > 0)    //死战状态
            {
                cardData.fightState.deathFightNums--;
                if (cardData.fightState.deathFightNums <= 0)
                {
                    cardData.fightState.deathFightNums = 0;
                    FightForManager.instance.DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_deathFight, true);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    [SerializeField]
    float yuanChengShakeTimeToGo = 0.5f;
    [SerializeField]
    float yuanChengShakeTimeToBack = 0.5f;
    [SerializeField]
    float yuanChengShakeTime = 0.1f;

    //武将行动
    IEnumerator InitiativeHeroAction(FightCardData attackUnit, FightCardData[] cardsDatas)
    {
        float waitTime = 0;
        waitTime = BeforeFightDoThingFun(attackUnit);
        yield return new WaitForSeconds(waitTime);

        /////////前摇//////////
        targetIndex = FindOpponentIndex(attackUnit);  //锁定目标卡牌
        //近战跟远程选择不同的进攻方式
        if (attackUnit.cardMoveType == 0)
        {
            MoveToFightWay1(attackUnit);
            yield return new WaitForSeconds(attackShakeTimeToGo);
        }
        else
        {
            MoveToFightWay0(attackUnit, yuanChengShakeTimeToGo);
            yield return new WaitForSeconds(yuanChengShakeTime);
        }
        /////////攻击//////////
        FightCardData attackedUnit = isPlayerRound ? FightForManager.instance.enemyFightCardsDatas[targetIndex] : FightForManager.instance.playerFightCardsDatas[targetIndex];

        yield return StartCoroutine(PuTongGongji(1f, attackUnit, attackedUnit, true));

        /////////后摇//////////
        if (attackUnit.cardMoveType == 0)
        {
            CardBackToSelfPosFun();
            yield return new WaitForSeconds(attackShakeTimeToBack);
        }
        else
        {
            yield return new WaitForSeconds(yuanChengShakeTimeToBack);
        }
        ////////////////////
        NextCardDoThingFun();
    }

    //卡牌进入战斗
    private void CardMoveToFight()
    {
        FightCardData[] cardDatas;
        FightCardData attackUnit = new FightCardData();
        if (isPlayerRound)
        {
            attackUnit = FightForManager.instance.playerFightCardsDatas[fightUnitIndex];
            cardDatas = FightForManager.instance.playerFightCardsDatas;
        }
        else
        {
            attackUnit = FightForManager.instance.enemyFightCardsDatas[fightUnitIndex];
            cardDatas = FightForManager.instance.enemyFightCardsDatas;
        }
        //主动单位并且可行动
        if (attackUnit != null && attackUnit.activeUnit && !attackUnit.isActed && !OffsetDizzyState(attackUnit) && attackUnit.nowHp > 0)
        {
            attackUnit.isActed = true;
            switch (attackUnit.cardType)
            {
                case 0:
                    StartCoroutine(InitiativeHeroAction(attackUnit, cardDatas));
                    break;
                case 2:
                    StartCoroutine(InitiativeTowerAction(attackUnit, cardDatas));
                    break;
                default:
                    break;
            }
        }
        else
        {
            NextCardDoThingFun();
        }
    }

    //刺客目标选择
    private int CiKeOpponentChoose(FightCardData attackUnit)
    {
        int index = -1;
        int maxDamage = 0;
        FightCardData[] fightCardDatas = attackUnit.isPlayerCard ? FightForManager.instance.enemyFightCardsDatas : FightForManager.instance.playerFightCardsDatas;
        for (int i = 0; i < fightCardDatas.Length; i++)
        {
            if (fightCardDatas[i] != null && fightCardDatas[i].cardType == 0 && fightCardDatas[i].nowHp > 0)
            {
                if (fightCardDatas[i].cardMoveType == 1)
                {
                    if (fightCardDatas[i].damage > maxDamage)
                    {
                        index = i;
                        maxDamage = fightCardDatas[i].damage;
                    }
                }
            }
        }
        return index;
    }

    //锁定目标卡牌
    private int FindOpponentIndex(FightCardData attackUnit)
    {
        if (attackUnit.fightState.imprisonedNums <= 0)//攻击者没有禁锢状态
        {
            switch (LoadJsonFile.heroTableDatas[attackUnit.cardId][5])
            {
                //刺客
                case "25":
                    int chooseIndex = CiKeOpponentChoose(attackUnit);
                    if (chooseIndex != -1)
                        return chooseIndex;
                    break;
                default:
                    break;
            }
        }

        int index = 0;
        int arrIndex = fightUnitIndex % 5;
        if (attackUnit.isPlayerCard)
        {
            for (; index < AttackSelectionOrder[arrIndex].Length; index++)
            {
                if (FightForManager.instance.enemyFightCardsDatas[AttackSelectionOrder[arrIndex][index]] != null
                    && FightForManager.instance.enemyFightCardsDatas[AttackSelectionOrder[arrIndex][index]].nowHp > 0)
                {
                    break;
                }
            }
            if (index >= AttackSelectionOrder[arrIndex].Length)
            {
                //Debug.Log("敌方无存活单位");
                return -1;
            }
        }
        else
        {
            for (; index < AttackSelectionOrder[arrIndex].Length; index++)
            {
                if (FightForManager.instance.playerFightCardsDatas[AttackSelectionOrder[arrIndex][index]] != null
                    && FightForManager.instance.playerFightCardsDatas[AttackSelectionOrder[arrIndex][index]].nowHp > 0)
                {
                    break;
                }
            }
            if (index >= AttackSelectionOrder[arrIndex].Length)
            {
                //Debug.Log("我方无存活单位");
                return -1;
            }
        }
        return AttackSelectionOrder[arrIndex][index];
    }

    //消除统帅火焰
    private void ClearTongShuaiBurnState()
    {
        if (tongShuaiBurnRoundPy != -1)
        {
            for (int i = 0; i < GoalGfSetFireRound[tongShuaiBurnRoundPy].Length; i++)
            {
                Transform obj = FightForManager.instance.enemyCardsPos[GoalGfSetFireRound[tongShuaiBurnRoundPy][i]].transform.Find(StringNameStatic.StateIconPath_burned);
                if (obj != null)
                    Destroy(obj.gameObject);
            }
            tongShuaiBurnRoundPy = -1;
        }
        if (tongShuaiBurnRoundEm != -1)
        {
            for (int i = 0; i < GoalGfSetFireRound[tongShuaiBurnRoundEm].Length; i++)
            {
                Transform obj = FightForManager.instance.playerCardsPos[GoalGfSetFireRound[tongShuaiBurnRoundEm][i]].transform.Find(StringNameStatic.StateIconPath_burned);
                if (obj != null)
                    Destroy(obj.gameObject);
            }
            tongShuaiBurnRoundEm = -1;
        }
    }

    //下一卡牌单位行动
    private void NextCardDoThingFun()
    {
        if (recordWinner != 0)
        {
            ClearTongShuaiBurnState();
            if (recordWinner == 1)
            {
                //Debug.Log("---击败敌方");
                UpdateCardStateAfterRound();

                CollectiveRecoveryHp(); //战斗结束上阵卡牌回复血量

                StartCoroutine(BattleSettlementFun());

                PlayerDataForGame.instance.ClearGarbageStationObj();
            }
            else
            {
                //Debug.Log("---被敌方击败");
                WarsUIManager.instance.BattleOverShow(false);
                PlayerDataForGame.instance.ClearGarbageStationObj();
            }
            //Debug.Log("------战斗结束------");
        }
        else
        {
            if (!isPlayerRound) //敌方单位索引已进行过行动
            {
                fightUnitIndex++;
                if (fightUnitIndex == 17)
                {
                    fightUnitIndex++;
                }
                if (fightUnitIndex > 19)    //回合结束
                {
                    //Debug.Log("----回合结束");
                    stateOfFight = StateOfFight.ReadyForFight;
                    UpdateCardStateAfterRound();
                    isRoundBegin = false;
                    startFightBtn.GetComponent<Button>().interactable = true;
                    startFightBtn.GetComponent<Animator>().SetBool("isShow", true);
                    roundTimeSlider.gameObject.SetActive(autoFightTog.isOn);
                    roundTimeSlider.fillRect.GetComponent<Image>().color = Color.white;
                    roundTimeSlider.fillRect.GetComponent<Image>().DOColor(Color.red, roundTime);
                    return;
                }
            }
            isPlayerRound = !isPlayerRound;
            CardMoveToFight();
        }
    }

    //战前行动准备
    private float BeforeFightDoThingFun(FightCardData attackUnit)
    {
        float needAllTime = 0;
        needAllTime += ConfirmAttackStatus(attackUnit);
        return needAllTime;
    }

    //确认此次攻击状态
    private float ConfirmAttackStatus(FightCardData attackUnit)
    {
        float needTime = 0;
        indexAttackType = 0;
        if (attackUnit.fightState.shenzhuNums <= 0 && attackUnit.fightState.neizhuNums <= 0 && attackUnit.fightState.cowardlyNums > 0) //怯战无法使用暴击和会心一击
        {
            attackUnit.fightState.cowardlyNums--;
            ShowSpellTextObj(attackUnit.cardObj, LoadJsonFile.GetStringText(21), true, true);
            if (attackUnit.fightState.cowardlyNums <= 0)
            {
                attackUnit.fightState.cowardlyNums = 0;
                FightForManager.instance.DestroySateIcon(attackUnit.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_cowardly, true);
            }
        }
        else
        {
            List<string> heroData = LoadJsonFile.heroTableDatas[attackUnit.cardId];
            int huixinPropNums = int.Parse(heroData[14]) + attackUnit.fightState.langyataiAddtion;
            //是否有神助
            if (OffsetShenZhuState(attackUnit))
                huixinPropNums = 100;
            //是否触发会心一击
            if (TakeSpecialAttack(huixinPropNums))
            {
                indexAttackType = 1;
                needTime = 1.2f;
                ShowAllScreenFightEffect(FullScreenEffectName.HuiXinEffect);
            }
            else
            {
                int criPropNums = int.Parse(heroData[12]) + attackUnit.fightState.pilitaiAddtion;
                //是否有内助
                if (OffsetNeiZhuState(attackUnit))
                    criPropNums = 100;
                //是否触发暴击
                if (TakeSpecialAttack(criPropNums))
                {
                    indexAttackType = 2;
                }
            }
        }
        return needTime;
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////
    /// </summary>
    [SerializeField]
    float towerFightTime0 = 0.3f;   //塔行动前摇时间
    [SerializeField]
    float towerFightTime1 = 0.5f;   //塔行动后摇时间
    //主动塔行动
    IEnumerator InitiativeTowerAction(FightCardData attackUnit, FightCardData[] cardsDatas)
    {
        MoveToFightWay0(attackUnit, towerFightTime0);
        yield return new WaitForSeconds(towerFightTime0 / 2);
        FightForManager.instance.ActiveTowerFight(attackUnit, cardsDatas);
        yield return new WaitForSeconds(towerFightTime1);
        //滚石滚木行动
        yield return StartCoroutine(GunMuGunShiSkill(gunMuCards, gunShiCards));
        //消除滚石滚木
        for (int i = 0; i < gunMuCards.Count; i++)
        {
            if (gunMuCards[i].nowHp <= 0)
            {
                gunMuCards.Remove(gunMuCards[i]);
            }
        }
        for (int i = 0; i < gunShiCards.Count; i++)
        {
            if (gunShiCards[i].nowHp <= 0)
            {
                gunShiCards.Remove(gunShiCards[i]);
            }
        }
        NextCardDoThingFun();
    }

    [SerializeField]
    GameObject fireUIObj;
    [SerializeField]
    GameObject boomUIObj;
    [SerializeField]
    GameObject gongKeUIObj;

    //战斗结算
    IEnumerator BattleSettlementFun()
    {
        boomUIObj.transform.position = FightForManager.instance.enemyCardsPos[17].transform.position;
        fireUIObj.transform.position = gongKeUIObj.transform.position = FightForManager.instance.enemyCardsPos[7].transform.position;
        yield return new WaitForSeconds(0.5f);
        PlayAudioForSecondClip(91, 0);
        boomUIObj.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        boomUIObj.SetActive(false);
        //欢呼声
        PlayAudioForSecondClip(90, 0);

        //火焰
        fireUIObj.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gongKeUIObj.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        fireUIObj.SetActive(false);
        gongKeUIObj.SetActive(false);
        yield return new WaitForSeconds(0.1f);

        Time.timeScale = 1;
        
        for (int i = 0; i < FightForManager.instance.enemyFightCardsDatas.Length; i++)
        {
            FightCardData cardData = FightForManager.instance.enemyFightCardsDatas[i];
            if (i != 17 && cardData != null && cardData.nowHp <= 0)
            {
                getGold += int.Parse(LoadJsonFile.enemyUnitTableDatas[cardData.unitId][4]);
                //暂时关闭打死单位获得的战役宝箱
                if (LoadJsonFile.enemyUnitTableDatas[cardData.unitId][5] != "")
                {
                    string[] arr = LoadJsonFile.enemyUnitTableDatas[cardData.unitId][5].Split(',');
                    for (int j = 0; j < arr.Length; j++)
                    {
                        if (arr[j] != "")
                        {
                            getBoxsList.Add(int.Parse(arr[j]));
                        }
                    }
                }
            }
        }
        getGold += int.Parse(LoadJsonFile.battleEventTableDatas[FightForManager.instance.battleIdIndex][5]);
        string[] arr1 = LoadJsonFile.battleEventTableDatas[FightForManager.instance.battleIdIndex][4].Split(',');
        for (int k = 0; k < arr1.Length; k++)
        {
            if (arr1[k] != "")
            {
                getBoxsList.Add(int.Parse(arr1[k]));
            }
        }
        WarsUIManager.instance.goldForCity += getGold;
        WarsUIManager.instance.treasureChestNums += getBoxsList.Count;
        WarsUIManager.instance.UpdateGoldandBoxNumsShow();
        WarsUIManager.instance.ShowOrHideGuideObj(2, true);

        if (getBoxsList.Count > 0)
        {
            for (int i = 0; i < getBoxsList.Count; i++)
            {
                PlayerDataForGame.instance.gbocData.fightBoxs.Add(getBoxsList[i]);
                PlayerDataForGame.instance.getBackTiLiNums = PlayerDataForGame.instance.getBackTiLiNums - PlayerDataForGame.instance.boxForTiLiNums;
            }

            PlayerDataForGame.instance.isNeedSaveData = true;
            LoadSaveData.instance.SaveGameData(4);

            WarsUIManager.instance.eventsWindows[4].transform.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = "×" + getBoxsList.Count;
            WarsUIManager.instance.eventsWindows[4].transform.GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            WarsUIManager.instance.eventsWindows[4].transform.GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(false);
        }
        WarsUIManager.instance.eventsWindows[4].transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "×" + getGold;
        WarsUIManager.instance.eventsWindows[4].transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
        WarsUIManager.instance.eventsWindows[4].transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
        WarsUIManager.instance.eventsWindows[4].SetActive(true);

        getGold = 0;
        getBoxsList.Clear();
    }

    //攻击行动方式0-适用于-主动塔,远程兵
    private void MoveToFightWay0(FightCardData actionUnit, float readyTime)
    {
        actionUnit.cardObj.transform.DOScale(new Vector3(1.15f, 1.15f, 1), readyTime).SetAutoKill(false).OnComplete(delegate ()
        {
            actionUnit.cardObj.transform.DOPlayBackwards();
        });
    }

    //攻击行动方式1-适用于-近战
    private void MoveToFightWay1(FightCardData attackUnit)
    {
        if (fightUnitIndex != -1)
        {
            //stateOfFight = StateOfFight.MoveNow;
            if (isPlayerRound)
            {
                CardMoveToTargetPos(
                    attackUnit.cardObj,
                    new Vector3(
                        FightForManager.instance.enemyCardsPos[targetIndex].transform.position.x,
                        FightForManager.instance.enemyCardsPos[targetIndex].transform.position.y - FightForManager.instance.floDisY,
                        FightForManager.instance.enemyCardsPos[targetIndex].transform.position.z),
                    false,
                    true
                    );
            }
            else
            {
                CardMoveToTargetPos(
                    attackUnit.cardObj,
                    new Vector3(
                        FightForManager.instance.playerCardsPos[targetIndex].transform.position.x,
                        FightForManager.instance.playerCardsPos[targetIndex].transform.position.y + FightForManager.instance.floDisY,
                        FightForManager.instance.playerCardsPos[targetIndex].transform.position.z),
                    false,
                    false
                    );
            }
        }
    }

    //返回原始位置
    private void CardBackToSelfPosFun()
    {
        if (isPlayerRound)
        {
            CardMoveToTargetPos(FightForManager.instance.playerFightCardsDatas[fightUnitIndex].cardObj, FightForManager.instance.playerCardsPos[fightUnitIndex].transform.position, true, true);
        }
        else
        {
            CardMoveToTargetPos(FightForManager.instance.enemyFightCardsDatas[fightUnitIndex].cardObj, FightForManager.instance.enemyCardsPos[fightUnitIndex].transform.position, true, false);
        }
    }

    public float one;   //去
    public float two;   //回
    public float three; //等
    public float four;  //去
    public float posFloat;
    //近战移动方式
    private void CardMoveToTargetPos(GameObject cardObj, Vector3 targetPos, bool isBack, bool isPlayerCard)
    {
        if (isBack)
        {
            cardObj.transform.DOMove(targetPos, attackShakeTimeToBack).SetEase(Ease.Unset).OnComplete(delegate ()
            {
                ChangeGameObjParent(cardObj, isPlayerCard ? FightForManager.instance.playerCardsBox : FightForManager.instance.enemyCardsBox);
            });
        }
        else
        {
            ChangeGameObjParent(cardObj, transferStation);

            Vector3 vec = new Vector3(
                targetPos.x,
                targetPos.y + (isPlayerRound ? (-1 * posFloat) : posFloat) * FightForManager.instance.oneDisY,
                targetPos.z
                );
            cardObj.transform.DOMove(targetPos, attackShakeTimeToGo * one).SetEase(Ease.Unset).OnComplete(delegate ()
            {
                cardObj.transform.DOMove(cardObj.transform.position, attackShakeTimeToGo * two).SetEase(Ease.Unset).OnComplete(delegate ()
                {
                    cardObj.transform.DOMove(vec, attackShakeTimeToGo * three).SetEase(Ease.Unset).OnComplete(delegate ()
                      {
                          cardObj.transform.DOMove(targetPos, attackShakeTimeToGo * four).SetEase(Ease.Unset);
                      });
                });
            });
        }
    }
}

/// <summary>
/// 战斗机状态
/// </summary>
public enum StateOfFight   
{
    ReadyForFight,  //0备战
    MoveNow,       //1攻击前摇
    FightInterval,  //2攻击间
    FightOver       //3攻击后摇
}

/// <summary>
/// 全屏特技名索引
/// </summary>
public enum FullScreenEffectName
{
    /// <summary>
    /// 会心一击特效
    /// </summary>
    HuiXinEffect,
    /// <summary>
    /// 羁绊激活特效
    /// </summary>
    JiBanEffect,
    /// <summary>
    /// 五虎上将主动技能特效
    /// </summary>
    JBWuHuShangJiang,
    /// <summary>
    /// 魏五奇谋主动技能特效
    /// </summary>
    JBWeiWuQiMou,
    /// <summary>
    /// 水师都督主动技能特效
    /// </summary>
    JBShuiShiDouDu
}

/// <summary>
/// 羁绊名索引
/// </summary>
public enum JiBanSkillName
{
    /// <summary>
    /// 桃园结义
    /// </summary>
    TaoYuanJieYi = 0,
    /// <summary>
    /// 五虎上将
    /// </summary>
    WuHuShangJiang = 1,
    /// <summary>
    /// 卧龙凤雏
    /// </summary>
    WoLongFengChu = 2,
    /// <summary>
    /// 虎痴恶来
    /// </summary>
    HuChiELai = 3,
    /// <summary>
    /// 五子良将
    /// </summary>
    WuZiLiangJiang = 4,
    /// <summary>
    /// 魏五谋士
    /// </summary>
    WeiWuMouShi = 5,
    /// <summary>
    /// 虎踞江东
    /// </summary>
    HuJuJiangDong = 6,
    /// <summary>
    /// 水师都督
    /// </summary>
    ShuiShiDouDu = 7,
    /// <summary>
    /// 天作之合
    /// </summary>
    TianZuoZhiHe = 8,
    /// <summary>
    /// 河北四庭柱
    /// </summary>
    HeBeiSiTingZhu = 9,
    /// <summary>
    /// 绝世无双
    /// </summary>
    JueShiWuShuang = 10,
    /// <summary>
    /// 汉末三仙
    /// </summary>
    HanMoSanXian = 11
}