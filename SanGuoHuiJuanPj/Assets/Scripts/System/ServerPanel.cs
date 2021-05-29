using System.Collections;
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
    private bool isConnecting;

    public void Init(SignalRClient signalR)
    {
        SignalR = signalR;
        SignalR.SubscribeAction(EventStrings.SC_Disconnect,ServerCallDisconnect);
        SignalR.OnStatusChanged += OnStatusChanged;
        reconnectButton.onClick.AddListener(ReconnectOnDisconnected);
        gameObject.SetActive(false);
        MaintenanceOrTimeoutMessage(false);
        SetException();
    }

    public void SetException(string exception = null)
    {
        exceptionMsg.gameObject.SetActive(!string.IsNullOrWhiteSpace(exception));
        exceptionMsg.text = exception;
    }

    private void MaintenanceOrTimeoutMessage(bool isMaintenance)
    {
        serverMaintenance.gameObject.SetActive(isMaintenance);
        requestTimeOut.gameObject.SetActive(!isMaintenance);
    }

    private void OnConnectAction(bool isConnected, int code, SignalRClient.SignalRConnectionInfo info)
    {
        isConnecting = false;
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
                ReconnectOnDisconnected();
                break;
        }
    }

    //当服务器强制离线
    private void ServerCallDisconnect(object[] arg)
    {
        SignalR.Disconnect();
        OnStatusChanged(HubConnectionState.Disconnected);
        MaintenanceOrTimeoutMessage(true);
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
        if(SignalR.Status == HubConnectionState.Disconnected) ReconnectOnDisconnected();
    }

    private void ReconnectOnDisconnected() => SignalR.ReconnectServer(OnRetryConnectToServer);

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
}
