using System;
using UnityEngine;
using UnityEngine.UI;

public class BaYeWindowUI : MiniWindowUI
{
    public Transform questionView;
    public Text question;
    public Button[] answer;

    public void Show(string quest, string[] answerStrings, int answerIndex, Action onCorrectAction, Action onFaultyAction)
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
        listView.gameObject.SetActive(false);
        questionView.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        questionView.gameObject.SetActive(false);
        base.Close();
    }
}