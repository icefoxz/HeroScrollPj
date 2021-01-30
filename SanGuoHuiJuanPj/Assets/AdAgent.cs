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
    public Image adFreeWindow;
    public Button successButton;
    public Button failedButton;
    [Header("点击不开启广告调用")]public bool isAdFree;

    public static AdAgent instance;
    public Text countdown;
    public Text message;
    public Button cancelButton;
    public Button retryButton;
    public int retrySeconds = 6;
    public int retrySecs;
    private int retries;
    private Coroutine coroutine;
    private CancellationTokenSource cancellationToken;
    private bool isBusy;
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
        retrySecs = retrySeconds;
        onSuccessAction = null;
        onCancelAction = null;
        isSuccess = false;
        isCanceled = false;
        message.text = string.Empty;
        retries = 0;
        coroutine = null;
    }

    private void OnCancel()
    {
        onCancelAction.Invoke();
        cancellationToken.Cancel(true);
        OnReset();
    }

    public void BusyRetry(Action requestAction, Action cancelAction , int secsForRetry = default)
    {
        if (isBusy)
        {
            XDebug.LogError<AdAgent>("等待网络响应挡板只允许同时运行一次！");
            throw new InvalidOperationException();
        }

        retrySecs = secsForRetry == default ? retrySeconds : secsForRetry;
        gameObject.SetActive(true);
        retryButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(true);
        countdown.gameObject.SetActive(false);
        isBusy = true;
        onCancelAction = cancelAction;
        onSuccessAction = requestAction;//success must cancel the countdown invocation

        successButton.gameObject.SetActive(isAdFree);
        failedButton.gameObject.SetActive(isAdFree);
        adFreeWindow.gameObject.SetActive(isAdFree);
        if (isAdFree)
        {
            successButton.onClick.RemoveAllListeners();
            successButton.onClick.AddListener(()=>
            {
                requestAction();
                OnReset();
            });
            failedButton.onClick.RemoveAllListeners();
            failedButton.onClick.AddListener(() =>
            {
                cancelAction();
                OnReset();
            });
            return;
        }

        StartService(retrySecs);
    }

    void OnTokenCancelled(CancellationToken token)
    {
        if (token == cancellationToken.Token)//如果已经换token代表已经有另一个请求了。所以无视
            isCanceled = true;
    }

    private void StartService(int countDown)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        cancellationToken = new CancellationTokenSource(); //用于直接取消。
        cancellationToken.Token.Register(() => OnTokenCancelled(cancellationToken.Token));
        message.text = "请求视频中...";
        adController.LoadRewardAd(RequestAdVideo, cancellationToken);
        coroutine = StartCoroutine(CountDown(countDown));
    }

    private void RequestAdVideo()
    {
        message.text = "请求完成，正加载视频...";
        cancellationToken = new CancellationTokenSource();
        adController.RequestRewardAd(()=>
        {
            onSuccessAction.Invoke();
            isSuccess = true;
        }, cancellationToken);//当请求视频的时候，token是作为取消调用的
    }

    private IEnumerator CountDown(int count = default)
    {
        countdown.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(false);
        retryButton.onClick.RemoveAllListeners();
        if (count == default) count = retrySecs;
        while (count > 0 
               && !isCanceled
               && !isSuccess) //while cancel must be the action success or user cancel
        {
            countdown.text = count.ToString();
            yield return new WaitForSeconds(1);
            count--;
            if (count > retrySecs / 2 || retries > 0) continue;
            retries++;
            StartService(count);
            yield return null;
        }

        if (isSuccess)
        {
            OnReset();
            yield return null;
        }
        if (!isBusy) yield return null;
        retries = 0;
        countdown.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(true);
        retryButton.onClick.AddListener(()=>
        {
            isCanceled = false;
            StartService(retrySecs);
            retryButton.gameObject.SetActive(false);
        });//重新引用请求
    }
}
