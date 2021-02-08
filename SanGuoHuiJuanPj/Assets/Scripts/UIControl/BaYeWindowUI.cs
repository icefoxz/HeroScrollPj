using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaYeWindowUI : MiniWindowUI
{
    private enum DisplayViews
    {
        Reward,
        Quest,
        Selection
    }
    public Transform questionView;
    public Transform selectionView;
    public Transform selectionContent;
    public BaYeLingSelectBtn selectionButtonPrefab;
    public Text question;
    public Button[] answer;
    public Button adBtn;
    public Button closeBtn;
    private List<BaYeLingSelectBtn> lingCaches;

    public override void Init()
    {
        base.Init();
        foreach (var btn in selectionView.GetComponentsInChildren<BaYeLingSelectBtn>(true))
            btn.gameObject.SetActive(false);
        lingCaches = new List<BaYeLingSelectBtn>();
    }

    public override void Show(Dictionary<int, int> rewardMap)
    {
        base.Show(rewardMap);
        DisplayViewChange(DisplayViews.Reward);
    }

    public void ShowQuest(string quest, string[] answerStrings, int answerIndex, Action onCorrectAction, Action onFaultyAction)
    {
        question.text = quest;
        for (int i = 0; i < answer.Length; i++)
        {
            var btn = answer[i];
            btn.GetComponentInChildren<Text>().text = answerStrings[i];
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(Close);
            if(answerIndex==i)
                btn.onClick.AddListener(onCorrectAction.Invoke);
            else btn.onClick.AddListener(onFaultyAction.Invoke);
        }
        DisplayViewChange(DisplayViews.Quest);
    }

    private void DisplayViewChange(DisplayViews display)
    {
        adBtn.gameObject.SetActive(display==DisplayViews.Reward);
        listView.gameObject.SetActive(display == DisplayViews.Reward);
        questionView.gameObject.SetActive(display == DisplayViews.Quest);
        selectionView.gameObject.SetActive(display == DisplayViews.Selection);
        gameObject.SetActive(true);
    }

    public void ShowSelection(Dictionary<int, int> rewardMap, UnityAction<int> recallAction)
    {
        closeBtn.gameObject.SetActive(false);
        foreach (var obj in rewardMap)
        {
            var orderId = obj.Key;
            var amt = obj.Value;
            var ling = Instantiate(selectionButtonPrefab, selectionContent);
            ling.Set((ForceFlags) orderId, amt);
            ling.gameObject.SetActive(true);
            ling.btn.onClick.AddListener(() =>
            {
                recallAction(orderId);
                OnPlayerSelected();
            });
            lingCaches.Add(ling);
        }
        DisplayViewChange(DisplayViews.Selection);
    }

    private void OnPlayerSelected()
    {
        if (lingCaches == null || lingCaches.Count == 0) return;
        foreach (var btn in lingCaches) 
            Destroy(btn.gameObject);
        lingCaches.Clear();
        Close();
    }

    public override void Close()
    {
        closeBtn.gameObject.SetActive(true);
        selectionView.gameObject.SetActive(false);
        questionView.gameObject.SetActive(false);
        adBtn.onClick.RemoveAllListeners();
        adBtn.gameObject.SetActive(false);
        base.Close();
    }

    public void ShowAdButton(UnityAction onClickAction)
    {
        adBtn.onClick.RemoveAllListeners();
        adBtn.onClick.AddListener(onClickAction);
        adBtn.gameObject.SetActive(true);
    }
}