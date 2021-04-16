using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using Object = System.Object;

/// <summary>
/// Signal客户端
/// </summary>
public class SignalRClient : MonoBehaviour
{
    /// <summary>
    /// SignalR 网络状态
    /// </summary>
    public HubConnectionState Status;

    public int ServerTimeOutMinutes = 3;
    public int KeeAliveIntervalSecs = 20;
    public int HandShakeTimeoutSecs = 20;
    public ServerPanel ServerPanel;
    public event UnityAction<HubConnectionState> OnStatusChanged;
    public static SignalRClient instance;
    private CancellationTokenSource cancellationTokenSource;
    private static HubConnection _connection;
    private Dictionary<string, UnityAction<object[]>> _actions;
    private static readonly Type _stringType = typeof(string);
    private bool isBusy;

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
        _actions = new Dictionary<string, UnityAction<object[]>>();
        OnStatusChanged += s => DebugLog($"状态更新[{s}]!");
        ServerPanel?.Init(this);
        SubscribeAction(EventStrings.SR_UploadPy,OnServerCalledUpload);
    }

    async void OnApplicationQuit()
    {
        if (_connection == null)return;
        await _connection.StopAsync();
        await _connection.DisposeAsync();
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
                case HttpStatusCode.HttpVersionNotSupported:
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

    public async Task SynchronizePlayerData()
    {
        var jData = await Invoke(EventStrings.Req_PlayerData);
        var args = Json.DeserializeList<object>(jData);
        var isSync = (bool)args[0];
        if(!isSync)return;
        var playerData = Json.Deserialize<PlayerData>(args[1].ToString());
        var codes = Json.Deserialize<Dictionary<int,string>>(args[2].ToString());
        PlayerDataForGame.instance.pyData = playerData;
        var redeems = PlayerDataForGame.instance.gbocData.redemptionCodeGotList;
        var rSave = codes.Join(redeems, r => r.Key, c => c.id, (c, r) =>
        {
            r.isGot = true;
            return r;
        }).ToList();
        PlayerDataForGame.instance.gbocData.redemptionCodeGotList = rSave;
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(6);
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
            _connection.ServerTimeout = TimeSpan.FromMinutes(ServerTimeOutMinutes);
            _connection.KeepAliveInterval = TimeSpan.FromSeconds(KeeAliveIntervalSecs);
            _connection.HandshakeTimeout = TimeSpan.FromSeconds(HandShakeTimeoutSecs);
            _connection.Closed += OnConnectionClose;
            _connection.Reconnected += OnReconnected;
            _connection.Reconnecting += OnReconnecting;
            _connection.On<string, string>(EventStrings.Server_Call, OnServerCall);
            await _connection.StartAsync(cancellationToken);
            StatusChanged(_connection.State,$"Host:{connectionInfo.Url},\nToken:{connectionInfo.AccessToken}\n连接成功！");
            cancellationTokenSource = null;
            Application.quitting += Disconnect;
        }
        catch (Exception e)
        {
            StatusChanged(_connection.State,$"连接失败！{e}");
            return false;
        }
        return true;
    }

    public void SubscribeAction(string method, UnityAction<object[]> action)
    {
        if (!_actions.ContainsKey(method))
            _actions.Add(method, default);
        _actions[method] += action;
    }

    public void UnSubscribeAction(string method, UnityAction<object[]> action)
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
        if(! _actions.TryGetValue(method,out var action))return;
        var args = Json.Deserialize<object[]>(content);
        action?.Invoke(args);
    }

    public async void Send(string method, object[] args, CancellationToken cancellationToken = default)
    {
        try
        {
            await _connection.SendAsync(method, Json.Serialize(args), cancellationToken);
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            XDebug.LogError<SignalRClient>(e.Message);
            throw;
#endif
        }
    }

    public async Task<string> Invoke(string method, object[] args = default, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _connection.InvokeCoreAsync(method, _stringType, new object[] {Json.Serialize(args)},
                cancellationToken);
            return result?.ToString();
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            XDebug.LogError<SignalRClient>(e.Message);
            throw;
#endif
        }
    }

    /// <summary>
    /// 强制离线
    /// </summary>
    public async void Disconnect()
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
        await _connection.StopAsync();
        ResetConnection();
    }

    private void ResetConnection()
    {
        _connection.Closed -= OnConnectionClose;
        _connection.Reconnected -= OnReconnected;
        _connection.Reconnecting -= OnReconnecting;
        Application.quitting -= Disconnect;
    }

    #region Upload

    private async void OnServerCalledUpload(object[] args)
    {
        var playerData = PlayerDataForGame.instance.pyData;
        var redeemCodeIds = PlayerDataForGame.instance.gbocData.redemptionCodeGotList.Where(c => c.isGot)
            .Select(c => c.id).ToList();
        var redeemedCodes = redeemCodeIds.Join(DataTable.RCode, id => id, d => d.Key, (_, d) => d.Value.Code).ToList();
        var token = args[0];
        var param = new[] {token, playerData, redeemedCodes};
        await Invoke(EventStrings.Req_UploadPy, param);
    }

    #endregion

    #region Event

    /// <summary>
    /// 当客户端尝试重新连接服务器
    /// </summary>
    private Task OnReconnecting(Exception exception)
    {
        StatusChanged(_connection.State, exception.ToString());
        return Task.CompletedTask;
    }

    /// <summary>
    /// 当客户端重新连线
    /// </summary>
    private Task OnReconnected(string msg)
    {
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
            ServerPanel?.OnSignalRDisconnected();
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