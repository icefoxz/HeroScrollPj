using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Beebyte.Obfuscator;
using Donews.mediation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DoNewAdController : AdControllerBase
{
    private static DoNewAdController instance;

    public override AdAgentBase.States Status => status;
    public AdAgentBase.States status;

    public AdModes Mode => mode;
    public AdModes mode;
    public bool isEditableActionSuccess;//仅用于unity编辑器

    public int Timer { get; private set; }
    public UnityEvent OnCached = new UnityEvent();
    private bool isCachedTriggered;

    public const string UnityPlayer = "com.unity3d.player.UnityPlayer";
    public const string CurrentActivity = "currentActivity";
    public const string RequestRewardVideo = "RequestRewardVideo";
    public const string RunOnUiThread = "runOnUiThread";
    public const string PlaceId = "5294";

    void Awake()
    {
        SDK.apiType = SDK.ApiType.Release;
        status = AdAgentBase.States.None;
    }

#if UNITY_EDITOR
    
    private bool isWaitingStateChange;
    void Update()//这个仅仅用在编辑器
    {
        if(!isWaitingStateChange)return;
        if (status == AdAgentBase.States.Loaded)
        {
            isWaitingStateChange = false;
            OnCached.Invoke();
            OnCached.RemoveAllListeners();
        }
    }
#endif
    public override void RequestLoad(UnityAction<bool,string> requestAction)
    {
#if UNITY_EDITOR
        mode = AdModes.NoAd;
        isWaitingStateChange = true;
#endif
        requestAction.Invoke(true, string.Empty);//直接播放，不需要load
    }

    public override void RequestShow(UnityAction<bool,string> onSuccessAction)
    {
#if UNITY_EDITOR
        mode = AdModes.NoAd;
#endif
        try
        {
            DirectPlayRewardVideoAd.RequestAd(onSuccessAction);
        }
        catch (Exception e)
        {
            onSuccessAction(false, e.ToString());
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
}