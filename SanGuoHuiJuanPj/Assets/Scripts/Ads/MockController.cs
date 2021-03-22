﻿using Assets.Scripts.Utl;
using UnityEngine;
using UnityEngine.Events;

public class MockController : AdControllerBase
{
    private UnityAction<bool, string> showAction;
    private UnityAction<bool, string> loadAction;
    public AdAgent adAgentPrefab;
    public override AdAgent.States Status => status;
    public AdAgent.States status;
    void Start()
    {
        var mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        var adAgent = Instantiate(adAgentPrefab, mainCanvas.transform);
        adAgent.Init(this);
    }

    public override void RequestShow(UnityAction<bool, string> requestAction) => showAction = requestAction;

    public override void RequestLoad(UnityAction<bool, string> loadingAction) => loadAction = loadingAction;

    public void OnLoad(bool isLoad) => loadAction(isLoad, string.Empty);
    public void OnShow(bool isShow) => showAction(isShow, string.Empty);

    public void OnCall() => AdAgent.instance.CallAd((success, msg) =>
    {
        UnityMainThread.thread.RunNextFrame(() => Debug.Log($"Call:{success} & {msg}"));
    });
}