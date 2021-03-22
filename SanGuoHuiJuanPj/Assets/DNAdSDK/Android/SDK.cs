﻿// 
//
using Beebyte.Obfuscator;
using UnityEngine;

namespace Donews.mediation
{
#if UNITY_ANDROID

    [Skip]
    public static class SDK
    {
        public const string DnUnitySdk = "com.donews.android.DnUnitySDK";
        public const string PlaceId = "5294";
        /// <summary>
        /// 每次呼叫后用必须调用IDisposable
        /// </summary>
        //public static AndroidJavaObject InstanceDnAdObject() => new AndroidJavaObject(DnUnitySdk);

        public static AndroidJavaObject DnSdkObj => dnSdkObj = dnSdkObj ?? new AndroidJavaObject(DnUnitySdk);
        //public static AndroidJavaObject InstanceDnAdClass() => new AndroidJavaClass(DnUnitySdk);

        public enum ApiType
        {
            Release = 0,
            Debug = 1
        }

        public static string SDKVersion
        {
            get
            {
                return "5.3";
            }
        }

        public static void StartService()
        {
            //dnAdObject = new AndroidJavaObject("com.donews.android.DnUnitySDK");
        }

        public static string AllSDKVersionInfo
        {
            get
            {
                return "安卓暂时不支持此方法";
            }
        }
        public static ApiType apiType = ApiType.Release;
        private static AndroidJavaObject dnSdkObj;
    }

#endif
}
