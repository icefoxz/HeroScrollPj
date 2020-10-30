using UnityEngine;
using System.Collections;

namespace AdUnion
{
    public sealed class AdUnionContext : MonoBehaviour
    {
 
        private AndroidJavaObject currentActivity;

        private static readonly AdUnionContext _AdUnionContext = new AdUnionContext();

        /*
         * 获取当前实例       
         */      
        public static AdUnionContext GetInstance()
        {
            return _AdUnionContext;
        }


        /*
         * 获取当前Activity       
         */
        public AndroidJavaObject GetActivity()
        {
            if (null == currentActivity)
            {
                currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                .GetStatic<AndroidJavaObject>("currentActivity");
            }

            return currentActivity;
        }

        /*
         * 运行在主UI线程       
         */       
        public void RunOnUIThread(AndroidJavaRunnable runnable)
        {
            GetActivity().Call("runOnUiThread", runnable);
        }


        /*
         * 获取根节点的布局，可用于添加banner广告,具体业务需要的容器，可自行处理      
         */       
        public AndroidJavaObject GetRootLayout()
        {
            AndroidJavaClass R = new AndroidJavaClass("android.R$id");
            return currentActivity.Call<AndroidJavaObject>("findViewById", R.GetStatic<int>("content"));

        }
    }

}
