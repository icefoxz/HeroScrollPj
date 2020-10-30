using System;
using UnityEngine;

namespace AdUnion
{
    /*
     * 4399广告联盟Banner   
     */
    public class AdUnionBanner
    {

        private static int MATCH_PARENT = -1;
        private static int WRAP_CONTENT = -2;

        private static int GRAVITY_TOP = 48;
        private static int GRAVITY_BOTTOM = 80;

        private AndroidJavaObject banner;

        //广告位ID
        private string posId;
		
        public AdUnionBanner(string posId)
        {
            this.posId = posId;
            OnAuBannerAdListenerProxy listener
            = new OnAuBannerAdListenerProxy(AdUnionContext.GetInstance().GetRootLayout());
            banner = new AndroidJavaObject("com.mob4399.adunion.AdUnionBanner",
                AdUnionContext.GetInstance().GetActivity(),
               posId,
               listener);
        }

        /*
         * 加载banner广告       
         */
        public void loadBanner()
        {
            LogUtils.print("loadBanner---");
            //加载广告
            banner.Call("loadAd");
        }

        /*
         * Banner广告回调方法       
         */
        class OnAuBannerAdListenerProxy : AndroidJavaProxy
        {
            private AndroidJavaObject parentContainer;
			
			private AndroidJavaObject bannerView;

            public OnAuBannerAdListenerProxy(AndroidJavaObject parent) : base("com.mob4399.adunion.listener.OnAuBannerAdListener")
            {
                this.parentContainer = parent;
            }

            /*
           * 加载成功，返回广告view
           */
            void onBannerLoaded(AndroidJavaObject mBannerView)
            {
                LogUtils.print("Banner loaded");
                AdUnionContext.GetInstance().RunOnUIThread(new AndroidJavaRunnable(() =>
                {
					bannerView = mBannerView;

                    //广告添加到容器中
                    AndroidJavaObject layoutParams = null;
                    layoutParams = new AndroidJavaObject("android.widget.FrameLayout$LayoutParams",
                        MATCH_PARENT, WRAP_CONTENT, GRAVITY_BOTTOM);
                    //add banner to parent container
                    parentContainer.Call("addView", mBannerView, layoutParams);
                }));

            }

            /**
             * 加载失败
             * @param message
             */
            void onBannerFailed(string message)
            {
                LogUtils.print("Banner onBannerLoadFailed:" + message);
            }

            /**
             *广告被点击
             */
            void onBannerClicked()
            {

            }

            /**
             * 广告关闭
             */
            void onBannerClosed()
            {
				if(parentContainer != null && bannerView != null) 
				{
					parentContainer.Call("removeView", bannerView);
				}
                LogUtils.print("close Banner");
            }
        }
    }


}
