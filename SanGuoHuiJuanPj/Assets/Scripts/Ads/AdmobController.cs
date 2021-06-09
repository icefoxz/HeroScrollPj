using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class AdmobController : AdControllerBase
{
    public AdModes Mode => AdModes.Preload;

    public override AdAgentBase.States Status => _status;

    public const string TestId = "ca-app-pub-3940256099942544/5224354917";
    public const string AdUnitId = "ca-app-pub-6126766415984891/1283385219";
    public int Retries = 3;
    public string LastMessage { get; private set; }
    private ResponseEvent onLoad = new ResponseEvent();
    private ResponseEvent onClose = new ResponseEvent();
    private RewardedAd rewardedAd;
    private AdAgentBase.States _status;

    public bool IsReady => _status == AdAgentBase.States.Loaded;

    public void Init(Action<bool, string> admobCallBack)
    {
#if !UNITY_EDITOR
        MobileAds.Initialize(_ => { });
        OnLoadAd(admobCallBack);
#endif
    }

    IEnumerator OnRetryLoadingAction(Action<bool, string> admobCallBack)
    {
        _status = AdAgentBase.States.Loading;
        RequestLoad(
            (success, _) => { _status = success ? AdAgentBase.States.Loaded : AdAgentBase.States.FailedToLoad; });
        yield return new WaitWhile(() => _status == AdAgentBase.States.Loading);
        if (_status == AdAgentBase.States.Loaded)
        {
            admobCallBack?.Invoke(true, string.Empty);
            yield return null;
        }
        admobCallBack?.Invoke(false, string.Empty);
    }

    public void OnLoadAd(Action<bool, string> admobCallBack)
    {
        StopAllCoroutines();
        StartCoroutine(OnRetryLoadingAction(admobCallBack));
    }

    public override void RequestLoad(UnityAction<bool,string> loadingAction)
    {
#if UNITY_EDITOR
        rewardedAd = new RewardedAd(TestId);
#else
        rewardedAd = new RewardedAd(AdUnitId);
#endif
        var request = new AdRequest.Builder().Build();
        onLoad.AddListener(loadingAction);
        OnStateChangeSubscriptionAction(AdAgentBase.States.Loading);
        rewardedAd.LoadAd(request);
    }

    public override void RequestShow(UnityAction<bool,string> requestAction)
    {
        onClose.AddListener(requestAction);
        OnStateChangeSubscriptionAction(AdAgentBase.States.Showing);
        rewardedAd.Show();
    }

    #region 封装内部执行代码
    private void OnStateChangeSubscriptionAction(AdAgentBase.States status, string message = null)
    {
        _status = status;
        switch (Status)
        {
            case AdAgentBase.States.Loading:
                rewardedAd.OnAdLoaded += OnAdLoad;
                rewardedAd.OnAdFailedToLoad += OnAdFailedToLoad;
                return;
            case AdAgentBase.States.None:
                break;
            case AdAgentBase.States.FailedToLoad:
            case AdAgentBase.States.Loaded:
                rewardedAd.OnAdLoaded -= OnAdLoad;
                rewardedAd.OnAdFailedToLoad -= OnAdFailedToLoad;
                LastMessage = message;
                break;
            case AdAgentBase.States.Showing:
                rewardedAd.OnUserEarnedReward += OnUserEarnedReward;
                rewardedAd.OnAdFailedToShow += OnAdFailedToShow;
                break;
            case AdAgentBase.States.Closed:
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
        OnStateChangeSubscriptionAction(AdAgentBase.States.Loaded);
        onLoad.Invoke(true, string.Empty);
        onLoad.RemoveAllListeners();
    }

    private void OnAdFailedToLoad(object sender, AdErrorEventArgs e)
    {
        OnStateChangeSubscriptionAction(AdAgentBase.States.FailedToLoad, e.Message);
        onLoad.Invoke(false,e.Message);
        onLoad.RemoveAllListeners();
    }

    private void OnAdFailedToShow(object sender, AdErrorEventArgs e)
    {
        OnStateChangeSubscriptionAction(AdAgentBase.States.FailedToLoad, e.Message);
        onClose.Invoke(false,e.Message);
        onClose.RemoveAllListeners();
    }

    private void OnUserEarnedReward(object sender, Reward e)
    {
        OnStateChangeSubscriptionAction(AdAgentBase.States.Closed, $"{e.Amount}");
        onClose.Invoke(true,$"{e.Amount}");
        onClose.RemoveAllListeners();
    }
    #endregion

    private class ResponseEvent : UnityEvent<bool,string>{}

}