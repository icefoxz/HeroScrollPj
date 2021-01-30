//
//

using Beebyte.Obfuscator;

namespace Donews.mediation
{
#if UNITY_ANDROID
    using UnityEngine;
    [Skip]
    public class BannerAd
    {

        public struct Layout
        {
            public int width;
            public int height;
            public int top;
            public int bottom;
            public int right;
            public int left;
        };

        private readonly AndroidBannerCallback callback;

        private BannerAd(AndroidBannerCallback callback)
        {
            this.callback = callback;
        }

        /// <summary>
        /// Load the banner Ad.
        /// </summary>
        internal static BannerAd LoadBannerAd(string placeId, Layout layout, IBannerAdListener listener)
        {
            AndroidBannerCallback callback = new AndroidBannerCallback(listener);
            BannerAd ad = new BannerAd(callback);
            //using (var javaObj = SDK.InstanceDnAdObject())
            //    javaObj.Call("requestBannerAd", placeId, layout.width, layout.height, layout.left, layout.right,
            //        layout.top,
            //        layout.bottom, callback);
            SDK.DnSdkObj.Call("requestBannerAd", placeId, layout.width, layout.height, layout.left, layout.right,
                    layout.top,
                    layout.bottom, callback);
            return ad;
        }

        private class AndroidBannerCallback : AndroidJavaProxy
        {
            public IBannerAdListener bannerCallback;
            public AndroidBannerCallback(IBannerAdListener bannerCallback) : base("com.donews.android.BannerCallBack")
            {
                this.bannerCallback = bannerCallback;
            }
            public void onAdError(string msg)
            {
                if (bannerCallback != null)
                {
                    bannerCallback.BannerAdDidLoadFaild(-1, msg);
                }
                Debug.Log("Banner广告错误信息：onAdError：" + msg);
            }
            public void onAdShow()
            {
                if (bannerCallback != null)
                {
                    bannerCallback.BannerAdDidLoadSuccess();
                }
                Debug.Log("Banner广告开始展示：OnAdShow");
            }
            public void onAdExposure()
            {
                if (bannerCallback != null)
                {
                    bannerCallback.BannerAdExposured();
                }
                Debug.Log("Banner广告开始曝光：onAdExposure");
            }
            public void onAdClick()
            {
                if (bannerCallback != null)
                {
                    bannerCallback.BannerAdDidClicked();
                }
                Debug.Log("Banner广告点击：onAdClick");
            }
            public void onAdClose()
            {
                if (bannerCallback != null)
                {
                    bannerCallback.BannerAdDidClickClose();
                }
                Debug.Log("Banner广告关闭：onAdClose");
            }
        }
    }
#endif
}