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
    /// <param name="nums"></param>
    public void AddYuanBao(int nums)
    {
        if (nums > 0)
        {
            AddOrCutYuanBao(nums, true);
        }
    }

    /// <summary>
    /// 减少元宝
    /// </summary>
    /// <param name="nums"></param>
    /// <returns></returns>
    public bool CutYuanBao(int nums)
    {
        if (nums > PlayerDataForGame.instance.pyData.yuanbao)
        {
            PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(0));
            //Debug.Log("元宝不足");
            return false;
        }
        else
        {
            AddOrCutYuanBao(nums, false);
            return true;
        }
    }

    //增加或减除元宝
    private void AddOrCutYuanBao(int nums, bool isAdd)
    {
        if (isAdd)
        {
            PlayerDataForGame.instance.ShowStringTips(string.Format(LoadJsonFile.GetStringText(1), nums));
            PlayerDataForGame.instance.pyData.yuanbao += nums;
        }
        else
        {
            PlayerDataForGame.instance.pyData.yuanbao -= nums;
        }
        UIManager.instance.yuanBaoNumText.text = PlayerDataForGame.instance.pyData.yuanbao.ToString();
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(1);
    }

    /// <summary>
    /// 增加玉阙
    /// </summary>
    /// <param name="nums"></param>
    public void AddYuQue(int nums)
    {
        if (nums > 0)
        {
            AddOrCutYuQue(nums, true);
        }
    }

    /// <summary>
    /// 减少玉阙
    /// </summary>
    /// <param name="nums"></param>
    /// <returns></returns>
    public bool CutYuQue(int nums)
    {
        if (nums > PlayerDataForGame.instance.pyData.yvque)
        {
            PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(2));
            return false;
        }
        else
        {
            AddOrCutYuQue(nums, false);
            return true;
        }
    }

    //增加或减除玉阙
    private void AddOrCutYuQue(int nums, bool isAdd)
    {
        if (isAdd)
        {
            PlayerDataForGame.instance.ShowStringTips(string.Format(LoadJsonFile.GetStringText(3), nums));
            PlayerDataForGame.instance.pyData.yvque += nums;
        }
        else
        {
            PlayerDataForGame.instance.pyData.yvque -= nums;
        }
        UIManager.instance.yvQueNumText.text = PlayerDataForGame.instance.pyData.yvque.ToString();
        PlayerDataForGame.instance.isNeedSaveData = true;
        LoadSaveData.instance.SaveGameData(1);
    }

}
