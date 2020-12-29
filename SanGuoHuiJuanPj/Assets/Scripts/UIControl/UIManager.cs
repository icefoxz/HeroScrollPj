using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    GameObject zhuChengHeroContentObj;  //主城卡牌集合框
    [SerializeField]
    GameObject heroCardCityPre; //主城武将卡牌prefab
    [SerializeField]
    GameObject playerInfoObj;   //玩家信息obj
    [SerializeField]
    public Text yuanBaoNumText;        //元宝数量Text
    [SerializeField]
    public Text yvQueNumText;          //玉阙数量Text
    [SerializeField]
    Text tiLiNumText;           //体力数量Text
    [SerializeField]
    Text cardsListTitle;         //主城卡牌列表标题
    [SerializeField]
    Text cardsNumsTitle;         //主城卡牌列表中的单位数量
    [SerializeField]
    Image changeCardsListBtn;         //主城卡牌列表切换按钮势力图片
    [SerializeField]
    Image changeCardsListNameImg;     //主城卡牌列表切换按钮势力文字图片
    [SerializeField]
    GameObject showCardObj;     //上部展示的卡牌
    [SerializeField]
    Transform infoTran;         //上部展示的信息栏
    [SerializeField]
    GameObject heChengBtn;      //合成按钮obj
    [SerializeField]
    GameObject heImgObj;        //合成文字图片
    [SerializeField]
    GameObject rewardsShowObj;  //奖品展示UI
    [SerializeField]
    GameObject[] zhuChengInterFaces;    //主城切换页面0桃园1主城2战役3霸业4对战
    [SerializeField]
    GameObject[] particlesForInterface;    //主城页面对应粒子效果0桃园1主城2战役
    [SerializeField]
    GameObject warsChooseListObj;   //战役选择列表obj
    [SerializeField]
    GameObject warsChooseBtnPreObj;   //战役选择按钮obj
    [SerializeField]
    Text warIntroText;   //战役介绍文本obj
    [SerializeField]
    GameObject holdOrFightBtn;      //出战或回城切换按钮obj
    [SerializeField]
    GameObject sellCardBtn;      //出售按钮obj
    [SerializeField]
    Text tiLiRecordTimer;   //体力恢复倒计时
    [SerializeField]
    GameObject queRenWindows;   //操作确认窗口
    [SerializeField]
    public GameObject[] boxBtnObjs;    //宝箱obj

    public Text JinNangQuota;    //锦囊配额文本
    public Text JiuTanQuota;    //酒坛配额文本

    [SerializeField]
    Transform rewardsParent;    //奖品父级
    [SerializeField]
    GameObject rewardObj;       //奖品预制件

    private int needYuanBaoNums;    //记录所需元宝数

    Image lastSelectImg;    //对上一个选择的卡牌selectImg的标记
    NowLevelAndHadChip selectCardData;  //记录选择的卡牌存档数据

    bool isJumping; //记录界面是否进行跳转

    private int minInitCardCount = 20;  //卡牌池基础数量
    private List<GameObject> heroCardPoolList = new List<GameObject>();     //卡牌池

    private int tiLiCostIndex; //记录难度对应索引-获取单场消耗体力数量

    [SerializeField]
    Transform chonseWarDifTran; //难度选择父级

    [SerializeField]
    GameObject upStarEffectObj; //升星特效

    [SerializeField]
    GameObject[] guideObjs; // 指引objs 0:桃园宝箱 1:战役宝箱 2:合成 3:开始战役

    public Button adBtnForFreeBox; //免费开启酒坛广告按钮

    [SerializeField]
    GameObject chickenEntObj;   //体力入口
    [SerializeField]
    GameObject chickenShopWindowObj;    //烧鸡商店窗口
    [SerializeField]
    Button[] chickenShopBtns;   //体力商店购买按钮
    [SerializeField]
    Text chickenCloseText;  //烧鸡关闭时间Text

    [SerializeField]
    GameObject cutTiLiTextObj;  //扣除体力动画Obj

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        isJumping = false;
        needYuanBaoNums = 0;
        indexChooseListForceId = 0;
        selectCardData = new NowLevelAndHadChip();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitHeroCardPooling();

        InitializationPlayerInfo();
        InitWarDifBtnShow();

        InitChickenOpenTs();
        InitChickenBtnFun();
        InitJiBanForMainFun();
        InitBaYeFun();
        PlayerDataForGame.instance.ClearGarbageStationObj();
    }

    //时间管理
    private void OnEnable()
    {
        AudioController1.instance.ChangeBackMusic();
        TimeSystemControl.instance.isOpenMainScene = true;

        Invoke("GetBackTiLiForFight", 2f);
    }
    private void OnDisable()
    {
        TimeSystemControl.instance.isOpenMainScene = false;
    }

    [SerializeField]
    GameObject huiJuanWinObj;   //绘卷窗口obj

    [SerializeField]
    GameObject jiBanBtnsConObj;  //羁绊按钮集合窗口obj

    [SerializeField]
    GameObject jiBanInfoConObj; //羁绊详情窗口obj

    [SerializeField]
    Transform jibanBtnBoxTran;  //羁绊按钮集合

    [SerializeField]
    Transform jibanHeroBoxTran; //羁绊详情武将集合

    [SerializeField]
    Button jiBanWinCloseBtn;    //羁绊界面关闭按钮

    [SerializeField]
    GameObject baYeEventsObj;   //霸业事件点父级
    [SerializeField]
    GameObject chooseBaYeEventImg;  //选择霸业地点的Img
    [SerializeField]
    GameObject baYeForceObj;    //霸业势力选择父级
    [SerializeField]
    Text baYeGoldNumText;   //霸业金币数量

    //开始霸业战斗
    public void StartBaYeFight()
    {
        if (baYeFoeceChooseImgObj != null && baYeEventChooseIndex != -1)
        {
            print("可以开始战斗");
            if (!isJumping)
            {
                isJumping = true;
                AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[12], AudioController0.instance.audioVolumes[12]);
                AudioController0.instance.PlayAudioSource(0);

                StartCoroutine(LateGoToFightScene());
            }
            else
            {
                PlayOnClickMusic();
            }
        }
        else
        {
            print("请选择");
        }
    }

    //初始化霸业界面内容
    private void InitBaYeFun()
    {
        baYeEventChooseIndex = -1;
        //城市点初始化
        for (int i = 0; i < baYeEventsObj.transform.childCount; i++)
        {
            baYeEventsObj.transform.GetChild(i).gameObject.SetActive(false);
        }
        string[] eventCitys = LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.level - 1][7].Split(',');
        for (int i = 0; i < eventCitys.Length; i++)
        {
            if (eventCitys[i] != "")
            {
                int indexId = i;
                //得到战役id
                int indexWarId = GetChooseEventsWarId(indexId);
                baYeEventsObj.transform.GetChild(i).GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                {
                    ChooseBaYeEventOnClick(indexId, indexWarId);
                });
                baYeEventsObj.transform.GetChild(i).GetComponentInChildren<Text>().text = LoadJsonFile.baYeDiTuTableDatas[i][3];    //城市名
                baYeEventsObj.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        //势力选择
        int forceUnlockId = int.Parse(LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.level - 1][6]);
        for (int i = 0; i < forceUnlockId + 1; i++)
        {
            int index = i;
            GameObject obj = baYeForceObj.transform.GetChild(index).gameObject;
            obj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load("Image/shiLi/Flag/" + index, typeof(Sprite)) as Sprite;
            obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/shiLi/Name/" + index, typeof(Sprite)) as Sprite;
            obj.GetComponent<Button>().onClick.AddListener(delegate() {
                ChooseBaYeForceOnClick(index, obj.transform.GetChild(2).gameObject);
            });
            obj.SetActive(true);
        }
        baYeGoldNumText.text = PlayerDataForGame.instance.baYeGoldNums.ToString();
    }

    //标记选择霸业势力的ChooseImg
    GameObject baYeFoeceChooseImgObj;
    //标记选择霸业城池id
    int baYeEventChooseIndex; 

    //选择霸业的势力方法
    private void ChooseBaYeForceOnClick(int forceId, GameObject chooseImgObj)
    {
        if (baYeFoeceChooseImgObj != null)
        {
            baYeFoeceChooseImgObj.SetActive(false);
        }
        baYeFoeceChooseImgObj = chooseImgObj;
        baYeFoeceChooseImgObj.SetActive(true);
        SeleceBaYeForceCards(forceId);
    }

    //刷新霸业的势力武将选择（派去战斗）
    private void SeleceBaYeForceCards(int forceId)
    {
        PlayerDataForGame.instance.fightHeroId.Clear();
        PlayerDataForGame.instance.fightTowerId.Clear();
        PlayerDataForGame.instance.fightTrapId.Clear();

        NowLevelAndHadChip heroDataIndex = new NowLevelAndHadChip();    //临时记录武将存档信息
        for (int i = 0; i < PlayerDataForGame.instance.hstData.heroSaveData.Count; i++)
        {
            heroDataIndex = PlayerDataForGame.instance.hstData.heroSaveData[i];
            if (forceId == int.Parse(LoadJsonFile.heroTableDatas[heroDataIndex.id][6]))
            {
                if (heroDataIndex.level > 0 || heroDataIndex.chips > 0)
                {
                    if (heroDataIndex.isFight > 0)
                    {
                        PlayerDataForGame.instance.fightHeroId.Add(heroDataIndex.id);
                    }
                }
            }
        }
        NowLevelAndHadChip fuzhuDataIndex = new NowLevelAndHadChip();
        for (int i = 0; i < PlayerDataForGame.instance.hstData.towerSaveData.Count; i++)
        {
            fuzhuDataIndex = PlayerDataForGame.instance.hstData.towerSaveData[i];
            if (forceId == int.Parse(LoadJsonFile.towerTableDatas[fuzhuDataIndex.id][15]))
            {
                if (fuzhuDataIndex.level > 0 || fuzhuDataIndex.chips > 0)
                {
                    if (fuzhuDataIndex.isFight > 0)
                    {
                        PlayerDataForGame.instance.fightTowerId.Add(fuzhuDataIndex.id);
                    }
                }
            }
        }
        for (int i = 0; i < PlayerDataForGame.instance.hstData.trapSaveData.Count; i++)
        {
            fuzhuDataIndex = PlayerDataForGame.instance.hstData.trapSaveData[i];
            if (forceId == int.Parse(LoadJsonFile.trapTableDatas[fuzhuDataIndex.id][14]))
            {
                if (fuzhuDataIndex.level > 0 || fuzhuDataIndex.chips > 0)
                {
                    if (fuzhuDataIndex.isFight > 0)
                    {
                        PlayerDataForGame.instance.fightTrapId.Add(fuzhuDataIndex.id);
                    }
                }
            }
        }
    }

    //选择某个霸业城池点的方法
    private void ChooseBaYeEventOnClick(int eventIndex, int warId)
    {
        PlayerDataForGame.instance.chooseWarsId = warId;
        chooseBaYeEventImg.transform.position = baYeEventsObj.transform.GetChild(eventIndex).transform.position;
        chooseBaYeEventImg.SetActive(true);
        baYeEventChooseIndex = warId;
        print(warId);
    }

    //获取霸业城池的战役id
    private int GetChooseEventsWarId(int eventIndex)
    {
        string[] eventsStr = LoadJsonFile.baYeDiTuTableDatas[eventIndex][2].Split(',');
        List<int> shiJianIdList = new List<int>();
        for (int i = 0; i < eventsStr.Length; i++)
        {
            if (eventsStr[i] != "")
            {
                shiJianIdList.Add(int.Parse(eventsStr[i]));
            }
        }
        //得到随机的霸业事件id
        int baYeBattleId = int.Parse(LoadJsonFile.baYeShiJianTableDatas[BackShiJianIdByWeightValue(shiJianIdList)][3]);
        return int.Parse(LoadJsonFile.baYeBattleTableDatas[baYeBattleId][int.Parse(LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.level - 1][9]) + 1]);
    }

    //根据权重得到随机id
    private int BackShiJianIdByWeightValue(List<int> datas)
    {
        int weightValueSum = 0;
        for (int i = 0; i < datas.Count; i++)
        {
            weightValueSum += int.Parse(LoadJsonFile.baYeShiJianTableDatas[datas[i]][1]);
        }
        int randNum = UnityEngine.Random.Range(0, weightValueSum);
        int indexTest = 0;
        while (randNum >= 0)
        {
            randNum -= int.Parse(LoadJsonFile.baYeShiJianTableDatas[datas[indexTest]][1]);
            indexTest++;
        }
        indexTest -= 1;
        return datas[indexTest];
    }

    //main场景羁绊内容的初始化
    private void InitJiBanForMainFun()
    {
        for (int i = 0; i < LoadJsonFile.jiBanTableDatas.Count; i++)
        {
            if (LoadJsonFile.jiBanTableDatas[i][2] == "1")
            {
                Transform tran = jibanBtnBoxTran.GetChild(i);
                if (tran != null)
                {
                    int index = i;
                    tran.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load("Image/JiBan/name_v/" + i, typeof(Sprite)) as Sprite;
                    tran.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate() {
                        ShowJiBanInfoOnClick(index);
                    });
                    tran.gameObject.SetActive(true);
                }
            }
        }
        jiBanWinCloseBtn.onClick.AddListener(CloseHuiJuanWinObjFun);
    }

    //点击单个羁绊按钮展示详细信息
    private void ShowJiBanInfoOnClick(int indexId)
    {
        for (int i = 0; i < jibanHeroBoxTran.childCount; i++)
        {
            jibanHeroBoxTran.transform.GetChild(i).gameObject.SetActive(false);
        }

        string[] arrs = LoadJsonFile.jiBanTableDatas[indexId][3].Split(';');
        for (int i = 0; i < arrs.Length; i++)
        {
            if (arrs[i] != "")
            {
                string[] arr = arrs[i].Split(',');
                if (arr[0] == "0")
                {
                    int heroId = int.Parse(arr[1]);
                    Transform tran = jibanHeroBoxTran.GetChild(i);
                    GameObject obj = tran.GetChild(0).gameObject;
                    //名字
                    ShowNameTextRules(obj.transform.GetChild(2).GetComponent<Text>(), LoadJsonFile.heroTableDatas[heroId][1]);
                    //名字颜色根据稀有度
                    obj.transform.GetChild(2).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.heroTableDatas[heroId][3]);
                    //卡牌
                    obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/Hero/" + LoadJsonFile.heroTableDatas[heroId][16], typeof(Sprite)) as Sprite;
                    //兵种名
                    obj.transform.GetChild(4).GetComponentInChildren<Text>().text = LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[heroId][5])][3];
                    //兵种框
                    obj.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 0, typeof(Sprite)) as Sprite;
                    tran.gameObject.SetActive(true);
                }
            }
        }
        jiBanInfoConObj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load("Image/JiBan/art/" + indexId, typeof(Sprite)) as Sprite;
        jiBanInfoConObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = LoadJsonFile.jiBanTableDatas[indexId][4];
        jiBanInfoConObj.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = Resources.Load("Image/JiBan/name_h/" + indexId, typeof(Sprite)) as Sprite;


        jiBanBtnsConObj.SetActive(false);
        jiBanInfoConObj.SetActive(true);
        jiBanWinCloseBtn.onClick.RemoveAllListeners();
        jiBanWinCloseBtn.onClick.AddListener(delegate() {
            jiBanInfoConObj.SetActive(false);
            jiBanBtnsConObj.SetActive(true);
            jiBanWinCloseBtn.onClick.RemoveAllListeners();
            jiBanWinCloseBtn.onClick.AddListener(CloseHuiJuanWinObjFun);
        });
    }

    /// <summary>
    /// 打开绘卷界面
    /// </summary>
    public void OpenHuiJuanWinObjFun()
    {
        jiBanBtnsConObj.SetActive(true);
        huiJuanWinObj.SetActive(true);
    }

    /// <summary>
    /// 关闭绘卷界面
    /// </summary>
    private void CloseHuiJuanWinObjFun()
    {
        huiJuanWinObj.SetActive(false);
        jiBanBtnsConObj.SetActive(false);
        jiBanInfoConObj.SetActive(false);
    }

    //获取战役返还的体力
    private void GetBackTiLiForFight()
    {
        if (PlayerDataForGame.instance.lastSenceIndex == 2 && PlayerDataForGame.instance.getBackTiLiNums > 0)
        {
            cutTiLiTextObj.SetActive(false);
            cutTiLiTextObj.GetComponent<Text>().color = ColorDataStatic.deep_green;
            cutTiLiTextObj.GetComponent<Text>().text = "+" + PlayerDataForGame.instance.getBackTiLiNums;
            cutTiLiTextObj.SetActive(true);
            AddTiLiNums(PlayerDataForGame.instance.getBackTiLiNums);
            PlayerDataForGame.instance.ShowStringTips(string.Format(LoadJsonFile.GetStringText(25), PlayerDataForGame.instance.getBackTiLiNums));
        }
        PlayerDataForGame.instance.lastSenceIndex = 1;
        PlayerDataForGame.instance.getBackTiLiNums = 0;
    }

    //初始化战役难度
    private void InitWarDifBtnShow()
    {
        int indexLastDifCanChoose = 0;
        //新手-困难关卡入口初始化
        for (int i = 0; i < 5; i++)
        {
            int index = i;
            chonseWarDifTran.GetChild(i).gameObject.SetActive(true);
            chonseWarDifTran.GetChild(i).GetComponentInChildren<Text>().text = LoadJsonFile.choseWarTableDatas[i][1];
            int unlockWarId = int.Parse(LoadJsonFile.choseWarTableDatas[i][3]);
            if (unlockWarId == 0 || PlayerDataForGame.instance.warsData.warUnlockSaveData[unlockWarId].unLockCount >= int.Parse(LoadJsonFile.warTableDatas[unlockWarId][4]))
            {
                indexLastDifCanChoose = index;
                chonseWarDifTran.GetChild(i).GetComponentInChildren<Text>().color = Color.white;
                chonseWarDifTran.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    InitWarsListInfo(index);
                    PlayOnClickMusic();
                });
            }
            else
            {
                chonseWarDifTran.GetChild(i).GetComponentInChildren<Text>().color = Color.gray;
                chonseWarDifTran.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.choseWarTableDatas[index][5]);
                    PlayOnClickMusic();
                });
                break;
            }
        }
        InitWarsListInfo(indexLastDifCanChoose);

        //远征关卡
        int unlockWarId_Yz = int.Parse(LoadJsonFile.choseWarTableDatas[6][3]);
        if (unlockWarId_Yz == 0 || PlayerDataForGame.instance.warsData.warUnlockSaveData[unlockWarId_Yz].unLockCount >= int.Parse(LoadJsonFile.warTableDatas[unlockWarId_Yz][4]))
        {
            chonseWarDifTran.GetChild(6).gameObject.SetActive(true);
            chonseWarDifTran.GetChild(6).GetComponentInChildren<Text>().text = LoadJsonFile.choseWarTableDatas[6][1];
            chonseWarDifTran.GetChild(6).GetComponentInChildren<Text>().color = Color.white;
            chonseWarDifTran.GetChild(6).GetComponent<Button>().onClick.AddListener(delegate ()
            {
                InitWarsListInfo(6);
                PlayOnClickMusic();
            });
        }

        ////炼狱关卡
        //int unlockWarId_Ly = int.Parse(LoadJsonFile.choseWarTableDatas[5][3]);
        //if (unlockWarId_Ly == 0 || PlayerDataForGame.instance.warsData.warUnlockSaveData[unlockWarId_Ly].unLockCount >= int.Parse(LoadJsonFile.warTableDatas[unlockWarId_Ly][4]))
        //{
        //    chonseWarDifTran.GetChild(5).gameObject.SetActive(true);
        //    //chonseWarDifTran.GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.choseWarTableDatas[5][1];
        //    chonseWarDifTran.GetChild(5).GetComponentInChildren<Text>().color = Color.white;
        //    chonseWarDifTran.GetChild(5).GetComponent<Button>().onClick.AddListener(delegate ()
        //    {
        //        InitWarsListInfoOfLy(5);
        //        PlayOnClickMusic();
        //    });
        //}
    }

    /// <summary>
    /// 初始化炼狱关卡列表
    /// </summary>
    private void InitWarsListInfoOfLy(int indexBtn)
    {
        //防止跳转界面时切换关卡
        if (isJumping)
            return;

        //右侧难度选择按钮刷新大小
        for (int i = 0; i < chonseWarDifTran.childCount; i++)
        {
            if (indexBtn == i)
            {
                chonseWarDifTran.GetChild(i).transform.localScale = new Vector3(1.2f, 1.2f);
            }
            else
            {
                chonseWarDifTran.GetChild(i).transform.localScale = new Vector3(1f, 1f);
            }
        }

        tiLiCostIndex = indexBtn;

        //删除战役列表
        for (int i = 0; i < warsChooseListObj.transform.childCount; i++)
        {
            Destroy(warsChooseListObj.transform.GetChild(i).gameObject);
        }

        int firstChooseWarId = -1;
        GameObject lastObj = new GameObject();
        PlayerDataForGame.garbageStationObjs.Add(lastObj);

        //展示炼狱战役列表
        for (int i = 5; i < LoadJsonFile.choseWarTableDatas.Count; i++)
        {
            int unlockWarId_Ly = int.Parse(LoadJsonFile.choseWarTableDatas[i][3]);
            if (PlayerDataForGame.instance.warsData.warUnlockSaveData[unlockWarId_Ly].unLockCount >= int.Parse(LoadJsonFile.warTableDatas[unlockWarId_Ly][4]))
            {
                //具体炼狱关的始末关卡
                int startId, endId;
                string[] arr = LoadJsonFile.choseWarTableDatas[i][2].Split(',');
                if (arr.Length != 2)
                {
                    continue;
                }
                startId = int.Parse(arr[0]);
                endId = int.Parse(arr[1]);

                int index = startId;
                for (; index <= endId; index++)
                {
                    int specificId = index;
                    //没有领过首通奖励
                    if (!PlayerDataForGame.instance.warsData.warUnlockSaveData[specificId].isTakeReward)
                    {
                        //战役的起始关卡id
                        int warIdIndex = PlayerDataForGame.instance.warsData.warUnlockSaveData[specificId].warId;
                        //战役总关卡数
                        int warTotalNums = int.Parse(LoadJsonFile.warTableDatas[warIdIndex][4]);
                        //创建具体战役选择obj
                        GameObject obj = Instantiate(warsChooseBtnPreObj, warsChooseListObj.transform);
                        Transform box = obj.transform.GetChild(2);

                        obj.GetComponentInChildren<Text>().text = LoadJsonFile.warTableDatas[warIdIndex][1] + "\u2000\u2000\u2000\u2000" + Mathf.Min(PlayerDataForGame.instance.warsData.warUnlockSaveData[specificId].unLockCount, warTotalNums) + "/" + warTotalNums;

                        if (PlayerDataForGame.instance.warsData.warUnlockSaveData[specificId].unLockCount < warTotalNums)
                        {
                            if (firstChooseWarId==-1)
                            {
                                firstChooseWarId = specificId;
                                lastObj = obj;
                            }
                            obj.GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                OnClickChangeWarsFun(warIdIndex, obj);
                                PlayOnClickMusic();
                            });
                            break;
                        }
                        else
                        {
                            //首通宝箱可领取
                            box.GetChild(1).gameObject.SetActive(false);
                            box.GetChild(0).gameObject.SetActive(true);
                            box.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                GetWarFirstRewards(specificId);
                                box.gameObject.SetActive(false);
                            });
                        }
                    }
                }
            }
            else
            {
                continue;
            }
        }
        OnClickChangeWarsFun(firstChooseWarId, lastObj);
        warsChooseListObj.transform.parent.parent.GetComponent<ScrollRect>().DOVerticalNormalizedPos(0f, 0.3f);
    }

    /// <summary>
    /// 领取战役首通宝箱
    /// </summary>
    /// <param name="jb"></param>
    private void GetWarFirstRewards(int jb)
    {
        int warId = PlayerDataForGame.instance.warsData.warUnlockSaveData[jb].warId;

        int yuanBaoNums = int.Parse(LoadJsonFile.warTableDatas[warId][8]);
        int yuQueNums = int.Parse(LoadJsonFile.warTableDatas[warId][9]);
        int tiLiNums = int.Parse(LoadJsonFile.warTableDatas[warId][10]);
        if (yuanBaoNums > 0)
        {
            ConsumeManager.instance.AddYuanBao(yuanBaoNums);
        }
        if (yuQueNums > 0)
        {
            ConsumeManager.instance.AddYuQue(yuQueNums);
        }
        if (tiLiNums > 0)
        {
            AddTiLiNums(tiLiNums);
        }
        
        string rewardsStr = LoadJsonFile.warTableDatas[warId][PlayerDataForGame.instance.pyData.forceId + 5];

        List<RewardsCardClass> rewards = new List<RewardsCardClass>();

        if (rewardsStr != "")
        {
            string[] arrs = rewardsStr.Split(',');
            int cardType = int.Parse(arrs[0]);
            int cardId = int.Parse(arrs[1]);
            int cardChips = int.Parse(arrs[2]);

            switch (cardType)
            {
                case 0:
                    PlayerDataForGame.instance.hstData.heroSaveData[FindIndexFromData(PlayerDataForGame.instance.hstData.heroSaveData, cardId)].chips += cardChips;
                    break;
                case 2:
                    PlayerDataForGame.instance.hstData.towerSaveData[FindIndexFromData(PlayerDataForGame.instance.hstData.towerSaveData, cardId)].chips += cardChips;
                    break;
                case 3:
                    PlayerDataForGame.instance.hstData.trapSaveData[FindIndexFromData(PlayerDataForGame.instance.hstData.trapSaveData, cardId)].chips += cardChips;
                    break;
                default:
                    break;
            }
            RewardsCardClass rewardCard = new RewardsCardClass();
            rewardCard.cardType = cardType;
            rewardCard.cardId = cardId;
            rewardCard.cardChips = cardChips;
            rewards.Add(rewardCard);
            PlayerDataForGame.instance.isNeedSaveData = true;
            LoadSaveData.instance.SaveGameData(2);
        }

        PlayerDataForGame.instance.warsData.warUnlockSaveData[jb].isTakeReward = true;
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(3);

        ShowRewardsThings(yuanBaoNums, yuQueNums, 0, tiLiNums, rewards, 0);
    }

    /// <summary>
    /// 选择难度按钮以改变战役列表
    /// </summary>
    private void InitWarsListInfo(int indexBtn)
    {
        //防止跳转界面时切换关卡
        if (isJumping)
            return;

        //右侧难度选择按钮刷新大小
        for (int i = 0; i < chonseWarDifTran.childCount; i++)
        {
            if (indexBtn == i)
            {
                chonseWarDifTran.GetChild(i).transform.localScale = new Vector3(1.2f, 1.2f);
            }
            else
            {
                chonseWarDifTran.GetChild(i).transform.localScale = new Vector3(1f, 1f);
            }
        }

        tiLiCostIndex = indexBtn;

        //删除战役列表
        for (int i = 0; i < warsChooseListObj.transform.childCount; i++)
        {
            Destroy(warsChooseListObj.transform.GetChild(i).gameObject);
        }

        int startId, endId;

        switch (indexBtn)
        {
            //远征特殊
            case 6:
                startId = endId = int.Parse(LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.level - 1][5]);
                break;
            //炼狱特殊
            //case 5:

            //    break;
            default:
                string[] arr = LoadJsonFile.choseWarTableDatas[indexBtn][2].Split(',');
                if (arr.Length != 2)
                {
                    return;
                }
                startId = int.Parse(arr[0]);
                endId = int.Parse(arr[1]);
                break;
        }

        int index = startId;
        GameObject lastObj = new GameObject();
        PlayerDataForGame.garbageStationObjs.Add(lastObj);

        for (; index <= endId; index++)
        {
            int jb = index; //delegate特需临时变量
            int warIdIndex = PlayerDataForGame.instance.warsData.warUnlockSaveData[jb].warId;
            GameObject obj = Instantiate(warsChooseBtnPreObj, warsChooseListObj.transform);
            Transform box = obj.transform.GetChild(2);
            int warTotalNums = int.Parse(LoadJsonFile.warTableDatas[warIdIndex][4]);
            if (PlayerDataForGame.instance.warsData.warUnlockSaveData[jb].isTakeReward)
            {
                box.gameObject.SetActive(false);
            }
            else
            {
                if (PlayerDataForGame.instance.warsData.warUnlockSaveData[jb].unLockCount >= warTotalNums)
                {
                    box.GetChild(1).gameObject.SetActive(false);
                    box.GetChild(0).gameObject.SetActive(true);
                    box.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        GetWarFirstRewards(jb);
                        box.gameObject.SetActive(false);
                    });
                }
                else
                {
                    box.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(26));
                    });
                }
            }
            //战役列拼接
            obj.GetComponentInChildren<Text>().text = LoadJsonFile.warTableDatas[warIdIndex][1] + "\u2000\u2000\u2000\u2000"
                + Mathf.Min(PlayerDataForGame.instance.warsData.warUnlockSaveData[jb].unLockCount, warTotalNums) + "/" + warTotalNums;
            obj.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                OnClickChangeWarsFun(warIdIndex, obj);

                PlayOnClickMusic();

            });
            lastObj = obj;
            if (PlayerDataForGame.instance.warsData.warUnlockSaveData[jb].unLockCount >=
                int.Parse(LoadJsonFile.warTableDatas[warIdIndex][4]))
            { }
            else
            {
                break;
            }
        }
        //默认选择最后一个关卡
        OnClickChangeWarsFun(PlayerDataForGame.instance.warsData.warUnlockSaveData[index > endId ? endId : index].warId, lastObj);
        warsChooseListObj.transform.parent.parent.GetComponent<ScrollRect>().DOVerticalNormalizedPos(0f, 0.3f);
    }

    //选择战役的改变
    private void OnClickChangeWarsFun(int warsId, GameObject obj)
    {
        for (int i = 0; i < warsChooseListObj.transform.childCount; i++)
        {
            warsChooseListObj.transform.GetChild(i).GetComponentInChildren<Image>().enabled = false;
        }
        //选中状态显示
        obj.GetComponentInChildren<Image>().enabled = true;

        PlayerDataForGame.instance.zhanYiColdNums = 10;
        PlayerDataForGame.instance.chooseWarsId = warsId;
        //战役介绍
        warIntroText.DOPause();
        warIntroText.text = "";
        warIntroText.color = new Color(warIntroText.color.r, warIntroText.color.g, warIntroText.color.b, 0);
        warIntroText.DOFade(1, 3f);
        warIntroText.DOText(("\u2000\u2000\u2000\u2000" + LoadJsonFile.warTableDatas[warsId][2]), 3f).SetEase(Ease.Linear).SetAutoKill(false);
    }

    int showTiLiNums = 0;

    //刷新体力相关的内容显示
    public void UpdateShowTiLiInfo(string recordStr)
    {
        if (recordStr != tiLiRecordTimer.text)
        {
            tiLiRecordTimer.text = recordStr;
        }
        int nowStaminaNums = PlayerPrefs.GetInt(TimeSystemControl.staminaStr);
        if (showTiLiNums != nowStaminaNums)
        {
            showTiLiNums = nowStaminaNums;
            tiLiNumText.text = nowStaminaNums + "/90";
        }
    }

    /// <summary>
    /// 开始对战
    /// </summary>
    public void OnClickStartWars()
    {
        if (!isJumping)
        {
            string[] tiLiCostArr = LoadJsonFile.choseWarTableDatas[tiLiCostIndex][4].Split(',');
            int cutStaminaNums = int.Parse(tiLiCostArr[0]);
            if (PlayerPrefs.GetInt(TimeSystemControl.staminaStr) >= cutStaminaNums)
            {
                ShowOrHideGuideObj(3, false);
                isJumping = true;
                AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[12], AudioController0.instance.audioVolumes[12]);
                AudioController0.instance.PlayAudioSource(0);
                TimeSystemControl.instance.LetTiLiTimerTake(cutStaminaNums);
                PlayerPrefs.SetInt(TimeSystemControl.staminaStr, (PlayerPrefs.GetInt(TimeSystemControl.staminaStr) - cutStaminaNums));
                showTiLiNums = PlayerPrefs.GetInt(TimeSystemControl.staminaStr);
                tiLiNumText.text = showTiLiNums + "/90";
                cutTiLiTextObj.SetActive(false);
                cutTiLiTextObj.GetComponent<Text>().color = ColorDataStatic.name_red;
                cutTiLiTextObj.GetComponent<Text>().text = "-"+ cutStaminaNums;
                cutTiLiTextObj.SetActive(true);

                PlayerDataForGame.instance.getBackTiLiNums = int.Parse(tiLiCostArr[1]);
                PlayerDataForGame.instance.boxForTiLiNums = int.Parse(tiLiCostArr[2]);

                StartCoroutine(LateGoToFightScene());
            }
            else
            {
                PlayOnClickMusic();
                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(27));
                //Debug.Log("体力不足，无法战斗");
            }
        }
        else
        {
            PlayOnClickMusic();
        }
    }

    IEnumerator LateGoToFightScene()
    {
        yield return new WaitForSeconds(1f);
        if (!PlayerDataForGame.instance.isJumping)
        {
            PlayerDataForGame.instance.JumpSceneFun(2, false);
        }
    }

    //刷新上阵数量的显示
    private void UpdateCardNumsShow()
    {
        cardsListTitle.text = "出战";
        cardsNumsTitle.text = PlayerDataForGame.instance.CalculationFightCount() + "/" + LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.level - 1][2];
    }

    //是否展示卡牌详情显示
    private void ShowOrHideInfo(bool isShow)
    {
        showCardObj.SetActive(isShow);
        infoTran.gameObject.SetActive(isShow);
        heChengBtn.SetActive(isShow);
        holdOrFightBtn.SetActive(isShow);
        sellCardBtn.SetActive(isShow);
    }

    public int indexChooseListForceId = 0; //标记主城展示哪个势力的id

    /// <summary>
    /// 改变武将列表和辅助列表显示
    /// </summary>
    public void ChangeScrollView()
    {
        AudioController0.instance.RandomPlayGuZhengAudio();

        indexChooseListForceId++;
        if (indexChooseListForceId > int.Parse(LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.level - 1][6]))
        {
            indexChooseListForceId = 0;
        }
        changeCardsListBtn.sprite = Resources.Load("Image/shiLi/Flag/" + indexChooseListForceId, typeof(Sprite)) as Sprite;
        changeCardsListNameImg.sprite = Resources.Load("Image/shiLi/Name/" + indexChooseListForceId, typeof(Sprite)) as Sprite;
        CreateHeroAndTowerContent();
        UpdateCardNumsShow();
        StartCoroutine(LiteToChangeViewShow(0));
    }

    /// <summary>
    /// 延时刷新列表置顶
    /// </summary>
    IEnumerator LiteToChangeViewShow(float startTime)
    {
        yield return new WaitForSeconds(startTime);

        //Debug.Log("----列表大小控制");

        int showCardCount = 0;
        for (int i = 0; i < zhuChengHeroContentObj.transform.childCount; i++)
        {
            if (zhuChengHeroContentObj.transform.GetChild(i).gameObject.activeSelf)
                showCardCount++;
        }

        //列表大小控制
        if (showCardCount >= 16)
        {
            zhuChengHeroContentObj.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;
        }
        else
        {
            zhuChengHeroContentObj.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            //zhuChengHeroContentObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            //zhuChengHeroContentObj.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            zhuChengHeroContentObj.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            zhuChengHeroContentObj.GetComponent<RectTransform>().offsetMax = new Vector2(1, 1);
        }
        zhuChengHeroContentObj.transform.parent.parent.GetComponent<ScrollRect>().DOVerticalNormalizedPos(1f, 0.2f);
        //zhuChengHeroContentObj.transform.parent.parent.GetComponent<ScrollRect>().content.localPosition = Vector2.left;
    }

    /// <summary>
    /// 显示单个辅助
    /// </summary>
    private void ShowOneFuZhuRules(List<List<string>> jsonDatas, NowLevelAndHadChip fuzhuData, int indexIcon)
    {
        GameObject obj = GetHeroCardToShow();
        //名字
        ShowNameTextRules(obj.transform.GetChild(3).GetComponent<Text>(), jsonDatas[fuzhuData.id][1]);
        //名字颜色根据稀有度
        obj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(jsonDatas[fuzhuData.id][3]);
        //卡牌
        obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + jsonDatas[fuzhuData.id][indexIcon], typeof(Sprite)) as Sprite;
        //兵种框
        obj.transform.GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
        //兵种名
        obj.transform.GetChild(5).GetComponentInChildren<Text>().text = jsonDatas[fuzhuData.id][5];
        //边框
        FrameChoose(jsonDatas[fuzhuData.id][3], obj.transform.GetChild(6).GetComponent<Image>());
        //碎片
        if (fuzhuData.level < LoadJsonFile.upGradeTableDatas.Count)
        {
            obj.transform.GetChild(2).GetComponent<Text>().text = fuzhuData.chips + "/" + LoadJsonFile.upGradeTableDatas[fuzhuData.level][1];
            obj.transform.GetChild(2).GetComponent<Text>().color = fuzhuData.chips >= int.Parse(LoadJsonFile.upGradeTableDatas[fuzhuData.level][1]) ? ColorDataStatic.deep_green : Color.white;

        }
        else
        {
            obj.transform.GetChild(2).GetComponent<Text>().text = "";
        }
        if (fuzhuData.level > 0)
        {
            obj.transform.GetChild(4).GetComponent<Image>().enabled = true;
            //设置星级展示
            obj.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + fuzhuData.level, typeof(Sprite)) as Sprite;
            obj.transform.GetChild(8).gameObject.SetActive(false);
            //出战标记
            if (fuzhuData.isFight > 0)
            {
                PlayerDataForGame.instance.AddOrCutFightCardId(fuzhuData.typeIndex, fuzhuData.id, true);
                obj.transform.GetChild(7).gameObject.SetActive(true);
            }
            else
            {
                obj.transform.GetChild(7).gameObject.SetActive(false);
            }
        }
        else
        {
            obj.transform.GetChild(4).GetComponent<Image>().enabled = false;
            obj.transform.GetChild(7).gameObject.SetActive(false);
            obj.transform.GetChild(8).gameObject.SetActive(true);
        }
        obj.GetComponent<Button>().onClick.RemoveAllListeners();
        obj.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            OnClickFuZhuCardFun(jsonDatas, fuzhuData, obj.transform.GetChild(9).GetComponent<Image>(), indexIcon);
        });
    }

    /// <summary>
    /// 点击辅助卡牌的方法
    /// </summary>
    /// <param name="fuzhuData"></param>
    private void OnClickFuZhuCardFun(List<List<string>> jsonDatas, NowLevelAndHadChip fuzhuData, Image selectImg, int indexIcon)
    {
        PlayOnClickMusic();

        //名字
        infoTran.GetChild(0).GetComponent<Text>().text = jsonDatas[fuzhuData.id][1];
        //名字颜色
        infoTran.GetChild(0).GetComponent<Text>().color = NameColorChoose(jsonDatas[fuzhuData.id][3]);
        //属性 为空
        infoTran.GetChild(1).GetComponent<Text>().text = "";
        infoTran.GetChild(2).GetComponent<Text>().text = "";
        //介绍
        infoTran.GetChild(3).GetComponent<Text>().text = jsonDatas[fuzhuData.id][2];
        //名字
        ShowNameTextRules(showCardObj.transform.GetChild(3).GetComponent<Text>(), jsonDatas[fuzhuData.id][1]);
        //名字颜色
        showCardObj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(jsonDatas[fuzhuData.id][3]);
        //卡牌
        showCardObj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + jsonDatas[fuzhuData.id][indexIcon], typeof(Sprite)) as Sprite;
        //兵种框
        showCardObj.transform.GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
        //兵种名
        showCardObj.transform.GetChild(5).GetComponentInChildren<Text>().text = jsonDatas[fuzhuData.id][5];
        //边框
        FrameChoose(jsonDatas[fuzhuData.id][3], showCardObj.transform.GetChild(6).GetComponent<Image>());
        //碎片
        if (fuzhuData.level < LoadJsonFile.upGradeTableDatas.Count)
        {
            showCardObj.transform.GetChild(2).GetComponent<Text>().text = fuzhuData.chips + "/" + LoadJsonFile.upGradeTableDatas[fuzhuData.level][1];
            showCardObj.transform.GetChild(2).GetComponent<Text>().color = fuzhuData.chips >= int.Parse(LoadJsonFile.upGradeTableDatas[fuzhuData.level][1]) ? ColorDataStatic.deep_green : Color.black;
        }
        else
        {
            showCardObj.transform.GetChild(2).GetComponent<Text>().text = "";
        }

        int getGoldNums = GetGoldNumsForSellCard(fuzhuData);
        sellCardBtn.transform.GetChild(0).GetComponent<Text>().text = getGoldNums.ToString();
        sellCardBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        sellCardBtn.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            OnClickForSellCard(fuzhuData, getGoldNums);
        });
        sellCardBtn.SetActive(true);

        if (fuzhuData.level > 0)
        {
            showCardObj.transform.GetChild(4).GetComponent<Image>().enabled = true;
            //设置星级展示
            showCardObj.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + fuzhuData.level, typeof(Sprite)) as Sprite;
            //出战相关设置
            holdOrFightBtn.SetActive(true);
            if (fuzhuData.isFight > 0)
            {
                showCardObj.transform.GetChild(7).gameObject.SetActive(true);
                holdOrFightBtn.GetComponentInChildren<Text>().text = LoadJsonFile.GetStringText(30);
            }
            else
            {
                showCardObj.transform.GetChild(7).gameObject.SetActive(false);
                holdOrFightBtn.GetComponentInChildren<Text>().text = LoadJsonFile.GetStringText(31);
            }
        }
        else
        {
            holdOrFightBtn.SetActive(false);
            showCardObj.transform.GetChild(7).gameObject.SetActive(false);
            showCardObj.transform.GetChild(4).GetComponent<Image>().enabled = false;
        }
        //选择框处理
        if (lastSelectImg != null)
        {
            lastSelectImg.enabled = false;
        }
        lastSelectImg = selectImg;
        lastSelectImg.enabled = true;

        selectCardData = fuzhuData;

        CalculatedNeedYuanBao(fuzhuData.level);
    }

    /// <summary>
    /// 创建并展示单位列表
    /// </summary>
    private void CreateHeroAndTowerContent()
    {
        TakeBackHeroCardPooling();

        PlayerDataForGame.instance.fightHeroId.Clear();
        PlayerDataForGame.instance.fightTowerId.Clear();
        PlayerDataForGame.instance.fightTrapId.Clear();

        int cardNums = 0;

        SortHSTData(PlayerDataForGame.instance.hstData.heroSaveData);   //  排序

        NowLevelAndHadChip heroDataIndex = new NowLevelAndHadChip();    //临时记录武将存档信息
        for (int i = 0; i < PlayerDataForGame.instance.hstData.heroSaveData.Count; i++)
        {
            heroDataIndex = PlayerDataForGame.instance.hstData.heroSaveData[i];
            if (indexChooseListForceId==int.Parse(LoadJsonFile.heroTableDatas[heroDataIndex.id][6]))
            {
                if (heroDataIndex.level > 0 || heroDataIndex.chips > 0)
                {
                    if (heroDataIndex.isFight>0)
                    {
                        PlayerDataForGame.instance.fightHeroId.Add(heroDataIndex.id);
                    }
                    cardNums++;
                    ShowOneHeroRules(heroDataIndex);
                }
            }
        }

        NowLevelAndHadChip fuzhuDataIndex = new NowLevelAndHadChip();
        //排序
        //SortHSTData(PlayerDataForGame.instance.hstData.soldierSaveData);
        //for (int i = 0; i < PlayerDataForGame.instance.hstData.soldierSaveData.Count; i++)
        //{
        //    fuzhuDataIndex = PlayerDataForGame.instance.hstData.soldierSaveData[i];
        //    if (fuzhuDataIndex.level > 0 || fuzhuDataIndex.chips > 0)
        //    {
        //        cardNums++;
        //        ShowOneFuZhuRules(LoadJsonFile.soldierTableDatas, fuzhuDataIndex, 13);
        //    }
        //}
        SortHSTData(PlayerDataForGame.instance.hstData.towerSaveData);
        for (int i = 0; i < PlayerDataForGame.instance.hstData.towerSaveData.Count; i++)
        {
            fuzhuDataIndex = PlayerDataForGame.instance.hstData.towerSaveData[i];
            if (indexChooseListForceId == int.Parse(LoadJsonFile.towerTableDatas[fuzhuDataIndex.id][15]))
            {
                if (fuzhuDataIndex.level > 0 || fuzhuDataIndex.chips > 0)
                {
                    if (fuzhuDataIndex.isFight > 0)
                    {
                        PlayerDataForGame.instance.fightTowerId.Add(fuzhuDataIndex.id);
                    }
                    cardNums++;
                    ShowOneFuZhuRules(LoadJsonFile.towerTableDatas, fuzhuDataIndex, 10);
                }
            }
        }
        SortHSTData(PlayerDataForGame.instance.hstData.trapSaveData);
        for (int i = 0; i < PlayerDataForGame.instance.hstData.trapSaveData.Count; i++)
        {
            fuzhuDataIndex = PlayerDataForGame.instance.hstData.trapSaveData[i];
            if (indexChooseListForceId == int.Parse(LoadJsonFile.trapTableDatas[fuzhuDataIndex.id][14]))
            {
                if (fuzhuDataIndex.level > 0 || fuzhuDataIndex.chips > 0)
                {
                    if (fuzhuDataIndex.isFight > 0)
                    {
                        PlayerDataForGame.instance.fightTrapId.Add(fuzhuDataIndex.id);
                    }
                    cardNums++;
                    ShowOneFuZhuRules(LoadJsonFile.trapTableDatas, fuzhuDataIndex, 8);
                }
            }
        }
        //SortHSTData(PlayerDataForGame.instance.hstData.spellSaveData);
        //for (int i = 0; i < PlayerDataForGame.instance.hstData.spellSaveData.Count; i++)
        //{
        //    fuzhuDataIndex = PlayerDataForGame.instance.hstData.spellSaveData[i];
        //    if (fuzhuDataIndex.level > 0 || fuzhuDataIndex.chips > 0)
        //    {
        //        cardNums++;
        //        ShowOneFuZhuRules(LoadJsonFile.spellTableDatas, fuzhuDataIndex, 6);
        //    }
        //}
        if (cardNums > 0)
        {
            StartCoroutine(LiteUpdateListChooseFirst(0));
            ShowOrHideInfo(true);
        }
        else
        {
            ShowOrHideInfo(false);
        }
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

    /// <summary>
    /// 显示单个武将
    /// </summary>
    /// <param name="heroData"></param>
    private void ShowOneHeroRules(NowLevelAndHadChip heroData)
    {
        GameObject obj = GetHeroCardToShow();
        //名字
        ShowNameTextRules(obj.transform.GetChild(3).GetComponent<Text>(), LoadJsonFile.heroTableDatas[heroData.id][1]);
        //名字颜色根据稀有度
        obj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.heroTableDatas[heroData.id][3]);
        //卡牌
        obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/Hero/" + LoadJsonFile.heroTableDatas[heroData.id][16], typeof(Sprite)) as Sprite;
        //兵种名
        obj.transform.GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[heroData.id][5])][3];
        //兵种框
        obj.transform.GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 0, typeof(Sprite)) as Sprite;
        //边框
        FrameChoose(LoadJsonFile.heroTableDatas[heroData.id][3], obj.transform.GetChild(6).GetComponent<Image>());
        //碎片
        if (heroData.level < LoadJsonFile.upGradeTableDatas.Count)
        {
            obj.transform.GetChild(2).GetComponent<Text>().text = heroData.chips + "/" + LoadJsonFile.upGradeTableDatas[heroData.level][1];
            obj.transform.GetChild(2).GetComponent<Text>().color = heroData.chips >= int.Parse(LoadJsonFile.upGradeTableDatas[heroData.level][1]) ? ColorDataStatic.deep_green : Color.white;
        }
        else
        {
            obj.transform.GetChild(2).GetComponent<Text>().text = "";
        }
        if (heroData.level > 0)
        {
            obj.transform.GetChild(4).GetComponent<Image>().enabled = true;
            obj.transform.GetChild(8).gameObject.SetActive(false);
            //设置星级展示
            obj.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + heroData.level, typeof(Sprite)) as Sprite;
            obj.transform.GetChild(7).gameObject.SetActive(false);
            if (heroData.isFight > 0) //出战标记
            {
                PlayerDataForGame.instance.AddOrCutFightCardId(heroData.typeIndex, heroData.id, true);
                obj.transform.GetChild(7).gameObject.SetActive(true);
            }
            else
            {
                obj.transform.GetChild(7).gameObject.SetActive(false);
            }
        }
        else
        {
            obj.transform.GetChild(4).GetComponent<Image>().enabled = false;
            obj.transform.GetChild(7).gameObject.SetActive(false);
            obj.transform.GetChild(8).gameObject.SetActive(true);
        }
        obj.GetComponent<Button>().onClick.RemoveAllListeners();
        obj.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            OnClickHeroCardFun(heroData, obj.transform.GetChild(9).GetComponent<Image>());
        });
    }

    /// <summary>
    /// 点击武将卡牌的方法
    /// </summary>
    /// <param name="heroData"></param>
    private void OnClickHeroCardFun(NowLevelAndHadChip heroData, Image selectImg)
    {
        PlayOnClickMusic();

        //Debug.Log("点击的武将id：" + heroData.id);
        //武将名字
        infoTran.GetChild(0).GetComponent<Text>().text = LoadJsonFile.heroTableDatas[heroData.id][1];
        //武将名字颜色
        infoTran.GetChild(0).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.heroTableDatas[heroData.id][3]);
        //武将属性
        string[] strs_attack = LoadJsonFile.heroTableDatas[heroData.id][7].Split(',');
        infoTran.GetChild(1).GetComponent<Text>().text = string.Format(LoadJsonFile.GetStringText(32), strs_attack[heroData.level > 0 ? heroData.level - 1 : 0]);
        string[] strs_health = LoadJsonFile.heroTableDatas[heroData.id][8].Split(',');
        infoTran.GetChild(2).GetComponent<Text>().text = string.Format(LoadJsonFile.GetStringText(33), strs_health[heroData.level > 0 ? heroData.level - 1 : 0]);
        //武将介绍
        infoTran.GetChild(3).GetComponent<Text>().text = LoadJsonFile.heroTableDatas[heroData.id][2];

        //名字
        ShowNameTextRules(showCardObj.transform.GetChild(3).GetComponent<Text>(), LoadJsonFile.heroTableDatas[heroData.id][1]);
        //名字颜色
        showCardObj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.heroTableDatas[heroData.id][3]);
        //卡牌
        showCardObj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/Hero/" + LoadJsonFile.heroTableDatas[heroData.id][16], typeof(Sprite)) as Sprite;
        //兵种名
        showCardObj.transform.GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[heroData.id][5])][3];
        //兵种框
        showCardObj.transform.GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 0, typeof(Sprite)) as Sprite;
        //边框
        FrameChoose(LoadJsonFile.heroTableDatas[heroData.id][3], showCardObj.transform.GetChild(6).GetComponent<Image>());
        //碎片
        if (heroData.level < LoadJsonFile.upGradeTableDatas.Count)
        {
            showCardObj.transform.GetChild(2).GetComponent<Text>().text = heroData.chips + "/" + LoadJsonFile.upGradeTableDatas[heroData.level][1];
            showCardObj.transform.GetChild(2).GetComponent<Text>().color = heroData.chips >= int.Parse(LoadJsonFile.upGradeTableDatas[heroData.level][1]) ? ColorDataStatic.deep_green : Color.black;
        }
        else
        {
            showCardObj.transform.GetChild(2).GetComponent<Text>().text = "";
        }

        int getGoldNums = GetGoldNumsForSellCard(heroData);
        sellCardBtn.transform.GetChild(0).GetComponent<Text>().text = getGoldNums.ToString();
        sellCardBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        sellCardBtn.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            OnClickForSellCard(heroData, getGoldNums);
        });
        sellCardBtn.SetActive(true);

        if (heroData.level > 0)
        {
            showCardObj.transform.GetChild(4).GetComponent<Image>().enabled = true;
            //设置星级展示
            showCardObj.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + heroData.level, typeof(Sprite)) as Sprite;
            //出战相关设置
            holdOrFightBtn.SetActive(true);
            if (heroData.isFight > 0)
            {
                showCardObj.transform.GetChild(7).gameObject.SetActive(true);
                holdOrFightBtn.GetComponentInChildren<Text>().text = LoadJsonFile.GetStringText(30);
            }
            else
            {
                showCardObj.transform.GetChild(7).gameObject.SetActive(false);
                holdOrFightBtn.GetComponentInChildren<Text>().text = LoadJsonFile.GetStringText(31);
            }
        }
        else
        {
            //sellCardBtn.SetActive(false);
            holdOrFightBtn.SetActive(false);
            showCardObj.transform.GetChild(7).gameObject.SetActive(false);
            showCardObj.transform.GetChild(4).GetComponent<Image>().enabled = false;
        }
        //选择框处理
        if (lastSelectImg != null)
        {
            lastSelectImg.enabled = false;
        }
        lastSelectImg = selectImg;
        lastSelectImg.enabled = true;

        selectCardData = heroData;

        CalculatedNeedYuanBao(heroData.level);
    }

    //根据卡牌类型和id得到其稀有度
    private int GetIdBackCardRarity(int cardType, int cardId)
    {
        string rarityStr = string.Empty;
        switch (cardType)
        {
            case 0:
                rarityStr = LoadJsonFile.heroTableDatas[cardId][3];
                break;
            case 1:
                rarityStr = LoadJsonFile.soldierTableDatas[cardId][3];
                break;
            case 2:
                rarityStr = LoadJsonFile.towerTableDatas[cardId][3];
                break;
            case 3:
                rarityStr = LoadJsonFile.trapTableDatas[cardId][3];
                break;
            case 4:
                rarityStr = LoadJsonFile.spellTableDatas[cardId][3];
                break;
            default:
                break;
        }
        return int.Parse(rarityStr);
    }

    //出售卡牌可得金币
    private int GetGoldNumsForSellCard(NowLevelAndHadChip heroData)
    {
        int chips = heroData.chips;
        for (int i = 0; i < heroData.level; i++)
        {
            chips += int.Parse(LoadJsonFile.upGradeTableDatas[i][1]);
        }
        int golds = 0;
        switch (GetIdBackCardRarity(heroData.typeIndex, heroData.id))
        {
            case 1:
                golds = 10;
                break;
            case 2:
                golds = 20;
                break;
            case 3:
                golds = 50;
                break;
            case 4:
                golds = 100;
                break;
            case 5:
                golds = 200;
                break;
            case 6:
                golds = 500;
                break;
            default:
                break;
        }
        return golds * chips;
    }

    //出售卡牌
    private void OnClickForSellCard(NowLevelAndHadChip heroData, int getGoldNums)
    {
        //if (GetIdBackCardRarity(heroData.typeIndex, heroData.id) >= 4)
        {
            AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[18], AudioController0.instance.audioVolumes[18]);
            AudioController0.instance.PlayAudioSource(0);
            queRenWindows.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            queRenWindows.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate ()
            {
                List<NowLevelAndHadChip> datas = new List<NowLevelAndHadChip>();
                switch (heroData.typeIndex)
                {
                    case 0:
                        datas = PlayerDataForGame.instance.hstData.heroSaveData;
                        break;
                    case 1:
                        datas = PlayerDataForGame.instance.hstData.soldierSaveData;
                        break;
                    case 2:
                        datas = PlayerDataForGame.instance.hstData.towerSaveData;
                        break;
                    case 3:
                        datas = PlayerDataForGame.instance.hstData.trapSaveData;
                        break;
                    case 4:
                        datas = PlayerDataForGame.instance.hstData.spellSaveData;
                        break;
                    default:
                        break;
                }
                //Debug.Log("---出售" + heroData.typeIndex + "类型的卡牌：" + heroData.id);
                heroData.chips = 0;
                heroData.level = 0;
                heroData.isFight = 0;
                //datas.Remove(heroData);
                //LoadSaveData.instance.SaveByJson(PlayerDataForGame.instance.hstData);
                PlayerDataForGame.instance.isNeedSaveData = true;
                LoadSaveData.instance.SaveGameData(2);
                ConsumeManager.instance.AddYuanBao(getGoldNums);
                AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[17], AudioController0.instance.audioVolumes[17]);
                AudioController0.instance.PlayAudioSource(0);
                //刷新主城列表
                ChangeScrollView();
                PlayerDataForGame.instance.AddOrCutFightCardId(heroData.typeIndex, heroData.id, false);
                UpdateCardNumsShow();
                queRenWindows.SetActive(false);
            });
            queRenWindows.SetActive(true);
        }
    }

    /// <summary>
    /// 匹配稀有度的颜色
    /// </summary>
    /// <returns></returns>
    public Color NameColorChoose(string rarity)
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

    // <summary>
    /// 匹配稀有度边框
    /// </summary>
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

    /// <summary>
    /// 对HST的数据进行排序
    /// </summary>
    private void SortHSTData(List<NowLevelAndHadChip> dataList)
    {
        //dataList.Sort((NowLevelAndHadChip n1, NowLevelAndHadChip n2) => n2.level.CompareTo(n1.level));
        dataList.Sort((NowLevelAndHadChip n1, NowLevelAndHadChip n2) =>
        {
            if (n2.isFight.CompareTo(n1.isFight) != 0)
            {
                return n2.isFight.CompareTo(n1.isFight);
            }
            else
            {
                if (n2.level.CompareTo(n1.level) != 0)
                {
                    return n2.level.CompareTo(n1.level);
                }
                else
                {
                    return GetIdBackCardRarity(n2.typeIndex, n2.id).CompareTo(GetIdBackCardRarity(n1.typeIndex, n1.id));
                }
            }
        });
    }

    /// <summary>
    /// 初始化玩家信息显示
    /// </summary>
    public void InitializationPlayerInfo()
    {
        //player`s name
        playerInfoObj.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = LoadJsonFile.playerInitialTableDatas[PlayerDataForGame.instance.pyData.forceId][1];
        if (PlayerDataForGame.instance.pyData.level >= LoadJsonFile.playerLevelTableDatas.Count)
        {
            playerInfoObj.transform.GetChild(0).GetComponent<Slider>().value = 1;
            playerInfoObj.transform.GetChild(2).GetChild(1).GetComponent<Text>().text =LoadJsonFile.GetStringText(34);
            playerInfoObj.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = PlayerDataForGame.instance.pyData.exp + "/" + 99999;
        }
        else
        {
            //exp
            playerInfoObj.transform.GetChild(0).GetComponent<Slider>().value = PlayerDataForGame.instance.pyData.exp / float.Parse(LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.level][1]);
            //level
            playerInfoObj.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = string.Format(LoadJsonFile.GetStringText(35), PlayerDataForGame.instance.pyData.level);//玩家等级
            playerInfoObj.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = PlayerDataForGame.instance.pyData.exp + "/" + LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.level][1];
        }
        //货币
        yuanBaoNumText.text = PlayerDataForGame.instance.pyData.yuanbao.ToString();
        yvQueNumText.text = PlayerDataForGame.instance.pyData.yvque.ToString();
        showTiLiNums = PlayerPrefs.GetInt(TimeSystemControl.staminaStr);
        tiLiNumText.text = showTiLiNums + "/90";

        CreateHeroAndTowerContent();
        UpdateCardNumsShow();

        StartCoroutine(LiteToChangeViewShow(0));
    }

    //得到合成所需元宝
    private void CalculatedNeedYuanBao(int nowLevel)
    {
        if (nowLevel == 0)
        {
            heImgObj.SetActive(true);
            ShowOrHideGuideObj(2, true);
        }
        else
        {
            heImgObj.SetActive(false);
        }
        heImgObj.SetActive(nowLevel == 0);
        if (nowLevel < LoadJsonFile.upGradeTableDatas.Count)
        {
            needYuanBaoNums = int.Parse(LoadJsonFile.upGradeTableDatas[nowLevel][2]);
            heChengBtn.transform.GetComponentInChildren<Text>().text = "" + needYuanBaoNums;
            heChengBtn.SetActive(true);
        }
        else
        {
            heChengBtn.SetActive(false);
        }
    }

    /// <summary>
    /// 合成卡牌
    /// </summary>
    public void SynthesizeCard()
    {
        if (selectCardData.chips >= int.Parse(LoadJsonFile.upGradeTableDatas[selectCardData.level][1]))
        {
            if (ConsumeManager.instance.CutYuanBao(needYuanBaoNums))
            {
                selectCardData.chips -= int.Parse(LoadJsonFile.upGradeTableDatas[selectCardData.level][1]);

                selectCardData.level++;
                if (!selectCardData.isHad)
                {
                    selectCardData.isHad = true;
                    MainWuBeiUIManager.instance.UpdateArmsBtnTextShow();
                }
                if (selectCardData.maxLevel < selectCardData.level)
                {
                    selectCardData.maxLevel = selectCardData.level;
                }
                //LoadSaveData.instance.SaveByJson(PlayerDataForGame.instance.hstData);
                PlayerDataForGame.instance.isNeedSaveData = true;
                LoadSaveData.instance.SaveGameData(2);

                upStarEffectObj.SetActive(false);
                upStarEffectObj.SetActive(true);
                StartCoroutine(HideTheEffectOfUpStar());

                AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[16], AudioController0.instance.audioVolumes[16]);
                AudioController0.instance.PlayAudioSource(0);

                UpdateLevelCard();

                ShowOrHideGuideObj(2, false);
            }
            else
            {
                //Debug.Log("元宝不足，合成失败");
                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(36));
                PlayOnClickMusic();
            }
        }
        else
        {
            //Debug.Log("碎片不足，合成失败");
            PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(37));
            PlayOnClickMusic();
        }
    }

    //隐藏升星特效
    IEnumerator HideTheEffectOfUpStar()
    {
        yield return new WaitForSeconds(1.7f);
        upStarEffectObj.SetActive(false);
    }

    /// <summary>
    /// 出战或回城设置方法
    /// </summary>
    public void ChuZhanOrStaySetFun()
    {
        Transform listCard = lastSelectImg.transform.parent;
        if (selectCardData.isFight > 0)
        {
            if (PlayerDataForGame.instance.AddOrCutFightCardId(selectCardData.typeIndex, selectCardData.id, false))
            {
                listCard.GetChild(7).gameObject.SetActive(false);
                showCardObj.transform.GetChild(7).gameObject.SetActive(false);
                holdOrFightBtn.GetComponentInChildren<Text>().text = LoadJsonFile.GetStringText(31);
                selectCardData.isFight = 0;
                AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[15], AudioController0.instance.audioVolumes[15]);
                AudioController0.instance.PlayAudioSource(0);
            }
            else
            {
                PlayOnClickMusic();
            }
        }
        else
        {
            if (PlayerDataForGame.instance.AddOrCutFightCardId(selectCardData.typeIndex, selectCardData.id, true))
            {
                listCard.GetChild(7).gameObject.SetActive(true);
                showCardObj.transform.GetChild(7).gameObject.SetActive(true);
                holdOrFightBtn.GetComponentInChildren<Text>().text = LoadJsonFile.GetStringText(30);
                selectCardData.isFight = 1;
                AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[14], AudioController0.instance.audioVolumes[14]);
                AudioController0.instance.PlayAudioSource(0);
            }
            else
            {
                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(38));

                PlayOnClickMusic();
            }
        }
        UpdateCardNumsShow();
        //LoadSaveData.instance.SaveByJson(PlayerDataForGame.instance.hstData);
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(2);
    }

    //升级卡牌后更新显示
    private void UpdateLevelCard()
    {
        //Debug.Log("selectCardData.level: " + selectCardData.level);
        Transform listCard = lastSelectImg.transform.parent;
        if (selectCardData.level < LoadJsonFile.upGradeTableDatas.Count)
        {
            listCard.GetChild(2).GetComponent<Text>().text = selectCardData.chips + "/" + LoadJsonFile.upGradeTableDatas[selectCardData.level][1];
            listCard.GetChild(2).GetComponent<Text>().color = selectCardData.chips >= int.Parse(LoadJsonFile.upGradeTableDatas[selectCardData.level][1]) ? ColorDataStatic.deep_green : Color.black;
        }
        else
        {
            listCard.GetChild(2).GetComponent<Text>().text = "";
        }
        listCard.GetChild(4).GetComponent<Image>().enabled = true;
        //设置星级展示
        listCard.GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + selectCardData.level, typeof(Sprite)) as Sprite;
        listCard.GetChild(8).gameObject.SetActive(false);
        listCard.GetComponent<Button>().onClick.Invoke();
    }

    /// <summary>
    /// 展示奖励
    /// </summary>
    /// <param name="yuanBaoNums">元宝</param>
    /// <param name="yuQueNums">玉阙</param>
    /// <param name="expNums">经验</param>
    /// <param name="tiLiNums">体力</param>
    /// <param name="rewardsCards">卡牌奖励</param>
    /// <param name="waitTime">展示等待时间</param>
    public void ShowRewardsThings(int yuanBaoNums, int yuQueNums, int expNums, int tiLiNums, List<RewardsCardClass> rewardsCards, float waitTime)
    {
        for (int i = 0; i < rewardsParent.childCount; i++)
        {
            if (rewardsParent.GetChild(i).gameObject.activeSelf)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (rewardsParent.GetChild(i).GetChild(j).gameObject.activeSelf)
                    {
                        rewardsParent.GetChild(i).GetChild(j).gameObject.SetActive(false);
                    }
                }
                rewardsParent.GetChild(i).gameObject.SetActive(false);
            }
        }

        //rewardsShowObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = str;
        if (yuanBaoNums > 0)
        {
            ShowOneReward(0, new RewardsCardClass() { cardChips = yuanBaoNums });
        }
        if (yuQueNums > 0)
        {
            ShowOneReward(1, new RewardsCardClass() { cardChips = yuQueNums });
        }
        if (expNums > 0)
        {
            ShowOneReward(2, new RewardsCardClass() { cardChips = expNums });
        }
        if (tiLiNums > 0)
        {
            ShowOneReward(3, new RewardsCardClass() { cardChips = tiLiNums });
        }
        for (int i = 0; i < rewardsCards.Count; i++)
        {
            ShowOneReward(4, rewardsCards[i]);
        }
        StartCoroutine(OpenRewardsWindows(waitTime));
    }
    //展示奖品
    IEnumerator OpenRewardsWindows(float startTime)
    {
        yield return new WaitForSeconds(startTime);
        for (int i = 0; i < boxBtnObjs.Length; i++)
        {
            boxBtnObjs[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            boxBtnObjs[i].transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        }
        rewardsShowObj.SetActive(true);
        //刷新主城列表
        ChangeScrollView();
        rewardsShowObj.transform.GetComponentInChildren<ScrollRect>().horizontalNormalizedPosition = 0f;
        yield return new WaitForSeconds(1f);
        rewardsShowObj.transform.GetComponentInChildren<ScrollRect>().DOHorizontalNormalizedPos(1f, 1f);

        yield return new WaitForSeconds(1f);
        PlayerDataForGame.instance.ClearGarbageStationObj();
    }

    //获取单个奖品展示框
    private GameObject FindShowRewardsBox()
    {
        GameObject go = new GameObject();
        PlayerDataForGame.garbageStationObjs.Add(go);

        for (int i = 0; i < rewardsParent.childCount; i++)
        {
            go = rewardsParent.GetChild(i).gameObject;
            if (!go.activeSelf)
            {
                go.SetActive(true);
                return go;
            }
        }
        go = Instantiate(rewardObj, rewardsParent);

        return go;
    }

    /// <summary>
    /// 展示单个奖品
    /// </summary>
    /// <param name="rewardType">0元宝1玉阙2经验3卡牌</param>
    /// <param name="rewardsCard"></param>
    private void ShowOneReward(int rewardType, RewardsCardClass rewardsCard)
    {
        if (rewardsCard.cardChips <= 0)
        {
            return;
        }

        GameObject obj = FindShowRewardsBox();

        obj.transform.GetChild(rewardType).gameObject.SetActive(true);
        if (rewardType == 4)
        {
            Transform cardTran = obj.transform.GetChild(4);
            switch (rewardsCard.cardType)
            {
                case 0:
                    cardTran.GetComponent<Image>().sprite = Resources.Load("Image/Cards/Hero/" + LoadJsonFile.heroTableDatas[rewardsCard.cardId][16], typeof(Sprite)) as Sprite;
                    ShowNameTextRules(cardTran.GetChild(0).GetComponent<Text>(), LoadJsonFile.heroTableDatas[rewardsCard.cardId][1]);
                    cardTran.GetChild(0).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.heroTableDatas[rewardsCard.cardId][3]);
                    cardTran.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 0, typeof(Sprite)) as Sprite;
                    cardTran.GetChild(1).GetChild(0).GetComponentInChildren<Text>().text = LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[rewardsCard.cardId][5])][3];
                    FrameChoose(LoadJsonFile.heroTableDatas[rewardsCard.cardId][3], cardTran.GetChild(2).GetComponent<Image>());
                    break;
                case 1:
                    //cardTran.GetChild(0).GetComponent<Text>().text = LoadJsonFile.soldierTableDatas[rewardsCard.cardId][1];
                    break;
                case 2:
                    cardTran.GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.towerTableDatas[rewardsCard.cardId][10], typeof(Sprite)) as Sprite;
                    ShowNameTextRules(cardTran.GetChild(0).GetComponent<Text>(), LoadJsonFile.towerTableDatas[rewardsCard.cardId][1]);
                    cardTran.GetChild(0).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.towerTableDatas[rewardsCard.cardId][3]);
                    cardTran.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
                    cardTran.GetChild(1).GetChild(0).GetComponentInChildren<Text>().text = LoadJsonFile.towerTableDatas[rewardsCard.cardId][5];
                    FrameChoose(LoadJsonFile.towerTableDatas[rewardsCard.cardId][3], cardTran.GetChild(2).GetComponent<Image>());
                    break;
                case 3:
                    cardTran.GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.trapTableDatas[rewardsCard.cardId][8], typeof(Sprite)) as Sprite;
                    ShowNameTextRules(cardTran.GetChild(0).GetComponent<Text>(), LoadJsonFile.trapTableDatas[rewardsCard.cardId][1]);
                    cardTran.GetChild(0).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.trapTableDatas[rewardsCard.cardId][3]);
                    cardTran.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
                    cardTran.GetChild(1).GetChild(0).GetComponentInChildren<Text>().text = LoadJsonFile.trapTableDatas[rewardsCard.cardId][5];
                    FrameChoose(LoadJsonFile.trapTableDatas[rewardsCard.cardId][3], cardTran.GetChild(2).GetComponent<Image>());
                    break;
                case 4:
                    //cardTran.GetChild(0).GetComponent<Text>().text = LoadJsonFile.spellTableDatas[rewardsCard.cardId][1];
                    break;
                default:
                    break;
            }
        }
        obj.transform.GetChild(5).GetComponent<Text>().text = "×" + rewardsCard.cardChips;
    }


    /// <summary>
    /// 主城界面切换
    /// </summary>
    /// <param name="index"></param>
    public void ZhuChengInterfaceSwitching(int index)
    {
        if (isJumping)
        {
            return;
        }

        PlayOnClickMusic();

        for (int i = 0; i < zhuChengInterFaces.Length; i++)
        {
            zhuChengInterFaces[i].SetActive(false);
            particlesForInterface[i].SetActive(false);
        }
        zhuChengInterFaces[index].SetActive(true);
        particlesForInterface[index].SetActive(true);

        switch (index)
        {
            case 0:
                ShowOrHideGuideObj(0, true);
                if (PlayerDataForGame.instance.gbocData.fightBoxs.Count > 0)
                {
                    ShowOrHideGuideObj(1, true);
                }
                break;
            case 2:
                PlayerDataForGame.instance.isZhanYi = true;
                ShowOrHideGuideObj(3, true);
                warsChooseListObj.transform.parent.parent.GetComponent<ScrollRect>().DOVerticalNormalizedPos(0f, 0.3f);
                break;
            case 4:
                PlayerDataForGame.instance.isZhanYi = false;
                break;
            default:
                break;
        }
    }

    //显示或隐藏指引
    public void ShowOrHideGuideObj(int index, bool isShow)
    {
        if (isShow)
        {
            if (PlayerDataForGame.instance.guideObjsShowed[index] == 0)
            {
                guideObjs[index].SetActive(true);
            }
        }
        else
        {
            if (PlayerDataForGame.instance.guideObjsShowed[index] == 0)
            {
                guideObjs[index].SetActive(false);
                PlayerDataForGame.instance.guideObjsShowed[index] = 1;
                switch (index)
                {
                    case 0:
                        PlayerPrefs.SetInt(StringForGuide.guideJinBaoXiang, 1);
                        break;
                    case 1:
                        PlayerPrefs.SetInt(StringForGuide.guideZYBaoXiang, 1);
                        break;
                    case 2:
                        PlayerPrefs.SetInt(StringForGuide.guideHeCheng, 1);
                        break;
                    case 3:
                        PlayerPrefs.SetInt(StringForGuide.guideStartZY, 1);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //用于刷新列表后选择第一个单位
    IEnumerator LiteUpdateListChooseFirst(float startTime)
    {
        yield return new WaitForSeconds(startTime);
        if (zhuChengHeroContentObj.transform.childCount > 0)
        {
            zhuChengHeroContentObj.transform.GetChild(0).GetComponent<Button>().onClick.Invoke();
        }
    }

    /// <summary>
    /// 获取玩家经验
    /// </summary>
    /// <param name="expNums"></param>
    public void GetPlayerExp(int expNums)
    {
        if (PlayerDataForGame.instance.pyData.level >= LoadJsonFile.playerLevelTableDatas.Count)
        {
            PlayerDataForGame.instance.pyData.exp += expNums;
            playerInfoObj.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = PlayerDataForGame.instance.pyData.exp + "/" + 99999;
        }
        else
        {
            PlayerDataForGame.instance.pyData.exp += expNums;
            while (int.Parse(LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.level][1]) <= PlayerDataForGame.instance.pyData.exp)
            {
                PlayerDataForGame.instance.pyData.exp -= int.Parse(LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.level][1]);
                PlayerDataForGame.instance.pyData.level++;
                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(39));
                if (PlayerDataForGame.instance.pyData.level >= LoadJsonFile.playerLevelTableDatas.Count)
                {
                    PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(40));
                    break;
                }
            }
            if (PlayerDataForGame.instance.pyData.level >= LoadJsonFile.playerLevelTableDatas.Count)
            {
                playerInfoObj.transform.GetChild(0).GetComponent<Slider>().value = 1;
                playerInfoObj.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = LoadJsonFile.GetStringText(34);
                playerInfoObj.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = PlayerDataForGame.instance.pyData.exp + "/" + 99999;
            }
            else
            {
                playerInfoObj.transform.GetChild(0).GetComponent<Slider>().value = PlayerDataForGame.instance.pyData.exp / float.Parse(LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.level][1]);
                playerInfoObj.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = string.Format(LoadJsonFile.GetStringText(35), PlayerDataForGame.instance.pyData.level);
                playerInfoObj.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = PlayerDataForGame.instance.pyData.exp + "/" + LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.level][1];
            }
            UpdateCardNumsShow();
        }
        //LoadSaveData.instance.SaveByJson(PlayerDataForGame.instance.pyData);
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(1);
    }

    /// <summary>
    /// 初始化卡牌池
    /// </summary>
    private void InitHeroCardPooling()
    {
        for (int i = 0; i < minInitCardCount; i++)
        {
            GameObject go = Instantiate(heroCardCityPre, zhuChengHeroContentObj.transform);
            go.SetActive(false);
            heroCardPoolList.Add(go);
        }
    }

    /// <summary>
    /// 从卡牌池中获取空卡牌
    /// </summary>
    /// <returns></returns>
    private GameObject GetHeroCardToShow()
    {
        foreach (GameObject item in heroCardPoolList)
        {
            if (!item.activeSelf)
            {
                item.SetActive(true);
                return item;
            }
        }
        GameObject go = Instantiate(heroCardCityPre, zhuChengHeroContentObj.transform);
        heroCardPoolList.Add(go);
        return go;
    }

    /// <summary>
    /// 回收卡牌池
    /// </summary>
    private void TakeBackHeroCardPooling()
    {
        for (int i = 0; i < heroCardPoolList.Count; i++)
        {
            if (heroCardPoolList[i].activeSelf)
            {
                heroCardPoolList[i].SetActive(false);
            }
        }
    }

    //定位到存档中cardId对应的索引号
    public int FindIndexFromData(List<NowLevelAndHadChip> saveData, int cardId)
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

    //添加体力
    public void AddTiLiNums(int addNums)
    {
        TimeSystemControl.instance.AddTiLiNums(addNums);
    }

    //播放点击音效
    public void PlayOnClickMusic()
    {
        AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[13], AudioController0.instance.audioVolumes[13]);
        AudioController0.instance.PlayAudioSource(0);
    }

    [SerializeField]
    Text musicBtnText;  //音乐开关文本

    //打开设置界面
    public void OpenSettingWinInit()
    {
        PlayOnClickMusic();
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
            PlayOnClickMusic();
        }
        else
        {
            //关闭
            PlayerPrefs.SetInt(LoadSaveData.instance.IsPlayMusicStr, 0);
            AudioController0.instance.isPlayMusic = 0;
            AudioController0.instance.audioSource.Pause();
            AudioController1.instance.audioSource.Pause();
            musicBtnText.text = LoadJsonFile.GetStringText(41);
        }
    }

    [SerializeField]
    GameObject jinNangObj;  //锦囊入口
    [SerializeField]
    GameObject jinNangWindowObj;  //锦囊窗口
    bool isCanOpenJinNang = true;  //记录是否可以开启锦囊

    //刷新锦囊入口的显示
    public void UpdateShowJinNangBtn(bool isCanOpen)
    {
        if (isCanOpenJinNang == isCanOpen) return;
        jinNangObj.SetActive(isCanOpen);
        isCanOpenJinNang = isCanOpen;
    }

    //开启锦囊
    public void OpenJinNangFun()
    {
        if (TimeSystemControl.instance.OnClickToGetJinNang())
        {
            AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[11], AudioController0.instance.audioVolumes[11]);
            AudioController0.instance.PlayAudioSource(0);

            //锦囊奖励界面
            Transform rewardsTran = jinNangWindowObj.transform.GetChild(1).GetChild(1);
            rewardsTran.gameObject.SetActive(false);
            //点击继续obj
            GameObject contuineObj = jinNangWindowObj.transform.GetChild(3).gameObject;
            contuineObj.SetActive(false);

            int randId = UnityEngine.Random.Range(0, LoadJsonFile.knowledgeTableDatas.Count);
            //锦囊底框
            Image jinNangImg = jinNangWindowObj.transform.GetChild(1).GetComponent<Image>();
            jinNangImg.color = new Color(jinNangImg.color.r, jinNangImg.color.g, jinNangImg.color.b, 0);
            //锦囊内容
            Text jinNangText = jinNangWindowObj.transform.GetChild(1).GetChild(0).GetComponent<Text>();
            //新增人物名
            Text jinNangTextName = jinNangWindowObj.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>();
            jinNangText.text = "";
            jinNangTextName.text = "";//新增人物名
            jinNangText.gameObject.SetActive(true);
            jinNangText.color = LoadJsonFile.knowledgeTableDatas[randId][1] == "1" ? ColorDataStatic.name_deepRed : ColorDataStatic.name_brown;
            jinNangText.color = new Color(jinNangText.color.r, jinNangText.color.g, jinNangText.color.b, 0);
            jinNangTextName.color = new Color(jinNangTextName.color.r, jinNangTextName.color.g, jinNangTextName.color.b, 0);

            //奖励内容
            int addTiLiNums = int.Parse(LoadJsonFile.knowledgeTableDatas[randId][3]);
            string[] arrs = LoadJsonFile.knowledgeTableDatas[randId][4].Split(',');
            int addYuanBaoNums = UnityEngine.Random.Range(int.Parse(arrs[0]), int.Parse(arrs[1]));
            //背景点击继续按钮
            Button jinNangBackBtn = jinNangWindowObj.transform.GetChild(0).GetComponent<Button>();
            jinNangBackBtn.onClick.RemoveAllListeners();
            //点击广告按钮
            Button watchAdFordoubleBtn = rewardsTran.GetChild(1).GetComponent<Button>();
            watchAdFordoubleBtn.onClick.RemoveAllListeners();
            watchAdFordoubleBtn.gameObject.SetActive(true);

            jinNangImg.DOFade(1, 0.5f).OnComplete(delegate ()
            {
                jinNangText.text = LoadJsonFile.knowledgeTableDatas[randId][2];
                jinNangTextName.text = LoadJsonFile.knowledgeTableDatas[randId][5];//新增人物名
                jinNangTextName.DOFade(1, 1.5f);
                jinNangText.DOFade(1, 1.5f).OnComplete(delegate ()
                {
                    contuineObj.SetActive(true);
                    jinNangBackBtn.onClick.AddListener(delegate ()
                    {
                        PlayOnClickMusic();
                        jinNangTextName.DOFade(0, 1f);
                        jinNangText.DOFade(0, 1f).OnComplete(delegate ()
                        {
                            //展示奖励内容
                            UpdateJinNangRewards(addYuanBaoNums, addTiLiNums);
                            rewardsTran.gameObject.SetActive(true);

                            //点击背景领取并退出锦囊
                            jinNangBackBtn.onClick.AddListener(delegate ()
                            {
                                PlayOnClickMusic();
                                if (addYuanBaoNums > 0)
                                    ConsumeManager.instance.AddYuanBao(addYuanBaoNums);
                                if (addTiLiNums > 0)
                                    TimeSystemControl.instance.AddTiLiNums(addTiLiNums);
                                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(43));
                                jinNangWindowObj.SetActive(false);
                            });
                            //点击广告双倍奖励
                            watchAdFordoubleBtn.onClick.AddListener(delegate ()
                            {
                                PlayOnClickMusic();
                                //背景按钮无效
                                jinNangBackBtn.enabled = false;
                                watchAdFordoubleBtn.enabled = false;
                                if (!DoNewAdController.instance.GetReWardVideo(
                                //if (!AdController.instance.ShowVideo(
                                    delegate ()
                                    {
                                        //奖励翻倍
                                        addYuanBaoNums = addYuanBaoNums * 2;
                                        addTiLiNums = addTiLiNums * 2;
                                        UpdateJinNangRewards(addYuanBaoNums, addTiLiNums);
                                        jinNangBackBtn.enabled = true;
                                        watchAdFordoubleBtn.gameObject.SetActive(false);
                                        watchAdFordoubleBtn.enabled = true;
                                    },
                                    delegate ()
                                    {
                                        PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(6));
                                        jinNangBackBtn.enabled = true;
                                        watchAdFordoubleBtn.enabled = true;
                                    }
                                    ))
                                {
                                    PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(6));
                                    jinNangBackBtn.enabled = true;
                                    watchAdFordoubleBtn.enabled = true;
                                }

                            });
                        });
                        jinNangBackBtn.onClick.RemoveAllListeners();
                    });
                });
            });
            jinNangWindowObj.SetActive(true);
        }
        else
        {
            PlayOnClickMusic();
            PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(44));
        }
    }

    [SerializeField]
    Transform jinNangRewardsTran;   //锦囊奖励父级

    //展示锦囊中的奖励内容
    private void UpdateJinNangRewards(int yuanBaoNums, int tiLiNums)
    {
        //元宝
        if (yuanBaoNums > 0)
        {
            jinNangRewardsTran.GetChild(0).GetChild(4).GetComponent<Text>().text = "×" + yuanBaoNums;
            jinNangRewardsTran.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            jinNangRewardsTran.GetChild(0).gameObject.SetActive(false);
        }
        //体力
        if (tiLiNums > 0)
        {
            jinNangRewardsTran.GetChild(3).GetChild(4).GetComponent<Text>().text = "×" + tiLiNums;
            jinNangRewardsTran.GetChild(3).gameObject.SetActive(true);
        }
        else
        {
            jinNangRewardsTran.GetChild(3).gameObject.SetActive(false);
        }
    }

    [SerializeField]
    Button rtCloseBtn;  //兑换界面关闭按钮
    [SerializeField]
    InputField rtInputField;  //兑换界面输入控件
    [SerializeField]
    Button rtconfirmBtn;  //兑换界面确认兑换按钮

    //兑换礼包方法
    public void RedemptionCodeFun()
    {
        string str = rtInputField.text;
        if (str == "")
        {
            PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(45));
            PlayOnClickMusic();
        }
        else
        {
            int indexId = -1;
            for (int i = 0; i < LoadJsonFile.rCodeTableDatas.Count; i++)
            {
                if (str == LoadJsonFile.rCodeTableDatas[i][1])
                {
                    indexId = i;
                    break;
                }
            }
            if (indexId != -1)
            {
                string[] arr = LoadJsonFile.rCodeTableDatas[indexId][2].Split('-');
                DateTime startTime = DateTime.ParseExact(arr[0], "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                DateTime endTime = DateTime.ParseExact(arr[1], "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                DateTime nowTime = TimeSystemControl.instance.SystemTimer.Now.LocalDateTime;

                if (nowTime < startTime || nowTime > endTime)
                {
                    rtInputField.text = "";
                    PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(47));
                    PlayOnClickMusic();
                }
                else
                {
                    if (!PlayerDataForGame.instance.gbocData.redemptionCodeGotList[indexId].isGot)
                    {
                        //获得奖励
                        int addYvQueNums = int.Parse(LoadJsonFile.rCodeTableDatas[indexId][4]);
                        ConsumeManager.instance.AddYuQue(addYvQueNums);
                        int addYuanBaoNums = int.Parse(LoadJsonFile.rCodeTableDatas[indexId][5]);
                        ConsumeManager.instance.AddYuanBao(addYuanBaoNums);
                        int tiLiNums = int.Parse(LoadJsonFile.rCodeTableDatas[indexId][6]);
                        AddTiLiNums(tiLiNums);
                        string[] arrRewards = LoadJsonFile.rCodeTableDatas[indexId][7].Split(';');
                        List<RewardsCardClass> rewards = new List<RewardsCardClass>();
                        int cardType = 0;
                        int cardId = 0;
                        int chips = 0;
                        for (int i = 0; i < arrRewards.Length; i++)
                        {
                            if (arrRewards[i] != "")
                            {
                                string[] arrs = arrRewards[i].Split(',');
                                cardType = int.Parse(arrs[0]);
                                cardId = int.Parse(arrs[1]);
                                chips = int.Parse(arrs[2]);
                                GetAndSaveCardChips(cardType, cardId, chips);

                                RewardsCardClass rewardCard = new RewardsCardClass();
                                rewardCard.cardType = cardType;
                                rewardCard.cardId = cardId;
                                rewardCard.cardChips = chips;
                                rewards.Add(rewardCard);
                            }
                        }
                        PlayerDataForGame.instance.isNeedSaveData = true;
                        LoadSaveData.instance.SaveGameData(2);
                        ShowRewardsThings(addYuanBaoNums, addYvQueNums, 0, tiLiNums, rewards, 0);

                        PlayerDataForGame.instance.gbocData.redemptionCodeGotList[indexId].isGot = true;
                        PlayerDataForGame.instance.isNeedSaveData = true;
                        LoadSaveData.instance.SaveGameData(4);

                        rtInputField.text = "";
                        PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.rCodeTableDatas[indexId][3]);
                        rtCloseBtn.onClick.Invoke();
                        AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[0], AudioController0.instance.audioVolumes[0]);
                        AudioController0.instance.PlayAudioSource(0);
                    }
                    else
                    {
                        rtInputField.text = "";
                        PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(48));
                        PlayOnClickMusic();
                    }
                }
            }
            else
            {
                rtInputField.text = "";
                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(49));
                PlayOnClickMusic();
            }
        }
    }

    //获取并存储奖励碎片
    private void GetAndSaveCardChips(int cardType, int cardId, int chips)
    {
        switch (cardType)
        {
            case 0:
                PlayerDataForGame.instance.hstData.heroSaveData[FindIndexFromData(PlayerDataForGame.instance.hstData.heroSaveData, cardId)].chips += chips;
                break;
            case 1:
                PlayerDataForGame.instance.hstData.soldierSaveData[FindIndexFromData(PlayerDataForGame.instance.hstData.soldierSaveData, cardId)].chips += chips;
                break;
            case 2:
                PlayerDataForGame.instance.hstData.towerSaveData[FindIndexFromData(PlayerDataForGame.instance.hstData.towerSaveData, cardId)].chips += chips;
                break;
            case 3:
                PlayerDataForGame.instance.hstData.trapSaveData[FindIndexFromData(PlayerDataForGame.instance.hstData.trapSaveData, cardId)].chips += chips;
                break;
            case 4:
                PlayerDataForGame.instance.hstData.spellSaveData[FindIndexFromData(PlayerDataForGame.instance.hstData.spellSaveData, cardId)].chips += chips;
                break;
            default:
                break;
        }
    }

    ///////////////////////////鸡坛相关/////////////////////////////////

    //给体力商店按钮添加方法
    private void InitChickenBtnFun()
    {
        for (int i = 0; i < chickenShopBtns.Length; i++)
        {
            int index = i;
            chickenShopBtns[i].onClick.AddListener(delegate ()
            {
                ChickenShoppingGetTiLi(index);
            });
            //显示体力的数量
            chickenShopBtns[i].transform.parent.GetChild(1).GetComponent<Text>().text = "×" + LoadJsonFile.tiLiStoreTableDatas[i][1];
            //显示消耗玉阙的数量
            if (i != 0)
            {
                chickenShopBtns[i].transform.GetChild(0).GetComponent<Text>().text = "×" + LoadJsonFile.tiLiStoreTableDatas[i][2];
            }
        }

        chickenEntObj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate() {
            AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[25], AudioController0.instance.audioVolumes[25]);
            AudioController0.instance.PlayAudioSource(0);
            chickenShopWindowObj.SetActive(true);
        });
    }

    //体力商店按钮统一处理
    private void OpenOrCloseChickenBtn(bool isCanTake)
    {
        for (int i = 0; i < chickenShopBtns.Length; i++)
        {
            chickenShopBtns[i].enabled = isCanTake;
        }
    }

    //消耗玉阙获得体力
    private bool GetTiLiForChicken(int quQueNums, int tiLiNums)
    {
        if (ConsumeManager.instance.CutYuQue(quQueNums))
        {
            AddTiLiNums(tiLiNums);
            return true;
        }
        else
        {
            return false;
        }
    }

    //商店购买体力
    private void ChickenShoppingGetTiLi(int indexBtn)
    {
        AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[13], AudioController0.instance.audioVolumes[13]);
        OpenOrCloseChickenBtn(false);
        int getTiLiNums = int.Parse(LoadJsonFile.tiLiStoreTableDatas[indexBtn][1]);
        int needYvQueNums = int.Parse(LoadJsonFile.tiLiStoreTableDatas[indexBtn][2]);
        switch (indexBtn)
        {
            case 0:
                if (!DoNewAdController.instance.GetReWardVideo(
                //if (!AdController.instance.ShowVideo(
                    delegate ()
                    {
                        GetTiLiForChicken(needYvQueNums, getTiLiNums);
                        PlayerDataForGame.instance.ShowStringTips(string.Format(LoadJsonFile.GetStringText(50), getTiLiNums));
                        GetCkChangeTimeAndWindow();
                        AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[25], AudioController0.instance.audioVolumes[25]);
                        AudioController0.instance.PlayAudioSource(0);
                    },
                    delegate ()
                    {
                        PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(6));
                        OpenOrCloseChickenBtn(true);
                    }))
                {
                    PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(6));
                    OpenOrCloseChickenBtn(true);
                }
                break;
            case 1:
                if (GetTiLiForChicken(needYvQueNums, getTiLiNums))
                {
                    PlayerDataForGame.instance.ShowStringTips(string.Format(LoadJsonFile.GetStringText(51), getTiLiNums));
                    GetCkChangeTimeAndWindow();
                    AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[25], AudioController0.instance.audioVolumes[25]);
                }
                else
                {
                    OpenOrCloseChickenBtn(true);
                }
                break;
            case 2:
                if (GetTiLiForChicken(needYvQueNums, getTiLiNums))
                {
                    PlayerDataForGame.instance.ShowStringTips(string.Format(LoadJsonFile.GetStringText(51), getTiLiNums));
                    GetCkChangeTimeAndWindow();
                    AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[25], AudioController0.instance.audioVolumes[25]);
                }
                else
                {
                    OpenOrCloseChickenBtn(true);
                }
                break;
        }
        AudioController0.instance.PlayAudioSource(0);
    }

    //成功获得体力后的方法
    private void GetCkChangeTimeAndWindow()
    {
        //当前时间点TimeOfDay
        TimeSpan dspNow = TimeSystemControl.instance.SystemTimer.Now.LocalDateTime.TimeOfDay;
        //TimeSpan dspNow = DateTime.Now.TimeOfDay;

        //在12点-14点之间
        if (chickenOpenTs[0][0] < dspNow && dspNow < chickenOpenTs[0][1])
        {
            openCKTime0 = 2;
            PlayerPrefs.SetInt(TimeSystemControl.openCKTime0_str, openCKTime0);
        }
        //在17点-19点之间
        if (chickenOpenTs[1][0] < dspNow && dspNow < chickenOpenTs[1][1])
        {
            openCKTime1 = 2;
            PlayerPrefs.SetInt(TimeSystemControl.openCKTime1_str, openCKTime1);
        }
        //在21点-23点之间
        if (chickenOpenTs[2][0] < dspNow && dspNow < chickenOpenTs[2][1])
        {
            openCKTime2 = 2;
            PlayerPrefs.SetInt(TimeSystemControl.openCKTime2_str, openCKTime2);
        }

        chickenShopWindowObj.SetActive(false);
        OpenOrCloseChickenBtn(true);
    }

    //鸡坛开启时间点
    string[][] chickenOpenTimeStr = new string[3][] {
        new string[2]{ "12:00", "14:00"}, 
        new string[2]{ "16:00", "18:00"},   //关闭
        new string[2]{ "19:00", "21:00"}  
    };

    TimeSpan[][] chickenOpenTs = new TimeSpan[3][];

    //初始化鸡坛开启时间
    private void InitChickenOpenTs()
    {
        for (int i = 0; i < chickenOpenTs.Length; i++)
        {
            TimeSpan[] ts = new TimeSpan[2];
            ts[0] = DateTime.Parse(chickenOpenTimeStr[i][0]).TimeOfDay;
            ts[1] = DateTime.Parse(chickenOpenTimeStr[i][1]).TimeOfDay;
            chickenOpenTs[i] = ts;
        }

        //这是当天首次进游戏
        if (TimeSystemControl.instance.isFInGame)
        {
            openCKTime0 = 0;
            PlayerPrefs.SetInt(TimeSystemControl.openCKTime0_str, openCKTime0);
            openCKTime1 = 0;
            PlayerPrefs.SetInt(TimeSystemControl.openCKTime1_str, openCKTime1);
            openCKTime2 = 0;
            PlayerPrefs.SetInt(TimeSystemControl.openCKTime2_str, openCKTime2);
        }
        else
        {
            openCKTime0 = PlayerPrefs.GetInt(TimeSystemControl.openCKTime0_str);
            openCKTime1 = PlayerPrefs.GetInt(TimeSystemControl.openCKTime1_str);
            openCKTime2 = PlayerPrefs.GetInt(TimeSystemControl.openCKTime2_str);
        }
    }

    //对开启鸡坛时间进行矫正
    public void InitOpenChickenTime(bool isGetNetTime)
    {
        if (!isGetNetTime)
        {
            //没有网络连接关闭鸡坛入口
            if (chickenEntObj.activeSelf)
            {
                chickenEntObj.SetActive(false);
            }
        }
        else
        {
            bool isOpen = CanOpenChickenEntr();
            if (chickenEntObj.activeSelf != isOpen)
            {
                chickenEntObj.SetActive(isOpen);
            }
        }
    }

    int openCKTime0 = 0;    //0未到时1可开启2已领取
    int openCKTime1 = 0;
    int openCKTime2 = 0;

    int closeCkWinSeconds = 7201;

    //刷新鸡坛关闭时间显示
    private void UpdateChickenCloseTime(TimeSpan dspNow, TimeSpan dspEnd)
    {
        int seconds = (int)(dspEnd.TotalSeconds - dspNow.TotalSeconds);
        if (seconds < closeCkWinSeconds)
        {
            closeCkWinSeconds = seconds;
            chickenCloseText.text = TimeSystemControl.instance.TimeDisplayText(closeCkWinSeconds);
        }
    }

    //是否可以开启鸡坛
    private bool CanOpenChickenEntr()
    {
        //当前时间点TimeOfDay
        TimeSpan dspNow = TimeSystemControl.instance.SystemTimer.Now.LocalDateTime.TimeOfDay;
        //TimeSpan dspNow = DateTime.Now.TimeOfDay;

        //在12点-14点之间
        if (chickenOpenTs[0][0] < dspNow && dspNow < chickenOpenTs[0][1])
        {
            //如果未领取过
            if (openCKTime0 != 2)
            {
                if (openCKTime0 == 0)
                {
                    openCKTime0 = 1;
                    PlayerPrefs.SetInt(TimeSystemControl.openCKTime0_str, openCKTime0);

                    openCKTime2 = 0;
                    PlayerPrefs.SetInt(TimeSystemControl.openCKTime2_str, openCKTime2);

                    closeCkWinSeconds = 7201;

                    TimeSystemControl.instance.UpdateIsNotFirstInGame();
                }
                UpdateChickenCloseTime(dspNow, chickenOpenTs[0][1]);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (openCKTime0 != 0)
            {
                openCKTime0 = 0;
                PlayerPrefs.SetInt(TimeSystemControl.openCKTime0_str, openCKTime0);
            }
        }
        //在17点-19点之间
        if (chickenOpenTs[1][0] < dspNow && dspNow < chickenOpenTs[1][1])
        {
            if (openCKTime1 != 2)
            {
                if (openCKTime1 == 0)
                {
                    openCKTime1 = 1;
                    PlayerPrefs.SetInt(TimeSystemControl.openCKTime1_str, openCKTime1);

                    openCKTime0 = 0;
                    PlayerPrefs.SetInt(TimeSystemControl.openCKTime0_str, openCKTime0);

                    closeCkWinSeconds = 7201;

                    TimeSystemControl.instance.UpdateIsNotFirstInGame();
                }
                UpdateChickenCloseTime(dspNow, chickenOpenTs[1][1]);
                return false;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (openCKTime1 != 0)
            {
                openCKTime1 = 0;
                PlayerPrefs.SetInt(TimeSystemControl.openCKTime1_str, openCKTime1);
            }
        }
        //在21点-23点之间
        if (chickenOpenTs[2][0] < dspNow && dspNow < chickenOpenTs[2][1])
        {
            if (openCKTime2 != 2)
            {
                if (openCKTime2 == 0)
                {
                    openCKTime2 = 1;
                    PlayerPrefs.SetInt(TimeSystemControl.openCKTime2_str, openCKTime2);

                    openCKTime1 = 0;
                    PlayerPrefs.SetInt(TimeSystemControl.openCKTime1_str, openCKTime1);

                    openCKTime0 = 0;
                    PlayerPrefs.SetInt(TimeSystemControl.openCKTime1_str, openCKTime1);

                    closeCkWinSeconds = 7201;

                    TimeSystemControl.instance.UpdateIsNotFirstInGame();
                }
                UpdateChickenCloseTime(dspNow, chickenOpenTs[2][1]);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (openCKTime2 != 0)
            {
                openCKTime2 = 0;
                PlayerPrefs.SetInt(TimeSystemControl.openCKTime2_str, openCKTime2);
            }
        }
        return false;
    }

    bool isShowQuitTips = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isShowQuitTips)
            {
                ExitGame();
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

    /// <summary>
    /// 存储游戏
    /// </summary>
    public void SaveGame()
    {
        LoadSaveData.instance.SaveGameData();
    }

    //退出游戏
    public void ExitGame() {
        PlayOnClickMusic();

#if UNITY_ANDROID
        Application.Quit();
#endif
    }
}