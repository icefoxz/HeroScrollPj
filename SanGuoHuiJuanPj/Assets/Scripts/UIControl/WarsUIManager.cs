using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Beebyte.Obfuscator;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WarsUIManager : MonoBehaviour
{
    public static WarsUIManager instance;

    public Transform herosCardListTran;
    public ScrollRect herosCardListScrollRect;
    public bool _isDragItem;


    [SerializeField]
    GameObject playerInfoObj;   //玩家信息obj
    [SerializeField]
    GameObject cityLevelObj;   //主城信息obj
    [SerializeField]
    public GameObject heroCardListObj; //武将卡牌初始列表
    [SerializeField]
    GameObject cardForWarListPres; //列表卡牌预制件
    [SerializeField]
    GameObject guanQiaPreObj;   //关卡按钮预制件
    [SerializeField]
    GameObject startBtn;    //关卡开始按钮
    [SerializeField]
    GameObject upLevelBtn;   //升级城池btn

    public Button adRefreshBtn;//看广告刷新按键

    public GameObject[] eventsWindows; //各种事件窗口 0-战斗；1-故事；2-答题；3-奇遇；4-通用
    public enum EventTypes
    {
        Generic,//通用
        Battle,//战斗
        Story,//故事
        Quest,//答题
        Adventure,//奇遇
        Trade,//交易
        Recover//回春
    }
    /// <summary>
    /// 上一个关卡类型
    /// </summary>
    private EventTypes currentEvent = EventTypes.Generic;

    public WarMiniWindowUI gameOverWindow;//战役结束ui
    [SerializeField]
    float percentReturnHp;    //回春回血百分比

    public int baYeDefaultLevel = 3;
    public int cityLevel;   //记录城池等级
    public int goldForCity; //记录城池金币

    public int GoldForCity
    {
        get
        {
            if (PlayerDataForGame.instance.WarType == PlayerDataForGame.WarTypes.Baye)
                return PlayerDataForGame.instance.warsData.baYe.gold;
            return goldForCity;
        }
        set
        {
            goldForCity = value;
            if (PlayerDataForGame.instance.WarType != PlayerDataForGame.WarTypes.Baye) return;
            BaYeManager.instance.SetGold(value);
        }
    }

    public int treasureChestNums;   //记录城池宝箱

    int indexLastGuanQiaId;   //记录上一个关卡id
    int passedGuanQiaNums;  //记录通过的关卡数

    public List<FightCardData> playerCardsDatas; //我方卡牌信息集合

    public float cardMoveSpeed; //卡牌移动速度

    public AudioClip[] audioClipsFightEffect;
    public float[] audioVolumeFightEffect;

    public AudioClip[] audioClipsFightBack;
    public float[] audioVolumeFightBack;

    [SerializeField]
    Image fightBackImage;

    [SerializeField]
    Text levelIntroText;    //关卡介绍文本
    [SerializeField]
    Text battleNameText;    //战役名文本
    [SerializeField]
    Text battleScheduleText;    //战役进度文本
    int nowGuanQiaIndex;    //记录当前关卡进度

    bool isEnteredLevel;    //记录是否进入了关卡

    private GameResources GameResources => GameResources.Instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        isPointMoveNow = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //------------Awake----------------//
        cityLevel = 1;
        switch (PlayerDataForGame.instance.WarType)
        {
            //战斗金币
            case PlayerDataForGame.WarTypes.Expedition:
                GoldForCity = PlayerDataForGame.instance.zhanYiColdNums;
                break;
            case PlayerDataForGame.WarTypes.Baye:
                GoldForCity = PlayerDataForGame.instance.warsData.baYe.gold;
                cityLevel = baYeDefaultLevel;
                break;
            case PlayerDataForGame.WarTypes.None:
                throw XDebug.Throw<WarsUIManager>($"未确认战斗类型[{PlayerDataForGame.WarTypes.None}]，请在调用战斗场景前预设战斗类型。");
            default:
                throw new ArgumentOutOfRangeException();
        }
        treasureChestNums = 0;
        indexLastGuanQiaId = 0;
        passedGuanQiaNums = -1;
        playerCardsDatas = new List<FightCardData>();
        cardMoveSpeed = 2f;
        nowGuanQiaIndex = 0;
        point0Pos = point0Tran.position;
        point1Pos = point1Tran.position;
        point2Pos = point2Tran.position;

        Input.multiTouchEnabled = false;    //限制多指拖拽
        _isDragItem = false;
        isEnteredLevel = false;
        //------------Awake----------------//
        PlayerDataForGame.instance.lastSenceIndex = 2;
        adRefreshBtn.onClick.AddListener(WatchAdForUpdateQiYv);
        gameOverWindow.Init();
        InitMainUIShow();

        InitCardListShow();

        InitGuanQiaShow();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)return;
        Time.timeScale = currentEvent == EventTypes.Battle ? PlayerDataForGame.instance.pyData.WarTimeScale : 1f;
    }

    //初始化关卡
    private void InitGuanQiaShow()
    {
        currentEvent = EventTypes.Generic;
        //尝试展示指引
        ShowOrHideGuideObj(0, true);
        InitShowParentGuanQia(new int[] {DataTable.War[PlayerDataForGame.instance.selectedWarId].BeginPoint});
    }

    int selectParentIndex = -1;

    //选择某个父级关卡初始化子集关卡
    private void ChooseParentGuanQia(int parentGuanQiaId, int randImg, Transform parentTran)
    {
        UpdateLevelInfoText(parentGuanQiaId);

        if (selectParentIndex != parentGuanQiaId)
        {
            point0Tran.gameObject.SetActive(false);
        }

        indexLastGuanQiaId = parentGuanQiaId;

        for (int i = 0; i < point0Tran.childCount; i++)
        {
            Destroy(point0Tran.GetChild(i).gameObject);
        }

        //最后一关
        var nexPoints = DataTable.Checkpoint[parentGuanQiaId].Next;
        if (nowGuanQiaIndex >= DataTable.War[PlayerDataForGame.instance.selectedWarId].CheckPoints)
        {
            nexPoints = new int[0];
        }

        List<Transform> childsTranform = new List<Transform>();

        for (int i = 0; i < nexPoints.Length; i++)
        {
            int guanQiaId = nexPoints[i];
            var checkPoint = DataTable.Checkpoint[guanQiaId];
            GameObject obj = Instantiate(guanQiaPreObj, point0Tran);
            obj.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            var eventType = DataTable.Checkpoint[guanQiaId].EventType;
            if (IsBattle(eventType))
            {
                obj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = checkPoint.Title;
                obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                if (eventType != 7)
                {
                    obj.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite =
                        GameResources.GuanQiaEventImg[(eventType == 1 ? 0 : 1)];
                    obj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = checkPoint.FlagTitle;
                    obj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().fontSize = checkPoint.FlagTitle.Length > 2 ? 45 : 50;
                    obj.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                }
            }

            obj.transform.GetChild(1).GetComponent<Image>().sprite =
                GameResources.GuanQiaEventImg[checkPoint.ImageId];

            childsTranform.Add(obj.transform);
            //暂时不能选择后面的子关卡
            //obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate ()
            //{
            //    startBtn.SetActive(false);
            //    SelectOneGuanQia(obj);
            //    UpdateLevelInfoText(guanQiaId);
            //});

        }

        if (selectParentIndex != parentGuanQiaId)
        {
            selectParentIndex = parentGuanQiaId;

            //关卡艺术图
            levelIntroText.transform.parent.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(delegate ()
            {
                levelIntroText.transform.parent.GetComponent<Image>().sprite = GameResources.ArtWindow[randImg];
                levelIntroText.transform.parent.GetComponent<Image>().DOFade(1, 1f);
            });

            pointWays.SetActive(false);
            //展示云朵动画
            ShowClouds();
            StartCoroutine(LiteInitChooseFirst111(2f, parentTran, childsTranform));
        }
    }

    IEnumerator LiteInitChooseFirst111(float startTime, Transform parentTran, List<Transform> childsTranform)
    {
        yield return new WaitForSeconds(startTime / 2);
        //显示子关卡
        point0Tran.gameObject.SetActive(true);

        //yield return new WaitForSeconds(startTime/2);
        //路径通向子关卡
        //TheWayToTheChildPoint(parentTran, childsTranform);
    }

    //更新关卡介绍显示
    private void UpdateLevelInfoText(int guanQiaId)
    {
        levelIntroText.DOPause();
        levelIntroText.text = "";
        levelIntroText.color = new Color(levelIntroText.color.r, levelIntroText.color.g, levelIntroText.color.b, 0);
        levelIntroText.DOFade(1, 3f);
        levelIntroText.DOText(("\u2000\u2000\u2000\u2000" + DataTable.Checkpoint[guanQiaId].Intro), 3f).SetEase(Ease.Linear).SetAutoKill(false);
    }

    //战役结束
    public void BattleOverShow(bool isWin)
    {
        Time.timeScale = 1;

        if (isWin)
        {
            //通关不返还体力
            PlayerDataForGame.instance.getBackTiLiNums = 0;
        }
        var rewardMap = new Dictionary<int, int>();
        var zhanLing = new Dictionary<int, int>();
        //如果是霸业
        if(PlayerDataForGame.instance.WarType == PlayerDataForGame.WarTypes.Baye)
        {
            /**
             * -判断.上一个场景是不是战斗。
             * -判断.第几个霸业战斗
             * -判断.霸业经验奖励是否已被领取
             * 1.加经验
             */
            if (isWin)
            {
                var baYeMgr = BaYeManager.instance;
                var baYe = PlayerDataForGame.instance.warsData.baYe;
                if(baYeMgr.CurrentEventType == BaYeManager.EventTypes.City)
                {
                    var cityEvent = baYe.data.Single(f => f.CityId == PlayerDataForGame.instance.selectedCity);
                    var warIndex = cityEvent.WarIds.IndexOf(PlayerDataForGame.instance.selectedWarId);
                    if (!cityEvent.PassedStages[warIndex]) //如果过关未被记录
                    {
                        var exp = cityEvent.ExpList[warIndex]; //获取相应经验值
                        cityEvent.PassedStages[warIndex] = true;
                        PlayerDataForGame.instance.BaYeManager.AddExp(cityEvent.CityId, exp); //给玩家加经验值
                        PlayerDataForGame.instance.mainSceneTips = $"获得经验值：{exp}";
                        rewardMap.Trade(1, exp);
                    }
                }
                else
                {
                    var sEvent = baYeMgr.CachedStoryEvent;
                    rewardMap.Trade(0, sEvent.GoldReward);//0
                    rewardMap.Trade(1, sEvent.ExpReward);//1
                    rewardMap.Trade(3, sEvent.YuanBaoReward);//3
                    rewardMap.Trade(4, sEvent.YvQueReward);//4
                    var ling = sEvent.ZhanLing.First();
                    zhanLing.Trade(ling.Key, ling.Value);
                    baYeMgr.OnBayeStoryEventReward(baYeMgr.CachedStoryEvent);
                }
            }
            //霸业的战斗金币传到主城
            PlayerDataForGame.instance.warsData.baYe.gold = GoldForCity;
        } else if (PlayerDataForGame.instance.WarType == PlayerDataForGame.WarTypes.Expedition)
        {
            PlayerDataForGame.instance.UpdateWarUnlockProgress(passedGuanQiaNums);
        }

        if (treasureChestNums > 0) rewardMap.Trade(2, treasureChestNums); //index2是宝箱图

        //gameOverWindow.Show(rewardMap);
        gameOverWindow.ShowWithZhanLing(rewardMap, zhanLing);

        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
        //gameOverObj.SetActive(true);
    }

    //初始化父级关卡
    private void InitShowParentGuanQia(int[] checkPoints)
    {
        passedGuanQiaNums++;
        if (passedGuanQiaNums >= DataTable.War[PlayerDataForGame.instance.selectedWarId].CheckPoints)
        {
            BattleOverShow(true);//通过所有关卡
            return;
        }

        selectParentIndex = -1;

        for (int i = 0; i < point1Tran.childCount; i++)
        {
            Destroy(point1Tran.GetChild(i).gameObject);
        }
        for (int i = 0; i < checkPoints.Length; i++)
        {
            var checkPoint = DataTable.Checkpoint[checkPoints[i]];
            GameObject obj = Instantiate(guanQiaPreObj, point1Tran);
            obj.transform.GetChild(1).GetComponent<Image>().sprite =
                GameResources.GuanQiaEventImg[checkPoint.ImageId];
            if (IsBattle(checkPoint.EventType)) //战斗关卡城池名
            {
                obj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = checkPoint.Title;
                obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                if (checkPoint.EventType != 7)
                {
                    obj.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite =
                        GameResources.GuanQiaEventImg[(checkPoint.EventType == 1 ? 0 : 1)];
                    obj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = checkPoint.FlagTitle;
                    obj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().fontSize = checkPoint.FlagTitle.Length > 2 ? 45 : 50;
                    obj.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                }
            }

            int randArtImg = Random.Range(0, 25); //随机艺术图
            obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate()
            {
                startBtn.GetComponentInChildren<Text>().text =
                    IsBattle(checkPoint.EventType) ? DataTable.GetStringText(53) : DataTable.GetStringText(54);
                startBtn.SetActive(true);
                SelectOneGuanQia(obj);
                ChooseParentGuanQia(checkPoint.Id, randArtImg, obj.transform);
                InterToDiffGuanQia(IsBattle(checkPoint.EventType) ? 1 : checkPoint.EventType, checkPoint.BattleEventTableId, checkPoint.Id);
            });
        }
        StartCoroutine(LiteInitChooseFirst(0));
    }

    //判断是否是战斗关卡
    private bool IsBattle(int guanQiaType)
    {
        if (guanQiaType == 1 || guanQiaType == 7 || guanQiaType == 8 || guanQiaType == 9 || guanQiaType == 10 || guanQiaType == 11 || guanQiaType == 12)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator LiteInitChooseFirst(float startTime)
    {
        yield return new WaitForSeconds(startTime);
        //默认选择第一个
        point1Tran.GetChild(0).GetChild(1).GetComponent<Button>().onClick.Invoke();
    }

    GameObject indexSelectGuanQia;
    private void SelectOneGuanQia(GameObject chooseObj)
    {
        AudioController0.instance.RandomPlayGuZhengAudio();

        startBtn.GetComponent<Button>().onClick.RemoveAllListeners();

        if (indexSelectGuanQia != null)
        {
            indexSelectGuanQia.transform.GetChild(2).gameObject.SetActive(false);
        }
        chooseObj.transform.GetChild(2).gameObject.SetActive(true);
        indexSelectGuanQia = chooseObj;
    }

    /// <summary>
    /// 进入不同关卡
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="eventId"></param>
    private void InterToDiffGuanQia(int eventType, int eventId, int guanQiaId)
    {
        switch (eventType)
        {
            //战斗关卡
            case 1:
                startBtn.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    if (isPointMoveNow || isEnteredLevel) return;
                    GoToTheFight(eventId, guanQiaId);
                    isEnteredLevel = true;
                });
                break;
            //答题关卡
            case 3:
                startBtn.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    if (isPointMoveNow || isEnteredLevel) return;
                    GoToTheTest();
                    isEnteredLevel = true;
                });
                break;
            //回血关卡
            case 4:
                startBtn.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    if (isPointMoveNow || isEnteredLevel) return;
                    GoToTheHuiXue();
                    isEnteredLevel = true;
                });
                break;
            //奇遇(三选)关卡
            case 5:
                startBtn.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    if (isPointMoveNow || isEnteredLevel) return;
                    GoToTheQiYu(false);
                    isEnteredLevel = true;
                });
                break;
            //购买关卡
            case 6:
                startBtn.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    if (isPointMoveNow || isEnteredLevel) return;
                    GoToTheQiYu(true);
                    isEnteredLevel = true;
                });
                break;
            //其他事件
            default:
                throw new ArgumentOutOfRangeException($"event type [{eventType}]");
        }
    }

    /// <summary>
    /// 进入战斗
    /// </summary>
    /// <param name="fightId"></param>
    private void GoToTheFight(int fightId, int guanQiaId)
    {
        currentEvent = EventTypes.Battle;
        PlayAudioClip(21);
        var checkPoint = DataTable.Checkpoint[guanQiaId];
        fightBackImage.sprite = GameResources.BattleBG[checkPoint.BattleBG];
        int bgmIndex = checkPoint.BattleBGM;
        AudioController1.instance.isNeedPlayLongMusic = true;
        AudioController1.instance.ChangeAudioClip(audioClipsFightBack[bgmIndex], audioVolumeFightBack[bgmIndex]);
        AudioController1.instance.PlayLongBackMusInit();

        FightForManager.instance.InitEnemyCardForFight(fightId);

        eventsWindows[0].SetActive(true);
    }

    /// <summary>
    /// 进入答题
    /// </summary>
    private void GoToTheTest()
    {
        currentEvent = EventTypes.Quest;
        PlayAudioClip(19);

        InitializeDaTi();

        eventsWindows[2].SetActive(true);
    }

    /// <summary>
    /// 进入奇遇或购买
    /// </summary>
    /// <param name="qiyuId"></param>
    private void GoToTheQiYu(bool isBuy)
    {
        currentEvent = isBuy ? EventTypes.Trade : EventTypes.Adventure;
        //尝试关闭指引
        ShowOrHideGuideObj(0, false);

        ShowOrHideGuideObj(1, true);

        PlayAudioClip(19);

        InitializeQYOrSp(isBuy);

        eventsWindows[3].SetActive(true);
    }

    /// <summary>
    /// 进入回血事件
    /// </summary>
    private void GoToTheHuiXue()
    {
        currentEvent = EventTypes.Recover;
        PlayAudioClip(19);

        ReturnToBloodForFightCards();

        eventsWindows[4].SetActive(true);
    }

    [SerializeField]
    Transform point0Tran;   //子关卡Transform
    [SerializeField]
    Transform point1Tran;   //父关卡Transform
    [SerializeField]
    Transform point2Tran;   //通过后的关卡Transform
    [SerializeField]
    GameObject warCityCloudsObj;    //子关卡遮盖云朵

    private Vector3 point0Pos;
    private Vector3 point1Pos;
    private Vector3 point2Pos;

    private bool isPointMoveNow;

    [SerializeField]
    GameObject pointWays;

    //通关关卡转换的动画表现
    private void TongGuanCityPointShow()
    {
        isPointMoveNow = true;

        point0Tran.gameObject.SetActive(false);
        point1Tran.position = point0Pos;

        point1Tran.DOMove(point1Pos, 1.5f).SetEase(Ease.Unset).OnComplete(delegate ()
        {
            isPointMoveNow = false;

            point0Tran.gameObject.SetActive(true);
        });
    }

    //父关卡通路子关卡
    private void TheWayToTheChildPoint(Transform parentPoint, List<Transform> childPoints)
    {
        pointWays.SetActive(true);

        List<GameObject> wayObjsList = new List<GameObject>();

        for (int i = 0; i < pointWays.transform.childCount; i++)
        {
            GameObject obj = pointWays.transform.GetChild(i).gameObject;
            wayObjsList.Add(obj);
            if (obj.activeSelf)
                obj.SetActive(false);
        }

        for (int i = 0; i < childPoints.Count; i++)
        {
            if (childPoints[i] != null)
            {
                wayObjsList[i].transform.position = parentPoint.position;
                wayObjsList[i].transform.DOPause();
                wayObjsList[i].SetActive(true);

                Vector3[] points = GetVector3Points(parentPoint.position, childPoints[i].position);

                wayObjsList[i].transform.DOPath(points, waySpeedFlo).SetEase(Ease.Unset);
            }
        }
    }

    [SerializeField]
    float waySpeedFlo;  //通路时长

    [SerializeField]
    float randomChaZhi; //随机插值

    [SerializeField]
    int pointNums;  //中途点数

    //返回两点之间的随机路径点
    private Vector3[] GetVector3Points(Vector3 point0,Vector3 point1)
    {
        Vector3[] wayPoints = new Vector3[pointNums];
        float floX = (point1.x - point0.x) / wayPoints.Length;// + Random.Range(-randomInterpolation, randomInterpolation);
        float floY= (point1.y - point0.y) / wayPoints.Length;
        for (int i = 0; i < wayPoints.Length - 1; i++)
        {
            Vector3 vec = new Vector3();
            vec.x = point0.x + floX * (i + 1) + Random.Range(-randomChaZhi, randomChaZhi);
            vec.y = point0.y + floY * (i + 1);
            vec.z = point0.z;
            wayPoints[i] = vec;
        }
        wayPoints[wayPoints.Length - 1] = point1;
        return wayPoints;
    }

    //展示关卡前进的云朵动画
    private void ShowClouds()
    {
        if (passedGuanQiaNums + 1 < DataTable.War[PlayerDataForGame.instance.selectedWarId].CheckPoints)
        {
            if (warCityCloudsObj.activeInHierarchy)
            {
                warCityCloudsObj.SetActive(false);
            }
            warCityCloudsObj.SetActive(true);
        }
    }

    //private void FixedUpdate()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        PassGuanQia();
    //    }
    //}

    /// <summary>
    /// 通关
    /// </summary>
    public void PassGuanQia()
    {
        if (isPointMoveNow)
            return;

        isEnteredLevel = false;

        PlayAudioClip(13);

        UpdateBattleSchedule();
        InitShowParentGuanQia(DataTable.Checkpoint[indexLastGuanQiaId].Next);
        TongGuanCityPointShow();
        //关闭所有战斗事件的物件
        for (int i = 0; i < eventsWindows.Length; i++)
        {
            if (eventsWindows[i].activeSelf)
            {
                eventsWindows[i].SetActive(false);
                if (i == 0)
                {
                    AudioController1.instance.ChangeBackMusic();
                }
            }
        }
    }

    //给上阵卡牌回血
    private void ReturnToBloodForFightCards()
    {
        eventsWindows[4].transform.GetChild(1).GetChild(2).GetComponent<Text>().text = DataTable.GetStringText(55);
        eventsWindows[4].transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
        eventsWindows[4].transform.GetChild(1).GetChild(2).gameObject.SetActive(true);

        for (int i = 0; i < FightForManager.instance.playerFightCardsDatas.Length; i++)
        {
            if (FightForManager.instance.playerFightCardsDatas[i] != null && FightForManager.instance.playerFightCardsDatas[i].nowHp > 0)
            {
                FightForManager.instance.playerFightCardsDatas[i].nowHp += (int)(percentReturnHp * FightForManager.instance.playerFightCardsDatas[i].fullHp);
                FightController.instance.UpdateUnitHpShow(FightForManager.instance.playerFightCardsDatas[i]);
            }
        }
    }

    private class WeightElement : IWeightElement
    {
        public int Id { get; set; }
        public int Weight { get; set; }
    }

    //关闭三选的附加方法
    public void CloseSanXuanWin()
    {
        if (shopInfoObj.activeSelf)
        {
            shopInfoObj.SetActive(false);
        }
        Transform woodsList = eventsWindows[3].transform.GetChild(0).GetChild(1);
        for (int i = 0; i < 3; i++)
        {
            GameObject go = woodsList.GetChild(i).GetChild(8).gameObject;
            if (go.activeSelf)
            {
                go.SetActive(false);
            }
        }
    }

    int updateShopNeedGold; //商店刷新所需金币
    //刷新商店添加金币
    public void AddUpdateShoppingGold()
    {
        bool isSuccessed = UpdateShoppingList(updateShopNeedGold);
        if (isSuccessed)
            updateShopNeedGold++;
        eventsWindows[3].transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<Text>().text = updateShopNeedGold.ToString();
    }

    /// <summary>
    /// 刷新商店列表
    /// </summary>
    /// <param name="updateMoney">刷新所需金币</param>
    [Skip] private bool UpdateShoppingList(int updateMoney)
    {
        var re = GameResources;
        if (updateMoney != 0)
        {
            PlayAudioClip(13);
        }

        if (updateMoney != 0 && updateMoney > GoldForCity)
        {
            PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(56));
            return false;
        }
        else
        {
            shopInfoObj.SetActive(false);
            Transform woodsList = eventsWindows[3].transform.GetChild(0).GetChild(1);
            for (int i = 0; i < 3; i++)
            {
                GameObject go = woodsList.GetChild(i).GetChild(8).gameObject;
                if (go.activeSelf)
                {
                    go.SetActive(false);
                }
            }

            GoldForCity -= updateMoney;
            playerInfoObj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = GoldForCity.ToString();

            for (int i = 0; i < 3; i++)
            {
                int btnIndex = i;
                int pick = DataTable.Mercenary.Values.Select(m => new WeightElement {Id = m.Id, Weight = m.Weight})
                    .Pick().Id;
                var mercenary = DataTable.Mercenary[pick];
                var cardType = (GameCardType)mercenary.Produce.CardType;
                var cardRarity = mercenary.Produce.Rarity;
                var cardLevel = mercenary.Produce.Star;

                var card = RandomPickFromRareClass(cardType, cardRarity);
                woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = cardType == GameCardType.Hero
                    ? re.HeroImg[card.Id]
                    : re.FuZhuImg[card.ImageId];
                ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), card.Name);
                //名字颜色
                woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(card.Rare);
                woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = card.Short;
                //兵种框
                woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite =
                    cardType == GameCardType.Hero ? re.ClassImg[0] : re.ClassImg[1];
                FrameChoose(card.Rare, woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
                woodsList.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                woodsList.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                    OnClickToShowShopInfo(btnIndex, card.About));
                woodsList.GetChild(i).GetChild(4).GetComponent<Image>().sprite = re.GradeImg[cardLevel];

                Transform getBtnTran = woodsList.GetChild(i).GetChild(9);

                //免费获取文字隐藏
                getBtnTran.GetChild(0).gameObject.SetActive(false);
                //清除按钮事件
                getBtnTran.GetComponent<Button>().onClick.RemoveAllListeners();
                Button adBtn = getBtnTran.GetComponent<Button>();
                adBtn.enabled = true;
                //需要的金币数
                int mercenaryCost = 0;
                //广告概率
                if (Random.Range(0, 100) < 25)
                {
                    mercenaryCost = 0;
                    getBtnTran.GetChild(1).gameObject.SetActive(false);
                    getBtnTran.GetChild(2).gameObject.SetActive(true);
                    adBtn.onClick.AddListener(() => AdAgent.instance.BusyRetry(() =>
                    {
                        adBtn.enabled = false;
                        //if (!AdController.instance.ShowVideo(
                        GetOrBuyCards(true, mercenaryCost, cardType, card.Id, cardLevel, btnIndex);
                        getBtnTran.GetChild(2).gameObject.SetActive(false);
                        PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(57));
                        adBtn.enabled = true;
                    }, () =>
                    {
                        PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(6));
                        adBtn.enabled = true;
                    }));
                }
                else
                {
                    mercenaryCost = mercenary.Cost;
                    getBtnTran.GetChild(1).GetComponent<Text>().text = mercenaryCost.ToString();
                    getBtnTran.GetChild(1).gameObject.SetActive(true);
                    getBtnTran.GetChild(2).gameObject.SetActive(false);

                    getBtnTran.GetComponent<Button>().onClick.AddListener(() =>
                        GetOrBuyCards(true, mercenaryCost, cardType, card.Id, cardLevel, btnIndex));
                }
                woodsList.GetChild(i).gameObject.SetActive(true);
            }

            return true;
        }
    }
    /// <summary>
    /// 匹配稀有度的颜色
    /// </summary>
    /// <returns></returns>
    private Color NameColorChoose(int rarity)
    {
        Color color = new Color();
        switch (rarity)
        {
            case 1:
                color = ColorDataStatic.name_gray;
                break;
            case 2:
                color = ColorDataStatic.name_green;
                break;
            case 3:
                color = ColorDataStatic.name_blue;
                break;
            case 4:
                color = ColorDataStatic.name_purple;
                break;
            case 5:
                color = ColorDataStatic.name_orange;
                break;
            case 6:
                color = ColorDataStatic.name_red;
                break;
            case 7:
                color = ColorDataStatic.name_black;
                break;
            default:
                color = ColorDataStatic.name_gray;
                break;
        }
        return color;
    }

    [SerializeField]
    GameObject shopInfoObj; //展示三选详细信息obj

    //展示三选单位个体信息
    private void OnClickToShowShopInfo(int btnIndex, string infoStr)
    {
        ShowOrHideGuideObj(1, false);

        Transform woodsList = eventsWindows[3].transform.GetChild(0).GetChild(1);
        for (int i = 0; i < 3; i++)
        {
            GameObject go = woodsList.GetChild(i).GetChild(8).gameObject;
            if (i != btnIndex)
            {
                if (go.activeSelf)
                {
                    go.SetActive(false);
                }
            }
            else
            {
                if (!go.activeSelf)
                {
                    go.SetActive(true);
                }
            }
        }
        Text infoText = shopInfoObj.GetComponentInChildren<Text>();
        if (!shopInfoObj.activeSelf)
        {
            infoText.text = "";
            Image infoImg = shopInfoObj.GetComponentInChildren<Image>();
            infoImg.color = new Color(1f, 1f, 1f, 0f);
            shopInfoObj.SetActive(true);
            infoImg.DOFade(1, 0.2f);
        }
        infoText.DOFade(0, 0.3f).OnComplete(delegate ()
        {
            infoText.text = infoStr;
            infoText.DOFade(1, 0.3f);
        });
    }

    //观看视频刷新奇遇
    [Skip]
    public void WatchAdForUpdateQiYv()
    {
        Button adBtn = eventsWindows[3].transform.GetChild(0).GetChild(5).GetComponent<Button>();
        adBtn.enabled = false;
        AdAgent.instance.BusyRetry(() =>
        {
            adBtn.gameObject.SetActive(false);
            UpdateQiYvWoods();
        }, () => PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(6)));
        adBtn.enabled = true;
    }

    //刷新奇遇商品
    private void UpdateQiYvWoods()
    {
        var resources = GameResources;
        int randomPick = DataTable.Mercenary.Values.Select(m => new WeightElement {Id = m.Id, Weight = m.Weight}).Pick()
            .Id;
        var mercenary = DataTable.Mercenary[randomPick];
        var cardType = (GameCardType)mercenary.Produce.CardType;
        var cardRarity = mercenary.Produce.Rarity;
        int cardLevel = mercenary.Produce.Star;
        Transform woodsList = eventsWindows[3].transform.GetChild(0).GetChild(1);
        for (int i = 0; i < 3; i++)
        {
            var card = RandomPickFromRareClass(cardType, cardRarity);
            woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = cardType == GameCardType.Hero
                ? resources.HeroImg[card.Id]
                : resources.FuZhuImg[card.ImageId];
            ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), card.Name);
            //名字颜色
            woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(card.Rare);
            woodsList.GetChild(i).GetChild(4).GetComponent<Image>().sprite = resources.GradeImg[cardLevel];
            woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = card.Short;
            //兵种框
            woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite =
                cardType == GameCardType.Hero ? resources.ClassImg[0] : resources.ClassImg[1];
            FrameChoose(card.Rare, woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
            Transform getBtnTran = woodsList.GetChild(i).GetChild(9);
            getBtnTran.GetChild(0).gameObject.SetActive(true);
            getBtnTran.GetChild(1).gameObject.SetActive(false);
            getBtnTran.GetChild(2).gameObject.SetActive(false);
            getBtnTran.GetComponent<Button>().onClick.RemoveAllListeners();
            int btnIndex = i;
            getBtnTran.GetComponent<Button>().onClick.AddListener(() =>
                GetOrBuyCards(false, 0, cardType, card.Id, cardLevel, btnIndex));
            woodsList.GetChild(i).gameObject.SetActive(true);

            woodsList.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            woodsList.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                OnClickToShowShopInfo(btnIndex, card.About));
        }
    }

    //奇遇或商店界面初始化
    private void InitializeQYOrSp(bool isBuy)
    {
        Transform tran = eventsWindows[3].transform.GetChild(0);
        if (isBuy)
        {
            //重置刷新商店所需金币
            updateShopNeedGold = 2;
            tran.GetChild(3).GetChild(1).GetComponent<Text>().text = updateShopNeedGold.ToString();
            //标题
            tran.GetChild(0).GetComponent<Image>().sprite = GameResources.GuanQiaEventImg[6];
            //刷新按钮
            tran.GetChild(3).gameObject.SetActive(true);
            //奇遇刷新按钮
            tran.GetChild(5).gameObject.SetActive(false);
            UpdateShoppingList(0);
        }
        else
        {
            //奇遇刷新按钮
            tran.GetChild(5).GetComponent<Button>().enabled = true;

            tran.GetChild(0).GetComponent<Image>().sprite = GameResources.GuanQiaEventImg[5];
            tran.GetChild(3).gameObject.SetActive(false);
            tran.GetChild(5).gameObject.SetActive(true);
            UpdateQiYvWoods();
        }
    }

    //获得或购买三选物品
    private void GetOrBuyCards(bool isBuy, int money, GameCardType cardType, int cardId, int cardLevel, int btnIndex)
    {
        if (isBuy)
        {
            PlayAudioClip(13);
            if (money > GoldForCity)
            {
                PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(56));
                return;
            }
            else
            {
                GoldForCity -= money;
                playerInfoObj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = GoldForCity.ToString();
                eventsWindows[3].transform.GetChild(0).GetChild(1).GetChild(btnIndex).gameObject.SetActive(false);
            }
        }
        NowLevelAndHadChip nowLevelAndHadChip = new NowLevelAndHadChip().Instance(cardType,cardId,cardLevel);
        CreateCardToList(nowLevelAndHadChip);
        if (!isBuy)
        {
            PassGuanQia();
        }
        if (shopInfoObj.activeSelf)
        {
            shopInfoObj.SetActive(false);
        }
        GameObject obj = eventsWindows[3].transform.GetChild(0).GetChild(1).GetChild(btnIndex).GetChild(8).gameObject;
        if (obj.activeSelf)
        {
            obj.SetActive(false);
        }
    }

    //初始化答题界面
    private void InitializeDaTi()
    {
        eventsWindows[2].transform.GetChild(6).gameObject.SetActive(false);
        var pick = DataTable.Quest.Values.Select(q=>new WeightElement{Id = q.Id,Weight = q.Weight}).Pick();
        var quest = DataTable.Quest[pick.Id];
        eventsWindows[2].transform.GetChild(2).GetComponent<Text>().text = quest.Question;
        var selections = new[] {quest.A, quest.B, quest.C};
        for (int i = 3; i < 6; i++)
        {
            bool isRight = (i - 2) == quest.Answer;
            int btnIndex = i;
            eventsWindows[2].transform.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.white;
            eventsWindows[2].transform.GetChild(i).GetChild(0).GetComponent<Text>().text = selections[i - 3];
            eventsWindows[2].transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
            {
                EndDaTiGiveReward(isRight, btnIndex);
            });
        }
    }
    //答题结束
    private void EndDaTiGiveReward(bool isCorrect, int btnIndex)
    {
        PlayAudioClip(13);

        for (int i = 3; i < 6; i++)
        {
            eventsWindows[2].transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
        }

        string rewardStr = "";
        if (isCorrect)
        {
            rewardStr = DataTable.GetStringText(58);
            eventsWindows[2].transform.GetChild(btnIndex).GetChild(0).GetComponent<Text>().color = Color.green;
            var pick = DataTable.QuestReward.Values.Select(r => new WeightElement {Id = r.Id, Weight = r.Weight})
                .Pick().Id;
            var reward = DataTable.QuestReward[pick].Produce;

            var info = RandomPickFromRareClass((GameCardType)reward.CardType, reward.Rarity);
            var card = new NowLevelAndHadChip().Instance(info.Type, info.Id, reward.Star);
            CreateCardToList(card);
        }
        else
        {
            rewardStr = DataTable.GetStringText(59);
            eventsWindows[2].transform.GetChild(btnIndex).GetChild(0).GetComponent<Text>().color = Color.red;
        }
        PlayerDataForGame.instance.ShowStringTips(rewardStr);
        eventsWindows[2].transform.GetChild(6).gameObject.SetActive(true);
    }

    //根据稀有度返回随机id
    public GameCardInfo RandomPickFromRareClass(GameCardType cardType,int rarity) => GameCardInfo.RandomPick(cardType, rarity);

    //初始化卡牌列表
    private void InitCardListShow()
    {
        var forceId = PlayerDataForGame.instance.CurrentWarForceId;
#if UNITY_EDITOR
        if (forceId == -2) //-2为测试用不重置卡牌，直接沿用卡牌上的阵容
        {
            PlayerDataForGame.instance.fightHeroId.Select(id=> new NowLevelAndHadChip().Instance(GameCardType.Hero,id))
                .Concat(PlayerDataForGame.instance.fightTowerId.Select(id=> new NowLevelAndHadChip().Instance(GameCardType.Tower,id)))
                .Concat(PlayerDataForGame.instance.fightTrapId.Select(id=> new NowLevelAndHadChip().Instance(GameCardType.Trap,id)))
                .ToList().ForEach(CreateCardToList);
            return;
        }
#endif
        PlayerDataForGame.instance.fightHeroId.Clear();
        PlayerDataForGame.instance.fightTowerId.Clear();
        PlayerDataForGame.instance.fightTrapId.Clear();

        var hstData = PlayerDataForGame.instance.hstData;
        //临时记录武将存档信息
        hstData.heroSaveData.Enlist(forceId).ToList()
            .ForEach(CreateCardToList);
        hstData.towerSaveData.Enlist(forceId).ToList()
            .ForEach(CreateCardToList);
        hstData.trapSaveData.Enlist(forceId).ToList()
            .ForEach(CreateCardToList);
    }

    //创建玩家卡牌
    private void CreateCardToList(NowLevelAndHadChip card)
    {
        var re = GameResources;
        var info = card.GetInfo();
        GameObject obj = Instantiate(cardForWarListPres, heroCardListObj.transform);
        obj.GetComponent<CardForDrag>().posIndex = -1;
        obj.transform.GetChild(1).GetComponent<Image>().sprite =
            info.Type == GameCardType.Hero ? re.HeroImg[card.id] : re.FuZhuImg[info.ImageId];
        ShowNameTextRules(obj.transform.GetChild(3).GetComponent<Text>(), info.Name);
        //名字颜色
        obj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(info.Rare);
        obj.transform.GetChild(4).GetComponent<Image>().sprite = re.GradeImg[card.level];
        obj.transform.GetChild(5).GetComponentInChildren<Text>().text = info.Short;
        //兵种框
        obj.transform.GetChild(5).GetComponent<Image>().sprite =
            info.Type == GameCardType.Hero ? re.ClassImg[0] : re.ClassImg[1];
        FrameChoose(info.Rare, obj.transform.GetChild(6).GetComponent<Image>());
        //添加按住抬起方法
        FightForManager.instance.GiveGameObjEventForHoldOn(obj, info.About);
        FightCardData data = new FightCardData();
        data.unitId = 1;
        data.cardObj = obj;
        data.cardType = card.typeIndex;
        data.cardId = card.id;
        data.posIndex = -1;
        data.cardGrade = card.level;
        data.fightState = new FightState();
        data.damage = info.GetDamage(data.cardGrade);
        data.hpr = info.GameSetRecovery;
        data.fullHp = data.nowHp = info.GetHp(data.cardGrade);
        data.activeUnit = info.Type == GameCardType.Hero || (info.Type == GameCardType.Tower &&
                                                               (data.cardId == 0 || data.cardId == 1 ||
                                                                data.cardId == 2 || data.cardId == 3 ||
                                                                data.cardId == 6));
        data.isPlayerCard = true;
        data.cardMoveType = info.CombatType;
        data.cardDamageType = info.DamageType;
        playerCardsDatas.Add(data);
    }

    //匹配稀有度边框
    public void FrameChoose(int rarity, Image img)
    {
        img.enabled = false;
        return;//暂时不提供边框
        img.enabled = true;
        switch (rarity)
        {
            case 4:
                img.sprite = GameResources.FrameImg[3];
                break;
            case 5:
                img.sprite = GameResources.FrameImg[2];
                break;
            case 6:
                img.sprite = GameResources.FrameImg[1];
                break;
            default:
                img.enabled = false;
                break;
        }
    }

    //定位到cardId对应的索引号
    private int FindIndexFromData(List<NowLevelAndHadChip> saveData, int cardId)
    {
        int index = 0;
        for (; index < saveData.Count; index++)
        {
            if (saveData[index].id == cardId)
            {
                break;
            }
        }
        return index;
    }

    //初始化场景内容
    private void InitMainUIShow()
    {
        battleNameText.text = DataTable.War[PlayerDataForGame.instance.selectedWarId].Title;
        playerInfoObj.transform.GetChild(0).GetComponent<Text>().text = DataTable.PlayerInitialConfig[PlayerDataForGame.instance.pyData.ForceId].Force;
        UpdateGoldandBoxNumsShow();
        UpdateLevelInfo();
        UpdateBattleSchedule();
    }

    //刷新战役进度显示
    private void UpdateBattleSchedule()
    {
        nowGuanQiaIndex++;
        var totalCheckPoints = DataTable.War[PlayerDataForGame.instance.selectedWarId].CheckPoints;
        if (nowGuanQiaIndex >= totalCheckPoints)
        {
            nowGuanQiaIndex = totalCheckPoints;
        }

        string str = nowGuanQiaIndex + "/" + totalCheckPoints;
        battleScheduleText.text = str;
    }

    //刷新金币宝箱的显示
    public void UpdateGoldandBoxNumsShow()
    {
        playerInfoObj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = GoldForCity.ToString();
        playerInfoObj.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = treasureChestNums.ToString();
    }

    /// <summary>
    /// 当前武将卡牌上阵最大数量
    /// </summary>
    [HideInInspector]
    public int maxHeroNums;


    //更新等级相关显示
    private void UpdateLevelInfo()
    {
        var baseCfg = DataTable.BaseLevel[cityLevel];
        //等级
        cityLevelObj.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = cityLevel + "级";
        //武将可上阵
        maxHeroNums = baseCfg.CardMax;
        cityLevelObj.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = baseCfg.CardMax.ToString();
        //升级金币
        upLevelBtn.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = baseCfg.Cost.ToString();
    }

    /// <summary>
    /// 升级城池
    /// </summary>
    public void OnClickUpLevel()
    {
        ShowOrHideGuideObj(2, false);
        var baseCfg = DataTable.BaseLevel[cityLevel];
        if (GoldForCity < baseCfg.Cost)
        {
            PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(56));
            PlayAudioClip(20);
        }
        else
        {
            GoldForCity -= baseCfg.Cost;
            cityLevel++;
            PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(60));
            playerInfoObj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = GoldForCity.ToString();
            UpdateLevelInfo();
            //满级
            if (cityLevel >= DataTable.BaseLevel.Count)
            {
                upLevelBtn.GetComponent<Button>().enabled = false;
                upLevelBtn.transform.GetChild(0).gameObject.SetActive(false);
                upLevelBtn.transform.GetChild(1).GetComponent<Text>().text = DataTable.GetStringText(61);
                PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(62));
            }
            FightForManager.instance.UpdateFightNumTextShow();
            PlayAudioClip(19);
        }
    }
    
    /// <summary>
    /// 返回主城
    /// </summary>
    public void OnClickGoBackToCity()
    {
        Time.timeScale = 1;
        PlayAudioClip(13);
        isEnteredLevel = true;  //限制返回主城时还能进入关卡
        if (!PlayerDataForGame.instance.isJumping)
        {
            PlayerDataForGame.instance.JumpSceneFun(1, false);
        }
    }

    /// <summary>
    /// 根据obj找到其父数据
    /// </summary>
    public int FindDataFromCardsDatas(GameObject obj)
    {
        int i = 0;
        for (; i < playerCardsDatas.Count; i++)
        {
            if (playerCardsDatas[i].cardObj == obj)
            {
                break;
            }
        }
        if (i >= playerCardsDatas.Count)
        {
            return -1;
        }
        else
        {
            return i;
        }
    }

    /// <summary>
    /// 卡牌移动
    /// </summary>
    public void CardMoveToPos(GameObject needMoveObj, Vector3 vec3)
    {
        needMoveObj.transform.DOMove(vec3, cardMoveSpeed)
            .SetEase(Ease.Unset)
            .OnComplete(delegate () {
                //Debug.Log("-----Move Over"); 
            }).SetAutoKill(true);
    }

    public void PlayAudioClip(int indexClips)
    {
        AudioController0.instance.ChangeAudioClip(indexClips);
        AudioController0.instance.PlayAudioSource(0);
    }

    /// <summary>
    /// 名字显示规则
    /// </summary>
    /// <param name="nameText"></param>
    /// <param name="str"></param>
    public void ShowNameTextRules(Text nameText, string str)
    {
        nameText.text = str;
        switch (str.Length)
        {
            case 1:
                nameText.fontSize = 50;
                nameText.lineSpacing = 1.1f;
                break;
            case 2:
                nameText.fontSize = 50;
                nameText.lineSpacing = 1.1f;
                break;
            case 3:
                nameText.fontSize = 50;
                nameText.lineSpacing = 0.9f;
                break;
            case 4:
                nameText.fontSize = 45;
                nameText.lineSpacing = 0.8f;
                break;
            default:
                nameText.fontSize = 45;
                nameText.lineSpacing = 0.8f;
                break;
        }
    }

    [SerializeField]
    Text musicBtnText;  //音乐开关文本

    //打开设置界面
    public void OpenSettingWinInit()
    {
        PlayAudioClip(13);
        if (AudioController0.instance.isPlayMusic != 1)
        {
            musicBtnText.text = DataTable.GetStringText(41);
        }
        else
        {
            musicBtnText.text = DataTable.GetStringText(42);
        }
    }

    //开关音乐
    public void OpenOrCloseMusic()
    {
        if (AudioController0.instance.isPlayMusic != 1)
        {
            //打开
            PlayerPrefs.SetInt(LoadSaveData.instance.IsPlayMusicStr, 1);
            AudioController0.instance.isPlayMusic = 1;
            AudioController1.instance.audioSource.Play();
            musicBtnText.text = DataTable.GetStringText(42);
            PlayAudioClip(13);
        }
        else
        {
            //关闭
            PlayerPrefs.SetInt(LoadSaveData.instance.IsPlayMusicStr, 0);
            AudioController0.instance.isPlayMusic = 0;
            AudioController0.instance.audioSource.Pause();
            AudioController1.instance.audioSource.Pause();
            FightController.instance.audioSource.Pause();
            musicBtnText.text = DataTable.GetStringText(41);
        }
    }

    [SerializeField]
    GameObject[] guideObjs; // 指引objs 0:开始关卡 1:查看奇遇 2:升级按钮

    //显示或隐藏指引
    public void ShowOrHideGuideObj(int index, bool isShow)
    {
        if (isShow)
        {
            if (PlayerDataForGame.instance.guideObjsShowed[index + 4] == 0)
            {
                guideObjs[index].SetActive(true);
            }
        }
        else
        {
            if (PlayerDataForGame.instance.guideObjsShowed[index + 4] == 0)
            {
                guideObjs[index].SetActive(false);
                PlayerDataForGame.instance.guideObjsShowed[index + 4] = 1;
                switch (index)
                {
                    case 0:
                        PlayerPrefs.SetInt(StringForGuide.guideStartGQ, 1);
                        break;
                    case 1:
                        PlayerPrefs.SetInt(StringForGuide.guideCheckCardInfo, 1);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    bool isShowQuitTips = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isShowQuitTips)
            {
                PlayAudioClip(13);
#if UNITY_ANDROID
                Application.Quit();
#endif
            }
            else
            {
                isShowQuitTips = true;
                PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(52));
                Invoke("ResetQuitBool", 2f);
            }
        }
    }
    //重置退出游戏判断参数
    private void ResetQuitBool()
    {
        isShowQuitTips = false;
    }
}