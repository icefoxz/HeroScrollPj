using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaYeMiniWindowUI : MonoBehaviour
{
    public GameObject BaYeMiniWindow;
    public MiniWindowElementUI prefab;
    public Sprite[] rewardImages;
    public Transform listView;
    public Transform questionView;
    public Text question;
    public Button[] answer;
    private List<MiniWindowElementUI> items = new List<MiniWindowElementUI>();

    public void Init()
    {
        gameObject.SetActive(false);
    }
    public void Show(Dictionary<int, int> rewardMap)
    {
        if (items.Count > 0) items.ForEach(element => element.gameObject.SetActive(false));
        var index = 0;
        foreach (var set in rewardMap)
        {
            if (items.Count == index)
            {
                var item = Instantiate(prefab, listView);
                items.Add(item);
            }
            items[index].image.sprite = rewardImages[set.Key];
            items[index].text.text = $"+{set.Value}";
            items[index].gameObject.SetActive(true);
            index++;
        }
        questionView.gameObject.SetActive(false);
        listView.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

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

    public void Close() => gameObject.SetActive(false);

    //关闭窗口
    public void CloseBaYeMiniWindow() 
    {
        BaYeMiniWindow.gameObject.SetActive(false);
    }
}