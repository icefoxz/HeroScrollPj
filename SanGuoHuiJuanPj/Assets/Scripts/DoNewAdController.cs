using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Beebyte.Obfuscator;
using Donews.mediation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface IAdController
{
    DoNewAdController.AdStates Status { get; }
    int Timer { get; }
    event Action OnCached;
    void RequestRewardAd(Action onSuccess, CancellationTokenSource tokenSource);
    /// <summary>
    /// 小心使用，多次调用会导致底层调用过多而崩溃
    /// </summary>
    /// <param name="forceReload"></param>
    void LoadRewardAd(bool forceReload = false);
}

[Skip]public class DoNewAdController : MonoBehaviour, IAdController
{
    public static IAdController AdController => instance;
    private static DoNewAdController instance;
    public enum AdStates
    {
        None,
        RequestingCache,
        Cached
    }

    public AdStates Status => status;
    public AdStates status;
    public bool isEditableActionSuccess;//仅用于unity编辑器
    public int forceReloadAfterSecond = 20;//重新请求的等待秒数

    public int Timer { get; private set; }
    public event Action OnCached;
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
        SDK.StartService();
        status = AdStates.None;

        StartCoroutine(UpdateEverySecond());
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SceneLoadRelocateCanvas;
        var mainCanvas = GameObject.FindGameObjectWithTag(MainCanvas);
        AdAgent = Instantiate(adAgentPrefab,mainCanvas.transform);
        AdAgent.Init(this);
    }

    IEnumerator UpdateEverySecond()
    {
        while (true)
        {
            if (status == AdStates.Cached)
            {
                if (isCachedTriggered) yield break;
                isCachedTriggered = true;
                OnCached?.Invoke();
                yield break;
            }
            Timer = Timer >= forceReloadAfterSecond ? 0 : Timer + 1;
            LoadRewardAd(Timer >= forceReloadAfterSecond);
            yield return new WaitForSeconds(1);
        }
    }

    private void SceneLoadRelocateCanvas(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0) return;
        var mainCanvas = GameObject.FindGameObjectWithTag(MainCanvas);
        if (AdAgent) Destroy(AdAgent);
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

    public void RequestRewardAd(Action onSuccess, CancellationTokenSource tokenSource)
    {
#if UNITY_EDITOR
        if (isEditableActionSuccess)
        {
            onSuccess.Invoke();
            LoadRewardAd(true);
        }
        else
        {
            Task.Delay(TimeSpan.FromSeconds(3)); //编辑器里，三秒后失败
            PlayerDataForGame.instance.ShowStringTips("视频加载失败!");
            tokenSource.Cancel();
            LoadRewardAd(true);
        }

        return;
#endif
        if (status == AdStates.None || status == AdStates.RequestingCache)
        {
            throw XDebug.Throw<DoNewAdController>($"Ad isn't cached! status = {status}");
        }
        try
        {
            currentAdListener.onFailed = tokenSource.Cancel;
            currentAdListener.onSuccess = () =>
            {
                onSuccess.Invoke();
                LoadRewardAd(true);
            };
            rewardedVideoAd.ShowRewardedVideoAd();
        }
        catch (Exception e)
        {
            PlayerDataForGame.instance.ShowStringTips($"视频加载失败!");
            tokenSource.Cancel();
            LoadRewardAd(true);
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