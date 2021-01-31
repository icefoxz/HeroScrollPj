using System;
using System.Collections;
using System.Collections.Generic;
using Donews.mediation;
using UnityEngine;

public class TestAd : MonoBehaviour
{
    public void Call()
    {
        try
        {
            SDK.DnSdkObj.Call("requestDirectAd", SDK.PlaceId, new RewardVideoCallBack());
        }
        catch (Exception e)
        {
            throw XDebug.Throw<TestAd>(e.ToString());
        }
    }
    public class RewardVideoCallBack : AndroidJavaProxy
{
    void onAdError(string msg) => XDebug.Log($"error = {msg}",nameof(onAdError),nameof(RewardVideoCallBack));

    void onAdShow() => XDebug.Log("Invoke()",nameof(onAdShow),nameof(RewardVideoCallBack));

    void onAdClick() => XDebug.Log("Invoke()",nameof(onAdClick),nameof(RewardVideoCallBack));

    void onAdClose()=> XDebug.Log("Invoke()",nameof(onAdClose),nameof(RewardVideoCallBack));

    void onVideoComplete()=> XDebug.Log("Invoke()",nameof(onVideoComplete),nameof(RewardVideoCallBack));

    void onRewardVerify(bool rewardVerify)=> XDebug.Log($"verify = {rewardVerify}",nameof(onRewardVerify),nameof(RewardVideoCallBack));

    void onSkippedVideo()=> XDebug.Log("Invoke()",nameof(onSkippedVideo),nameof(RewardVideoCallBack));

    public RewardVideoCallBack() : base("com.donews.android.RewardVideoCallBack")
    {
    }

}
}
