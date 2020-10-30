using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainWuBeiUIManager : MonoBehaviour
{
    public static MainWuBeiUIManager instance;

    [SerializeField]
    GameObject wuBeiZongBtnObj; //武碑总父级按钮
    [SerializeField]
    GameObject armsListObj; //武碑兵种按钮父级
    [SerializeField]
    GameObject armsBtnObj;  //武碑兵种按钮预制件
    [SerializeField]
    GameObject armsBtnListObj;
    [SerializeField]
    Transform cardsContentTran; //武碑卡牌池
    [SerializeField]
    GameObject wuBeiCardObj;    //卡牌预制件
    [SerializeField]
    Transform[] rarityCardsContentTrans;    //稀有度列表集合
    [SerializeField]
    GameObject cardContentObj;  //卡牌展示框
    [SerializeField]
    GameObject detailsObj;  //详细信息显示栏

    List<GameObject> wuBeiCardList = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        InitializedCardsContentTran();
        InitArmsBtnListFun();
    }

    //初始化卡牌池
    private void InitializedCardsContentTran()
    {
        for (int j = 0; j < 50; j++)
        {
            GameObject cardObj = Instantiate(wuBeiCardObj, cardsContentTran);
            cardObj.SetActive(false);
            wuBeiCardList.Add(cardObj);
        }
    }

    //从池中抽取卡牌
    private GameObject GetCardFromPooing(Transform usedTran)
    {
        foreach (GameObject go in wuBeiCardList)
        {
            if (!go.activeSelf)
            {
                go.transform.SetParent(usedTran);
                go.SetActive(true);
                return go;
            }
        }
        GameObject cardObj = Instantiate(wuBeiCardObj, usedTran);
        cardObj.SetActive(true);
        wuBeiCardList.Add(cardObj);
        return cardObj;
    }

    //回收卡牌
    private void TakeBackCardToPooing()
    {
        foreach (GameObject go in wuBeiCardList)
        {
            if (go.activeSelf)
            {
                go.transform.SetParent(cardsContentTran);
                go.SetActive(false);
            }
        }
    }

    //初始化武将分类列表
    private void InitArmsBtnListFun()
    {
        for (int i = 0; i < LoadJsonFile.classTableDatas.Count; i++)
        {
            GameObject btnObj = Instantiate(armsBtnObj, armsBtnListObj.transform).transform.GetChild(0).gameObject;
            string classType = i.ToString();
            btnObj.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                OnClickToShowArmsCards(classType);
            });
        }
        UpdateArmsBtnTextShow();
    }

    //点击兵种按钮显示内容
    private void OnClickToShowArmsCards(string classType)
    {
        cardContentObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = LoadJsonFile.classTableDatas[int.Parse(classType)][1];
        cardContentObj.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        cardContentObj.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate ()
        {
            GoOrBackToArmsBtnList(true);
        });
        GiveGameObjEventForHoldOn(cardContentObj.transform.GetChild(0).GetChild(1).gameObject, LoadJsonFile.classTableDatas[int.Parse(classType)][4]);


        for (int i = 0; i < PlayerDataForGame.instance.hstData.heroSaveData.Count; i++)
        {
            if (LoadJsonFile.heroTableDatas[PlayerDataForGame.instance.hstData.heroSaveData[i].id][5] == classType)
            {
                GameObject obj = GetCardFromPooing(rarityCardsContentTrans[GetIdBackCardRarity(PlayerDataForGame.instance.hstData.heroSaveData[i].typeIndex, PlayerDataForGame.instance.hstData.heroSaveData[i].id) - 1]);
                //Debug.Log("---CardId: " + PlayerDataForGame.instance.hstData.heroSaveData[i].id);
                obj.transform.GetChild(0).GetChild(3).gameObject.SetActive(!PlayerDataForGame.instance.hstData.heroSaveData[i].isHad);
                if (PlayerDataForGame.instance.hstData.heroSaveData[i].isHad)
                {
                    //卡牌
                    obj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load("Image/Cards/Hero/" + LoadJsonFile.heroTableDatas[PlayerDataForGame.instance.hstData.heroSaveData[i].id][16], typeof(Sprite)) as Sprite;
                    GiveGameObjEventForHoldOn(obj.transform.GetChild(0).gameObject,
                        LoadJsonFile.heroTableDatas[PlayerDataForGame.instance.hstData.heroSaveData[i].id][1] + ":\n" + LoadJsonFile.heroTableDatas[PlayerDataForGame.instance.hstData.heroSaveData[i].id][2]);
                    //名字
                    UIManager.instance.ShowNameTextRules(obj.transform.GetChild(0).GetChild(0).GetComponent<Text>(), LoadJsonFile.heroTableDatas[PlayerDataForGame.instance.hstData.heroSaveData[i].id][1]);
                    obj.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = UIManager.instance.NameColorChoose(GetIdBackCardRarity(PlayerDataForGame.instance.hstData.heroSaveData[i].typeIndex, PlayerDataForGame.instance.hstData.heroSaveData[i].id).ToString());
                    //星级
                    obj.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + PlayerDataForGame.instance.hstData.heroSaveData[i].maxLevel, typeof(Sprite)) as Sprite;
                    //兵种
                    obj.transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 0, typeof(Sprite)) as Sprite;
                    obj.transform.GetChild(0).GetChild(2).GetComponentInChildren<Text>().text = LoadJsonFile.classTableDatas[int.Parse(classType)][3];
                }
                else
                {
                    GiveGameObjEventForHoldOn(obj.transform.GetChild(0).gameObject, "???");
                }
                //边框
                UIManager.instance.FrameChoose(GetIdBackCardRarity(PlayerDataForGame.instance.hstData.heroSaveData[i].typeIndex, PlayerDataForGame.instance.hstData.heroSaveData[i].id).ToString(), obj.transform.GetChild(0).GetChild(4).GetComponent<Image>());
            }
        }
        GoOrBackToArmsBtnList(false);
    }

    //兵种按钮界面和卡牌界面互切
    public void GoOrBackToArmsBtnList(bool isGo)
    {
        cardContentObj.SetActive(!isGo);
        armsListObj.SetActive(isGo);
        if (isGo)
        {
            TakeBackCardToPooing();
        }
    }

    //刷新兵种按钮上的文字显示
    public void UpdateArmsBtnTextShow()
    {
        for (int i = 0; i < armsBtnListObj.transform.childCount; i++)
        {
            armsBtnListObj.transform.GetChild(i).GetComponentInChildren<Text>().text = LoadJsonFile.classTableDatas[i][1] + "  " + GetClassHeroHadCount(i.ToString()) + "/" + GetClassHeroCount(i.ToString());
        }
    }

    //查找已拥有过的数量
    private int GetClassHeroHadCount(string classType)
    {
        int count = 0;
        for (int i = 0; i < PlayerDataForGame.instance.hstData.heroSaveData.Count; i++)
        {
            if (PlayerDataForGame.instance.hstData.heroSaveData[i].isHad && LoadJsonFile.heroTableDatas[PlayerDataForGame.instance.hstData.heroSaveData[i].id][5] == classType)
            {
                count++;
            }
        }
        return count;
    }

    //点击武将大按钮分类列表置顶
    public void ShowHeroClassBtnList(bool isShowClassBtn)
    {
        wuBeiZongBtnObj.SetActive(!isShowClassBtn);
        armsListObj.SetActive(isShowClassBtn);
        if (isShowClassBtn)
        {
            armsBtnListObj.transform.parent.parent.GetComponent<ScrollRect>().DOVerticalNormalizedPos(1f, 0);
        }
    }

    //获取职业武将数量
    private int GetClassHeroCount(string classType)
    {
        int count = 0;
        for (int i = 0; i < LoadJsonFile.heroTableDatas.Count; i++)
        {
            if (LoadJsonFile.heroTableDatas[i][5] == classType)
                count++;
        }
        return count;
    }


    //点击塔按钮显示内容
    public void OnClickToShowTowerCards()
    {
        cardContentObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "塔";
        cardContentObj.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        cardContentObj.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate ()
        {
            TakeBackCardToPooing();
            wuBeiZongBtnObj.SetActive(true);
            cardContentObj.SetActive(false);
        });
        GiveGameObjEventForHoldOn(cardContentObj.transform.GetChild(0).GetChild(1).gameObject, "就是一座塔");

        for (int i = 0; i < PlayerDataForGame.instance.hstData.towerSaveData.Count; i++)
        {
            GameObject obj = GetCardFromPooing(rarityCardsContentTrans[GetIdBackCardRarity(PlayerDataForGame.instance.hstData.towerSaveData[i].typeIndex, PlayerDataForGame.instance.hstData.towerSaveData[i].id) - 1]);
            //Debug.Log("---CardId: " + PlayerDataForGame.instance.hstData.towerSaveData[i].id);
            obj.transform.GetChild(0).GetChild(3).gameObject.SetActive(!PlayerDataForGame.instance.hstData.towerSaveData[i].isHad);
            if (PlayerDataForGame.instance.hstData.towerSaveData[i].isHad)
            {
                //卡牌
                obj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.towerTableDatas[PlayerDataForGame.instance.hstData.towerSaveData[i].id][10], typeof(Sprite)) as Sprite;
                GiveGameObjEventForHoldOn(obj.transform.GetChild(0).gameObject,
                    LoadJsonFile.towerTableDatas[PlayerDataForGame.instance.hstData.towerSaveData[i].id][1] + ":\n" + LoadJsonFile.towerTableDatas[PlayerDataForGame.instance.hstData.towerSaveData[i].id][2]);
                //名字
                UIManager.instance.ShowNameTextRules(obj.transform.GetChild(0).GetChild(0).GetComponent<Text>(), LoadJsonFile.towerTableDatas[PlayerDataForGame.instance.hstData.towerSaveData[i].id][1]);
                obj.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = UIManager.instance.NameColorChoose(GetIdBackCardRarity(PlayerDataForGame.instance.hstData.towerSaveData[i].typeIndex, PlayerDataForGame.instance.hstData.towerSaveData[i].id).ToString());
                //星级
                obj.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + PlayerDataForGame.instance.hstData.towerSaveData[i].maxLevel, typeof(Sprite)) as Sprite;
                //兵种
                obj.transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
                obj.transform.GetChild(0).GetChild(2).GetComponentInChildren<Text>().text = LoadJsonFile.towerTableDatas[PlayerDataForGame.instance.hstData.towerSaveData[i].id][5];
            }
            else
            {
                GiveGameObjEventForHoldOn(obj.transform.GetChild(0).gameObject, "???");
            }
            //边框
            UIManager.instance.FrameChoose(
                GetIdBackCardRarity(PlayerDataForGame.instance.hstData.towerSaveData[i].typeIndex, PlayerDataForGame.instance.hstData.towerSaveData[i].id).ToString(),
                obj.transform.GetChild(0).GetChild(4).GetComponent<Image>()
                );
        }
        cardContentObj.SetActive(true);
        wuBeiZongBtnObj.SetActive(false);
    }

    //点击陷阱按钮显示内容
    public void OnClickToShowTrapCards()
    {
        cardContentObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "陷阱";
        cardContentObj.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        cardContentObj.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate ()
        {
            TakeBackCardToPooing();
            wuBeiZongBtnObj.SetActive(true);
            cardContentObj.SetActive(false);
        });
        GiveGameObjEventForHoldOn(cardContentObj.transform.GetChild(0).GetChild(1).gameObject, "就是一个陷阱");

        for (int i = 0; i < PlayerDataForGame.instance.hstData.trapSaveData.Count; i++)
        {
            GameObject obj = GetCardFromPooing(rarityCardsContentTrans[GetIdBackCardRarity(PlayerDataForGame.instance.hstData.trapSaveData[i].typeIndex, PlayerDataForGame.instance.hstData.trapSaveData[i].id) - 1]);
            //Debug.Log("---CardId: " + PlayerDataForGame.instance.hstData.trapSaveData[i].id);
            obj.transform.GetChild(0).GetChild(3).gameObject.SetActive(!PlayerDataForGame.instance.hstData.trapSaveData[i].isHad);
            if (PlayerDataForGame.instance.hstData.trapSaveData[i].isHad)
            {
                //卡牌
                obj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load("Image/Cards/FuZhu/" + LoadJsonFile.trapTableDatas[PlayerDataForGame.instance.hstData.trapSaveData[i].id][8], typeof(Sprite)) as Sprite;
                GiveGameObjEventForHoldOn(obj.transform.GetChild(0).gameObject,
                    LoadJsonFile.trapTableDatas[PlayerDataForGame.instance.hstData.trapSaveData[i].id][1] + ":\n" + LoadJsonFile.trapTableDatas[PlayerDataForGame.instance.hstData.trapSaveData[i].id][2]);
                //名字
                UIManager.instance.ShowNameTextRules(obj.transform.GetChild(0).GetChild(0).GetComponent<Text>(), LoadJsonFile.trapTableDatas[PlayerDataForGame.instance.hstData.trapSaveData[i].id][1]);
                obj.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = UIManager.instance.NameColorChoose(GetIdBackCardRarity(PlayerDataForGame.instance.hstData.trapSaveData[i].typeIndex, PlayerDataForGame.instance.hstData.trapSaveData[i].id).ToString());
                //星级
                obj.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = Resources.Load("Image/gradeImage/" + PlayerDataForGame.instance.hstData.trapSaveData[i].maxLevel, typeof(Sprite)) as Sprite;
                //兵种
                obj.transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite = Resources.Load("Image/classImage/" + 1, typeof(Sprite)) as Sprite;
                obj.transform.GetChild(0).GetChild(2).GetComponentInChildren<Text>().text = LoadJsonFile.trapTableDatas[PlayerDataForGame.instance.hstData.trapSaveData[i].id][5];
            }
            else
            {
                GiveGameObjEventForHoldOn(obj.transform.GetChild(0).gameObject, "???");
            }
            //边框
            UIManager.instance.FrameChoose(
                GetIdBackCardRarity(PlayerDataForGame.instance.hstData.trapSaveData[i].typeIndex, PlayerDataForGame.instance.hstData.trapSaveData[i].id).ToString(),
                obj.transform.GetChild(0).GetChild(4).GetComponent<Image>()
                );
        }
        cardContentObj.SetActive(true);
        wuBeiZongBtnObj.SetActive(false);
    }

    //展示详细信息
    private void ShowInfoOfCardOrArms(string infoStr)
    {
        detailsObj.GetComponentInChildren<Text>().text = infoStr;
        detailsObj.SetActive(true);
    }

    //附加按下抬起方法
    private void GiveGameObjEventForHoldOn(GameObject obj, string str)
    {
        EventTriggerListener.Get(obj).onDown += (go) =>
        {
            ShowInfoOfCardOrArms(str);
        };
        EventTriggerListener.Get(obj).onUp += (go) =>
        {
            detailsObj.SetActive(false);
        };
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
}