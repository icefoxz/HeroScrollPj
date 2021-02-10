using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaYeForceSelectorUi : ForceSelectorUi
{
    //public Button button;
    //public Image image;
    //public ForceFlagUI forceFlag;

    //public bool displayLing;
    //// Start is called before the first frame update
    //public void Start() => DisplayLing(false);
    //public void DisplayLing(bool yes)
    //{
    //    displayLing = yes;
    //    image.gameObject.SetActive(yes);
    //}

    /// <summary>
    /// key = forceId, value = totalLing
    /// </summary>

    public override void Init(PlayerDataForGame.WarTypes warType)
    {
        base.Init(warType);
        UpdateZhanLing();
    }

    public void UpdateZhanLing()
    {
        var baYe = PlayerDataForGame.instance.warsData.baYe;
        foreach (var pair in Data)
        {
            var forceId = pair.Key;
            var flagUi = pair.Value;
            var amount = baYe.zhanLingMap[forceId];
            flagUi.SetLing(amount);
        }
    }
}

