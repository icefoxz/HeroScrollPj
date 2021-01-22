using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryEventUIController : MonoBehaviour
{
    public List<Button> StoryEventPoints;
    public List<Sprite> StoryEventImages;
    private Dictionary<int, Button> buttons;

    public void ResetUI()
    {
        StoryEventPoints.ForEach(b => b.gameObject.SetActive(false));
        var storyMap = PlayerDataForGame.instance.warsData.baYe.storyMap;
        buttons = new Dictionary<int, Button>();
        for (int i = 0; i < StoryEventPoints.Count; i++)
        {
            var btn = StoryEventPoints[i];
            var isContainEvent = storyMap.ContainsKey(i);
            btn.interactable = isContainEvent;
            btn.gameObject.SetActive(isContainEvent);
            buttons.Add(i, btn);
            if (!isContainEvent) continue;
            var sEvent = storyMap[i];
            btn.image.sprite = StoryEventImages[sEvent.Type - 1];//由于0为无事件，所以第一个是事件1的图标
        }
    }

    public void OnStoryEventClick(int id)
    {
        var isSuccess = BaYeManager.instance.OnStoryEventTrigger(id);
        var btn = buttons[id];
        btn.interactable = false;
        btn.gameObject.SetActive(false);
        btn.image.sprite = null;
        if(isSuccess)return;
#if DEBUG
        var resultText = isSuccess ? "成功" : "失败";
        XDebug.Log<StoryEventUIController>($"霸业故事事件[{id}]获取{resultText}！");
#endif
    }
}
