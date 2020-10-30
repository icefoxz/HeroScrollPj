using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdUnion
{
    public class LogUtils
    {

        private const string TAG = "ad_unity";
        public static void print(string text)
        {
            AndroidJavaClass Toast = new AndroidJavaClass("android.util.Log");
            Toast.CallStatic<int>("i", TAG, text);
        }
    }
}

