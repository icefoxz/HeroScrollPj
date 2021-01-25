using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryEventUIController : MonoBehaviour
{
    public List<StoryEventPoint> storyEventPoints;
    public List<GameObject> eventTypePrefabs;
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

    public void OnStoryEventClick(int id)
    {
        var sEvent = PlayerDataForGame.instance.warsData.baYe.storyMap[id];
        OnClickAudioPlay(sEvent.Type);
        var isSuccess = BaYeManager.instance.OnStoryEventTrigger(id);
        var point = points[id];
        Destroy(point.content);
        point.gameObject.SetActive(false);
        if(isSuccess)return;
#if UNITY_EDITOR
        var resultText = isSuccess ? "成功" : "失败";
        XDebug.Log<StoryEventUIController>($"霸业故事事件[{id}]获取{resultText}！");
#endif
    }
    private void OnClickAudioPlay(int type) 
    {
        switch (type) 
        {
            case 0:
                break;                
            case 1://宝箱
                AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[17], AudioController0.instance.audioVolumes[17]);
                break;
            case 2://答题
                AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[19], AudioController0.instance.audioVolumes[19]);
                break;       
        }
        AudioController0.instance.PlayAudioSource(0);
    }
}