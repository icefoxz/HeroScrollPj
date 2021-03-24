using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Signal客户端
/// </summary>
public class SignalRClient : MonoBehaviour
{
    /// <summary>
    /// SignalR 网络状态
    /// </summary>
    public HubConnectionState Status;
    /// <summary>
    /// 重试次数
    /// </summary>
    public int Retries = 5;
    public ServerPanel ServerPanel;
    public event UnityAction<HubConnectionState> OnStatusChanged;
    public static SignalRClient instance;
    private CancellationTokenSource cancellationTokenSource;
    private static HubConnection _connection;
    private Dictionary<string, UnityAction<string>> _actions;
    private bool isBusy;
    private int retryCount;
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
        _actions = new Dictionary<string, UnityAction<string>>();
        OnStatusChanged += s => DebugLog($"状态更新[{s}]!");
        ServerPanel.Init(this);
    }

    public async void Login(Action<bool,int> recallAction,string username = null,string password = null)
    {
        if(isBusy)return;
        if (username == null) username = PlayerDataForGame.instance.acData.Username;
        if (password == null) password = PlayerDataForGame.instance.acData.Password;
        cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Token.Register(()=>OnConnectionClose(XDebug.Throw<SignalRClient>("取消连接！")));
        var response = await Http.PostAsync(Server.SIGNALR_LOGIN_API,Json.Serialize(GetUserInfo(username,password)), cancellationTokenSource.Token);
        if (!response.IsSuccessStatusCode)
        {
            DebugLog($"连接失败！[{response.StatusCode}]");
            var severBackCode = ServerBackCode.ERR_INVALIDOPERATION;
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    severBackCode = ServerBackCode.ERR_PW_ERROR;
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    severBackCode = ServerBackCode.ERR_SERVERSTATE_ZERO;
                    break;
            }
            isBusy = false;
            recallAction.Invoke(false, (int) severBackCode);
            return;
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        var connectionInfo = JsonConvert.DeserializeObject<SignalRConnectionInfo>(jsonString);
        var result = await ConnectSignalRAsync(connectionInfo, cancellationTokenSource.Token);
        isBusy = false;
        recallAction?.Invoke(result, (int) response.StatusCode);
    }

    private UserInfo GetUserInfo(string username,string password)
    {
        return new UserInfo
        {
            DeviceId = SystemInfo.deviceUniqueIdentifier,
            Password = password,
            Phone = PlayerDataForGame.instance?.acData?.Phone,
            Username = username,
            GameVersion = float.Parse(Application.version)
        };
    }
    
    private async Task<bool> ConnectSignalRAsync(SignalRConnectionInfo connectionInfo,CancellationToken cancellationToken)
    {
        try
        {
            if (_connection != null)
            {
                ResetConnection();
                await _connection.DisposeAsync();
            }
            _connection = new HubConnectionBuilder()
                .WithUrl(connectionInfo.Url, cfg =>
                    cfg.AccessTokenProvider = () => Task.Run(() => connectionInfo.AccessToken, cancellationToken))
                .WithAutomaticReconnect(new TimeSpan[]
                {
                    TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(5)
                })
                .Build();
            _connection.ServerTimeout = TimeSpan.FromMinutes(3);
            _connection.KeepAliveInterval = TimeSpan.FromSeconds(20);
            _connection.HandshakeTimeout = TimeSpan.FromSeconds(20);
            _connection.Closed += OnConnectionClose;
            _connection.Reconnected += OnReconnected;
            _connection.Reconnecting += OnReconnecting;
            _connection.On<string, string>(EventStrings.Server_Call, OnServerCall);
            await _connection.StartAsync(cancellationToken);
            StatusChanged(_connection.State,$"Host:{connectionInfo.Url},\nToken:{connectionInfo.AccessToken}\n连接成功！");
            cancellationTokenSource = null;
        }
        catch (Exception e)
        {
            StatusChanged(_connection.State,$"连接失败！{e}");
            return false;
        }
        return true;
    }

    public void SubscribeAction(string method, UnityAction<string> action)
    {
        if (!_actions.ContainsKey(method))
            _actions.Add(method, default);
        _actions[method] += action;
    }

    public void UnSubscribeAction(string method, UnityAction<string> action)
    {
        if (!_actions.ContainsKey(method))
            throw XDebug.Throw<SignalRClient>($"Method[{method}] not registered!");
        _actions[method] -= action;
    }

    private void OnServerCall(string method, string content)
    {
#if UNITY_EDITOR
        DebugLog($"{method}: {content}");
#endif
        if(! _actions.TryGetValue(method.ToString(),out var action))return;
        action?.Invoke(content);
    }

    private async Task InvokeAsync(string method,string arg,CancellationToken cancellationToken = default) => await _connection.SendAsync(method, arg, cancellationToken);
    public async void Invoke(string method, string arg) => await InvokeAsync(method, arg);

    /// <summary>
    /// 强制离线
    /// </summary>
    public void Disconnect()
    {
        if (_connection.State == HubConnectionState.Disconnected)
            return;

        if (cancellationTokenSource?.IsCancellationRequested == false)
        {
            cancellationTokenSource.Cancel();
            ResetConnection();
            return;
        }
        if (_connection.State == HubConnectionState.Disconnected) return;
        _connection.StopAsync().Wait();
        ResetConnection();
    }

    private void ResetConnection()
    {
        _connection.Closed -= OnConnectionClose;
        _connection.Reconnected -= OnReconnected;
        _connection.Reconnecting -= OnReconnecting;
    }

    #region Event

    /// <summary>
    /// 当客户端尝试重新连接服务器
    /// </summary>
    private Task OnReconnecting(Exception exception)
    {
        retryCount++;
        StatusChanged(_connection.State, exception.ToString());
        return Task.CompletedTask;
    }

    /// <summary>
    /// 当客户端重新连线
    /// </summary>
    private Task OnReconnected(string msg)
    {
        retryCount = 0;
        StatusChanged(_connection.State, msg);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 当客户端断线的处理方法
    /// </summary>
    private Task OnConnectionClose(Exception exception)
    {
        StatusChanged(_connection.State, exception.ToString());
        if (_connection.State == HubConnectionState.Disconnected)
            ServerPanel.OnSignalRDisconnected();
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