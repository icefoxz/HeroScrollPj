using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WorldMapCityUi : MonoBehaviour
{
    public Button UiBtn;
    public Image Img;
    public Text Name;
    public FlagUi Flag;

    public void Set(int cityId,string cityName ,int iconIndex, UnityAction<int> onButtonClickAction)
    {
        Img.sprite = GameResources.Instance.CityIcon[iconIndex];
        Name.text = cityName;
        UiBtn.onClick.RemoveAllListeners();
        UiBtn.onClick.AddListener(() => onButtonClickAction(cityId));
    }
}