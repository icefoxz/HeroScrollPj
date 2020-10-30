using System;
using UnityEngine;

namespace AdUnion
{
    /*
     * 4399广告联盟-插屏广告 
     */
    public class AdUnionInterstitial
    {

        private AndroidJavaObject interstitialAd;
        private string posId;

        public AdUnionInterstitial(string posId)
        {
            this.posId = posId;
            OnAuInterstitialAdListenerProxy listenerProxy = new OnAuInterstitialAdListenerProxy();
            interstitialAd = new AndroidJavaObject("com.mob4399.adunion.AdUnionInterstitial",
               AdUnionContext.GetInstance().GetActivity(),
               posId,
               listenerProxy
            );

        }

        /*
         * 显示插屏广告       
         */
        public void show()
        {

            AdUnionContext.GetInstance().RunOnUIThread(new AndroidJavaRunnable(() =>
            {
                if (interstitialAd != null)
                {
                    //加载广告
                    interstitialAd.Call("show");
                }

            }));
        }

        /*
         * 插屏广告回调方法       
         */

        internal class OnAuInterstitialAdListenerProxy : AndroidJavaProxy
        {
            public OnAuInterstitialAdListenerProxy() : base("com.mob4399.adunion.listener.OnAuInterstitialAdListener")
            {
            }
            /**
             * 加载成功
             */
            void onInterstitialLoaded()
            {
                //ToastUtils.showToast("插屏广告加载完毕");
                LogUtils.print("interstitial loaded---");
            }

            /**
             * 加载失败
             * @param message
             */
            void onInterstitialLoadFailed(string message)
            {
                LogUtils.print("interstitial onInterstitialLoadFailed---："+message);
            }

            /**
             *广告被点击
             */
            void onInterstitialClicked()
            {

            }

            /**
             * 广告关闭
             */
            void onInterstitialClosed()
            {
                LogUtils.print("close interstitial");
            }
        }

    }

}
