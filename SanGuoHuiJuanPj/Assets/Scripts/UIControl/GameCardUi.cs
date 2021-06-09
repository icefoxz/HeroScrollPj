using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CorrelateLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameCardUi : MonoBehaviour
{
    public enum CardModes
    {
        Desk,
        War
    }
    public CardModes Mode { get; private set; }
    public GameCardInfo CardInfo { get; private set; }
    public NowLevelAndHadChip Card { get; private set; }
    //persist components
    public Image Image;
    public Text Name;
    public Image Level;
    public TextImageUi Short;
    public Image Frame;
    //dynamic components
    public GameCardCityUiOperation CityOperation;
    public GameCardWarUiOperation WarOperation;

    public bool IsSelected
    {
        get
        {
            switch (Mode)
            {
                case CardModes.Desk:
                    return CityOperation.IsSelected;
                case CardModes.War:
                    return WarOperation.State == GameCardWarUiOperation.States.Selected;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void Set(NowLevelAndHadChip card,CardModes mode)
    {
        Card = card;
        CardInfo = card.GetInfo();
        Image.sprite = CardInfo.Type == GameCardType.Hero
            ? GameResources.Instance.HeroImg[card.CardId]
            : GameResources.Instance.FuZhuImg[CardInfo.ImageId];
        Short.Set(CardInfo.Short, GameResources.Instance.ClassImg[CardInfo.Type == GameCardType.Hero ? 0 : 1]);
        SetName(CardInfo);
        SetLevel(card.Level);
        SetFrame(false);
        SetMode(mode);
        gameObject.SetActive(true);
    }

    public void Off()
    {
        Selected(false);
        gameObject.SetActive(false);
    }

    public void SetMode(CardModes mode)
    {
        WarOperation.ResetUi();
        CityOperation.ResetUi();
        if(mode == CardModes.Desk)
        {
            CityOperation.Show(this);
        }

        if (mode == CardModes.War)
        {
            WarOperation.Show(this);
        }
        WarOperation.gameObject.SetActive(mode == CardModes.War);
        CityOperation.gameObject.SetActive(mode == CardModes.Desk);
    }

    public void Selected(bool isSelected)
    {
        switch (Mode)
        {
            case CardModes.Desk:
                CityOperation.SetSelected(isSelected);
                break;
            case CardModes.War:
                WarOperation.SetState(GameCardWarUiOperation.States.Selected);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetFrame(bool on) => Frame.gameObject.SetActive(on);
    public void SetName(GameCardInfo info)
    {
        NameTextSizeAlignment(Name, info.Name);
        Name.color = info.GetNameColor();
    }

    public void SetLevel(int level) => Level.sprite = GameResources.Instance.GradeImg[level];

    /// <summary> 
    /// 名字显示规则 
    /// </summary> 
    /// <param name="nameText"></param> 
    /// <param name="str"></param> 
    public static void NameTextSizeAlignment(Text nameText, string str)
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

}