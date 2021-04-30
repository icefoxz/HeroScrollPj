using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardForDrag : MonoBehaviour
{
    [HideInInspector]
    public int posIndex;    //上场位置记录
    [HideInInspector]
    public bool isFightCard = false; //记录是否是上阵卡牌

    /// <summary>
    /// 场景中的Panel，设置拖拽过程中的父物体
    /// </summary>
    private Transform _herosCardList;
    /// <summary>
    /// Scroll View上的Scroll Rect组件
    /// </summary>
    private ScrollRect _scrollRect;

    void Awake()
    {
        _herosCardList = WarsUIManager.instance.herosCardListTran;

        _scrollRect = WarsUIManager.instance.herosCardListScrollRect;
    }


    //开始拖拽
    public void BeginDrag(BaseEventData data)
    {
        if (isFightCard)
        {
            return;
        }

        Vector2 touchDeltaPosition = Vector2.zero;
#if UNITY_EDITOR
        float delta_x = Input.GetAxis("Mouse X");
        float delta_y = Input.GetAxis("Mouse Y");
        touchDeltaPosition = new Vector2(delta_x, delta_y);

#elif UNITY_ANDROID || UNITY_IPHONE
        touchDeltaPosition = Input.GetTouch(0).deltaPosition;  
#endif
        if (transform.parent != WarsUIManager.instance.heroCardListObj.transform || Mathf.Abs(touchDeltaPosition.x) / 2 < Mathf.Abs(touchDeltaPosition.y))
        {
            WarsUIManager.instance._isDragItem = true;
        }
        else
        {
            WarsUIManager.instance._isDragItem = false;
            PointerEventData _eventData = data as PointerEventData;
            //调用Scroll的OnBeginDrag方法，有了区分，就不会被item的拖拽事件屏蔽
            _scrollRect.OnBeginDrag(_eventData);
        }

        if (!WarsUIManager.instance._isDragItem)
        {
            return;
        }
        transform.SetParent(_herosCardList);
        transform.SetAsLastSibling();   //设置为同父物体的最从底层，也就是不会被其同级遮挡。
        if (transform.GetComponent<Image>().raycastTarget)
            transform.GetComponent<Image>().raycastTarget = false;
    }

    //拖动中
    public void OnDrag(BaseEventData data)
    {
        if (isFightCard)
        {
            return;
        }

        if (!WarsUIManager.instance._isDragItem)
        {
            PointerEventData _eventData = data as PointerEventData;
            _scrollRect.OnDrag(_eventData);
            return;
        }
        if (FightController.instance.recordWinner != 0)
        {
            EndDrag(data);
            return;
        }
        transform.position = Input.mousePosition;
    }

    //结束时
    public void EndDrag(BaseEventData data)
    {
        if (isFightCard)
        {
            return;
        }

        if (!WarsUIManager.instance._isDragItem)    //判断是否拖动的是滑动列表
        {
            PointerEventData eventData = data as PointerEventData;
            _scrollRect.OnEndDrag(eventData);
            return;
        }
        else
        {
            if (FightController.instance.isRoundBegin)  //战斗回合突然开始
            {
                transform.SetParent(WarsUIManager.instance.heroCardListObj.transform);
                transform.GetChild(8).gameObject.SetActive(false);
                transform.GetComponent<Image>().raycastTarget = true;
                posIndex = -1;
                return;
            }

            PointerEventData _eventData = data as PointerEventData; //获取拖拽释放事件
            if (_eventData == null)
                return;

            GameObject go = _eventData.pointerCurrentRaycast.gameObject;    //释放时鼠标透过拖动的Image后的物体
            //是否拖放在 战斗格子 或 卡牌上
            if (go != null && (go.CompareTag("CardPos") || go.CompareTag("PyCard")))
            {
                //放在 战斗格子上
                if (go.CompareTag("CardPos"))
                {
                    //拖动牌原位置 在上阵位
                    if (posIndex != -1)
                    {
                        int num = int.Parse(go.GetComponentInChildren<Text>().text);    //目标位置编号
                        if (posIndex != num)    //上阵位置改变
                        {
                            FightForManager.instance.CardGoIntoBattleProcess(FightForManager.instance.playerFightCardsDatas[posIndex], posIndex, FightForManager.instance.playerFightCardsDatas, false);

                            FightForManager.instance.playerFightCardsDatas[num] = FightForManager.instance.playerFightCardsDatas[posIndex];
                            FightForManager.instance.playerFightCardsDatas[num].posIndex = num;
                            FightForManager.instance.playerFightCardsDatas[posIndex] = null;
                            posIndex = num;

                            FightForManager.instance.CardGoIntoBattleProcess(FightForManager.instance.playerFightCardsDatas[posIndex], posIndex, FightForManager.instance.playerFightCardsDatas, true);
                        }
                        transform.SetParent(FightForManager.instance.playerCardsBox);
                        transform.position = go.transform.position;
                        FightController.instance.PlayAudioForSecondClip(85, 0);

                        EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManager.instance.playerCardsPos[posIndex].transform);
                        //GameObject eftObj = EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManager.instance.playerCardsPos[posIndex].transform);
                        //eftObj.transform.SetParent(FightForManager.instance.playerCardsPos[posIndex].transform);
                    }
                    //拖动牌原位置 在备战位
                    else
                    {
                        //是否可以上阵
                        if (FightForManager.instance.ChangeNumsSuccess(true))
                        {
                            int num = int.Parse(go.GetComponentInChildren<Text>().text);
                            int index = WarsUIManager.instance.FindDataFromCardsDatas(gameObject);
                            if (index != -1)
                            {
                                FightForManager.instance.playerFightCardsDatas[num] = WarsUIManager.instance.playerCardsDatas[index];
                                FightForManager.instance.playerFightCardsDatas[num].posIndex = num;
                                //Debug.Log(">>>>>>>" + num);
                                FightForManager.instance.CardGoIntoBattleProcess(FightForManager.instance.playerFightCardsDatas[num], num, FightForManager.instance.playerFightCardsDatas, true);
                            }
                            transform.SetParent(FightForManager.instance.playerCardsBox);
                            transform.position = go.transform.position;
                            transform.GetChild(8).gameObject.SetActive(true);
                            posIndex = num;
                            //isFightCard = true;
                            FightController.instance.PlayAudioForSecondClip(85, 0);

                            EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManager.instance.playerCardsPos[posIndex].transform);
                            //GameObject eftObj = EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManager.instance.playerCardsPos[posIndex].transform);
                            //eftObj.transform.SetParent(FightForManager.instance.playerCardsPos[posIndex].transform);
                        }
                        else
                        {
                            //Debug.Log("上阵位已满");
                            transform.SetParent(WarsUIManager.instance.heroCardListObj.transform);
                            transform.GetChild(8).gameObject.SetActive(false);
                            posIndex = -1;
                        }
                    }
                }
                //放在 卡牌上
                if (go.CompareTag("PyCard"))
                {
                    int goIndexPos = go.GetComponent<CardForDrag>().posIndex;
                    //目的地 卡牌在上阵位 并且 不是上锁卡牌
                    if (goIndexPos != -1 && !go.GetComponent<CardForDrag>().isFightCard)
                    {
                        //拖动牌原位置 在上阵位
                        if (posIndex != -1)
                        {
                            FightForManager.instance.CardGoIntoBattleProcess(FightForManager.instance.playerFightCardsDatas[posIndex], posIndex, FightForManager.instance.playerFightCardsDatas, false);
                            FightForManager.instance.CardGoIntoBattleProcess(FightForManager.instance.playerFightCardsDatas[goIndexPos], goIndexPos, FightForManager.instance.playerFightCardsDatas, false);

                            transform.SetParent(FightForManager.instance.playerCardsBox);
                            transform.position = go.transform.position;
                            go.transform.position = FightForManager.instance.playerCardsPos[posIndex].transform.position;
                            //WarsUIManager.instance.CardMoveToPos(go, FightForManager.instance.playerCardsPos[posIndex].transform.position);
                            FightCardData dataTemp = FightForManager.instance.playerFightCardsDatas[goIndexPos];
                            FightForManager.instance.playerFightCardsDatas[goIndexPos] = FightForManager.instance.playerFightCardsDatas[posIndex];
                            FightForManager.instance.playerFightCardsDatas[goIndexPos].posIndex = goIndexPos;
                            FightForManager.instance.playerFightCardsDatas[posIndex] = dataTemp;
                            FightForManager.instance.playerFightCardsDatas[posIndex].posIndex = posIndex;
                            go.GetComponent<CardForDrag>().posIndex = posIndex;

                            FightForManager.instance.CardGoIntoBattleProcess(FightForManager.instance.playerFightCardsDatas[posIndex], posIndex, FightForManager.instance.playerFightCardsDatas, true);
                            FightForManager.instance.CardGoIntoBattleProcess(FightForManager.instance.playerFightCardsDatas[goIndexPos], goIndexPos, FightForManager.instance.playerFightCardsDatas, true);

                            EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManager.instance.playerCardsPos[posIndex].transform);
                            //GameObject eftObj = EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManager.instance.playerCardsPos[posIndex].transform);
                            EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManager.instance.playerCardsPos[goIndexPos].transform);
                            //GameObject eftObj1 = EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManager.instance.playerCardsPos[goIndexPos].transform);
                            //eftObj.transform.SetParent(FightForManager.instance.playerCardsPos[posIndex].transform);
                            //eftObj1.transform.SetParent(FightForManager.instance.playerCardsPos[goIndexPos].transform);

                            posIndex = goIndexPos;
                            FightController.instance.PlayAudioForSecondClip(85, 0);
                        }
                        //拖动牌原位置 在备战位
                        else
                        {
                            FightForManager.instance.ChangeNumsSuccess(false);
                            if (FightForManager.instance.ChangeNumsSuccess(true))
                            {
                                transform.SetParent(FightForManager.instance.playerCardsBox);
                                transform.position = go.transform.position;
                                transform.GetChild(8).gameObject.SetActive(true);

                                FightForManager.instance.CardGoIntoBattleProcess(FightForManager.instance.playerFightCardsDatas[goIndexPos], goIndexPos, FightForManager.instance.playerFightCardsDatas, false);

                                go.transform.SetParent(WarsUIManager.instance.heroCardListObj.transform);

                                int index = WarsUIManager.instance.FindDataFromCardsDatas(gameObject);
                                if (index != -1)
                                {
                                    FightForManager.instance.playerFightCardsDatas[go.GetComponent<CardForDrag>().posIndex] = WarsUIManager.instance.playerCardsDatas[index];
                                    FightForManager.instance.playerFightCardsDatas[go.GetComponent<CardForDrag>().posIndex].posIndex = go.GetComponent<CardForDrag>().posIndex;
                                }
                                go.GetComponent<CardForDrag>().posIndex = -1;
                                go.transform.GetChild(8).gameObject.SetActive(false);
                                posIndex = goIndexPos;

                                FightForManager.instance.CardGoIntoBattleProcess(FightForManager.instance.playerFightCardsDatas[posIndex], posIndex, FightForManager.instance.playerFightCardsDatas, true);
                                FightController.instance.PlayAudioForSecondClip(85, 0);

                                EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManager.instance.playerCardsPos[posIndex].transform);
                                //GameObject eftObj =  EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManager.instance.playerCardsPos[posIndex].transform);
                                //eftObj.transform.SetParent(FightForManager.instance.playerCardsPos[posIndex].transform);
                            } 
                            else
                            {
                                FightForManager.instance.ChangeNumsSuccess(true);
                                //Debug.Log("上阵位已满");
                                transform.SetParent(WarsUIManager.instance.heroCardListObj.transform);
                                transform.GetChild(8).gameObject.SetActive(false);
                                posIndex = -1;
                            }
                        }
                    }
                    //目的地 卡牌在备战位
                    else
                    {
                        if (posIndex != -1)
                        {
                            FightForManager.instance.ChangeNumsSuccess(false);
                            FightForManager.instance.CardGoIntoBattleProcess(FightForManager.instance.playerFightCardsDatas[posIndex], posIndex, FightForManager.instance.playerFightCardsDatas, false);
                            FightForManager.instance.playerFightCardsDatas[posIndex] = null;
                        }
                        transform.SetParent(WarsUIManager.instance.heroCardListObj.transform);
                        transform.GetChild(8).gameObject.SetActive(false);
                        posIndex = -1;
                    }
                }
            }
            else //目的地为其他
            {
                if (posIndex != -1) //原位置在上阵位
                {
                    FightForManager.instance.ChangeNumsSuccess(false);
                    FightForManager.instance.CardGoIntoBattleProcess(FightForManager.instance.playerFightCardsDatas[posIndex], posIndex, FightForManager.instance.playerFightCardsDatas, false);
                    FightForManager.instance.playerFightCardsDatas[posIndex] = null;
                }
                transform.SetParent(WarsUIManager.instance.heroCardListObj.transform);
                transform.GetChild(8).gameObject.SetActive(false);
                posIndex = -1;
            }
            transform.GetComponent<Image>().raycastTarget = true;
        }
    }
}