//
//

namespace Donews.mediation
{
#if UNITY_ANDROID
    using UnityEngine;

    public class SplashAd
    {
        private readonly AndroidSplashCallback callback;
        private SplashAd(AndroidSplashCallback callback)
        {
            this.callback = callback;
        }

        /// <summary>
        /// Load the  splash Ad.
        /// </summary>
        internal static SplashAd LoadSplashAd(string placeId, ISplashAdListener listener)
        {
            AndroidSplashCallback callback = new AndroidSplashCallback(listener);
            SplashAd ad = new SplashAd(callback);
            using (var javaObj = SDK.InstanceDnAdObject())
                javaObj.Call("showSplashAd", placeId, callback);
            return ad;
        }

        private class AndroidSplashCallback : AndroidJavaProxy
        {
            public ISplashAdListener splashCallBack;
            public AndroidSplashCallback(ISplashAdListener splashCallBack) : base("com.donews.android.SplashCallBack")
            {
                this.splashCallBack = splashCallBack;
            }
            public void onNoAD(string msg)
            {
                if (splashCallBack != null)
                {
                    splashCallBack.SplashAdDidLoadFaild(-1, msg);
                }
                Debug.Log("开屏没有获取到广告：" + msg);
            }
            public void onAdShow()
            {
                if (splashCallBack != null)
                {
                    splashCallBack.SplashAdDidLoadSuccess();
                }
                Debug.Log("开屏开始展示：");
            }
            public void onAdClick()
            {
                if (splashCallBack != null)
                {
                    splashCallBack.SplashAdDidClicked();
                }
                Debug.Log("开屏点击：");
            }
            public void onPresent()
            {
                if (splashCallBack != null)
                {
                    splashCallBack.SplashAdExposured();
                }
                Debug.Log("开屏曝光：");
            }
            public void onADDismissed()
            {
                if (splashCallBack != null)
                {
                    splashCallBack.SplashAdDidClose();
                }
                Debug.Log("开屏播放完成：");
            }
            public void extendExtra(string s)
            {
                Debug.Log("开屏透传参数：");
            }
        }
    }
#endif
}

