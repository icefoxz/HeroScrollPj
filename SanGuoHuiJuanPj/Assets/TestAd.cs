using System;
using System.Collections;
using System.Collections.Generic;
using Donews.mediation;
using UnityEngine;
using UnityEngine.UI;

public class TestAd : MonoBehaviour
{
    public Text reportText;

    public void Load() => AdAgent.instance.BusyRetry((msg)=>
    {
        Debug.Log($"Ad call reward = {msg}");
        reportText.text += $"Call : {msg}\n";
    }, () =>
    {
        reportText.text += "cancel!\n";
    });

}
