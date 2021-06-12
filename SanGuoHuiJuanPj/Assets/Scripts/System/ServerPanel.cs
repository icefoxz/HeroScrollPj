using System.Collections;
using System.Collections.Generic;
using CorrelateLib;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.UI;

public class ServerPanel : MonoBehaviour
{
    enum States
    {
        None,
        ServerMaintenance,
        TimeOut,
        Other
    }
    private int count;
    public Button reconnectButton;
    public Button exitButton;
    public Text serverMaintenance;
    public Text requestTimeOut;
    public Text Message;
    public Text exceptionMsg;
    private SignalRClient SignalR;
    private bool isDisconnectRequested = false;

    private Dictionary<Text, States> StateSet
    {
        get
        {
            if (stSet == null)
            {
                stSet = new Dictionary<Text, States>
                {
                    {Message, States.Other},
                    {serverMaintenance, States.ServerMaintenance},
                    {requestTimeOut, States.TimeOut}
                };
            }
            return stSet;
        }
    }
    private Dictionary<Text, States> stSet; 

    public void Init(SignalRClient signalR)
    {
        SignalR = signalR;
        SignalR.SubscribeAction(EventStrings.SC_Disconnect, ServerCallDisconnect);
        SignalR.SubscribeAction(EventStrings.SC_ReLogin, args => ClientReconnect());
        SignalR.OnStatusChanged += OnStatusChanged;
        reconnectButton.onClick.AddListener(()=>Reconnect());
        exitButton.onClick.AddListener(Application.Quit);
        exitButton.gameObject.SetActive(false);
        gameObject.SetActive(false);
        UiShow(States.None);
        SetException();
    }

    public void SetException(string exception = null)
    {
        exceptionMsg.gameObject.SetActive(!string.IsNullOrWhiteSpace(exception));
        exceptionMsg.text = exception;
    }

    private void UiShow(States state)
    {
        foreach (var ui in StateSet)
        {
            ui.Key.gameObject.SetActive(ui.Value == state);
        }
    }

    private void OnConnectAction(bool isConnected, int code, SignalRClient.SignalRConnectionInfo info)
    {
        gameObject.SetActive(!isConnected);
        SetException(code.ToString());
    }

    private void OnStatusChanged(HubConnectionState state)
    {
        StopAllCoroutines();
        gameObject.SetActive(state != HubConnectionState.Connected);
        UpdateReconnectBtn(state);
        switch (state)
        {
            case HubConnectionState.Connecting:
            case HubConnectionState.Reconnecting:
                StartCoroutine(Counting(state));
                return;
            case HubConnectionState.Disconnected:
                Reconnect();
                break;
        }
    }

    //当服务器强制离线
    private void ServerCallDisconnect(string arg)
    {
        isDisconnectRequested = true;
        exitButton.gameObject.SetActive(true);
        SignalR.Disconnect();
        StopAllCoroutines();
        OnStatusChanged(HubConnectionState.Disconnected);
        var state = States.ServerMaintenance;
        if (!string.IsNullOrWhiteSpace(arg))
        {
            Message.text = arg;
            state = States.Other;
        }
        UiShow(state);
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

    void OnApplicationFocus(bool isFocus)
    {
        if(!isFocus)return;
        if(SignalR.Status == HubConnectionState.Disconnected) Reconnect();
    }

    private void Reconnect()
    {
        if (!isDisconnectRequested) SignalR.ReconnectServer(OnRetryConnectToServer);
    }

    private void OnRetryConnectToServer(bool success)
    {
        if(success)return;
        if (GamePref.ClientLoginMethod == 1) SignalR.DirectLogin(ReLoginToServer);
        else SignalR.UserLogin(ReLoginToServer, GamePref.Username, GamePref.Password);
    }

    private void ReLoginToServer(bool success, int code, SignalRClient.SignalRConnectionInfo connectionInfo)
    {
        var msg = success ? "重连成功！" : $"连接失败,错误码：{code}";
        PlayerDataForGame.instance.ShowStringTips(msg);
    }

    private void ClientReconnect()
    {
        var isDeviceLogin = GamePref.ClientLoginMethod == 1;
        if (isDeviceLogin)
        {
            SignalR.Disconnect(() =>
            {
                SignalR.DirectLogin(DebugCallBack);
            });
        }else SignalR.Disconnect(() =>
        {
            SignalR.UserLogin(DebugCallBack, GamePref.Username, GamePref.Password);
        });
    }

    private void DebugCallBack(bool success, int code, SignalRClient.SignalRConnectionInfo info)
    {
        XDebug.Log<ServerPanel>(
            $"Success = {success}, Code = {code}, user: {info.Username}, Token = {info.AccessToken}");
    }
}
