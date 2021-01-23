using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

/// <summary>
/// 游戏系统时钟，每隔一段时间请求网络API获取网络时间戳，同步本地时间。以便防止玩家利用不合法的时间影响游戏数据。
/// </summary>
public class SystemTimer : MonoBehaviour
{
    private const string TaobaoTimeStampApi = "http://api.m.taobao.com/rest/api3.do?api=mtop.common.getTimestamp";
    private static readonly DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
    public static SystemTimer instance;
    public static DateTimeOffset UnixToDateTime(long unixTicks)
    {
        var utc = Epoch.AddMilliseconds(unixTicks);
        return utc.ToLocalTime();
    }
    public static bool IsToday(DateTimeOffset date) => instance.Now.LocalDateTime.Day == date.LocalDateTime.Day &&
                                           instance.Now.LocalDateTime.Month == date.LocalDateTime.Month &&
                                           instance.Now.LocalDateTime.Year == date.LocalDateTime.Year;
    public static bool IsToday(long unixTicks) => IsToday(UnixToDateTime(unixTicks));

    private DateTimeOffset startTime;
    /// <summary>
    /// 当前时间
    /// </summary>
    public DateTimeOffset Now => startTime == default ? default : startTime.AddTicks(StopWatch.ElapsedTicks);

    /// <summary>
    /// 网络请求同步失败重试次数。
    /// </summary>
    public int RetryLimit = 5;

    [InspectorName("服务器同步时间间隔")]
    public int SyncSecs = 5;
    //public int Year;
    //public int Month;
    //public int Day;
    //public int Hour;
    //public int Min;
    //public int Sec;

    private int connectionFailureCount;

    /// <summary>
    /// Unix时间戳(毫秒ms)
    /// </summary>
    public long NowUnixTicks
    {
        get
        {
            var unixTicks = (long) Now.Subtract(Epoch).TotalMilliseconds;
            return unixTicks;
        }
    }

    private Stopwatch StopWatch { get; set; }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        UpdateSystemTime(DateTime.Now);
        //UpdateDateTime(startTime.Ticks);
        StartCoroutine(SynchronizeDateTime());
    }

    private void UpdateSystemTime(DateTime systemTime)
    {
        startTime = systemTime;
        if (StopWatch == null)
        {
            StopWatch = new Stopwatch();
            StopWatch.Start();
            //Now = systemTime;
            return;
        }
        StopWatch.Restart();
    }

    private IEnumerator SynchronizeDateTime()
    {
        while (true)
        {
            RequestSynchronizeDatetime();
            yield return new WaitForSeconds(SyncSecs);
        }
    }

    private async void RequestSynchronizeDatetime()
    {
#if UNITY_EDITOR
        //DebugLog($"请求服务器...{TaobaoTimeStampApi}");
#endif
        var sw = new Stopwatch();
        sw.Start();
        var response = await Http.GetAsync(TaobaoTimeStampApi);
        sw.Stop();
#if UNITY_EDITOR
        //DebugLog($"服务器返回[{jsonApi}]"+$"DateTimeNow:[{DateTime.Now.Ticks}]");
#endif
        if (!response.IsSuccess())
        {
            if (connectionFailureCount >= RetryLimit)
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(RequestSynchronizeDatetime)}:尝试重连服务器失败次数={connectionFailureCount}/{RetryLimit}");
#endif
                PlayerDataForGame.instance.ShowStringTips("服务器连接失败，请检查网络！");
                connectionFailureCount = 0;
                return;
            }
            connectionFailureCount++;
            return;
        }

        try
        {
            var jsonApi = await response.Content.ReadAsStringAsync();
            var apiObj = Json.Deserialize<TaobaoJsonApi>(jsonApi);
            var serverTicks = long.Parse(apiObj.data.t);
            var serverTimeNow = UnixToDateTime(serverTicks).AddTicks(sw.ElapsedTicks);
#if UNITY_EDITOR
            //DebugLog($"同步游戏[{DateTime.Now:T}]和服务器[{serverTimeNow:T}]+请求时间[{sw.Elapsed.Seconds}]秒");
#endif
            UpdateSystemTime(serverTimeNow.AddSeconds(SyncSecs).LocalDateTime);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
    }
    private class TaobaoJsonApi
    {
        public string api { get; set; }
        public string v { get; set; }
        public string[] ret { get; set; }
        public Data data { get; set; }
        public class Data
        {
            public string t { get; set; }
        }
    }
}