using System;
using UnityEngine;

namespace AdUnion
{
    /*
     * 4399广告联盟-原生广告 
     */
    public class AdUnionNative
    {

        private static int MATCH_PARENT = -1;
        private static int WRAP_CONTENT = -2;

        private static int GRAVITY_CENTER = 17;

        private AndroidJavaObject nativeAd;
        private string posId;

        public AdUnionNative(string posId)
        {
            this.posId = posId;
            OnAuNativeAdListenerProxy listenerProxy = new OnAuNativeAdListenerProxy();

            //自定义尺寸，默认值可不传
            // int fullWidth = -1;
            //int autoHeight = -2;
            //AndroidJavaObject nativeADSize = new AndroidJavaObject("com.mob4399.adunion.model.NativeAdSize",
            // fullWidth, autoHeight);

            //实例化原生广告
            nativeAd = new AndroidJavaObject("com.mob4399.adunion.AdUnionNative",
               AdUnionContext.GetInstance().GetActivity(),
               posId,
               null,
               listenerProxy
            );

        }

        /*
         * 原生广告回调方法       
         */

        internal class OnAuNativeAdListenerProxy : AndroidJavaProxy
        {
            private AndroidJavaObject nativeAdView;
            public OnAuNativeAdListenerProxy() : base("com.mob4399.adunion.listener.AuNativeAdListener")
            {
            }

            /**
            * 广告加载成功,并返回广告view
            */
            void onNativeAdLoaded(AndroidJavaObject view)
            {
                LogUtils.print("-onNativeAdLoaded-");
                nativeAdView = view;

                AdUnionContext.GetInstance().RunOnUIThread(new AndroidJavaRunnable(() =>
               {

                   //根据具体业务需要，添加到适当位置
                   if (view != null)
                   {
                       // int count = parentLayout.Call<int>("getChildCount");
                       // if(count > 0) {
                       //     parentLayout.Call("removeAllViews");
                       // }
                       //添加到容器中
                       AndroidJavaObject layoutParams = null;
                       layoutParams = new AndroidJavaObject("android.widget.FrameLayout$LayoutParams",
                           MATCH_PARENT, WRAP_CONTENT, GRAVITY_CENTER);
                       AdUnionContext.GetInstance().GetRootLayout().Call("addView", nativeAdView, layoutParams);
                   }


               }));

            }

            /**
                        * 广告加载失败
                        *
                        * @param message 错误信息
                        */
            void onNativeAdError(string message)
            {
                LogUtils.print("-onNativeAdError-" + message);
            }

            /**
             * 广告曝光
             */

            void onNativeAdExposure()
            {
                LogUtils.print("-onNativeAdExposure-");
            }


            /**
             * 广告被点击
             */
            void onNativeAdClicked()
            {
                LogUtils.print("-onNativeAdClicked-");
            }

            /**
             * 广告关闭
             */
            void onNativeAdClosed()
            {
                LogUtils.print("-onNativeAdClosed-");
                //关闭广告时移除
                if (nativeAdView != null)
                {
                    AdUnionContext.GetInstance().GetRootLayout().Call("removeView", nativeAdView);
                }

            }
        }

    }

}
