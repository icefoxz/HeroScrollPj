using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TradableGameCardUi : MonoBehaviour
{
    private enum States
    {
        Free,
        Sell,
        Ad
    }
    public GameCardUi GameCard;
    public GameCard Card { get; private set; }
    public Button BuyBtn;
    public Text FreeLabel;
    public Text PriceLabel;
    public Image AdLabel;
    public UnityEvent OnClickAction;
    private Dictionary<States, GameObject> stateSet;
    private Dictionary<States, GameObject> StateSet
    {
        get
        {
            if (stateSet == null)
            {
                stateSet = new Dictionary<States, GameObject>
                {
                    {States.Free, FreeLabel.gameObject},
                    {States.Sell, PriceLabel.gameObject},
                    {States.Ad, AdLabel.gameObject}
                };
            }
            return stateSet;
        }
    }

    void Start() => GameCard.CityOperation.OnclickAction.AddListener(()=>OnClickAction.Invoke());

    public void SetAd(UnityAction action)
    {
        SetState(States.Ad);
        BuyBtn.onClick.RemoveAllListeners();
        BuyBtn.onClick.AddListener(action);
    }

    public void SetPrice(int price, UnityAction buyAction)
    {
        PriceLabel.text = price.ToString();
        SetState(price == 0 ? States.Free : States.Sell);
        BuyBtn.onClick.RemoveAllListeners();
        BuyBtn.onClick.AddListener(buyAction);
    }

    private void SetState(States state)
    {
        foreach (var item in StateSet) item.Value.SetActive(item.Key == state);
    }

    public void SetSelect(bool isSelected) => GameCard.Selected(isSelected);

    public void SetGameCard(GameCard card)
    {
        Card = card;
        gameObject.SetActive(true);
        GameCard.Set(card,GameCardUi.CardModes.Desk);
        GameCard.CityOperation.OffChipValue();
    }

    public void Off() => gameObject.SetActive(false);
}