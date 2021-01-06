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

    void Awake()
    {
        selected?.gameObject.SetActive(false);
    }

    public void Set(UIManager.ForceFlags flag, bool display = true)
    {
        var index = -1;
        switch (flag)
        {
            case UIManager.ForceFlags.蜀:
            case UIManager.ForceFlags.魏:
            case UIManager.ForceFlags.吴:
            case UIManager.ForceFlags.袁:
            case UIManager.ForceFlags.吕:
                index = (int) flag;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(flag), flag, null);
        }

        forceFlag.sprite = UIManager.instance.forceFlags[index];
        forceName.sprite = UIManager.instance.forceName[index];
        gameObject.SetActive(display);
    }

    public void Hide() => gameObject.SetActive(false);

    public void Select(bool isSelected) => selected.gameObject.SetActive(isSelected);
}
