using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ForceSelectorUi : MonoBehaviour
{
    //public Sprite[] flags;
    //public Sprite[] forceName;
    public Button button;
    public HorizontalLayoutGroup content;

    //private Dictionary<int, Image[]> data = new Dictionary<int, Image[]>();
    private Dictionary<int, ForceFlagUI> data = new Dictionary<int, ForceFlagUI>();
    //0=旗帜，1=势力名字，2=光圈
    //private Image[] images;
    private int selectedId;
    public void Start()
    {
        int totalUnlockForce =
            int.Parse(LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.Level - 1][6]);

        for (int i = 0; i < totalUnlockForce + 1; i++)
        {
            int forceIndex = i;
            var btn = Instantiate(button); //复制按钮
            btn.transform.SetParent(content.transform); //把按钮贴在控制器
            btn.transform.localScale = Vector3.one;
            btn.gameObject.name = forceIndex.ToString();
            //var list = btn.GetComponentsInChildren<Image>(true);
            //images = list.OrderBy(img => int.Parse(img.name)).ToArray();//获取子物件的列，根据小到大排列
            //images[0].sprite = flags[forceIndex];
            //images[1].sprite = forceName[forceIndex];
            var forceFlag = btn.GetComponentInChildren<ForceFlagUI>(true);
            forceFlag.Set((UIManager.ForceFlags)forceIndex);
            data.Add(forceIndex, forceFlag);
            btn.onClick.AddListener(() =>
            {
                OnSelected(forceIndex);
                ChooseCardForBattle(forceIndex);
            });
            btn.gameObject.SetActive(true);
        }
    }

    public void UpdateForceSelector()
    {

        OnSelected(selectedId);
        ChooseCardForBattle(selectedId);
    }

    public void OnSelected(int forceId)
    {
        foreach (var obj in data)
        {
            var ui = obj.Value;
            var force = obj.Key;
            ui.Select(force == forceId);
        }
        selectedId = forceId;
    }

    public void ChooseCardForBattle(int forceId) => BaYeManager.instance.SelectBaYeForceCards(forceId);
}
