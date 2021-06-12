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

    public void Set(int cityId, UnityAction<int> onButtonClickAction)
    {
        var city = DataTable.City[cityId];
        Img.sprite = GameResources.Instance.CityIcon[city.Icon];
        Name.text = city.Name;
        Flag.Set(city.Short, city.Flag);
        UiBtn.onClick.RemoveAllListeners();
        UiBtn.onClick.AddListener(() => onButtonClickAction(cityId));
    }
}