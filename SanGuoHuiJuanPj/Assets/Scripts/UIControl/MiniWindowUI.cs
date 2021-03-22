using System.Collections.Generic;
using UnityEngine;

public class MiniWindowUI : MonoBehaviour
{
    public MiniWindowElementUI prefab;
    public Sprite[] rewardImages;
    public Transform listView;
    private List<MiniWindowElementUI> items = new List<MiniWindowElementUI>();
    public virtual void Init()
    {
        foreach (var ui in listView.GetComponentsInChildren<MiniWindowElementUI>(true))
            ui.gameObject.SetActive(false);
        listView.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public virtual void Show(Dictionary<int, int> rewardMap)
    {
        if (items.Count > 0) items.ForEach(element => element.gameObject.SetActive(false));
        var index = 0;
        foreach (var set in rewardMap)
        {
            if (set.Value <= 0) continue;
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
        listView.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        listView.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}