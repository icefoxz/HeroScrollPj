using UnityEngine;
using UnityEngine.UI;

public class FlagUi : MonoBehaviour
{
    public Text Short;
    public Image Image;
    
    public void Set(string flagShort,int colorIndex)
    {
        Short.text = flagShort;
        Image.sprite = GameResources.Instance.CityFlag[colorIndex];
    }
}