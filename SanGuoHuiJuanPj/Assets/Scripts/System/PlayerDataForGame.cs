using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CorrelateLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerDataForGame : MonoBehaviour
{
    public static PlayerDataForGame instance;

    public int staminaMax = 500;
    public int secsPerStamina = 600;
    private int staminaIncreaseLimit = 90;
    //修复v1.89无限刷霸业宝箱3的Bug 
    //玉阙检查值 
    public int Bug1_9YvQueCheck = 5000;
    //玉阙修正数量 
    public int Bug1_9YvQueSet = 4999;
    [Serializable]
    public enum WarTypes
    {
        None = 0,
        Expedition = 1, //主线战役 
        Baye = 2, //霸业 
    }
    public enum RedeemTypes
    {
        JinNang = 0, // 锦囊 
        JiuTan = 1  //酒坛 
    }
    public WarTypes WarType;//标记当前战斗类型 

    public Dictionary<WarTypes, int> WarForceMap { get; } = new Dictionary<WarTypes, int>
    {
        {WarTypes.Expedition, 0},
        {WarTypes.Baye, 0}
    };
    /// <summary>
    /// 当前战役选择的势力
    /// </summary>
    public int CurrentWarForceId => WarForceMap[WarType];

    [HideInInspector]
    public bool isNeedSaveData; //记录是否需要存档 

    [HideInInspector]
    public bool isHadNewSaveData; //记录游戏内是否有最新的读档数据 

    public UserInfo acData = new UserInfo();  //玩家账户信息 
    public int Arrangement { get; set; }//服务器账号标记

    public PlayerData pyData;  //玩家基本信息 
    public GetBoxOrCodeData gbocData = new GetBoxOrCodeData();  //玩家宝箱与兑换码信息 
    public HSTDataClass hstData = new HSTDataClass(); //玩家武将士兵塔等信息 

    public WarsDataClass warsData
    {
        get => _warsData;
        set
        {
            _warsData = value;
        }
    } //玩家战役解锁+霸业进度信息 

    public int[] GuideObjsShowed
    {
        get => guideObjsShowed;
        set => guideObjsShowed = value;
    } //存放各个指引展示情况 

    //记录出战单位 
    [HideInInspector]
    public List<int> fightHeroId = new List<int>();
    [HideInInspector]
    public List<int> fightSoLdierId = new List<int>();
    [HideInInspector]
    public List<int> fightTowerId = new List<int>();
    [HideInInspector]
    public List<int> fightTrapId = new List<int>();
    [HideInInspector]
    public List<int> fightSpellId = new List<int>();

    [HideInInspector]
    public int selectedWarId = -1;    //记录选择的战役id 

    [HideInInspector]
    public int zhanYiColdNums = 0;  //记录战役的金币数 
                                    //[HideInInspector] 
                                    //public int baYeGoldNums = 0;    //记录霸业金币数 

    float fadeSpeed = 1.5f;   //渐隐渐显时间 
    [HideInInspector]
    public bool isJumping;
    float loadPro;      //加载进度 
    AsyncOperation asyncOp;
    [SerializeField]
    Text loadingText;   //加载进度文本 
    [SerializeField]
    Text infoText;      //小提示文本 
    [SerializeField]
    Image loadingImg;   //遮布 

    [HideInInspector]
    public int lastSenceIndex;  //上一个场景索引记录 
    [HideInInspector]
    public int getBackTiLiNums;  //返还最大体力记录 
    [HideInInspector]
    public int boxForTiLiNums;  //返还体力单个宝箱扣除体力数 

    private bool isRequestingSaveFile; //存档请求中
    //计算出战总数量 
    public int TotalCardsEnlisted => fightHeroId.Count + fightTowerId.Count + fightTrapId.Count;

    public LocalStamina Stamina { get; private set; }

    public WarReward WarReward { get; set; }
    public BaYeManager BaYeManager { get; set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        selectedWarId = -1;
        isJumping = false;
        loadPro = 0;
        asyncOp = null;

        lastSenceIndex = 0;
        getBackTiLiNums = 0;
        isNeedSaveData = false;
        isHadNewSaveData = false;

        garbageStationObjs = new List<GameObject>();
        StartCoroutine(FadeTransitionEffect(0));

    }

    private void Update()
    {
        if (!isJumping) return;
        loadPro = asyncOp?.progress??0.3f; //获取加载进度,最大为0.9 
        if (loadPro >= 0.9f)
        {
            loadPro = 1;
        }
        loadingText.text = string.Format(DataTable.GetStringText(63), (int)(loadPro * 100));
        if (loadPro < 1) return;
        if (LoadSaveData.instance.isLoadingSaveData) return;
        loadPro = 0;
        isJumping = false;
    }

    /// <summary> 
    /// 跳转场景 
    /// </summary> 
    /// <param name="sceneIndex"></param> 
    /// <param name="isRequestSyncData">是否请求同步存档</param>
    /// <param name="untilTrue">加载锁，直到返回true才会转换场景</param> 
    public void JumpSceneFun(int sceneIndex, bool isRequestSyncData, Func<bool> untilTrue = null)
    {
        if (isJumping) return;
        loadingImg.DOPause();
        StartCoroutine(ShowTransitionEffect(sceneIndex, isRequestSyncData, untilTrue));
    }

    IEnumerator ShowTransitionEffect(int sceneIndex, bool isRequestSyncData,Func<bool> untilTrue)
    {
        isJumping = true;
        if(isRequestSyncData)
        {
            isRequestingSaveFile = true;
            if (Arrangement == 0)
            {
                LoadSaveData.instance.LoadByJson();
                isRequestingSaveFile = false;
            }else ApiPanel.instance.SyncSaved(() => isRequestingSaveFile = false);
        }
        loadingImg.gameObject.SetActive(true);
        loadingImg.DOFade(1, fadeSpeed/2);

        yield return new WaitForSeconds(fadeSpeed);

        var tips = DataTable.Tips.RandomPick().Value;
        infoText.text = tips.Text;
        infoText.transform.GetChild(0).GetComponent<Text>().text = tips.Sign;

        loadingText.gameObject.SetActive(true);
        infoText.gameObject.SetActive(true);
        yield return new WaitWhile(() => isRequestingSaveFile);//这里阻塞下个场景初始化逻辑。等待存档加载完毕
        if (untilTrue != null) yield return new WaitUntil(untilTrue);
        asyncOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);//异步加载场景，Single:不保留现有场景 
        yield return new WaitUntil(() => asyncOp.isDone);
        StartCoroutine(FadeTransitionEffect(fadeSpeed));
    }

    //隐藏 
    IEnumerator FadeTransitionEffect(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        loadingText.gameObject.SetActive(false);
        infoText.gameObject.SetActive(false);
        loadingImg.gameObject.SetActive(true);
        loadingImg.DOFade(0, fadeSpeed).OnComplete(() => loadingImg.gameObject.SetActive(false));
    }

    private int scaleWidth = 0;
    private int scaleHeight = 0;
    public void setDesignContentScale()
    {
        //#if UNITY_ANDROID 
        if (scaleWidth == 0 && scaleHeight == 0)
        {
            var width = Screen.currentResolution.width * 1f;
            var height = Screen.currentResolution.height * 1f;
            float designWidth = 1080;
            float designHeight = 1920;
            float s1 = designWidth / designHeight;
            float s2 = width / height;
            if (s1 < s2)
            {
                designWidth = Mathf.FloorToInt(designHeight * s2);
            }
            else if (s1 > s2)
            {
                designHeight = Mathf.FloorToInt(designWidth / s2);
            }
            float contentScale = designWidth / width;
            if (contentScale < 1.0f)
            {
                scaleWidth = (int)designWidth;
                scaleHeight = (int)designHeight;
            }
        }
        if (scaleWidth > 0 && scaleHeight > 0)
        {
            if (scaleWidth % 2 == 0)
            {
                scaleWidth += 1;
            }
            else
            {
                scaleWidth -= 1;
            }
            Screen.SetResolution(scaleWidth, scaleHeight, true);
        }
        //#endif 
    }

    void OnApplicationPause(bool paused)
    {
        if (paused) return;
        setDesignContentScale();
    }

    

    /// <summary> 
    /// 添加或删除卡牌id到出战列表 
    /// </summary> 
    public bool EnlistCard(NowLevelAndHadChip card, bool isAdd)
    {
        var cardType = (GameCardType) card.typeIndex;
        var cardLimit = DataTable.PlayerLevelConfig[pyData.Level].CardLimit;
        List<int> cardList = null;
        switch (cardType)
        {
            case GameCardType.Hero:
                cardList = fightHeroId;
                break;
            case GameCardType.Tower:
                cardList = fightTowerId;
                break;
            case GameCardType.Trap:
                cardList = fightTrapId;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if(isAdd)
        {
            if (TotalCardsEnlisted >= cardLimit) return false;
            if (!cardList.Contains(card.id))
                cardList.Add(card.id);
            return true;
        }

        if (cardList.Contains(card.id))
            cardList.Remove(card.id);
        return true;
    }

    ///////////////////////////////////游戏垃圾回收////////////////////////////////////////////// 

    //游戏内GameObject垃圾池 
    public static List<GameObject> garbageStationObjs;

    /// <summary> 
    /// 清空GameObject垃圾池 
    /// </summary> 
    public void ClearGarbageStationObj()
    {
        //Debug.Log("GameObject垃圾池数量：" + garbageStationObjs.Count); 
        try
        {
            for (int i = garbageStationObjs.Count - 1; i >= 0; i--)
            {
                if (garbageStationObjs[i] != null)
                {
                    Destroy(garbageStationObjs[i]);
                }
            }
            garbageStationObjs.Clear();
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    [SerializeField]
    GameObject textTipsObj;     //文本提示obj  

    public int selectedBaYeEventId; //当前选择的霸业城池 
    public int selectedCity;
    public string mainSceneTips;
    private WarsDataClass _warsData = new WarsDataClass();
    private int[] guideObjsShowed = new int[7];
    

    /// <summary> 
    /// 场景底部文本提示 
    /// </summary> 
    /// <param name="str"></param> 
    public void ShowStringTips(string str)
    {
        textTipsObj.SetActive(false);
        textTipsObj.transform.GetComponent<Text>().text = str;
        textTipsObj.SetActive(true);
    }

    public void SetRedeemCount(RedeemTypes type, int count)
    {
        switch (type)
        {
            case RedeemTypes.JinNang:
                pyData.DailyJinNangRedemptionCount = count;
                break;
            case RedeemTypes.JiuTan:
                pyData.DailyJiuTanRedemptionCount = count;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(1);
    }

    public void Redemption(RedeemTypes type)
    {
        switch (type)
        {
            case RedeemTypes.JinNang:
                pyData.DailyJinNangRedemptionCount++;
                pyData.LastJinNangRedeemTime = SystemTimer.instance.NowUnixTicks;
                break;
            case RedeemTypes.JiuTan:
                pyData.DailyJiuTanRedemptionCount++;
                pyData.LastJiuTanRedeemTime = SystemTimer.instance.NowUnixTicks;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(1);
    }

    public bool ConsumeZhanLing(int amt = 1)
    {
        var force = WarForceMap[WarTypes.Baye];
        if (BaYeManager.instance.TradeZhanLing(force, amt * -1)) return true;
        ShowStringTips("战令不足以消费！");
        return false;
    }

    public void SaveBaYeWarEvent()
    {
        var baYe = warsData.baYe;
        if (baYe.data.Any(d => d.CityId == selectedCity)) return;
        var city = BaYeManager.instance.Map.Single(e => e.CityId == selectedCity);
        baYe.data.Add(new BaYeCityEvent
        {
            CityId = selectedCity,
            EventId = selectedBaYeEventId,
            WarIds = city.WarIds,
            ExpList = city.ExpList,
            PassedStages = new bool[city.ExpList.Count]
        });
        isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
    }

    public void UpdateWarUnlockProgress(int totalStagesPass)
    {
        if (totalStagesPass > warsData.GetCampaign(selectedWarId).unLockCount)
            warsData.GetCampaign(selectedWarId).unLockCount = totalStagesPass;
        isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
    }

    public void AddStamina(int stamina)
    {
        Stamina.AddStamina(stamina);
        pyData.Stamina += stamina;
        if (pyData.Stamina < 0)
        {
            throw new InvalidOperationException($"体力小于0! stamina = ({stamina})");
            pyData.Stamina = 0;
        }
        isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(1);
    }

    public void UpdateGameCards(GameCardDto[] gameCardList, TroopDto[] troops = null)
    {
        var cards = gameCardList.Select(NowLevelAndHadChip.Instance)
            .Where(c => c.IsOwning())
            .GroupBy(c => (GameCardType) c.typeIndex, c => c)
            .ToDictionary(c => c.Key, c => c.ToList());
        if (troops != null)
        {
            var enlisted = troops.SelectMany(t => t.EnList).ToList();
            foreach (var chip in enlisted.SelectMany(set =>
                cards[set.Key].Join(set.Value, c => c.CardId, i => i, (c, _) => c)))
                chip.isFight = 1;
        }
        if (!cards.TryGetValue(GameCardType.Hero, out hstData.heroSaveData))
            hstData.heroSaveData = new List<NowLevelAndHadChip>();
        if (!cards.TryGetValue(GameCardType.Tower, out hstData.towerSaveData))
            hstData.towerSaveData = new List<NowLevelAndHadChip>();
        if (!cards.TryGetValue(GameCardType.Trap, out hstData.trapSaveData))
            hstData.trapSaveData = new List<NowLevelAndHadChip>();
    }

    public void SendTroopToWarApi()
    {
        //todo: 暂时霸业不请求Api
        if(WarType != WarTypes.Expedition)return;
        var cards = hstData.heroSaveData.Concat(hstData.towerSaveData).Concat(hstData.trapSaveData)
            .Enlist(CurrentWarForceId).Select(c => c.ToDto()).ToList();
        ApiPanel.instance.Invoke(vb =>
            {
                WarReward = new WarReward(vb.Values[0].ToString(), selectedWarId);
            }, msg =>
            {
                ShowStringTips(msg);
                WarReward = new WarReward(string.Empty, selectedWarId);
            }, EventStrings.Req_TroopToCampaign,
            ViewBag.Instance().TroopDto(new TroopDto
            {
                EnList = cards.GroupBy(c => c.Type, c => c.CardId).ToDictionary(c => c.Key, c => c.ToArray()),
                ForceId = CurrentWarForceId
            }).SetValues(selectedWarId, UIManager.instance.expedition.CurrentMode.Id));
    }

    public void RefreshEnlisted(int forceId)
    {
        fightHeroId = hstData.heroSaveData.Enlist(forceId).Select(h=>h.id).ToList();
        fightTowerId = hstData.towerSaveData.Enlist(forceId).Select(t => t.id).ToList();
        fightTrapId = hstData.trapSaveData.Enlist(forceId).Select(t => t.id).ToList();
    }

    public void GenerateLocalStamina()
    {
        Stamina = new LocalStamina(pyData.LastStaminaUpdateTicks, pyData.Stamina, secsPerStamina, staminaIncreaseLimit,
            staminaMax);
    }

    public void RetrieveStamina()
    {
        AddStamina(getBackTiLiNums);
        getBackTiLiNums = 0;
    }
}