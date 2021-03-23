using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.UI;

public class ServerPanel : MonoBehaviour
{
    private int count;
    public Button reconnectButton;
    public Text serverMaintenance;
    public Text requestTimeOut;
    public Text exceptionMsg;
    private SignalRClient SignalR;
    public void Init(SignalRClient signalR)
    {
        SignalR = signalR;
        SignalR.SubscribeAction(EventStrings.SC_Disconnect,ServerCallDisconnect);
        SignalR.OnStatusChanged += Instance_OnStatusChanged;
        reconnectButton.onClick.AddListener(()=>SignalRClient.instance.Login(OnConnectAction));
        gameObject.SetActive(false);
        MessageSwitch(false);
        SetException();
    }

    public void SetException(string exception = null)
    {
        exceptionMsg.gameObject.SetActive(!string.IsNullOrWhiteSpace(exception));
        exceptionMsg.text = exception;
    }
    public void ApplicationQuit() => Application.Quit();

    private void MessageSwitch(bool isMaintenance)
    {
        serverMaintenance.gameObject.SetActive(isMaintenance);
        requestTimeOut.gameObject.SetActive(!isMaintenance);
    }

    private void OnConnectAction(bool isConnected, int code)
    {
        gameObject.SetActive(!isConnected);
        SetException(code.ToString());
    }

    private void Instance_OnStatusChanged(HubConnectionState state)
    {
        StopAllCoroutines();
        gameObject.SetActive(state != HubConnectionState.Connected);
        UpdateReconnectBtn(state);
        if (state == HubConnectionState.Connecting || state == HubConnectionState.Reconnecting)
            StartCoroutine(Counting(state));
    }

    private void ShowServerMaintenance()
    {
        MessageSwitch(true);
        reconnectButton.gameObject.SetActive(false);
    }
    //当因为各种原因断线
    public void OnSignalRDisconnected() => Instance_OnStatusChanged(HubConnectionState.Disconnected);

    //当服务器强制离线
    private void ServerCallDisconnect(string arg)
    {
        SignalR.Disconnect();
        Instance_OnStatusChanged(HubConnectionState.Disconnected);
        ShowServerMaintenance();
    }

    private void UpdateReconnectBtn(HubConnectionState status)
    {
        reconnectButton.gameObject.SetActive(status != HubConnectionState.Connected);
        reconnectButton.interactable = status == HubConnectionState.Disconnected;
    }

    IEnumerator Counting(HubConnectionState state)
    {
        UpdateReconnectBtn(state);
        while (true)
        {
            Debug.Log($"等待......{count}");
            yield return new WaitForSeconds(1);
            count++;
        }
    }

}
