using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Signal客户端
/// </summary>
public class SignalRClient : MonoBehaviour
{
    /// <summary>
    /// SignalR 网络状态
    /// </summary>
    public HubConnectionState Status;

    public event UnityAction<HubConnectionState> OnStatusChanged;
    public event UnityAction<string> OnSignalRNotify;
    public event UnityAction<string> OnSignalRMessage;
    
    public static SignalRClient instance;
    private CancellationTokenSource cancellationTokenSource;
    private static HubConnection _connection;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //Login();
        OnStatusChanged += s => DebugLog($"状态更新[{s}]!");
        OnSignalRNotify += s => DebugLog($"{nameof(OnSignalRNotify)}：{s}");
        OnSignalRMessage += s=>DebugLog($"{nameof(OnSignalRMessage)}:{s}");// (channel, msg) => DebugLog($"频道[{channel}]：{msg}");
    }

    public async void Login()
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Token.Register(() => OnConnectionClose(HubConnectionState.Disconnected, XDebug.Throw<SignalRClient>("取消连接！")));
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("UserId","TestId1");
        var response = await client.PostAsync("http://localhost:7071/api/negotiate", new StringContent(string.Empty),cancellationTokenSource.Token);
        if (!response.IsSuccessStatusCode)
        {
            DebugLog("连接失败！");
            return;
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        var connectionInfo = JsonConvert.DeserializeObject<SignalRConnectionInfo>(jsonString);
        await ConnectSignalRAsync(connectionInfo, cancellationTokenSource.Token);
    }
    
    private async Task ConnectSignalRAsync(SignalRConnectionInfo connectionInfo,CancellationToken cancellationToken)
    {
        try
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(connectionInfo.Url, cfg =>
                    cfg.AccessTokenProvider = ()=> Task.Run(() => connectionInfo.AccessToken, cancellationToken))
                .Build();
            _connection.Closed += e => OnConnectionClose(_connection.State, e);
            _connection.Reconnected += s => OnReconnected(_connection.State, s);
            _connection.Reconnecting += e => OnReconnecting(_connection.State, e);
            _connection.On<string>(EventStrings.Req_JinNang, arg=> OnSignalRNotify?.Invoke(arg));
            _connection.On<string>(EventStrings.Req_JinNang, arg => OnSignalRMessage?.Invoke(arg));
            await _connection.StartAsync(cancellationToken);
            StatusChanged(_connection.State,$"Host:{connectionInfo.Url},\nToken:{connectionInfo.AccessToken}\n连接成功！");
            cancellationTokenSource = null;
        }
        catch (Exception e)
        {
            StatusChanged(_connection.State,$"连接失败！{e}");
        }
    }

    private async Task InvokeAsync(string method,string arg,CancellationToken cancellationToken = default) => await _connection.SendAsync(method, arg, cancellationToken);
    public async void Invoke(string method, string arg) => await InvokeAsync(method, arg);

    public void RequestJinNang()
    {
        Invoke(EventStrings.Req_JinNang, "198");
    }
    /// <summary>
    /// 强制离线
    /// </summary>
    public async void Disconnect()
    {
        if (_connection.State == HubConnectionState.Disconnected)
        {
            throw new InvalidOperationException(DebugMsg("客户端当前离线。"));
        }

        if (cancellationTokenSource?.IsCancellationRequested == false)
            cancellationTokenSource.Cancel();
        if (_connection.State != HubConnectionState.Disconnected)
            await _connection.StopAsync();
    }

    #region Event

    /// <summary>
    /// 当客户端尝试重新连接服务器
    /// </summary>
    /// <param name="state"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    private Task OnReconnecting(HubConnectionState state,Exception e)
    {
        StatusChanged(state, e.ToString());
        return Task.CompletedTask;
    }

    /// <summary>
    /// 当客户端重新连线
    /// </summary>
    /// <param name="state"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    private Task OnReconnected(HubConnectionState state,string arg)
    {
        StatusChanged(state, arg);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 当客户端断线的处理方法
    /// </summary>
    /// <param name="state"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    private Task OnConnectionClose(HubConnectionState state,Exception e)
    {
        StatusChanged(state, e.ToString());
        return Task.CompletedTask;
    }
    private void StatusChanged(HubConnectionState status, string message)
    {
        Status = status; 
        OnStatusChanged?.Invoke(status);
        DebugLog(message);
    }
    #endregion

    #region DebugLog
    private string DebugMsg(string message) => $"SignalR客户端: {message}";
    private void DebugLog(string message)
    {
#if DEBUG
        XDebug.Log<SignalRClient>(DebugMsg(message));
#endif
    }

    #endregion

    private class SignalRConnectionInfo
    {
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("accessToken")] public string AccessToken { get; set; }
    }
}