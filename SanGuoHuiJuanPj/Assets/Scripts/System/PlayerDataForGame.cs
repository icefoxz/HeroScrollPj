using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerDataForGame : MonoBehaviour
{
    public static PlayerDataForGame instance;
    //修复v1.89无限刷霸业宝箱3的Bug 
    //玉阙检查值 
    public int Bug1_9YvQueCheck = 5000;
    //玉阙修正数量 
    public int Bug1_9YvQueSet = 4999;
    public enum GameScene
    {
        StartScene,
        MainScene,
        WarScene
    }
    public GameScene CurrentScene { get; private set; }
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

    public PlayerData pyData;  //玩家基本信息 
    public GetBoxOrCodeData gbocData = new GetBoxOrCodeData();  //玩家宝箱与兑换码信息 
    public HSTDataClass hstData = new HSTDataClass();       //玩家武将士兵塔等信息 
    public WarsDataClass warsData = new WarsDataClass();       //玩家战役解锁+霸业进度信息 

    [HideInInspector]
    public int[] guideObjsShowed;   //存放各个指引展示情况 

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

    public GameResources gameResources;
    public BaYeManager baYeManager;

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

        DontDestroyOnLoad(gameObject);
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
        gameResources = new GameResources();
        gameResources.Init();

        SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
    }

    private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode mode) => CurrentScene = (GameScene) scene.buildIndex;

    private void Update()
    {
        if (isJumping)
        {
            loadPro = asyncOp.progress; //获取加载进度,最大为0.9 
            if (loadPro >= 0.9f)
            {
                loadPro = 1;
            }
            loadingText.text = string.Format(LoadJsonFile.GetStringText(63), (int)(loadPro * 100));

            if (loadPro >= 1 && !LoadSaveData.instance.isLoadingSaveData)
            {
                loadPro = 0;
                isJumping = false;
                asyncOp.allowSceneActivation = true;//场景加载完毕跳转 
                StartCoroutine(FadeTransitionEffect(fadeSpeed));
            }
        }
    }

    /// <summary> 
    /// 跳转场景 
    /// </summary> 
    /// <param name="sceneIndex"></param> 
    /// <param name="isNeedLoadData"></param> 
    public void JumpSceneFun(int sceneIndex, bool isNeedLoadData)
    {
        if (!isJumping)
        {
            loadingImg.DOPause();
            StartCoroutine(ShowTransitionEffect(sceneIndex, isNeedLoadData));
        }
    }

    IEnumerator ShowTransitionEffect(int sceneIndex, bool isNeedLoadData)
    {
        loadingImg.gameObject.SetActive(true);
        loadingImg.DOFade(1, fadeSpeed);

        yield return new WaitForSeconds(fadeSpeed);

        asyncOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);//异步加载场景，Single:不保留现有场景 

        int index = Random.Range(0, LoadJsonFile.knowledgeTableDatas.Count);
        infoText.text = LoadJsonFile.knowledgeTableDatas[index][2];
        infoText.transform.GetChild(0).GetComponent<Text>().text = LoadJsonFile.knowledgeTableDatas[index][5];

        loadingText.gameObject.SetActive(true);
        infoText.gameObject.SetActive(true);
        asyncOp.allowSceneActivation = false;
        isJumping = true;
        if (isNeedLoadData)
        {
            LoadSaveData.instance.LoadByJson();
        }
    }

    //隐藏 
    IEnumerator FadeTransitionEffect(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        loadingText.gameObject.SetActive(false);
        infoText.gameObject.SetActive(false);
        loadingImg.gameObject.SetActive(true);
        loadingImg.DOFade(0, fadeSpeed).OnComplete(delegate ()
        {
            loadingImg.gameObject.SetActive(false);
        });
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
        if (paused)
        {
        }
        else
        {
            setDesignContentScale();
        }
    }

    //计算出战总数量 
    public int CalculationFightCount()
    {
        int count = 0;
        count = fightHeroId.Count + fightTowerId.Count + fightTrapId.Count;
        return count;
    }


    /// <summary> 
    /// 添加或删除卡牌id到出战列表 
    /// </summary> 
    public bool AddOrCutFightCardId(int typeIndex, int cardId, bool isAdd)
    {
        switch (typeIndex)
        {
            case 0:
                if (isAdd)
                {
                    if (CalculationFightCount() < int.Parse(LoadJsonFile.playerLevelTableDatas[pyData.Level - 1][2]))
                    {
                        if (!fightHeroId.Contains(cardId))
                        {
                            fightHeroId.Add(cardId);
                        }
                        else
                        {
                            //Debug.Log("fightHeroId出战列表已有" + cardId); 
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (fightHeroId.Contains(cardId))
                    {
                        fightHeroId.Remove(cardId);
                    }
                    else
                    {
                        //Debug.Log("回城错误，出战列表无此单位"); 
                        return false;
                    }
                }
                break;
            case 1:
                if (isAdd)
                {
                    if (CalculationFightCount() < int.Parse(LoadJsonFile.playerLevelTableDatas[pyData.Level - 1][2]))
                    {
                        if (!fightSoLdierId.Contains(cardId))
                        {
                            fightSoLdierId.Add(cardId);
                        }
                        else
                        {
                            //Debug.Log("fightSoLdierId出战列表已有" + cardId); 
                        }
                    }
                    else
                    {
                        //Debug.Log("出战列表已满"); 
                        return false;
                    }
                }
                else
                {
                    if (fightSoLdierId.Contains(cardId))
                    {
                        fightSoLdierId.Remove(cardId);
                    }
                    else
                    {
                        //Debug.Log("回城错误，出战列表无此单位"); 
                        return false;
                    }
                }
                break;
            case 2:
                if (isAdd)
                {
                    if (CalculationFightCount() < int.Parse(LoadJsonFile.playerLevelTableDatas[pyData.Level - 1][2]))
                    {
                        if (!fightTowerId.Contains(cardId))
                        {
                            fightTowerId.Add(cardId);
                        }
                        else
                        {
                            //Debug.Log("fightTowerId出战列表已有" + cardId); 
                        }
                    }
                    else
                    {
                        //Debug.Log("出战列表已满"); 
                        return false;
                    }
                }
                else
                {
                    if (fightTowerId.Contains(cardId))
                    {
                        fightTowerId.Remove(cardId);
                    }
                    else
                    {
                        //Debug.Log("回城错误，出战列表无此单位"); 
                        return false;
                    }
                }
                break;
            case 3:
                if (isAdd)
                {
                    if (CalculationFightCount() < int.Parse(LoadJsonFile.playerLevelTableDatas[pyData.Level - 1][2]))
                    {
                        if (!fightTrapId.Contains(cardId))
                        {
                            fightTrapId.Add(cardId);
                        }
                        else
                        {
                            //Debug.Log("fightTrapId出战列表已有" + cardId); 
                        }
                    }
                    else
                    {
                        //Debug.Log("出战列表已满"); 
                        return false;
                    }
                }
                else
                {
                    if (fightTrapId.Contains(cardId))
                    {
                        fightTrapId.Remove(cardId);
                    }
                    else
                    {
                        //Debug.Log("回城错误，出战列表无此单位"); 
                        return false;
                    }
                }
                break;
            case 4:
                if (isAdd)
                {
                    if (CalculationFightCount() < int.Parse(LoadJsonFile.playerLevelTableDatas[pyData.Level - 1][2]))
                    {
                        if (!fightSpellId.Contains(cardId))
                        {
                            fightSpellId.Add(cardId);
                        }
                        else
                        {
                            //Debug.Log("fightSpellId出战列表已有" + cardId); 
                        }
                    }
                    else
                    {
                        //Debug.Log("出战列表已满"); 
                        return false;
                    }
                }
                else
                {
                    if (fightSpellId.Contains(cardId))
                    {
                        fightSpellId.Remove(cardId);
                    }
                    else
                    {
                        //Debug.Log("回城错误，出战列表无此单位"); 
                        return false;
                    }
                }
                break;
            default:
                return false;
        }
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

    /// <summary> 
    /// 为战斗类型预设个记号，在场景转换的时候将读取战斗记号并打开相应的页面 
    /// </summary> 
    /// <param name="warType"></param> 
    public void FlagWarTypeBeforeBattle(int warType)
    {
        WarType = warType >= 0 && warType <= 2
            ? (WarTypes)warType
            : throw new ArgumentOutOfRangeException($"Type of {nameof(WarTypes)} : {warType}");
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

    public void SaveBaYeWarEvent()
    {
        var force = WarForceMap[WarTypes.Baye];
        if (!BaYeManager.instance.TradeZhanLing(force, -1))
        {
            ShowStringTips("战令不足以消费！");
            return;
        }
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

}