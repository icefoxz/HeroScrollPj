using System;
using System.Collections;
using System.Collections.Generic;
using Beebyte.Obfuscator;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryEventUIController : MonoBehaviour
{
    public List<StoryEventPoint> storyEventPoints;
    public List<GameObject> eventTypePrefabs;
    [Header("对应上面事件的音效，-1为为随机古筝，其余负数为无音效。")]
    public List<int> eventTypeAudioIds;
    private Dictionary<int, StoryEventPoint> points;

    public void ResetUi()
    {
        if(SceneManager.GetActiveScene().buildIndex != 1)return;//如果不是主场景不更新。
        storyEventPoints.ForEach(b =>
        {
            if (!b) return;
            b.gameObject.SetActive(false);
            if (b.content)
                Destroy(b.content);
        });
        var storyMap = PlayerDataForGame.instance.warsData.baYe.storyMap;
        points = new Dictionary<int, StoryEventPoint>();
        for (int i = 0; i < storyEventPoints.Count; i++)
        {
            var point =  storyEventPoints[i];
            var isContainEvent = storyMap.ContainsKey(i);
            point.gameObject.SetActive(isContainEvent);
            if (!isContainEvent) continue;
            var sEvent = storyMap[i];
            point.content = Instantiate(eventTypePrefabs[sEvent.Type - 1],point.transform);//由于0为无事件，所以第一个是事件1的图标
            points.Add(i, point);
        }
    }

    [SkipRename]public void OnStoryEventClick(int eventPoint)
    {
        var sEvent = PlayerDataForGame.instance.warsData.baYe.storyMap[eventPoint];
        OnClickAudioPlay(sEvent.Type);
        BaYeManager.instance.OnBaYeWarEventPointSelected(BaYeManager.EventTypes.Story, eventPoint);
        if ((BaYeManager.StoryEventTypes) sEvent.Type == BaYeManager.StoryEventTypes.讨伐)
            return; //讨伐事件ui在第一次点击是不会销毁的。
        var point = points[eventPoint];
        Destroy(point.content);
        point.gameObject.SetActive(false);
    }

    private void OnClickAudioPlay(int type)
    {
        if (type > eventTypeAudioIds.Count) return;
        var audioId = eventTypeAudioIds[type - 1];
        if (audioId < -1) return;
        if (audioId == -1)
        {
            AudioController0.instance.RandomPlayGuZhengAudio();
            return;
        }
        AudioController0.instance.ForcePlayAudio(audioId);
    }
}