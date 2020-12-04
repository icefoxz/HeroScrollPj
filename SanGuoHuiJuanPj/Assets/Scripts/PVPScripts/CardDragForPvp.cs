using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardDragForPvp : MonoBehaviour
{
    [HideInInspector]
    public int posIndex;    //上场位置记录
    [HideInInspector]
    public bool isFightCard; //记录是否是上阵卡牌

    /// <summary>
    /// 场景中的Panel，设置拖拽过程中的父物体
    /// </summary>
    private Transform _herosCardList;
    /// <summary>
    /// Scroll View上的Scroll Rect组件
    /// </summary>
    private ScrollRect _scrollRect;

    void Start()
    {
        _herosCardList = FightManagerForPvp.instance.cardListTran;

        _scrollRect = FightManagerForPvp.instance.cardListScrollRect;
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
        if (transform.parent != FightManagerForPvp.instance.herosCardListTran || Mathf.Abs(touchDeltaPosition.x) / 2 < Mathf.Abs(touchDeltaPosition.y))
        {
            FightManagerForPvp.instance._isDragItem = true;
        }
        else
        {
            FightManagerForPvp.instance._isDragItem = false;
            PointerEventData _eventData = data as PointerEventData;
            //调用Scroll的OnBeginDrag方法，有了区分，就不会被item的拖拽事件屏蔽
            _scrollRect.OnBeginDrag(_eventData);
        }

        if (!FightManagerForPvp.instance._isDragItem)
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

        if (!FightManagerForPvp.instance._isDragItem)
        {
            PointerEventData _eventData = data as PointerEventData;
            _scrollRect.OnDrag(_eventData);
            return;
        }
        if (FightControlForPvp.instance.recordWinner != 0)
        {
            EndDrag(data);
            return;
        }
        //transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        //获取到鼠标的位置(鼠标水平的输入和竖直的输入以及距离)
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //物体的位置，屏幕坐标转换为世界坐标
        Vector3 objectPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        //把鼠标位置传给物体
        transform.position = objectPosition;
    }


    //结束时
    public void EndDrag(BaseEventData data)
    {
        if (isFightCard)
        {
            return;
        }

        if (!FightManagerForPvp.instance._isDragItem)    //判断是否拖动的是滑动列表
        {
            PointerEventData eventData = data as PointerEventData;
            _scrollRect.OnEndDrag(eventData);
            return;
        }
        else
        {
            if (FightControlForPvp.instance.isRoundBegin)  //战斗回合突然开始
            {
                transform.SetParent(FightManagerForPvp.instance.herosCardListTran);
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
            if (go != null && (go.tag == "CardPos" || go.tag == "PyCard"))
            {
                //放在 战斗格子上
                if (go.tag == "CardPos")
                {
                    //拖动牌原位置 在上阵位
                    if (posIndex != -1)
                    {
                        int num = int.Parse(go.GetComponentInChildren<Text>().text);    //目标位置编号
                        if (posIndex != num)    //上阵位置改变
                        {
                            FightManagerForPvp.instance.CardGoIntoBattleProcess(FightManagerForPvp.instance.playerFightCardsDatas[posIndex], posIndex, FightManagerForPvp.instance.playerFightCardsDatas, false);

                            FightManagerForPvp.instance.playerFightCardsDatas[num] = FightManagerForPvp.instance.playerFightCardsDatas[posIndex];
                            FightManagerForPvp.instance.playerFightCardsDatas[num].posIndex = num;
                            FightManagerForPvp.instance.playerFightCardsDatas[posIndex] = null;
                            posIndex = num;

                            FightManagerForPvp.instance.CardGoIntoBattleProcess(FightManagerForPvp.instance.playerFightCardsDatas[posIndex], posIndex, FightManagerForPvp.instance.playerFightCardsDatas, true);
                        }
                        transform.SetParent(FightManagerForPvp.instance.playerCardsBox);
                        transform.position = go.transform.position;
                        FightControlForPvp.instance.PlayAudioForSecondClip(85, 0);

                        EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightManagerForPvp.instance.playerCardsPos[posIndex].transform);
                    }
                    //拖动牌原位置 在备战位
                    else
                    {
                        int num = int.Parse(go.GetComponentInChildren<Text>().text);
                        int index = FightManagerForPvp.instance.FindDataFromCardsDatas(gameObject);
                        if (index != -1)
                        {
                            FightManagerForPvp.instance.playerFightCardsDatas[num] = FightManagerForPvp.instance.playerCardsDatas[index];
                            FightManagerForPvp.instance.playerFightCardsDatas[num].posIndex = num;
                            FightManagerForPvp.instance.CardGoIntoBattleProcess(FightManagerForPvp.instance.playerFightCardsDatas[num], num, FightManagerForPvp.instance.playerFightCardsDatas, true);
                        }
                        transform.SetParent(FightManagerForPvp.instance.playerCardsBox);
                        transform.position = go.transform.position;
                        transform.GetChild(8).gameObject.SetActive(true);
                        posIndex = num;

                        FightControlForPvp.instance.PlayAudioForSecondClip(85, 0);
                        EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightManagerForPvp.instance.playerCardsPos[posIndex].transform);
                        ////隐藏第一个提示
                        //FightManagerForPvp.instance.ChangeGuideForFight(0);
                    }
                }
                //放在 卡牌上
                if (go.tag == "PyCard")
                {
                    int goIndexPos = go.GetComponent<CardDragForStart>().posIndex;
                    //目的地 卡牌在上阵位 并且 不是上锁卡牌
                    if (goIndexPos != -1 && !go.GetComponent<CardDragForStart>().isFightCard)
                    {
                        //拖动牌原位置 在上阵位
                        if (posIndex != -1)
                        {
                            FightManagerForPvp.instance.CardGoIntoBattleProcess(FightManagerForPvp.instance.playerFightCardsDatas[posIndex], posIndex, FightManagerForPvp.instance.playerFightCardsDatas, false);
                            FightManagerForPvp.instance.CardGoIntoBattleProcess(FightManagerForPvp.instance.playerFightCardsDatas[goIndexPos], goIndexPos, FightManagerForPvp.instance.playerFightCardsDatas, false);

                            transform.SetParent(FightManagerForPvp.instance.playerCardsBox);
                            transform.position = go.transform.position;
                            go.transform.position = FightManagerForPvp.instance.playerCardsPos[posIndex].transform.position;
                            FightCardData dataTemp = FightManagerForPvp.instance.playerFightCardsDatas[goIndexPos];
                            FightManagerForPvp.instance.playerFightCardsDatas[goIndexPos] = FightManagerForPvp.instance.playerFightCardsDatas[posIndex];
                            FightManagerForPvp.instance.playerFightCardsDatas[goIndexPos].posIndex = goIndexPos;
                            FightManagerForPvp.instance.playerFightCardsDatas[posIndex] = dataTemp;
                            FightManagerForPvp.instance.playerFightCardsDatas[posIndex].posIndex = posIndex;
                            go.GetComponent<CardDragForStart>().posIndex = posIndex;

                            FightManagerForPvp.instance.CardGoIntoBattleProcess(FightManagerForPvp.instance.playerFightCardsDatas[posIndex], posIndex, FightManagerForPvp.instance.playerFightCardsDatas, true);
                            FightManagerForPvp.instance.CardGoIntoBattleProcess(FightManagerForPvp.instance.playerFightCardsDatas[goIndexPos], goIndexPos, FightManagerForPvp.instance.playerFightCardsDatas, true);

                            EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightManagerForPvp.instance.playerCardsPos[posIndex].transform);
                            EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightManagerForPvp.instance.playerCardsPos[goIndexPos].transform);

                            posIndex = goIndexPos;
                            FightControlForPvp.instance.PlayAudioForSecondClip(85, 0);
                        }
                        //拖动牌原位置 在备战位
                        else
                        {
                            transform.SetParent(FightManagerForPvp.instance.playerCardsBox);
                            transform.position = go.transform.position;
                            transform.GetChild(8).gameObject.SetActive(true);

                            FightManagerForPvp.instance.CardGoIntoBattleProcess(FightManagerForPvp.instance.playerFightCardsDatas[goIndexPos], goIndexPos, FightManagerForPvp.instance.playerFightCardsDatas, false);

                            go.transform.SetParent(FightManagerForPvp.instance.herosCardListTran);

                            int index = FightManagerForPvp.instance.FindDataFromCardsDatas(gameObject);
                            if (index != -1)
                            {
                                FightManagerForPvp.instance.playerFightCardsDatas[go.GetComponent<CardDragForStart>().posIndex] = FightManagerForPvp.instance.playerCardsDatas[index];
                                FightManagerForPvp.instance.playerFightCardsDatas[go.GetComponent<CardDragForStart>().posIndex].posIndex = go.GetComponent<CardDragForStart>().posIndex;
                            }
                            go.GetComponent<CardDragForStart>().posIndex = -1;
                            go.transform.GetChild(8).gameObject.SetActive(false);
                            posIndex = goIndexPos;

                            FightManagerForPvp.instance.CardGoIntoBattleProcess(FightManagerForPvp.instance.playerFightCardsDatas[posIndex], posIndex, FightManagerForPvp.instance.playerFightCardsDatas, true);
                            FightControlForPvp.instance.PlayAudioForSecondClip(85, 0);

                            EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightManagerForPvp.instance.playerCardsPos[posIndex].transform);
                        }
                    }
                    //目的地 卡牌在备战位
                    else
                    {
                        if (posIndex != -1)
                        {
                            FightManagerForPvp.instance.CardGoIntoBattleProcess(FightManagerForPvp.instance.playerFightCardsDatas[posIndex], posIndex, FightManagerForPvp.instance.playerFightCardsDatas, false);
                            FightManagerForPvp.instance.playerFightCardsDatas[posIndex] = null;
                        }
                        transform.SetParent(FightManagerForPvp.instance.herosCardListTran);
                        transform.GetChild(8).gameObject.SetActive(false);
                        posIndex = -1;
                    }
                }
            }
            else //目的地为其他
            {
                if (posIndex != -1) //原位置在上阵位
                {
                    FightManagerForPvp.instance.CardGoIntoBattleProcess(FightManagerForPvp.instance.playerFightCardsDatas[posIndex], posIndex, FightManagerForPvp.instance.playerFightCardsDatas, false);
                    FightManagerForPvp.instance.playerFightCardsDatas[posIndex] = null;
                }
                transform.SetParent(FightManagerForPvp.instance.herosCardListTran);
                transform.GetChild(8).gameObject.SetActive(false);
                posIndex = -1;
            }
            transform.GetComponent<Image>().raycastTarget = true;
        }
    }
}