using System;
using UnityEngine.Events;

public class MoPubController : AdControllerBase
{
    public override AdAgentBase.States Status => status;

    private const string AdUnit = "e2cad8daa80d43aea756699efa66a6a1";
    private UnityAction<bool, string> loadingAction;
    private UnityAction<bool, string> requestAction;
    private AdAgentBase.States status;
    private bool isRewardReceived;
    public bool HasRewardVideo => MoPub.HasRewardedVideo(AdUnit);

    public void Init()
    {
        MoPub.InitializeSdk(AdUnit);
        MoPubManager.OnRewardedVideoLoadedEvent += OnRewardVideoLoaded;
        MoPubManager.OnRewardedVideoFailedEvent += OnRewardVideoLoadFailed;
        MoPubManager.OnRewardedVideoReceivedRewardEvent += (arg1, arg2, value) => isRewardReceived = true;
        MoPubManager.OnRewardedVideoClosedEvent += msg => requestAction?.Invoke(isRewardReceived, msg);
    }

    public override void RequestShow(UnityAction<bool, string> requestAction)
    {
        MoPub.ShowRewardedVideo(AdUnit);
    }

    public override void RequestLoad(UnityAction<bool, string> loadingAction)
    {
        status = AdAgentBase.States.Loading;
        MoPub.RequestRewardedVideo(AdUnit);
    }

    private void OnRewardVideoLoaded(string msg)
    {
        status = AdAgentBase.States.Loaded;
        loadingAction?.Invoke(true, msg);
    }

    private void OnRewardVideoLoadFailed(string arg1, string arg2)
    {
        status = AdAgentBase.States.FailedToLoad;
        loadingAction?.Invoke(false, $"{arg1}, {arg2}");
    }
}