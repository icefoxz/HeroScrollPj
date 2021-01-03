﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaYeProgressUI : MonoBehaviour
{
    public Slider slider;
    public Text displayText;

    public void Set(float value,float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = value;
        displayText.text = $"当前经验：{value}";
    }
}