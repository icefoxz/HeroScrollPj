//
//

namespace Donews.mediation
{
#if UNITY_ANDROID
    using UnityEngine;

    public class InterstitialAd
    {

        private readonly AndroidInsterstialCallback callback;

        private InterstitialAd(AndroidInsterstialCallback callback)
        {
            this.callback = callback;
        }

        /// <summary>
        /// Load the interstitial Ad.
        /// </summary>
        internal static InterstitialAd LoadInterstitialAd(string placeId, IInterstitialAdListener listener)
        {
            AndroidInsterstialCallback callback = new AndroidInsterstialCallback(listener);
            InterstitialAd ad = new InterstitialAd(callback);
            //using (var javaObj = SDK.InstanceDnAdObject())
            //    javaObj.Call("requestInstertialAd", placeId, Screen.width, 0, callback);
            SDK.DnSdkObj.Call("requestInstertialAd", placeId, Screen.width, 0, callback);
            return ad;
        }

        //插屏接口 采用接口回调方式进行交互
        private class AndroidInsterstialCallback : AndroidJavaProxy
        {
            public IInterstitialAdListener insterstialCallback;
            public AndroidInsterstialCallback(IInterstitialAdListener insterstialCallback) : base("com.donews.android.InsterStialCallBack")
            {
                this.insterstialCallback = insterstialCallback;
            }
            public void onAdError(string msg)
            {
                if (insterstialCallback != null)
                {
                    insterstialCallback.InterstitialDidLoadFaild(-1, msg);
                }
                Debug.Log("插屏错误信息：" + msg);
            }
            public void onAdShow()
            {
                if (insterstialCallback != null)
                {
                    insterstialCallback.InterstitialDidLoadSuccess();
                    insterstialCallback.InterstitialAdWillVisible();
                }
                Debug.Log("插屏广告开始显示onAdShow：");
            }
            public void onADExposure()
            {
                if (insterstialCallback != null)
                {
                    insterstialCallback.InterstitialAdExposured();
                }
                Debug.Log("插屏广告开始曝光onADExposure：");
            }
            public void onAdClick()
            {
                if (insterstialCallback != null)
                {
                    insterstialCallback.InterstitialAdDidClicked();
                }
                Debug.Log("插屏广告点击onAdClick：");
            }
            public void onAdClose()
            {
                if (insterstialCallback != null)
                {
                    insterstialCallback.InterstitialAdDidClose();
                }
                Debug.Log("插屏广告关闭onAdClose：");
            }
        }

    }
#endif
}

