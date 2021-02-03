using System;
using GoogleMobileAds.Api;
using UnityEngine.Events;

public class AdmobController : AdControllerBase
{
    public enum States
    {
        None,
        Loading,
        Loaded,
        FailedToLoad,
        Showing,
        Closed
    }
    public override AdModes Mode => AdModes.Preload;
    public States Status { get; private set; }
    public const string AdUnitId = "ca-app-pub-6126766415984891/1283385219";
    public const string TestId = "ca-app-pub-3940256099942544/5224354917";
    public string LastMessage { get; private set; }
    private ResponseEvent onLoad = new ResponseEvent();
    private ResponseEvent onClose = new ResponseEvent();
    private RewardedAd rewardedAd;

    void Start() => Init();

    public void RequestLoad(UnityAction<bool,string> loadingAction)
    {
        rewardedAd = new RewardedAd(AdUnitId);
        var request = new AdRequest.Builder().Build();
        onLoad.AddListener(loadingAction);
        OnStateChangeSubscriptionAction(States.Loading);
        rewardedAd.LoadAd(request);
    }

    public void RequestShow(UnityAction<bool,string> requestAction)
    {
        onClose.AddListener(requestAction);
        OnStateChangeSubscriptionAction(States.Showing);
        rewardedAd.Show();
    }

    #region 封装内部执行代码
    private void OnStateChangeSubscriptionAction(States status, string message = null)
    {
        this.Status = status;
        switch (this.Status)
        {
            case States.Loading:
                rewardedAd.OnAdLoaded += OnAdLoad;
                rewardedAd.OnAdFailedToLoad += OnAdFailedToLoad;
                return;
            case States.None:
                break;
            case States.FailedToLoad:
            case States.Loaded:
                rewardedAd.OnAdLoaded -= OnAdLoad;
                rewardedAd.OnAdFailedToLoad -= OnAdFailedToLoad;
                LastMessage = message;
                break;
            case States.Showing:
                rewardedAd.OnUserEarnedReward += OnUserEarnedReward;
                rewardedAd.OnAdFailedToShow += OnAdFailedToShow;
                break;
            case States.Closed:
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
        OnStateChangeSubscriptionAction(States.Loaded);
        onLoad.Invoke(true, string.Empty);
        onLoad.RemoveAllListeners();
    }

    private void OnAdFailedToLoad(object sender, AdErrorEventArgs e)
    {
        OnStateChangeSubscriptionAction(States.FailedToLoad, e.Message);
        onLoad.Invoke(false,e.Message);
        onLoad.RemoveAllListeners();
    }

    private void OnAdFailedToShow(object sender, AdErrorEventArgs e)
    {
        OnStateChangeSubscriptionAction(States.FailedToLoad, e.Message);
        onClose.Invoke(false,e.Message);
        onClose.RemoveAllListeners();
    }

    private void OnUserEarnedReward(object sender, Reward e)
    {
        OnStateChangeSubscriptionAction(States.Closed, $"{e.Amount}");
        onClose.Invoke(true,$"{e.Amount}");
        onClose.RemoveAllListeners();
    }
    #endregion

    private class ResponseEvent : UnityEvent<bool,string>{}
}