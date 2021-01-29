using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Donews.mediation;
using UnityEngine;

public class RewardAdController : MonoBehaviour
{
    public RewardAdUi[] List;
    void Start()
    {
        ResetBtn();
    }

    public async void ResetBtn()
    {
        foreach (var adUi in List)
        {
            adUi.message.text = string.Empty;
        }

        foreach (var rewardAdUi in List)
        {
            try
            {
                var rd = RewardedVideoAd.LoadRewardedVideoAd(DoNewAdController.PlaceId, rewardAdUi);
                rewardAdUi.Init(rd);
            }
            catch (Exception e)
            {
                rewardAdUi.Init(null, e.ToString());
            }
            await Task.Delay(300);
        }
    }
}
