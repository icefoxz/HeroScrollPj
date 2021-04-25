using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Utl;
using Beebyte.Obfuscator;
using CorrelateLib;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = System.Object;

[Skip]
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
    public ApiPanel ApiPanel;

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
    }

    private void Start()
    {
        //Login();
        _actions = new Dictionary<string, UnityAction<object[]>>();
        OnStatusChanged += s => DebugLog($"状态更新[{s}]!");
        ServerPanel?.Init(this);
        SubscribeAction(EventStrings.SR_UploadPy, OnServerCalledUpload);
        ApiPanel.Init(this);
    }

    async void OnApplicationQuit()
    {
        if (_connection == null)return;
        await _connection.StopAsync();
        await _connection.DisposeAsync();
    }

    /// <summary>
    /// login
    /// </summary>
    /// <param name="recallAction">( isSuccess, statusCode, arrangement )</param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public async void UserLogin(UnityAction<bool,int ,SignalRConnectionInfo> recallAction, string username = null,
        string password = null)
    {
        if (isBusy) return;
        if (username == null) username = PlayerDataForGame.instance.acData.Username;
        if (password == null) password = PlayerDataForGame.instance.acData.Password;
        cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Token.Register(() => OnConnectionClose(XDebug.Throw<SignalRClient>("取消连接！")));
        var response = await Http.PostAsync(Server.SIGNALR_LOGIN_API,
            Json.Serialize(Server.GetUserInfo(username, password)), cancellationTokenSource.Token);
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
            recallAction.Invoke(false, (int) severBackCode, null);
            return;
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        var connectionInfo = JsonConvert.DeserializeObject<SignalRConnectionInfo>(jsonString);
        var result = await ConnectSignalRAsync(connectionInfo, cancellationTokenSource.Token);
        isBusy = false;
        recallAction?.Invoke(result, (int) response.StatusCode, connectionInfo);
    }

    public async void DirectLogin(UnityAction<bool, int,SignalRConnectionInfo> recallAction)
    {
        if (isBusy) return;
        cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Token.Register(() => OnConnectionClose(XDebug.Throw<SignalRClient>("取消连接！")));
        var response = await Http.PostAsync(Server.DEVICE_LOGIN_API,
            Json.Serialize(Server.GetUserInfo(GamePref.Username, GamePref.Password)), cancellationTokenSource.Token);
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
            recallAction.Invoke(false, (int) severBackCode,null);
            return;
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        var connectionInfo = JsonConvert.DeserializeObject<SignalRConnectionInfo>(jsonString);
        var result = await ConnectSignalRAsync(connectionInfo, cancellationTokenSource.Token);
        isBusy = false;
        recallAction?.Invoke(result, (int) response.StatusCode, connectionInfo);
    }

    public async Task SynchronizeSaved()
    {
        var jData = await Invoke(EventStrings.Req_Saved);
        var bag = Json.Deserialize<ViewBag>(jData);
        var playerData = bag.GetPlayerDataDto();
        var warChestList = bag.GetPlayerWarChests();
        var redeemedList = bag.GetPlayerRedeemedCodes();
        var warCampaignList = bag.GetPlayerWarCampaignDtos();
        var gameCardList = bag.GetPlayerGameCardDtos();
        var troops = bag.GetPlayerTroopDtos();
        PlayerDataForGame.instance.pyData = PlayerData.Instance(playerData);
        PlayerDataForGame.instance.warsData.warUnlockSaveData = warCampaignList.Select(w => new UnlockWarCount
        {
            warId = w.WarId,
            isTakeReward = w.IsFirstRewardTaken,
            unLockCount = w.UnlockProgress
        }).ToList();
        PlayerDataForGame.instance.UpdateGameCards(gameCardList, troops);
        PlayerDataForGame.instance.gbocData.redemptionCodeGotList = redeemedList.Join(DataTable.RCode.Values, c => c,
            r => r.Code, (_, r) => new RedemptionCodeGot {id = r.Id, isGot = true}).ToList();
        PlayerDataForGame.instance.gbocData.fightBoxs = warChestList.ToList();
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData();
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
            StatusChanged(_connection?.State ?? HubConnectionState.Disconnected, $"连接失败！{e}");
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

    public async void Invoke(string method, UnityAction<string> recallAction , IViewBag bag = default,
        CancellationToken cancellationToken = default)
    {
        var result = await Invoke(method, bag, cancellationToken);
        UnityMainThread.thread.RunNextFrame(()=>recallAction?.Invoke(result));
    }

    private async Task<string> Invoke(string method, IViewBag bag = default, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _connection.InvokeCoreAsync(method, _stringType,
                bag == null ? new object[0] : new object[] {Json.Serialize(bag)},
                cancellationToken);
            return result?.ToString();
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            XDebug.LogError<SignalRClient>($"{method}:{e.Message}");
            throw;
#endif
            return null;
        }
    }

    /// <summary>
    /// 强制离线
    /// </summary>
    public async void Disconnect()
    {
        if (_connection.State == HubConnectionState.Disconnected)
            return;

        if (cancellationTokenSource!=null && cancellationTokenSource.IsCancellationRequested == false)
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
        Application.quitting -= Disconnect;
        if (_connection == null) return;
        _connection.Closed -= OnConnectionClose;
        _connection.Reconnected -= OnReconnected;
        _connection.Reconnecting -= OnReconnecting;
    }

    #region Upload

    private async void OnServerCalledUpload(object[] args)
    {
        var saved = PlayerDataForGame.instance;
        var playerData = saved.pyData;
        var redeemCodeIds = saved.gbocData.redemptionCodeGotList.Where(c => c.isGot)
            .Select(c => c.id).ToList();
        var warChest = saved.gbocData.fightBoxs.ToArray();
        var redeemedCodes = redeemCodeIds.Join(DataTable.RCode, id => id, d => d.Key, (_, d) => d.Value.Code).ToArray();
        var token = args[0];
        var campaign = saved.warsData.warUnlockSaveData.Select(w => new WarCampaignDto
                {WarId = w.warId, IsFirstRewardTaken = w.isTakeReward, UnlockProgress = w.unLockCount})
            .Where(w => w.UnlockProgress > 0).ToArray();
        var cards = saved.hstData.heroSaveData
            .Join(DataTable.Hero, c => c.id, h => h.Key, (c, h) => new {ForceId = h.Value.ForceTableId, c})
            .Concat(saved.hstData.towerSaveData.Join(DataTable.Tower, c => c.id, t => t.Key,
                (c, t) => new {t.Value.ForceId, c})).Concat(saved.hstData.trapSaveData.Join(DataTable.Trap, c => c.id,
                t => t.Key, (c, t) => new {t.Value.ForceId, c})).Where(c => c.c.chips > 0 || c.c.level > 0).ToList();
        var troops = cards.GroupBy(c => c.ForceId, c => c).Select(c =>
        {
            var list = c.GroupBy(o => o.c.typeIndex, o => o.c)
                .ToDictionary(o => (GameCardType) o.Key, o => o.Select(a => a).ToArray());
            return new TroopDto
            {
                ForceId = c.Key,
                Cards = list.ToDictionary(l => l.Key, l => l.Value.Select(o => o.id).ToArray()),
                EnList = list.ToDictionary(l => l.Key,
                    l => l.Value.Where(o => o.isFight > 0).Select(o => o.id).ToArray())
            };
        }).ToArray();
        var viewBag = ViewBag.Instance()
            .SetValue(token)
            .PlayerDataDto(playerData.ToDto())
            .PlayerRedeemedCodes(redeemedCodes)
            .PlayerWarChests(warChest)
            .PlayerWarCampaignDtos(campaign)
            .PlayerGameCardDtos(cards.Select(c => c.c.ToDto()).ToArray())
            .PlayerTroopDtos(troops);
        await Invoke(EventStrings.Req_UploadPy, viewBag);
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

    public class SignalRConnectionInfo
    {
        public string Url { get; set; }
        public string AccessToken { get; set; }
        public int Arrangement { get; set; }
        public int IsNewRegistered { get; set; }
        public string Username { get; set; }

        public SignalRConnectionInfo(string url, string accessToken, string username, int arrangement, int isNewRegistered)
        {
            Url = url;
            AccessToken = accessToken;
            Arrangement = arrangement;
            IsNewRegistered = isNewRegistered;
            Username = username;
        }
    }
}