using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System;
using System.Linq;

using Beebyte.Obfuscator;
using CorrelateLib;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;
    //玩家势力 
    public enum Pages
    {
        桃园,
        主城,
        战役,
        对决,
        霸业
    }

    public Pages currentPage;
    public Image waitWhileImpress;//敬请期待 
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
    GameObject holdOrFightBtn;      //出战或回城切换按钮obj 
    [SerializeField]
    GameObject sellCardBtn;      //出售按钮obj 
    [SerializeField]
    Text tiLiRecordTimer;   //体力恢复倒计时 
    [SerializeField]
    GameObject queRenWindows;   //操作确认窗口 
    //[SerializeField] 
    public GameObject[] boxBtnObjs;    //宝箱obj 
    public Expedition expedition;//战役 
    public TaoYuan taoYuan;//桃园 
    public Text JinNangQuota;    //锦囊配额文本 

    [SerializeField]
    Transform rewardsParent;    //奖品父级 
    [SerializeField]
    GameObject rewardObj;       //奖品预制件 

    private int needYuanBaoNums;    //记录所需元宝数 

    Image lastSelectImg;    //对上一个选择的卡牌selectImg的标记 
    NowLevelAndHadChip selectCardData;  //记录选择的卡牌存档数据 

    public bool IsJumping { get; private set; } //记录界面是否进行跳转 

    private int minInitCardCount = 20;  //卡牌池基础数量 
    private List<GameObject> heroCardPoolList = new List<GameObject>();     //卡牌池 

    [SerializeField]
    Transform chonseWarDifTran; //难度选择父级 

    [SerializeField]
    GameObject upStarEffectObj; //升星特效 

    [SerializeField]
    GameObject[] guideObjs; // 指引objs 0:桃园宝箱 1:战役宝箱 2:合成 3:开始战役 

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

    //public ForceSelectorUi warForceSelectorUi;//战役势力选择器 
    //private int lastAvailableStageIndex;//最远可战的战役索引 
    public BaYeForceSelectorUi baYeForceSelectorUi;//战役势力选择器 
    public BaYeProgressUI baYeProgressUi; //霸业经验条 
    public ChestUI[] baYeChestButtons; //霸业宝箱 
    public StoryEventUIController storyEventUiController;//霸业的故事事件控制器 
    public BaYeWindowUI baYeWindowUi;//霸业地图小弹窗 
    public Button baYeWarButton;//霸业开始战斗按键 
    public Image bayeBelowLevelPanel;//霸业等级不足挡板 

    private List<BaYeCityField> cityFields; //霸业的地图物件 
    private List<BaYeForceField> forceFields; //可选势力物件 
    [HideInInspector] public RewardManager rewardManager;
    private GameResources GameResources => PlayerDataForGame.instance.GameResources;
    public bool IsInit { get; private set; }

    [SerializeField]
    GameObject InfoWindowObj; //说明窗口 
    [SerializeField]
    Text InfoTitle;
    [SerializeField]
    Text InfoText;

    bool isShowInfo;//说明窗口是否开启 


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        IsJumping = false;
        needYuanBaoNums = 0;
        indexChooseListForceId = 0;
        selectCardData = new NowLevelAndHadChip();
        rewardManager = gameObject.AddComponent<RewardManager>();
    }

    // Start is called before the first frame update 
    void Start()
    {
        AudioController1.instance.ChangeBackMusic();
        TimeSystemControl.instance.isOpenMainScene = true;

        Invoke(nameof(GetBackTiLiForFight), 2f);

        ItemsRedemptionFunc();
        //版本修正 
        BugHotFix.OnFixYvQueV1_90();
        BugHotFix.OnFixLianYuV2_02();
        TimeSystemControl.instance.InitStaminaCount(PlayerDataForGame.instance.pyData.Stamina <
                                                    TimeSystemControl.instance.MaxStamina);

        //第一次进入主场景的时候初始化霸业管理器 
        if (PlayerDataForGame.instance.BaYeManager == null)
        {
            PlayerDataForGame.instance.BaYeManager = PlayerDataForGame.instance.gameObject.AddComponent<BaYeManager>();
            PlayerDataForGame.instance.BaYeManager.Init();
        }

        InitHeroCardPooling();

        InitializationPlayerInfo();
        expedition.Init();

        InitChickenOpenTs();
        InitChickenBtnFun();
        InitJiBanForMainFun();
        InitBaYeFun();
        PlayerDataForGame.instance.ClearGarbageStationObj();

        OnStartMainScene();
        PlayerDataForGame.instance.selectedWarId = -1;
        IsInit = true;
    }

    //时间管理 

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
    public BaYeEventUIController baYeBattleEventController;   //霸业事件点父级 
    [SerializeField]
    public GameObject chooseBaYeEventImg;  //选择霸业地点的Img 
    [SerializeField]
    Text baYeGoldNumText;   //霸业金币数量 

    /// <summary> 
    /// 游戏物品获取次数计算函数 
    /// </summary> 
    public void ItemsRedemptionFunc()
    {
        if (!SystemTimer.IsToday(PlayerDataForGame.instance.pyData.LastJinNangRedeemTime))
            PlayerDataForGame.instance.SetRedeemCount(PlayerDataForGame.RedeemTypes.JinNang, 0);
        if (!SystemTimer.IsToday(PlayerDataForGame.instance.pyData.LastJiuTanRedeemTime))
            PlayerDataForGame.instance.SetRedeemCount(PlayerDataForGame.RedeemTypes.JiuTan, 0);
        //根据系统时间计算本地的天数是否是同一天 
    }

    /// <summary> 
    /// Main场景(切换)初始化 
    /// </summary> 
    private void OnStartMainScene()
    {
        switch (PlayerDataForGame.instance.WarType)
        {
            case PlayerDataForGame.WarTypes.None:
            case PlayerDataForGame.WarTypes.Expedition:
                MainPageSwitching(1);
                break;
            case PlayerDataForGame.WarTypes.Baye:
                MainPageSwitching(4);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        PlayerDataForGame.instance.WarType = PlayerDataForGame.WarTypes.None;
        var tips = PlayerDataForGame.instance.mainSceneTips;
        if (!string.IsNullOrWhiteSpace(tips))
        {
            PlayerDataForGame.instance.ShowStringTips(tips);
            PlayerDataForGame.instance.mainSceneTips = string.Empty;
        }
    }
    //显示霸业玩法说明 
    public void ShowInfoBaYe()
    {
        string title = DataTable.GetStringText(68);
        string text = DataTable.GetStringText(69);
        ShowInfo(title, text);
    }

    /// <summary> 
    /// 打开说明窗口 
    /// </summary> 
    public void ShowInfo(string infoTitle, string infoText)
    {
        if (!isShowInfo)
        {
            InfoWindowObj.SetActive(true);
            //标题 
            InfoTitle.text = infoTitle;
            //文本 
            InfoText.text = infoText;
        }
        isShowInfo = true;
    }
    /// <summary> 
    /// 关闭说明窗口 
    /// </summary> 
    public void HideInfo()
    {
        if (isShowInfo)
        {
            InfoWindowObj.SetActive(false);
        }
        isShowInfo = false;
    }
    //开始霸业战斗 
    public void StartBaYeFight()
    {
        PlayerDataForGame.instance.WarType = PlayerDataForGame.WarTypes.Baye;
        var selectedForce = PlayerDataForGame.instance.CurrentWarForceId;
        switch (BaYeManager.instance.CurrentEventType)
        {
            case BaYeManager.EventTypes.Story:
                {
                    var map = PlayerDataForGame.instance.warsData.baYe.storyMap;
                    if (!map.TryGetValue(BaYeManager.instance.CurrentEventPoint, out var storyEvent))
                    {
                        PlayerDataForGame.instance.ShowStringTips("请选择有效的战斗点。");
                        return;
                    }
                    if (selectedForce < 0) break;
                    if (!PlayerDataForGame.instance.ConsumeZhanLing()) return;//消费战令 
                    BaYeManager.instance.CacheCurrentStoryEvent();
                    PlayerDataForGame.instance.warsData.baYe.storyMap.Remove(BaYeManager.instance.CurrentEventPoint);
                    PlayerDataForGame.instance.isNeedSaveData = true;
                    LoadSaveData.instance.SaveGameData(3);
                    StartBattle(storyEvent.WarId);
                    return;
                }
            case BaYeManager.EventTypes.City:
                {
                    var city = BaYeManager.instance.Map.SingleOrDefault(e =>
                        e.CityId == PlayerDataForGame.instance.selectedCity);
                    if (city == null || selectedForce < 0) break;
                    var savedEvent =
                        PlayerDataForGame.instance.warsData.baYe.data.SingleOrDefault(e => e.CityId == city.CityId);
                    var passes = 0;
                    if (savedEvent != null)
                        passes = savedEvent.PassedStages.Count(pass => pass);
                    if (passes == city.WarIds.Count)
                    {
                        PlayerDataForGame.instance.ShowStringTips("该地区已经平定了噢。");
                        return;
                    }
                    if (!PlayerDataForGame.instance.ConsumeZhanLing()) return;//消费战令 
                    PlayerDataForGame.instance.SaveBaYeWarEvent();
                    StartBattle(city.WarIds[passes]);
                    return;
                }
            default:
                throw new ArgumentOutOfRangeException();
        }

        print("请选择");
        //提示选择势力后进行战斗 
        PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(65));

        void StartBattle(int warId)
        {
            if (IsJumping) return;
            IsJumping = true;
            AudioController0.instance.ForcePlayAudio(12);
            AudioController0.instance.ChangeAudioClip(12);
            AudioController0.instance.PlayAudioSource(0);
            PlayerDataForGame.instance.selectedWarId = warId;
            print($"开始战斗 WarId[{warId}]");
            StartCoroutine(LateGoToFightScene());
        }
    }

    //初始化霸业界面内容 
    private void InitBaYeFun()
    {
        baYeForceSelectorUi.Init(PlayerDataForGame.WarTypes.Baye);
        baYeWarButton.onClick.RemoveAllListeners();
        baYeWarButton.onClick.AddListener(StartBaYeFight);
        storyEventUiController.ResetUi();
        baYeWindowUi.Init();
        var baYe = PlayerDataForGame.instance.warsData.baYe;
        PlayerDataForGame.instance.selectedBaYeEventId = -1;
        PlayerDataForGame.instance.selectedCity = -1;
        //霸业经验条和宝箱初始化 
        ResetBaYeProgressAndGold();
        //城市点初始化 
        var cityLvlMap = new Dictionary<int,int>();//maxCity, level
        DataTable.PlayerLevelConfig.Values.Select(lvl =>
        {
            var maxCity = lvl.BaYeCityPoints.Max();
            return new {level = lvl.Level, maxCity};
        }).ToList().ForEach(lvl =>
        {
            if (!cityLvlMap.ContainsKey(lvl.maxCity))
            {
                cityLvlMap.Add(lvl.maxCity, lvl.level);
                return;
            }
            cityLvlMap[lvl.maxCity] = cityLvlMap[lvl.maxCity] > lvl.level ? cityLvlMap[lvl.maxCity] : lvl.level;
        });
        var cityList = DataTable.PlayerLevelConfig[PlayerDataForGame.instance.pyData.Level].BaYeCityPoints;
        if (cityFields != null && cityFields.Count > 0)
            cityFields.ForEach(Destroy);
        cityFields = new List<BaYeCityField>();
        for (int i = 0; i < baYeBattleEventController.eventList.Length; i++)
        {
            int cityPoint = i;
            //得到战役id 
            var ui = baYeBattleEventController.eventList[i];
            var cityField = ui.gameObject.AddComponent<BaYeCityField>();
            cityField.id = cityPoint;
            cityField.button = ui.button;
            cityFields.Add(cityField);
            var baYeEvent = BaYeManager.instance.Map.Single(e => e.CityId == cityPoint);
            var baYeRecord = baYe.data.SingleOrDefault(f => f.CityId == cityPoint);
            var flag = (ForceFlags)DataTable.BaYeCityEvent[baYeEvent.EventId].FlagId;//旗帜id 
            var flagName = DataTable.BaYeCityEvent[baYeEvent.EventId].FlagText;//旗帜文字 
            ui.button.interactable = cityList.Length > i;
            ui.forceFlag.Set(flag, true, flagName);
            if (cityList.Length > i)
            {
                var city = BaYeManager.instance.Map.Single(c => c.CityId == i);
                ui.Init(city.WarIds.Count);
                ui.button.onClick.RemoveAllListeners();
                ui.button.onClick
                    .AddListener(
                        () => BaYeManager.instance.OnBaYeWarEventPointSelected(BaYeManager.EventTypes.City, baYeEvent.CityId));
                ui.text.text = DataTable.BaYeCity[cityPoint].Name; //城市名 
            }
            else
            {
                ui.text.text = $"{cityLvlMap[i]}级开启";
                ui.InactiveCityColor();
            }

            if (baYeRecord == null) continue;
            cityField.boundWars = baYeRecord.WarIds;
            var passCount = baYeRecord.PassedStages.Count(isPass => isPass);
            ui.SetValue(passCount);
            if (baYeRecord.PassedStages.Length == passCount)//如果玩家已经过关 
                ui.forceFlag.Hide();
        }
    }

    public void ResetBaYeProgressAndGold()
    {
        var baYe = PlayerDataForGame.instance.warsData.baYe;
        var baYeReward = DataTable.BaYeTask.Values
            .Select(task => new { id = task.Id, exp = task.Exp, warChestId = task.WarChestTableId })
            .ToList();
        baYeGoldNumText.text = $"{baYe.gold}/{BaYeManager.instance.BaYeMaxGold}";
        baYeProgressUi.Set(baYe.CurrentExp, baYeReward[baYeReward.Count - 1].exp);
        for (int i = 0; i < baYeReward.Count; i++)
        {
            baYeChestButtons[i].Text.text = baYeReward[i].exp.ToString();
            //如果玩家霸业的经验值大于宝箱经验值并宝箱未被开过 
            if (baYe.CurrentExp < baYeReward[i].exp)
            {
                baYeChestButtons[i].Disabled();
                continue;
            }

            //如果宝箱未被开过 
            if (!baYe.openedChest[i])
                baYeChestButtons[i].Ready();
            else baYeChestButtons[i].Open();
        }
    }

    //main场景羁绊内容的初始化 
    private void InitJiBanForMainFun()
    {
        foreach (var jiBan in DataTable.JiBan.Values)
        {
            if (jiBan.IsOpen == 0) continue;
            Transform tran = jibanBtnBoxTran.GetChild(jiBan.Id);
            if (tran != null)
            {
                tran.GetChild(0).GetChild(0).GetComponent<Image>().sprite =
                    Resources.Load("Image/JiBan/name_v/" + jiBan.Id, typeof(Sprite)) as Sprite;
                tran.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
                    ShowJiBanInfoOnClick(jiBan.Id));
                tran.gameObject.SetActive(true);
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

        var jiBan = DataTable.JiBan[indexId];
        for (int i = 0; i < jiBan.Cards.Length; i++)
        {
            var card = jiBan.Cards[i];
            var heroType = (int) GameCardType.Hero;
            if (card.CardType == heroType)
            {
                var hero = DataTable.Hero[card.CardId];
                Transform tran = jibanHeroBoxTran.GetChild(i);
                GameObject obj = tran.GetChild(0).gameObject;
                //名字 
                ShowNameTextRules(obj.transform.GetChild(2).GetComponent<Text>(), hero.Name);
                //名字颜色根据稀有度 
                obj.transform.GetChild(2).GetComponent<Text>().color = NameColorChoose(hero.Rarity);
                //卡牌 
                obj.transform.GetChild(1).GetComponent<Image>().sprite =
                    GameResources.HeroImg[hero.Id];
                //兵种名 
                obj.transform.GetChild(4).GetComponentInChildren<Text>().text =
                    DataTable.Military[hero.MilitaryUnitTableId].Short;
                //兵种框 
                obj.transform.GetChild(4).GetComponent<Image>().sprite = GameResources.ClassImg[0];
                tran.gameObject.SetActive(true);
            }
        }

        jiBanInfoConObj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load("Image/JiBan/art/" + indexId, typeof(Sprite)) as Sprite;
        jiBanInfoConObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = DataTable.JiBan[indexId].JiBanEffect;
        jiBanInfoConObj.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = Resources.Load("Image/JiBan/name_h/" + indexId, typeof(Sprite)) as Sprite;


        jiBanBtnsConObj.SetActive(false);
        jiBanInfoConObj.SetActive(true);
        jiBanWinCloseBtn.onClick.RemoveAllListeners();
        jiBanWinCloseBtn.onClick.AddListener(delegate ()
        {
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
            AddStamina(PlayerDataForGame.instance.getBackTiLiNums);
            PlayerDataForGame.instance.ShowStringTips(string.Format(DataTable.GetStringText(25), PlayerDataForGame.instance.getBackTiLiNums));
        }
        PlayerDataForGame.instance.lastSenceIndex = 1;
        PlayerDataForGame.instance.getBackTiLiNums = 0;
    }

    public void GetBaYeProgressReward(int index)
    {
        var baYe = PlayerDataForGame.instance.warsData.baYe;
        if (baYe.CurrentExp < DataTable.BaYeTask[index].Exp)
        {
            PlayerDataForGame.instance.ShowStringTips("当前经验不足以领取！");
            return;
        }

        var btnIndex = index - 1;
        var isOpen = baYe.openedChest[btnIndex];
        if (isOpen)
        {
            PlayerDataForGame.instance.ShowStringTips("该奖励已经领取了噢！");
            baYeChestButtons[btnIndex].Open();
            return;
        }

        var warChestId = DataTable.BaYeTask[index].WarChestTableId;
        ApiPanel.instance.Invoke(bag => WarChestRecallAction(bag),
            PlayerDataForGame.instance.ShowStringTips,
            EventStrings.Req_WarChest,
            ViewBag.Instance().SetValues(warChestId, 3));
        baYeChestButtons[btnIndex].Open();
        PlayerDataForGame.instance.warsData.baYe.openedChest[btnIndex] = true;
        PlayerDataForGame.instance.isNeedSaveData = true;
        AudioController0.instance.ForcePlayAudio(0);
    }

    public PlayerDataDto WarChestRecallAction(ViewBag bag)
    {
        var warChest = bag.GetWarChest();
        var player = bag.GetPlayerDataDto();
        var rewards = new List<RewardsCardClass>();
        foreach (var chestCard in warChest.Cards)
        {
            var type = (GameCardType) chestCard.Key;
            chestCard.Value.ForEach(c =>
            {
                var card = new RewardsCardClass
                {
                    cardId = c[0],
                    cardChips = c[1],
                    cardType = chestCard.Key
                };
                RewardManager.instance.RewardCard(type, card.cardId, card.cardChips); //获取卡牌
                rewards.Add(card);
            });
        }

        ShowRewardsThings(warChest.YuanBao, warChest.YvQue, warChest.Exp, 0, rewards, 1.5f); //显示奖励窗口
        ConsumeManager.instance.SaveChangeUpdatePlayerData(player, 0);
        return player;
    }

    int showTiLiNums = 0;

    //刷新体力相关的内容显示 
    public void UpdateShowTiLiInfo(string recordStr)
    {
        if (recordStr != tiLiRecordTimer.text)
        {
            tiLiRecordTimer.text = recordStr;
        }

        int nowStaminaNums = PlayerDataForGame.instance.pyData.Stamina;
        if (showTiLiNums != nowStaminaNums)
        {
            showTiLiNums = nowStaminaNums;
            tiLiNumText.text = nowStaminaNums + "/90";
        }
    }

    /// <summary> 
    /// 开始对战 
    /// </summary> 
    [SkipRename]public void OnClickStartExpedition()
    {
        if (PlayerDataForGame.instance.selectedWarId < 0 || expedition.RecordedExpeditionWarId < 0)
        {
            PlayerDataForGame.instance.ShowStringTips("请选择战役！");
            PlayOnClickMusic();
            return;
        }

        if (expedition.warForceSelectorUi.Data.Values.All(ui => !ui.Selected))
        {
            PlayerDataForGame.instance.ShowStringTips("请选择军团！");
            PlayOnClickMusic();
            return;
        }
        PlayerDataForGame.instance.WarType = PlayerDataForGame.WarTypes.Expedition;
        if (!IsJumping)
        {
            var staminaMap = expedition.SelectedWarStaminaCost;
            int staminaCost = staminaMap.Cost;
            if (PlayerDataForGame.instance.pyData.Stamina >= staminaCost)
            {
                ShowOrHideGuideObj(3, false);
                IsJumping = true;
                AudioController0.instance.ChangeAudioClip(12);
                AudioController0.instance.PlayAudioSource(0);
                TimeSystemControl.instance.LetTiLiTimerTake(staminaCost);
                PlayerDataForGame.instance.AddStamina(-staminaCost);
                showTiLiNums = PlayerDataForGame.instance.pyData.Stamina;
                tiLiNumText.text = showTiLiNums + "/90";
                cutTiLiTextObj.SetActive(false);
                cutTiLiTextObj.GetComponent<Text>().color = ColorDataStatic.name_red;
                cutTiLiTextObj.GetComponent<Text>().text = "-" + staminaCost;
                cutTiLiTextObj.SetActive(true);

                PlayerDataForGame.instance.getBackTiLiNums = staminaMap.MaxReturn;
                PlayerDataForGame.instance.boxForTiLiNums = staminaMap.CostOfChest;
                StartCoroutine(LateGoToFightScene());
            }
            else
            {
                PlayOnClickMusic();
                PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(27));
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
        if (PlayerDataForGame.instance.isJumping) yield break;
        PlayerDataForGame.instance.SendTroopToWarApi();
        yield return new WaitForSeconds(1f);
        PlayerDataForGame.instance.JumpSceneFun(2, false, () => PlayerDataForGame.instance.WarReward != null);
    }

    //刷新上阵数量的显示 
    private void UpdateCardNumsShow()
    {
        cardsListTitle.text = "出战";
        cardsNumsTitle.text = PlayerDataForGame.instance.TotalCardsEnlisted + "/" + DataTable.PlayerLevelConfig[PlayerDataForGame.instance.pyData.Level].CardLimit;
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
        AudioController0.instance.RandomPlayGuZhengAudio();//播放随机音效 

        indexChooseListForceId++;
        if (indexChooseListForceId > DataTable.PlayerLevelConfig[PlayerDataForGame.instance.pyData.Level].UnlockForces)
        {
            indexChooseListForceId = 0;
        }
        changeCardsListBtn.sprite = Resources.Load("Image/shiLi/Flag/" + indexChooseListForceId, typeof(Sprite)) as Sprite;
        changeCardsListNameImg.sprite = Resources.Load("Image/shiLi/Name/" + indexChooseListForceId, typeof(Sprite)) as Sprite;
        RefreshCardList();
        UpdateCardNumsShow();
        StartCoroutine(LateToChangeViewShow(0));
    }

    /// <summary> 
    /// 延时刷新列表置顶 
    /// </summary> 
    IEnumerator LateToChangeViewShow(float startTime)
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
        //zhuChengHeroContentObj.transform.parent.parent.GetComponent<ScrollRect>().listView.localPosition = Vector2.left; 
    }

    /// <summary> 
    /// 显示单个辅助 
    /// </summary> 
    private void GenerateFuZhuUi(NowLevelAndHadChip card)
    {
        var info = card.GetInfo();
        GameObject obj = GetHeroCardFromPool();
        //名字 
        ShowNameTextRules(obj.transform.GetChild(3).GetComponent<Text>(), info.Name);
        //名字颜色根据稀有度 
        obj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(info.Rare);
        //卡牌 
        obj.transform.GetChild(1).GetComponent<Image>().sprite = GameResources.FuZhuImg[info.ImageId];
        //兵种框 
        obj.transform.GetChild(5).GetComponent<Image>().sprite = GameResources.ClassImg[1];
        //兵种名 
        obj.transform.GetChild(5).GetComponentInChildren<Text>().text = info.Short;
        //边框 
        FrameChoose(info.Rare, obj.transform.GetChild(6).GetComponent<Image>());
        //碎片 
        if (card.level < DataTable.CardLevel.Keys.Max())
        {
            var consume = DataTable.CardLevel[card.level + 1].ChipsConsume;
            obj.transform.GetChild(2).GetComponent<Text>().text = card.chips + "/" + consume;
            obj.transform.GetChild(2).GetComponent<Text>().color = card.chips >= consume ? ColorDataStatic.deep_green : Color.white;
        }
        else
        {
            obj.transform.GetChild(2).GetComponent<Text>().text = "";
        }
        if (card.level > 0)
        {
            obj.transform.GetChild(4).GetComponent<Image>().enabled = true;
            //设置星级展示 
            obj.transform.GetChild(4).GetComponent<Image>().sprite = GameResources.GradeImg[card.level];
            obj.transform.GetChild(8).gameObject.SetActive(false);
            //出战标记 
            if (card.isFight > 0)
            {
                PlayerDataForGame.instance.EnlistCard(card, true);
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
            OnClickFuZhuCardFun(info, card, obj.transform.GetChild(9).GetComponent<Image>());
        });
    }

    /// <summary> 
    /// 点击辅助卡牌的方法 
    /// </summary> 
    private void OnClickFuZhuCardFun(GameCardInfo info,NowLevelAndHadChip card, Image selectImg)
    {
        PlayOnClickMusic();

        //名字 
        infoTran.GetChild(0).GetComponent<Text>().text = info.Name;
        //名字颜色 
        infoTran.GetChild(0).GetComponent<Text>().color = NameColorChoose(info.Rare);
        //属性 为空 
        infoTran.GetChild(1).GetComponent<Text>().text = "";
        infoTran.GetChild(2).GetComponent<Text>().text = "";
        //介绍 
        infoTran.GetChild(3).GetComponent<Text>().text = info.Intro;
        //名字 
        ShowNameTextRules(showCardObj.transform.GetChild(3).GetComponent<Text>(), info.Name);
        //名字颜色 
        showCardObj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(info.Rare);
        //卡牌 
        showCardObj.transform.GetChild(1).GetComponent<Image>().sprite = GameResources.FuZhuImg[info.ImageId];
        //兵种框 
        showCardObj.transform.GetChild(5).GetComponent<Image>().sprite = GameResources.ClassImg[1];
        //兵种名 
        showCardObj.transform.GetChild(5).GetComponentInChildren<Text>().text = info.Short;
        //边框 
        FrameChoose(info.Rare, showCardObj.transform.GetChild(6).GetComponent<Image>());
        //碎片 
        if (card.level < DataTable.CardLevel.Keys.Max())
        {
            var consume = DataTable.CardLevel[card.level + 1].ChipsConsume;
            showCardObj.transform.GetChild(2).GetComponent<Text>().text = card.chips + "/" + consume;
            showCardObj.transform.GetChild(2).GetComponent<Text>().color =
                card.chips >= consume ? ColorDataStatic.deep_green : Color.black;
        }
        else
        {
            showCardObj.transform.GetChild(2).GetComponent<Text>().text = "";
        }

        int goldPrice = GetGoldPrice(card);
        sellCardBtn.transform.GetChild(0).GetComponent<Text>().text = goldPrice.ToString();
        sellCardBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        sellCardBtn.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            OnClickForSellCard(card);
        });
        sellCardBtn.SetActive(true);

        if (card.level > 0)
        {
            showCardObj.transform.GetChild(4).GetComponent<Image>().enabled = true;
            //设置星级展示 
            showCardObj.transform.GetChild(4).GetComponent<Image>().sprite = GameResources.GradeImg[card.level];
            //出战相关设置 
            holdOrFightBtn.SetActive(true);
            if (card.isFight > 0)
            {
                showCardObj.transform.GetChild(7).gameObject.SetActive(true);
                holdOrFightBtn.GetComponentInChildren<Text>().text = DataTable.GetStringText(30);
            }
            else
            {
                showCardObj.transform.GetChild(7).gameObject.SetActive(false);
                holdOrFightBtn.GetComponentInChildren<Text>().text = DataTable.GetStringText(31);
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

        selectCardData = card;

        CalculatedNeedYuanBao(card.level);
    }

    /// <summary> 
    /// 创建并展示单位列表 
    /// </summary> 
    private void RefreshCardList()
    {
        TakeBackHeroCardPooling();
        var total = 0;
        SortHSTData(PlayerDataForGame.instance.hstData.heroSaveData);   //  排序 
        SortHSTData(PlayerDataForGame.instance.hstData.towerSaveData);
        SortHSTData(PlayerDataForGame.instance.hstData.trapSaveData);
        PlayerDataForGame.instance.RefreshEnlisted(indexChooseListForceId);
        PlayerDataForGame.instance.hstData.heroSaveData.Concat(PlayerDataForGame.instance.hstData.towerSaveData).Concat(PlayerDataForGame.instance.hstData.trapSaveData).Select(c=>new{Card=c,Info=c.GetInfo()}).Where(c=>c.Info.ForceId == indexChooseListForceId).ToList().ForEach(c=>
        {
            if (c.Info.Type == GameCardType.Hero)
                GenerateHeroUi(c.Card);
            else GenerateFuZhuUi(c.Card);
            total++;
        });

        if (total <= 0)
        {
            ShowOrHideInfo(false);
            return;
        }

        StartCoroutine(LiteUpdateListChooseFirst(0));
        ShowOrHideInfo(true);
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
    /// <param name="card"></param> 
    private void GenerateHeroUi(NowLevelAndHadChip card)
    {
        var info = card.GetInfo();
        GameObject obj = GetHeroCardFromPool();
        //名字 
        ShowNameTextRules(obj.transform.GetChild(3).GetComponent<Text>(), info.Name);
        //名字颜色根据稀有度 
        obj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(info.Rare);
        //卡牌 
        obj.transform.GetChild(1).GetComponent<Image>().sprite = GameResources.HeroImg[info.Id];
        //兵种名 
        obj.transform.GetChild(5).GetComponentInChildren<Text>().text = info.Short;
        //兵种框 
        obj.transform.GetChild(5).GetComponent<Image>().sprite = GameResources.ClassImg[0];
        //边框 
        FrameChoose(info.Rare, obj.transform.GetChild(6).GetComponent<Image>());
        //碎片 
        if (card.level < DataTable.CardLevel.Keys.Max())
        {
            var chipsConsume = DataTable.CardLevel[card.level + 1].ChipsConsume;
            obj.transform.GetChild(2).GetComponent<Text>().text = card.chips + "/" + chipsConsume;
            obj.transform.GetChild(2).GetComponent<Text>().color =
                card.chips >= chipsConsume ? ColorDataStatic.deep_green : Color.white;
        }
        else
        {
            obj.transform.GetChild(2).GetComponent<Text>().text = "";
        }
        if (card.level > 0)
        {
            obj.transform.GetChild(4).GetComponent<Image>().enabled = true;
            obj.transform.GetChild(8).gameObject.SetActive(false);
            //设置星级展示 
            obj.transform.GetChild(4).GetComponent<Image>().sprite = GameResources.GradeImg[card.level];
            obj.transform.GetChild(7).gameObject.SetActive(false);
            if (card.isFight > 0) //出战标记 
            {
                PlayerDataForGame.instance.EnlistCard(card, true);
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
            OnClickHeroCardFun(card, obj.transform.GetChild(9).GetComponent<Image>());
        });
    }

    /// <summary> 
    /// 点击武将卡牌的方法 
    /// </summary> 
    /// <param name="card"></param> 
    private void OnClickHeroCardFun(NowLevelAndHadChip card, Image selectImg)
    {
        PlayOnClickMusic();
        var info = card.GetInfo();
        //Debug.Log("点击的武将id：" + heroData.id); 
        //武将名字 
        infoTran.GetChild(0).GetComponent<Text>().text = info.Name;
        //武将名字颜色 
        infoTran.GetChild(0).GetComponent<Text>().color = NameColorChoose(info.Rare);
        //武将属性 
        var damages = DataTable.Hero[card.id].Damages;
        infoTran.GetChild(1).GetComponent<Text>().text = string.Format(DataTable.GetStringText(32), damages[card.level > 0 ? card.level - 1 : 0]);
        var hps = DataTable.Hero[card.id].Hps;
        infoTran.GetChild(2).GetComponent<Text>().text = string.Format(DataTable.GetStringText(33), hps[card.level > 0 ? card.level - 1 : 0]);
        //武将介绍 
        infoTran.GetChild(3).GetComponent<Text>().text = info.Intro;

        //名字 
        ShowNameTextRules(showCardObj.transform.GetChild(3).GetComponent<Text>(), info.Name);
        //名字颜色 
        showCardObj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(info.Rare);
        //卡牌 
        showCardObj.transform.GetChild(1).GetComponent<Image>().sprite = GameResources.HeroImg[card.id];
        //兵种名 
        showCardObj.transform.GetChild(5).GetComponentInChildren<Text>().text = info.Short;
        //兵种框 
        showCardObj.transform.GetChild(5).GetComponent<Image>().sprite = GameResources.ClassImg[0];
        //边框 
        FrameChoose(info.Rare, showCardObj.transform.GetChild(6).GetComponent<Image>());
        //碎片 
        if (card.level < DataTable.CardLevel.Keys.Max())
        {
            var consumeChips = DataTable.CardLevel[card.level + 1].ChipsConsume;
            showCardObj.transform.GetChild(2).GetComponent<Text>().text = card.chips + "/" + consumeChips;
            showCardObj.transform.GetChild(2).GetComponent<Text>().color = card.chips >= consumeChips ? ColorDataStatic.deep_green : Color.black;
        }
        else
        {
            showCardObj.transform.GetChild(2).GetComponent<Text>().text = "";
        }

        int goldPrice = GetGoldPrice(card);
        sellCardBtn.transform.GetChild(0).GetComponent<Text>().text = goldPrice.ToString();
        sellCardBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        sellCardBtn.GetComponent<Button>().onClick.AddListener(() => OnClickForSellCard(card));
        sellCardBtn.SetActive(true);

        if (card.level > 0)
        {
            showCardObj.transform.GetChild(4).GetComponent<Image>().enabled = true;
            //设置星级展示 
            showCardObj.transform.GetChild(4).GetComponent<Image>().sprite = GameResources.GradeImg[card.level];
            //出战相关设置 
            holdOrFightBtn.SetActive(true);
            if (card.isFight > 0)
            {
                showCardObj.transform.GetChild(7).gameObject.SetActive(true);
                holdOrFightBtn.GetComponentInChildren<Text>().text = DataTable.GetStringText(30);
            }
            else
            {
                showCardObj.transform.GetChild(7).gameObject.SetActive(false);
                holdOrFightBtn.GetComponentInChildren<Text>().text = DataTable.GetStringText(31);
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

        selectCardData = card;
        
        CalculatedNeedYuanBao(card.level);
    }

    //出售卡牌可得金币 
    private int GetGoldPrice(NowLevelAndHadChip heroData)
    {
        var info = heroData.GetInfo();
        int chips = heroData.chips + DataTable.CardLevel.Where(lv => lv.Key <= heroData.level).Sum(kv => kv.Value.ChipsConsume);
        int golds = 0;
        switch (info.Rare)
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
    private void OnClickForSellCard(NowLevelAndHadChip gameCard)
    {
        AudioController0.instance.ChangeAudioClip(18);
        AudioController0.instance.PlayAudioSource(0);
        queRenWindows.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
        queRenWindows.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
        {
            ApiPanel.instance.Invoke(vb =>
                {
                    var player = vb.GetPlayerDataDto();
                    var gameCards = vb.GetPlayerGameCardDtos();
                    var troops = vb.GetPlayerTroopDtos();
                    ConsumeManager.instance.SaveUpdatePlayerTroops(player, troops, gameCards);
                    AudioController0.instance.ChangeAudioClip(17);
                    AudioController0.instance.PlayAudioSource(0);
                    RefreshList();
                }, msg =>
                {
                    PlayerDataForGame.instance.ShowStringTips(msg);
                    RefreshList();
                },
                EventStrings.Req_CardSell,
                ViewBag.Instance().SetValues(new object[] {gameCard.id, gameCard.typeIndex}));
        });
        queRenWindows.SetActive(true);

        //刷新主城列表 
        void RefreshList()
        {
            gameCard.chips = 0;
            gameCard.level = 0;
            gameCard.isFight = 0;
            ChangeScrollView();
            PlayerDataForGame.instance.EnlistCard(gameCard, false);
            UpdateCardNumsShow();
            queRenWindows.SetActive(false);
        }
    }

    /// <summary> 
    /// 匹配稀有度的颜色 
    /// </summary> 
    /// <returns></returns> 
    public Color NameColorChoose(int rarity)
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

    // <summary> 
    /// 匹配稀有度边框 
    /// </summary> 
    public void FrameChoose(int rarity, Image img)
    {
        img.enabled = false;//暂时不提供边框 
        return;
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

    /// <summary> 
    /// 对HST的数据进行排序 
    /// </summary> 
    private void SortHSTData(List<NowLevelAndHadChip> dataList)
    {
        dataList.Sort((c1, c2) =>
        {
            if (c2.isFight.CompareTo(c1.isFight) != 0)
            {
                return c2.isFight.CompareTo(c1.isFight);
            }

            if (c2.level.CompareTo(c1.level) != 0)
            {
                return c2.level.CompareTo(c1.level);
            }

            return GetRare(c2).CompareTo(GetRare(c1));
        });

        int GetRare(NowLevelAndHadChip c)
        {
            switch ((GameCardType)c.typeIndex)
            {
                case GameCardType.Hero:
                    return DataTable.Hero[c.id].Rarity;
                case GameCardType.Tower:
                    return DataTable.Tower[c.id].Rarity;
                case GameCardType.Trap:
                    return DataTable.Trap[c.id].Rarity;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    /// <summary> 
    /// 初始化玩家信息显示 
    /// </summary> 
    public void InitializationPlayerInfo()
    {
        RefreshPlayerInfoUi();
        RefreshCardList();
        UpdateCardNumsShow();

        StartCoroutine(LateToChangeViewShow(0));
    }

    public void RefreshPlayerInfoUi()
    {
        //player`s name 
        playerInfoObj.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = DataTable.PlayerInitialConfig[PlayerDataForGame.instance.pyData.ForceId].Force;
        if (PlayerDataForGame.instance.pyData.Level >= DataTable.PlayerLevelConfig.Keys.Max())
        {
            playerInfoObj.transform.GetChild(0).GetComponent<Slider>().value = 1;
            playerInfoObj.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = DataTable.GetStringText(34);
            playerInfoObj.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = PlayerDataForGame.instance.pyData.Exp + "/" + 99999;
        }
        else
        {
            //Exp 
            playerInfoObj.transform.GetChild(0).GetComponent<Slider>().value = 1f * PlayerDataForGame.instance.pyData.Exp / DataTable.PlayerLevelConfig[PlayerDataForGame.instance.pyData.Level + 1].Exp;
            //Level 
            playerInfoObj.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = string.Format(DataTable.GetStringText(35), PlayerDataForGame.instance.pyData.Level);//玩家等级 
            playerInfoObj.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = PlayerDataForGame.instance.pyData.Exp + "/" + DataTable.PlayerLevelConfig[PlayerDataForGame.instance.pyData.Level + 1].Exp;
        }
        //货币 
        yuanBaoNumText.text = PlayerDataForGame.instance.pyData.YuanBao.ToString();
        yvQueNumText.text = PlayerDataForGame.instance.pyData.YvQue.ToString();
        showTiLiNums = PlayerDataForGame.instance.pyData.Stamina;
        tiLiNumText.text = showTiLiNums + "/90";
    }

    //得到合成所需元宝 
    private void CalculatedNeedYuanBao(int nowLevel)
    {
        if (nowLevel == 0) ShowOrHideGuideObj(2, true);
        heImgObj.SetActive(nowLevel == 0);
        if (nowLevel < DataTable.CardLevel.Keys.Max())
        {
            needYuanBaoNums = DataTable.CardLevel[nowLevel + 1].YuanBaoConsume;
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
        var nextLevel = DataTable.CardLevel[selectCardData.level + 1];
        var isChipsEnough = selectCardData.chips >= nextLevel.ChipsConsume;
        var isYanBaoEnough = PlayerDataForGame.instance.pyData.YuanBao >= nextLevel.YuanBaoConsume;

        if (!isChipsEnough || !isYanBaoEnough || !ConsumeManager.instance.DeductYuanBao(nextLevel.YuanBaoConsume))
        {
            PlayerDataForGame.instance.ShowStringTips(isYanBaoEnough
                ? DataTable.GetStringText(36)
                : DataTable.GetStringText(37));
            PlayOnClickMusic();
            return;
        }

        ApiPanel.instance.Invoke(vb =>
            {
                var player = vb.GetPlayerDataDto();
                var dto = vb.GetGameCardDto();
                NowLevelAndHadChip card;
                var hst = PlayerDataForGame.instance.hstData;
                switch (dto.Type)
                {
                    case GameCardType.Hero:
                        card = hst.heroSaveData.First(c => c.id == dto.CardId);
                        break;
                    case GameCardType.Tower:
                        card = hst.towerSaveData.First(c => c.id == dto.CardId);
                        break;
                    case GameCardType.Trap:
                        card = hst.trapSaveData.First(c => c.id == dto.CardId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                card.chips = dto.Chips;
                card.level = dto.Level;
                ConsumeManager.instance.SaveChangeUpdatePlayerData(player, 7);
                StartCoroutine(PlayCardUpgradeEffect());
                AudioController0.instance.ChangeAudioClip(16);
                AudioController0.instance.PlayAudioSource(0);
                UpdateLevelCardUi();
                ShowOrHideGuideObj(2, false);
            }, PlayerDataForGame.instance.ShowStringTips,
            EventStrings.Req_CardMerge,
            ViewBag.Instance().SetValues(new object[] {selectCardData.id, selectCardData.typeIndex}));
    }

    //隐藏升星特效 
    IEnumerator PlayCardUpgradeEffect()
    {
        upStarEffectObj.SetActive(false);
        upStarEffectObj.SetActive(true);
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
            if (PlayerDataForGame.instance.EnlistCard(selectCardData, false))
            {
                listCard.GetChild(7).gameObject.SetActive(false);
                showCardObj.transform.GetChild(7).gameObject.SetActive(false);
                holdOrFightBtn.GetComponentInChildren<Text>().text = DataTable.GetStringText(31);
                selectCardData.isFight = 0;
                AudioController0.instance.ChangeAudioClip(15);
                AudioController0.instance.PlayAudioSource(0);
            }
            else
            {
                PlayOnClickMusic();
            }
        }
        else
        {
            if (PlayerDataForGame.instance.EnlistCard(selectCardData, true))
            {
                listCard.GetChild(7).gameObject.SetActive(true);
                showCardObj.transform.GetChild(7).gameObject.SetActive(true);
                holdOrFightBtn.GetComponentInChildren<Text>().text = DataTable.GetStringText(30);
                selectCardData.isFight = 1;
                AudioController0.instance.ChangeAudioClip(14);
                AudioController0.instance.PlayAudioSource(0);
            }
            else
            {
                PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(38));

                PlayOnClickMusic();
            }
        }
        UpdateCardNumsShow();
        //LoadSaveData.instance.SaveByJson(PlayerDataForGame.instance.hstData); 
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(2);
    }

    //升级卡牌后更新显示 
    private void UpdateLevelCardUi()
    {
        //Debug.Log("selectCardData.Level: " + selectCardData.Level); 
        Transform listCard = lastSelectImg.transform.parent;
        if (selectCardData.level < DataTable.CardLevel.Keys.Max())
        {
            var consume = DataTable.CardLevel[selectCardData.level + 1].ChipsConsume;
            listCard.GetChild(2).GetComponent<Text>().text = selectCardData.chips + "/" + consume;
            listCard.GetChild(2).GetComponent<Text>().color =
                selectCardData.chips >= consume ? ColorDataStatic.deep_green : Color.white;
        }
        else
        {
            listCard.GetChild(2).GetComponent<Text>().text = "";
        }
        listCard.GetChild(4).GetComponent<Image>().enabled = true;
        //设置星级展示 
        listCard.GetChild(4).GetComponent<Image>().sprite = GameResources.GradeImg[selectCardData.level];
        listCard.GetChild(8).gameObject.SetActive(false);
        listCard.GetComponent<Button>().onClick.Invoke();
    }

    /// <summary> 
    /// 展示奖励 
    /// </summary> 
    /// <param name="yuanBao">元宝</param> 
    /// <param name="yvQue">玉阙</param> 
    /// <param name="exp">经验</param> 
    /// <param name="stamina">体力</param> 
    /// <param name="rewardsCards">卡牌奖励</param> 
    /// <param name="waitTime">展示等待时间</param> 
    public void ShowRewardsThings(int yuanBao, int yvQue, int exp, int stamina, List<RewardsCardClass> rewardsCards, float waitTime)
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
        if (yuanBao > 0)
        {
            ShowOneReward(0, new RewardsCardClass() { cardChips = yuanBao });
        }
        if (yvQue > 0)
        {
            ShowOneReward(1, new RewardsCardClass() { cardChips = yvQue });
        }
        if (exp > 0)
        {
            ShowOneReward(2, new RewardsCardClass() { cardChips = exp });
        }
        if (stamina > 0)
        {
            ShowOneReward(3, new RewardsCardClass() { cardChips = stamina });
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
        taoYuan.CloseAllChests();
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
    /// <param name="rewardType">0元宝,1玉阙,2经验,3体力,4卡牌</param> 
    /// <param name="rewardsCard"></param> 
    private void ShowOneReward(int rewardType, RewardsCardClass rewardsCard)
    {
        if (rewardsCard.cardChips <= 0)
        {
            return;
        }

        GameObject obj = FindShowRewardsBox();
        var info = new NowLevelAndHadChip().Instance((GameCardType)rewardsCard.cardType,rewardsCard.cardId, 0).GetInfo();
        obj.transform.GetChild(rewardType).gameObject.SetActive(true);
        if (rewardType == 4)
        {
            Transform cardTran = obj.transform.GetChild(4);
            cardTran.GetComponent<Image>().sprite = info.Type == GameCardType.Hero ? GameResources.HeroImg[info.Id] : GameResources.FuZhuImg[info.ImageId];
            ShowNameTextRules(cardTran.GetChild(0).GetComponent<Text>(), info.Name);
            cardTran.GetChild(0).GetComponent<Text>().color = NameColorChoose(info.Rare);
            cardTran.GetChild(1).GetComponent<Image>().sprite = info.Type == GameCardType.Hero ? GameResources.ClassImg[0] : GameResources.ClassImg[1];
            cardTran.GetChild(1).GetChild(0).GetComponentInChildren<Text>().text = info.Short;
            FrameChoose(info.Rare, cardTran.GetChild(2).GetComponent<Image>());
        }

        obj.transform.GetChild(5).GetComponent<Text>().text = "×" + rewardsCard.cardChips;
    }

    /// <summary> 
    /// 主城界面切换 
    /// </summary> 
    /// <param name="index"></param> 
    public void MainPageSwitching(int index)
    {
        if (IsJumping) return;
        currentPage = (Pages)index;
        PlayOnClickMusic();

        for (int i = 0; i < zhuChengInterFaces.Length; i++)
        {
            zhuChengInterFaces[i].SetActive(false);
            particlesForInterface[i].SetActive(false);
        }
        zhuChengInterFaces[index].SetActive(true);
        particlesForInterface[index].SetActive(true);
        //暂时未开启的页面 
        waitWhileImpress.gameObject.SetActive(
            index == 3 //对决 
        );
        switch (index)
        {
            case 0://桃园 
                ShowOrHideGuideObj(0, true);
                if (PlayerDataForGame.instance.gbocData.fightBoxs.Count > 0) ShowOrHideGuideObj(1, true);
                break;
            case 2://战役 
                ShowOrHideGuideObj(3, true);
                expedition.OnClickChangeWarsFun(expedition.RecordedExpeditionWarId);
                warsChooseListObj.transform.parent.parent.GetComponent<ScrollRect>().DOVerticalNormalizedPos(0f, 0.3f);
                break;
            case 4://霸业 
                bayeBelowLevelPanel.gameObject.SetActive(PlayerDataForGame.instance.pyData.Level < 5);
                if (!SystemTimer.IsToday(PlayerDataForGame.instance.warsData.baYe.lastBaYeActivityTime))
                {
                    BaYeManager.instance.Init();
                    InitBaYeFun();
                }
                if (BaYeManager.instance.isShowTips)
                {
                    PlayerDataForGame.instance.ShowStringTips(BaYeManager.instance.tipsText);
                    BaYeManager.instance.tipsText = string.Empty;
                    BaYeManager.instance.isShowTips = false;
                }
                break;
            case 1://主城 
                break;
            case 3://对决 
                PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(67));
                break;
            default:
                XDebug.LogError<UIManager>($"未知页面索引[{index}]。");
                throw new ArgumentOutOfRangeException();
        }
    }

    //显示或隐藏指引 
    public void ShowOrHideGuideObj(int index, bool isShow)
    {
        if (isShow)
        {
            if (PlayerDataForGame.instance.GuideObjsShowed[index] == 0)
            {
                guideObjs[index].SetActive(true);
            }
        }
        else
        {
            if (PlayerDataForGame.instance.GuideObjsShowed[index] == 0)
            {
                guideObjs[index].SetActive(false);
                PlayerDataForGame.instance.GuideObjsShowed[index] = 1;
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
        if (PlayerDataForGame.instance.pyData.Level >= DataTable.PlayerLevelConfig.Count)
        {
            PlayerDataForGame.instance.pyData.Exp += expNums;
            playerInfoObj.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = PlayerDataForGame.instance.pyData.Exp + "/" + 99999;
            PlayerDataForGame.instance.isNeedSaveData = true;
            LoadSaveData.instance.SaveGameData(1);
            return;
        }

        PlayerDataForGame.instance.pyData.Exp += expNums;
        while (DataTable.PlayerLevelConfig[PlayerDataForGame.instance.pyData.Level + 1].Exp <= PlayerDataForGame.instance.pyData.Exp)
        {
            PlayerDataForGame.instance.pyData.Exp -= DataTable.PlayerLevelConfig[PlayerDataForGame.instance.pyData.Level + 1].Exp;
            PlayerDataForGame.instance.pyData.Level++;
            PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(39));
            if (PlayerDataForGame.instance.pyData.Level == DataTable.PlayerLevelConfig.Keys.Max())
            {
                PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(40));
                break;
            }
        }

        if (PlayerDataForGame.instance.pyData.Level >= DataTable.PlayerLevelConfig.Keys.Max())
        {
            playerInfoObj.transform.GetChild(0).GetComponent<Slider>().value = 1;
            playerInfoObj.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = DataTable.GetStringText(34);
            playerInfoObj.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = PlayerDataForGame.instance.pyData.Exp + "/" + 99999;
        }
        else
        {
            playerInfoObj.transform.GetChild(0).GetComponent<Slider>().value = 1f * PlayerDataForGame.instance.pyData.Exp / DataTable.PlayerLevelConfig[PlayerDataForGame.instance.pyData.Level + 1].Exp;
            playerInfoObj.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = string.Format(DataTable.GetStringText(35), PlayerDataForGame.instance.pyData.Level);
            playerInfoObj.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = PlayerDataForGame.instance.pyData.Exp + "/" + DataTable.PlayerLevelConfig[PlayerDataForGame.instance.pyData.Level + 1].Exp;
        }
        UpdateCardNumsShow();
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
    private GameObject GetHeroCardFromPool()
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

    //添加体力 
    public void AddStamina(int addNums)
    {
        TimeSystemControl.instance.AddStamina(addNums);
    }

    //播放点击音效 
    public void PlayOnClickMusic()
    {
        AudioController0.instance.ChangeAudioClip(13);
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
            PlayOnClickMusic();
        }
        else
        {
            //关闭 
            PlayerPrefs.SetInt(LoadSaveData.instance.IsPlayMusicStr, 0);
            AudioController0.instance.isPlayMusic = 0;
            AudioController0.instance.audioSource.Pause();
            AudioController1.instance.audioSource.Pause();
            musicBtnText.text = DataTable.GetStringText(41);
        }
    }

    bool isJinNangReady = true;  //记录是否可以开启锦囊 

    //刷新锦囊入口的显示 
    public void UpdateShowJinNangBtn(bool isReady)
    {
        if (isJinNangReady == isReady) return;
        taoYuan.jinNangBtn.gameObject.SetActive(isReady);
        isJinNangReady = isReady;
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
        var code = rtInputField.text;

        if (string.IsNullOrWhiteSpace(code))
        {
            ShowMessage(45);
            return;
        }

        var rCode = DataTable.RCode.Values.FirstOrDefault(c => c.Code == code);
        if(rCode == null)
        {
            ShowMessage(49);
            return;
        }

        var now = TimeSystemControl.instance.SystemTimer.Now.LocalDateTime;
        if (!rCode.Lasting.IsTableTimeInRange(now))
        {
            ShowMessage(47);
            return;
        }

        var playerRecord = PlayerDataForGame.instance.gbocData.redemptionCodeGotList
            .FirstOrDefault(c => c.id == rCode.Id);
        var isRedeemed = playerRecord != null && playerRecord.isGot;

        if (isRedeemed)
        {
            ShowMessage(48);
            return;
        }

        ApiPanel.instance.Invoke(vb =>
            {
                var rC = vb.GetRCode();
                var py = vb.GetPlayerDataDto();
                OnSuccessRedeemed(rC, playerRecord, py);
            }, PlayerDataForGame.instance.ShowStringTips,
            EventStrings.Req_RCode, ViewBag.Instance().SetValue(rCode.Code));

        void ShowMessage(int textId)
        {
            PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(textId));
            rtInputField.text = string.Empty;
            PlayOnClickMusic();
        }
    }

    private void OnSuccessRedeemed(RCodeTable rCode, RedemptionCodeGot playerRecord,PlayerDataDto playerData)
    {
        var rewards = rCode.Cards.Select(c => new RewardsCardClass
            {cardId = c.CardId, cardChips = c.Chips, cardType = c.Type}).ToList();

        if (playerRecord == null)
        {
            playerRecord = new RedemptionCodeGot {id = rCode.Id};
            PlayerDataForGame.instance.gbocData.redemptionCodeGotList.Add(playerRecord);
        }

        playerRecord.isGot = true;
        ConsumeManager.instance.SaveChangeUpdatePlayerData(playerData, 0);

        rtInputField.text = "";
        PlayerDataForGame.instance.ShowStringTips(rCode.Info);
        rtCloseBtn.onClick.Invoke();
        AudioController0.instance.ChangeAudioClip(0);
        AudioController0.instance.PlayAudioSource(0);
        ShowRewardsThings(rCode.YuanBao, rCode.YuQue, 0, rCode.TiLi, rewards, 0);
    }

    ///////////////////////////鸡坛相关///////////////////////////////// 

    //给体力商店按钮添加方法 
    private void InitChickenBtnFun()
    {
        var chickenTables = DataTable.Chicken.Values.ToList();
        for (int i = 0; i < chickenShopBtns.Length; i++)
        {
            var chicken = chickenTables[i];
            chickenShopBtns[i].onClick.AddListener(() => ChickenShoppingGetTiLi(chicken.Id));
            //显示体力的数量 
            chickenShopBtns[i].transform.parent.GetChild(1).GetComponent<Text>().text = "×" + chicken.Stamina;
            //显示消耗玉阙的数量 
            if (i != 0)
            {
                chickenShopBtns[i].transform.GetChild(0).GetComponent<Text>().text = "×" + chicken.YuQueCost;
            }
        }

        chickenEntObj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate ()
        {
            AudioController0.instance.ChangeAudioClip(25);
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
        if (ConsumeManager.instance.DeductYuQue(quQueNums))
        {
            AddStamina(tiLiNums);
            return true;
        }
        else
        {
            return false;
        }
    }

    //商店购买体力 
    [Skip]
    private void ChickenShoppingGetTiLi(int chickenId)
    {
        AudioController0.instance.ChangeAudioClip(13);
        OpenOrCloseChickenBtn(false);
        if (chickenId == 1)
            AdAgent.instance.BusyRetry(InvokeApi
                , () =>
                {
                    PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(6));
                    OpenOrCloseChickenBtn(true);
                });
        else InvokeApi();

        void InvokeApi()
        {
            ApiPanel.instance.Invoke(bag =>
                {
                    var chicken = bag.GetChicken();
                    var player = bag.GetPlayerDataDto();
                    ConsumeManager.instance.SaveChangeUpdatePlayerData(player);
                    //GetTiLiForChicken(yuQueCost, stamina);
                    OnSuccessRequestChicken(chickenId == 2 ? 50 : 51, chicken.Stamina);
                }, msg =>
                {
                    PlayerDataForGame.instance.ShowStringTips(msg);
                    OpenOrCloseChickenBtn(true);
                }, EventStrings.Req_Chicken,
                ViewBag.Instance().SetValue(chickenId));
        }
    }

    private void OnSuccessRequestChicken(int textIndex, int stamina)
    {
        PlayerDataForGame.instance.ShowStringTips(string.Format(DataTable.GetStringText(textIndex), stamina));
        GetCkChangeTimeAndWindow();
        AudioController0.instance.ChangeAudioClip(25);
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

    /// <summary> 
    /// 存储游戏 
    /// </summary> 
    public void SaveGame()
    {
        LoadSaveData.instance.SaveGameData();
    }

    //退出游戏 
    public void ExitGame()
    {
        PlayOnClickMusic();

#if UNITY_ANDROID
        Application.Quit();
#endif
    }
}