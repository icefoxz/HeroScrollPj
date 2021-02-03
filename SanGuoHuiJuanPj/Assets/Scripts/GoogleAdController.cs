using System;
using System.Collections;
using System.Threading;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

public class GoogleAdController : AdControllerBase, IAdController
{
    public enum States
    {
        None,
        Load,
        Loading,
        Show,
        Showing,
        Error
    }

    public static IAdController AdController => instance;
    private static GoogleAdController instance;
    public States Status { get; private set; }
    public override AdModes Mode => AdModes.DirectLoad;
    public int Timer { get; }
    public const string AdUnitId = "ca-app-pub-6126766415984891/1283385219";
    public const string TestId = "ca-app-pub-3940256099942544/5224354917";
    private float repeatSecs = 0.5f;
    private float runningSecs;
    private float retrySecs = 30;//半分钟重试请求
    private RewardedAd rewardedAd;
    private UnityEvent onSuccess = new UnityEvent();
    private UnityEvent onFailed = new UnityEvent();
    private UnityEvent onLoad = new UnityEvent();

    void Start()
    {
        if (instance != null && instance != this)
            throw XDebug.Throw<DoNewAdController>("Duplicate singleton instance!");
        instance = this;
        MobileAds.Initialize(_=>{});
        Init();
        StartCoroutine(RepeatingLoadAd());
    }

    private IEnumerator RepeatingLoadAd()
    {
        while (true)
        {
            switch (Status)
            {
                case States.None:
                    if(runningSecs==0 || runningSecs>=retrySecs)
                    {
                        RequestLoad();
                        runningSecs = 0;
                    }
                    runningSecs += repeatSecs;
                    break;
                case States.Show:
                    Status = States.None;
                    runningSecs = 0;
                    break;
                case States.Load:
                case States.Showing:
                case States.Loading:
                    break;
                case States.Error:
                    Status = States.None;
                    runningSecs = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            yield return new WaitForSeconds(repeatSecs);
        }
    }

    public void RequestLoad()
    {
        rewardedAd = new RewardedAd(AdUnitId);
        var request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);
        Status = States.Loading;
        rewardedAd.OnAdLoaded += OnAdLoad;
        rewardedAd.OnAdFailedToLoad += OnAdFailedToLoad;
    }

    public void RequestShow()
    {
        Status = States.Showing;
        rewardedAd.Show();
        rewardedAd.OnUserEarnedReward += OnUserEarnedReward;
        rewardedAd.OnAdFailedToShow += OnAdFailedToShow;
    }

    private void OnLoadRecall(States status)
    {
        Status = status;
        rewardedAd.OnAdLoaded -= OnAdLoad;
        rewardedAd.OnAdFailedToLoad -= OnAdFailedToLoad;
    }

    private void OnShowRecall(States status)
    {
        Status = status;
        rewardedAd.OnUserEarnedReward -= OnUserEarnedReward;
        rewardedAd.OnAdFailedToShow -= OnAdFailedToShow;
    }

    private void OnAdLoad(object sender, EventArgs e)
    {
        OnLoadRecall(States.Load);
        onLoad.Invoke();
    }

    private void OnAdFailedToLoad(object sender, AdErrorEventArgs e) => OnLoadRecall(States.Error);

    private void OnAdFailedToShow(object sender, AdErrorEventArgs e)
    {
        OnLoadRecall(States.Error);
        onFailed.Invoke();
    }

    private void OnUserEarnedReward(object sender, Reward e)
    {
        OnShowRecall(States.Show);
        onSuccess.Invoke();
    }

    public void RequestRewardAd(Action onSuccessAction, CancellationTokenSource tokenSource)
    {
#if UNITY_EDITOR
        onSuccess.RemoveAllListeners();
        onSuccess.AddListener(()=>onSuccessAction());
        OnUserEarnedReward(null, null);
        return;
#endif
        if (Status == States.Load)
        {
            onSuccess.RemoveAllListeners();
            onSuccess.AddListener(()=>onSuccessAction());
            RequestShow();
        }
        else OnWaitingLoadAction(onSuccessAction,tokenSource);
    }

    private void OnWaitingLoadAction(Action action, CancellationTokenSource tokenSource)
    {
        onLoad.RemoveAllListeners();
        onLoad.AddListener(() => RequestRewardAd(action, tokenSource));
    }

    /// <summary>
    /// 内部直接循环请求，所以是直接返回成功的。
    /// </summary>
    public void LoadRewardAd(Action onLoad, CancellationTokenSource cancellationToken)
    {
        onLoad();
    }
}