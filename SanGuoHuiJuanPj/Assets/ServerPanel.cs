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
    public Text message;
    private SignalRClient SignalR;
    public void Init(SignalRClient signalR)
    {
        SignalR = signalR;
        SignalR.SubscribeAction(EventStrings.SC_Disconnect,OnDisconnect);
        SignalR.OnStatusChanged += Instance_OnStatusChanged;
        reconnectButton.onClick.AddListener(()=>SignalRClient.instance.Login(OnConnectAction));
        Debug.Log($"{nameof(ServerPanel)}:Start!");
        gameObject.SetActive(false);
    }

    private void OnConnectAction(bool isConnected, HttpStatusCode code)
    {
        gameObject.SetActive(!isConnected);
        message.text = code.ToString();
    }

    private void Instance_OnStatusChanged(HubConnectionState state)
    {
        StopAllCoroutines();
        gameObject.SetActive(state != HubConnectionState.Connected);
        UpdateReconnectBtn(state);
        if (state == HubConnectionState.Connecting || state == HubConnectionState.Reconnecting)
            StartCoroutine(Counting(state));
    }

    private void OnDisconnect(string arg)
    {
        SignalR.Disconnect();
        Instance_OnStatusChanged(HubConnectionState.Disconnected);
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
