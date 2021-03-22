using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardDragForStart : MonoBehaviour
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
        _herosCardList = FightForManagerForStart.instance.cardListTran;

        _scrollRect = FightForManagerForStart.instance.cardListScrollRect;
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
        if (transform.parent != FightForManagerForStart.instance.herosCardListTran || Mathf.Abs(touchDeltaPosition.x) / 2 < Mathf.Abs(touchDeltaPosition.y))
        {
            FightForManagerForStart.instance._isDragItem = true;
        }
        else
        {
            FightForManagerForStart.instance._isDragItem = false;
            PointerEventData _eventData = data as PointerEventData;
            //调用Scroll的OnBeginDrag方法，有了区分，就不会被item的拖拽事件屏蔽
            _scrollRect.OnBeginDrag(_eventData);
        }

        if (!FightForManagerForStart.instance._isDragItem)
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

        if (!FightForManagerForStart.instance._isDragItem)
        {
            PointerEventData _eventData = data as PointerEventData;
            _scrollRect.OnDrag(_eventData);
            return;
        }
        if (FightControlForStart.instance.recordWinner != 0)
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

        if (!FightForManagerForStart.instance._isDragItem)    //判断是否拖动的是滑动列表
        {
            PointerEventData eventData = data as PointerEventData;
            _scrollRect.OnEndDrag(eventData);
            return;
        }
        else
        {
            if (FightControlForStart.instance.isRoundBegin)  //战斗回合突然开始
            {
                transform.SetParent(FightForManagerForStart.instance.herosCardListTran);
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
                            FightForManagerForStart.instance.CardGoIntoBattleProcess(FightForManagerForStart.instance.playerFightCardsDatas[posIndex], posIndex, FightForManagerForStart.instance.playerFightCardsDatas, false);

                            FightForManagerForStart.instance.playerFightCardsDatas[num] = FightForManagerForStart.instance.playerFightCardsDatas[posIndex];
                            FightForManagerForStart.instance.playerFightCardsDatas[num].posIndex = num;
                            FightForManagerForStart.instance.playerFightCardsDatas[posIndex] = null;
                            posIndex = num;

                            FightForManagerForStart.instance.CardGoIntoBattleProcess(FightForManagerForStart.instance.playerFightCardsDatas[posIndex], posIndex, FightForManagerForStart.instance.playerFightCardsDatas, true);
                        }
                        transform.SetParent(FightForManagerForStart.instance.playerCardsBox);
                        transform.position = go.transform.position;
                        FightControlForStart.instance.PlayAudioForSecondClip(85, 0);

                        EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManagerForStart.instance.playerCardsPos[posIndex].transform);
                    }
                    //拖动牌原位置 在备战位
                    else
                    {
                        int num = int.Parse(go.GetComponentInChildren<Text>().text);
                        int index = FightForManagerForStart.instance.FindDataFromCardsDatas(gameObject);
                        if (index != -1)
                        {
                            FightForManagerForStart.instance.playerFightCardsDatas[num] = FightForManagerForStart.instance.playerCardsDatas[index];
                            FightForManagerForStart.instance.playerFightCardsDatas[num].posIndex = num;
                            FightForManagerForStart.instance.CardGoIntoBattleProcess(FightForManagerForStart.instance.playerFightCardsDatas[num], num, FightForManagerForStart.instance.playerFightCardsDatas, true);
                        }
                        transform.SetParent(FightForManagerForStart.instance.playerCardsBox);
                        transform.position = go.transform.position;
                        transform.GetChild(8).gameObject.SetActive(true);
                        posIndex = num;

                        FightControlForStart.instance.PlayAudioForSecondClip(85, 0);
                        EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManagerForStart.instance.playerCardsPos[posIndex].transform);
                        //隐藏第一个提示
                        FightForManagerForStart.instance.ChangeGuideForFight(0);
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
                            FightForManagerForStart.instance.CardGoIntoBattleProcess(FightForManagerForStart.instance.playerFightCardsDatas[posIndex], posIndex, FightForManagerForStart.instance.playerFightCardsDatas, false);
                            FightForManagerForStart.instance.CardGoIntoBattleProcess(FightForManagerForStart.instance.playerFightCardsDatas[goIndexPos], goIndexPos, FightForManagerForStart.instance.playerFightCardsDatas, false);

                            transform.SetParent(FightForManagerForStart.instance.playerCardsBox);
                            transform.position = go.transform.position;
                            go.transform.position = FightForManagerForStart.instance.playerCardsPos[posIndex].transform.position;
                            FightCardData dataTemp = FightForManagerForStart.instance.playerFightCardsDatas[goIndexPos];
                            FightForManagerForStart.instance.playerFightCardsDatas[goIndexPos] = FightForManagerForStart.instance.playerFightCardsDatas[posIndex];
                            FightForManagerForStart.instance.playerFightCardsDatas[goIndexPos].posIndex = goIndexPos;
                            FightForManagerForStart.instance.playerFightCardsDatas[posIndex] = dataTemp;
                            FightForManagerForStart.instance.playerFightCardsDatas[posIndex].posIndex = posIndex;
                            go.GetComponent<CardDragForStart>().posIndex = posIndex;

                            FightForManagerForStart.instance.CardGoIntoBattleProcess(FightForManagerForStart.instance.playerFightCardsDatas[posIndex], posIndex, FightForManagerForStart.instance.playerFightCardsDatas, true);
                            FightForManagerForStart.instance.CardGoIntoBattleProcess(FightForManagerForStart.instance.playerFightCardsDatas[goIndexPos], goIndexPos, FightForManagerForStart.instance.playerFightCardsDatas, true);

                            EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManagerForStart.instance.playerCardsPos[posIndex].transform);
                            EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManagerForStart.instance.playerCardsPos[goIndexPos].transform);

                            posIndex = goIndexPos;
                            FightControlForStart.instance.PlayAudioForSecondClip(85
                                , 0);
                        }
                        //拖动牌原位置 在备战位
                        else
                        {
                            transform.SetParent(FightForManagerForStart.instance.playerCardsBox);
                            transform.position = go.transform.position;
                            transform.GetChild(8).gameObject.SetActive(true);

                            FightForManagerForStart.instance.CardGoIntoBattleProcess(FightForManagerForStart.instance.playerFightCardsDatas[goIndexPos], goIndexPos, FightForManagerForStart.instance.playerFightCardsDatas, false);

                            go.transform.SetParent(FightForManagerForStart.instance.herosCardListTran);

                            int index = FightForManagerForStart.instance.FindDataFromCardsDatas(gameObject);
                            if (index != -1)
                            {
                                FightForManagerForStart.instance.playerFightCardsDatas[go.GetComponent<CardDragForStart>().posIndex] = FightForManagerForStart.instance.playerCardsDatas[index];
                                FightForManagerForStart.instance.playerFightCardsDatas[go.GetComponent<CardDragForStart>().posIndex].posIndex = go.GetComponent<CardDragForStart>().posIndex;
                            }
                            go.GetComponent<CardDragForStart>().posIndex = -1;
                            go.transform.GetChild(8).gameObject.SetActive(false);
                            posIndex = goIndexPos;

                            FightForManagerForStart.instance.CardGoIntoBattleProcess(FightForManagerForStart.instance.playerFightCardsDatas[posIndex], posIndex, FightForManagerForStart.instance.playerFightCardsDatas, true);
                            FightControlForStart.instance.PlayAudioForSecondClip(85, 0);

                            EffectsPoolingControl.instance.GetEffectToFight1("toBattle", 0.7f, FightForManagerForStart.instance.playerCardsPos[posIndex].transform);
                        }
                    }
                    //目的地 卡牌在备战位
                    else
                    {
                        if (posIndex != -1)
                        {
                            FightForManagerForStart.instance.CardGoIntoBattleProcess(FightForManagerForStart.instance.playerFightCardsDatas[posIndex], posIndex, FightForManagerForStart.instance.playerFightCardsDatas, false);
                            FightForManagerForStart.instance.playerFightCardsDatas[posIndex] = null;
                        }
                        transform.SetParent(FightForManagerForStart.instance.herosCardListTran);
                        transform.GetChild(8).gameObject.SetActive(false);
                        posIndex = -1;
                    }
                }
            }
            else //目的地为其他
            {
                if (posIndex != -1) //原位置在上阵位
                {
                    FightForManagerForStart.instance.CardGoIntoBattleProcess(FightForManagerForStart.instance.playerFightCardsDatas[posIndex], posIndex, FightForManagerForStart.instance.playerFightCardsDatas, false);
                    FightForManagerForStart.instance.playerFightCardsDatas[posIndex] = null;
                }
                transform.SetParent(FightForManagerForStart.instance.herosCardListTran);
                transform.GetChild(8).gameObject.SetActive(false);
                posIndex = -1;
            }
            transform.GetComponent<Image>().raycastTarget = true;
        }
    }
}