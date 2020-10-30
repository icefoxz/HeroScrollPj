using System;
using System.Collections;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TimeSystemControl : MonoBehaviour
{
    public static TimeSystemControl instance;

    public static string staminaStr = "staminaNum";

    private string timeWebPath = "http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=1";
    private string timeWebPath0 = "http://api.m.taobao.com/rest/api3.do?api=mtop.common.getTimestamp";

    public static string NetworkTimestampStr = "NetworkTimestamp";

    private string fBoxOpenNeedTimes = "fBONT"; //游戏内的计时器记录免费宝箱开启时间
    private string fBoxOpenNeedTimes1 = "fBONT1";
    private string fBoxOpenNeedTimes2 = "fBONT2";
    private string jinNangOpenNeedTime = "jNBONT";   //游戏内的计时器记录锦囊开启时间
    private string freeBoxOpenTime = "fBOT"; //记录免费宝箱网络开启时间
    private string freeBoxOpenTime1 = "fBOT1";
    private string freeBoxOpenTime2 = "fBOT2";
    private string jinNangOpenTime = "jNBOT";   //记录锦囊网络开启时间

    int openNeedSeconds;   //宝箱开启时间
    int openNeedSeconds1;
    int openNeedSeconds2;
    int openJNNeedSeconds;//锦囊开启时间
    int secondsNetTime_FreeBox = 0; //记录免费宝箱网络开启剩余时间
    int secondsNetTime_FreeBox1 = 0;
    int secondsNetTime_FreeBox2 = 0;
    int secondsNetTime_JinNang = 0; //记录锦囊网络开启剩余时间
    long openFreeBoxTimeLong;   //记录宝箱开启的网络long
    long openFreeBoxTimeLong1;
    long openFreeBoxTimeLong2;
    long openJinNangTimeLong;   //记录锦囊开启的网络long

    private string tiLiHuiFuNeedTimes = "tLHFNT"; //游戏内的计时器记录体力恢复时间 单位秒
    private string tiLiHuiFuTime = "tLHFT"; //记录体力恢复满的网络时间点
    int oneTiLiHfSeconds = 600;   //单个体力恢复时间12分钟
    int secondsNetTime_TiLiHf = 0; //记录体力恢复网络剩余时间
    long tiLiHfTimeLong;   //记录体力恢复满的网络long
    int maxStaminaNum;  //记录最大体力值

    [HideInInspector]
    public bool isGetNetworkTime;   //标记是否获取到网络时间

    [HideInInspector]
    public bool isOpenMainScene;    //是否在主城界面

    bool isCanGetBox;   //记录是否能开启宝箱
    bool isCanGetBox1;
    bool isCanGetBox2;
    bool isCanGetJN;    //记录是否能开启锦囊

    bool isNeedHuiFuTiLi;   //记录是否需要恢复体力

    DateTime timeNow;
    DateTime startTime;

    float secondHandAccurate = 0;   //精准游戏毫秒针
    float secondHandAccurate1 = 0;
    float secondHandAccurate2 = 0;
    float secondHandAccurateJN = 0;

    float secondHandAccurate_TL = 0;   //精准游戏毫秒针0

    long nowTimeLong;   //当前网络时间戳long

    //鸡坛相关存档字符
    public static string openCKTime0_str = "openCKTime0"; //12点
    public static string openCKTime1_str = "openCKTime1"; //17点
    public static string openCKTime2_str = "openCKTime2"; //21点

    [HideInInspector]
    public bool isFInGame;  //记录当天首次进入游戏
    private static string dayOfyearStr = "dayOfyearStr";        //存放上次进入游戏是哪一天
    private static string yearStr = "yearStr";                  //存放上次进入游戏是哪一年
    
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
        isGetNetworkTime = false;
        isOpenMainScene = false;
        timeNow = DateTime.MinValue;
        startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        
        StartCoroutine(GetTime());

        //StartCoroutine(GetTime0());

    }

    private void Start()
    {
        StartGameToInitOpenTime();
    }

    //初次存档对时间相关数据进行初始化
    public void InitTimeRelatedData()
    {
        isCanGetBox = true;
        openFreeBoxTimeLong = 0;
        PlayerPrefs.SetInt(fBoxOpenNeedTimes, 0);
        PlayerPrefs.SetString(freeBoxOpenTime, "0");


        isCanGetBox1 = false;
        openNeedSeconds1 = LoadJsonFile.GetGameValue(1);
        PlayerPrefs.SetInt(fBoxOpenNeedTimes1, openNeedSeconds1);
        if (isGetNetworkTime)
        {
            long nowTimeStr = nowTimeLong;
            nowTimeStr += (openNeedSeconds1 * 1000);
            openFreeBoxTimeLong1 = nowTimeStr;
            PlayerPrefs.SetString(freeBoxOpenTime1, nowTimeStr.ToString());
        }
        else
        {
            openFreeBoxTimeLong1 = 0;
            PlayerPrefs.SetString(freeBoxOpenTime1, "0");
        }
        //openFreeBoxTimeLong1 = 0;
        //PlayerPrefs.SetInt(fBoxOpenNeedTimes1, 0);
        //PlayerPrefs.SetString(freeBoxOpenTime1, "0");

        isCanGetBox2 = true;
        openFreeBoxTimeLong2 = 0;
        PlayerPrefs.SetInt(fBoxOpenNeedTimes2, 0);
        PlayerPrefs.SetString(freeBoxOpenTime2, "0");

        isCanGetJN = true;
        openJinNangTimeLong = 0;
        PlayerPrefs.SetInt(jinNangOpenNeedTime, 0);
        PlayerPrefs.SetString(jinNangOpenTime, "0");

        //isNeedHuiFuTiLi = false;
        PlayerPrefs.SetInt(staminaStr, int.Parse(LoadJsonFile.assetTableDatas[2].startValue));
        //tiLiHfTimeLong = 0;
        PlayerPrefs.SetInt(tiLiHuiFuNeedTimes, 0);
        PlayerPrefs.SetString(tiLiHuiFuTime, "0");
        maxStaminaNum = int.Parse(LoadJsonFile.assetTableDatas[2].startValue);
        isNeedHuiFuTiLi = (PlayerPrefs.GetInt(staminaStr) < maxStaminaNum);
        tiLiHfTimeLong = long.Parse(PlayerPrefs.GetString(tiLiHuiFuTime));

        PlayerPrefs.SetInt(openCKTime0_str, 0);
        PlayerPrefs.SetInt(openCKTime1_str, 0);
        PlayerPrefs.SetInt(openCKTime2_str, 0);

        PlayerPrefs.SetInt(dayOfyearStr, 0);
        PlayerPrefs.SetInt(yearStr, 2020);
    }

    //每次进入游戏对开启时间进行初始化
    private void StartGameToInitOpenTime()
    {
        openNeedSeconds = LoadJsonFile.GetGameValue(0);  //宝箱开启时间
        openNeedSeconds1 = LoadJsonFile.GetGameValue(1);
        openNeedSeconds2 = LoadJsonFile.GetGameValue(2);
        openJNNeedSeconds = 3600; //锦囊开启时间

        isCanGetBox = (PlayerPrefs.GetInt(fBoxOpenNeedTimes) <= 0);
        openFreeBoxTimeLong = long.Parse(PlayerPrefs.GetString(freeBoxOpenTime));

        isCanGetBox1 = (PlayerPrefs.GetInt(fBoxOpenNeedTimes1) <= 0);
        openFreeBoxTimeLong1 = long.Parse(PlayerPrefs.GetString(freeBoxOpenTime1));

        isCanGetBox2 = (PlayerPrefs.GetInt(fBoxOpenNeedTimes2) <= 0);
        openFreeBoxTimeLong2 = long.Parse(PlayerPrefs.GetString(freeBoxOpenTime2));

        isCanGetJN = (PlayerPrefs.GetInt(jinNangOpenNeedTime) <= 0);
        openJinNangTimeLong = long.Parse(PlayerPrefs.GetString(jinNangOpenTime));

        maxStaminaNum = int.Parse(LoadJsonFile.assetTableDatas[2].startValue);
        isNeedHuiFuTiLi = (PlayerPrefs.GetInt(staminaStr) < maxStaminaNum);
        tiLiHfTimeLong = long.Parse(PlayerPrefs.GetString(tiLiHuiFuTime));
    }

    private void Update()
    {
        //StartCoroutine(GetTime());
        UpdateFreeBoxTimer();
        UpdateFreeBoxTimer1();
        UpdateFreeBoxTimer2();
        UpdateJinNangTimer();
        UpdateStaminaTimer();

        UpdateChickenShoping();
    }

    /// <summary>
    /// 确认是否是当天第一次加载游戏
    /// </summary>
    public void InitIsTodayFirstLoadingGame()
    {
        if (isGetNetworkTime)
        {
            DateTime nowTime = GetStrBackTime();
            //DateTime nowTime = DateTime.Now;

            if (nowTime.Year > PlayerPrefs.GetInt(yearStr))
            {
                PlayerPrefs.SetInt(yearStr, nowTime.Year);
                PlayerPrefs.SetInt(dayOfyearStr, nowTime.DayOfYear);
                isFInGame = true;
            }
            else
            {
                if (nowTime.DayOfYear > PlayerPrefs.GetInt(dayOfyearStr))
                {
                    PlayerPrefs.SetInt(dayOfyearStr, nowTime.DayOfYear);
                    isFInGame = true;
                }
                else
                {
                    isFInGame = false;
                }
            }
        }
        else
        {
            isFInGame = false;
        }
    }

    //修正下次进入已经不是今天第一次进入游戏了
    public void UpdateIsNotFirstInGame()
    {
        DateTime nowTime = GetStrBackTime();
        //DateTime nowTime = DateTime.Now;

        PlayerPrefs.SetInt(yearStr, nowTime.Year);
        PlayerPrefs.SetInt(dayOfyearStr, nowTime.DayOfYear);
        isFInGame = false;
    }

    //尝试刷新主城体力商店状态
    private void UpdateChickenShoping()
    {
        //在主界面的话
        if (isOpenMainScene)
        {
            UIManager.instance.InitOpenChickenTime(isGetNetworkTime);
            //UIManager.instance.InitOpenChickenTime(true);
        }
    }

    //返回时间格式
    public string BackToTimeShow(int seconds)
    {
        string str = string.Empty;
        if (seconds <= 0)
        {
            str = "";
        }
        else
        {
            if (seconds < 3600)
            {
                str = seconds / 60 + "分" + seconds % 60 + "秒";
            }
            else
            {
                if (seconds < 86400)
                {
                    str = seconds / 3600 + "时" + (seconds % 3600) / 60 + "分";
                }
                else
                {
                    str = seconds / 86400 + "天" + (seconds % 86400) / 3600 + "时";
                }
            }
        }
        return str;
    }

    /// <summary>
    /// 添加体力
    /// </summary>
    /// <param name="addNums"></param>
    public void AddTiLiNums(int addNums)
    {
        int needTimes = PlayerPrefs.GetInt(tiLiHuiFuNeedTimes);

        int cutTimes = oneTiLiHfSeconds * addNums;

        if (cutTimes < needTimes)
        {
            needTimes = needTimes - cutTimes;
            secondsNetTime_TiLiHf = secondsNetTime_TiLiHf - (cutTimes * 1000);
            PlayerPrefs.SetInt(tiLiHuiFuNeedTimes, needTimes);
        }
        else
        {
            int nowStamina = PlayerPrefs.GetInt(staminaStr);
            PlayerPrefs.SetInt(staminaStr, nowStamina + addNums);   //存入增加后得体力值

            if (isNeedHuiFuTiLi)
            {
                isNeedHuiFuTiLi = false;    //记录不需要再倒计时恢复体力

                PlayerPrefs.SetInt(tiLiHuiFuNeedTimes, 0);  //游戏内恢复满总时间归零

                tiLiHfTimeLong = 0; //体力恢复满的网络long标签归零

                PlayerPrefs.SetString(tiLiHuiFuTime, tiLiHfTimeLong.ToString());  //体力恢复满的网络时间点归零

                secondsNetTime_TiLiHf = 0;  //体力恢复网络剩余时间标签归零
            }

            if (isOpenMainScene)
            {
                UIManager.instance.UpdateShowTiLiInfo(BackToTimeShow(0));
            }
        }

        tiLiHfTimeLong = 0;
    }

    private void UpdateStaminaTimer()
    {
        if (isNeedHuiFuTiLi)
        {
            if (isGetNetworkTime)
            {
                if (tiLiHfTimeLong == 0)
                {
                    tiLiHfTimeLong = nowTimeLong + PlayerPrefs.GetInt(tiLiHuiFuNeedTimes) * 1000;
                    PlayerPrefs.SetString(tiLiHuiFuTime, tiLiHfTimeLong.ToString());
                }
                int secondsCha = (int)((tiLiHfTimeLong - nowTimeLong) / 1000);
                if (secondsNetTime_TiLiHf > secondsCha || (secondsNetTime_TiLiHf <= 0 && secondsCha != 0))
                {
                    secondsNetTime_TiLiHf = secondsCha;
                    int totalTimes = PlayerPrefs.GetInt(tiLiHuiFuNeedTimes);
                    UpdateTiLiHuiFuTime(totalTimes - secondsNetTime_TiLiHf, totalTimes);
                }
            }
            else
            {
                secondHandAccurate_TL += Time.deltaTime;
                if (secondHandAccurate_TL >= 1f)
                {
                    secondHandAccurate_TL = 0;
                    UpdateTiLiHuiFuTime(1, PlayerPrefs.GetInt(tiLiHuiFuNeedTimes));
                }
            }
        }
        else
        {
            if (isOpenMainScene)
            {
                UIManager.instance.UpdateShowTiLiInfo(BackToTimeShow(0));
            }
        }
    }

    //体力恢复时间缩减
    private void UpdateTiLiHuiFuTime(int cutSeconds, int totalTimes)
    {
        //Debug.Log("cutSeconds: " + cutSeconds + "  totalTimes: " + totalTimes);
        int secondsRemaining = 0;
        int totalTimesNums = totalTimes;    //恢复满总需时间

        totalTimesNums -= cutSeconds;
        if (totalTimesNums <= 0)
        {
            totalTimesNums = 0;
            PlayerPrefs.SetInt(staminaStr, maxStaminaNum);
            secondsRemaining = 0;
            isNeedHuiFuTiLi = false;
            tiLiHfTimeLong = 0;
            PlayerPrefs.SetString(tiLiHuiFuTime, "0");
        }
        else
        {
            secondsRemaining = totalTimesNums % oneTiLiHfSeconds;
            int nowStaminaNum = (int.Parse(LoadJsonFile.assetTableDatas[2].startValue) * oneTiLiHfSeconds - totalTimesNums) / oneTiLiHfSeconds;
            PlayerPrefs.SetInt(staminaStr, nowStaminaNum);
        }
        if (isOpenMainScene)
        {
            UIManager.instance.UpdateShowTiLiInfo(BackToTimeShow(secondsRemaining));
        }
        PlayerPrefs.SetInt(tiLiHuiFuNeedTimes, totalTimesNums);
    }

    //消耗体力
    public void LetTiLiTimerTake(int usedNums)
    {
        int nowStamina = PlayerPrefs.GetInt(staminaStr);

        if ((nowStamina - usedNums) < maxStaminaNum)
        {
            isNeedHuiFuTiLi = true;
            if (nowStamina > maxStaminaNum)
            {
                usedNums = usedNums - (nowStamina - maxStaminaNum);
            }

            PlayerPrefs.SetInt(tiLiHuiFuNeedTimes, oneTiLiHfSeconds * usedNums + PlayerPrefs.GetInt(tiLiHuiFuNeedTimes));

            secondsNetTime_TiLiHf += (usedNums * oneTiLiHfSeconds * 1000);

            if (isGetNetworkTime)
            {
                if (tiLiHfTimeLong == 0)
                {
                    long huiFuTimeLong = nowTimeLong;
                    huiFuTimeLong += (PlayerPrefs.GetInt(tiLiHuiFuNeedTimes) * 1000);
                    tiLiHfTimeLong = huiFuTimeLong;
                }
                else
                {
                    tiLiHfTimeLong += (oneTiLiHfSeconds * 1000 * usedNums);
                }
                PlayerPrefs.SetString(tiLiHuiFuTime, tiLiHfTimeLong.ToString());
            }
            else
            {
                tiLiHfTimeLong = 0;
                long tiLiHuiFu_Net = long.Parse(PlayerPrefs.GetString(tiLiHuiFuTime));
                if (tiLiHuiFu_Net != 0)
                {
                    PlayerPrefs.SetString(tiLiHuiFuTime, (tiLiHuiFu_Net + oneTiLiHfSeconds * 1000 * usedNums).ToString());
                }
                else
                {
                    PlayerPrefs.SetString(tiLiHuiFuTime, "0");
                }
            }
        }
        else
        {
            isNeedHuiFuTiLi = false;
        }
    }

    private void UpdateFreeBoxTimer()
    {
        if (!isCanGetBox)
        {
            if (isGetNetworkTime)   //联网
            {
                if (openFreeBoxTimeLong == 0)
                {
                    openFreeBoxTimeLong = nowTimeLong + PlayerPrefs.GetInt(fBoxOpenNeedTimes) * 1000;
                    PlayerPrefs.SetString(freeBoxOpenTime, openFreeBoxTimeLong.ToString());
                }
                int secondCha = (int)((openFreeBoxTimeLong - nowTimeLong) / 1000);
                if (secondsNetTime_FreeBox > secondCha || (secondsNetTime_FreeBox <= 0 && secondCha != 0))
                {
                    //Debug.Log("secondCha: " + secondCha);
                    secondsNetTime_FreeBox = secondCha;
                    int totalTimes = PlayerPrefs.GetInt(fBoxOpenNeedTimes);
                    UpdateBoxOpenTimeFromGame(totalTimes - secondsNetTime_FreeBox, totalTimes);
                }
            }
            else
            {
                secondHandAccurate += Time.deltaTime;
                if (secondHandAccurate >= 1f)
                {
                    secondHandAccurate = 0;
                    UpdateBoxOpenTimeFromGame(1, PlayerPrefs.GetInt(fBoxOpenNeedTimes));
                }
            }
        }
        else
        {
            if (isOpenMainScene)
            {
                UIManager.instance.transform.GetComponent<GetOrOpenBox>().UpdateOpenTimeTips(string.Empty, 0, true);
            }
        }
    }

    private void UpdateFreeBoxTimer1()
    {
        if (!isCanGetBox1)
        {
            if (isGetNetworkTime)   //联网
            {
                if (openFreeBoxTimeLong1 == 0)
                {
                    openFreeBoxTimeLong1 = nowTimeLong + PlayerPrefs.GetInt(fBoxOpenNeedTimes1) * 1000;
                    PlayerPrefs.SetString(freeBoxOpenTime1, openFreeBoxTimeLong1.ToString());
                }
                int secondCha = (int)((openFreeBoxTimeLong1 - nowTimeLong) / 1000);
                if (secondsNetTime_FreeBox1 > secondCha || (secondsNetTime_FreeBox1 <= 0 && secondCha != 0))
                {
                    secondsNetTime_FreeBox1 = secondCha;
                    int totalTimes = PlayerPrefs.GetInt(fBoxOpenNeedTimes1);
                    UpdateBoxOpenTimeFromGame1(totalTimes - secondsNetTime_FreeBox1, totalTimes);
                }
            }
            else
            {
                secondHandAccurate1 += Time.deltaTime;
                if (secondHandAccurate1 >= 1f)
                {
                    secondHandAccurate1 = 0;
                    UpdateBoxOpenTimeFromGame1(1, PlayerPrefs.GetInt(fBoxOpenNeedTimes1));
                }
            }
        }
        else
        {
            if (isOpenMainScene)
            {
                UIManager.instance.transform.GetComponent<GetOrOpenBox>().UpdateOpenTimeTips(string.Empty, 1, true);
            }
        }
    }

    private void UpdateFreeBoxTimer2()
    {
        if (!isCanGetBox2)
        {
            if (isGetNetworkTime)   //联网
            {
                if (openFreeBoxTimeLong2 == 0)
                {
                    openFreeBoxTimeLong2 = nowTimeLong + PlayerPrefs.GetInt(fBoxOpenNeedTimes2) * 1000;
                    PlayerPrefs.SetString(freeBoxOpenTime2, openFreeBoxTimeLong2.ToString());
                }
                int secondCha = (int)((openFreeBoxTimeLong2 - nowTimeLong) / 1000);
                if (secondsNetTime_FreeBox2 > secondCha || (secondsNetTime_FreeBox2 <= 0 && secondCha != 0))
                {
                    secondsNetTime_FreeBox2 = secondCha;
                    int totalTimes = PlayerPrefs.GetInt(fBoxOpenNeedTimes2);
                    UpdateBoxOpenTimeFromGame2(totalTimes - secondsNetTime_FreeBox2, totalTimes);
                }
            }
            else
            {
                secondHandAccurate2 += Time.deltaTime;
                if (secondHandAccurate2 >= 1f)
                {
                    secondHandAccurate2 = 0;
                    UpdateBoxOpenTimeFromGame2(1, PlayerPrefs.GetInt(fBoxOpenNeedTimes2));
                }
            }
        }
        else
        {
            if (isOpenMainScene)
            {
                UIManager.instance.transform.GetComponent<GetOrOpenBox>().UpdateOpenTimeTips(string.Empty, 2, true);
            }
        }
    }

    private void UpdateJinNangTimer()
    {
        if (!isCanGetJN)
        {
            if (isGetNetworkTime)   //联网
            {
                if (openJinNangTimeLong == 0)
                {
                    openJinNangTimeLong = nowTimeLong + PlayerPrefs.GetInt(jinNangOpenNeedTime) * 1000;
                    PlayerPrefs.SetString(jinNangOpenTime, openJinNangTimeLong.ToString());
                }
                int secondCha = (int)((openJinNangTimeLong - nowTimeLong) / 1000);
                if (secondsNetTime_JinNang > secondCha || (secondsNetTime_JinNang <= 0 && secondCha != 0))
                {
                    secondsNetTime_JinNang = secondCha;
                    int totalTimes = PlayerPrefs.GetInt(jinNangOpenNeedTime);
                    UpdateJinNangTimeFromGame(totalTimes - secondsNetTime_JinNang, totalTimes);
                }
            }
            else
            {
                secondHandAccurateJN += Time.deltaTime;
                if (secondHandAccurateJN >= 1f)
                {
                    secondHandAccurateJN = 0;
                    UpdateJinNangTimeFromGame(1, PlayerPrefs.GetInt(jinNangOpenNeedTime));
                }
            }
        }
        else
        {
            //可以开启时
            if (isOpenMainScene)
            {
                UIManager.instance.UpdateShowJinNangBtn(isCanGetJN);
            }
        }
    }


    //宝箱开启时间缩减
    private void UpdateBoxOpenTimeFromGame(int cutSeconds, int totalTimes)
    {
        int countDownNums = totalTimes;
        countDownNums -= cutSeconds;
        if (countDownNums <= 0)
        {
            countDownNums = 0;
            isCanGetBox = true;
            if (isOpenMainScene)
            {
                UIManager.instance.transform.GetComponent<GetOrOpenBox>().UpdateOpenTimeTips(string.Empty, 0, true);
            }
        }
        else
        {
            if (isOpenMainScene)
            {
                UIManager.instance.transform.GetComponent<GetOrOpenBox>().UpdateOpenTimeTips(BackToTimeShow(countDownNums), 0, false);
            }
        }
        PlayerPrefs.SetInt(fBoxOpenNeedTimes, countDownNums);
    }

    private void UpdateBoxOpenTimeFromGame1(int cutSeconds, int totalTimes)
    {
        int countDownNums = totalTimes;
        countDownNums -= cutSeconds;
        if (countDownNums <= 0)
        {
            countDownNums = 0;
            isCanGetBox1 = true;
            if (isOpenMainScene)
            {
                UIManager.instance.transform.GetComponent<GetOrOpenBox>().UpdateOpenTimeTips(string.Empty, 1, true);
            }
        }
        else
        {
            if (isOpenMainScene)
            {
                UIManager.instance.transform.GetComponent<GetOrOpenBox>().UpdateOpenTimeTips(BackToTimeShow(countDownNums), 1, false);
            }
        }
        PlayerPrefs.SetInt(fBoxOpenNeedTimes1, countDownNums);
    }

    private void UpdateBoxOpenTimeFromGame2(int cutSeconds, int totalTimes)
    {
        int countDownNums = totalTimes;
        countDownNums -= cutSeconds;
        if (countDownNums <= 0)
        {
            countDownNums = 0;
            isCanGetBox2 = true;
            if (isOpenMainScene)
            {
                UIManager.instance.transform.GetComponent<GetOrOpenBox>().UpdateOpenTimeTips(string.Empty, 2, true);
            }
        }
        else
        {
            if (isOpenMainScene)
            {
                UIManager.instance.transform.GetComponent<GetOrOpenBox>().UpdateOpenTimeTips(BackToTimeShow(countDownNums), 2, false);
            }
        }
        PlayerPrefs.SetInt(fBoxOpenNeedTimes2, countDownNums);
    }

    private void UpdateJinNangTimeFromGame(int cutSeconds, int totalTimes)
    {
        int countDownNums = totalTimes;
        countDownNums -= cutSeconds;
        if (countDownNums <= 0)
        {
            countDownNums = 0;
            isCanGetJN = true;
            //可以开启锦囊
            if (isOpenMainScene)
            {
                UIManager.instance.UpdateShowJinNangBtn(isCanGetJN);
            }
        }
        else
        {
            if (isOpenMainScene)
            {
                UIManager.instance.UpdateShowJinNangBtn(isCanGetJN);
            }
        }
        PlayerPrefs.SetInt(jinNangOpenNeedTime, countDownNums);
    }



    /// <summary>
    /// 开启免费宝箱
    /// </summary>
    /// <returns></returns>
    public bool OnClickToGetFreeBox()
    {
        if (isCanGetBox)
        {
            //Debug.Log("打开免费宝箱");
            secondsNetTime_FreeBox = 0;
            isCanGetBox = false;
            PlayerPrefs.SetInt(fBoxOpenNeedTimes, openNeedSeconds);
            if (isGetNetworkTime)
            {
                long nowTimeStr = nowTimeLong;//long.Parse(PlayerPrefs.GetString(NetworkTimestampStr));
                nowTimeStr += (openNeedSeconds * 1000);
                openFreeBoxTimeLong = nowTimeStr;
                PlayerPrefs.SetString(freeBoxOpenTime, nowTimeStr.ToString());
            }
            else
            {
                openFreeBoxTimeLong = 0;
                PlayerPrefs.SetString(freeBoxOpenTime, "0");
            }
            return true;
        }
        else
        {
            UIManager.instance.ShowStringTips("尚未灌满酒坛");
            return false;
        }
    }

    public bool OnClickToGetFreeBox1()
    {
        if (isCanGetBox1)
        {
            //Debug.Log("免费打开宝箱1");
            secondsNetTime_FreeBox1 = 0;
            isCanGetBox1 = false;
            PlayerPrefs.SetInt(fBoxOpenNeedTimes1, openNeedSeconds1);
            if (isGetNetworkTime)
            {
                long nowTimeStr = nowTimeLong;//long.Parse(PlayerPrefs.GetString(NetworkTimestampStr));
                nowTimeStr += (openNeedSeconds1 * 1000);
                openFreeBoxTimeLong1 = nowTimeStr;
                PlayerPrefs.SetString(freeBoxOpenTime1, nowTimeStr.ToString());
            }
            else
            {
                openFreeBoxTimeLong1 = 0;
                PlayerPrefs.SetString(freeBoxOpenTime1, "0");
            }
            return true;
        }
        else
        {
            //Debug.Log("宝箱免费开启时间未到");
            return false;
        }
    }

    public bool OnClickToGetFreeBox2()
    {
        if (isCanGetBox2)
        {
            //Debug.Log("免费打开宝箱2");
            secondsNetTime_FreeBox2 = 0;
            isCanGetBox2 = false;
            PlayerPrefs.SetInt(fBoxOpenNeedTimes2, openNeedSeconds2);
            if (isGetNetworkTime)
            {
                long nowTimeStr = nowTimeLong;//long.Parse(PlayerPrefs.GetString(NetworkTimestampStr));
                nowTimeStr += (openNeedSeconds2 * 1000);
                openFreeBoxTimeLong2 = nowTimeStr;
                PlayerPrefs.SetString(freeBoxOpenTime2, nowTimeStr.ToString());
            }
            else
            {
                openFreeBoxTimeLong2 = 0;
                PlayerPrefs.SetString(freeBoxOpenTime2, "0");
            }
            return true;
        }
        else
        {
            //Debug.Log("宝箱免费开启时间未到");
            return false;
        }
    }

    //开启锦囊
    public bool OnClickToGetJinNang()
    {
        if (isCanGetJN)
        {
            //Debug.Log("打开锦囊");
            secondsNetTime_JinNang = 0;
            isCanGetJN = false;
            PlayerPrefs.SetInt(jinNangOpenNeedTime, openJNNeedSeconds);
            if (isGetNetworkTime)
            {
                long nowTimeStr = nowTimeLong;
                nowTimeStr += (openJNNeedSeconds * 1000);
                openJinNangTimeLong = nowTimeStr;
                PlayerPrefs.SetString(jinNangOpenTime, nowTimeStr.ToString());
            }
            else
            {
                openJinNangTimeLong = 0;
                PlayerPrefs.SetString(jinNangOpenTime, "0");
            }
            return true;
        }
        else
        {
            //Debug.Log("宝箱免费开启时间未到");
            return false;
        }
    }

    //更新时间
    IEnumerator GetTime()
    {
        string timeStr = string.Empty;

        while (true)
        {
            //WWW www = new WWW(timeWebPath);
            WWW www = new WWW(timeWebPath0);
            yield return www;

            //Debug.Log(www.text);

            if (www.text == "" || www.text.Trim() == "")//如果断网
            {
                isGetNetworkTime = false;
            }
            else//成功获取网络时间
            {
                try
                {
                    //0=1600529802400
                    //timeStr = www.text.Substring(2); //获取网络准确时间戳
                    timeStr = www.text.Substring(81, 13); //获取网络准确时间戳

                    nowTimeLong = long.Parse(timeStr);

                    isGetNetworkTime = true;
                }
                catch (Exception e)
                {
                    isGetNetworkTime = false;
                }

            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    //获取当前时间
    public DateTime GetStrBackTime()
    {
        //return nowTimeLong;
        return startTime.AddMilliseconds(Convert.ToDouble(nowTimeLong));
    }

    /// <summary>
    /// 获取百度时间
    /// </summary>
    /// <returns></returns>
    private string GetNetDateTime()
    {
        WebRequest request = null;
        WebResponse response = null;
        WebHeaderCollection headerCollection = null;
        string datetime = string.Empty;
        try
        {
            request = WebRequest.Create("https://www.baidu.com");
            request.Timeout = 3000;
            request.Credentials = CredentialCache.DefaultCredentials;
            response = (WebResponse)request.GetResponse();
            headerCollection = response.Headers;
            foreach (var h in headerCollection.AllKeys)
            { if (h == "Date") { datetime = headerCollection[h]; } }
            return datetime;
        }
        catch (Exception) { return datetime; }
        finally
        {
            if (request != null)
            { request.Abort(); }
            if (response != null)
            { response.Close(); }
            if (headerCollection != null)
            { headerCollection.Clear(); }
        }
    }
    
    //获取网址内容
    private static string GetWebRequest(string getUrl)
    {
        string responseContent = "";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getUrl);
        request.ContentType = "application/json";
        request.Method = "GET";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //在这里对接收到的页面内容进行处理
        using (System.IO.Stream resStream = response.GetResponseStream())
        {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(resStream, Encoding.UTF8))
            {
                responseContent = reader.ReadToEnd().ToString();
            }
        }
        return responseContent;
    }
}