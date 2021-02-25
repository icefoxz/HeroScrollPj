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

    private GameResources GameResources => PlayerDataForGame.instance.gameResources;

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
        InitShowParentGuanQia(DataTable.WarData[PlayerDataForGame.instance.selectedWarId][3]);
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
        string str = DataTable.PointData[parentGuanQiaId][1];
        if (nowGuanQiaIndex >= int.Parse(DataTable.WarData[PlayerDataForGame.instance.selectedWarId][4]))
        {
            str = "";
        }

        List<Transform> childsTranform = new List<Transform>();

        string[] arrs = str.Split(',');
        for (int i = 0; i < arrs.Length; i++)
        {
            if (arrs[i] != "")
            {
                int guanQiaId = int.Parse(arrs[i]);
                int eventId = int.Parse(DataTable.PointData[guanQiaId][4]);
                GameObject obj = Instantiate(guanQiaPreObj, point0Tran);
                obj.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                var eventType = int.Parse(DataTable.PointData[guanQiaId][3]);
                if (IsBattle(eventType))
                {
                    obj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = DataTable.PointData[guanQiaId][2];
                    obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                    if (eventType != 7)
                    {
                        obj.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite = GameResources.GuanQiaEventImg[(eventType == 1 ? 0 : 1)];
                        obj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = DataTable.PointData[guanQiaId][9];
                        obj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().fontSize = (DataTable.PointData[guanQiaId][9].Length > 2 ? 45 : 50);
                        obj.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                    }
                }
                obj.transform.GetChild(1).GetComponent<Image>().sprite = GameResources.GuanQiaEventImg[int.Parse(DataTable.PointData[guanQiaId][6])];

                childsTranform.Add(obj.transform);
                //暂时不能选择后面的子关卡
                //obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate ()
                //{
                //    startBtn.SetActive(false);
                //    SelectOneGuanQia(obj);
                //    UpdateLevelInfoText(guanQiaId);
                //});
            }
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
        levelIntroText.DOText(("\u2000\u2000\u2000\u2000" + DataTable.PointData[guanQiaId][5]), 3f).SetEase(Ease.Linear).SetAutoKill(false);
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
                        PlayerDataForGame.instance.baYeManager.AddExp(cityEvent.CityId, exp); //给玩家加经验值
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

        int guanQiaSum = int.Parse(DataTable.WarData[PlayerDataForGame.instance.selectedWarId][4]);
        if (treasureChestNums > 0) rewardMap.Trade(2, treasureChestNums); //index2是宝箱图

        //gameOverWindow.Show(rewardMap);
        gameOverWindow.ShowWithZhanLing(rewardMap, zhanLing);

        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
        //gameOverObj.SetActive(true);
    }

    //初始化父级关卡
    private void InitShowParentGuanQia(string str)
    {
        passedGuanQiaNums++;
        if (passedGuanQiaNums >= int.Parse(DataTable.WarData[PlayerDataForGame.instance.selectedWarId][4]))
        {//最后一关
            str = "";
        }
        if (str == "")    //通过所有关卡
        {
            BattleOverShow(true);
        }
        else
        {
            selectParentIndex = -1;

            for (int i = 0; i < point1Tran.childCount; i++)
            {
                Destroy(point1Tran.GetChild(i).gameObject);
            }
            string[] arrs = str.Split(',');
            for (int i = 0; i < arrs.Length; i++)
            {
                if (arrs[i] != "")
                {
                    int guanQiaId = int.Parse(arrs[i]);
                    int eventType = int.Parse(DataTable.PointData[guanQiaId][3]);
                    int eventId = int.Parse(DataTable.PointData[guanQiaId][4]);
                    GameObject obj = Instantiate(guanQiaPreObj, point1Tran);
                    obj.transform.GetChild(1).GetComponent<Image>().sprite =
                        GameResources.GuanQiaEventImg[
                            int.Parse(DataTable.PointData[guanQiaId][6])];
                    if (IsBattle(eventType))  //战斗关卡城池名
                    {
                        obj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = DataTable.PointData[guanQiaId][2];
                        obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                        if (eventType != 7)
                        {
                            obj.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite = GameResources.GuanQiaEventImg[(eventType == 1 ? 0 : 1)];
                            obj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = DataTable.PointData[guanQiaId][9];
                            obj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().fontSize = (DataTable.PointData[guanQiaId][9].Length > 2 ? 45 : 50);
                            obj.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                        }
                    }
                    int randArtImg = Random.Range(0, 25);   //随机艺术图
                    obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        startBtn.GetComponentInChildren<Text>().text = IsBattle(eventType) ? DataTable.GetStringText(53) : DataTable.GetStringText(54);
                        startBtn.SetActive(true);
                        SelectOneGuanQia(obj);
                        ChooseParentGuanQia(guanQiaId, randArtImg, obj.transform);
                        InterToDiffGuanQia(IsBattle(eventType) ? 1 : eventType, eventId, guanQiaId);
                    });
                }
            }
            StartCoroutine(LiteInitChooseFirst(0));
        }
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
            //故事关卡
            case 2:
                startBtn.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    if (isPointMoveNow || isEnteredLevel) return;
                    GoToTheStory();
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
                break;
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

        fightBackImage.sprite = GameResources.BattleBG[int.Parse(DataTable.PointData[guanQiaId][7])];
        int bgmIndex = int.Parse(DataTable.PointData[guanQiaId][8]);
        AudioController1.instance.isNeedPlayLongMusic = true;
        AudioController1.instance.ChangeAudioClip(audioClipsFightBack[bgmIndex], audioVolumeFightBack[bgmIndex]);
        AudioController1.instance.PlayLongBackMusInit();

        FightForManager.instance.InitEnemyCardForFight(fightId);

        eventsWindows[0].SetActive(true);
    }

    /// <summary>
    /// 进入故事
    /// </summary>
    /// <param name="storyId"></param>
    private void GoToTheStory()
    {
        currentEvent = EventTypes.Story;
        PlayAudioClip(19);

        InitializeDianGu();

        eventsWindows[1].SetActive(true);
    }

    /// <summary>
    /// 进入答题
    /// </summary>
    /// <param name="testId"></param>
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
        if (passedGuanQiaNums + 1 < int.Parse(DataTable.WarData[PlayerDataForGame.instance.selectedWarId][4]))
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
        InitShowParentGuanQia(DataTable.PointData[indexLastGuanQiaId][1]);
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

    //根据权重得到随机id
    private int GetRandomBaseOnWeight(IReadOnlyDictionary<int,IReadOnlyList<string>> data, int weightIndex)
    {
        return data.Select(map => new WeightElement
        {
            Id = map.Key,
            Weight = int.Parse(map.Value[weightIndex])
        }).Pick().Id;
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
                int indexId = GetRandomBaseOnWeight(DataTable.Encounter, 1);
                var cardType = (GameCardType)int.Parse(DataTable.Encounter[indexId][2]);
                string cardRarity = DataTable.Encounter[indexId][3];
                int cardLevel = int.Parse(DataTable.Encounter[indexId][4]);
                int cardId = 0;
                switch (cardType)
                {
                    case GameCardType.Hero:
                        cardId = RandomPickFromRareClass(DataTable.Hero, cardRarity);
                        woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = re.HeroImg[cardId];
                        ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), DataTable.Hero[cardId][1]);
                        //名字颜色
                        woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(DataTable.Hero[cardId][3]);
                        woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = DataTable.Class[int.Parse(DataTable.Hero[cardId][5])][3];
                        //兵种框
                        woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = re.ClassImg[0];
                        FrameChoose(int.Parse(DataTable.Hero[cardId][3]), woodsList.GetChild(i).GetChild(6).GetComponent<Image>());

                        woodsList.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                        woodsList.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                        {
                            OnClickToShowShopInfo(btnIndex, DataTable.Class[int.Parse(DataTable.Hero[cardId][5])][4]);
                        });
                        break;
                    case GameCardType.Soldier:
                        cardId = RandomPickFromRareClass(DataTable.Soldier, cardRarity);
                        woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = re.FuZhuImg[int.Parse(DataTable.Soldier[cardId][13])];
                        ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), DataTable.Soldier[cardId][1]);
                        //名字颜色
                        woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(DataTable.Soldier[cardId][3]);
                        woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = DataTable.Class[int.Parse(DataTable.Soldier[cardId][5])][3];
                        //兵种框
                        woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = re.ClassImg[1];
                        FrameChoose(int.Parse(DataTable.Soldier[cardId][3]),
                            woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
                        woodsList.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                        woodsList.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                        {
                            OnClickToShowShopInfo(btnIndex, DataTable.Class[int.Parse(DataTable.Soldier[cardId][5])][4]);
                        });
                        break;
                    case GameCardType.Tower:
                        cardId = RandomPickFromRareClass(DataTable.Tower, cardRarity);
                        woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = re.FuZhuImg[int.Parse(DataTable.Tower[cardId][10])];
                        ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), DataTable.Tower[cardId][1]);
                        //名字颜色
                        woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(DataTable.Tower[cardId][3]);
                        woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = DataTable.Tower[cardId][5];
                        //兵种框
                        woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = re.ClassImg[1];
                        FrameChoose(int.Parse(DataTable.Tower[cardId][3]), woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
                        woodsList.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                        woodsList.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                        {
                            OnClickToShowShopInfo(btnIndex, DataTable.Tower[cardId][13]);
                        });
                        break;
                    case GameCardType.Trap:
                        cardId = RandomPickFromRareClass(DataTable.Trap, cardRarity);
                        woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = re.FuZhuImg[int.Parse(DataTable.Trap[cardId][9])];
                        ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), DataTable.Trap[cardId][1]);
                        //名字颜色
                        woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(DataTable.Trap[cardId][3]);
                        woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = DataTable.Trap[cardId][5];
                        //兵种框
                        woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = re.ClassImg[1];
                        FrameChoose(int.Parse(DataTable.Trap[cardId][3]), woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
                        woodsList.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                        woodsList.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                        {
                            OnClickToShowShopInfo(btnIndex, DataTable.Trap[cardId][12]);
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                woodsList.GetChild(i).GetChild(4).GetComponent<Image>().sprite = re.GradeImg[cardLevel];

                Transform getBtnTran = woodsList.GetChild(i).GetChild(9);

                //免费获取文字隐藏
                getBtnTran.GetChild(0).gameObject.SetActive(false);
                //清除按钮事件
                getBtnTran.GetComponent<Button>().onClick.RemoveAllListeners();
                Button adBtn = getBtnTran.GetComponent<Button>();
                adBtn.enabled = true;
                //需要的金币数
                int needMoney = 0;
                //广告概率17%
                if (Random.Range(0, 100) < 20)
                {
                    needMoney = 0;
                    getBtnTran.GetChild(1).gameObject.SetActive(false);
                    getBtnTran.GetChild(2).gameObject.SetActive(true);
                    adBtn.onClick.AddListener(() => AdAgent.instance.BusyRetry(() =>
                    {
                        adBtn.enabled = false;
                        //if (!AdController.instance.ShowVideo(
                        GetOrBuyCards(true, needMoney, cardType, cardId, cardLevel, btnIndex);
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
                    needMoney = int.Parse(DataTable.EncounterData[indexId][5]);
                    getBtnTran.GetChild(1).GetComponent<Text>().text = needMoney.ToString();
                    getBtnTran.GetChild(1).gameObject.SetActive(true);
                    getBtnTran.GetChild(2).gameObject.SetActive(false);

                    getBtnTran.GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        GetOrBuyCards(true, needMoney, cardType, cardId, cardLevel, btnIndex);
                    });
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
    private Color NameColorChoose(string rarity)
    {
        Color color = new Color();
        switch (rarity)
        {
            case "1":
                color = ColorDataStatic.name_gray;
                break;
            case "2":
                color = ColorDataStatic.name_green;
                break;
            case "3":
                color = ColorDataStatic.name_blue;
                break;
            case "4":
                color = ColorDataStatic.name_purple;
                break;
            case "5":
                color = ColorDataStatic.name_orange;
                break;
            case "6":
                color = ColorDataStatic.name_red;
                break;
            case "7":
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
        int indexId = GetRandomBaseOnWeight(DataTable.Encounter, 1);
        var cardType = (GameCardType)int.Parse(DataTable.Encounter[indexId][2]);
        string cardRarity = DataTable.Encounter[indexId][3];
        int cardLevel = int.Parse(DataTable.Encounter[indexId][4]);
        Transform woodsList = eventsWindows[3].transform.GetChild(0).GetChild(1);
        switch (cardType)
        {
            case GameCardType.Hero:
                for (int i = 0; i < 3; i++)
                {
                    int cardId = RandomPickFromRareClass(DataTable.Hero, cardRarity);
                    woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = resources.HeroImg[cardId];
                    ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), DataTable.Hero[cardId][1]);
                    //名字颜色
                    woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(DataTable.Hero[cardId][3]);
                    woodsList.GetChild(i).GetChild(4).GetComponent<Image>().sprite = resources.GradeImg[cardLevel];
                    woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = DataTable.ClassData[int.Parse(DataTable.Hero[cardId][5])][3];
                    //兵种框
                    woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = resources.ClassImg[0];
                    FrameChoose(int.Parse(DataTable.Hero[cardId][3]),
                        woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
                    Transform getBtnTran = woodsList.GetChild(i).GetChild(9);
                    getBtnTran.GetChild(0).gameObject.SetActive(true);
                    getBtnTran.GetChild(1).gameObject.SetActive(false);
                    getBtnTran.GetChild(2).gameObject.SetActive(false);
                    getBtnTran.GetComponent<Button>().onClick.RemoveAllListeners();
                    int btnIndex = i;
                    getBtnTran.GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        GetOrBuyCards(false, 0, cardType, cardId, cardLevel, btnIndex);
                    });
                    woodsList.GetChild(i).gameObject.SetActive(true);

                    woodsList.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                    woodsList.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        OnClickToShowShopInfo(btnIndex, DataTable.ClassData[int.Parse(DataTable.Hero[cardId][5])][4]);
                    });
                }
                break;
            case GameCardType.Soldier:
                for (int i = 0; i < 3; i++)
                {
                    int cardId = RandomPickFromRareClass(DataTable.Soldier, cardRarity);
                    woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = resources.FuZhuImg[int.Parse(DataTable.Soldier[cardId][13])];
                    ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), DataTable.Soldier[cardId][1]);
                    //名字颜色
                    woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(DataTable.Soldier[cardId][3]);
                    woodsList.GetChild(i).GetChild(4).GetComponent<Image>().sprite = resources.GradeImg[cardLevel];
                    woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = DataTable.ClassData[int.Parse(DataTable.Soldier[cardId][5])][3];
                    //兵种框
                    woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = resources.ClassImg[1];
                    FrameChoose(int.Parse(DataTable.Soldier[cardId][3]), woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
                    Transform getBtnTran = woodsList.GetChild(i).GetChild(9);
                    getBtnTran.GetChild(0).gameObject.SetActive(true);
                    getBtnTran.GetChild(1).gameObject.SetActive(false);
                    getBtnTran.GetChild(2).gameObject.SetActive(false);
                    getBtnTran.GetComponent<Button>().onClick.RemoveAllListeners();
                    int btnIndex = i;
                    getBtnTran.GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        GetOrBuyCards(false, 0, cardType, cardId, cardLevel, btnIndex);
                    });
                    woodsList.GetChild(i).gameObject.SetActive(true);

                    woodsList.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                    woodsList.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        OnClickToShowShopInfo(btnIndex, DataTable.ClassData[int.Parse(DataTable.Soldier[cardId][5])][4]);
                    });
                }
                break;
            case GameCardType.Tower:
                for (int i = 0; i < 3; i++)
                {
                    int cardId = RandomPickFromRareClass(DataTable.Tower, cardRarity);
                    woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite =
                        resources.FuZhuImg[int.Parse(DataTable.Tower[cardId][10])];
                    ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), DataTable.Tower[cardId][1]);
                    //名字颜色
                    woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(DataTable.Tower[cardId][3]);
                    woodsList.GetChild(i).GetChild(4).GetComponent<Image>().sprite = resources.GradeImg[cardLevel];
                    woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = DataTable.Tower[cardId][5];
                    FrameChoose(int.Parse(DataTable.Tower[cardId][3]), woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
                    //兵种框
                    woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = resources.ClassImg[1];
                    Transform getBtnTran = woodsList.GetChild(i).GetChild(9);
                    getBtnTran.GetChild(0).gameObject.SetActive(true);
                    getBtnTran.GetChild(1).gameObject.SetActive(false);
                    getBtnTran.GetChild(2).gameObject.SetActive(false);
                    getBtnTran.GetComponent<Button>().onClick.RemoveAllListeners();
                    int btnIndex = i;
                    getBtnTran.GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        GetOrBuyCards(false, 0, cardType, cardId, cardLevel, btnIndex);
                    });
                    woodsList.GetChild(i).gameObject.SetActive(true);

                    woodsList.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                    woodsList.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        OnClickToShowShopInfo(btnIndex, DataTable.Tower[cardId][13]);
                    });
                }
                break;
            case GameCardType.Trap:
                for (int i = 0; i < 3; i++)
                {
                    int cardId = RandomPickFromRareClass(DataTable.Trap, cardRarity);
                    woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = resources.FuZhuImg[int.Parse(DataTable.Trap[cardId][9])];
                    ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), DataTable.Trap[cardId][1]);
                    //名字颜色
                    woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(DataTable.Trap[cardId][3]);
                    woodsList.GetChild(i).GetChild(4).GetComponent<Image>().sprite = resources.GradeImg[cardLevel];
                    woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = DataTable.Trap[cardId][5];
                    //兵种框
                    woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = resources.ClassImg[1];
                    FrameChoose(int.Parse(DataTable.Trap[cardId][3]), woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
                    Transform getBtnTran = woodsList.GetChild(i).GetChild(9);
                    getBtnTran.GetChild(0).gameObject.SetActive(true);
                    getBtnTran.GetChild(1).gameObject.SetActive(false);
                    getBtnTran.GetChild(2).gameObject.SetActive(false);
                    getBtnTran.GetComponent<Button>().onClick.RemoveAllListeners();
                    int btnIndex = i;
                    getBtnTran.GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        GetOrBuyCards(false, 0, cardType, cardId, cardLevel, btnIndex);
                    });
                    woodsList.GetChild(i).gameObject.SetActive(true);

                    woodsList.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                    woodsList.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        OnClickToShowShopInfo(btnIndex, DataTable.Trap[cardId][12]);
                    });
                }
                break;
            case GameCardType.Spell:
            default:
                throw new ArgumentOutOfRangeException();
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
        NowLevelAndHadChip nowLevelAndHadChip = new NowLevelAndHadChip();
        nowLevelAndHadChip.id = cardId;
        nowLevelAndHadChip.level = cardLevel;
        switch (cardType)
        {
            case GameCardType.Hero:
                CreateHeroCardToFightList(nowLevelAndHadChip);
                break;
            case GameCardType.Soldier:
                CreateSoldierCardToFightList(nowLevelAndHadChip);
                break;
            case GameCardType.Tower:
                CreateTowerCardToFightList(nowLevelAndHadChip);
                break;
            case GameCardType.Trap:
                CreateTrapCardToFightList(nowLevelAndHadChip);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(cardType), cardType, null);
        }
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

    //典故界面初始化
    private void InitializeDianGu()
    {
        //随机数
        int rand = Random.Range(0, DataTable.StoryData.Count);
        //典故名
        eventsWindows[1].transform.GetChild(0).GetChild(3).GetComponent<Text>().text = DataTable.StoryData[rand][1];
        //故事
        eventsWindows[1].transform.GetChild(1).GetComponent<Text>().text = "\u2000\u2000\u2000\u2000" + DataTable.StoryData[rand][2];
        //选项
        for (int i = 2; i < 4; i++)
        {
            eventsWindows[1].transform.GetChild(i).GetChild(0).GetComponent<Text>().text = DataTable.StoryData[rand][i + 2];
            int indexI = i + 4;
            eventsWindows[1].transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
            {
                OnClickChooseAnsForDianGu(int.Parse(DataTable.StoryData[rand][indexI]));
            });
            eventsWindows[1].transform.GetChild(i).gameObject.SetActive(true);
        }
        eventsWindows[1].transform.GetChild(4).gameObject.SetActive(false);
    }
    //点击典故选项
    private void OnClickChooseAnsForDianGu(int indexAns)
    {
        PlayAudioClip(13);

        eventsWindows[1].transform.GetChild(2).gameObject.SetActive(false);
        eventsWindows[1].transform.GetChild(3).gameObject.SetActive(false);
        Text storyEndText = eventsWindows[1].transform.GetChild(1).GetComponent<Text>();
        storyEndText.DOPause();
        storyEndText.text = "";
        storyEndText.color = new Color(storyEndText.color.r, storyEndText.color.g, storyEndText.color.b, 0);

        int goldReward = int.Parse(DataTable.StoryRData[indexAns][6]);
        if (goldReward < 0)
        {
            goldReward = GoldForCity >= Mathf.Abs(goldReward) ? goldReward : -GoldForCity;
        }
        if (goldReward != 0)
        {
            GoldForCity += goldReward;
            playerInfoObj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = GoldForCity.ToString();
        }
        int unitType = int.Parse(DataTable.StoryRData[indexAns][2]);
        if (unitType >= 0)
        {
            NowLevelAndHadChip nowLevelAndHadChip = new NowLevelAndHadChip();
            nowLevelAndHadChip.id = int.Parse(DataTable.StoryRData[indexAns][3]);
            nowLevelAndHadChip.level = int.Parse(DataTable.StoryRData[indexAns][4]);
            int getNums = int.Parse(DataTable.StoryRData[indexAns][5]);
            switch (unitType)
            {
                case 0:
                    for (int i = 0; i < getNums; i++)
                    {
                        CreateHeroCardToFightList(nowLevelAndHadChip);
                    }
                    break;
                case 1:
                    for (int i = 0; i < getNums; i++)
                    {
                        CreateSoldierCardToFightList(nowLevelAndHadChip);
                    }
                    break;
                case 2:
                    for (int i = 0; i < getNums; i++)
                    {
                        CreateTowerCardToFightList(nowLevelAndHadChip);
                    }
                    break;
                case 3:
                    for (int i = 0; i < getNums; i++)
                    {
                        CreateTrapCardToFightList(nowLevelAndHadChip);
                    }
                    break;
                default:
                    break;
            }
        }

        storyEndText.DOFade(1, 4f);
        storyEndText.DOText(("\u2000\u2000\u2000\u2000" + DataTable.StoryRData[indexAns][1]), 4f).OnComplete(delegate ()
        {
            eventsWindows[1].transform.GetChild(4).gameObject.SetActive(true);
        });
    }

    //初始化答题界面
    private void InitializeDaTi()
    {
        eventsWindows[2].transform.GetChild(6).gameObject.SetActive(false);
        int indexTest = GetRandomBaseOnWeight(DataTable.Test, 6);
        int truthNum = int.Parse(DataTable.TestData[indexTest][2]);
        eventsWindows[2].transform.GetChild(2).GetComponent<Text>().text = DataTable.TestData[indexTest][1];
        for (int i = 3; i < 6; i++)
        {
            bool isRight = (i - 2) == truthNum;
            int btnIndex = i;
            eventsWindows[2].transform.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.white;
            eventsWindows[2].transform.GetChild(i).GetChild(0).GetComponent<Text>().text = DataTable.TestData[indexTest][i];
            eventsWindows[2].transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
            {
                EndDaTiGiveReward(isRight, btnIndex);
            });
        }
    }
    //答题结束
    private void EndDaTiGiveReward(bool isRight, int btnIndex)
    {
        PlayAudioClip(13);

        for (int i = 3; i < 6; i++)
        {
            eventsWindows[2].transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
        }

        string rewardStr = "";
        if (isRight)
        {
            rewardStr = DataTable.GetStringText(58);
            eventsWindows[2].transform.GetChild(btnIndex).GetChild(0).GetComponent<Text>().color = Color.green;
            int indexTest = GetRandomBaseOnWeight(DataTable.TestR, 1);
            int unitType = int.Parse(DataTable.TestRData[indexTest][2]);
            if (unitType >= 0)
            {
                NowLevelAndHadChip nowLevelAndHadChip = new NowLevelAndHadChip();
                nowLevelAndHadChip.level = int.Parse(DataTable.TestRData[indexTest][4]);
                switch (unitType)
                {
                    case 0:
                        nowLevelAndHadChip.id = RandomPickFromRareClass(DataTable.Hero, DataTable.TestRData[indexTest][3]);
                        CreateHeroCardToFightList(nowLevelAndHadChip);
                        break;
                    case 1:
                        nowLevelAndHadChip.id = RandomPickFromRareClass(DataTable.Soldier, DataTable.TestRData[indexTest][3]);
                        CreateSoldierCardToFightList(nowLevelAndHadChip);
                        break;
                    case 2:
                        nowLevelAndHadChip.id = RandomPickFromRareClass(DataTable.Tower, DataTable.TestRData[indexTest][3]);
                        CreateTowerCardToFightList(nowLevelAndHadChip);
                        break;
                    case 3:
                        nowLevelAndHadChip.id = RandomPickFromRareClass(DataTable.Trap, DataTable.TestRData[indexTest][3]);
                        CreateTrapCardToFightList(nowLevelAndHadChip);
                        break;
                    default:
                        break;
                }
            }
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
    public int RandomPickFromRareClass(IReadOnlyDictionary<int,IReadOnlyList<string>> data, string rarity)
    {
        var list = data.Where(map => map.Value[3] == rarity).ToList();
        return list[Random.Range(0, list.Count)].Key;
    }

    //初始化卡牌列表
    private void InitCardListShow()
    {
        var forceId = PlayerDataForGame.instance.CurrentWarForceId;

            PlayerDataForGame.instance.fightHeroId.Clear();
            PlayerDataForGame.instance.fightTowerId.Clear();
            PlayerDataForGame.instance.fightTrapId.Clear();

            var hstData = PlayerDataForGame.instance.hstData;
            //临时记录武将存档信息
            hstData.heroSaveData.Enlist(forceId).ToList()
                .ForEach(CreateHeroCardToFightList);
            hstData.soldierSaveData.Enlist(forceId).ToList()
                .ForEach(CreateSoldierCardToFightList);
            hstData.towerSaveData.Enlist(forceId).ToList()
                .ForEach(CreateTowerCardToFightList);
            hstData.trapSaveData.Enlist(forceId).ToList()
                .ForEach(CreateTrapCardToFightList);
    }
    //创建玩家武将卡牌
    private void CreateHeroCardToFightList(NowLevelAndHadChip cardData)
    {
        var re = GameResources;
        GameObject obj = Instantiate(cardForWarListPres, heroCardListObj.transform);
        obj.GetComponent<CardForDrag>().posIndex = -1;
        obj.transform.GetChild(1).GetComponent<Image>().sprite = re.HeroImg[cardData.id];
        ShowNameTextRules(obj.transform.GetChild(3).GetComponent<Text>(), DataTable.Hero[cardData.id][1]);
        //名字颜色
        obj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(DataTable.Hero[cardData.id][3]);
        obj.transform.GetChild(4).GetComponent<Image>().sprite = re.GradeImg[cardData.level];
        obj.transform.GetChild(5).GetComponentInChildren<Text>().text = DataTable.ClassData[int.Parse(DataTable.Hero[cardData.id][5])][3];
        //兵种框
        obj.transform.GetChild(5).GetComponent<Image>().sprite = re.ClassImg[0];
        FrameChoose(int.Parse(DataTable.Hero[cardData.id][3]), obj.transform.GetChild(6).GetComponent<Image>());
        //添加按住抬起方法
        FightForManager.instance.GiveGameObjEventForHoldOn(obj, DataTable.ClassData[int.Parse(DataTable.Hero[cardData.id][5])][4]);
        FightCardData data = new FightCardData();
        data.unitId = 1;
        data.cardObj = obj;
        data.cardType = 0;
        data.cardId = cardData.id;
        data.posIndex = -1;
        data.cardGrade = cardData.level;
        data.fightState = new FightState();
        data.damage = int.Parse(DataTable.Hero[data.cardId][7].Split(',')[data.cardGrade - 1]);
        data.hpr = int.Parse(DataTable.Hero[data.cardId][9]);
        data.fullHp = data.nowHp = int.Parse(DataTable.Hero[data.cardId][8].Split(',')[data.cardGrade - 1]);
        data.activeUnit = true;
        data.isPlayerCard = true;
        data.cardMoveType = int.Parse(DataTable.Hero[data.cardId][17]);
        data.cardDamageType = int.Parse(DataTable.Hero[data.cardId][18]);
        playerCardsDatas.Add(data);
    }
    //创建玩家士兵卡牌
    private void CreateSoldierCardToFightList(NowLevelAndHadChip cardData)
    {
        var re = GameResources;
        GameObject obj = Instantiate(cardForWarListPres, heroCardListObj.transform);
        obj.GetComponent<CardForDrag>().posIndex = -1;
        obj.transform.GetChild(1).GetComponent<Image>().sprite = re.FuZhuImg[int.Parse(DataTable.Soldier[cardData.id][13])];
        ShowNameTextRules(obj.transform.GetChild(3).GetComponent<Text>(), DataTable.Soldier[cardData.id][1]);
        //名字颜色
        obj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(DataTable.Soldier[cardData.id][3]);
        obj.transform.GetChild(4).GetComponent<Image>().sprite = re.GradeImg[cardData.level];
        obj.transform.GetChild(5).GetComponentInChildren<Text>().text = DataTable.ClassData[int.Parse(DataTable.Soldier[cardData.id][5])][3];
        //兵种框
        obj.transform.GetChild(5).GetComponent<Image>().sprite = re.ClassImg[1];
        FrameChoose(int.Parse(DataTable.Soldier[cardData.id][3]), obj.transform.GetChild(6).GetComponent<Image>());
        //添加按住抬起方法
        FightForManager.instance.GiveGameObjEventForHoldOn(obj, DataTable.ClassData[int.Parse(DataTable.Soldier[cardData.id][5])][4]);
        FightCardData data = new FightCardData();
        data.unitId = 1;
        data.cardObj = obj;
        data.cardType = 1;
        data.cardId = cardData.id;
        data.posIndex = -1;
        data.cardGrade = cardData.level;
        data.fightState = new FightState();
        data.damage = int.Parse(DataTable.Soldier[data.cardId][6].Split(',')[data.cardGrade - 1]);
        data.hpr = int.Parse(DataTable.Soldier[data.cardId][8]);
        data.fullHp = data.nowHp = int.Parse(DataTable.Soldier[data.cardId][7].Split(',')[data.cardGrade - 1]);
        data.activeUnit = true;
        data.isPlayerCard = true;
        playerCardsDatas.Add(data);
    }
    //创建玩家塔卡牌
    private void CreateTowerCardToFightList(NowLevelAndHadChip cardData)
    {
        var re = GameResources;
        GameObject obj = Instantiate(cardForWarListPres, heroCardListObj.transform);
        obj.GetComponent<CardForDrag>().posIndex = -1;
        obj.transform.GetChild(1).GetComponent<Image>().sprite = re.FuZhuImg[int.Parse(DataTable.Tower[cardData.id][10])];
        ShowNameTextRules(obj.transform.GetChild(3).GetComponent<Text>(), DataTable.Tower[cardData.id][1]);
        //名字颜色
        obj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(DataTable.Tower[cardData.id][3]);
        obj.transform.GetChild(4).GetComponent<Image>().sprite = re.GradeImg[cardData.level];
        obj.transform.GetChild(5).GetComponentInChildren<Text>().text = DataTable.Tower[cardData.id][5];
        //兵种框
        obj.transform.GetChild(5).GetComponent<Image>().sprite = re.ClassImg[1];
        FrameChoose(int.Parse(DataTable.Tower[cardData.id][3]), obj.transform.GetChild(6).GetComponent<Image>());
        //添加按住抬起方法
        FightForManager.instance.GiveGameObjEventForHoldOn(obj, DataTable.Tower[cardData.id][13]);
        FightCardData data = new FightCardData();
        data.unitId = 1;
        data.cardObj = obj;
        data.cardType = 2;
        data.cardId = cardData.id;
        data.posIndex = -1;
        data.cardGrade = cardData.level;
        data.fightState = new FightState();
        data.damage = int.Parse(DataTable.Tower[data.cardId][6].Split(',')[data.cardGrade - 1]);
        data.hpr = int.Parse(DataTable.Tower[data.cardId][8]);
        data.fullHp = data.nowHp = int.Parse(DataTable.Tower[data.cardId][7].Split(',')[data.cardGrade - 1]);
        data.activeUnit = (data.cardId == 0 || data.cardId == 1 || data.cardId == 2 || data.cardId == 3 || data.cardId == 6);
        data.isPlayerCard = true;
        data.cardMoveType = 1;
        data.cardDamageType = 0;
        playerCardsDatas.Add(data);
    }
    //创建玩家陷阱卡牌
    private void CreateTrapCardToFightList(NowLevelAndHadChip cardData)
    {
        var re = GameResources;
        GameObject obj = Instantiate(cardForWarListPres, heroCardListObj.transform);
        obj.GetComponent<CardForDrag>().posIndex = -1;
        obj.transform.GetChild(1).GetComponent<Image>().sprite = re.FuZhuImg[int.Parse(DataTable.Trap[cardData.id][9])];
        ShowNameTextRules(obj.transform.GetChild(3).GetComponent<Text>(), DataTable.Trap[cardData.id][1]);
        //名字颜色
        obj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(DataTable.Trap[cardData.id][3]);
        obj.transform.GetChild(4).GetComponent<Image>().sprite = re.GradeImg[cardData.level];
        obj.transform.GetChild(5).GetComponentInChildren<Text>().text = DataTable.Trap[cardData.id][5];
        //兵种框
        obj.transform.GetChild(5).GetComponent<Image>().sprite = re.ClassImg[1];
        FrameChoose(int.Parse(DataTable.Trap[cardData.id][3]), obj.transform.GetChild(6).GetComponent<Image>());
        //添加按住抬起方法
        FightForManager.instance.GiveGameObjEventForHoldOn(obj, DataTable.Trap[cardData.id][12]);
        FightCardData data = new FightCardData();
        data.unitId = 1;
        data.cardObj = obj;
        data.cardType = 3;
        data.cardId = cardData.id;
        data.posIndex = -1;
        data.cardGrade = cardData.level;
        data.fightState = new FightState();
        data.damage = int.Parse(DataTable.Trap[data.cardId][6].Split(',')[data.cardGrade - 1]);
        data.hpr = int.Parse(DataTable.Trap[data.cardId][8]);
        data.fullHp = data.nowHp = int.Parse(DataTable.Trap[data.cardId][7].Split(',')[data.cardGrade - 1]);
        data.activeUnit = false;
        data.isPlayerCard = true;
        data.cardMoveType = 0;
        data.cardDamageType = 0;
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
        battleNameText.text = DataTable.WarData[PlayerDataForGame.instance.selectedWarId][1];
        playerInfoObj.transform.GetChild(0).GetComponent<Text>().text = DataTable.PlayerInitialData[PlayerDataForGame.instance.pyData.ForceId][1];
        UpdateGoldandBoxNumsShow();
        UpdateLevelInfo();
        UpdateBattleSchedule();
    }

    //刷新战役进度显示
    private void UpdateBattleSchedule()
    {
        nowGuanQiaIndex++;
        if (nowGuanQiaIndex >= int.Parse(DataTable.WarData[PlayerDataForGame.instance.selectedWarId][4]))
        {
            nowGuanQiaIndex = int.Parse(DataTable.WarData[PlayerDataForGame.instance.selectedWarId][4]);
        }

        string str = nowGuanQiaIndex + "/" + DataTable.WarData[PlayerDataForGame.instance.selectedWarId][4];
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
        //等级
        cityLevelObj.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = cityLevel + "级";
        //武将可上阵
        maxHeroNums = int.Parse(DataTable.CityLevelData[cityLevel - 1][3]);
        cityLevelObj.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = DataTable.CityLevelData[cityLevel - 1][3];
        //升级金币
        upLevelBtn.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = DataTable.CityLevelData[cityLevel - 1][1];
    }

    /// <summary>
    /// 升级城池
    /// </summary>
    public void OnClickUpLevel()
    {
        ShowOrHideGuideObj(2, false);

        int needGold = int.Parse(DataTable.CityLevelData[cityLevel - 1][1]);
        if (GoldForCity < needGold)
        {
            PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(56));
            PlayAudioClip(20);
        }
        else
        {
            GoldForCity -= needGold;
            cityLevel++;
            PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(60));
            playerInfoObj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = GoldForCity.ToString();
            UpdateLevelInfo();
            //满级
            if (cityLevel >= DataTable.CityLevelData.Count)
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