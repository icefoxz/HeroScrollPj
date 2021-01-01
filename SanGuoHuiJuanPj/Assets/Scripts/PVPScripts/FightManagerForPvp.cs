using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FightManagerForPvp : MonoBehaviour
{
    public static FightManagerForPvp instance;
    
    [SerializeField]
    public Transform enemyCardsBox;     //敌方卡牌父级
    [SerializeField]
    public Transform playerCardsBox;    //我方卡牌父级
    [SerializeField]
    GameObject fightCardPre;            //敌方战斗卡牌预制件
    [SerializeField]
    GameObject fightCardPyPre;          //我方战斗卡牌预制件

    [SerializeField]
    GameObject homeCardObj;             //老巢卡牌预制件

    public Transform herosCardListTran; //我方卡牌备战位

    public List<FightCardData> playerCardsDatas; //我方备战位卡牌信息集合

    public List<GameObject> enemyCardsPos;  //敌方卡牌位置列表
    public List<GameObject> playerCardsPos; //我方卡牌位置列表

    public FightCardData[] enemyFightCardsDatas;    //敌方战斗卡牌信息集合
    public FightCardData[] playerFightCardsDatas;   //我方战斗卡牌信息集合

    [SerializeField]
    GridLayoutGroup gridLayoutGroup;    //棋盘Group
    [HideInInspector]
    public float floDisY;  //加成比
    [HideInInspector]
    public float oneDisY;  //半格高
    
    [SerializeField]
    GameObject guideStoryObj;   //剧情obj

    public Transform cardListTran;          //备战位总位
    public ScrollRect cardListScrollRect;   //备战位ScrollRect

    public AudioClip[] audioClipsFightEffect;
    public float[] audioVolumeFightEffect;

    public AudioClip[] audioClipsFightBack; //战斗背景音乐
    public float[] audioVolumeFightBack;
    
    [HideInInspector]
    public bool _isDragItem;    //记录是否有卡牌在拖动

    private int indexNowStoryId;    //当前剧情故事编号
    private int storyNums;          //剧情故事数量

    //卡牌附近单位遍历次序
    public int[][] CardNearbyAdditionForeach = new int[20][] {
        new int[3]{ 2, 3, 5},           //0
        new int[3]{ 2, 4, 6},           //1
        new int[5]{ 0, 1, 5, 6, 7},     //2
        new int[3]{ 0, 5, 8},           //3
        new int[3]{ 1, 6, 9},           //4
        new int[6]{ 0, 2, 3, 7, 8, 10}, //5
        new int[6]{ 1, 2, 4, 7, 9, 11}, //6
        new int[6]{ 2, 5, 6, 10,11,12}, //7
        new int[4]{ 3, 5, 10,13},       //8
        new int[4]{ 4, 6, 11,14},       //9
        new int[6]{ 5, 7, 8, 12,13,15}, //10
        new int[6]{ 6, 7, 9, 12,14,16}, //11
        new int[6]{ 7, 10,11,15,16,17}, //12
        new int[4]{ 8, 10,15,18},       //13
        new int[4]{ 9, 11,16,19},       //14
        new int[5]{ 10,12,13,17,18},    //15
        new int[5]{ 11,12,14,17,19},    //16
        new int[3]{ 12,15,16},          //17
        new int[2]{ 13,15},             //18
        new int[2]{ 14,16},             //19
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        playerCardsDatas = new List<FightCardData>();
        enemyFightCardsDatas = new FightCardData[20];
        playerFightCardsDatas = new FightCardData[20];

        Input.multiTouchEnabled = false;    //限制多指拖拽
        _isDragItem = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        oneDisY = Screen.height / (1920 / gridLayoutGroup.cellSize.y) / 9;
        float xFlo = (1920f / 1080f) / ((float)Screen.height / Screen.width);
        floDisY = 2 * oneDisY * xFlo;
        indexNowStoryId = 0;
        storyNums = LoadJsonFile.guideTableDatas.Count;
    }

    /// <summary>
    /// 初始化卡牌到战斗位上
    /// </summary>
    public void InitEnemyCardForFight()
    {
        guideStoryObj.transform.GetChild(1).gameObject.SetActive(true);
        guideStoryObj.transform.GetChild(2).gameObject.SetActive(true);
        FightControlForPvp.instance.InitStartFight();
        UpdateCardDataForStory();
    }

    /// <summary>
    /// 创建玩家家园卡牌
    /// </summary>
    private void CreatePlayerHomeCard()
    {
        FightCardData playerHomeData = new FightCardData();
        playerHomeData.cardObj = Instantiate(homeCardObj, playerCardsBox);
        playerHomeData.cardObj.transform.position = playerCardsPos[17].transform.position;
        playerHomeData.hpr = 0;
        playerHomeData.cardType = 522;
        playerHomeData.posIndex = 17;
        playerHomeData.isPlayerCard = true;
        playerHomeData.activeUnit = false;
        playerHomeData.fightState = new FightState();
        playerFightCardsDatas[17] = playerHomeData;
        playerFightCardsDatas[17].fullHp = playerFightCardsDatas[17].nowHp = int.Parse(LoadJsonFile.guideTableDatas[indexNowStoryId][8]);  //我方血量

        FightCardData enemyHomeData = new FightCardData();
        enemyHomeData.cardObj = Instantiate(homeCardObj, enemyCardsBox);
        enemyHomeData.cardObj.transform.position = enemyCardsPos[17].transform.position;
        enemyHomeData.hpr = 0;
        enemyHomeData.cardType = 522;
        enemyHomeData.posIndex = 17;
        enemyHomeData.isPlayerCard = false;
        enemyHomeData.activeUnit = false;
        enemyHomeData.fightState = new FightState();
        enemyFightCardsDatas[17] = enemyHomeData;
        enemyFightCardsDatas[17].fullHp = enemyFightCardsDatas[17].nowHp = int.Parse(LoadJsonFile.guideTableDatas[indexNowStoryId][9]);    //敌方血量
    }

    //更新故事战斗数据
    private void UpdateCardDataForStory()
    {
        //羁绊数据重置
        InitJiBanTypeListFun(FightControlForPvp.instance.playerJiBanAllTypes);
        InitJiBanTypeListFun(FightControlForPvp.instance.enemyJiBanAllTypes);

        if (playerFightCardsDatas[17] == null)
        {
            CreatePlayerHomeCard();
        }
        else
        {
            playerFightCardsDatas[17].cardObj.transform.GetChild(4).gameObject.SetActive(false);
            enemyFightCardsDatas[17].cardObj.transform.GetChild(4).gameObject.SetActive(false);

            playerFightCardsDatas[17].fullHp = playerFightCardsDatas[17].nowHp = int.Parse(LoadJsonFile.guideTableDatas[indexNowStoryId][8]);  //我方血量
            enemyFightCardsDatas[17].fullHp = enemyFightCardsDatas[17].nowHp = int.Parse(LoadJsonFile.guideTableDatas[indexNowStoryId][9]);    //敌方血量

            playerFightCardsDatas[17].cardObj.transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
            enemyFightCardsDatas[17].cardObj.transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
        }
        //清空备战位
        for (int i = 0; i < herosCardListTran.childCount; i++)
        {
            Destroy(herosCardListTran.GetChild(i).gameObject);
        }
        playerCardsDatas.Clear();
        //清空棋盘单位
        for (int i = 0; i < 20; i++)
        {
            if (i != 17)
            {
                if (playerFightCardsDatas[i] != null)
                {
                    Destroy(playerFightCardsDatas[i].cardObj);
                    playerFightCardsDatas[i] = null;
                }
                if (enemyFightCardsDatas[i] != null)
                {
                    Destroy(enemyFightCardsDatas[i].cardObj);
                    enemyFightCardsDatas[i] = null;
                }
            }
        }
        FightControlForPvp.instance.ClearEmTieQiCardList();

        //创建玩家备战单位
        for (int i = 3; i < 8; i++)
        {
            if (LoadJsonFile.guideTableDatas[indexNowStoryId][i] != "")
            {
                FightCardData data = CreateFightUnit(LoadJsonFile.guideTableDatas[indexNowStoryId][i], herosCardListTran, true);
                data.cardObj.GetComponent<CardDragForPvp>().posIndex = -1;
                data.cardObj.GetComponent<CardDragForPvp>().isFightCard = false;
                data.posIndex = -1;
                data.isPlayerCard = true;
                playerCardsDatas.Add(data);
            }
        }
        //创建玩家棋盘单位
        for (int i = 0; i < 20; i++)
        {
            if (LoadJsonFile.guideTableDatas[indexNowStoryId][i + 10] != "")
            {
                FightCardData data = CreateFightUnit(LoadJsonFile.guideTableDatas[indexNowStoryId][i + 10], playerCardsBox, true);
                data.cardObj.GetComponent<CardDragForPvp>().posIndex = i;
                data.cardObj.GetComponent<CardDragForPvp>().isFightCard = true;
                data.posIndex = i;
                data.isPlayerCard = true;
                data.cardObj.transform.position = playerCardsPos[i].transform.position;
                playerFightCardsDatas[i] = data;
                CardGoIntoBattleProcess(playerFightCardsDatas[i], i, playerFightCardsDatas, true);
            }
        }
        //创建敌人棋盘单位
        for (int i = 0; i < 20; i++)
        {
            if (LoadJsonFile.guideTableDatas[indexNowStoryId][i + 30] != "")
            {
                FightCardData data = CreateFightUnit(LoadJsonFile.guideTableDatas[indexNowStoryId][i + 30], enemyCardsBox, false);
                data.posIndex = i;
                data.isPlayerCard = false;
                data.cardObj.transform.position = enemyCardsPos[i].transform.position;
                enemyFightCardsDatas[i] = data;
                CardGoIntoBattleProcess(enemyFightCardsDatas[i], i, enemyFightCardsDatas, true);
            }
        }
    }

    //创建敌方卡牌战斗单位数据
    private FightCardData CreateFightUnit(string dataStr, Transform cardsBoxTran, bool isPlayerCard)
    {
        string[] arrs = dataStr.Split(',');

        FightCardData data = new FightCardData();
        data.cardObj = Instantiate(isPlayerCard ? fightCardPyPre : fightCardPre, cardsBoxTran);
        data.cardType = int.Parse(arrs[0]);
        data.cardId = int.Parse(arrs[1]);
        data.cardGrade = int.Parse(arrs[2]);
        data.fightState = new FightState();

        switch (data.cardType)
        {
            case 0:
                //兵种
                data.cardObj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/Hero/" + LoadJsonFile.heroTableDatas[data.cardId][16], typeof(Sprite)) as Sprite;
                //血量
                data.cardObj.transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
                //名字
                ShowNameTextRules(data.cardObj.transform.GetChild(3).GetComponent<Text>(), LoadJsonFile.heroTableDatas[data.cardId][1]);
                //名字颜色
                data.cardObj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.heroTableDatas[data.cardId][3]);
                //星级
                data.cardObj.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + data.cardGrade, typeof(Sprite)) as Sprite;
                //兵种
                data.cardObj.transform.GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[data.cardId][5])][3];
                //兵种框
                data.cardObj.transform.GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 0, typeof(Sprite)) as Sprite;
                //边框
                FrameChoose(LoadJsonFile.heroTableDatas[data.cardId][3], data.cardObj.transform.GetChild(6).GetComponent<Image>());

                data.damage = int.Parse(LoadJsonFile.heroTableDatas[data.cardId][7].Split(',')[data.cardGrade - 1]);
                data.hpr = int.Parse(LoadJsonFile.heroTableDatas[data.cardId][9]);
                data.fullHp = data.nowHp = int.Parse(LoadJsonFile.heroTableDatas[data.cardId][8].Split(',')[data.cardGrade - 1]);
                data.activeUnit = true;
                data.cardMoveType = int.Parse(LoadJsonFile.heroTableDatas[data.cardId][17]);
                data.cardDamageType = int.Parse(LoadJsonFile.heroTableDatas[data.cardId][18]);
                break;
            case 1:
                data.cardObj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.soldierTableDatas[data.cardId][13], typeof(Sprite)) as Sprite;
                data.cardObj.transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
                ShowNameTextRules(data.cardObj.transform.GetChild(3).GetComponent<Text>(), LoadJsonFile.soldierTableDatas[data.cardId][1]);
                //名字颜色
                data.cardObj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.soldierTableDatas[data.cardId][3]);
                data.cardObj.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + data.cardGrade, typeof(Sprite)) as Sprite;
                data.cardObj.transform.GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.soldierTableDatas[data.cardId][5])][3];
                //兵种框
                data.cardObj.transform.GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
                FrameChoose(LoadJsonFile.soldierTableDatas[data.cardId][3], data.cardObj.transform.GetChild(6).GetComponent<Image>());

                data.damage = int.Parse(LoadJsonFile.soldierTableDatas[data.cardId][6].Split(',')[data.cardGrade - 1]);
                data.hpr = int.Parse(LoadJsonFile.soldierTableDatas[data.cardId][8]);
                data.fullHp = data.nowHp = int.Parse(LoadJsonFile.soldierTableDatas[data.cardId][7].Split(',')[data.cardGrade - 1]);
                data.activeUnit = true;
                break;
            case 2:
                data.cardObj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.towerTableDatas[data.cardId][10], typeof(Sprite)) as Sprite;
                data.cardObj.transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
                ShowNameTextRules(data.cardObj.transform.GetChild(3).GetComponent<Text>(), LoadJsonFile.towerTableDatas[data.cardId][1]);
                //名字颜色
                data.cardObj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.towerTableDatas[data.cardId][3]);
                data.cardObj.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + data.cardGrade, typeof(Sprite)) as Sprite;
                data.cardObj.transform.GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.towerTableDatas[data.cardId][5];
                FrameChoose(LoadJsonFile.towerTableDatas[data.cardId][3], data.cardObj.transform.GetChild(6).GetComponent<Image>());
                //兵种框
                data.cardObj.transform.GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
                data.damage = int.Parse(LoadJsonFile.towerTableDatas[data.cardId][6].Split(',')[data.cardGrade - 1]);
                data.hpr = int.Parse(LoadJsonFile.towerTableDatas[data.cardId][8]);
                data.fullHp = data.nowHp = int.Parse(LoadJsonFile.towerTableDatas[data.cardId][7].Split(',')[data.cardGrade - 1]);
                data.activeUnit = (data.cardId == 0 || data.cardId == 1 || data.cardId == 2 || data.cardId == 3 || data.cardId == 6);
                data.cardMoveType = 1;
                data.cardDamageType = 0;
                break;
            case 3:
                data.cardObj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.trapTableDatas[data.cardId][8], typeof(Sprite)) as Sprite;
                data.cardObj.transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
                ShowNameTextRules(data.cardObj.transform.GetChild(3).GetComponent<Text>(), LoadJsonFile.trapTableDatas[data.cardId][1]);
                //名字颜色
                data.cardObj.transform.GetChild(3).GetComponent<Text>().color = NameColorChoose(LoadJsonFile.trapTableDatas[data.cardId][3]);
                data.cardObj.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + data.cardGrade, typeof(Sprite)) as Sprite;
                data.cardObj.transform.GetChild(5).GetComponentInChildren<Text>().text = LoadJsonFile.trapTableDatas[data.cardId][5];
                //兵种框
                data.cardObj.transform.GetChild(5).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
                FrameChoose(LoadJsonFile.trapTableDatas[data.cardId][3], data.cardObj.transform.GetChild(6).GetComponent<Image>());

                data.damage = int.Parse(LoadJsonFile.trapTableDatas[data.cardId][6].Split(',')[data.cardGrade - 1]);
                data.hpr = int.Parse(LoadJsonFile.trapTableDatas[data.cardId][10]);
                data.fullHp = data.nowHp = int.Parse(LoadJsonFile.trapTableDatas[data.cardId][7].Split(',')[data.cardGrade - 1]);
                data.activeUnit = false;
                data.cardMoveType = 0;
                data.cardDamageType = 0;
                break;
        }
        return data;
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
    /// 名字显示规则
    /// </summary>
    private void ShowNameTextRules(Text nameText, string str)
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

    //匹配稀有度边框
    private void FrameChoose(string rarity, Image img)
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

    //主动塔行动
    public void ActiveTowerFight(FightCardData cardData, FightCardData[] cardsDatas)
    {
        switch (cardData.cardId)
        {
            case 0://营寨
                YingZhaiFun(cardData, cardsDatas);
                break;
            case 1://投石台
                TouShiTaiAttackFun(cardData);
                break;
            case 2://奏乐台
                ZouYueTaiAddtionFun(cardData, cardsDatas);
                break;
            case 3://箭楼
                FightControlForPvp.instance.JianLouYuanSheSkill(cardData, GetTowerAddValue(cardData.cardId, cardData.cardGrade));
                break;
            case 6://轩辕台
                XuanYuanTaiAddtionFun(cardData, cardsDatas);
                break;
            default:
                break;
        }
    }

    //营寨行动
    private void YingZhaiFun(FightCardData cardData, FightCardData[] cardsDatas)
    {
        if (cardData.nowHp > 1)
        {
            FightControlForPvp.instance.indexAttackType = 0;
            FightControlForPvp.instance.PlayAudioForSecondClip(42, 0);

            int indexCutMaxHp = 0;  //最多扣除血量
            FightCardData needAddHpCard = new FightCardData();

            for (int i = 0; i < CardNearbyAdditionForeach[cardData.posIndex].Length; i++)
            {
                FightCardData addedFightCard = cardsDatas[CardNearbyAdditionForeach[cardData.posIndex][i]];
                if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
                {
                    if (addedFightCard.fullHp - addedFightCard.nowHp > indexCutMaxHp)
                    {
                        indexCutMaxHp = addedFightCard.fullHp - addedFightCard.nowHp;
                        needAddHpCard = addedFightCard;
                    }
                }
            }

            if (indexCutMaxHp > 0)
            {
                int canAddHpNum = cardData.nowHp - 1;
                if (canAddHpNum > indexCutMaxHp)
                {
                    canAddHpNum = indexCutMaxHp;
                }
                //自身减血
                cardData.nowHp -= canAddHpNum;
                FightControlForPvp.instance.AttackedAnimShow(cardData, canAddHpNum, false);
                //单位加血
                FightControlForPvp.instance.AttackToEffectShow(needAddHpCard, false, "42A");
                needAddHpCard.nowHp += canAddHpNum;
                FightControlForPvp.instance.ShowSpellTextObj(needAddHpCard.cardObj, LoadJsonFile.GetStringText(15), true, false);
                FightControlForPvp.instance.AttackedAnimShow(needAddHpCard, canAddHpNum, true);
            }
        }
    }

    /// <summary>
    /// 投石台攻击,对位与周围单位
    /// </summary>
    /// <param name="cardData"></param>
    private void TouShiTaiAttackFun(FightCardData cardData)
    {
        FightCardData[] attackedUnits = cardData.isPlayerCard ? enemyFightCardsDatas : playerFightCardsDatas;
        int damage = (int)(LoadJsonFile.GetGameValue(122) / 100f * GetTowerAddValue(cardData.cardId, cardData.cardGrade)); //造成的伤害
        FightControlForPvp.instance.indexAttackType = 0;

        FightControlForPvp.instance.PlayAudioForSecondClip(24, 0);

        List<GameObject> posListToThunder = cardData.isPlayerCard ? enemyCardsPos : playerCardsPos;
        EffectsPoolingControl.instance.GetEffectToFight1("101A", 1f, posListToThunder[cardData.posIndex].transform);

        if (attackedUnits[cardData.posIndex] != null && attackedUnits[cardData.posIndex].nowHp > 0)
        {
            int finalDamage = FightControlForPvp.instance.DefDamageProcessFun(cardData, attackedUnits[cardData.posIndex], damage);
            attackedUnits[cardData.posIndex].nowHp -= finalDamage;
            FightControlForPvp.instance.AttackedAnimShow(attackedUnits[cardData.posIndex], finalDamage, false);
            if (attackedUnits[cardData.posIndex].cardType == 522)
            {
                if (attackedUnits[cardData.posIndex].nowHp <= 0)
                {
                    FightControlForPvp.instance.recordWinner = attackedUnits[cardData.posIndex].isPlayerCard ? -1 : 1;
                }
            }
        }
        for (int i = 0; i < CardNearbyAdditionForeach[cardData.posIndex].Length; i++)
        {
            FightCardData attackedUnit = attackedUnits[CardNearbyAdditionForeach[cardData.posIndex][i]];
            if (attackedUnit != null && attackedUnit.nowHp > 0)
            {
                int finalDamage = FightControlForPvp.instance.DefDamageProcessFun(cardData, attackedUnit, damage);
                attackedUnit.nowHp -= finalDamage;
                FightControlForPvp.instance.AttackedAnimShow(attackedUnit, finalDamage, false);
                if (attackedUnit.cardType == 522)
                {
                    if (attackedUnit.nowHp <= 0)
                    {
                        FightControlForPvp.instance.recordWinner = attackedUnits[cardData.posIndex].isPlayerCard ? -1 : 1;
                    }
                }
            }
        }
    }

    //奏乐台血量回复
    private void ZouYueTaiAddtionFun(FightCardData cardData, FightCardData[] cardsDatas)
    {
        int addtionNums = GetTowerAddValue(cardData.cardId, cardData.cardGrade);    //回复血量基值
        addtionNums = (int)(addtionNums * LoadJsonFile.GetGameValue(123) / 100f);
        FightControlForPvp.instance.indexAttackType = 0;
        FightControlForPvp.instance.PlayAudioForSecondClip(42, 0);
        for (int i = 0; i < CardNearbyAdditionForeach[cardData.posIndex].Length; i++)
        {
            FightCardData addedFightCard = cardsDatas[CardNearbyAdditionForeach[cardData.posIndex][i]];
            if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
            {
                FightControlForPvp.instance.AttackToEffectShow(addedFightCard, false, "42A");
                addedFightCard.nowHp += addtionNums;
                FightControlForPvp.instance.ShowSpellTextObj(addedFightCard.cardObj, LoadJsonFile.GetStringText(15), true, false);
                FightControlForPvp.instance.AttackedAnimShow(addedFightCard, addtionNums, true);
            }
        }
    }

    //轩辕台护盾加成
    private void XuanYuanTaiAddtionFun(FightCardData cardData, FightCardData[] cardsDatas)
    {
        FightControlForPvp.instance.PlayAudioForSecondClip(4, 0);
        int addtionNums = GetTowerAddValue(cardData.cardId, cardData.cardGrade);    //添加护盾的单位最大数
        for (int i = 0; i < CardNearbyAdditionForeach[cardData.posIndex].Length; i++)
        {
            FightCardData addedFightCard = cardsDatas[CardNearbyAdditionForeach[cardData.posIndex][i]];
            if (addedFightCard != null && addedFightCard.nowHp > 0)
            {
                if (addedFightCard.cardType == 0 && addedFightCard.fightState.withStandNums <= 0)
                {
                    FightControlForPvp.instance.AttackToEffectShow(addedFightCard, false, "4A");
                    addedFightCard.fightState.withStandNums = 1;
                    CreateSateIcon(addedFightCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_withStand, true);
                    addtionNums--;
                    if (addtionNums <= 0)
                        break;
                }
            }
        }
    }

    //武将卡牌移动消除附加状态
    private void HeroCardRemoveStatus(FightCardData cardData)
    {
        //奏乐台图标
        {
            Transform tranZyt = cardData.cardObj.transform.GetChild(7).Find(StringNameStatic.StateIconPath_zouyuetaiAddtion);
            if (tranZyt != null)
                DestroyImmediate(tranZyt.gameObject);
        }
        //战斗力图标
        if (cardData.fightState.zhangutaiAddtion > 0)
        {
            cardData.fightState.zhangutaiAddtion = 0;
            DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
        }
        //风神台图标
        if (cardData.fightState.fengShenTaiAddtion > 0)
        {
            cardData.fightState.fengShenTaiAddtion = 0;
            DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_fengShenTaiAddtion, false);
        }
        //霹雳台图标
        if (cardData.fightState.pilitaiAddtion > 0)
        {
            cardData.fightState.pilitaiAddtion = 0;
            DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_pilitaiAddtion, false);
        }
        //霹雳台图标
        if (cardData.fightState.pilitaiAddtion > 0)
        {
            cardData.fightState.pilitaiAddtion = 0;
            DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_pilitaiAddtion, false);
        }
        //狼牙台图标
        if (cardData.fightState.langyataiAddtion > 0)
        {
            cardData.fightState.langyataiAddtion = 0;
            DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_langyataiAddtion, false);
        }
        //烽火台图标
        if (cardData.fightState.fenghuotaiAddtion > 0)
        {
            cardData.fightState.fenghuotaiAddtion = 0;
            DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_fenghuotaiAddtion, false);
        }
        //迷雾阵图标
        if (cardData.fightState.miWuZhenAddtion > 0)
        {
            cardData.fightState.miWuZhenAddtion = 0;
            DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_miWuZhenAddtion, false);
        }
    }

    /// <summary>
    /// 初始化羁绊集合
    /// </summary>
    private void InitJiBanTypeListFun(Dictionary<int, JiBanActivedClass> jiBanActivedClasses)
    {
        jiBanActivedClasses.Clear();
        for (int i = 0; i < LoadJsonFile.jiBanTableDatas.Count; i++)
        {
            if (LoadJsonFile.jiBanTableDatas[i][2] != "0")
            {
                JiBanActivedClass jiBanActivedClass = new JiBanActivedClass();
                jiBanActivedClass.jiBanIndex = i;
                jiBanActivedClass.isActived = false;
                jiBanActivedClass.cardTypeLists = new List<JiBanCardTypeClass>();
                string[] arrs = LoadJsonFile.jiBanTableDatas[i][3].Split(';');
                for (int j = 0; j < arrs.Length; j++)
                {
                    if (arrs[j] != "")
                    {
                        JiBanCardTypeClass jiBanCardTypeClass = new JiBanCardTypeClass();
                        string[] arrss = arrs[j].Split(',');
                        jiBanCardTypeClass.cardType = int.Parse(arrss[0]);
                        jiBanCardTypeClass.cardId = int.Parse(arrss[1]);
                        jiBanCardTypeClass.cardLists = new List<FightCardData>();
                        jiBanActivedClass.cardTypeLists.Add(jiBanCardTypeClass);
                    }
                }
                jiBanActivedClasses.Add(i, jiBanActivedClass);
            }
        }
    }

    /// <summary>
    /// 尝试激活或取消羁绊
    /// </summary>
    /// <param name="cardData">行动卡牌</param>
    /// <param name="cardDatas">上阵卡牌集合</param>
    /// <param name="isAdd">是否上阵</param>
    public void TryToActivatedBond(FightCardData cardData, bool isAdd)
    {
        if (LoadJsonFile.heroTableDatas[cardData.cardId][25] != "")
        {
            Dictionary<int, JiBanActivedClass> jiBanActivedClasses = cardData.isPlayerCard ? FightControlForPvp.instance.playerJiBanAllTypes : FightControlForPvp.instance.enemyJiBanAllTypes;

            string[] arrs = LoadJsonFile.heroTableDatas[cardData.cardId][25].Split(',');
            //遍历所属羁绊
            for (int i = 0; i < arrs.Length; i++)
            {
                if (arrs[i] != "" && LoadJsonFile.jiBanTableDatas[int.Parse(arrs[i])][2] != "0")
                {
                    JiBanActivedClass jiBanActivedClass = jiBanActivedClasses[int.Parse(arrs[i])];
                    bool isActived = true;
                    for (int j = 0; j < jiBanActivedClass.cardTypeLists.Count; j++)
                    {
                        if (jiBanActivedClass.cardTypeLists[j].cardType == cardData.cardType && jiBanActivedClass.cardTypeLists[j].cardId == cardData.cardId)
                        {
                            if (isAdd)
                            {
                                if (!jiBanActivedClass.cardTypeLists[j].cardLists.Exists(t => t == cardData))
                                {
                                    jiBanActivedClass.cardTypeLists[j].cardLists.Add(cardData);
                                }
                            }
                            else
                            {
                                jiBanActivedClass.cardTypeLists[j].cardLists.Remove(cardData);
                            }
                        }
                        if (jiBanActivedClass.cardTypeLists[j].cardLists.Count <= 0)
                        {
                            isActived = false;
                        }
                    }
                    if (!jiBanActivedClass.isActived && isActived)
                    {
                        for (int k = 0; k < jiBanActivedClass.cardTypeLists.Count; k++)
                        {
                            for (int s = 0; s < jiBanActivedClass.cardTypeLists[k].cardLists.Count; s++)
                            {
                                jiBanActivedClass.cardTypeLists[k].cardLists[s].cardObj.transform.GetChild(10).gameObject.SetActive(false);
                                jiBanActivedClass.cardTypeLists[k].cardLists[s].cardObj.transform.GetChild(10).gameObject.SetActive(true);
                            }
                        }
                    }
                    jiBanActivedClass.isActived = isActived;
                }
            }
        }
    }

    //上阵卡牌特殊相关处理
    public void CardGoIntoBattleProcess(FightCardData cardData, int posIndex, FightCardData[] cardDatas, bool isAdd)
    {
        switch (cardData.cardType)
        {
            case 0:
                TryToActivatedBond(cardData, isAdd);

                switch (LoadJsonFile.heroTableDatas[cardData.cardId][5])
                {
                    case "4"://盾兵
                        if (isAdd)
                        {
                            if (cardData.fightState.withStandNums <= 0)
                            {
                                CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_withStand, true);
                            }
                            cardData.fightState.withStandNums = 1;
                        }
                        else
                        {
                            cardData.fightState.withStandNums = 0;
                            DestroySateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_withStand, true);
                        }
                        break;
                    case "58"://铁骑
                        FightControlForPvp.instance.UpdateTieQiStateIconShow(cardData, isAdd);
                        break;
                    default:
                        break;
                }
                if (!isAdd)
                {
                    HeroCardRemoveStatus(cardData);
                }
                else
                {
                    HeroSoldierAddtionFun(cardData, posIndex, cardDatas);
                }
                break;
            case 1:
                if (!isAdd)
                {
                    HeroCardRemoveStatus(cardData);
                }
                else
                {
                    HeroSoldierAddtionFun(cardData, posIndex, cardDatas);
                }
                break;
            case 2:
                switch (cardData.cardId)
                {
                    case 2://奏乐台
                        ZouYueTaiAddIcon(posIndex, cardDatas, isAdd);
                        break;
                    case 4://战鼓台
                        ZhanGuTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 5://风神台
                        FengShenTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 7://霹雳台
                        PiLiTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 8://狼牙台
                        LangYaTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 9://烽火台
                        FengHuoTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 10://号角台
                        ZhanGuTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 11://瞭望台
                        ZhanGuTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 12://七星坛
                        ZhanGuTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 13://斗神台
                        ZhanGuTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 14://曹魏旗
                        ZhanGuTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 15://蜀汉旗
                        ZhanGuTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 16://东吴旗
                        ZhanGuTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 17://迷雾阵
                        miWuZhenAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 18://迷雾阵
                        miWuZhenAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 19://骑兵营
                        ZhanGuTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 20://弓弩营
                        ZhanGuTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 22://长持营
                        ZhanGuTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;
                    case 23://战船营
                        ZhanGuTaiAddtionFun(cardData, posIndex, cardDatas, isAdd);
                        break;

                    default:
                        break;
                }

                break;
            default:
                break;
        }
    }

    //武将士兵卡牌加成效果
    private void HeroSoldierAddtionFun(FightCardData cardData, int posIndex, FightCardData[] cardDatas)
    {
        for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
        {
            FightCardData addtionCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
            if (addtionCard != null && addtionCard.cardType == 2 && addtionCard.nowHp > 0)
            {
                int addtionNums = GetTowerAddValue(addtionCard.cardId, addtionCard.cardGrade);
                switch (addtionCard.cardId)
                {
                    case 2://奏乐台
                        if (cardData.cardObj.transform.GetChild(7).Find(StringNameStatic.StateIconPath_zouyuetaiAddtion) == null)
                        {
                            CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zouyuetaiAddtion, false);
                        }
                        break;
                    case 4://战鼓台
                        if (cardData.fightState.zhangutaiAddtion <= 0)
                        {
                            CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
                        }
                        cardData.fightState.zhangutaiAddtion += addtionNums;
                        break;
                    case 5://风神台
                        if (cardData.fightState.fengShenTaiAddtion <= 0)
                        {
                            CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_fengShenTaiAddtion, false);
                        }
                        cardData.fightState.fengShenTaiAddtion += addtionNums;
                        break;
                    case 7://霹雳台
                        if (cardData.fightState.pilitaiAddtion <= 0)
                        {
                            CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_pilitaiAddtion, false);
                        }
                        cardData.fightState.pilitaiAddtion += addtionNums;
                        break;
                    case 8://狼牙台
                        if (cardData.fightState.langyataiAddtion <= 0)
                        {
                            CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_langyataiAddtion, false);
                        }
                        cardData.fightState.langyataiAddtion += addtionNums;
                        break;
                    case 9://烽火台
                        if (cardData.fightState.fenghuotaiAddtion <= 0)
                        {
                            CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_fenghuotaiAddtion, false);
                        }
                        cardData.fightState.fenghuotaiAddtion += addtionNums;
                        break;
                    case 10://号角台
                        if (cardData.cardMoveType == 0)   //近战
                        {
                            if (cardData.fightState.zhangutaiAddtion <= 0)
                            {
                                CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
                            }
                            cardData.fightState.zhangutaiAddtion += addtionNums;
                        }
                        break;
                    case 11://瞭望台
                        if (cardData.cardMoveType == 1)   //远程
                        {
                            if (cardData.fightState.zhangutaiAddtion <= 0)
                            {
                                CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
                            }
                            cardData.fightState.zhangutaiAddtion += addtionNums;
                        }
                        break;
                    case 12://七星坛
                        if (cardData.cardDamageType == 1) //法术
                        {
                            if (cardData.fightState.zhangutaiAddtion <= 0)
                            {
                                CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
                            }
                            cardData.fightState.zhangutaiAddtion += addtionNums;
                        }
                        break;
                    case 13://斗神台
                        if (cardData.cardDamageType == 0) //物理
                        {
                            if (cardData.fightState.zhangutaiAddtion <= 0)
                            {
                                CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
                            }
                            cardData.fightState.zhangutaiAddtion += addtionNums;
                        }
                        break;
                    case 14://曹魏旗
                        if (LoadJsonFile.heroTableDatas[cardData.cardId][6] == "1")
                        {
                            if (cardData.fightState.zhangutaiAddtion <= 0)
                            {
                                CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
                            }
                            cardData.fightState.zhangutaiAddtion += addtionNums;
                        }
                        break;
                    case 15://蜀汉旗
                        if (LoadJsonFile.heroTableDatas[cardData.cardId][6] == "0")
                        {
                            if (cardData.fightState.zhangutaiAddtion <= 0)
                            {
                                CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
                            }
                            cardData.fightState.zhangutaiAddtion += addtionNums;
                        }
                        break;
                    case 16://东吴旗
                        if (LoadJsonFile.heroTableDatas[cardData.cardId][6] == "2")
                        {
                            if (cardData.fightState.zhangutaiAddtion <= 0)
                            {
                                CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
                            }
                            cardData.fightState.zhangutaiAddtion += addtionNums;
                        }
                        break;
                    case 17://迷雾阵
                        if (cardData.fightState.miWuZhenAddtion <= 0)
                        {
                            CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_miWuZhenAddtion, false);
                        }
                        cardData.fightState.miWuZhenAddtion += addtionNums;
                        break;
                    case 18://迷雾阵
                        if (cardData.fightState.miWuZhenAddtion <= 0)
                        {
                            CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_miWuZhenAddtion, false);
                        }
                        cardData.fightState.miWuZhenAddtion += addtionNums;
                        break;
                    case 19://骑兵营
                        if (LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[cardData.cardId][5])][5] == "5")
                        {
                            if (cardData.fightState.zhangutaiAddtion <= 0)
                            {
                                CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
                            }
                            cardData.fightState.zhangutaiAddtion += addtionNums;
                        }
                        break;
                    case 20://弓弩营
                        if (LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[cardData.cardId][5])][5] == "9")
                        {
                            if (cardData.fightState.zhangutaiAddtion <= 0)
                            {
                                CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
                            }
                            cardData.fightState.zhangutaiAddtion += addtionNums;
                        }
                        break;
                    case 22://长持营
                        if (LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[cardData.cardId][5])][5] == "3")
                        {
                            if (cardData.fightState.zhangutaiAddtion <= 0)
                            {
                                CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
                            }
                            cardData.fightState.zhangutaiAddtion += addtionNums;
                        }
                        break;
                    case 23://战船营
                        if (LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[cardData.cardId][5])][5] == "8")
                        {
                            if (cardData.fightState.zhangutaiAddtion <= 0)
                            {
                                CreateSateIcon(cardData.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
                            }
                            cardData.fightState.zhangutaiAddtion += addtionNums;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //奏乐台状态栏添加
    private void ZouYueTaiAddIcon(int posIndex, FightCardData[] cardDatas, bool isAdd)
    {
        for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
        {
            FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
            if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
            {
                if (isAdd)
                {
                    if (addedFightCard.cardObj.transform.GetChild(7).Find(StringNameStatic.StateIconPath_zouyuetaiAddtion) == null)
                    {
                        CreateSateIcon(addedFightCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zouyuetaiAddtion, false);
                    }
                }
                else
                {
                    if (addedFightCard.cardObj.transform.GetChild(7).Find(StringNameStatic.StateIconPath_zouyuetaiAddtion) != null)
                    {
                        DestroyImmediate(addedFightCard.cardObj.transform.GetChild(7).Find(StringNameStatic.StateIconPath_zouyuetaiAddtion).gameObject);
                    }
                }
            }
        }
    }

    //烽火台加成技能
    private void FengHuoTaiAddtionFun(FightCardData cardData, int posIndex, FightCardData[] cardDatas, bool isAdd)
    {
        int addtionNums = GetTowerAddValue(cardData.cardId, cardData.cardGrade);

        for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
        {
            FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
            if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
            {
                if (isAdd)
                {
                    if (addedFightCard.fightState.fenghuotaiAddtion <= 0)
                    {
                        CreateSateIcon(addedFightCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_fenghuotaiAddtion, false);
                    }
                    addedFightCard.fightState.fenghuotaiAddtion += addtionNums;
                }
                else
                {
                    addedFightCard.fightState.fenghuotaiAddtion -= addtionNums;
                    if (addedFightCard.fightState.fenghuotaiAddtion <= 0)
                    {
                        addedFightCard.fightState.fenghuotaiAddtion = 0;
                        DestroySateIcon(addedFightCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_fenghuotaiAddtion, false);
                    }
                }
            }
        }
    }

    //狼牙台加成技能
    private void LangYaTaiAddtionFun(FightCardData cardData, int posIndex, FightCardData[] cardDatas, bool isAdd)
    {
        int addtionNums = GetTowerAddValue(cardData.cardId, cardData.cardGrade);

        for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
        {
            FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
            if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
            {
                if (isAdd)
                {
                    if (addedFightCard.fightState.langyataiAddtion <= 0)
                    {
                        CreateSateIcon(addedFightCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_langyataiAddtion, false);
                    }
                    addedFightCard.fightState.langyataiAddtion += addtionNums;
                }
                else
                {
                    addedFightCard.fightState.langyataiAddtion -= addtionNums;
                    if (addedFightCard.fightState.langyataiAddtion <= 0)
                    {
                        addedFightCard.fightState.langyataiAddtion = 0;
                        DestroySateIcon(addedFightCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_langyataiAddtion, false);
                    }
                }
            }
        }
    }

    //霹雳台加成技能
    private void PiLiTaiAddtionFun(FightCardData cardData, int posIndex, FightCardData[] cardDatas, bool isAdd)
    {
        int addtionNums = GetTowerAddValue(cardData.cardId, cardData.cardGrade);

        for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
        {
            FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
            if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
            {
                if (isAdd)
                {
                    if (addedFightCard.fightState.pilitaiAddtion <= 0)
                    {
                        CreateSateIcon(addedFightCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_pilitaiAddtion, false);
                    }
                    addedFightCard.fightState.pilitaiAddtion += addtionNums;
                }
                else
                {
                    addedFightCard.fightState.pilitaiAddtion -= addtionNums;
                    if (addedFightCard.fightState.pilitaiAddtion <= 0)
                    {
                        addedFightCard.fightState.pilitaiAddtion = 0;
                        DestroySateIcon(addedFightCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_pilitaiAddtion, false);
                    }
                }
            }
        }
    }

    //迷雾阵加成技能
    private void miWuZhenAddtionFun(FightCardData cardData, int posIndex, FightCardData[] cardDatas, bool isAdd)
    {
        int addtionNums = GetTowerAddValue(cardData.cardId, cardData.cardGrade);

        List<GameObject> posListToSetMiWu = cardData.isPlayerCard ? playerCardsPos : enemyCardsPos;

        for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
        {
            if (!cardData.isPlayerCard)
            {
                //迷雾动画
                GameObject stateDinObj = EffectsPoolingControl.instance.GetStateIconToFight(StringNameStatic.StateIconPath_miWuZhenAddtion, cardData.cardObj.transform);
                stateDinObj.name = StringNameStatic.StateIconPath_miWuZhenAddtion + "Din";
                stateDinObj.transform.position = posListToSetMiWu[CardNearbyAdditionForeach[posIndex][i]].transform.position;
                stateDinObj.GetComponent<Animator>().enabled = false;
            }
            FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
            if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
            {
                if (isAdd)
                {
                    if (addedFightCard.fightState.miWuZhenAddtion <= 0)
                    {
                        CreateSateIcon(addedFightCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_miWuZhenAddtion, false);
                    }
                    addedFightCard.fightState.miWuZhenAddtion += addtionNums;
                }
                else
                {
                    addedFightCard.fightState.miWuZhenAddtion -= addtionNums;
                    if (addedFightCard.fightState.miWuZhenAddtion <= 0)
                    {
                        addedFightCard.fightState.miWuZhenAddtion = 0;
                        DestroySateIcon(addedFightCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_miWuZhenAddtion, false);
                    }
                }
            }
        }

        if (!isAdd && !cardData.isPlayerCard)
        {
            for (int i = 0; i < cardData.cardObj.transform.childCount; i++)
            {
                Transform tran = cardData.cardObj.transform.GetChild(i);
                if (tran.name == StringNameStatic.StateIconPath_miWuZhenAddtion + "Din")
                {
                    tran.gameObject.SetActive(false);
                }
            }
        }
    }

    //风神台加成技能
    private void FengShenTaiAddtionFun(FightCardData cardData, int posIndex, FightCardData[] cardDatas, bool isAdd)
    {
        int addtionNums = GetTowerAddValue(cardData.cardId, cardData.cardGrade);

        for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
        {
            FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
            if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
            {
                if (isAdd)
                {
                    if (addedFightCard.fightState.fengShenTaiAddtion <= 0)
                    {
                        CreateSateIcon(addedFightCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_fengShenTaiAddtion, false);
                    }
                    addedFightCard.fightState.fengShenTaiAddtion += addtionNums;
                }
                else
                {
                    addedFightCard.fightState.fengShenTaiAddtion -= addtionNums;
                    if (addedFightCard.fightState.fengShenTaiAddtion <= 0)
                    {
                        addedFightCard.fightState.fengShenTaiAddtion = 0;
                        DestroySateIcon(addedFightCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_fengShenTaiAddtion, false);
                    }
                }
            }
        }
    }

    //战鼓台(10-13塔)加成技能
    private void ZhanGuTaiAddtionFun(FightCardData cardData, int posIndex, FightCardData[] cardDatas, bool isAdd)
    {
        int addtionNums = GetTowerAddValue(cardData.cardId, cardData.cardGrade);

        switch (cardData.cardId)
        {
            case 4://战鼓台
                for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
                {
                    FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
                    if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
                        DamageTowerAdditionFun(addedFightCard, isAdd, addtionNums);
                }
                break;
            case 10://号角台
                for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
                {
                    FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
                    if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
                    {
                        if (addedFightCard.cardMoveType == 0)   //近战
                        {
                            DamageTowerAdditionFun(addedFightCard, isAdd, addtionNums);
                        }
                    }
                }
                break;
            case 11://瞭望台
                for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
                {
                    FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
                    if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
                    {
                        if (addedFightCard.cardMoveType == 1)   //远程
                        {
                            DamageTowerAdditionFun(addedFightCard, isAdd, addtionNums);
                        }
                    }
                }
                break;
            case 12://七星坛
                for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
                {
                    FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
                    if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
                    {
                        if (addedFightCard.cardDamageType == 1) //法术
                        {
                            DamageTowerAdditionFun(addedFightCard, isAdd, addtionNums);
                        }
                    }
                }
                break;
            case 13://斗神台
                for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
                {
                    FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
                    if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
                    {
                        if (addedFightCard.cardDamageType == 0) //物理
                        {
                            DamageTowerAdditionFun(addedFightCard, isAdd, addtionNums);
                        }
                    }
                }
                break;
            case 14://曹魏旗
                for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
                {
                    FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
                    if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
                    {
                        if (LoadJsonFile.heroTableDatas[addedFightCard.cardId][6] == "1") //魏势力
                        {
                            DamageTowerAdditionFun(addedFightCard, isAdd, addtionNums);
                        }
                    }
                }
                break;
            case 15://蜀汉旗
                for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
                {
                    FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
                    if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
                    {
                        if (LoadJsonFile.heroTableDatas[addedFightCard.cardId][6] == "0") //蜀势力
                        {
                            DamageTowerAdditionFun(addedFightCard, isAdd, addtionNums);
                        }
                    }
                }
                break;
            case 16://东吴旗
                for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
                {
                    FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
                    if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
                    {
                        if (LoadJsonFile.heroTableDatas[addedFightCard.cardId][6] == "2") //吴势力
                        {
                            DamageTowerAdditionFun(addedFightCard, isAdd, addtionNums);
                        }
                    }
                }
                break;
            case 19://骑兵营
                for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
                {
                    FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
                    if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
                    {
                        if (LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[addedFightCard.cardId][5])][5] == "5")
                        {
                            DamageTowerAdditionFun(addedFightCard, isAdd, addtionNums);
                        }
                    }
                }
                break;
            case 20://弓弩营
                for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
                {
                    FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
                    if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
                    {
                        if (LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[addedFightCard.cardId][5])][5] == "9")
                        {
                            DamageTowerAdditionFun(addedFightCard, isAdd, addtionNums);
                        }
                    }
                }
                break;
            case 22://长持营
                for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
                {
                    FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
                    if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
                    {
                        if (LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[addedFightCard.cardId][5])][5] == "3")
                        {
                            DamageTowerAdditionFun(addedFightCard, isAdd, addtionNums);
                        }
                    }
                }
                break;
            case 23://战船营
                for (int i = 0; i < CardNearbyAdditionForeach[posIndex].Length; i++)
                {
                    FightCardData addedFightCard = cardDatas[CardNearbyAdditionForeach[posIndex][i]];
                    if (addedFightCard != null && addedFightCard.cardType == 0 && addedFightCard.nowHp > 0)
                    {
                        if (LoadJsonFile.classTableDatas[int.Parse(LoadJsonFile.heroTableDatas[addedFightCard.cardId][5])][5] == "8")
                        {
                            DamageTowerAdditionFun(addedFightCard, isAdd, addtionNums);
                        }
                    }
                }
                break;
            default:
                break;
        }

    }

    //伤害加成塔的上阵下阵对周为单位的处理
    private void DamageTowerAdditionFun(FightCardData addedFightCard, bool isAdd, int addtionNums)
    {
        if (isAdd)
        {
            if (addedFightCard.fightState.zhangutaiAddtion <= 0)
            {
                CreateSateIcon(addedFightCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
            }
            addedFightCard.fightState.zhangutaiAddtion += addtionNums;
        }
        else
        {
            addedFightCard.fightState.zhangutaiAddtion -= addtionNums;
            if (addedFightCard.fightState.zhangutaiAddtion <= 0)
            {
                addedFightCard.fightState.zhangutaiAddtion = 0;
                DestroySateIcon(addedFightCard.cardObj.transform.GetChild(7), StringNameStatic.StateIconPath_zhangutaiAddtion, false);
            }
        }
    }

    //获取塔的加成值
    private int GetTowerAddValue(int towerId, int towerGrade)
    {
        string[] arrs = LoadJsonFile.towerTableDatas[towerId][6].Split(',');
        return int.Parse(arrs[towerGrade - 1]);
    }

    /// <summary>
    /// 创建状态图标
    /// </summary>
    /// <param name="tran"></param>
    /// <param name="stateName"></param>
    public void CreateSateIcon(Transform tran, string stateName, bool isShowEffect)
    {
        if (tran.Find(stateName) == null)
        {
            GameObject stateIconObj = EffectsPoolingControl.instance.GetStateIconToFight("stateIcon", tran);
            stateIconObj.name = stateName;
            stateIconObj.GetComponent<Image>().sprite = Resources.Load("Image/fightStateIcon/" + stateName, typeof(Sprite)) as Sprite;
        }
        if (isShowEffect && tran.parent.Find(stateName + "Din") == null)
        {
            GameObject stateDinObj = EffectsPoolingControl.instance.GetStateIconToFight(stateName, tran.parent);
            stateDinObj.name = stateName + "Din";
        }
    }

    //删除状态图标
    public void DestroySateIcon(Transform tran, string stateName, bool isShowEffect)
    {
        Transform tran0 = tran.Find(stateName);
        if (tran0 != null)
            EffectsPoolingControl.instance.TakeBackStateIcon(tran0.gameObject);
        if (isShowEffect)
        {
            Transform tran1 = tran.parent.Find(stateName + "Din");
            if (tran1 != null)
                EffectsPoolingControl.instance.TakeBackStateIcon(tran1.gameObject);
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
}