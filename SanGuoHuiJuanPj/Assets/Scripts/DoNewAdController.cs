﻿using System;
using System.Threading.Tasks;
using Beebyte.Obfuscator;
using UnityEngine;

[Skip]public class DoNewAdController : MonoBehaviour
{
    public static DoNewAdController instance;

    [HideInInspector]
    public bool isWacthing; //是否正在观看视频
    private bool isCanGetReward;    //是否可以获取奖励

    private const string UnityPlayer = "com.unity3d.player.UnityPlayer";
    private const string CurrentActivity = "currentActivity";
    private const string RequestRewardVideo = "RequestRewardVideo";

    Action delForOverVideo;    //结束奖励视频后应执行的事件
    Action delForOverVideoForError;    //请求失败后应执行的事件

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);

        isWacthing = false;
        isCanGetReward = false;  
    }

    ////////////////安卓回调//////////////////////////////////////

    //视频观看有效与否回调
    private void CallBackFromRewardedVideo(string isDone)
    {
        if (isDone == "1")
        {
            isCanGetReward = true;

            //Debug.Log("观看成功回调");
        }
        else
        {
            isCanGetReward = false;

            //Debug.Log("观看失败回调");
        }
    }

    //广告视频关闭回调
    private void ClosedRewardedVideo(string str)
    {
        if (isCanGetReward)
        {
            delForOverVideo();
            //获取奖励
        }
        else
        {
            delForOverVideoForError();
            //无法获取
        }
        isCanGetReward = false;
        isWacthing = false;
    }

    //广告请求错误回调
    private void ErrorRewardedVideo(string str)
    {
        delForOverVideoForError();
        Debug.LogError("广告请求错误：" + str);
        isCanGetReward = false;
        isWacthing = false;
    }

    ////////////////交互安卓//////////////////////////////////////

    private string UNITY_CLASS = "com.unity3d.player.UnityPlayer";
    //private string JAVA_CLASS = "com.MoTa.LegendOfHeroAs.MainActivity";

    private AndroidJavaClass jc;    //unityPlayer安卓类
    private AndroidJavaObject jo;


    //尝试观看视频
    [Skip]public Task<bool> GetReWardVideo()
    {
#if UNITY_EDITOR
        return Task.FromResult(true);
#else
        try
        {
            if (jo == null)
            {
                jc = new AndroidJavaClass(UnityPlayer);
                jo = jc.GetStatic<AndroidJavaObject>(CurrentActivity);
            }
            isWacthing = true;
            jo.Call(RequestRewardVideo);
            isWacthing = false;
            return Task.FromResult(true);
        }
        catch (Exception)
        {
            return Task.FromResult(false);
        }
#endif
    }

    [Skip]public void GetReWardVideo(Action action, Action actionForError)
    {
#if UNITY_EDITOR
        action?.Invoke();
        return;
#else
        try
        {
            if (jo == null)
            {
                jc = new AndroidJavaClass(UnityPlayer);
                jo = jc.GetStatic<AndroidJavaObject>(CurrentActivity);
            }
            jo.Call(RequestRewardVideo);
            action?.Invoke();
            
        }
        catch (Exception)
        {
            actionForError?.Invoke();
        }
#endif
    }

}

//void InitNative()
//{
//    //调用有参方法
//    object[] objects = new object[] { "", "", false };
//    jo.Call("initNativeAd", objects);
//}

//void ShowNative()
//{
//    //调用有返回值方法
//    if (jo.Call<bool>("isNativeReady"))
//        jo.Call("showNativeAd");
//}

//void HideNative()
//{
//    //调用无参方法
//    jo.Call("hideNativeAd");
//}