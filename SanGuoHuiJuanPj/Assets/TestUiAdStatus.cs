using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUiAdStatus : MonoBehaviour
{
    public Text statusText;
    public Text timer;
    public Image bulb;
    public Button forceLoadButton;
    private IAdController adController;

    void Start()
    {
        adController = DoNewAdController.AdController;
        forceLoadButton.onClick.RemoveAllListeners();
        forceLoadButton.onClick.AddListener(OnForceLoad);
    }
    public void OnForceLoad()
    {
        adController.LoadRewardAd(true);
    }

    public void UpdateStatus()
    {
        var status = adController.Status;
        statusText.text = status.ToString();
        var color = Color.gray;
        switch (status)
        {
            case DoNewAdController.AdStates.None:
                break;
            case DoNewAdController.AdStates.RequestingCache:
                color = Color.yellow;
                break;
            case DoNewAdController.AdStates.Cached:
                color = Color.green;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        bulb.color = color;
        timer.text = adController.Timer.ToString();
    }
    // Update is called once per frame
    void Update() => UpdateStatus();
}