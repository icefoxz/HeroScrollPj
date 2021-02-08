using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class LoadSaveData : MonoBehaviour
{
    public static LoadSaveData instance;

    [HideInInspector]
    public string IsPlayMusicStr = "IsPlayMusicStr";    //PlayerPrefs 0静音1播放 
    [HideInInspector]
    public int firstForceId;    //传递记录初始势力 
    [HideInInspector]
    public bool isLoadingSaveData;  //标记是否在加载存档 
    [HideInInspector]
    public bool isHadSaveData;  //是否有存档 

    int isEncrypted = 0;    //记录是否加密过 
    private static readonly string ISNEEDENCRYPT = "IsNeedEncrypt";

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
        isLoadingSaveData = true;
        isHadSaveData = IsSaveFilesExist();
        if(AudioController0.instance)
        {
            if (isHadSaveData)
            {
                AudioController0.instance.isPlayMusic = PlayerPrefs.GetInt(IsPlayMusicStr); //游戏音乐开关标签 
            }
            else
            {
                PlayerPrefs.SetInt(IsPlayMusicStr, 1);
                AudioController0.instance.isPlayMusic = 1;
            }
        }

        //打印log附加代码 
        Application.logMessageReceived += AppDebugClass.LogForUnityLog;

        isEncrypted = PlayerPrefs.GetInt(ISNEEDENCRYPT);

        AppDebugClass.DeleteOldAppLog();   //删除原先DebugFile 
    }

    /// <summary> 
    /// 是否已有完整存档 
    /// </summary> 
    /// <returns></returns> 
    private bool IsSaveFilesExist()
    {
        string filePath = AppDebugClass.playerDataString;
        string filePath00 = AppDebugClass.pyDataString;

        string filePath0 = AppDebugClass.plyDataString;
        string filePath1 = AppDebugClass.hstDataString;
        string filePath2 = AppDebugClass.warUnlockDataString;
        return (File.Exists(filePath0) || File.Exists(filePath00) || File.Exists(filePath)) && File.Exists(filePath1) && File.Exists(filePath2);
    }

    /// <summary> 
    /// 删除所有存档 
    /// </summary> 
    public void DeleteAllSaveData()
    {
        Debug.Log("删除存档");

        string filePath = AppDebugClass.plyDataString;
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        filePath = AppDebugClass.hstDataString;
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        filePath = AppDebugClass.warUnlockDataString;
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        filePath = AppDebugClass.gbocDataString;
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
#if UNITY_EDITOR
        throw new Exception("删除完成，请重启游戏！");
#endif
        SceneManager.LoadScene(0);
        Destroy(LoadJsonFile.instance.gameObject);
        Destroy(PlayerDataForGame.instance.gameObject);
        Destroy(LoadSaveData.instance.gameObject);
    }

    /// <summary> 
    /// 存档json 
    /// </summary> 
    private void SaveByJson(PlayerData save)
    {
        if (save.Exp <= 0 &&
            save.ForceId <= 0 &&
            save.DailyJinNangRedemptionCount <= 0 &&
            save.DailyJiuTanRedemptionCount <= 0 &&
            save.LastJinNangRedeemTime <= 0 &&
            save.LastJiuTanRedeemTime <= 0 &&
            save.Level <= 1 &&
            save.YuanBao <= 0 &&
            save.YvQue <= 0)
        {
            XDebug.LogError<LoadSaveData>("存档数据异常");
        }

        isLoadingSaveData = true;
        try
        {
            File.WriteAllText(AppDebugClass.plyDataString, EncryptDecipherTool.DESEncrypt(JsonConvert.SerializeObject(save)));
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
        isLoadingSaveData = false;
    }
    private void SaveByJson(HSTDataClass save)
    {
        isLoadingSaveData = true;
        try
        {
            File.WriteAllText(AppDebugClass.hstDataString, EncryptDecipherTool.DESEncrypt(JsonConvert.SerializeObject(save)));
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
        isLoadingSaveData = false;
    }
    private void SaveByJson(WarsDataClass save)
    {
        isLoadingSaveData = true;
        try
        {
            File.WriteAllText(AppDebugClass.warUnlockDataString, EncryptDecipherTool.DESEncrypt(JsonConvert.SerializeObject(save)));
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
        isLoadingSaveData = false;
    }
    private void SaveByJson(GetBoxOrCodeData save)
    {
        isLoadingSaveData = true;
        try
        {
            File.WriteAllText(AppDebugClass.gbocDataString, EncryptDecipherTool.DESEncrypt(JsonConvert.SerializeObject(save)));
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
        isLoadingSaveData = false;
    }

    /// <summary> 
    /// 备份存档,传入json字符 
    /// </summary> 
    /// <param name="pyDataStr"></param> 
    /// <param name="hSTDataStr"></param> 
    /// <param name="warsDataStr"></param> 
    private void BackupArchiveForGame(string pyDataStr, string hSTDataStr, string warsDataStr, string gbocDataStr)
    {
        //string plyDataString1 = Application.dataPath + "/StreamingAssets/data.json"; 
        //string gbocDataString1 = Application.dataPath + "/StreamingAssets/data2.json"; 
        //string hstDataString1 = Application.dataPath + "/StreamingAssets/data3.json"; 
        //string warUnlockDataString1 = Application.dataPath + "/StreamingAssets/data4.json"; 
        //File.WriteAllText(plyDataString1, pyDataStr); 
        //File.WriteAllText(gbocDataString1, hSTDataStr); 
        //File.WriteAllText(hstDataString1, warsDataStr); 
        //File.WriteAllText(warUnlockDataString1, gbocDataStr); 

        string filePath0 = AppDebugClass.plyDataString1;
        string filePath1 = AppDebugClass.hstDataString1;
        string filePath2 = AppDebugClass.warUnlockDataString1;
        string filePath3 = AppDebugClass.gbocDataString1;

        try
        {
            File.WriteAllText(filePath0, EncryptDecipherTool.DESEncrypt(pyDataStr));
            File.WriteAllText(filePath1, EncryptDecipherTool.DESEncrypt(hSTDataStr));
            File.WriteAllText(filePath2, EncryptDecipherTool.DESEncrypt(warsDataStr));
            File.WriteAllText(filePath3, EncryptDecipherTool.DESEncrypt(gbocDataStr));

            Debug.Log("存档备份成功");
        }
        catch (System.Exception e)
        {
            Debug.LogError("存档备份失败 " + e.ToString());
        }

        //if (PlayerDataForGame.instance.acData.Phone != "") 
        //{ 
        //    UploadArchiveToServer(pyDataStr, hSTDataStr, warsDataStr, gbocDataStr); 
        //} 
    }
    /// <summary> 
    /// 读档json 
    /// </summary> 
    public void LoadByJson()
    {
        isLoadingSaveData = true;

        if (isHadSaveData)
        {
            if (!PlayerDataForGame.instance.isHadNewSaveData)
            {
                string filePath = AppDebugClass.playerDataString;
                string filePath00 = AppDebugClass.pyDataString;
                ObsoletedPlayerData save = new ObsoletedPlayerData();
                ObsoletedPyData save00 = new ObsoletedPyData();
                string jsonStr = string.Empty;

                string filePath0 = AppDebugClass.plyDataString;
                string filePath1 = AppDebugClass.hstDataString;
                string filePath2 = AppDebugClass.warUnlockDataString;
                string filePath3 = AppDebugClass.gbocDataString;

                PlayerData save0 = new PlayerData();
                HSTDataClass save1 = new HSTDataClass();
                WarsDataClass save2 = new WarsDataClass();
                GetBoxOrCodeData save3 = new GetBoxOrCodeData();

                string jsonStr0 = string.Empty;
                string jsonStr1 = string.Empty;
                string jsonStr2 = string.Empty;
                string jsonStr3 = string.Empty;

                try
                {
                    //玩家数据读档 
                    if (File.Exists(filePath) || File.Exists(filePath00))  //存在老玩家数据存档的话 
                    {
                        if (File.Exists(filePath))
                        {
                            jsonStr = File.ReadAllText(filePath);
                            jsonStr = InspectionAndCorrectionString(jsonStr, new string[] { "true}]}", "false}]}" }, filePath);
                            save = JsonConvert.DeserializeObject<ObsoletedPlayerData>(jsonStr);

                            File.Delete(filePath);
                            if (File.Exists(AppDebugClass.playerDataString1))
                            {
                                File.Delete(AppDebugClass.playerDataString1);
                            }
                            {
                                save0.Level = save.level;
                                save0.Exp = save.exp;
                                save0.YuanBao = save.yuanbao;
                                save0.YvQue = save.yvque;
                                save0.ForceId = save.forceId;
                                SaveByJson(save0);
                            }
                            {
                                save3.fightBoxs = save.fightBoxs;
                                save3.redemptionCodeGotList = save.redemptionCodeGotList;
                                SaveByJson(save3);
                            }
                        }
                        if (File.Exists(filePath00))
                        {
                            jsonStr = File.ReadAllText(filePath00);
                            jsonStr = InspectionAndCorrectionString(jsonStr, new string[] { "true}]}", "false}]}" }, filePath00);
                            save00 = JsonConvert.DeserializeObject<ObsoletedPyData>(jsonStr);

                            File.Delete(filePath00);
                            if (File.Exists(AppDebugClass.pyDataString1))
                            {
                                File.Delete(AppDebugClass.pyDataString1);
                            }
                            {
                                save0.Level = save00.level;
                                save0.Exp = save00.exp;
                                save0.YuanBao = save00.yuanbao;
                                save0.YvQue = save00.yvque;
                                save0.ForceId = save00.forceId;
                                SaveByJson(save0);
                            }
                            {
                                save3.fightBoxs = save00.fightBoxs;
                                save3.redemptionCodeGotList = save00.redemptionCodeGotList;
                                SaveByJson(save3);
                            }
                        }

                        Debug.Log("读老版玩家档成功，删除并建立新档成功");

                        jsonStr0 = File.ReadAllText(filePath0);
                        jsonStr3 = File.ReadAllText(filePath3);
                    }
                    else
                    {
                        jsonStr0 = File.ReadAllText(filePath0);                 //读取文件 
                        if (isEncrypted != 0)
                            jsonStr0 = EncryptDecipherTool.DESDecrypt(jsonStr0);    //解密json 
                        jsonStr0 = InspectionAndCorrectionString(jsonStr0, new string[] { "}" }, filePath0);    //修正json 
                        save0 = JsonConvert.DeserializeObject<PlayerData>(jsonStr0);                          //解析json 

                        jsonStr3 = File.ReadAllText(filePath3);
                        if (isEncrypted != 0)
                            jsonStr3 = EncryptDecipherTool.DESDecrypt(jsonStr3);
                        jsonStr3 = InspectionAndCorrectionString(jsonStr3, new string[] { "}]}" }, filePath3);
                        save3 = ArchiveCorrection(JsonConvert.DeserializeObject<GetBoxOrCodeData>(jsonStr3));
                    }

                    //武将士兵塔数据读档 
                    jsonStr1 = File.ReadAllText(filePath1);
                    if (isEncrypted != 0)
                        jsonStr1 = EncryptDecipherTool.DESDecrypt(jsonStr1);
                    jsonStr1 = InspectionAndCorrectionString(jsonStr1, new string[] { ":0}]}" }, filePath1);
                    save1 = ArchiveCorrection(JsonConvert.DeserializeObject<HSTDataClass>(jsonStr1));

                    //关卡解锁进度数据读档 
                    jsonStr2 = File.ReadAllText(filePath2);
                    if (isEncrypted != 0)
                        jsonStr2 = EncryptDecipherTool.DESDecrypt(jsonStr2);
                    jsonStr2 = InspectionAndCorrectionString(jsonStr2, new string[] { "true}]}", "false}]}" }, filePath2);
                    save2 = ArchiveCorrection(JsonConvert.DeserializeObject<WarsDataClass>(jsonStr2));
                    if (save2.baYe == null) save2.baYe = new BaYeDataClass();
                    Debug.Log("读档成功");

                    //备份存档 
                    BackupArchiveForGame(jsonStr0, jsonStr1, jsonStr2, jsonStr3);

                    if (isEncrypted == 0)
                    {
                        File.WriteAllText(filePath0, EncryptDecipherTool.DESEncrypt(jsonStr0));
                        File.WriteAllText(filePath1, EncryptDecipherTool.DESEncrypt(jsonStr1));
                        File.WriteAllText(filePath2, EncryptDecipherTool.DESEncrypt(jsonStr2));
                        File.WriteAllText(filePath3, EncryptDecipherTool.DESEncrypt(jsonStr3));
                        isEncrypted = 1;
                        PlayerPrefs.SetInt(ISNEEDENCRYPT, isEncrypted);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("读档失败 " + e.ToString());

                    filePath0 = AppDebugClass.plyDataString1;
                    filePath1 = AppDebugClass.hstDataString1;
                    filePath2 = AppDebugClass.warUnlockDataString1;
                    filePath3 = AppDebugClass.gbocDataString1;

                    //尝试获取备份存档 
                    try
                    {
                        if (File.Exists(AppDebugClass.playerDataString1))
                        {
                            jsonStr = File.ReadAllText(AppDebugClass.playerDataString1);
                            jsonStr = InspectionAndCorrectionString(jsonStr, new string[] { "true}]}", "false}]}" }, AppDebugClass.playerDataString1);
                            save = JsonConvert.DeserializeObject<ObsoletedPlayerData>(jsonStr);
                            {
                                save0.Level = save.level;
                                save0.Exp = save.exp;
                                save0.YuanBao = save.yuanbao;
                                save0.YvQue = save.yvque;
                                save0.ForceId = save.forceId;
                                SaveByJson(save0);
                            }
                            {
                                save3.fightBoxs = save.fightBoxs;
                                save3.redemptionCodeGotList = save.redemptionCodeGotList;
                                SaveByJson(save3);
                            }
                            File.Delete(AppDebugClass.playerDataString1);
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }
                        else
                        {
                            if (File.Exists(AppDebugClass.pyDataString1))
                            {
                                jsonStr = File.ReadAllText(AppDebugClass.pyDataString1);
                                jsonStr = InspectionAndCorrectionString(jsonStr, new string[] { "true}]}", "false}]}" }, AppDebugClass.pyDataString1);
                                save00 = JsonConvert.DeserializeObject<ObsoletedPyData>(jsonStr);
                                {
                                    save0.Level = save00.level;
                                    save0.Exp = save00.exp;
                                    save0.YuanBao = save00.yuanbao;
                                    save0.YvQue = save00.yvque;
                                    save0.ForceId = save00.forceId;
                                    SaveByJson(save0);
                                }
                                {
                                    save3.fightBoxs = save00.fightBoxs;
                                    save3.redemptionCodeGotList = save00.redemptionCodeGotList;
                                    SaveByJson(save3);
                                }
                                File.Delete(AppDebugClass.pyDataString1);
                                if (File.Exists(filePath00))
                                {
                                    File.Delete(filePath00);
                                }
                            }
                            else
                            {
                                jsonStr0 = File.ReadAllText(filePath0);
                                if (isEncrypted != 0)
                                    jsonStr0 = EncryptDecipherTool.DESDecrypt(jsonStr0);
                                save0 = JsonConvert.DeserializeObject<PlayerData>(jsonStr0);

                                jsonStr3 = File.ReadAllText(filePath3);
                                if (isEncrypted != 0)
                                    jsonStr3 = EncryptDecipherTool.DESDecrypt(jsonStr3);
                                save3 = ArchiveCorrection(JsonConvert.DeserializeObject<GetBoxOrCodeData>(jsonStr3));
                            }
                        }
                        jsonStr1 = File.ReadAllText(filePath1);
                        if (isEncrypted != 0)
                            jsonStr1 = EncryptDecipherTool.DESDecrypt(jsonStr1);
                        jsonStr1 = InspectionAndCorrectionString(jsonStr1, new string[] { ":0}]}" }, filePath1);
                        save1 = ArchiveCorrection(JsonConvert.DeserializeObject<HSTDataClass>(jsonStr1));

                        jsonStr2 = File.ReadAllText(filePath2);
                        if (isEncrypted != 0)
                            jsonStr2 = EncryptDecipherTool.DESDecrypt(jsonStr2);
                        jsonStr2 = InspectionAndCorrectionString(jsonStr2, new string[] { "true}]}", "false}]}" }, filePath2);
                        save2 = ArchiveCorrection(JsonConvert.DeserializeObject<WarsDataClass>(jsonStr2));

                        Debug.Log("读取备份存档成功");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("备份存档损坏 " + ex.ToString());
                    }
                }

                //存档数据提取到游戏中 
                SetGamePlayerBasicData(save0, save1, save2, save3);
            }
            //初始化新手指引 
            InitGuideData(false);
        }
        else
        {
            CreatePlayerDataSave();

            LoadByJson();
        }
        //设置游戏音乐开关标签 
        if(AudioController0.instance)
        {
            AudioController0.instance.isPlayMusic = PlayerPrefs.GetInt(IsPlayMusicStr);
        }

        isLoadingSaveData = false;
    }

    //武将塔等数据存档修正 
    private HSTDataClass ArchiveCorrection(HSTDataClass save)
    {
        bool isNeedSaveDate = true;

        //武将卡牌数据 
        int nowDataCount = save.heroSaveData.Count;
        int jsonDataCount = LoadJsonFile.heroTableDatas.Count;
        List<NowLevelAndHadChip> heroSaveData = new List<NowLevelAndHadChip>();
        for (int i = 0; i < jsonDataCount; i++)
        {
            if (LoadJsonFile.heroTableDatas[i][21] != "0")
            {
                NowLevelAndHadChip nlahc = new NowLevelAndHadChip();
                nlahc.id = i;
                nlahc.level = 0;
                nlahc.chips = 0;
                nlahc.isFight = 0;
                nlahc.typeIndex = 0;
                nlahc.isHad = false;
                nlahc.maxLevel = 0;
                heroSaveData.Add(nlahc);
            }
        }
        for (int i = 0; i < save.heroSaveData.Count; i++)
        {
            //判断是否投放 
            if (LoadJsonFile.heroTableDatas[save.heroSaveData[i].id][21] != "0")
            {
                for (int j = 0; j < heroSaveData.Count; j++)
                {
                    if (heroSaveData[j].id == save.heroSaveData[i].id)
                    {
                        heroSaveData[j] = save.heroSaveData[i];
                    }
                }
            }
        }
        save.heroSaveData = heroSaveData;

        ////添加新投放卡牌 
        //if (nowDataCount < jsonDataCount) 
        //{ 
        //    isNeedSaveDate = true; 
        //    for (int i = nowDataCount; i < jsonDataCount; i++) 
        //    { 
        //        NowLevelAndHadChip nlahc = new NowLevelAndHadChip(); 
        //        nlahc.id = i; 
        //        nlahc.Level = 0; 
        //        nlahc.chips = 0; 
        //        nlahc.isFight = 0; 
        //        nlahc.typeIndex = 0; 
        //        nlahc.isHad = false; 
        //        nlahc.maxLevel = 0; 
        //        save.heroSaveData.Add(nlahc); 
        //    } 
        //} 

        //塔卡牌数据 
        nowDataCount = save.towerSaveData.Count;
        jsonDataCount = LoadJsonFile.towerTableDatas.Count;

        List<NowLevelAndHadChip> towerSaveData = new List<NowLevelAndHadChip>();
        for (int i = 0; i < jsonDataCount; i++)
        {
            if (LoadJsonFile.towerTableDatas[i][14] != "0")
            {
                NowLevelAndHadChip nlahc = new NowLevelAndHadChip();
                nlahc.id = i;
                nlahc.level = 0;
                nlahc.chips = 0;
                nlahc.isFight = 0;
                nlahc.typeIndex = 2;
                nlahc.isHad = false;
                nlahc.maxLevel = 0;
                towerSaveData.Add(nlahc);
            }
        }
        for (int i = 0; i < save.towerSaveData.Count; i++)
        {
            //判断是否投放 
            if (LoadJsonFile.towerTableDatas[save.towerSaveData[i].id][14] != "0")
            {
                for (int j = 0; j < towerSaveData.Count; j++)
                {
                    if (towerSaveData[j].id == save.towerSaveData[i].id)
                    {
                        towerSaveData[j] = save.towerSaveData[i];
                    }
                }
            }
        }
        save.towerSaveData = towerSaveData;

        //if (nowDataCount < jsonDataCount) 
        //{ 
        //    isNeedSaveDate = true; 
        //    for (int i = nowDataCount; i < jsonDataCount; i++) 
        //    { 
        //        NowLevelAndHadChip nlahc = new NowLevelAndHadChip(); 
        //        nlahc.id = i; 
        //        nlahc.Level = 0; 
        //        nlahc.chips = 0; 
        //        nlahc.isFight = 0; 
        //        nlahc.typeIndex = 2; 
        //        nlahc.isHad = false; 
        //        nlahc.maxLevel = 0; 
        //        save.towerSaveData.Add(nlahc); 
        //    } 
        //} 

        //陷阱卡牌数据 
        nowDataCount = save.trapSaveData.Count;
        jsonDataCount = LoadJsonFile.trapTableDatas.Count;

        List<NowLevelAndHadChip> trapSaveData = new List<NowLevelAndHadChip>();
        for (int i = 0; i < jsonDataCount; i++)
        {
            if (LoadJsonFile.trapTableDatas[i][13] != "0")
            {
                NowLevelAndHadChip nlahc = new NowLevelAndHadChip();
                nlahc.id = i;
                nlahc.level = 0;
                nlahc.chips = 0;
                nlahc.isFight = 0;
                nlahc.typeIndex = 2;
                nlahc.isHad = false;
                nlahc.maxLevel = 0;
                trapSaveData.Add(nlahc);
            }
        }
        for (int i = 0; i < save.trapSaveData.Count; i++)
        {
            //判断是否投放 
            if (LoadJsonFile.trapTableDatas[save.trapSaveData[i].id][13] != "0")
            {
                for (int j = 0; j < trapSaveData.Count; j++)
                {
                    if (trapSaveData[j].id == save.trapSaveData[i].id)
                    {
                        trapSaveData[j] = save.trapSaveData[i];
                    }
                }
            }
        }
        save.trapSaveData = trapSaveData;

        //if (nowDataCount < jsonDataCount) 
        //{ 
        //    isNeedSaveDate = true; 
        //    for (int i = nowDataCount; i < jsonDataCount; i++) 
        //    { 
        //        NowLevelAndHadChip nlahc = new NowLevelAndHadChip(); 
        //        nlahc.id = i; 
        //        nlahc.Level = 0; 
        //        nlahc.chips = 0; 
        //        nlahc.isFight = 0; 
        //        nlahc.typeIndex = 3; 
        //        nlahc.isHad = false; 
        //        nlahc.maxLevel = 0; 
        //        save.trapSaveData.Add(nlahc); 
        //    } 
        //} 
        //技能卡牌数据 
        //nowDataCount = save.spellSaveData.Count; 
        //jsonDataCount = LoadJsonFile.spellTableDatas.Count; 
        //if (nowDataCount < jsonDataCount) 
        //{ 
        //    isNeedSaveDate = true; 
        //    for (int i = nowDataCount; i < jsonDataCount; i++) 
        //    { 
        //        NowLevelAndHadChip nlahc = new NowLevelAndHadChip(); 
        //        nlahc.id = i; 
        //        nlahc.Level = 0; 
        //        nlahc.chips = 0; 
        //        nlahc.isFight = 0; 
        //        nlahc.typeIndex = 4; 
        //        nlahc.isHad = false; 
        //        nlahc.maxLevel = 0; 
        //        save.spellSaveData.Add(nlahc); 
        //    } 
        //} 
        if (isNeedSaveDate)
        {
            SaveByJson(save);
        }
        return save;
    }
    //战役数据存档修正 
    private WarsDataClass ArchiveCorrection(WarsDataClass save)
    {
        bool isNeedSaveDate = false;
        //战役存档修正 
        int nowDataCount = save.warUnlockSaveData.Count;
        int jsonDataCount = LoadJsonFile.warTableDatas.Count;
        if (nowDataCount < jsonDataCount)
        {
            isNeedSaveDate = true;
            for (int i = nowDataCount; i < jsonDataCount; i++)
            {
                UnlockWarCount unlock = new UnlockWarCount();
                unlock.warId = i;
                unlock.unLockCount = 0;
                unlock.isTakeReward = false;
                save.warUnlockSaveData.Add(unlock);
            }
        }
        if (isNeedSaveDate)
        {
            SaveByJson(save);
            //Debug.Log("战役数据存档修正"); 
        }
        return save;
    }
    //玩家宝箱与兑换码存档修正 
    private GetBoxOrCodeData ArchiveCorrection(GetBoxOrCodeData save)
    {
        bool isNeedSaveDate = false;
        //兑换码存档 
        int nowDataCount = save.redemptionCodeGotList.Count;
        int jsonDataCount = LoadJsonFile.rCodeTableDatas.Count;
        if (nowDataCount < jsonDataCount)
        {
            isNeedSaveDate = true;
            for (int i = nowDataCount; i < jsonDataCount; i++)
            {
                RedemptionCodeGot redemptionCodeGot = new RedemptionCodeGot();
                redemptionCodeGot.id = i;
                redemptionCodeGot.isGot = false;
                save.redemptionCodeGotList.Add(redemptionCodeGot);
            }
        }
        if (isNeedSaveDate)
        {
            SaveByJson(save);
            //Debug.Log("玩家个人数据存档修正"); 
        }
        return save;
    }

    /// <summary> 
    /// 游戏数据初次存档 
    /// </summary> 
    /// <returns></returns> 
    public void CreatePlayerDataSave()
    {
        //////////////////////////////////////////////////////////////////////////////////////// 
        PlayerData pySaveData = new PlayerData();
        pySaveData.Level = 1;
        pySaveData.Exp = 0;
        pySaveData.YvQue = int.Parse(LoadJsonFile.assetTableDatas[0].startValue);
        pySaveData.YuanBao = int.Parse(LoadJsonFile.assetTableDatas[1].startValue);
        pySaveData.ForceId = firstForceId;   //暂给初始势力
        pySaveData.LastGameVersion = float.Parse(Application.version);
        SaveByJson(pySaveData);
        //////////////////////////////////////////////////////////////////////////////////////// 
        GetBoxOrCodeData gbocSaveData = new GetBoxOrCodeData();
        gbocSaveData.fightBoxs = new List<int>();
        gbocSaveData.redemptionCodeGotList = new List<RedemptionCodeGot>();
        for (int i = 0; i < LoadJsonFile.rCodeTableDatas.Count; i++)
        {
            RedemptionCodeGot redemptionCodeGot = new RedemptionCodeGot();
            redemptionCodeGot.id = i;
            redemptionCodeGot.isGot = false;
            gbocSaveData.redemptionCodeGotList.Add(redemptionCodeGot);
        }
        SaveByJson(gbocSaveData);
        //////////////////////////////////////////////////////////////////////////////////////// 
        HSTDataClass hstSaveData = new HSTDataClass();
        hstSaveData.heroSaveData = new List<NowLevelAndHadChip>();
        for (int i = 0; i < LoadJsonFile.heroTableDatas.Count; i++)
        {
            NowLevelAndHadChip nlahc = new NowLevelAndHadChip();
            nlahc.id = i;
            nlahc.level = 0;
            nlahc.chips = 0;
            nlahc.isFight = 0;
            nlahc.typeIndex = 0;
            nlahc.isHad = false;
            nlahc.maxLevel = 0;
            hstSaveData.heroSaveData.Add(nlahc);
        }
        string[] strs0 = LoadJsonFile.playerInitialTableDatas[pySaveData.ForceId][2].Split(',');
        for (int i = 0; i < strs0.Length; i++)
        {
            if (strs0[i] != "")
            {
                hstSaveData.heroSaveData[int.Parse(strs0[i])].level = 1; //设置等级为1，初始武将 
                hstSaveData.heroSaveData[int.Parse(strs0[i])].isHad = true;
                hstSaveData.heroSaveData[int.Parse(strs0[i])].maxLevel = 1;
                if (i == 0 || i == 1)
                    hstSaveData.heroSaveData[int.Parse(strs0[i])].isFight = 1;
            }
        }
        hstSaveData.soldierSaveData = new List<NowLevelAndHadChip>();
        for (int i = 0; i < LoadJsonFile.soldierTableDatas.Count; i++)
        {
            NowLevelAndHadChip nlahc = new NowLevelAndHadChip();
            nlahc.id = i;
            nlahc.level = 0;
            nlahc.chips = 0;
            nlahc.isFight = 0;
            nlahc.typeIndex = 1;
            nlahc.isHad = false;
            nlahc.maxLevel = 0;
            hstSaveData.soldierSaveData.Add(nlahc);
        }
        hstSaveData.towerSaveData = new List<NowLevelAndHadChip>();
        for (int i = 0; i < LoadJsonFile.towerTableDatas.Count; i++)
        {
            NowLevelAndHadChip nlahc = new NowLevelAndHadChip();
            nlahc.id = i;
            nlahc.level = 0;
            nlahc.chips = 0;
            nlahc.isFight = 0;
            nlahc.typeIndex = 2;
            nlahc.isHad = false;
            nlahc.maxLevel = 0;
            hstSaveData.towerSaveData.Add(nlahc);
        }
        string[] strs2 = LoadJsonFile.playerInitialTableDatas[pySaveData.ForceId][4].Split(',');
        for (int i = 0; i < strs2.Length; i++)
        {
            if (strs2[i] != "")
            {
                hstSaveData.towerSaveData[int.Parse(strs2[i])].level = 1; //设置等级为1，初始塔 
                hstSaveData.towerSaveData[int.Parse(strs2[i])].isHad = true;
                hstSaveData.towerSaveData[int.Parse(strs2[i])].maxLevel = 1;
                if (i == 0)
                    hstSaveData.towerSaveData[int.Parse(strs2[i])].isFight = 1;
            }
        }
        hstSaveData.trapSaveData = new List<NowLevelAndHadChip>();
        for (int i = 0; i < LoadJsonFile.trapTableDatas.Count; i++)
        {
            NowLevelAndHadChip nlahc = new NowLevelAndHadChip();
            nlahc.id = i;
            nlahc.level = 0;
            nlahc.chips = 0;
            nlahc.isFight = 0;
            nlahc.typeIndex = 3;
            nlahc.isHad = false;
            nlahc.maxLevel = 0;
            hstSaveData.trapSaveData.Add(nlahc);
        }
        hstSaveData.spellSaveData = new List<NowLevelAndHadChip>();
        for (int i = 0; i < LoadJsonFile.spellTableDatas.Count; i++)
        {
            NowLevelAndHadChip nlahc = new NowLevelAndHadChip();
            nlahc.id = i;
            nlahc.level = 0;
            nlahc.chips = 0;
            nlahc.isFight = 0;
            nlahc.typeIndex = 4;
            nlahc.isHad = false;
            nlahc.maxLevel = 0;
            hstSaveData.spellSaveData.Add(nlahc);
        }
        SaveByJson(hstSaveData);
        /////////////////////////////////////////////////////////////////////////////////////////// 
        WarsDataClass warsSaveData = new WarsDataClass();
        warsSaveData.warUnlockSaveData = new List<UnlockWarCount>();
        for (int i = 0; i < LoadJsonFile.warTableDatas.Count; i++)
        {
            UnlockWarCount unlock = new UnlockWarCount();
            unlock.warId = i;
            unlock.unLockCount = 0;
            unlock.isTakeReward = false;
            warsSaveData.warUnlockSaveData.Add(unlock);
        }
        warsSaveData.baYe = new BaYeDataClass();
        SaveByJson(warsSaveData);
        //////////////////////////////////////////////////////////////////////////////////////////// 
        isEncrypted = 1;
        PlayerPrefs.SetInt(ISNEEDENCRYPT, isEncrypted);

        PlayerPrefs.SetInt(IsPlayMusicStr, 1);

        TimeSystemControl.instance.InitTimeRelatedData();

        InitGuideData(true);

        isHadSaveData = true;
        Debug.Log("初创存档成功");
    }

    /// <summary> 
    /// 初始化指引数据 
    /// </summary> 
    /// <param name="isFirst"></param> 
    private void InitGuideData(bool isFirst)
    {
        //是否是首次进游戏 
        if (isFirst)
        {
            PlayerPrefs.SetInt(StringForGuide.guideJinBaoXiang, 0);
            PlayerPrefs.SetInt(StringForGuide.guideZYBaoXiang, 0);
            PlayerPrefs.SetInt(StringForGuide.guideHeCheng, 0);
            PlayerPrefs.SetInt(StringForGuide.guideStartZY, 0);
            PlayerPrefs.SetInt(StringForGuide.guideStartGQ, 0);
            PlayerPrefs.SetInt(StringForGuide.guideCheckCardInfo, 0);
            PlayerPrefs.SetInt(StringForGuide.guideShengJIZG, 0);
        }
        else
        {
            PlayerDataForGame.instance.guideObjsShowed = new int[7];
            PlayerDataForGame.instance.guideObjsShowed[0] = PlayerPrefs.GetInt(StringForGuide.guideJinBaoXiang);
            PlayerDataForGame.instance.guideObjsShowed[1] = PlayerPrefs.GetInt(StringForGuide.guideZYBaoXiang);
            PlayerDataForGame.instance.guideObjsShowed[2] = PlayerPrefs.GetInt(StringForGuide.guideHeCheng);
            PlayerDataForGame.instance.guideObjsShowed[3] = PlayerPrefs.GetInt(StringForGuide.guideStartZY);
            PlayerDataForGame.instance.guideObjsShowed[4] = PlayerPrefs.GetInt(StringForGuide.guideStartGQ);
            PlayerDataForGame.instance.guideObjsShowed[5] = PlayerPrefs.GetInt(StringForGuide.guideCheckCardInfo);
            PlayerDataForGame.instance.guideObjsShowed[6] = PlayerPrefs.GetInt(StringForGuide.guideShengJIZG);
        }
    }

    /// <summary> 
    /// 提取存档数据到游戏中 
    /// </summary> 
    /// <param name="save">玩家数据存档</param> 
    /// <param name="save1">卡牌数据存档</param> 
    /// <param name="save2">战役数据存档</param> 
    public void SetGamePlayerBasicData(PlayerData save, HSTDataClass save1, WarsDataClass save2, GetBoxOrCodeData save3)
    {
        PlayerDataForGame.instance.pyData = save;
        PlayerDataForGame.instance.hstData = save1;
        PlayerDataForGame.instance.warsData = save2;
        PlayerDataForGame.instance.gbocData = save3;
        PlayerDataForGame.instance.isHadNewSaveData = true;
    }

    /// <summary> 
    /// 存储游戏 
    /// </summary> 
    /// <param name="indexFun">默认0：全部存储，1：存储pyData，2：存储hstData，3：存储warsData，4：存储gbocData</param> 
    public void SaveGameData(int indexFun = 0)
    {
        if (PlayerDataForGame.instance.warsData.warUnlockSaveData == null)
            throw new InvalidDataContractException(
                $"{nameof(LoadSaveData)}.{nameof(SaveGameData)}: warUnlockSaveData = null!");
        if (PlayerDataForGame.instance.isNeedSaveData)
        {
            switch (indexFun)
            {
                case 0:
                    SaveByJson(PlayerDataForGame.instance.pyData);
                    SaveByJson(PlayerDataForGame.instance.hstData);
                    SaveByJson(PlayerDataForGame.instance.warsData);
                    SaveByJson(PlayerDataForGame.instance.gbocData);
                    break;
                case 1:
                    SaveByJson(PlayerDataForGame.instance.pyData);
                    break;
                case 2:
                    SaveByJson(PlayerDataForGame.instance.hstData);
                    break;
                case 3:
                    SaveByJson(PlayerDataForGame.instance.warsData);
                    break;
                case 4:
                    SaveByJson(PlayerDataForGame.instance.gbocData);
                    break;
                default:
                    break;
            }
            PlayerDataForGame.instance.isNeedSaveData = false;
        }
    }

    //游戏切除和再次进入的回调 
    private void OnApplicationPause(bool focus)
    {
        if (PlayerDataForGame.instance.lastSenceIndex != 0)
        {
            if (focus)
            {
                //SaveAllGameData(); 
                //Debug.Log("切出游戏，OnApplicationPause(true)"); 
            }
            else
            {
                //Debug.Log("切出游戏再次进入，OnApplicationPause(false)"); 
            }
        }
    }

    //退出游戏的回调 
    private void OnApplicationQuit()
    {
        if (PlayerDataForGame.instance.lastSenceIndex != 0)
        {
            //SaveAllGameData(); 
            //Debug.Log("正常退出游戏,OnApplicationQuit()"); 
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            //Debug.Log("暂停状态后进入游戏内，OnApplicationFocus(true)"); 
        }
        else
        {
            //Debug.Log("切出游戏画面进入暂停状态，OnApplicationFocus(false)"); 
        }
    }

    /// <summary> 
    /// 检查与矫正字符串(修复末尾) 
    /// </summary> 
    /// <param name="jsonStr"></param> 
    /// <param name="endStr"></param> 
    /// <param name="fileName"></param> 
    /// <returns></returns> 
    private string InspectionAndCorrectionString(string jsonStr, string[] endStr, string fileName)
    {
        //Debug.Log(jsonStr); 
        try
        {
            for (int i = 0; i < endStr.Length; i++)
            {
                int lastIndex = jsonStr.IndexOf(endStr[i]);
                if (lastIndex != -1)
                {
                    lastIndex = lastIndex + endStr[i].Length;
                    if (lastIndex < jsonStr.Length)
                    {
                        jsonStr = jsonStr.Remove(lastIndex);
                        //Debug.Log(fileName + "：存档格式损坏修复"); 
                    }
                    break;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("fileName: " + e.ToString());
        }
        //Debug.Log(jsonStr); 
        return jsonStr;
    }
}