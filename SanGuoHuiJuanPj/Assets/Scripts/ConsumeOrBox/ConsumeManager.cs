using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeManager : MonoBehaviour
{
    public static ConsumeManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    /// <summary>
    /// 增加元宝
    /// </summary>
    /// <param name="value"></param>
    public void AddYuanBao(int value)
    {
        if (value < 0)
            XDebug.LogError<ConsumeManager>("数量不可以小于0");
        else if (value == 0) return;

        AddYuanBaoValue(value);
    }

    /// <summary>
    /// 减少元宝
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool DeductYuanBao(int value)
    {
        if (value < 0)
            XDebug.LogError<ConsumeManager>("数量不可以小于0");
        else if (value == 0) return true;

        if (value > PlayerDataForGame.instance.pyData.YuanBao)
        {
            PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(0));
            //Debug.Log("元宝不足");
            return false;
        }

        AddYuanBaoValue(-value);
        return true;
    }

    //增加或减除元宝
    private void AddYuanBaoValue(int value)
    {
        if (value > 0) PlayerDataForGame.instance.ShowStringTips(string.Format(DataTable.GetStringText(1), value));

        PlayerDataForGame.instance.pyData.YuanBao += value;
        UIManager.instance.yuanBaoNumText.text = PlayerDataForGame.instance.pyData.YuanBao.ToString();
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(1);
    }

    /// <summary>
    /// 增加玉阙
    /// </summary>
    /// <param name="value"></param>
    public void AddYuQue(int value)
    {
        if (value < 0)
        {
            XDebug.LogError<ConsumeManager>("数量不可以小于0");
            return;
        }

        if (value == 0) return;

        AddYvQueValue(value);
    }

    /// <summary>
    /// 减少玉阙
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool DeductYuQue(int value)
    {
        if (value < 0)
        {
            XDebug.LogError<ConsumeManager>("数量不可以小于0");
            return false;
        }

        if (value == 0)
        {
            return true;
        }

        if (value > PlayerDataForGame.instance.pyData.YvQue)
        {
            PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(2));
            return false;
        }

        AddYvQueValue(-value);
        return true;

    }

    //增加或减除玉阙
    private void AddYvQueValue(int value)
    {
        if (value > 0)
        {
            PlayerDataForGame.instance.ShowStringTips(string.Format(DataTable.GetStringText(3), value));
        }

        PlayerDataForGame.instance.pyData.YvQue += value;
        UIManager.instance.yvQueNumText.text = PlayerDataForGame.instance.pyData.YvQue.ToString();
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(1);
    }

}
