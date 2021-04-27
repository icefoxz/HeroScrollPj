using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 势力旗帜UI
/// </summary>
public class ForceFlagUI : MonoBehaviour
{
    public Image forceFlag;
    public Image forceName;
    public Image selected;
    public Image panel;
    public Text lingText;
    public GameObject lingObj;
    public Text nameText;
    public Text panelText;
    public bool Selected => selected.gameObject.activeSelf;

    public void Set(ForceFlags flag, bool display = true, string nameInText = null)
    {
        var resources = GameSystem.GameResources;
        forceFlag.sprite = resources.ForceFlag[flag];
        nameText.gameObject.SetActive(nameInText != null);
        forceName.gameObject.SetActive(nameInText == null);
        if (nameInText == null) forceName.sprite = resources.ForceName[flag];
        else nameText.text = nameInText;
        gameObject.SetActive(display);
    }

    public void Hide() => gameObject.SetActive(false);

    public void Select(bool isSelected) => selected.gameObject.SetActive(isSelected);

    public void Interaction(bool enable, string text = null)
    {
        panel.gameObject.SetActive(!enable);
        panelText.text = text;
    }

    public void SetLing(int amount, bool display = true)
    {
        lingText.text = amount.ToString();
        lingObj.gameObject.SetActive(display);
    }
}
