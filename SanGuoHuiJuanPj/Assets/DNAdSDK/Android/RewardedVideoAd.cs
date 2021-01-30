//
//

using System;
using System.Threading;
using Beebyte.Obfuscator;

namespace Donews.mediation
{
#if UNITY_ANDROID
    using UnityEngine;
    [Skip]
    public class RewardedVideoAd
    {

        private readonly AndroidRVPloadCallback callback;
        private readonly string placeId;

        private RewardedVideoAd(AndroidRVPloadCallback callback, string placeId)
        {
            this.callback = callback;
            this.placeId = placeId;
        }

        /// <summary>
        /// Load the rewardedVideo Ad.
        /// </summary>
        internal static RewardedVideoAd LoadRewardedVideoAd(string placeId, IRewardedVideoAdListener listener)
        {
            AndroidRVPloadCallback callback = new AndroidRVPloadCallback(listener);
            RewardedVideoAd ad = new RewardedVideoAd(callback, placeId);
            SDK.DnSdkObj.Call("requestRVPload", placeId, callback);
            return ad;
        }

        /// <summary>
        /// Show the rewardedVideo Ad.
        /// </summary>
        public void ShowRewardedVideoAd()
        {
            //using (var javaObj = SDK.InstanceDnAdObject())
            //    javaObj.Call("requestRVPloadShow", placeId);
            SDK.DnSdkObj.Call("requestRVPloadShow", placeId);
        }

        //激励视频预加载回调接口 采用接口回调方式进行交互
        private class AndroidRVPloadCallback : AndroidJavaProxy
        {
            public IRewardedVideoAdListener rVPloadCallBack;
            public AndroidRVPloadCallback(IRewardedVideoAdListener rVPloadCallBack) : base("com.donews.android.RewardVideoPloadCallBack")
            {
                this.rVPloadCallBack = rVPloadCallBack;
            }
            public void onAdError(int code, string msg)
            {
                if (rVPloadCallBack != null)
                {
                    rVPloadCallBack.RewardVideoAdDidLoadFaild(code, msg);
                }
                Debug.Log("激励视频预加载错误信息：" + msg);
            }
            public void onADLoad()
            {
                if (rVPloadCallBack != null)
                {
                    rVPloadCallBack.RewardVideoAdDidLoadSuccess();
                }
                Debug.Log("激励视频数据预加载成功：");
            }
            public void onVideoCached()
            {
                if (rVPloadCallBack != null)
                {
                    rVPloadCallBack.RewardVideoAdVideoDidLoad();
                }
                Debug.Log("激励视频已经缓存成功：");
            }
            public void onAdShow()
            {
                if (rVPloadCallBack != null)
                {
                    rVPloadCallBack.RewardVideoAdWillVisible();
                    rVPloadCallBack.RewardVideoAdExposured();
                }
                Debug.Log("激励视频开始展示：");
            }
            public void onAdClick()
            {
                if (rVPloadCallBack != null)
                {
                    rVPloadCallBack.RewardVideoAdDidClicked();
                }
                Debug.Log("激励视频点击：");
            }
            public void onAdClose()
            {
                if (rVPloadCallBack != null)
                {
                    rVPloadCallBack.RewardVideoAdDidClose();
                }
                Debug.Log("激励视频关闭：");
            }
            public void onVideoComplete()
            {
                if (rVPloadCallBack != null)
                {
                    rVPloadCallBack.RewardVideoAdDidPlayFinish();
                }
                Debug.Log("激励视频播放完毕：");
            }
            public void onRewardVerify(bool rewardVerfy)
            {
                if (rewardVerfy)
                {
                    if (rVPloadCallBack != null)
                    {
                        rVPloadCallBack.RewardVideoAdDidRewardEffective();
                    }
                    Debug.Log("激励视频播放获取奖励：" + rewardVerfy);
                }
                else
                {
                    Debug.Log("激励视频播放未获取奖励：" + rewardVerfy);
                }
            }
        }
    }

    public class OldRewardVideoAd
    {
        private const string UnityPlayer = "com.unity3d.player.UnityPlayer";
        private const string CurrentActivity = "currentActivity";
        public Action OnAdClose;//当视频关闭
        public Action<bool> OnRewardVerify;//当视频播放完成后的奖励验证回调是否有效

        public OldRewardVideoAd(Action onSuccess,CancellationTokenSource tokenSource,Action onAdClose)
        {
            OnAdClose = onAdClose;
            OnRewardVerify = success =>
            {
                if (success)
                    onSuccess?.Invoke();
                else tokenSource.Cancel();
            };
        }

        internal static OldRewardVideoAd RequestAd(Action onSuccess, CancellationTokenSource tokenSource,
            Action onAdClose = null)
        {
            var rewardAdObj = new OldRewardVideoAd(onSuccess, tokenSource, onAdClose);

            //using (var javaObj = SDK.InstanceDnAdObject())
            //{
            //    javaObj.Call("requestRewardVideo", new RewardVideoAdCallBack(rewardAdObj));
            //}
            SDK.DnSdkObj.Call("requestRewardVideo", new RewardVideoAdCallBack(rewardAdObj));
            return rewardAdObj;
        }

        private class RewardVideoAdCallBack : AndroidJavaProxy
        {
            private OldRewardVideoAd rewardVideoAdObj;
            public RewardVideoAdCallBack(OldRewardVideoAd rewardVideoAdObj) : base("com.donews.android.RewardVideoCallBack")
            {
                this.rewardVideoAdObj = rewardVideoAdObj;
            }

            public void onAdError(string msg) => rewardVideoAdObj.OnRewardVerify?.Invoke(false);

            public void onAdShow()
            {
            }

            public void onAdClick()
            {
            }

            public void onAdClose() => rewardVideoAdObj.OnAdClose?.Invoke();

            public void onVideoComplete()
            {
            }

            public void onRewardVerify(bool rewardVerify) => rewardVideoAdObj.OnRewardVerify?.Invoke(rewardVerify);

            public void onSkippedVideo()
            {
            }
        }
    }
#endif
}
