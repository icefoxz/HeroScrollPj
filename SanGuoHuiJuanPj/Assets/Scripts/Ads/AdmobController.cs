using System;
using GoogleMobileAds.Api;
using UnityEngine.Events;

public class AdmobController : AdControllerBase
{
    public AdModes Mode => AdModes.Preload;

    public override AdAgent.States Status => status;

    public const string AdUnitId = "ca-app-pub-6126766415984891/1283385219";
    public const string TestId = "ca-app-pub-3940256099942544/5224354917";
    public string LastMessage { get; private set; }
    private ResponseEvent onLoad = new ResponseEvent();
    private ResponseEvent onClose = new ResponseEvent();
    private RewardedAd rewardedAd;
    private AdAgent.States status;

    public override void RequestLoad(UnityAction<bool,string> loadingAction)
    {
        rewardedAd = new RewardedAd(AdUnitId);
        var request = new AdRequest.Builder().Build();
        onLoad.AddListener(loadingAction);
        OnStateChangeSubscriptionAction(AdAgent.States.Loading);
        rewardedAd.LoadAd(request);
    }

    public override void RequestShow(UnityAction<bool,string> requestAction)
    {
        onClose.AddListener(requestAction);
        OnStateChangeSubscriptionAction(AdAgent.States.Showing);
        rewardedAd.Show();
    }

    #region 封装内部执行代码
    private void OnStateChangeSubscriptionAction(AdAgent.States status, string message = null)
    {
        this.status = status;
        switch (Status)
        {
            case AdAgent.States.Loading:
                rewardedAd.OnAdLoaded += OnAdLoad;
                rewardedAd.OnAdFailedToLoad += OnAdFailedToLoad;
                return;
            case AdAgent.States.None:
                break;
            case AdAgent.States.FailedToLoad:
            case AdAgent.States.Loaded:
                rewardedAd.OnAdLoaded -= OnAdLoad;
                rewardedAd.OnAdFailedToLoad -= OnAdFailedToLoad;
                LastMessage = message;
                break;
            case AdAgent.States.Showing:
                rewardedAd.OnUserEarnedReward += OnUserEarnedReward;
                rewardedAd.OnAdFailedToShow += OnAdFailedToShow;
                break;
            case AdAgent.States.Closed:
                rewardedAd.OnUserEarnedReward -= OnUserEarnedReward;
                rewardedAd.OnAdFailedToShow -= OnAdFailedToShow;
                LastMessage = message;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

#if UNITY_EDITOR
    public void TestOnAdLoad() => OnAdLoad(null, null);
    public void TestOnAdFailedToLoad() => OnAdFailedToLoad(null, new AdErrorEventArgs {Message = "test failed"});
    public void TestOnAdFailedToShow() =>
        OnAdFailedToShow(null, new AdErrorEventArgs {Message = "test failed to show"});
    public void TestOnUserEarnedReward() => OnUserEarnedReward(null, new Reward {Amount = 5});
#endif

    private void OnAdLoad(object sender, EventArgs e)
    {
        OnStateChangeSubscriptionAction(AdAgent.States.Loaded);
        onLoad.Invoke(true, string.Empty);
        onLoad.RemoveAllListeners();
    }

    private void OnAdFailedToLoad(object sender, AdErrorEventArgs e)
    {
        OnStateChangeSubscriptionAction(AdAgent.States.FailedToLoad, e.Message);
        onLoad.Invoke(false,e.Message);
        onLoad.RemoveAllListeners();
    }

    private void OnAdFailedToShow(object sender, AdErrorEventArgs e)
    {
        OnStateChangeSubscriptionAction(AdAgent.States.FailedToLoad, e.Message);
        onClose.Invoke(false,e.Message);
        onClose.RemoveAllListeners();
    }

    private void OnUserEarnedReward(object sender, Reward e)
    {
        OnStateChangeSubscriptionAction(AdAgent.States.Closed, $"{e.Amount}");
        onClose.Invoke(true,$"{e.Amount}");
        onClose.RemoveAllListeners();
    }
    #endregion

    private class ResponseEvent : UnityEvent<bool,string>{}
}