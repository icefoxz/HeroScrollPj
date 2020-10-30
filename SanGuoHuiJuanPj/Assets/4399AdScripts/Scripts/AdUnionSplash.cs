using System;
using UnityEngine;

namespace AdUnion
{
    /*
     * 4399广告联盟-原生广告 
     */
    public class AdUnionSplash
    {

        private AndroidJavaObject aUnionSplash;

        private string posId;

        public AdUnionSplash(string posId)
        {
            this.posId = posId;


            //创建开屏广告的父容器
            AndroidJavaObject splashContainer = new AndroidJavaObject("android.widget.FrameLayout",
             AdUnionContext.GetInstance().GetActivity());
            AdUnionContext.GetInstance().GetRootLayout().Call("addView", splashContainer);

            //实例化监听
            OnAuSplashAdListenerProxy listenerProxy = new OnAuSplashAdListenerProxy(AdUnionContext.GetInstance().GetRootLayout(),
             splashContainer);

            aUnionSplash = new AndroidJavaObject("com.mob4399.adunion.AdUnionSplash");
            aUnionSplash.Call("loadSplashAd", AdUnionContext.GetInstance().GetActivity(),
                splashContainer, posId, listenerProxy);

        }



        /*
         *开屏广告
         */
        internal class OnAuSplashAdListenerProxy : AndroidJavaProxy
        {
            private AndroidJavaObject parentContainer;
            private AndroidJavaObject splashContainer;

            /*
             * parentContainer 根节点布局
             * splashContainer  承载splash的容器
             */
            public OnAuSplashAdListenerProxy(AndroidJavaObject parentContainer,
             AndroidJavaObject splashContainer) : base("com.mob4399.adunion.listener.OnAuSplashAdListener")
            {
                this.parentContainer = parentContainer;
                this.splashContainer = splashContainer;
            }
            /**
             * 加载成功
             */
            void onSplashDismissed()
            {
                LogUtils.print("splash clo loaded");

                AdUnionContext.GetInstance().RunOnUIThread(new AndroidJavaRunnable(() =>
                {

                    parentContainer.Call("removeView", splashContainer);
                }));
            }

            /**
             * 加载失败
             * @param message
             */
            void onSplashLoadFailed(string message)
            {
                AdUnionContext.GetInstance().RunOnUIThread(new AndroidJavaRunnable(() =>
                {

                    parentContainer.Call("removeView", splashContainer);
                }));
                LogUtils.print("interstitial onInterstitialLoadFailed" + message);
            }

            /**
             *广告被点击
             */
            void onSplashClicked()
            {
                AdUnionContext.GetInstance().RunOnUIThread(new AndroidJavaRunnable(() =>
                {

                    parentContainer.Call("removeView", splashContainer);
                }));
            }

        }

    }
}
