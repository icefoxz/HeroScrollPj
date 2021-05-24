using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 广告源控制器
/// </summary>
public class AdManager : AdControllerBase
{
    [Serializable]public enum Ads
    {
        Unity,
        Admob,
        DoNew
    }

    public AdAgentBase adAgent;
    public int AdmobRetry = 3;
    private bool isInit;
    public AdAgentBase AdAgent => adAgent;
    public DoNewAdController DoNewAdController { get; private set; }
    public AdmobController AdmobController { get; private set; }
    public UnityAdController UnityAdController { get; private set; }
    public override AdAgentBase.States Status => AdmobController.Status;
    [Header("广告播放顺序")]public Ads[] Series;
    [Header("广告源比率")]
    public int UnityRatio = 3;
    public int AdmobRatio = 3;
    public int DoNewRatio = 2;
    private QueueByRatio<AdControllerBase> Queue;

    private Dictionary<Ads, (int, AdControllerBase)> Controllers
    {
        get
        {
            if (_controllers == null)
            {
                _controllers = new Dictionary<Ads, (int, AdControllerBase)>
                {
                    {Ads.Unity, (UnityRatio, UnityAdController)},
                    {Ads.Admob, (AdmobRatio, AdmobController)},
                    {Ads.DoNew, (DoNewRatio, DoNewAdController)}
                };
            }

            return _controllers;
        }
    }

    private Dictionary<Ads, (int,AdControllerBase)> _controllers;

    void Start()
    {
        if (isInit) throw XDebug.Throw<AdManager>("Duplicate init!");
        isInit = true;
        DoNewAdController = gameObject.AddComponent<DoNewAdController>();
        AdmobController = gameObject.AddComponent<AdmobController>();
        AdmobController.Init(AdmobRetryCallBack);
        UnityAdController = gameObject.AddComponent<UnityAdController>();
        UnityAdController.Init();
        Queue = new QueueByRatio<AdControllerBase>(
            Series.Join(Controllers,ad=>ad,c=>c.Key,(_,c)=>c.Value).ToArray()
        );
        AdAgent.Init(this);
    }

    public override void RequestShow(UnityAction<bool, string> requestAction)
    {
        admobRetryCount = 0;
        var controller = Queue.Dequeue();
        if (controller.Status != AdAgentBase.States.Loaded)
        {
            AdControllerBase tryResolveController = null;
            do
            {
                tryResolveController = Queue.Dequeue(true);
                if (controller != tryResolveController) continue;//如果广告控制器未重复会一直找
                requestAction.Invoke(false, "无广告源!");//广告控制器重复了
                ControllersAdResolve();
                return;
            } while (tryResolveController.Status != AdAgentBase.States.Loaded); //循环直到到下一个已准备的广告源

            controller = tryResolveController;
        }
        PlayerDataForGame.instance.ShowStringTips($"广告源:{Controllers.First(c=>c.Value.Item2 == controller).Key}");
        controller.RequestShow((success, msg) =>
        {
            requestAction(success, msg);
            ControllersAdResolve();
        });
    }

    public override void RequestLoad(UnityAction<bool, string> loadingAction) => loadingAction(true, string.Empty);

    #region 内部请求admob
    private int admobRetryCount;

    private void ControllersAdResolve()
    {
        if(!AdmobController.IsReady) AdmobController.OnLoadAd(AdmobRetryCallBack);
        if(DoNewAdController.status != AdAgentBase.States.Loaded) DoNewAdController.RequestLoad(null);
    }

    private void AdmobRetryCallBack(bool success, string message)
    {
        if (success) return;
        admobRetryCount++;
        if (admobRetryCount >= AdmobRetry) return;
        ControllersAdResolve();
    }
    #endregion
}

/// <summary>
/// 根据比率排队类
/// </summary>
/// <typeparam name="T"></typeparam>
public class QueueByRatio<T>
{
    private Dictionary<T, int> data;
    private Queue<T> queue;
    public int Count { get; private set; }
    public T Current { get; private set; }

    public QueueByRatio(params (int, T)[] controllers)
    {
        data = controllers.ToDictionary(c => c.Item2, c => c.Item1);
        queue = new Queue<T>(controllers.Select(c => c.Item2));
    }

    public T Dequeue(bool forceChange = false)
    {
        Count--;
        if (!forceChange && Count > 0) return Current;
        var current = Current;
        Current = queue.Dequeue();
        Count = data[Current];
        if (current != null) queue.Enqueue(current);
        return Current;
    }
}