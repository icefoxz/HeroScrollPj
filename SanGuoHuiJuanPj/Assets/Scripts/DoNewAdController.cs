using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Beebyte.Obfuscator;
using Donews.mediation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface IAdController
{
    DoNewAdController.AdStates Status { get; }
    DoNewAdController.Modes Mode { get; }
    int Timer { get; }
    void RequestRewardAd(Action onSuccess, CancellationTokenSource tokenSource);
    void LoadRewardAd(Action onLoad, CancellationTokenSource cancellationToken);
}

public class DoNewAdController : MonoBehaviour, IAdController
{
    public static IAdController AdController => instance;
    private static DoNewAdController instance;
    public enum AdStates
    {
        None,
        RequestingCache,
        Cached
    }
    public enum Modes
    {
        NoAd,
        DirectLoad,
        Preload
    }
    public AdStates Status => status;
    public Modes Mode => mode;
    public AdStates status;
    public Modes mode;
    public bool isEditableActionSuccess;//仅用于unity编辑器

    public int Timer { get; private set; }
    public UnityEvent OnCached = new UnityEvent();
    private bool isCachedTriggered;
    public static AndroidJavaObject CurrentActivityJo =>
        new AndroidJavaObject(UnityPlayer).GetStatic<AndroidJavaObject>(CurrentActivity);
    public static void CallUiThread(Action action) => CurrentActivityJo.Call(RunOnUiThread, action);

    public const string UnityPlayer = "com.unity3d.player.UnityPlayer";
    public const string CurrentActivity = "currentActivity";
    public const string RequestRewardVideo = "RequestRewardVideo";
    public const string RunOnUiThread = "runOnUiThread";
    public const string PlaceId = "5294";
    public const string MainCanvas = "MainCanvas";

    private RewardedVideoAd rewardedVideoAd;
    private RewardAdListener currentAdListener;
    public RewardAdListener CurrentAdListener => currentAdListener;
    public AdAgent adAgentPrefab;
    public AdAgent AdAgent { get; private set; }

    private bool isInit;

    void Awake()
    {
        if (instance != null && instance != this)
            throw XDebug.Throw<DoNewAdController>("Duplicate singleton instance!");

        instance = this;
        Init();
    }

    private void Init()
    {
        if (isInit) throw XDebug.Throw<DoNewAdController>("Duplicate init!");
        isInit = true;
        SDK.apiType = SDK.ApiType.Release;
        //SDK.StartService();
        status = AdStates.None;

        //StartCoroutine(UpdateEverySecond());
        DontDestroyOnLoad(gameObject);
        var mainCanvas = GameObject.FindGameObjectWithTag(MainCanvas);
        AdAgent = Instantiate(adAgentPrefab,mainCanvas.transform);
        AdAgent.Init(this);
        SceneManager.sceneLoaded += SceneLoadRelocateCanvas;
    }

    //IEnumerator UpdateEverySecond()
    //{
    //    while (true)
    //    {
    //        if (status == AdStates.Cached)
    //        {
    //            if (isCachedTriggered) continue;
    //            isCachedTriggered = true;
    //            OnCached?.Invoke();
    //            continue;
    //        }
    //        if(IsPreloadMode)
    //        {
    //            Timer = Timer >= forceReloadAfterSecond ? 0 : Timer + 1;
    //            LoadRewardAd(Timer >= forceReloadAfterSecond);
    //        }
    //        yield return new WaitForSeconds(1);
    //    }
    //}

    private void SceneLoadRelocateCanvas(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0) return;
        var mainCanvas = GameObject.FindGameObjectWithTag(MainCanvas);
        if (AdAgent) return;
        AdAgent = Instantiate(adAgentPrefab,mainCanvas.transform);
        AdAgent.Init(this);
    }

    public void LoadRewardAd(bool forceReload = false)
    {
#if UNITY_EDITOR
        if (forceReload) status = AdStates.None;
        if (status == AdStates.RequestingCache) return;
        return;
#endif
        if (forceReload)
        {
            status = AdStates.None;
            currentAdListener = null;
        }
        if (status != AdStates.None) return;
        if (currentAdListener != null && !currentAdListener.IsLoadSuccess) return;
        status = AdStates.RequestingCache;
        currentAdListener = new RewardAdListener(() => status = AdStates.Cached);
        rewardedVideoAd = RewardedVideoAd.LoadRewardedVideoAd(PlaceId, currentAdListener);
    }

#if UNITY_EDITOR
    
    private bool isWaitingStateChange;
    void Update()//这个仅仅用在编辑器
    {
        if(!isWaitingStateChange)return;
        if (status == AdStates.Cached)
        {
            isWaitingStateChange = false;
            OnCached.Invoke();
            OnCached.RemoveAllListeners();
        }
    }
#endif
    public void LoadRewardAd(Action onLoad, CancellationTokenSource cancellationToken)
    {
#if UNITY_EDITOR
        mode = Modes.NoAd;
        isWaitingStateChange = true;
#endif
        try
        {
            switch (mode)
            {
                case Modes.NoAd:
                    OnCached.AddListener(() => onLoad());
                    status = AdStates.RequestingCache;
                    break;
                case Modes.DirectLoad:
                    onLoad.Invoke();
                    break;
                case Modes.Preload:
                    currentAdListener = new RewardAdListener(onLoad);
                    rewardedVideoAd = RewardedVideoAd.LoadRewardedVideoAd(PlaceId, currentAdListener);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        catch (Exception e)
        {
            PlayerDataForGame.instance.ShowStringTips("视频加载失败1");
            cancellationToken.Cancel();
        }
    }

    public void RequestRewardAd(Action onSuccess, CancellationTokenSource tokenSource)
    {
#if UNITY_EDITOR
        mode = Modes.NoAd;
#endif
        try
        {
            switch (mode)
            {
                case Modes.NoAd:
                    if (isEditableActionSuccess)
                        onSuccess.Invoke();
                    else tokenSource.Cancel();
                    break;
                case Modes.DirectLoad:
                    DirectPlayRewardVideoAd.RequestAd(onSuccess, tokenSource);
                    break;
                case Modes.Preload:
                    currentAdListener.onFailed = tokenSource.Cancel;
                    currentAdListener.onSuccess = onSuccess.Invoke;
                    rewardedVideoAd.ShowRewardedVideoAd();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception e)
        {
            if (tokenSource.IsCancellationRequested) return;
            tokenSource.Cancel();
            PlayerDataForGame.instance.ShowStringTips("视频加载失败!");
        }
    }

    private AndroidJavaObject currentActivity;
    

    public class RewardAdListener : IRewardedVideoAdListener
    {
        public Action onSuccess;

        public Action onFailed;
        public bool IsLoadSuccess { get; private set; }
        public RewardedVideoAd videoAd;

        private Action onLoadSuccessAction;
        public RewardAdListener(Action onLoadSuccess)
        {
            onLoadSuccessAction = onLoadSuccess;
        }

        // 广告数据加载成功回调
        public void RewardVideoAdDidLoadSuccess()
        {
            onLoadSuccessAction.Invoke();
            IsLoadSuccess = true;
        }

        // 视频广告播放达到激励条件回调
        public void RewardVideoAdDidRewardEffective() => onSuccess.Invoke();
        // 视频广告视频播放完成
        public void RewardVideoAdDidPlayFinish() { }
        // 视频广告曝光回调
        public void RewardVideoAdExposured() { }

        // 视频广告各种错误信息回调
        public void RewardVideoAdDidLoadFaild(int errorCode, string errorMsg) => onFailed.Invoke();
        // 视频数据下载成功回调，已经下载过的视频会直接回调
        public void RewardVideoAdVideoDidLoad(){}
        // 视频播放页即将展示回调
        public void RewardVideoAdWillVisible(){}
        // 视频播放页关闭回调
        public void RewardVideoAdDidClose(){}
        // 视频广告信息点击回调
        public void RewardVideoAdDidClicked(){}
    }





    private AndroidJavaObject jo;
    private AndroidJavaClass jc;    //unityPlayer安卓类

    //[Skip]
    //public Task<bool> GetReWardVideo()
    //{
    //    try
    //    {
    //        if (jo == null)
    //        {
    //            jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //            jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
    //        }

    //        jo.Call("RequestRewardVideo");
    //        return Task.FromResult(true);
    //    }
    //    catch (Exception)
    //    {
    //        return Task.FromResult(false);
    //    }
    //}

}