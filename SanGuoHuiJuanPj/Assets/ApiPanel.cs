﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Utl;
using CorrelateLib;
using UnityEngine;
using UnityEngine.Events;

public class ApiPanel : MonoBehaviour
{
    public static ApiPanel instance;
    public static bool IsBusy { get; private set; }
    private SignalRClient Client { get; set; }
    // Start is called before the first frame update
    public void Init(SignalRClient client)
    {
        if (instance != null && instance != this)
            throw new InvalidOperationException();
        instance = this;
        Client = client;
        SetBusy(false);
    }

    public void Invoke(UnityAction<string> recallAction, string method, IViewBag bag = default,
        CancellationToken cancellationToken = default)
    {
        SetBusy(true);
        Client.Invoke(method, value =>
        {
            recallAction?.Invoke(value);
            SetBusy(false);
        }, bag, cancellationToken);
    }

    public void Invoke(UnityAction<ViewBag> successAction, UnityAction<string> failedAction, string method,
        IViewBag bag = default,
        CancellationToken cancellationToken = default)
    {
        SetBusy(this);
        Client.Invoke(method, result =>
        {
            var viewBag = Json.Deserialize<ViewBag>(result);
            if (viewBag == null) failedAction?.Invoke(result);
            else successAction.Invoke(viewBag);
            SetBusy(false);
        }, bag, cancellationToken);
    }

    public async void SyncSaved(UnityAction onCompleteAction)
    {
        SetBusy(this);
        await Client.SynchronizeSaved();
        onCompleteAction?.Invoke();
        SetBusy(false);
    }

    private void SetBusy(bool busy)
    {
        UnityMainThread.thread.RunNextFrame(() =>
        {
            gameObject.SetActive(busy);
            IsBusy = busy;
        });
    }

}