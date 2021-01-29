// 
//

namespace Donews.mediation
{
#if UNITY_ANDROID
    using UnityEngine;

    public static class SDK
    {
        private const string DnUnitySdk = "com.donews.android.DnUnitySDK";
        /// <summary>
        /// 每次呼叫后用必须调用IDisposable
        /// </summary>
        public static AndroidJavaObject InstanceDnAdObject() => new AndroidJavaObject(DnUnitySdk);

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
    }

#endif
}
