using System;
using System.Collections;
using System.Collections.Generic;
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
    public Ads Ad;
    public AdAgentBase AdAgent => adAgent;
    public DoNewAdController DoNewAdController { get; private set; }
    public AdmobController AdmobController { get; private set; }
    public UnityAdController UnityAdController { get; private set; }
    public override AdAgentBase.States Status => AdmobController.Status;

    private Dictionary<Ads, AdControllerBase> Controllers
    {
        get
        {
            if (_controllers == null)
            {
                _controllers = new Dictionary<Ads, AdControllerBase>
                {
                    {(Ads)0,UnityAdController},
                    {Ads.Admob,AdmobController},
                    {Ads.DoNew,DoNewAdController}
                };
            }

            return _controllers;
        }
    }

    private Dictionary<Ads, AdControllerBase> _controllers;

    void Start()
    {
        if (isInit) throw XDebug.Throw<AdManager>("Duplicate init!");
        isInit = true;
        DoNewAdController = gameObject.AddComponent<DoNewAdController>();
        AdmobController = gameObject.AddComponent<AdmobController>();
        AdmobController.Init(AdmobCallBack);
        UnityAdController = gameObject.AddComponent<UnityAdController>();
        UnityAdController.Init();
        AdAgent.Init(this);
    }

    private IEnumerator DelayedInit()
    {
        yield return new WaitForSeconds(3);
        ResolveAdvertisement();
    }

    public override void RequestShow(UnityAction<bool, string> requestAction)
    {
        admobRetryCount = 0;
        var controller = Controllers[Ad];
        controller.RequestShow((success, msg) =>
        {
            requestAction(success, msg);
            ResolveAdvertisement();
        });
    }

    public override void RequestLoad(UnityAction<bool, string> loadingAction) => loadingAction(true, string.Empty);

    #region 封装内部请求admob
    private int admobRetryCount;

    private void ResolveAdvertisement()
    {
        if(!AdmobController.IsReady) AdmobController.OnLoadAd(AdmobCallBack);
    }

    private void AdmobCallBack(bool success, string message)
    {
        if (success)
        {
            foreach (var item in Controllers)
            {
                if(item.Value.Status != AdAgentBase.States.Loaded)continue;
                Ad = item.Key;
                break;
            }
            return;
        }
        admobRetryCount++;
        if(admobRetryCount>=AdmobRetry)return;
        ResolveAdvertisement();
    }
    #endregion
}