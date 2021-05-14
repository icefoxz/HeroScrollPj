using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaoYuanChestUI : MonoBehaviour
{
    public Text value;
    public Button chestButton;
    public Text lasting;
    public GameObject opened;
    public GameObject closed;
    public void SetChest(bool isOpen)
    {
        opened.gameObject.SetActive(isOpen);
        closed.gameObject.SetActive(!isOpen);
    }

    public virtual void UpdateUi(string valueText, string lastingText)
    {
        lasting.text = lastingText;
        value.text = valueText;
    }
}