using System;
using UnityEngine;

public class AppDebugClass
{
#if UNITY_EDITOR
    
    private static readonly string logFileUrl = Application.dataPath + "/StreamingAssets/DebugFileV1.89.txt";
    private static readonly string logFileUrlOld = Application.dataPath + "/StreamingAssets/DebugFileV1.86.txt";

    //现有存档
    public static readonly string playerDataString = Application.dataPath + "/StreamingAssets/PlayerDataSave.json";
    public static readonly string pyDataString = Application.dataPath + "/StreamingAssets/PyDataSave.json";
    public static readonly string plyDataString = Application.dataPath + "/StreamingAssets/PlyDataSave.json";
    public static readonly string gbocDataString = Application.dataPath + "/StreamingAssets/GbocDataSave.json";
    public static readonly string hstDataString = Application.dataPath + "/StreamingAssets/HSTDataSave.json";
    public static readonly string warUnlockDataString = Application.dataPath + "/StreamingAssets/WarUnlockDataSave.json";

    //旧存档
    public static readonly string playerDataString1 = Application.dataPath + "/StreamingAssets/PlayerDataSave_old";
    public static readonly string pyDataString1 = Application.dataPath + "/StreamingAssets/PyDataSave_old.json";
    public static readonly string plyDataString1 = Application.dataPath + "/StreamingAssets/PlyDataSave_old";
    public static readonly string gbocDataString1 = Application.dataPath + "/StreamingAssets/GbocDataSave_old";
    public static readonly string hstDataString1 = Application.dataPath + "/StreamingAssets/HSTDataSave_old";
    public static readonly string warUnlockDataString1 = Application.dataPath + "/StreamingAssets/WarUnlockDataSave_old";

#elif UNITY_ANDROID  && !UNITY_EDITOR

    private static readonly string logFileUrl = Application.persistentDataPath + "/DebugFileV1.89.txt";
    private static readonly string logFileUrlOld = Application.persistentDataPath + "/DebugFileV1.86.txt";

    public static readonly string playerDataString = Application.persistentDataPath + "/PlayerDataSave.json";
    public static readonly string pyDataString = Application.persistentDataPath + "/PyDataSave.json";
    public static readonly string plyDataString = Application.persistentDataPath + "/PlyDataSave.json";
    public static readonly string gbocDataString = Application.persistentDataPath + "/GbocDataSave.json";
    public static readonly string hstDataString = Application.persistentDataPath + "/HSTDataSave.json";
    public static readonly string warUnlockDataString = Application.persistentDataPath + "/WarUnlockDataSave.json";

    public static readonly string playerDataString1 = Application.persistentDataPath + "/PlayerDataSave_old";
    public static readonly string pyDataString1 = Application.persistentDataPath + "/PyDataSave_old.json";
    public static readonly string plyDataString1 = Application.persistentDataPath + "/PlyDataSave_old";
    public static readonly string gbocDataString1 = Application.persistentDataPath + "/GbocDataSave_old";
    public static readonly string hstDataString1 = Application.persistentDataPath + "/HSTDataSave_old";
    public static readonly string warUnlockDataString1 = Application.persistentDataPath + "/WarUnlockDataSave_old";

#endif

    private static bool enabledLogFile = true;

    /// <summary>
    /// UnityLog附加打印方法
    /// </summary>
    /// <param name="logContent">log内容</param>
    /// <param name="logScript">log具体代码位置</param>
    /// <param name="logType">log类型</param>
    public static void LogForUnityLog(string logContent, string logScript, LogType logType)
    {
        string logStr = GetNowTime();
        switch (logType)
        {
            case LogType.Error:
                logStr += "__AppDebugError__";
                logStr += (logContent + "\n");
                logStr += logScript;
                WriteFLogToFile(logStr);
                break;
            case LogType.Assert:
                logStr += "__AppDebugAssert__";
                logStr += (logContent + "\n");
                logStr += logScript;
                WriteFLogToFile(logStr);
                break;
            case LogType.Warning:
                //logStr += "__AppDebugWarning__";
                //logStr += (logContent + "\n");
                //logStr += logScript;
                //WriteFLogToFile(logStr);
                break;
            case LogType.Log:
                logStr += "__AppDebugLog__";
                logStr += (logContent + "\n");
                logStr += logScript;
                WriteFLogToFile(logStr);
                break;
            case LogType.Exception:
                logStr += "__AppDebugException__";
                logStr += (logContent + "\n");
                logStr += logScript;
                WriteFLogToFile(logStr);
                break;
        }
    }

    /// <summary>
    /// 删除老debug文件
    /// </summary>
    public static void DeleteOldAppLog()
    {
        if (System.IO.File.Exists(logFileUrlOld))
        {
            System.IO.File.Delete(logFileUrlOld);
        }
    }

    private static void WriteFLogToFile(string str)
    {
        if (enabledLogFile)
        {
            enabledLogFile = false;
            string logText = "";
            if (System.IO.File.Exists(logFileUrl))
            {
                logText = System.IO.File.ReadAllText(logFileUrl);
            }
            logText += ("\n" + str);
            System.IO.File.WriteAllText(logFileUrl, logText);
            enabledLogFile = true;
        }
    }

    //获取当前时间字符串
    private static string GetNowTime()
    {
        return DateTime.Now.ToString();
        // DateTime.Now.ToLocalTime().ToString(); // 2008-9-4 20:12:12
    }
}