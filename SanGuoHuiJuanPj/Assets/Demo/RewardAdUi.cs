using System.Collections;
using System.Collections.Generic;
using Donews.mediation;
using UnityEngine;
using UnityEngine.UI;

public class RewardAdUi : MonoBehaviour, IRewardedVideoAdListener
{
    public Image bulb;
    public Button play;
    public Text message;

    public void Init(RewardedVideoAd ad,string msg = "Start Loading...")
    {
        message.text = msg;
        if (ad != null)
        {
            play.onClick.RemoveAllListeners();
            play.onClick.AddListener(() =>
            {
                message.text = "Requesting ad....";
                ad.ShowRewardedVideoAd();
            });
        }

        play.interactable = ad != null;
    }

    public void RewardVideoAdDidLoadSuccess()
    {
        bulb.color = Color.green;
        message.text = "LoadSuccess!";
    }

    public void RewardVideoAdDidLoadFaild(int errorCode, string errorMsg)
    {
        bulb.color = Color.red;
        message.text = $"Code[{errorCode}]:{errorMsg}";
    }

    public void RewardVideoAdVideoDidLoad()
    {
        bulb.color = Color.yellow;
        message.text = "Load!";
    }

    public void RewardVideoAdWillVisible()
    {
        bulb.color = Color.gray;
        message.text = "Will Visible!";
    }

    public void RewardVideoAdExposured()
    {
        bulb.color = Color.black;
        message.text = "Expose!";
    }

    public void RewardVideoAdDidClose()
    {
        bulb.color = Color.blue;
        message.text = "Close!";
    }

    public void RewardVideoAdDidClicked()
    {
        bulb.color = Color.magenta;
        message.text = "Clicked!";
    }

    public void RewardVideoAdDidRewardEffective()
    {
        bulb.color = Color.white;
        message.text = "effective!";
    }

    public void RewardVideoAdDidPlayFinish()
    {
        bulb.color = Color.cyan;
        message.text = "Completed Video!";
    }
}
