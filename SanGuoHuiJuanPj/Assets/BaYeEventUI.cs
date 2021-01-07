using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaYeEventUI : MonoBehaviour
{
    public Image prefab;
    public Color defaultColor;
    public Color activeColor;
    public Text text;
    public RectTransform contentLayout;
    public int space = 13;
    public Button button;
    private List<Image> list;
    public Color cityNameColor;
    public Color defaultCityColor;
    public ForceFlagUI forceFlag;
    public void Init(int maxValue)
    {
        if (list != null && list.Count > 0) list.ForEach(f => Destroy(f.gameObject));
        list = new List<Image>();
        for (int i = 0; i < maxValue; i++)
        {
            var box = Instantiate(prefab, contentLayout);
            box.color = defaultColor;
            box.gameObject.SetActive(true);
            list.Add(box);
        }
        text.color = cityNameColor;
        contentLayout.gameObject.SetActive(true);
        contentLayout.sizeDelta = new Vector2(list.Count * space, contentLayout.sizeDelta.y);
        forceFlag.Hide();
    }
    public void InactiveCityColor()
    {
        text.color = defaultCityColor;
    }

    public void SetValue(int value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].color = i < value ? activeColor : defaultColor;
        }
    }
}
