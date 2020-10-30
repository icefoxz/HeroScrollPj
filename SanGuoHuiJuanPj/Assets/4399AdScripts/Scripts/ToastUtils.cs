using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdUnion
{
    public class ToastUtils
    {
        public static void showToast(string text)
        {
            AdUnionContext.GetInstance().RunOnUIThread(new AndroidJavaRunnable(() =>
            {
                AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
                Toast.CallStatic<AndroidJavaObject>("makeText", AdUnionContext.GetInstance().GetActivity(), text, Toast.GetStatic<int>("LENGTH_SHORT")).Call("show");
            }));
        }
    }
}

