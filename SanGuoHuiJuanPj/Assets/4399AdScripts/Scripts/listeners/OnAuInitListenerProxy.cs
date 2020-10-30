using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdUnion
{
    public class OnAuInitListenerProxy : AndroidJavaProxy{

        public OnAuInitListenerProxy() : base("com.mob4399.adunion.listener.OnAuInitListener")
        {}

        public void onSucceed()
        {
            AdController.instance.LoadVideo(true);
            //ToastUtils.showToast("初始化成功");
            LogUtils.print("初始化成功");
        }

        public void onFailed(string msg)
        {
            AdController.instance.LoadVideo(false);
            //ToastUtils.showToast("初始化失败，" + msg);
            LogUtils.print("初始化失败，" + msg);
        }
    } 
}