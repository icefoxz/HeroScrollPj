using UnityEngine;

public class YvQueChestUI : TaoYuanChestUI
{
    public GameObject yvQueCostDisplay;
    public GameObject freeOpenDisplay;
    public void UpdateChest(string lastingText , bool isFreeOpen)
    {
        lasting.text = lastingText;
        freeOpenDisplay.gameObject.SetActive(isFreeOpen);
        yvQueCostDisplay.gameObject.SetActive(!isFreeOpen);
    }
}