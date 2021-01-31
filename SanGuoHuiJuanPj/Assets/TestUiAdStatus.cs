using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUiAdStatus : MonoBehaviour
{
    public Text statusText;
    public Text timer;
    public Text mode;
    public Image modeBulb;
    public Image bulb;
    public Button modeButton;
    public Button forceLoadButton;
    private IAdController adController;

    //void Start()
    //{
    //    adController = DoNewAdController.AdController;
    //    forceLoadButton.onClick.RemoveAllListeners();
    //    forceLoadButton.onClick.AddListener(()=>adController.LoadRewardAd(true));
    //    modeButton.onClick.RemoveAllListeners();
    //    modeButton.onClick.AddListener(()=>adController.SetPreloadMode(!adController.IsPreloadMode));
    //}

    //public void UpdateStatus()
    //{
    //    var status = adController.Status;
    //    statusText.text = status.ToString();
    //    var color = Color.gray;
    //    modeBulb.color = adController.IsPreloadMode ? Color.cyan : Color.magenta;
    //    mode.text = adController.IsPreloadMode ? "预加载模式" : "旧广告模式";
    //    switch (status)
    //    {
    //        case DoNewAdController.AdStates.None:
    //            break;
    //        case DoNewAdController.AdStates.RequestingCache:
    //            color = Color.yellow;
    //            break;
    //        case DoNewAdController.AdStates.Cached:
    //            color = Color.green;
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException();
    //    }
    //    bulb.color = color;
    //    timer.text = adController.Timer.ToString();
    //}
    //// Update is called once per frame
    //void Update() => UpdateStatus();
}