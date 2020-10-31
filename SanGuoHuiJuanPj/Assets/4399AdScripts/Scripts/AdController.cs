using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdUnion;
using System;

public class AdController : MonoBehaviour
{
    public static AdController instance;

    private AdUnionContext adContext;

    private AdUnionVideo video;         //激励视频

    private bool isCanClickVideo;         //标记是否能点击观看广告

    private bool isSuccessWatchVideo;    //标记是否成功观看

    delegate void DelForOverVideo();
    DelForOverVideo delForOverVideo;            //成功观看视频后应执行的事件
    DelForOverVideo delForOverVideoForError;    //失败观看视频后应执行的事件

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

        isCanClickVideo = true;
        isSuccessWatchVideo = false;

        //InitAdSDK();
    }

    private void Start()
    {
        InitAdSDK();
    }

    //初始化广告
    public void InitAdSDK()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            adContext = AdUnionContext.GetInstance();

            //ToastUtils.showToast("InitSDK");
            AndroidJavaClass adUnionClass = new AndroidJavaClass("com.mob4399.adunion.AdUnionSDK");
            //应用id
            string appId = "3048";
            //调用初始化方法      
            adUnionClass.CallStatic("init", adContext.GetActivity(), appId, new OnAuInitListenerProxy());
        }
        catch (System.Exception e)
        {
            isCanClickVideo = false;
            Debug.Log(e.ToString());
        }
#endif
    }

    //加载激励视频
    public void LoadVideo(bool isCan)
    {
        if (!isCan)
            return;
        video = new AdUnionVideo("12730");
    }

    //尝试展示激励视频
    public bool ShowVideo(Action action, Action actionForError)
    {
        if (!isCanClickVideo)
        {
            return false;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            isCanClickVideo = false;
            if (video == null)
            {
                LoadVideo(true);
            }
            else
            {
                video.show();

                delForOverVideo = new DelForOverVideo(action);
                delForOverVideoForError = new DelForOverVideo(actionForError);
            }
            return true;
        }
        catch (Exception e)
        {
            isCanClickVideo = true;
            Debug.Log(e.ToString());
            return false;
        }
#endif
        action();
        return false;
    }

    ////////////////安卓回调//////////////

    /// <summary>
    /// 广告加载成功与否
    /// </summary>
    /// <param name="isSuccessed"></param>
    public void VideoAdLoaded(bool isSuccessed)
    {
        isCanClickVideo = true;
        //Debug.Log("广告预加载: " + isSuccessed);
    }


    /// <summary>
    /// 视频观看有效
    /// </summary>
    public void WatchVideoSuccessed()
    {
        //Debug.Log("广告观看有效");

        isSuccessWatchVideo = true;
    }

    /// <summary>
    /// 广告关闭
    /// </summary>
    public void ClosedVideo()
    {
        if (isSuccessWatchVideo)
        {
            //获取奖励
            //delForOverVideo();
            StartCoroutine(SucceedForWatchVideo());
        }
        else
        {
            //无法获取
            //delForOverVideoForError();
            StartCoroutine(FailedForWatchVideo());
        }
        isSuccessWatchVideo = false;
        isCanClickVideo = true;
    }

    IEnumerator SucceedForWatchVideo()
    {
        yield return new WaitForSeconds(0);
        delForOverVideo();
    }

    IEnumerator FailedForWatchVideo()
    {
        yield return new WaitForSeconds(0);
        delForOverVideoForError();
    }
}
