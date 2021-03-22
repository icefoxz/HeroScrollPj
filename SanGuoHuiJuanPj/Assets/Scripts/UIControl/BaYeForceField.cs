﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 霸业势力Ui字段类，用于标记字段
/// </summary>
public class BaYeForceField : MonoBehaviour
{
    public int id =-1;
    public int boundCity = -1;
    public List<int> boundWars = new List<int>();
    public BaYeForceSelectorUi forceUi;
}