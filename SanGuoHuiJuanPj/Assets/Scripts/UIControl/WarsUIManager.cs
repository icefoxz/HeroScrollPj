using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public GameObject[] eventsWindows; //各种事件窗口 0-战斗；1-故事；2-答题；3-奇遇；4-通用
    public enum EventTypes
    {
        Generic,//通用
        Battle,//战斗
        Story,//故事
        Quest,//答题
        Adventure,//奇遇
        Trade//交易
    }
    /// <summary>
    /// 上一个关卡类型
    /// </summary>
    private EventTypes lastEvent = EventTypes.Generic;
    [SerializeField]
    GameObject gameOverObj;  //战役结束ui
    [SerializeField]
    float percentReturnHp;    //回春回血百分比

    public int cityLevel;   //记录城池等级
    public int goldForCity; //记录城池金币

    public int GoldForCity
    {
        get
        {
            //为了容易调试，同步编辑器输入的金币数量。
            if (goldForCity != PlayerDataForGame.instance.warsData.baYe.gold)
                PlayerDataForGame.instance.warsData.baYe.gold = goldForCity;
            if (PlayerDataForGame.instance.WarType == PlayerDataForGame.WarTypes.Baye)
                return PlayerDataForGame.instance.warsData.baYe.gold;
            return goldForCity;
        }
        set
        {
            goldForCity = value;
            if (PlayerDataForGame.instance.WarType != PlayerDataForGame.WarTypes.Baye) return;
            PlayerDataForGame.instance.warsData.baYe.gold = value;
            PlayerDataForGame.instance.isNeedSaveData = true;
            LoadSaveData.instance.SaveGameData(3);
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
    List<int> baYeBattleList; //记录霸业的战斗关卡

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        isPointMoveNow = false;
        cityLevel = 1;
        switch (PlayerDataForGame.instance.WarType)
        {
            //战斗金币
            case PlayerDataForGame.WarTypes.Expedition:
                GoldForCity = PlayerDataForGame.instance.zhanYiColdNums;
                break;
            case PlayerDataForGame.WarTypes.Baye:
                GoldForCity = PlayerDataForGame.instance.warsData.baYe.gold;
                break;
            case PlayerDataForGame.WarTypes.None:
                XDebug.LogError<WarsUIManager>($"未确认战斗类型[{PlayerDataForGame.WarTypes.None}]，请在调用战斗场景前预设战斗类型。");
                throw new InvalidOperationException();
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
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerDataForGame.instance.lastSenceIndex = 2;
        //如果战斗是霸业，初始化霸业战斗id记录器，非霸业不使用
        if (PlayerDataForGame.instance.WarType == PlayerDataForGame.WarTypes.Baye)
            baYeBattleList = new List<int>();
        InitMainUIShow();

        InitCardListShow();

        InitGuanQiaShow();
    }

    //初始化关卡
    private void InitGuanQiaShow()
    {
        lastEvent = EventTypes.Generic;
        //尝试展示指引
        ShowOrHideGuideObj(0, true);
        InitShowParentGuanQia(LoadJsonFile.warTableDatas[PlayerDataForGame.instance.chooseWarsId][3]);
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
        string str = LoadJsonFile.pointTableDatas[parentGuanQiaId][1];
        if (nowGuanQiaIndex >= int.Parse(LoadJsonFile.warTableDatas[PlayerDataForGame.instance.chooseWarsId][4]))
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
                int eventId = int.Parse(LoadJsonFile.pointTableDatas[guanQiaId][4]);
                GameObject obj = Instantiate(guanQiaPreObj, point0Tran);
                obj.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                string eventType = LoadJsonFile.pointTableDatas[guanQiaId][3];
                if (DetermineIsFightGuanQia(int.Parse(eventType)))
                {
                    obj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = LoadJsonFile.pointTableDatas[guanQiaId][2];
                    obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                    if (eventType != "7")
                    {
                        obj.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/guanQiaEvents/" + (eventType == "1" ? "flag0" : "flag1"), typeof(Sprite)) as Sprite;
                        obj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = LoadJsonFile.pointTableDatas[guanQiaId][9];
                        obj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().fontSize = (LoadJsonFile.pointTableDatas[guanQiaId][9].Length > 2 ? 45 : 50);
                        obj.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                    }
                }
                obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/guanQiaEvents/" + LoadJsonFile.pointTableDatas[guanQiaId][6], typeof(Sprite)) as Sprite;

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
                levelIntroText.transform.parent.GetComponent<Image>().sprite = Resources.Load("Image/ArtWindow/" + randImg, typeof(Sprite)) as Sprite;
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
        levelIntroText.DOText(("\u2000\u2000\u2000\u2000" + LoadJsonFile.pointTableDatas[guanQiaId][5]), 3f).SetEase(Ease.Linear).SetAutoKill(false);
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

        int guanQiaSum = int.Parse(LoadJsonFile.warTableDatas[PlayerDataForGame.instance.chooseWarsId][4]);

        gameOverObj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = "×" + treasureChestNums;

        if (passedGuanQiaNums > PlayerDataForGame.instance.warsData.warUnlockSaveData[PlayerDataForGame.instance.chooseWarsId].unLockCount)
        {
            PlayerDataForGame.instance.warsData.warUnlockSaveData[PlayerDataForGame.instance.chooseWarsId].unLockCount = passedGuanQiaNums;
        }
        
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);
        gameOverObj.SetActive(true);

        //霸业的战斗金币传到主城
        if (PlayerDataForGame.instance.WarType == PlayerDataForGame.WarTypes.Baye)
        {
            PlayerDataForGame.instance.warsData.baYe.gold = GoldForCity;
        }
    }

    //初始化父级关卡
    private void InitShowParentGuanQia(string str)
    {
        passedGuanQiaNums++;
        if (passedGuanQiaNums >= int.Parse(LoadJsonFile.warTableDatas[PlayerDataForGame.instance.chooseWarsId][4]))
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
                    int guanQiaType = int.Parse(LoadJsonFile.pointTableDatas[guanQiaId][3]);
                    int eventId = int.Parse(LoadJsonFile.pointTableDatas[guanQiaId][4]);
                    GameObject obj = Instantiate(guanQiaPreObj, point1Tran);
                    obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/guanQiaEvents/" + LoadJsonFile.pointTableDatas[guanQiaId][6], typeof(Sprite)) as Sprite;
                    if (DetermineIsFightGuanQia(guanQiaType))  //战斗关卡城池名
                    {
                        obj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = LoadJsonFile.pointTableDatas[guanQiaId][2];
                        obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                        if (guanQiaType != 7)
                        {
                            obj.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/guanQiaEvents/" + (guanQiaType == 1 ? "flag0" : "flag1"), typeof(Sprite)) as Sprite;
                            obj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = LoadJsonFile.pointTableDatas[guanQiaId][9];
                            obj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().fontSize = (LoadJsonFile.pointTableDatas[guanQiaId][9].Length > 2 ? 45 : 50);
                            obj.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                        }
                    }
                    int randArtImg = Random.Range(0, 25);   //随机艺术图
                    obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        startBtn.GetComponentInChildren<Text>().text = DetermineIsFightGuanQia(guanQiaType) ? LoadJsonFile.GetStringText(53) : LoadJsonFile.GetStringText(54);
                        startBtn.SetActive(true);
                        SelectOneGuanQia(obj);
                        ChooseParentGuanQia(guanQiaId, randArtImg, obj.transform);
                        InterToDiffGuanQia(DetermineIsFightGuanQia(guanQiaType) ? 1 : guanQiaType, eventId, guanQiaId);
                    });
                }
            }
            StartCoroutine(LiteInitChooseFirst(0));
        }
    }

    //判断是否是战斗关卡
    private bool DetermineIsFightGuanQia(int guanQiaType)
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
        //如果是霸业，添加临时的战斗关卡记录
        if (PlayerDataForGame.instance.WarType == PlayerDataForGame.WarTypes.Baye) baYeBattleList.Add(guanQiaId);
        lastEvent = EventTypes.Battle;
        PlayAudioClip(21);

        fightBackImage.sprite = Resources.Load("Image/battleBG/" + LoadJsonFile.pointTableDatas[guanQiaId][7], typeof(Sprite)) as Sprite;
        int bgmIndex = int.Parse(LoadJsonFile.pointTableDatas[guanQiaId][8]);
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
        lastEvent = EventTypes.Story;
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
        lastEvent = EventTypes.Quest;
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
        lastEvent = isBuy ? EventTypes.Trade : EventTypes.Adventure;
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
        if (passedGuanQiaNums + 1 < int.Parse(LoadJsonFile.warTableDatas[PlayerDataForGame.instance.chooseWarsId][4]))
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
        InitShowParentGuanQia(LoadJsonFile.pointTableDatas[indexLastGuanQiaId][1]);
        TongGuanCityPointShow();

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
        eventsWindows[4].transform.GetChild(1).GetChild(2).GetComponent<Text>().text = LoadJsonFile.GetStringText(55);
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
    private int BackCardIdByWeightValue(List<List<string>> datas, int wvIndex)
    {
        int weightValueSum = 0;
        for (int i = 0; i < datas.Count; i++)
        {
            weightValueSum += int.Parse(datas[i][wvIndex]);
        }
        int randNum = Random.Range(0, weightValueSum);
        int indexTest = 0;
        while (randNum >= 0)
        {
            randNum -= int.Parse(datas[indexTest][wvIndex]);
            indexTest++;
        }
        indexTest -= 1;
        return indexTest;
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
    private bool UpdateShoppingList(int updateMoney)
    {
        if (updateMoney != 0)
        {
            PlayAudioClip(13);
        }

        if (updateMoney != 0 && updateMoney > GoldForCity)
        {
            PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(56));
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
                int indexId = BackCardIdByWeightValue(LoadJsonFile.encounterTableDatas, 1);
                int cardType = int.Parse(LoadJsonFile.encounterTableDatas[indexId][2]);
                string cardRarity = LoadJsonFile.encounterTableDatas[indexId][3];
                int cardLevel = int.Parse(LoadJsonFile.encounterTableDatas[indexId][4]);
                int cardId = 0;
                switch (cardType)
                {
                    case 0:
                        cardId = BackIdFromTableByRarity(LoadJsonFile.heroTableDatas, cardRarity);
                        woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/Hero/" + LoadJsonFile.heroTableDatas[cardId][16], typeof(Sprite)) as Sprite;
                        ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), LoadJsonFile.heroTableDatas[cardId][1]);
                        //名字颜色
                        woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.heroTableDatas[cardId][3]);
                        woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[cardId][5])][3];
                        //兵种框
                        woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 0, typeof(Sprite)) as Sprite;
                        FrameChoose(LoadJsonFile.heroTableDatas[cardId][3], woodsList.GetChild(i).GetChild(6).GetComponent<Image>());

                        woodsList.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                        woodsList.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                        {
                            OnClickToShowShopInfo(btnIndex, LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[cardId][5])][4]);
                        });
                        break;
                    case 1:
                        cardId = BackIdFromTableByRarity(LoadJsonFile.soldierTableDatas, cardRarity);
                        woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.soldierTableDatas[cardId][13], typeof(Sprite)) as Sprite;
                        ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), LoadJsonFile.soldierTableDatas[cardId][1]);
                        //名字颜色
                        woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.soldierTableDatas[cardId][3]);
                        woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.soldierTableDatas[cardId][5])][3];
                        //兵种框
                        woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
                        FrameChoose(LoadJsonFile.soldierTableDatas[cardId][3], woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
                        woodsList.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                        woodsList.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                        {
                            OnClickToShowShopInfo(btnIndex, LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.soldierTableDatas[cardId][5])][4]);
                        });
                        break;
                    case 2:
                        cardId = BackIdFromTableByRarity(LoadJsonFile.towerTableDatas, cardRarity);
                        woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.towerTableDatas[cardId][10], typeof(Sprite)) as Sprite;
                        ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), LoadJsonFile.towerTableDatas[cardId][1]);
                        //名字颜色
                        woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.towerTableDatas[cardId][3]);
                        woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.towerTableDatas[cardId][5];
                        //兵种框
                        woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
                        FrameChoose(LoadJsonFile.towerTableDatas[cardId][3], woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
                        woodsList.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                        woodsList.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                        {
                            OnClickToShowShopInfo(btnIndex, LoadJsonFile.towerTableDatas[cardId][12]);
                        });
                        break;
                    case 3:
                        cardId = BackIdFromTableByRarity(LoadJsonFile.trapTableDatas, cardRarity);
                        woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.trapTableDatas[cardId][8], typeof(Sprite)) as Sprite;
                        ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), LoadJsonFile.trapTableDatas[cardId][1]);
                        //名字颜色
                        woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.trapTableDatas[cardId][3]);
                        woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.trapTableDatas[cardId][5];
                        //兵种框
                        woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
                        FrameChoose(LoadJsonFile.trapTableDatas[cardId][3], woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
                        woodsList.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                        woodsList.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                        {
                            OnClickToShowShopInfo(btnIndex, LoadJsonFile.trapTableDatas[cardId][11]);
                        });
                        break;
                    case 4:
                        break;
                    default:
                        break;
                }
                woodsList.GetChild(i).GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + cardLevel, typeof(Sprite)) as Sprite;

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
                    adBtn.onClick.AddListener(delegate ()
                    {
                        adBtn.enabled = false;
                        if (!DoNewAdController.instance.GetReWardVideo(
                        //if (!AdController.instance.ShowVideo(
                            delegate ()
                            {
                                GetOrBuyCards(true, needMoney, cardType, cardId, cardLevel, btnIndex);
                                getBtnTran.GetChild(2).gameObject.SetActive(false);
                                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(57));
                                adBtn.enabled = true;
                            },
                            delegate ()
                            {
                                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(6));
                                adBtn.enabled = true;
                            }))
                        {
                            PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(6));
                            adBtn.enabled = true;
                        }
                    });
                }
                else
                {
                    needMoney = int.Parse(LoadJsonFile.encounterTableDatas[indexId][5]);
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
    public void WatchAdForUpdateQiYv()
    {
        Button adBtn = eventsWindows[3].transform.GetChild(0).GetChild(5).GetComponent<Button>();
        adBtn.enabled = false;
        if (!DoNewAdController.instance.GetReWardVideo(
        //if (!AdController.instance.ShowVideo(
            delegate ()
            {
                adBtn.gameObject.SetActive(false);
                UpdateQiYvWoods();
                adBtn.enabled = true;
            },
            delegate ()
            {
                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(6));
                adBtn.enabled = true;
            }
            ))
        {
            PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(6));
            adBtn.enabled = true;
        }
    }

    //刷新奇遇商品
    private void UpdateQiYvWoods()
    {
        int indexId = BackCardIdByWeightValue(LoadJsonFile.encounterTableDatas, 1);
        int cardType = int.Parse(LoadJsonFile.encounterTableDatas[indexId][2]);
        string cardRarity = LoadJsonFile.encounterTableDatas[indexId][3];
        int cardLevel = int.Parse(LoadJsonFile.encounterTableDatas[indexId][4]);
        Transform woodsList = eventsWindows[3].transform.GetChild(0).GetChild(1);
        switch (cardType)
        {
            case 0:
                for (int i = 0; i < 3; i++)
                {
                    int cardId = BackIdFromTableByRarity(LoadJsonFile.heroTableDatas, cardRarity);
                    woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/Hero/" + LoadJsonFile.heroTableDatas[cardId][16], typeof(Sprite)) as Sprite;
                    ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), LoadJsonFile.heroTableDatas[cardId][1]);
                    //名字颜色
                    woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.heroTableDatas[cardId][3]);
                    woodsList.GetChild(i).GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + cardLevel, typeof(Sprite)) as Sprite;
                    woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[cardId][5])][3];
                    //兵种框
                    woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 0, typeof(Sprite)) as Sprite;
                    FrameChoose(LoadJsonFile.heroTableDatas[cardId][3], woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
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
                        OnClickToShowShopInfo(btnIndex, LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[cardId][5])][4]);
                    });
                }
                break;
            case 1:
                for (int i = 0; i < 3; i++)
                {
                    int cardId = BackIdFromTableByRarity(LoadJsonFile.soldierTableDatas, cardRarity);
                    woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.soldierTableDatas[cardId][13], typeof(Sprite)) as Sprite;
                    ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), LoadJsonFile.soldierTableDatas[cardId][1]);
                    //名字颜色
                    woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.soldierTableDatas[cardId][3]);
                    woodsList.GetChild(i).GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + cardLevel, typeof(Sprite)) as Sprite;
                    woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.soldierTableDatas[cardId][5])][3];
                    //兵种框
                    woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
                    FrameChoose(LoadJsonFile.soldierTableDatas[cardId][3], woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
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
                        OnClickToShowShopInfo(btnIndex, LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.soldierTableDatas[cardId][5])][4]);
                    });
                }
                break;
            case 2:
                for (int i = 0; i < 3; i++)
                {
                    int cardId = BackIdFromTableByRarity(LoadJsonFile.towerTableDatas, cardRarity);
                    woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.towerTableDatas[cardId][10], typeof(Sprite)) as Sprite;
                    ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), LoadJsonFile.towerTableDatas[cardId][1]);
                    //名字颜色
                    woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.towerTableDatas[cardId][3]);
                    woodsList.GetChild(i).GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + cardLevel, typeof(Sprite)) as Sprite;
                    woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.towerTableDatas[cardId][5];
                    FrameChoose(LoadJsonFile.towerTableDatas[cardId][3], woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
                    //兵种框
                    woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
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
                        OnClickToShowShopInfo(btnIndex, LoadJsonFile.towerTableDatas[cardId][12]);
                    });
                }
                break;
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    int cardId = BackIdFromTableByRarity(LoadJsonFile.trapTableDatas, cardRarity);
                    woodsList.GetChild(i).GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.trapTableDatas[cardId][8], typeof(Sprite)) as Sprite;
                    ShowNameTextRules(woodsList.GetChild(i).GetChild(3).GetComponent<Text>(), LoadJsonFile.trapTableDatas[cardId][1]);
                    //名字颜色
                    woodsList.GetChild(i).GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.trapTableDatas[cardId][3]);
                    woodsList.GetChild(i).GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + cardLevel, typeof(Sprite)) as Sprite;
                    woodsList.GetChild(i).GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.trapTableDatas[cardId][5];
                    //兵种框
                    woodsList.GetChild(i).GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
                    FrameChoose(LoadJsonFile.trapTableDatas[cardId][3], woodsList.GetChild(i).GetChild(6).GetComponent<Image>());
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
                        OnClickToShowShopInfo(btnIndex, LoadJsonFile.trapTableDatas[cardId][11]);
                    });
                }
                break;
            case 4:
                break;
            default:
                break;
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
            tran.GetChild(0).GetComponent<Image>().sprite = Resources.Load("Image/guanQiaEvents/6", typeof(Sprite)) as Sprite;
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

            tran.GetChild(0).GetComponent<Image>().sprite = Resources.Load("Image/guanQiaEvents/5", typeof(Sprite)) as Sprite;
            tran.GetChild(3).gameObject.SetActive(false);
            tran.GetChild(5).gameObject.SetActive(true);
            UpdateQiYvWoods();
        }
    }

    //获得或购买三选物品
    private void GetOrBuyCards(bool isBuy, int money, int cardType, int cardId, int cardLevel, int btnIndex)
    {
        if (isBuy)
        {
            PlayAudioClip(13);
            if (money > GoldForCity)
            {
                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(56));
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
            case 0:
                CreateHeroCardToFightList(nowLevelAndHadChip);
                break;
            case 1:
                CreateSoldierCardToFightList(nowLevelAndHadChip);
                break;
            case 2:
                CreateTowerCardToFightList(nowLevelAndHadChip);
                break;
            case 3:
                CreateTrapCardToFightList(nowLevelAndHadChip);
                break;
            default:
                break;
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
        int rand = Random.Range(0, LoadJsonFile.storyTableDatas.Count);
        //典故名
        eventsWindows[1].transform.GetChild(0).GetChild(3).GetComponent<Text>().text = LoadJsonFile.storyTableDatas[rand][1];
        //故事
        eventsWindows[1].transform.GetChild(1).GetComponent<Text>().text = "\u2000\u2000\u2000\u2000" + LoadJsonFile.storyTableDatas[rand][2];
        //选项
        for (int i = 2; i < 4; i++)
        {
            eventsWindows[1].transform.GetChild(i).GetChild(0).GetComponent<Text>().text = LoadJsonFile.storyTableDatas[rand][i + 2];
            int indexI = i + 4;
            eventsWindows[1].transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
            {
                OnClickChooseAnsForDianGu(int.Parse(LoadJsonFile.storyTableDatas[rand][indexI]));
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

        int goldReward = int.Parse(LoadJsonFile.storyRTableDatas[indexAns][6]);
        if (goldReward < 0)
        {
            goldReward = GoldForCity >= Mathf.Abs(goldReward) ? goldReward : -GoldForCity;
        }
        if (goldReward != 0)
        {
            GoldForCity += goldReward;
            playerInfoObj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = GoldForCity.ToString();
        }
        int unitType = int.Parse(LoadJsonFile.storyRTableDatas[indexAns][2]);
        if (unitType >= 0)
        {
            NowLevelAndHadChip nowLevelAndHadChip = new NowLevelAndHadChip();
            nowLevelAndHadChip.id = int.Parse(LoadJsonFile.storyRTableDatas[indexAns][3]);
            nowLevelAndHadChip.level = int.Parse(LoadJsonFile.storyRTableDatas[indexAns][4]);
            int getNums = int.Parse(LoadJsonFile.storyRTableDatas[indexAns][5]);
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
        storyEndText.DOText(("\u2000\u2000\u2000\u2000" + LoadJsonFile.storyRTableDatas[indexAns][1]), 4f).OnComplete(delegate ()
        {
            eventsWindows[1].transform.GetChild(4).gameObject.SetActive(true);
        });
    }

    //初始化答题界面
    private void InitializeDaTi()
    {
        eventsWindows[2].transform.GetChild(6).gameObject.SetActive(false);
        int indexTest = BackCardIdByWeightValue(LoadJsonFile.testTableDatas, 6);
        int truthNum = int.Parse(LoadJsonFile.testTableDatas[indexTest][2]);
        eventsWindows[2].transform.GetChild(2).GetComponent<Text>().text = LoadJsonFile.testTableDatas[indexTest][1];
        for (int i = 3; i < 6; i++)
        {
            bool isRight = (i - 2) == truthNum;
            int btnIndex = i;
            eventsWindows[2].transform.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.white;
            eventsWindows[2].transform.GetChild(i).GetChild(0).GetComponent<Text>().text = LoadJsonFile.testTableDatas[indexTest][i];
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
            rewardStr = LoadJsonFile.GetStringText(58);
            eventsWindows[2].transform.GetChild(btnIndex).GetChild(0).GetComponent<Text>().color = Color.green;
            int indexTest = BackCardIdByWeightValue(LoadJsonFile.testRTableDatas, 1);
            int unitType = int.Parse(LoadJsonFile.testRTableDatas[indexTest][2]);
            if (unitType >= 0)
            {
                NowLevelAndHadChip nowLevelAndHadChip = new NowLevelAndHadChip();
                nowLevelAndHadChip.level = int.Parse(LoadJsonFile.testRTableDatas[indexTest][4]);
                switch (unitType)
                {
                    case 0:
                        nowLevelAndHadChip.id = BackIdFromTableByRarity(LoadJsonFile.heroTableDatas, LoadJsonFile.testRTableDatas[indexTest][3]);
                        CreateHeroCardToFightList(nowLevelAndHadChip);
                        break;
                    case 1:
                        nowLevelAndHadChip.id = BackIdFromTableByRarity(LoadJsonFile.soldierTableDatas, LoadJsonFile.testRTableDatas[indexTest][3]);
                        CreateSoldierCardToFightList(nowLevelAndHadChip);
                        break;
                    case 2:
                        nowLevelAndHadChip.id = BackIdFromTableByRarity(LoadJsonFile.towerTableDatas, LoadJsonFile.testRTableDatas[indexTest][3]);
                        CreateTowerCardToFightList(nowLevelAndHadChip);
                        break;
                    case 3:
                        nowLevelAndHadChip.id = BackIdFromTableByRarity(LoadJsonFile.trapTableDatas, LoadJsonFile.testRTableDatas[indexTest][3]);
                        CreateTrapCardToFightList(nowLevelAndHadChip);
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            rewardStr = LoadJsonFile.GetStringText(59);
            eventsWindows[2].transform.GetChild(btnIndex).GetChild(0).GetComponent<Text>().color = Color.red;
        }
        PlayerDataForGame.instance.ShowStringTips(rewardStr);
        eventsWindows[2].transform.GetChild(6).gameObject.SetActive(true);
    }

    //根据稀有度返回随机id
    public int BackIdFromTableByRarity(List<List<string>> datas, string rarity)
    {
        int cardId = 0;
        List<int> idList = new List<int>();
        for (int i = 0; i < datas.Count; i++)
        {
            if (datas[i][3] == rarity)
            {
                idList.Add(i);
            }
        }
        if (idList.Count > 0)
        {
            cardId = idList[Random.Range(0, idList.Count)];
        }
        return cardId;
    }

    //初始化卡牌列表
    private void InitCardListShow()
    {
        NowLevelAndHadChip cardData = new NowLevelAndHadChip();
        for (int i = 0; i < PlayerDataForGame.instance.fightHeroId.Count; i++)
        {
            cardData = PlayerDataForGame.instance.hstData.heroSaveData[
                FindIndexFromData(PlayerDataForGame.instance.hstData.heroSaveData, PlayerDataForGame.instance.fightHeroId[i])];
            CreateHeroCardToFightList(cardData);
        }

        for (int i = 0; i < PlayerDataForGame.instance.fightSoLdierId.Count; i++)
        {
            cardData = PlayerDataForGame.instance.hstData.soldierSaveData[
                FindIndexFromData(PlayerDataForGame.instance.hstData.soldierSaveData, PlayerDataForGame.instance.fightSoLdierId[i])];
            CreateSoldierCardToFightList(cardData);
        }
        for (int i = 0; i < PlayerDataForGame.instance.fightTowerId.Count; i++)
        {
            cardData = PlayerDataForGame.instance.hstData.towerSaveData[
                FindIndexFromData(PlayerDataForGame.instance.hstData.towerSaveData, PlayerDataForGame.instance.fightTowerId[i])];
            CreateTowerCardToFightList(cardData);
        }
        for (int i = 0; i < PlayerDataForGame.instance.fightTrapId.Count; i++)
        {
            cardData = PlayerDataForGame.instance.hstData.trapSaveData[
                FindIndexFromData(PlayerDataForGame.instance.hstData.trapSaveData, PlayerDataForGame.instance.fightTrapId[i])];
            CreateTrapCardToFightList(cardData);
        }
    }
    //创建玩家武将卡牌
    private void CreateHeroCardToFightList(NowLevelAndHadChip cardData)
    {
        GameObject obj = Instantiate(cardForWarListPres, heroCardListObj.transform);
        obj.GetComponent<CardForDrag>().posIndex = -1;
        obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/Hero/" + LoadJsonFile.heroTableDatas[cardData.id][16], typeof(Sprite)) as Sprite;
        ShowNameTextRules(obj.transform.GetChild(3).GetComponent<Text>(), LoadJsonFile.heroTableDatas[cardData.id][1]);
        //名字颜色
        obj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.heroTableDatas[cardData.id][3]);
        obj.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + cardData.level, typeof(Sprite)) as Sprite;
        obj.transform.GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[cardData.id][5])][3];
        //兵种框
        obj.transform.GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 0, typeof(Sprite)) as Sprite;
        FrameChoose(LoadJsonFile.heroTableDatas[cardData.id][3], obj.transform.GetChild(6).GetComponent<Image>());
        //添加按住抬起方法
        FightForManager.instance.GiveGameObjEventForHoldOn(obj, LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[cardData.id][5])][4]);
        FightCardData data = new FightCardData();
        data.unitId = 1;
        data.cardObj = obj;
        data.cardType = 0;
        data.cardId = cardData.id;
        data.posIndex = -1;
        data.cardGrade = cardData.level;
        data.fightState = new FightState();
        data.damage = int.Parse(LoadJsonFile.heroTableDatas[data.cardId][7].Split(',')[data.cardGrade - 1]);
        data.hpr = int.Parse(LoadJsonFile.heroTableDatas[data.cardId][9]);
        data.fullHp = data.nowHp = int.Parse(LoadJsonFile.heroTableDatas[data.cardId][8].Split(',')[data.cardGrade - 1]);
        data.activeUnit = true;
        data.isPlayerCard = true;
        data.cardMoveType = int.Parse(LoadJsonFile.heroTableDatas[data.cardId][17]);
        data.cardDamageType = int.Parse(LoadJsonFile.heroTableDatas[data.cardId][18]);
        playerCardsDatas.Add(data);
    }
    //创建玩家士兵卡牌
    private void CreateSoldierCardToFightList(NowLevelAndHadChip cardData)
    {
        GameObject obj = Instantiate(cardForWarListPres, heroCardListObj.transform);
        obj.GetComponent<CardForDrag>().posIndex = -1;
        obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.soldierTableDatas[cardData.id][13], typeof(Sprite)) as Sprite;
        ShowNameTextRules(obj.transform.GetChild(3).GetComponent<Text>(), LoadJsonFile.soldierTableDatas[cardData.id][1]);
        //名字颜色
        obj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.soldierTableDatas[cardData.id][3]);
        obj.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + cardData.level, typeof(Sprite)) as Sprite;
        obj.transform.GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.soldierTableDatas[cardData.id][5])][3];
        //兵种框
        obj.transform.GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
        FrameChoose(LoadJsonFile.soldierTableDatas[cardData.id][3], obj.transform.GetChild(6).GetComponent<Image>());
        //添加按住抬起方法
        FightForManager.instance.GiveGameObjEventForHoldOn(obj, LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.soldierTableDatas[cardData.id][5])][4]);
        FightCardData data = new FightCardData();
        data.unitId = 1;
        data.cardObj = obj;
        data.cardType = 1;
        data.cardId = cardData.id;
        data.posIndex = -1;
        data.cardGrade = cardData.level;
        data.fightState = new FightState();
        data.damage = int.Parse(LoadJsonFile.soldierTableDatas[data.cardId][6].Split(',')[data.cardGrade - 1]);
        data.hpr = int.Parse(LoadJsonFile.soldierTableDatas[data.cardId][8]);
        data.fullHp = data.nowHp = int.Parse(LoadJsonFile.soldierTableDatas[data.cardId][7].Split(',')[data.cardGrade - 1]);
        data.activeUnit = true;
        data.isPlayerCard = true;
        playerCardsDatas.Add(data);
    }
    //创建玩家塔卡牌
    private void CreateTowerCardToFightList(NowLevelAndHadChip cardData)
    {
        GameObject obj = Instantiate(cardForWarListPres, heroCardListObj.transform);
        obj.GetComponent<CardForDrag>().posIndex = -1;
        obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.towerTableDatas[cardData.id][10], typeof(Sprite)) as Sprite;
        ShowNameTextRules(obj.transform.GetChild(3).GetComponent<Text>(), LoadJsonFile.towerTableDatas[cardData.id][1]);
        //名字颜色
        obj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.towerTableDatas[cardData.id][3]);
        obj.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + cardData.level, typeof(Sprite)) as Sprite;
        obj.transform.GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.towerTableDatas[cardData.id][5];
        //兵种框
        obj.transform.GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
        FrameChoose(LoadJsonFile.towerTableDatas[cardData.id][3], obj.transform.GetChild(6).GetComponent<Image>());
        //添加按住抬起方法
        FightForManager.instance.GiveGameObjEventForHoldOn(obj, LoadJsonFile.towerTableDatas[cardData.id][12]);
        FightCardData data = new FightCardData();
        data.unitId = 1;
        data.cardObj = obj;
        data.cardType = 2;
        data.cardId = cardData.id;
        data.posIndex = -1;
        data.cardGrade = cardData.level;
        data.fightState = new FightState();
        data.damage = int.Parse(LoadJsonFile.towerTableDatas[data.cardId][6].Split(',')[data.cardGrade - 1]);
        data.hpr = int.Parse(LoadJsonFile.towerTableDatas[data.cardId][8]);
        data.fullHp = data.nowHp = int.Parse(LoadJsonFile.towerTableDatas[data.cardId][7].Split(',')[data.cardGrade - 1]);
        data.activeUnit = (data.cardId == 0 || data.cardId == 1 || data.cardId == 2 || data.cardId == 3 || data.cardId == 6);
        data.isPlayerCard = true;
        data.cardMoveType = 1;
        data.cardDamageType = 0;
        playerCardsDatas.Add(data);
    }
    //创建玩家陷阱卡牌
    private void CreateTrapCardToFightList(NowLevelAndHadChip cardData)
    {
        GameObject obj = Instantiate(cardForWarListPres, heroCardListObj.transform);
        obj.GetComponent<CardForDrag>().posIndex = -1;
        obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.trapTableDatas[cardData.id][8], typeof(Sprite)) as Sprite;
        ShowNameTextRules(obj.transform.GetChild(3).GetComponent<Text>(), LoadJsonFile.trapTableDatas[cardData.id][1]);
        //名字颜色
        obj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.trapTableDatas[cardData.id][3]);
        obj.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + cardData.level, typeof(Sprite)) as Sprite;
        obj.transform.GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.trapTableDatas[cardData.id][5];
        //兵种框
        obj.transform.GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
        FrameChoose(LoadJsonFile.trapTableDatas[cardData.id][3], obj.transform.GetChild(6).GetComponent<Image>());
        //添加按住抬起方法
        FightForManager.instance.GiveGameObjEventForHoldOn(obj, LoadJsonFile.trapTableDatas[cardData.id][11]);
        FightCardData data = new FightCardData();
        data.unitId = 1;
        data.cardObj = obj;
        data.cardType = 3;
        data.cardId = cardData.id;
        data.posIndex = -1;
        data.cardGrade = cardData.level;
        data.fightState = new FightState();
        data.damage = int.Parse(LoadJsonFile.trapTableDatas[data.cardId][6].Split(',')[data.cardGrade - 1]);
        data.hpr = int.Parse(LoadJsonFile.trapTableDatas[data.cardId][10]);
        data.fullHp = data.nowHp = int.Parse(LoadJsonFile.trapTableDatas[data.cardId][7].Split(',')[data.cardGrade - 1]);
        data.activeUnit = false;
        data.isPlayerCard = true;
        data.cardMoveType = 0;
        data.cardDamageType = 0;
        playerCardsDatas.Add(data);
    }

    //匹配稀有度边框
    public void FrameChoose(string rarity, Image img)
    {
        img.enabled = true;
        switch (rarity)
        {
            case "4":
                img.sprite = Resources.Load("Image/frameImage/tong", typeof(Sprite)) as Sprite;
                break;
            case "5":
                img.sprite = Resources.Load("Image/frameImage/yin", typeof(Sprite)) as Sprite;
                break;
            case "6":
                img.sprite = Resources.Load("Image/frameImage/jin", typeof(Sprite)) as Sprite;
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
        battleNameText.text = LoadJsonFile.warTableDatas[PlayerDataForGame.instance.chooseWarsId][1];
        playerInfoObj.transform.GetChild(0).GetComponent<Text>().text = LoadJsonFile.playerInitialTableDatas[PlayerDataForGame.instance.pyData.forceId][1];
        UpdateGoldandBoxNumsShow();
        UpdateLevelInfo();
        UpdateBattleSchedule();
    }

    //刷新战役进度显示
    private void UpdateBattleSchedule()
    {
        //如果是霸业
        if(PlayerDataForGame.instance.WarType == PlayerDataForGame.WarTypes.Baye)
        {
            /**
             * -判断.上一个场景是不是战斗。
             * -判断.第几个霸业战斗
             * -判断.霸业经验奖励是否已被领取
             * 1.加经验
             */
            if (lastEvent == EventTypes.Battle)
            {
                var baYe = PlayerDataForGame.instance.warsData.baYe;
                var field = baYe.data.Single(f => f.CityId == PlayerDataForGame.instance.selectedCity);
                if(!field.PassedStages[baYeBattleList.Count-1])//如果过关未被记录
                {
                    var exp = field.ExpList[baYeBattleList.Count - 1];//获取相应经验值
                    field.PassedStages[baYeBattleList.Count -1] = true;
                    PlayerDataForGame.instance.baYeManager.SetExp(exp);//给玩家加经验值
                }
            }
        }
        nowGuanQiaIndex++;
        if (nowGuanQiaIndex >= int.Parse(LoadJsonFile.warTableDatas[PlayerDataForGame.instance.chooseWarsId][4]))
        {
            nowGuanQiaIndex = int.Parse(LoadJsonFile.warTableDatas[PlayerDataForGame.instance.chooseWarsId][4]);
        }

        string str = nowGuanQiaIndex + "/" + LoadJsonFile.warTableDatas[PlayerDataForGame.instance.chooseWarsId][4];
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
        maxHeroNums = int.Parse(LoadJsonFile.cityLevelTableDatas[cityLevel - 1][3]);
        cityLevelObj.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = LoadJsonFile.cityLevelTableDatas[cityLevel - 1][3];
        //升级金币
        upLevelBtn.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = LoadJsonFile.cityLevelTableDatas[cityLevel - 1][1];
    }

    /// <summary>
    /// 升级城池
    /// </summary>
    public void OnClickUpLevel()
    {
        ShowOrHideGuideObj(2, false);

        int needGold = int.Parse(LoadJsonFile.cityLevelTableDatas[cityLevel - 1][1]);
        if (GoldForCity < needGold)
        {
            PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(56));
            PlayAudioClip(20);
        }
        else
        {
            GoldForCity -= needGold;
            cityLevel++;
            PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(60));
            playerInfoObj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = GoldForCity.ToString();
            UpdateLevelInfo();
            //满级
            if (cityLevel >= LoadJsonFile.cityLevelTableDatas.Count)
            {
                upLevelBtn.GetComponent<Button>().enabled = false;
                upLevelBtn.transform.GetChild(0).gameObject.SetActive(false);
                upLevelBtn.transform.GetChild(1).GetComponent<Text>().text = LoadJsonFile.GetStringText(61);
                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(62));
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
        AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[indexClips], AudioController0.instance.audioVolumes[indexClips]);
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
            musicBtnText.text = LoadJsonFile.GetStringText(41);
        }
        else
        {
            musicBtnText.text = LoadJsonFile.GetStringText(42);
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
            musicBtnText.text = LoadJsonFile.GetStringText(42);
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
            musicBtnText.text = LoadJsonFile.GetStringText(41);
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
                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(52));
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