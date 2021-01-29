using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 网络响应挡板
/// </summary>
public class AdAgent : MonoBehaviour
{
    public static AdAgent instance;
    public Text countdown;
    public Button cancelButton;
    public Button retryButton;
    private CancellationTokenSource cancellationToken;
    private bool isBusy;
    private int retrySecs;
    private Action onSuccessAction;
    private Action onCancelAction;
    private bool isSuccess;
    private bool isCanceled;
    private IAdController adController;
    public void Init(IAdController adController)
    {
        if (instance != null && instance != this)
            throw XDebug.Throw<DoNewAdController>("Duplicate singleton instance!");
        instance = this;
        this.adController = adController;
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(OnCancel);
        var rect = GetComponent<RectTransform>();
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.one;
        gameObject.SetActive(false);
    }

    private void OnReset()
    {
        cancellationToken = null;
        gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        isBusy = false;
        retrySecs = 0;
        onSuccessAction = null;
        onCancelAction = null;
        isSuccess = false;
        isCanceled = false;
    }

    private void OnCancel()
    {
        onCancelAction.Invoke();
        cancellationToken.Cancel(true);
        OnReset();
    }

    public void BusyRetry(Action requestAction, Action cancelAction , int secsForRetry = 10)
    {
        if (isBusy)
        {
            XDebug.LogError<AdAgent>("等待网络响应挡板只允许同时运行一次！");
            throw new InvalidOperationException();
        }
        gameObject.SetActive(true);
        retryButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(true);
        countdown.gameObject.SetActive(false);
        isBusy = true;
        retrySecs = secsForRetry;
        onCancelAction = cancelAction;
        cancellationToken = new CancellationTokenSource();//初始token，用于直接取消。
        cancellationToken.Token.Register(() => isCanceled = true);
        onSuccessAction = requestAction;//success must cancel the countdown invocation
        StartService();
    }

    private void StartService()
    {
        if (adController.Status == DoNewAdController.AdStates.Cached)
            RequestAdVideo();
        else adController.OnCached += RequestAdVideo;
        StartCoroutine(CountDown());
    }

    private void RequestAdVideo()
    {
        adController.OnCached -= RequestAdVideo;
        adController.RequestRewardAd(()=>
        {
            onSuccessAction.Invoke();
            isSuccess = true;
        }, cancellationToken);//当请求视频的时候，token是作为取消调用的
    }

    private IEnumerator CountDown()
    {
        countdown.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(false);
        retryButton.onClick.RemoveAllListeners();
        var count = retrySecs;
        while (count > 0 
               && !isCanceled
               && !isSuccess) //while cancel must be the action success or user cancel
        {
            countdown.text = count.ToString();
            yield return new WaitForSeconds(1);
            if (count == 7)//一般上请求是预載的，如果第三秒仍然没播放，刷新请求
                adController.LoadRewardAd(true);
            count--;
        }

        if (isSuccess)
        {
            OnReset();
            yield return null;
        }
        if (!isBusy) yield return null;
        countdown.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(true);
        retryButton.onClick.AddListener(()=>
        {
            isCanceled = false;
            StartService();
            retryButton.gameObject.SetActive(false);
        });//重新引用请求
    }
}
