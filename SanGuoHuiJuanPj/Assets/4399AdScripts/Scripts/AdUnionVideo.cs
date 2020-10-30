using System;
using UnityEngine;

namespace AdUnion
{
    /*
     * 4399广告联盟-视频广告 
     */
    public class AdUnionVideo
    {

        private AndroidJavaObject videoAd;
        private string posId;

        public AdUnionVideo(string posId)
        {
            this.posId = posId;
            OnAuVideoAdListenerProxy listenerProxy = new OnAuVideoAdListenerProxy();
            //实例化视频广告
            videoAd = new AndroidJavaObject("com.mob4399.adunion.AdUnionVideo",
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
                bool isReady = videoAd.Call<bool>("isReady");
                if (videoAd != null && isReady)
                {
                    //加载广告
                    videoAd.Call("show");
                }
                else
                {
                    LogUtils.print("请先初始化广告");
                }
            }));
        }


        /*
         * 视频广告回调方法       
         */

        internal class OnAuVideoAdListenerProxy : AndroidJavaProxy
        {
            public OnAuVideoAdListenerProxy() : base("com.mob4399.adunion.listener.OnAuVideoAdListener")
            {
            }

            /**
            * 广告加载完成
            */
            void onVideoAdLoaded()
            {
                //ToastUtils.showToast("视频广告加载完毕");
                LogUtils.print("-onVideoAdLoaded-");

                AdController.instance.VideoAdLoaded(true);
            }

            /**
             * 广告显示并播放
             */

            void onVideoAdShow()
            {
                LogUtils.print("-onVideoAdShow-");
            }

            /**
             * 广告加载失败
             *
             * @param message 错误信息
             */
            void onVideoAdFailed(string message)
            {
                LogUtils.print("-onVideoAdFailed-：" + message);

                //Debug.Log("广告加载失败: " + message);
                AdController.instance.VideoAdLoaded(false);
            }

            /**
             * 广告被点击
             */
            void onVideoAdClicked()
            {
                LogUtils.print("-onVideoAdClicked-");
            }

            /**
             * 广告关闭
             */
            void onVideoAdClosed()
            {
                LogUtils.print("-onVideoAdClosed-");

                AdController.instance.ClosedVideo();
            }

            /**
             * 视频播放完成
             */
            void onVideoAdComplete()
            {
                LogUtils.print("-onVideoAdComplete-");

                AdController.instance.WatchVideoSuccessed();
            }
        }

    }

}