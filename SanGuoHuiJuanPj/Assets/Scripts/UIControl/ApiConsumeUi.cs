using System;
using CorrelateLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ApiConsumeUi : ConsumeBtnUi
{
    public enum Currencies
    {
        YvQue,
        YuanBao
    }

    public Currencies Currency { get; private set; }
    private bool IsEdit { get; set; }
    public GameObject Edit;
    private int Cost { get; set; }
    private UnityAction apiAction;
    private Selectable[] coms;
    public UnityEvent OnEditAction;
    public void SetCost(Currencies currency,int cost)
    {
        Currency = currency;
        Cost = cost;
    }

    public void ResetApi(Func<bool> onSuccessInvokeApiAction,string apiEvent, UnityAction<ViewBag> successAction, UnityAction<string> failedAction = null)
    {
        if (failedAction == null) failedAction = PlayerDataForGame.instance.ShowStringTips;
        Button.onClick.RemoveAllListeners();
        apiAction = () =>
        {
            if (onSuccessInvokeApiAction.Invoke()) ApiPanel.instance.Invoke(successAction, failedAction, apiEvent);
        };
        Button.onClick.AddListener(ButtonClick);
        SetBtnEdit(true);
    }

    private void ButtonClick()
    {        
        if(!IsEdit)
        {
            apiAction.Invoke();
            return;
        }
        SetBtnEdit(false);
    }

    private void UpdateUi()
    {
        UpdateInteractive();
        Edit.SetActive(IsEdit);
        if (IsEdit)
        {
            SetNone();
            OnEditAction.Invoke();
            return;
        }
        if (Cost == 0)
        {
            SetFree();
            return;
        }

        switch (Currency)
        {
            case Currencies.YvQue:
                SetYuQue(Cost);
                break;
            case Currencies.YuanBao:
                SetYuanBao(Cost);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    internal void SetEditInteractive(Selectable[] com)
    {
        coms = com;
        UpdateInteractive();
    }

    private void UpdateInteractive()
    {
        if (coms == null) return;
        foreach (var com in coms)
        {
            com.interactable = !IsEdit;
        }
    }

    private void SetBtnEdit(bool isEdit)
    {
        IsEdit = isEdit;
        UpdateUi();        
    }
}